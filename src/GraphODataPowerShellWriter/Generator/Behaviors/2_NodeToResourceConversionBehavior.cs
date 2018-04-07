// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils;
    using PowerShellGraphSDK;
    using Vipr.Core.CodeModel;

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
                // TODO: Support POST
                //yield return oDataRoute.CreatePostCmdlet();
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

        private static Cmdlet CreateGetCmdlet(this ODataRoute oDataRoute)
        {
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }

            // Get the ODCM property for this resource
            OdcmProperty property = oDataRoute.ResourceOdcmProperty;

            // Create the cmdlet
            Cmdlet cmdlet = new Cmdlet(new CmdletName("Get", oDataRoute.ToCmdletNameNounString()))
            {
                HttpMethod = "GET",
                ImpactLevel = CmdletImpactLevel.Low,
            };

            // Create the OData route
            string oDataRouteString = oDataRoute.ToODataRouteString();

            // Check whether this route needs to have an ID parameter added to the end
            if (oDataRoute.TryCreateIdParameter(out CmdletParameter idParameter))
            {
                // Create parameter set with the ID parameter
                CmdletParameterSet parameterSet = new CmdletParameterSet(ODataGetOrSearchPowerShellSDKCmdlet.ParameterSetGet)
                {
                    idParameter
                };

                // Add parameter set to cmdlet
                cmdlet.ParameterSets.Add(parameterSet);

                // Set the URL to use this parameter
                cmdlet.CallUrl = $"{oDataRouteString}/{{{idParameter.Name} ?? string.Empty}}";

                // Since the resource has an optional ID parameter, use the "search" base type to handle cases where the ID isn't provided
                cmdlet.BaseType = CmdletOperationType.GetOrSearch;
            }
            else
            {
                // This resource does have an ID parameter, so use the basic "get" base type
                cmdlet.CallUrl = oDataRouteString;
                cmdlet.BaseType = CmdletOperationType.Get;
            }

            // Add parameters to represent the ID placeholders in the URL
            cmdlet.AddIdParametersForRoutePlaceholders(oDataRoute);

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
            Cmdlet cmdlet = new Cmdlet(new CmdletName("Update", oDataRoute.ToCmdletNameNounString()))
            {
                HttpMethod = "PATCH",
                BaseType = CmdletOperationType.Patch,
                ImpactLevel = CmdletImpactLevel.Medium,
            };

            // Create the OData route
            string oDataRouteString = oDataRoute.ToODataRouteString();

            // Check whether this route needs to have an ID parameter added to the end
            if (oDataRoute.TryCreateIdParameter(out CmdletParameter idParameter))
            {
                // Add parameter to default parameter set
                cmdlet.DefaultParameterSet.Add(idParameter);

                // Set the URL to use this parameter
                cmdlet.CallUrl = $"{oDataRouteString}/{{{idParameter.Name}}}";
            }
            else
            {
                cmdlet.CallUrl = oDataRouteString;
            }

            // Add parameters to represent the ID placeholders in the URL
            cmdlet.AddIdParametersForRoutePlaceholders(oDataRoute);

            // Add properties of derived types as parameters to this cmdlet
            OdcmType resourceBaseType = resource.Type;
            Stack<OdcmType> unvisited = new Stack<OdcmType>();
            unvisited.Push(resourceBaseType);
            IDictionary<string, CmdletParameter> parameterLookup = new Dictionary<string, CmdletParameter>();
            while (unvisited.Any())
            {
                OdcmType type = unvisited.Pop();

                // Add derived types to the unvisited list
                foreach (OdcmType derivedType in type.GetDerivedTypes())
                {
                    unvisited.Push(derivedType);
                }

                // Evaluate the properties on this type
                IEnumerable<OdcmProperty> properties = type.EvaluateProperties(type == resourceBaseType)
                    .Where(prop => prop.Name != IdParameterName)
                    .Where(prop => !prop.ReadOnly && !prop.IsEnumeration() && !prop.IsLink);

                // Add the properties as parameters to this cmdlet
                foreach (OdcmProperty property in properties)
                {
                    // Create the parameter for this property if it doesn't already exist
                    if (!parameterLookup.TryGetValue(property.Name, out CmdletParameter parameter))
                    {
                        parameter = new CmdletParameter(property.Name, typeof(object));
                        parameterLookup.Add(property.Name, parameter);
                    }

                    // Iterate through base types
                    OdcmType currentType = type;
                    bool finished = false;
                    while (!finished)
                    {
                        // Create the parameter set if it doesn't already exist
                        string parameterSetName = property.Class.FullName;
                        CmdletParameterSet parameterSet = cmdlet.ParameterSets[parameterSetName];
                        if (parameterSet == null)
                        {
                            parameterSet = new CmdletParameterSet(parameterSetName);
                            cmdlet.ParameterSets.Add(parameterSet);
                        }

                        // Add this property to the parameter set
                        parameterSet.Add(parameter);

                        // Check if we're done (i.e. reached the base type)
                        if (currentType == resourceBaseType)
                        {
                            finished = true;
                        }
                        else
                        {
                            // Get the next base type
                            currentType = currentType.GetBaseType();
                        }
                    }
                }
            }

            return cmdlet;
        }

        private static Cmdlet CreateDeleteCmdlet(this ODataRoute oDataRoute)
        {
            if (oDataRoute == null)
            {
                throw new ArgumentNullException(nameof(oDataRoute));
            }

            // Create the cmdlet
            Cmdlet cmdlet = new Cmdlet(new CmdletName("Remove", oDataRoute.ToCmdletNameNounString()))
            {
                HttpMethod = "DELETE",
                BaseType = CmdletOperationType.Delete,
                ImpactLevel = CmdletImpactLevel.High,
            };

            // Create the OData route
            string oDataRouteString = oDataRoute.ToODataRouteString();

            // Check whether this route needs to have an ID parameter added to the end
            if (oDataRoute.TryCreateIdParameter(out CmdletParameter idParameter))
            {
                // Add parameter to default parameter set
                cmdlet.DefaultParameterSet.Add(idParameter);

                // Set the URL to use this parameter
                cmdlet.CallUrl = $"{oDataRouteString}/{{{idParameter.Name}}}";
            }
            else
            {
                cmdlet.CallUrl = oDataRouteString;
            }

            // Add parameters to represent the ID placeholders in the URL
            cmdlet.AddIdParametersForRoutePlaceholders(oDataRoute);

            return cmdlet;
        }

        /// <summary>
        /// Creates an ID parameter if it is required by the resource.
        /// </summary>
        /// <param name="parameterSet">The parameter set to add the ID parameter to</param>
        /// <param name="oDataRoute">The OData route to the resource</param>
        /// <returns>True if the ID parameter was created, otherwise false.</returns>
        private static bool TryCreateIdParameter(this ODataRoute oDataRoute, out CmdletParameter idParameter)
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
                    ValueFromPipeline = true,
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
    }
}
