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
        /// The name of the "ID" property's parameter.
        /// </summary>
        private const string IdParameterName = "id";

        /// <summary>
        /// The contract for an ODCM type processor.
        /// </summary>
        /// <param name="currentType">The type that is currently being visited</param>
        private delegate void TypeProcessor(OdcmType currentType);

        /// <summary>
        /// The mapping of Edm types to PowerShell types.  
        /// </summary>
        private static IDictionary<string, Type> EdmToPowerShellTypeMappings = new Dictionary<string, Type>()
        {
            { "Edm.Boolean", typeof(bool) },

            { "Edm.String", typeof(string) },

            { "Edm.Byte", typeof(byte) },
            { "Edm.Stream", typeof(byte[]) },

            { "Edm.Int16", typeof(short) },
            { "Edm.Int32", typeof(int) },
            { "Edm.Int64", typeof(long) },

            { "Edm.Single", typeof(float) },
            { "Edm.Double", typeof(double) },
            { "Edm.Decimal", typeof(decimal) },

            { "Edm.Guid", typeof(Guid) },

            { "Edm.DateTimeOffset", typeof(DateTime) },
            { "Edm.TimeOfDay", typeof(TimeSpan) },
            { "Edm.Duration", typeof(TimeSpan) },
        };

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
            OdcmProperty property = oDataRoute.ResourceOdcmProperty;

            // Create the cmdlet
            Cmdlet cmdlet = new Cmdlet(new CmdletName(PS.VerbsCommon.Get, oDataRoute.ToCmdletNameNounString()))
            {
                ImpactLevel = PS.ConfirmImpact.None,
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

            // Create the cmdlet
            Cmdlet cmdlet = new Cmdlet(new CmdletName(PS.VerbsCommon.Remove, oDataRoute.ToCmdletNameNounString()))
            {
                OperationType = CmdletOperationType.Delete,
                ImpactLevel = PS.ConfirmImpact.High,
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

            // We can only create action cmdlets if the resource type is a class with methods
            if (resource.Type is OdcmClass resourceType)
            {
                foreach (OdcmMethod method in resourceType.Methods)
                {
                    // Create the cmdlet
                    Cmdlet cmdlet = new Cmdlet(new CmdletName(PS.VerbsLifecycle.Invoke, oDataRoute.ToCmdletNameNounString(method.Name)));

                    // Figure out if this method is a function or an action
                    string urlPostfixSegment = method.Name;
                    if (method.IsFunction)
                    {
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
                        cmdlet.AddFunctionParameters(method);

                        // Add the placeholders for the function arguments in the URL
                        string paramNamesArgumentsString = string.Join(
                            ",",
                            method.Parameters.Select(param =>
                            {
                                // Set the default placeholder for the parameter's value
                                string valuePlaceholder = $"{{{param.Name}}}";

                                // Check if we need special handling of the value based on the parameter type
                                Type paramType = param.Type.ToPowerShellType(param.IsCollection);
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
                        // Set the cmdlet up as a method
                        cmdlet.OperationType = CmdletOperationType.Action;
                        cmdlet.ImpactLevel = PS.ConfirmImpact.High;

                        // Setup the action parameters
                        cmdlet.AddActionParameters(method);
                    }

                    // Setup the ID parameters and call URL
                    CmdletParameter idParameter = cmdlet.SetupIdParametersAndCallUrl(
                        oDataRoute,
                        addEntityId: !method.IsBoundToCollection, // if the function is bound to a collection, we don't need an ID parameter
                        postfixUrlSegments: urlPostfixSegment);

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
            OdcmProperty property = oDataRoute.ResourceOdcmProperty;

            // We need the ID parameter only if the property is an enumeration
            if (property.IsEnumeration())
            {
                // Create the ID parameter
                idParameter = new CmdletParameter(IdParameterName, typeof(string))
                {
                    Mandatory = entityIdIsMandatory,
                    ValueFromPipeline = valueFromPipeline,
                    ValueFromPipelineByPropertyName = true,
                    ValidateNotNullOrEmpty = true,
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

            // For each ID in the URL, add a parameter
            foreach (string idParameterName in oDataRoute.IdParameters)
            {
                CmdletParameter idParameter = new CmdletParameter(idParameterName, typeof(string))
                {
                    Mandatory = true,
                    ValidateNotNullOrEmpty = true,
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
            string sharedParameterSetName)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }
            if (baseType == null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }

            // Track parameters as we visit each type
            IDictionary<OdcmType, IEnumerable<string>> parameterNameLookup = new Dictionary<OdcmType, IEnumerable<string>>();
            IDictionary<string, CmdletParameter> parameterLookup = new Dictionary<string, CmdletParameter>();

            // Visit all derived types
            baseType.VisitDerivedTypes((OdcmType type) =>
            {
                string parameterName = type.Name;
                string parameterSetName = "#" + type.FullName;

                // Create the parameter set for this type if it doesn't already exist
                CmdletParameterSet parameterSet = cmdlet.GetOrCreateParameterSet(parameterSetName);

                // Add a switch parameter for this type if required
                bool isAbstractType = type is OdcmClass @class && @class.IsAbstract;
                if (!isAbstractType)
                {
                    // Add the switch parameter
                    parameterSet.Add(new CmdletParameter(parameterName, typeof(PS.SwitchParameter))
                    {
                        Mandatory = true,
                        ParameterSetSelectorName = parameterSetName,
                        ValueFromPipelineByPropertyName = false,
                    });
                }

                // Evaluate the properties on this type
                IEnumerable<OdcmProperty> properties = type.EvaluateProperties(type == baseType)
                    .Where(prop => prop.Name != IdParameterName)
                    .Where(prop => !prop.ReadOnly && !prop.IsEnumeration() && !prop.IsLink);

                // Add this type into the parmeter name lookup table
                parameterNameLookup.Add(type, properties.Select(prop => prop.Name).Distinct());

                // Add the base types' properties as parameters to this parameter set
                // NOTE: Safe lookups are not necessary since all base types are guaranteed to have already been processed by the VisitDerivedTypes() method
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
                        parameter = new CmdletParameter(property.Name, propertyType)
                        {
                            Mandatory = property.IsRequired,
                            ValueFromPipelineByPropertyName = false,
                        };
                        parameterLookup.Add(property.Name, parameter);
                    }
                    else if (propertyType != parameter.Type)
                    {
                        // If all uses of this parameter don't use the same type, default to System.Object
                        parameter.Type = typeof(object);
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
                };

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
                CmdletParameter cmdletParameter = new CmdletParameter(parameter.Name, parameter.Type.ToPowerShellType(parameter.IsCollection))
                {
                    Mandatory = true,
                    ValidateNotNull = !parameter.IsNullable,
                };

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

            // TODO: Handle enums

            // Convert the type (default to System.Object if we can't convert the type)
            if (!EdmToPowerShellTypeMappings.TryGetValue(odcmType.FullName, out Type result))
            {
                result = typeof(object);
            }

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
