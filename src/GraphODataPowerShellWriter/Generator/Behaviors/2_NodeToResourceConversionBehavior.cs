// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils;
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
            IEnumerable<Cmdlet> cmdlets = oDataRoute.GetCmdlets();

            // Add the cmdlets to the resource
            resource.AddAll(cmdlets);

            return resource;
        }

        /// <summary>
        /// Creates a set of cmdlets for an OData route.
        /// </summary>
        /// <param name="oDataRoute">The route to the resource (with ID placeholders)</param>
        /// <returns>The cmdlets.</returns>
        private static IEnumerable<Cmdlet> GetCmdlets(this ODataRoute oDataRoute)
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
        }

        #region HTTP Operation Cmdlets

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
                HttpMethod = "GET",
                ImpactLevel = PS.ConfirmImpact.None,
            };

            // Setup the parameters for the cmdlet
            CmdletParameter idParameter = cmdlet.SetupIdParameters(oDataRoute);
            if (idParameter != null)
            {
                // Allow an array of IDs to be passed in through the pipeline
                idParameter.ValueFromPipeline = true;

                // Create parameter set with the ID parameter
                CmdletParameterSet parameterSet = new CmdletParameterSet(ODataGetPowerShellSDKCmdlet.OperationName)
                {
                    idParameter
                };

                // Add parameter set to cmdlet
                cmdlet.ParameterSets.Add(parameterSet);

                // Since the resource has an optional ID parameter, use the "search" base type to handle cases where the ID isn't provided
                cmdlet.BaseType = CmdletOperationType.GetOrSearch;
            }
            else
            {
                // This resource doesn't have an ID parameter, so use the basic "get" base type
                cmdlet.BaseType = CmdletOperationType.Get;
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
                HttpMethod = "POST",
                BaseType = CmdletOperationType.Post,
                ImpactLevel = PS.ConfirmImpact.Low,
            };

            // Setup the parameters for the cmdlet
            cmdlet.SetupIdParameters(oDataRoute, true);

            // Add properties of derived types as parameters to this cmdlet by traversing the tree of derived types
            cmdlet.AddParametersForEntityProperties(resource.Type, ODataPostPowerShellSDKCmdlet.OperationName);

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
                HttpMethod = "PATCH",
                BaseType = CmdletOperationType.Patch,
                ImpactLevel = PS.ConfirmImpact.Medium,
            };

            // Setup the parameters for the cmdlet
            cmdlet.SetupIdParameters(oDataRoute);

            // Add properties of derived types as parameters to this cmdlet by traversing the tree of derived types
            cmdlet.AddParametersForEntityProperties(resource.Type, ODataPatchPowerShellSDKCmdlet.OperationName);

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
                HttpMethod = "DELETE",
                BaseType = CmdletOperationType.Delete,
                ImpactLevel = PS.ConfirmImpact.High,
            };

            // Setup the parameters for the cmdlet
            CmdletParameter idParameter = cmdlet.SetupIdParameters(oDataRoute);
            if (idParameter != null)
            {
                // Allow an array of IDs to be passed in through the pipeline
                idParameter.ValueFromPipeline = true;
            }

            return cmdlet;
        }

        #endregion HTTP Operation Cmdlets

        #region Helpers

        /// <summary>
        /// Sets up the ID parameters for a cmdlet based on the resource.
        /// </summary>
        /// <param name="cmdlet">The cmdlet</param>
        /// <param name="oDataRoute">The OData route to the resource</param>
        /// <returns>The ID parameter if it is required, otherwise null</returns>
        private static CmdletParameter SetupIdParameters(this Cmdlet cmdlet, ODataRoute oDataRoute, bool dontAddEntityId = false)
        {
            CmdletParameter idParameter = null;

            // Create the OData route
            string oDataRouteString = oDataRoute.ToODataRouteString();

            // Check whether this route needs to have an ID parameter added to the end
            if (!dontAddEntityId && oDataRoute.TryCreateEntityIdParameter(out idParameter))
            {
                // Add parameter to default parameter set
                cmdlet.DefaultParameterSet.Add(idParameter);

                // Set the URL to use this parameter
                cmdlet.CallUrl = $"{oDataRouteString}/{{{idParameter.Name} ?? string.Empty}}";
            }
            else
            {
                cmdlet.CallUrl = oDataRouteString;
            }

            // Add parameters to represent the ID placeholders in the URL
            cmdlet.AddIdParametersForRoutePlaceholders(oDataRoute);

            return idParameter;
        }

        /// <summary>
        /// Creates an ID parameter if it is required by the resource.
        /// </summary>
        /// <param name="parameterSet">The parameter set to add the ID parameter to</param>
        /// <param name="oDataRoute">The OData route to the resource</param>
        /// <returns>True if the ID parameter was created, otherwise false.</returns>
        private static bool TryCreateEntityIdParameter(this ODataRoute oDataRoute, out CmdletParameter idParameter)
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
                    Mandatory = true,
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
            // Track parameters as we visit each type
            IDictionary<OdcmType, IEnumerable<string>> parameterNameLookup = new Dictionary<OdcmType, IEnumerable<string>>();
            // TODO: Allow multiple instances of CmdletParameter (1 per parameter set) so validation can be correctly enforced
            IDictionary<string, CmdletParameter> parameterLookup = new Dictionary<string, CmdletParameter>();

            // Visit all derived types
            baseType.VisitDerivedTypes((OdcmType type) =>
            {
                string parameterName = type.Name;
                string parameterSetName = "#" + type.FullName;

                // Create the parameter set for this type if it doesn't already exist
                CmdletParameterSet parameterSet = cmdlet.ParameterSets[parameterSetName];
                if (parameterSet == null)
                {
                    parameterSet = new CmdletParameterSet(parameterSetName);
                    cmdlet.ParameterSets.Add(parameterSet);
                }

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
                    // Create the parameter for this property if it doesn't already exist
                    if (!parameterLookup.TryGetValue(property.Name, out CmdletParameter parameter))
                    {
                        // TODO: Set the property type correctly, as defined in the ODCM property - we need to track the types for all instances of this property name and then resolve the C# type later
                        parameter = new CmdletParameter(property.Name, typeof(object))
                        {
                            Mandatory = property.IsRequired,
                            ValueFromPipelineByPropertyName = false,
                        };
                        parameterLookup.Add(property.Name, parameter);
                    }

                    // Add this type's properties as parameters to this parameter set
                    parameterSet.Add(parameter);
                }
            });

            // Get/create the shared parameter set if required (e.g. the "Post" parameter set for the "Create" cmdlet)
            CmdletParameterSet sharedParameterSet = null;
            if (sharedParameterSetName != null)
            {
                sharedParameterSet = cmdlet.ParameterSets[sharedParameterSetName];
                if (sharedParameterSet == null)
                {
                    sharedParameterSet = new CmdletParameterSet(sharedParameterSetName);
                    cmdlet.ParameterSets.Add(sharedParameterSet);
                }

                // All properties should be part of the shared parameter set
                sharedParameterSet.AddAll(parameterLookup.Values);
            }
        }

        #endregion Helpers
    }
}
