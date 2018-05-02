// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils;
    using PowerShellGraphSDK;
    using PowerShellGraphSDK.PowerShellCmdlets;
    using Vipr.Core.CodeModel;
    using PS = System.Management.Automation;

    /// <summary>
    /// The behavior to convert an ODCM tree to a collection of resources.
    /// </summary>
    public static class NodeToResourceConversionBehavior
    {
        /// <summary>
        /// The contract for an ODCM type processor.
        /// </summary>
        /// <param name="currentType">The type that is currently being visited</param>
        private delegate void TypeProcessor(OdcmType currentType);

        /// <summary>
        /// Converts an OData route to a resource.
        /// </summary>
        /// <param name="node">The ODCM node which represents the OData route</param>
        /// <returns>The resource that was generated from the OData route.</returns>
        public static Resource ConvertToResource(this OdcmNode node, string pathPrefix = "")
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            // Calculate route
            ODataRoute oDataRoute = new ODataRoute(node);

            // Create file system path
            string fileSystemPath = $"{pathPrefix.Trim('\\')}\\{oDataRoute.ToRelativeFilePathString()}";

            // Create a resource
            Resource resource = new Resource(fileSystemPath);

            // Convert each ODCM property into a set of cmdlets
            IEnumerable<Cmdlet> cmdlets = oDataRoute.CreateCmdlets();

            // Add the cmdlets to the resource
            resource.AddAll(cmdlets);

            return resource;
        }

        /// <summary>
        /// Creates a set of cmdlets for an OData route.
        /// </summary>
        /// <param name="oDataRoute">The route to the resource (with ID placeholders)</param>
        /// <returns>The cmdlets.</returns>
        private static IEnumerable<Cmdlet> CreateCmdlets(this ODataRoute oDataRoute)
        {
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }

            // Get the ODCM property
            OdcmProperty property = oDataRoute.ResourceOdcmProperty;

            // Get/Search
            yield return oDataRoute.CreateGetCmdlet();

            // Post
            if (property.Projection.SupportsInsert())
            {
                yield return oDataRoute.CreatePostCmdlet();
            }

            // Patch
            if (property.Projection.SupportsUpdate())
            {
                yield return oDataRoute.CreatePatchCmdlet();
            }

            // Patch navigation property
            if (property.Projection.SupportsUpdateLink())
            {
                // TODO: Support PATCH on navigation properties
                //yield return oDataRoute.CreateNavigationPatchCmdlet();
            }

            // Delete
            if (property.Projection.SupportsDelete() && property.IsEnumeration())
            {
                yield return oDataRoute.CreateDeleteCmdlet();
            }

            // Delete navigation property
            if (property.Projection.SupportsDeleteLink())
            {
                // TODO: Support DELETE on navigation properties
                //yield return oDataRoute.CreateNavigationDeleteCmdlet();
            }

            // Actions and functions
            foreach (Cmdlet actionCmdlet in oDataRoute.CreateActionAndFunctionCmdlets())
            {
                yield return actionCmdlet;
            }
        }

        #region Operation Cmdlets

        private static Cmdlet CreateGetCmdlet(this ODataRoute oDataRoute)
        {
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }

            // Get the ODCM property for this resource
            OdcmProperty resource = oDataRoute.ResourceOdcmProperty;

            // Create the cmdlet
            Cmdlet cmdlet = new Cmdlet(new CmdletName(PS.VerbsCommon.Get, oDataRoute.ToCmdletNameNounString()))
            {
                ImpactLevel = PS.ConfirmImpact.None,
                Documentation = new CmdletDocumentation()
                {
                    Synopsis = resource.IsCollection
                        ? $"Gets '{resource.Type.FullName}' objects."
                        : $"Gets the '{resource.Name}'.",
                    Descriptions = new string[]
                    {
                        $"GET {oDataRoute.ToODataRouteString()}",
                        resource.IsCollection
                            ? $"Gets '{resource.Type.FullName}' objects in the '{resource.Name}' collection."
                            : $"Gets the '{resource.Name}' (which is of type '{resource.Type.FullName}').",
                        resource.Description,
                        resource.LongDescription,
                    },
                },
            };

            // Setup the parameters for the cmdlet
            CmdletParameter idParameter = cmdlet.SetupIdParametersAndCallUrl(oDataRoute, idParameterSetName: GetCmdlet.OperationName);
            if (idParameter != null)
            {
                // Since the resource has an optional ID parameter, use the "search" base type to handle cases where the ID isn't provided
                cmdlet.OperationType = CmdletOperationType.GetOrSearch;
                cmdlet.DefaultParameterSetName = GetOrSearchCmdlet.OperationName;
            }
            else
            {
                // This resource doesn't have an ID parameter, so use the basic "get" base type
                cmdlet.OperationType = CmdletOperationType.Get;
            }

            // Add the properties without marking them as PowerShell parameters to allow for auto-complete when picking columns for $select and $expand
            cmdlet.AddParametersForEntityProperties(resource.Type, null, false, false);

            return cmdlet;
        }

        private static Cmdlet CreatePostCmdlet(this ODataRoute oDataRoute)
        {
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }

            // Get the ODCM property for this resource
            OdcmProperty resource = oDataRoute.ResourceOdcmProperty;

            // Create the cmdlet
            Cmdlet cmdlet = new Cmdlet(new CmdletName(PS.VerbsCommon.New, oDataRoute.ToCmdletNameNounString()))
            {
                OperationType = CmdletOperationType.Post,
                ImpactLevel = PS.ConfirmImpact.Low,
                Documentation = new CmdletDocumentation()
                {
                    Synopsis = $"Creates a '{resource.Type.FullName}'.",
                    Descriptions = new string[]
                    {
                        $"POST {oDataRoute.ToODataRouteString()}",
                        resource.IsCollection
                            ? $"Creates a '{resource.Type.FullName}' in the '{resource.Name}' collection."
                            : $"Creates the '{resource.Name}' (which is of type '{resource.Type.FullName}').",
                        resource.Description,
                        resource.LongDescription,
                    },
                },
            };

            // Setup the parameters for the cmdlet
            cmdlet.SetupIdParametersAndCallUrl(oDataRoute, addEntityId: false, idValueFromPipeline: false);

            // Add properties of derived types as parameters to this cmdlet by traversing the tree of derived types
            cmdlet.AddParametersForEntityProperties(resource.Type, PostCmdlet.OperationName);

            return cmdlet;
        }

        private static Cmdlet CreatePatchCmdlet(this ODataRoute oDataRoute)
        {
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }

            // Get the ODCM property for this resource
            OdcmProperty resource = oDataRoute.ResourceOdcmProperty;

            // Create the cmdlet
            Cmdlet cmdlet = new Cmdlet(new CmdletName(PS.VerbsData.Update, oDataRoute.ToCmdletNameNounString()))
            {
                OperationType = CmdletOperationType.Patch,
                ImpactLevel = PS.ConfirmImpact.Medium,
                Documentation = new CmdletDocumentation()
                {
                    Synopsis = $"Updates a '{resource.Type.FullName}'.",
                    Descriptions = new string[]
                    {
                        $"PATCH {oDataRoute.ToODataRouteString()}",
                        resource.IsCollection
                            ? $"Updates a '{resource.Type.FullName}' in the '{resource.Name}' collection."
                            : $"Updates the '{resource.Name}' (which is of type '{resource.Type.FullName}').",
                        resource.Description,
                        resource.LongDescription,
                    },
                },
            };

            // Setup the parameters for the cmdlet
            cmdlet.SetupIdParametersAndCallUrl(oDataRoute, idValueFromPipeline: false);

            // Add properties of derived types as parameters to this cmdlet by traversing the tree of derived types
            cmdlet.AddParametersForEntityProperties(resource.Type, PatchCmdlet.OperationName);

            return cmdlet;
        }

        private static Cmdlet CreateDeleteCmdlet(this ODataRoute oDataRoute)
        {
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }

            // Get the ODCM property for this resource
            OdcmProperty resource = oDataRoute.ResourceOdcmProperty;

            // Create the cmdlet
            Cmdlet cmdlet = new Cmdlet(new CmdletName(PS.VerbsCommon.Remove, oDataRoute.ToCmdletNameNounString()))
            {
                OperationType = CmdletOperationType.Delete,
                ImpactLevel = PS.ConfirmImpact.High,
                Documentation = new CmdletDocumentation()
                {
                    Synopsis = $"Deletes a '{resource.Type.FullName}'.",
                    Descriptions = new string[]
                    {
                        $"DELETE {oDataRoute.ToODataRouteString()}",
                        resource.IsCollection
                            ? $"Deletes a '{resource.Type.FullName}' from the '{resource.Name}' collection."
                            : $"Deletes the '{resource.Name}' (which is of type '{resource.Type.FullName}').",
                        resource.Description,
                        resource.LongDescription,
                    },
                },
            };

            // Setup the parameters for the cmdlet
            cmdlet.SetupIdParametersAndCallUrl(oDataRoute);

            return cmdlet;
        }
        
        private static IEnumerable<Cmdlet> CreateActionAndFunctionCmdlets(this ODataRoute oDataRoute)
        {
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }

            // Get the ODCM property for this resource
            OdcmProperty resource = oDataRoute.ResourceOdcmProperty;

            // We can only create action/function cmdlets if the resource type is a class with methods
            if (resource.Type is OdcmClass resourceType)
            {
                foreach (OdcmMethod method in resourceType.Methods)
                {
                    // Create the cmdlet
                    Cmdlet cmdlet = new Cmdlet(new CmdletName(PS.VerbsLifecycle.Invoke, oDataRoute.ToCmdletNameNounString(method.Name)));

                    // Figure out if this method is a function or an action
                    string urlPostfixSegment = method.Name;
                    string methodType;
                    if (method.IsFunction)
                    {
                        methodType = "function";

                        // Since this is a function, it should not have any observable side-effects (according to the OData v4 spec)
                        cmdlet.ImpactLevel = PS.ConfirmImpact.None;

                        // The behavior of the function should be different based on whether it returns a collection or not
                        if (method.IsCollection)
                        {
                            cmdlet.DefaultParameterSetName = GetOrSearchCmdlet.OperationName;
                            cmdlet.OperationType = CmdletOperationType.FunctionReturningCollection;
                        }
                        else
                        {
                            cmdlet.DefaultParameterSetName = GetCmdlet.OperationName;
                            cmdlet.OperationType = CmdletOperationType.FunctionReturningEntity;
                        }

                        // Setup the function parameters
                        // TODO: create a parameter set per overload
                        cmdlet.AddFunctionParameters(method);

                        // Add the placeholders for the function arguments in the URL
                        string paramNamesArgumentsString = string.Join(
                            ",",
                            method.Parameters.Select(param =>
                            {
                                // Set the default placeholder for the parameter's value
                                string valuePlaceholder = $"{{{param.Name}}}";

                                // Get the parameter's type
                                Type paramType = param.Type.ToPowerShellType(param.IsCollection);

                                // Check if we need special handling of the value based on the parameter type
                                if (paramType == typeof(DateTime)
                                    || paramType == typeof(DateTimeOffset)
                                    || paramType == typeof(TimeSpan))
                                {
                                    // If the type should be converted to a string, call "ToString()" on it
                                    valuePlaceholder = $"{{{param.Name}.{nameof(Object.ToString)}()}}";
                                }
                                else if (paramType == typeof(string))
                                {
                                    // If the type is a basic string, surround it in single quotes
                                    valuePlaceholder = $"'{{{param.Name}}}'";
                                }
                                else if (!paramType.IsPrimitive)
                                {
                                    // If the type is complex, serialize it as JSON
                                    valuePlaceholder = $"{{{nameof(JsonUtils)}.{nameof(JsonUtils.WriteJson)}({param.Name})}}";
                                }

                                // Create the parameter mapping
                                return $"{param.Name}={valuePlaceholder}";
                            }));
                        urlPostfixSegment += $"({paramNamesArgumentsString})";
                    }
                    else
                    {
                        methodType = "action";

                        // Set the cmdlet up as a method
                        cmdlet.OperationType = CmdletOperationType.Action;
                        cmdlet.ImpactLevel = PS.ConfirmImpact.High;

                        // Setup the action parameters
                        cmdlet.AddActionParameters(method);
                    }

                    // Documentation
                    cmdlet.Documentation = new CmdletDocumentation()
                    {
                        Descriptions = new string[]
                        {
                            methodType == "action"
                                ? $"POST {oDataRoute.ToODataRouteString()}" // action
                                : $"GET {oDataRoute.ToODataRouteString()}", // function
                            $"The {methodType} '{method.FullName}', which exists on type '{resourceType.FullName}'",
                            method.ReturnType != null
                                ? method.IsCollection
                                    ? $"This {methodType} returns a collection of '{method.ReturnType?.FullName}'."
                                    : $"This {methodType} returns a '{method.ReturnType?.FullName}'."
                                : $"This {methodType} does not return any objects",
                            method.Description,
                        },
                    };

                    // Setup the ID parameters and call URL
                    CmdletParameter idParameter = cmdlet.SetupIdParametersAndCallUrl(
                        oDataRoute,
                        addEntityId: !method.IsBoundToCollection, // if the function is bound to a collection, we don't need an ID parameter
                        postfixUrlSegments: urlPostfixSegment);

                    // TODO: Add fake parameters for the properties in the result entities
                    //cmdlet.AddParametersForEntityProperties(method.ReturnType, null, false, false);

                    // Return the cmdlet representing the action or function
                    yield return cmdlet;
                }
            }
        }

        #endregion Operation Cmdlets

        #region Helpers

        /// <summary>
        /// Gets the parameter set with the given name if it exists on the cmdlet,
        /// otherwise creates a new parameter set and adds it to the cmdlet.
        /// </summary>
        /// <param name="cmdlet">The cmdlet</param>
        /// <param name="parameterSetName">The name of the parameter set</param>
        /// <returns>The parameter set that was either retrieved or created</returns>
        private static CmdletParameterSet GetOrCreateParameterSet(this Cmdlet cmdlet, string parameterSetName)
        {
            if (parameterSetName == null)
            {
                throw new ArgumentNullException(nameof(parameterSetName));
            }

            // Create the parameter set if it doesn't already exist
            CmdletParameterSet parameterSet = cmdlet.ParameterSets.Get(parameterSetName);
            if (parameterSet == null)
            {
                parameterSet = new CmdletParameterSet(parameterSetName);
                cmdlet.ParameterSets.Add(parameterSet);
            }

            return parameterSet;
        }

        /// <summary>
        /// Sets up the ID parameters for a cmdlet based on the resource.
        /// </summary>
        /// <param name="cmdlet">The cmdlet</param>
        /// <param name="oDataRoute">The OData route to the resource</param>
        /// <param name="addEntityId">Whether or not to add the entity ID (i.e. the final ID in the route)</param>
        /// <param name="idParameterSetName">
        /// The name of the parameter set to add the entity ID parameter to - if this is null, it will be added to the default parameter set
        /// and will be mandatory.
        /// </param>
        /// <param name="postfixUrlSegments">The URL segments to postfix if required</param>
        /// <returns>The ID parameter if it was required and added to the cmdlet, otherwise null</returns>
        private static CmdletParameter SetupIdParametersAndCallUrl(
            this Cmdlet cmdlet,
            ODataRoute oDataRoute,
            bool addEntityId = true,
            string idParameterSetName = null,
            bool idValueFromPipeline = true,
            params string[] postfixUrlSegments)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }

            // Create the OData route
            string oDataRouteString = oDataRoute.ToODataRouteString();

            // Check whether this route needs to have an ID parameter added to the end
            CmdletParameter idParameter = null;
            bool idParameterIsMandatory = idParameterSetName == null;
            if (addEntityId && oDataRoute.TryCreateEntityIdParameter(out idParameter, idParameterIsMandatory, idValueFromPipeline))
            {
                // Set the URL to use this parameter and add it to the appropriate parameter set
                cmdlet.CallUrl = oDataRouteString;
                if (idParameterIsMandatory)
                {
                    cmdlet.DefaultParameterSet.Add(idParameter);
                    cmdlet.CallUrl += $"/{{{idParameter.Name}}}";
                }
                else
                {
                    cmdlet.GetOrCreateParameterSet(idParameterSetName).Add(idParameter);
                    cmdlet.CallUrl += $"/{{{idParameter.Name} ?? string.Empty}}";
                }
            }
            else
            {
                cmdlet.CallUrl = oDataRouteString;
            }

            // Postfix the custom URL segments
            if (postfixUrlSegments.Any())
            {
                cmdlet.CallUrl += "/" + string.Join("/", postfixUrlSegments);
            }

            // Add parameters to represent the ID placeholders in the URL
            cmdlet.AddIdParametersForRoutePlaceholders(oDataRoute);

            return idParameter;
        }

        /// <summary>
        /// Creates an ID parameter if it is required by the resource.
        /// </summary>
        /// <param name="oDataRoute">The OData route to the resource</param>
        /// <param name="entityIdIsMandatory">Whether or not the ID parameter should be mandatory</param>
        /// <param name="idParameter">The resulting ID parameter if it was created, otherwise null</param>
        /// <returns>True if the ID parameter was created, otherwise false.</returns>
        private static bool TryCreateEntityIdParameter(
            this ODataRoute oDataRoute,
            out CmdletParameter idParameter,
            bool entityIdIsMandatory,
            bool valueFromPipeline)
        {
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }

            // Get the ODCM property for this resource
            OdcmProperty resource = oDataRoute.ResourceOdcmProperty;

            // We need the ID parameter only if the property is an enumeration
            if (resource.IsEnumeration())
            {
                // Create the ID parameter
                idParameter = new CmdletParameter(ODataConstants.RequestProperties.Id, typeof(string))
                {
                    Mandatory = entityIdIsMandatory,
                    ValueFromPipeline = valueFromPipeline,
                    ValueFromPipelineByPropertyName = true,
                    ValidateNotNullOrEmpty = true,
                    Documentation = new CmdletParameterDocumentation()
                    {
                        Descriptions = new string[]
                        {
                            $"The ID for a '{resource.Type.FullName}' in the '{resource.Name}' collection",
                        },
                    },
                };

                return true;
            }
            else
            {
                // Don't create the ID parameter
                idParameter = null;
                return false;
            }
        }

        /// <summary>
        /// Adds the parameters which correspond to the ID placeholders in the OData route.
        /// </summary>
        /// <param name="cmdlet">The cmdlet to add the ID parameters to</param>
        /// <param name="oDataRoute">The OData route to the resource</param>
        private static void AddIdParametersForRoutePlaceholders(this Cmdlet cmdlet, ODataRoute oDataRoute)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }

            // Get the ODCM property for this resource
            OdcmProperty resource = oDataRoute.ResourceOdcmProperty;

            // For each ID in the URL, add a parameter
            foreach (string idParameterName in oDataRoute.IdParameters)
            {
                CmdletParameter idParameter = new CmdletParameter(idParameterName, typeof(string))
                {
                    Mandatory = true,
                    ValidateNotNullOrEmpty = true,
                    Documentation = new CmdletParameterDocumentation()
                    {
                        Descriptions = new string[]
                        {
                            $"A required ID for referencing a '{resource.Type.FullName}' in the '{resource.Name}' collection",
                        },
                    },
                };
                cmdlet.DefaultParameterSet.Add(idParameter);
            }
        }

        /// <summary>
        /// Traverses the tree of a type and its subtypes. 
        /// </summary>
        /// <param name="baseType">The base type (i.e. root of the tree)</param>
        /// <param name="typeProcessor">The processor for handling each type</param>
        /// <remarks>
        /// Types are guaranteed to be processed before the types that derive from them.
        /// In other words, when a type is visited, it is guaranteed that all of its base types have been processed.
        /// </remarks>
        private static void VisitDerivedTypes(this OdcmType baseType, TypeProcessor typeProcessor)
        {
            if (baseType == null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }
            if (typeProcessor == null)
            {
                throw new ArgumentNullException(nameof(typeProcessor));
            }

            Stack<OdcmType> unvisited = new Stack<OdcmType>();
            unvisited.Push(baseType);
            while (unvisited.Any())
            {
                // Get the next type
                OdcmType type = unvisited.Pop();

                // Add derived types to the unvisited list
                foreach (OdcmType derivedType in type.GetDerivedTypes())
                {
                    unvisited.Push(derivedType);
                }

                // Process the type
                typeProcessor(type);
            }
        }

        /// <summary>
        /// Create the processor for adding parameters to a cmdlet.
        /// </summary>
        /// <param name="cmdlet">The cmdlet</param>
        /// <param name="baseType">The type for which we should add parameters</param>
        /// <param name="addSwitchParametersForAbstractTypes">Whether or not to create a switch parameter for abstract types</param>
        /// <param name="sharedParameterSetName">
        /// The name of the shared parameter set, or null if parameters shouldn't be added to the shared parameter set
        /// </param>
        private static void AddParametersForEntityProperties(
            this Cmdlet cmdlet,
            OdcmType baseType,
            string sharedParameterSetName = null,
            bool addSwitchParameters = true,
            bool markAsPowerShellParameter = true)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }
            if (baseType == null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }

            // Don't try to add parameters for Edm types
            if (baseType.Namespace.Name.StartsWith("Edm"))
            {
                return;
            }

            // Track parameters as we visit each type
            IDictionary<OdcmType, IEnumerable<string>> parameterNameLookup = new Dictionary<OdcmType, IEnumerable<string>>();
            IDictionary<string, CmdletParameter> parameterLookup = new Dictionary<string, CmdletParameter>();

            // Visit all derived types
            baseType.VisitDerivedTypes((OdcmType type) =>
            {
                string parameterName = type.Name;
                string parameterSetName = "#" + type.FullName;

                // Determine if this is the only entity type for this cmdlet
                bool isTheOnlyType = !(type != baseType || !type.GetDerivedTypes().Any());

                // Create the parameter set for this type if it doesn't already exist
                CmdletParameterSet parameterSet = cmdlet.GetOrCreateParameterSet(parameterSetName);

                // Add a switch parameter for this type if required
                if (addSwitchParameters
                    && !(type is OdcmClass @class && @class.IsAbstract) // don't add a switch for abstract types
                    && !isTheOnlyType) // if there is only 1 type, don't add a switch parameter for it
                {
                    // Add the switch parameter
                    parameterSet.Add(new CmdletParameter(parameterName, typeof(PS.SwitchParameter))
                    {
                        Mandatory = true,
                        ParameterSetSelectorName = parameterSetName,
                        ValueFromPipelineByPropertyName = false,
                        Documentation = new CmdletParameterDocumentation()
                        {
                            Descriptions = new string[]
                            {
                                $"A switch parameter for selecting the parameter set which corresponds to the '{type.FullName}' type.",
                            },
                        },
                    });
                }

                // Evaluate the properties on this type
                // TODO: Include collections and navigation properties as expandable, selectable and sortable
                IEnumerable<OdcmProperty> properties = type.EvaluateProperties(type == baseType)
                    .Where(prop => prop.Name != ODataConstants.RequestProperties.Id);

                // Add this type into the parmeter name lookup table
                parameterNameLookup.Add(type, properties
                    .Where(prop => !prop.ReadOnly && !prop.IsEnumeration() && !prop.IsLink)
                    .Select(prop => prop.Name)
                    .Distinct());

                // Add the base types' properties as parameters to this parameter set
                // NOTE: Safe lookups are not necessary since all base types are guaranteed to have already been processed
                // by the VisitDerivedTypes() method
                OdcmType currentType = type;
                while (currentType != baseType)
                {
                    // Get the next type
                    currentType = currentType.GetBaseType();

                    // Lookup the properties for this type
                    IEnumerable<string> parameterNames = parameterNameLookup[currentType];

                    // Lookup the CmdletParameter objects for each parameter name
                    IEnumerable<CmdletParameter> parameters = parameterNames.Select(paramName => parameterLookup[paramName]);

                    // Add the parameters to the parameter set
                    parameterSet.AddAll(parameters);
                }

                // Iterate over properties
                foreach (OdcmProperty property in properties)
                {
                    // Get the PowerShell type for this property from the Edm type
                    Type propertyType = property.Type.ToPowerShellType(property.IsCollection);

                    // Create the parameter for this property if it doesn't already exist
                    if (!parameterLookup.TryGetValue(property.Name, out CmdletParameter parameter))
                    {
                        // Get the valid values if it is an enum
                        IEnumerable<string> enumMembers = null;
                        if (markAsPowerShellParameter && property.Type is OdcmEnum @enum)
                        {
                            enumMembers = @enum.Members.Select(enumMember => enumMember.Name);
                        }

                        parameter = CreateEntityParameter(
                            property,
                            propertyType,
                            markAsPowerShellParameter,
                            type == baseType,
                            type.FullName,
                            enumMembers);
                        parameterLookup.Add(property.Name, parameter);
                    }
                    else if (propertyType != parameter.Type)
                    {
                        // If all uses of this parameter don't use the same type, default to System.Object
                        parameter = CreateEntityParameter(
                            property,
                            typeof(object),
                            markAsPowerShellParameter,
                            type == baseType,
                            type.FullName);
                        parameterLookup.Add(property.Name, parameter);
                    }

                    // Add this type's properties as parameters to this parameter set
                    parameterSet.Add(parameter);
                }
            });

            // Get/create the shared parameter set if required (e.g. the "Post" parameter set for the "Create" cmdlet)
            if (sharedParameterSetName != null)
            {
                // Create the parameter set if it doesn't already exist
                CmdletParameterSet sharedParameterSet = cmdlet.GetOrCreateParameterSet(sharedParameterSetName);

                // All properties should be part of the shared parameter set
                sharedParameterSet.AddAll(parameterLookup.Values);
            }
        }

        private static CmdletParameter CreateEntityParameter(
            OdcmProperty property,
            Type powerShellType,
            bool markAsPowerShellParameter,
            bool isBaseType,
            string entityTypeFullName,
            IEnumerable<string> enumValues = null)
        {
            var result = new CmdletParameter(property.Name, powerShellType)
            {
                Mandatory = property.IsRequired,
                ValueFromPipelineByPropertyName = false,
                IsPowerShellParameter = markAsPowerShellParameter && !property.ReadOnly,

                DerivedTypeName = markAsPowerShellParameter || isBaseType
                                ? null
                                : entityTypeFullName,
                IsExpandable = !markAsPowerShellParameter && property.IsLink,
                IsSortable = !markAsPowerShellParameter && !property.IsCollection,
                Documentation = new CmdletParameterDocumentation()
                {
                    Descriptions = new string[] {
                        $"The '{property.Name}' property, of type '{property.Type.FullName}'.",
                        $"This property is on the '{entityTypeFullName}' type.",
                        property.Description,
                    },
                    ValidValues = enumValues,
                }
            };

            return result;
        }

        private static void AddActionParameters(this Cmdlet cmdlet, OdcmMethod action)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            if (action.IsFunction)
            {
                throw new ArgumentException($"The given ODCM method '{action.Name}' does not represent an action - it represents a function", nameof(action));
            }

            // Iterate over each parameter
            foreach (OdcmParameter parameter in action.Parameters)
            {
                // Create the equivalent CmdletParameter object
                CmdletParameter cmdletParameter = new CmdletParameter(parameter.Name, parameter.Type.ToPowerShellType(parameter.IsCollection))
                {
                    Mandatory = true,
                    ValidateNotNull = !parameter.IsNullable,
                    Documentation = new CmdletParameterDocumentation()
                    {
                        Descriptions = new string[]
                        {
                            $"The '{parameter.Name}' parameter, which is accepted by the '{action.FullName}' action.",
                            parameter.Description,
                        },
                    }
                };

                // Setup valid values if the parameter type is an enum
                if (parameter.Type is OdcmEnum @enum)
                {
                    cmdletParameter.Documentation.ValidValues = @enum.Members.Select(enumMember => enumMember.Name);
                }

                // Add the CmdletParameter to the default parameter set
                cmdlet.DefaultParameterSet.Add(cmdletParameter);
            }
        }

        private static void AddFunctionParameters(this Cmdlet cmdlet, OdcmMethod function, string defaultParameterSetName = null)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }
            if (function == null)
            {
                throw new ArgumentNullException(nameof(function));
            }
            if (!function.IsFunction)
            {
                throw new ArgumentException($"The given ODCM method '{function.Name}' does not represent a function - it represents an action", nameof(function));
            }

            // Iterate over each parameter
            foreach (OdcmParameter parameter in function.Parameters)
            {
                // Create the equivalent CmdletParameter object
                Type powerShellType = parameter.Type.ToPowerShellType(parameter.IsCollection);
                CmdletParameter cmdletParameter = new CmdletParameter(parameter.Name, powerShellType)
                {
                    Mandatory = true,
                    ValidateNotNull = !parameter.IsNullable,
                    Documentation = new CmdletParameterDocumentation()
                    {
                        Descriptions = new string[]
                        {
                            $"The '{parameter.Name}' parameter, which is accepted by the '{function.FullName}' function.",
                            parameter.Description,
                        },
                    }
                };

                // Setup valid values if the parameter type is an enum
                if (parameter.Type is OdcmEnum @enum)
                {
                    cmdletParameter.Documentation.ValidValues = @enum.Members.Select(enumMember => enumMember.Name);
                }

                // Add the CmdletParameter to the specified parameter set
                if (defaultParameterSetName == null)
                {
                    cmdlet.DefaultParameterSet.Add(cmdletParameter);
                }
                else
                {
                    cmdlet.GetOrCreateParameterSet(defaultParameterSetName).Add(cmdletParameter);
                }
            }
        }

        private static Type ToPowerShellType(this OdcmType odcmType, bool isCollection = false)
        {
            if (odcmType == null)
            {
                throw new ArgumentNullException(nameof(odcmType));
            }

            // Convert the type (default to System.Object if we can't convert the type)
            Type result = odcmType.ToDotNetType();

            // Make it an array type if necessary
            if (isCollection)
            {
                result = result.MakeArrayType();
            }

            return result;
        }

        #endregion Helpers
    }
}
