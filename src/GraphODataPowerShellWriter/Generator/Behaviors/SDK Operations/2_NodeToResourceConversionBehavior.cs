﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils;
    using PowerShellGraphSDK.ODataConstants;
    using PowerShellGraphSDK.PowerShellCmdlets;
    using Vipr.Core;
    using Vipr.Core.CodeModel;
    using PS = System.Management.Automation;

    /// <summary>
    /// The behavior to convert an ODCM tree to a collection of resources.
    /// </summary>
    public static class NodeToResourceConversionBehavior
    {
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
            if (pathPrefix == null)
            {
                throw new ArgumentNullException(nameof(pathPrefix));
            }

            // Calculate route
            ODataRoute oDataRoute = new ODataRoute(node);

            // Create file system path
            string fileSystemPath = $"{pathPrefix.TrimEnd('\\')}\\{oDataRoute.ToRelativeFilePathString()}";

            // Create a resource
            Resource resource = new Resource(fileSystemPath);

            // Convert each ODCM property into a set of cmdlets
            IEnumerable<OperationCmdlet> cmdlets = oDataRoute.CreateCmdlets();

            // Add the cmdlets to the resource
            resource.AddAll(cmdlets);

            return resource;
        }

        /// <summary>
        /// Creates a set of cmdlets for an OData route.
        /// </summary>
        /// <param name="oDataRoute">The route to the resource (with ID placeholders)</param>
        /// <returns>The cmdlets.</returns>
        private static IEnumerable<OperationCmdlet> CreateCmdlets(this ODataRoute oDataRoute)
        {
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }

            // Get the ODCM property
            OdcmProperty property = oDataRoute.Property;

            // Get the parent ODCM property
            OdcmProperty parentProperty = oDataRoute.ParentProperty;

            // Figure out whether this should be a "$ref" path
            bool isReference = property.IsReference(parentProperty);

            // Get/Search for the objects irrespective of whether it is a "$ref" path
            yield return oDataRoute.CreateGetCmdlet();

            // If this is for a "$ref" path, only implement POST/PUT and DELETE as well as a GET for the reference URLs
            if (isReference)
            {
                // Get the reference URLs (not the actual referenced objects)
                yield return oDataRoute.CreateGetCmdlet(isReference);

                // Post (create a reference)
                if (property.SupportsInsert())
                {
                    yield return oDataRoute.CreatePostRefCmdlet(parentProperty);
                }

                // Delete (remove a reference)
                if (property.SupportsDelete())
                {
                    yield return oDataRoute.CreateDeleteRefCmdlet(parentProperty);
                }
            }
            else
            {
                // Post
                if (property.SupportsInsert() && !property.IsComputed())
                {
                    yield return oDataRoute.CreatePostCmdlet();
                }

                // Patch
                if (property.SupportsUpdate() && !property.IsComputed() && !property.IsImmutable())
                {
                    yield return oDataRoute.CreatePatchCmdlet();
                }

                // Delete
                if (property.SupportsDelete() && !property.IsComputed())
                {
                    yield return oDataRoute.CreateDeleteCmdlet();
                }

                // Actions and Functions
                foreach (OperationCmdlet actionCmdlet in oDataRoute.CreateActionAndFunctionCmdlets())
                {
                    yield return actionCmdlet;
                }
            }
        }

        #region Operation Cmdlets

        private static OperationCmdlet CreateGetCmdlet(this ODataRoute oDataRoute, bool isReference = false)
        {
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }

            // Get the ODCM property for this resource
            OdcmProperty resource = oDataRoute.Property;

            // Create the cmdlet
            OperationCmdlet cmdlet = new OperationCmdlet(PS.VerbsCommon.Get, oDataRoute.ToCmdletNameNounString(isReference))
            {
                ResourceTypeFullName = oDataRoute.Property.Type.FullName,
                ImpactLevel = PS.ConfirmImpact.None,
                Documentation = new CmdletDocumentation()
                {
                    Synopsis = resource.IsCollection
                        ? $"Retrieves \"{resource.Type.FullName}\" objects."
                        : $"Retrieves the \"{resource.Name}\" object.",
                    Descriptions = new string[]
                    {
                        $"GET ~/{oDataRoute.ToODataRouteString()}",
                        resource.IsCollection
                            ? $"Retrieves \"{resource.Type.FullName}\" objects in the \"{resource.Name}\" collection."
                            : $"Retrieves the \"{resource.Name}\" object (which is of type \"{resource.Type.FullName}\").",
                        resource.Description,
                        resource.LongDescription,
                    },
                },
            };

            // Setup the parameters for the cmdlet
            cmdlet.SetupIdParametersAndCallUrl(
                oDataRoute,
                idParameterSetName: GetCmdlet.OperationName,
                cmdletReturnsReferenceableEntities: !isReference,
                postfixUrlSegments: isReference ? new string[] { "$ref" } : Array.Empty<string>());
            if (cmdlet.IdParameter != null)
            {
                // Since the resource has an optional ID parameter, use the "SEARCH" base type to handle cases where the ID isn't provided
                cmdlet.OperationType = CmdletOperationType.GetOrSearch;
                cmdlet.DefaultParameterSetName = GetOrSearchCmdlet.OperationName;
            }
            else
            {
                // This resource doesn't have an ID parameter, so use the basic "GET" base type
                cmdlet.OperationType = CmdletOperationType.Get;
                cmdlet.DefaultParameterSetName = GetCmdlet.OperationName;
            }

            // Add the properties without marking them as PowerShell parameters to allow for auto-complete when picking columns for $select and $expand
            cmdlet.AddParametersForEntityProperties(
                baseType: resource.Type,
                isReadOnlyFunc: cmdlet.GetIsReadOnlyFunc(),
                sharedParameterSetName: null,
                addSwitchParameters: false,
                markAsPowerShellParameter: false);

            return cmdlet;
        }

        private static OperationCmdlet CreatePostCmdlet(this ODataRoute oDataRoute)
        {
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }

            // Get the ODCM property for this resource
            OdcmProperty resource = oDataRoute.Property;

            // Create the cmdlet
            OperationCmdlet cmdlet = new OperationCmdlet(PS.VerbsCommon.New, oDataRoute.ToCmdletNameNounString())
            {
                ResourceTypeFullName = oDataRoute.Property.Type.FullName,
                OperationType = CmdletOperationType.Post,
                ImpactLevel = PS.ConfirmImpact.Low,
                Documentation = new CmdletDocumentation()
                {
                    Synopsis = $"Creates a \"{resource.Type.FullName}\" object.",
                    Descriptions = new string[]
                    {
                        $"POST ~/{oDataRoute.ToODataRouteString()}",
                        resource.IsCollection
                            ? $"Adds a \"{resource.Type.FullName}\" object to the \"{resource.Name}\" collection."
                            : $"Creates the \"{resource.Name}\" object (which is of type \"{resource.Type.FullName}\").",
                        resource.Description,
                        resource.LongDescription,
                    },
                },
            };

            // Setup the parameters for the cmdlet
            cmdlet.SetupIdParametersAndCallUrl(oDataRoute, addEntityId: false, idValueFromPipeline: false, cmdletReturnsReferenceableEntities: true);

            // Add properties of derived types as parameters to this cmdlet by traversing the tree of derived types
            cmdlet.AddParametersForEntityProperties(resource.Type, cmdlet.GetIsReadOnlyFunc(), PostOrPatchCmdlet.SharedParameterSet);

            return cmdlet;
        }

        private static OperationCmdlet CreatePostRefCmdlet(this ODataRoute oDataRoute, OdcmProperty parentResource)
        {
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }
            if (parentResource == null)
            {
                throw new ArgumentNullException(nameof(parentResource));
            }

            // Get the ODCM property for this resource
            OdcmProperty resource = oDataRoute.Property;

            // Create the cmdlet
            OperationCmdlet cmdlet = new OperationCmdlet(PS.VerbsCommon.New, oDataRoute.ToCmdletNameNounString(isReference: true))
            {
                ResourceTypeFullName = oDataRoute.Property.Type.FullName,
                OperationType = resource.IsCollection
                    ? CmdletOperationType.PostRefToCollection
                    : CmdletOperationType.PutRefToSingleEntity,
                ImpactLevel = PS.ConfirmImpact.Low,
                Documentation = new CmdletDocumentation()
                {
                    Synopsis = $"Creates a reference from a \"{parentResource.Name.Singularize()}\" to a \"{resource.Type.FullName}\" object.",
                    Descriptions = new string[]
                    {
                        $"{(resource.IsCollection ? "POST" : "PUT")} ~/{oDataRoute.ToODataRouteString(includeEntityId: false)}/$ref",
                        resource.IsCollection
                            ? $"Creates a reference from the specified \"{parentResource.Name.Singularize()}\" object to a \"{resource.Name.Singularize()}\"."
                            : $"Creates a reference from the \"{parentResource.Name.Singularize()}\" object to a \"{resource.Name.Singularize()}\".",
                        resource.Description,
                        resource.LongDescription,
                    },
                },
            };

            // Create the URL and setup ID parameters
            cmdlet.SetupIdParametersAndCallUrl(oDataRoute, addEntityId: false, postfixUrlSegments: "$ref");

            // Create the reference URL parameter
            cmdlet.SetupReferenceUrlParameter(oDataRoute);

            return cmdlet;
        }

        private static OperationCmdlet CreatePatchCmdlet(this ODataRoute oDataRoute)
        {
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }

            // Get the ODCM property for this resource
            OdcmProperty resource = oDataRoute.Property;

            // Create the cmdlet
            OperationCmdlet cmdlet = new OperationCmdlet(PS.VerbsData.Update, oDataRoute.ToCmdletNameNounString())
            {
                ResourceTypeFullName = oDataRoute.Property.Type.FullName,
                OperationType = CmdletOperationType.Patch,
                ImpactLevel = PS.ConfirmImpact.Medium,
                Documentation = new CmdletDocumentation()
                {
                    Synopsis = $"Updates a \"{resource.Type.FullName}\".",
                    Descriptions = new string[]
                    {
                        $"PATCH ~/{oDataRoute.ToODataRouteString()}",
                        resource.IsCollection
                            ? $"Updates a \"{resource.Type.FullName}\" object in the \"{resource.Name}\" collection."
                            : $"Updates the \"{resource.Name}\" object (which is of type \"{resource.Type.FullName}\").",
                        resource.Description,
                        resource.LongDescription,
                    },
                },
            };

            // Setup the parameters for the cmdlet
            cmdlet.SetupIdParametersAndCallUrl(oDataRoute, idValueFromPipeline: false);

            // Add properties of derived types as parameters to this cmdlet by traversing the tree of derived types
            cmdlet.AddParametersForEntityProperties(resource.Type, cmdlet.GetIsReadOnlyFunc(), PostOrPatchCmdlet.SharedParameterSet);

            return cmdlet;
        }

        private static OperationCmdlet CreateDeleteCmdlet(this ODataRoute oDataRoute)
        {
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }

            // Get the ODCM property for this resource
            OdcmProperty resource = oDataRoute.Property;

            // Create the cmdlet
            OperationCmdlet cmdlet = new OperationCmdlet(PS.VerbsCommon.Remove, oDataRoute.ToCmdletNameNounString())
            {
                ResourceTypeFullName = oDataRoute.Property.Type.FullName,
                OperationType = CmdletOperationType.Delete,
                ImpactLevel = PS.ConfirmImpact.High,
            };

            // Setup the parameters for the cmdlet
            CmdletParameter idParameter = cmdlet.SetupIdParametersAndCallUrl(oDataRoute);

            // Add documentation
            string idParameterSegment = idParameter != null ? $"/{idParameter.Name}" : string.Empty;
            cmdlet.Documentation = new CmdletDocumentation()
            {
                Synopsis = $"Removes a \"{resource.Type.FullName}\" object.",
                Descriptions = new string[]
                {
                    $"DELETE ~/{oDataRoute.ToODataRouteString()}{idParameterSegment}",
                    resource.IsCollection
                        ? $"Removes a \"{resource.Type.FullName}\" object from the \"{resource.Name}\" collection."
                        : $"Removes the \"{resource.Name}\" object (which is of type \"{resource.Type.FullName}\").",
                    resource.Description,
                    resource.LongDescription,
                },
            };

            return cmdlet;
        }

        private static OperationCmdlet CreateDeleteRefCmdlet(this ODataRoute oDataRoute, OdcmProperty parentResource)
        {
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }
            if (parentResource == null)
            {
                throw new ArgumentNullException(nameof(parentResource));
            }

            // Get the ODCM property for this resource
            OdcmProperty resource = oDataRoute.Property;

            // Create the cmdlet
            OperationCmdlet cmdlet = new OperationCmdlet(PS.VerbsCommon.Remove, oDataRoute.ToCmdletNameNounString(isReference: true))
            {
                ResourceTypeFullName = oDataRoute.Property.Type.FullName,
                OperationType = CmdletOperationType.Delete,
                ImpactLevel = PS.ConfirmImpact.High,
            };

            // Setup the parameters for the cmdlet
            CmdletParameter idParameter = cmdlet.SetupIdParametersAndCallUrl(oDataRoute, postfixUrlSegments: "$ref");

            // Add documentation
            string idParameterSegment = idParameter != null ? $"/{idParameter.Name}" : string.Empty;
            cmdlet.Documentation = new CmdletDocumentation()
            {
                Synopsis = $"Removes a reference from a \"{parentResource.Name.Singularize()}\" to a \"{resource.Type.FullName}\" object.",
                Descriptions = new string[]
                {
                    $"DELETE ~/{oDataRoute.ToODataRouteString()}{idParameterSegment}/$ref",
                    resource.IsCollection
                        ? $"Removes a reference from the specified \"{parentResource.Name.Singularize()}\" object to a \"{resource.Name.Singularize()}\"."
                        : $"Removes a reference from the \"{parentResource.Name.Singularize()}\" object to a \"{resource.Name.Singularize()}\".",
                    resource.Description,
                    resource.LongDescription,
                },
            };

            return cmdlet;
        }

        private static IEnumerable<OperationCmdlet> CreateActionAndFunctionCmdlets(this ODataRoute oDataRoute)
        {
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }

            // Get the ODCM property for this resource
            OdcmProperty resource = oDataRoute.Property;

            // We can only create action/function cmdlets if the resource type is a class with methods
            if (resource.Type is OdcmClass resourceType)
            {
                foreach (OdcmMethod method in resourceType.Methods)
                {
                    // Create the cmdlet
                    OperationCmdlet cmdlet = new OperationCmdlet(PS.VerbsLifecycle.Invoke, oDataRoute.ToCmdletNameNounString(postfixSegments: method.Name))
                    {
                        ResourceTypeFullName = oDataRoute.Property.Type.FullName,
                    };

                    // Figure out if this method is a function or an action
                    string methodType;
                    if (method.IsFunction)
                    {
                        methodType = "function";
                        cmdlet.SetupFunctionDetails(method);
                    }
                    else
                    {
                        methodType = "action";
                        cmdlet.SetupActionDetails(method);
                    }

                    // Documentation
                    string oDataRouteString = $"{oDataRoute.ToODataRouteString()}/{method.Name}";
                    cmdlet.Documentation = new CmdletDocumentation()
                    {
                        Descriptions = new string[]
                        {
                            method.IsFunction
                                ? $"GET ~/{oDataRouteString}" // function
                                : $"POST ~/{oDataRouteString}", // action
                            $"The {methodType} \"{method.FullName}\", which exists on the type \"{resourceType.FullName}\".",
                            method.ReturnType != null
                                ? method.IsCollection
                                    ? $"This {methodType} returns a collection of \"{method.ReturnType.FullName}\" objects." // collection
                                    : $"This {methodType} returns a \"{method.ReturnType.FullName}\" object." // single entity
                                : $"This {methodType} does not return any objects.", // void return type
                            method.Description,
                        },
                    };

                    // Setup the ID parameters and call URL
                    cmdlet.SetupIdParametersAndCallUrl(
                        oDataRoute,
                        addEntityId: !method.IsBoundToCollection, // if the function is bound to a collection, we don't need an ID parameter
                        postfixUrlSegments: method.Name);

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
        /// Sets the ID parameter on the cmdlet to be a parameter which accepts a reference URL.
        /// </summary>
        /// <param name="cmdlet">The cmdlet</param>
        /// <param name="oDataRoute">The OData route that the cmdlet operates on</param>
        /// <returns>The parameter which accepts a reference URL.</returns>
        private static CmdletParameter SetupReferenceUrlParameter(this OperationCmdlet cmdlet, ODataRoute oDataRoute)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }

            // Get the resource type
            OdcmType resourceType = oDataRoute.Property.Type;

            // Get the name of the parameter which accepts a resource URL
            string referenceUrlParameterName = resourceType.GetResourceUrlParameterName();

            // Create parameter aliases for all derived types
            IEnumerable<string> aliases = resourceType.GetDerivedTypes().Select(type => type.GetResourceUrlParameterName());

            // Create the parameter
            CmdletParameter referenceUrlParameter = new CmdletParameter(referenceUrlParameterName, typeof(string))
            {
                Aliases = aliases,
                Mandatory = true,
                ValueFromPipeline = true,
                ValueFromPipelineByPropertyName = true,
                ValidateNotNullOrEmpty = true,
                ValidateUrlIsAbsolute = true,
                Documentation = new CmdletParameterDocumentation()
                {
                    Descriptions = new string[]
                    {
                        $"The URL which should be used to access a \"{resourceType.FullName}\" object.",
                    },
                },
            };

            // Add this parameter to the default parameter set
            cmdlet.DefaultParameterSet.Add(referenceUrlParameter);

            // Set this as the ID parameter so it can be used to extract the user-provided URL at runtime
            cmdlet.IdParameter = referenceUrlParameter;

            return referenceUrlParameter;
        }

        /// <summary>
        /// Sets up the ID parameters for a cmdlet based on the resource.
        /// 
        /// If an ID parameter is required for the ID, a reference will be provided in the cmdlet's <see cref="OperationCmdlet.IdParameter"/> property.
        /// </summary>
        /// <param name="cmdlet">The cmdlet</param>
        /// <param name="oDataRoute">The OData route to the resource</param>
        /// <param name="addEntityId">Whether or not to add the entity ID (i.e. the final ID in the route)</param>
        /// <param name="idParameterSetName">
        /// The name of the parameter set to add the entity ID parameter to - if this is null, it will be added to the default parameter set
        /// and will be mandatory
        /// </param>
        /// <param name="idValueFromPipeline">Whether or not the ID parameter should be extracted from objects on the pipeline</param>
        /// <param name="cmdletReturnsReferenceableEntities">
        /// Whether or not this cmdlet may return objects that can be referenced by "$ref" requests
        /// </param>
        /// <param name="postfixUrlSegments">The URL segments to postfix if required</param>
        /// <returns>The ID parameter if it was required and added to the cmdlet, otherwise null</returns>
        private static CmdletParameter SetupIdParametersAndCallUrl(
            this OperationCmdlet cmdlet,
            ODataRoute oDataRoute,
            bool addEntityId = true,
            string idParameterSetName = null,
            bool idValueFromPipeline = true,
            bool cmdletReturnsReferenceableEntities = false,
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
                    // Add the parameter to the parameter set
                    cmdlet.DefaultParameterSet.Add(idParameter);

                    // Add the ID parameter to the URL
                    cmdlet.CallUrl += $"/{{{idParameter.Name}}}";
                }
                else
                {
                    // Add the parameter to the parameter set
                    cmdlet.GetOrCreateParameterSet(idParameterSetName).Add(idParameter);

                    // Add the ID parameter to the URL
                    cmdlet.CallUrl += $"/{{{idParameter.Name} ?? string.Empty}}";
                }

                // Track the ID parameter in the cmdlet
                cmdlet.IdParameter = idParameter;
            }
            else
            {
                cmdlet.CallUrl = oDataRouteString;
            }

            // Postfix the custom URL segments
            bool hasPostfixUrlSegments = postfixUrlSegments.Any();
            if (hasPostfixUrlSegments)
            {
                cmdlet.CallUrl += "/" + string.Join("/", postfixUrlSegments);
            }

            // Add parameters to represent the ID placeholders in the URL
            cmdlet.AddIdParametersForRoutePlaceholders(oDataRoute);

            // Check whether we should mark this as a resource that can be referenced from a "$ref" cmdlet
            if (!hasPostfixUrlSegments && oDataRoute.Property.ContainsTarget && cmdletReturnsReferenceableEntities)
            {
                cmdlet.IsReferenceable = true;
            }

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
            OdcmProperty resource = oDataRoute.Property;

            // Add the ID parameter only if it requires it
            if (oDataRoute.Property.TryGetIdParameterName(out string idParameterName))
            {
                // Create the ID parameter
                idParameter = new CmdletParameter(idParameterName, typeof(string))
                {
                    Aliases = RequestProperties.Id.SingleObjectAsEnumerable(),
                    Mandatory = entityIdIsMandatory,
                    ValueFromPipeline = valueFromPipeline,
                    ValueFromPipelineByPropertyName = true,
                    ValidateNotNullOrEmpty = true,
                    Documentation = new CmdletParameterDocumentation()
                    {
                        Descriptions = new string[]
                        {
                            $"The ID for a \"{resource.Type.FullName}\" object in the \"{resource.Name}\" collection.",
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
        private static void AddIdParametersForRoutePlaceholders(this OperationCmdlet cmdlet, ODataRoute oDataRoute)
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
            OdcmProperty resource = oDataRoute.Property;

            // For each ID in the URL, add a parameter
            foreach (OdcmProperty segmentProperty in oDataRoute.IdParameterProperties)
            {
                string idParameterName = oDataRoute.GetIdParameterName(segmentProperty);
                CmdletParameter idParameter = new CmdletParameter(idParameterName, typeof(string))
                {
                    Mandatory = true,
                    ValidateNotNullOrEmpty = true,
                    Documentation = new CmdletParameterDocumentation()
                    {
                        Descriptions = new string[]
                        {
                            $"A required ID for referencing a \"{segmentProperty.Type.FullName}\" object in the \"{segmentProperty.Name}\" collection.",
                        },
                    },
                };
                cmdlet.DefaultParameterSet.Add(idParameter);
            }
        }

        /// <summary>
        /// Sets up the cmdlet's values for the specified function.
        /// </summary>
        /// <param name="cmdlet">The cmdlet</param>
        /// <param name="function">The function</param>
        private static void SetupFunctionDetails(this OperationCmdlet cmdlet, OdcmMethod function)
        {
            // Since this is a function, it should not have any observable side-effects (according to the OData v4 spec)
            cmdlet.ImpactLevel = PS.ConfirmImpact.None;

            // The behavior of the function should be different based on whether it returns a collection or not
            if (function.IsCollection)
            {
                cmdlet.DefaultParameterSetName = GetOrSearchCmdlet.OperationName;
                cmdlet.OperationType = CmdletOperationType.FunctionReturningCollection;
            }
            else
            {
                cmdlet.DefaultParameterSetName = GetCmdlet.OperationName;
                cmdlet.OperationType = CmdletOperationType.FunctionReturningEntity;
            }

            // Setup the function parameters for each overload
            IEnumerable<OdcmMethod> functionOverloads = function.SingleObjectAsEnumerable().Concat(function.Overloads); // combined list
            int numOverloads = functionOverloads.Count();
            int overloadIndex = 1;
            CmdletParameterSet selectedParameterSet = null;
            foreach (OdcmMethod currentFunction in functionOverloads)
            {
                string parameterSetName = numOverloads == 1
                    ? null
                    : $"Overload_{overloadIndex}";
                CmdletParameterSet createdParameterSet = cmdlet.AddFunctionParameters(currentFunction, parameterSetName);

                // Only increment the count if the function's overload takes parameters
                if (createdParameterSet != cmdlet.DefaultParameterSet)
                {
                    overloadIndex++;
                }

                // If we have selected the default parameter set, we know that we cannot find any overloads with less parameters
                if (selectedParameterSet != cmdlet.DefaultParameterSet)
                {
                    // Decide whether this should be the default parameter set
                    if (selectedParameterSet == null)
                    {
                        selectedParameterSet = createdParameterSet;
                    }
                    else
                    {
                        int createdNumMandatoryParams = createdParameterSet.Where(param => param.Mandatory).Count();
                        int selectedNumMandatoryParams = selectedParameterSet.Where(param => param.Mandatory).Count();

                        if (// Use the parameter set with the smallest number of mandatory parameters
                            createdNumMandatoryParams < selectedNumMandatoryParams
                            // Use the parameter set with the smallest number of parameters if the mandatory parameters are equal
                            || (createdNumMandatoryParams == selectedNumMandatoryParams && createdParameterSet.Count < selectedParameterSet.Count))
                        {
                            selectedParameterSet = createdParameterSet;
                        }
                    }
                }
            }

            // Set the default parameter set if it is not the default parameter set
            if (selectedParameterSet != cmdlet.DefaultParameterSet)
            {
                cmdlet.DefaultParameterSetName = selectedParameterSet.Name;
            }
        }

        /// <summary>
        /// Sets up the cmdlet's values for the specified action.
        /// </summary>
        /// <param name="cmdlet">The cmdlet</param>
        /// <param name="action">The action</param>
        private static void SetupActionDetails(this OperationCmdlet cmdlet, OdcmMethod action)
        {
            // Set the cmdlet up as a method
            cmdlet.OperationType = CmdletOperationType.Action;
            cmdlet.ImpactLevel = PS.ConfirmImpact.High;

            // Setup the action parameters
            cmdlet.AddActionParameters(action);
        }

        private static CmdletParameterSet AddFunctionParameters(this OperationCmdlet cmdlet, OdcmMethod function, string parameterSetName = null)
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
                throw new ArgumentException($"The given ODCM method \"{function.Name}\" does not represent a function - it represents an action", nameof(function));
            }

            // Don't continue if there aren't any parameters
            if (!function.Parameters.Any())
            {
                return cmdlet.DefaultParameterSet;
            }

            // Get the parameter set that we should add the parameters to
            CmdletParameterSet selectedParameterSet;
            if (parameterSetName == null)
            {
                selectedParameterSet = cmdlet.DefaultParameterSet;
            }
            else
            {
                selectedParameterSet = cmdlet.GetOrCreateParameterSet(parameterSetName);
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
                    ODataTypeFullName = parameter.Type.FullName,
                    Documentation = new CmdletParameterDocumentation()
                    {
                        Descriptions = new string[]
                        {
                            $"The \"{parameter.Name}\" parameter, which is accepted by the \"{function.FullName}\" function.",
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
                selectedParameterSet.Add(cmdletParameter);
            }

            return selectedParameterSet;
        }

        private static void AddActionParameters(this OperationCmdlet cmdlet, OdcmMethod action)
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
                throw new ArgumentException($"The given ODCM method \"{action.Name}\" does not represent an action - it represents a function", nameof(action));
            }

            // Iterate over each parameter
            foreach (OdcmParameter parameter in action.Parameters)
            {
                // Create the equivalent CmdletParameter object
                CmdletParameter cmdletParameter = new CmdletParameter(parameter.Name, parameter.Type.ToPowerShellType(parameter.IsCollection))
                {
                    Mandatory = true,
                    ValidateNotNull = !parameter.IsNullable,
                    ODataTypeFullName = parameter.Type.FullName,
                    Documentation = new CmdletParameterDocumentation()
                    {
                        Descriptions = new string[]
                        {
                            $"The \"{parameter.Name}\" parameter, which is accepted by the \"{action.FullName}\" action.",
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

        private static Func<OdcmProperty, bool> GetIsReadOnlyFunc(this OperationCmdlet cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return property => (property.IsComputed() && cmdlet.OperationType.IsInsertUpdateOrDeleteOperation())
                            || (property.IsImmutable() && cmdlet.OperationType.IsInsertOrDeleteOperation());
        }

        #endregion Helpers
    }
}