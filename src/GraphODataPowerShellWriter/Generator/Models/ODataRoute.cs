// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Inflector;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils;
    using Vipr.Core.CodeModel;

    /// <summary>
    /// Represents the route to an OData resource.
    /// </summary>
    public class ODataRoute
    {
        /// <summary>
        /// The segments in the route.
        /// </summary>
        public IEnumerable<OdcmProperty> Segments { get; }

        /// <summary>
        /// The parameters for the IDs of entities in the route.
        /// </summary>
        public IEnumerable<string> IdParameters => this._idParameters.Values;
        private IDictionary<OdcmProperty, string> _idParameters = new Dictionary<OdcmProperty, string>();

        /// <summary>
        /// Creates an OData route from an ODCM node.
        /// </summary>
        /// <param name="node">The ODCM node</param>
        public ODataRoute(OdcmNode node)
        {
            // Use a stack to add segments so we don't have to reverse them later
            Stack<OdcmProperty> segments = new Stack<OdcmProperty>();

            // Keep track of the ID parameters
            ICollection<string> idParameters = new List<string>();

            // Iterate over the node's parents
            OdcmNode currentNode = node;
            while (currentNode != null)
            {
                // Get the current node's property
                OdcmProperty property = currentNode.OdcmProperty;

                // Add this node to the route
                segments.Push(property);

                // If this node is not the final node in the route and the property it represents is an enumeration, track it's ID as a parameter
                string idParameterName;
                if (currentNode != node && property.TryGetIdParameterName(out idParameterName))
                {
                    _idParameters.Add(property, idParameterName);
                }

                // Get the parent node
                currentNode = currentNode.Parent;
            }

            this.Segments = segments;
        }

        /// <summary>
        /// Generates the OData route string with ID placeholders.
        /// It will have neither a leading slash nor trailing slash.
        /// </summary>
        /// <returns>The OData route.</returns>
        public string ToODataRouteString()
        {
            IList<string> segments = new List<string>();
            foreach (OdcmProperty property in this.Segments)
            {
                // Add this node to the route
                segments.Add(property.Name);

                // If this segment requires an ID, add it to the route
                string idParameter;
                if (this._idParameters.TryGetValue(property, out idParameter))
                {
                    segments.Add($"{{{idParameter}}}");
                }
            }

            return string.Join("/", segments);
        }

        /// <summary>
        /// Generates the relative file path string for the file that should be generated.
        /// It will have neither a leading slash nor trailing slash.
        /// </summary>
        /// <returns>The relative file path.</returns>
        public string ToRelativeFilePathString()
        {
            IEnumerable<string> segments = this.Segments
                .Select(property => this.ConvertPropertyNameToSingularPascalCase(property.Name));

            string result = string.Join("/", segments);

            return result;
        }

        /// <summary>
        /// Generates a cmdlet name's noun for the resource that can be found at the given ODataRoute.
        /// </summary>
        /// <returns>The cmdlet name.</returns>
        public string ToCmdletNameNounString()
        {
            IEnumerable<string> segments = this.Segments
                .Select(property => this.ConvertPropertyNameToSingularPascalCase(property.Name));

            string result = string.Join("_", segments);

            return result;
        }

        /// <summary>
        /// Generates the string OData route by calling <see cref="ToODataRouteString"/>.
        /// </summary>
        /// <returns>The OData route with ID placeholders.</returns>
        public override string ToString()
        {
            return this.ToODataRouteString();
        }

        /// <summary>
        /// Converts a property name to singular and then to pascal case.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>The singular pascal case property name.</returns>
        private string ConvertPropertyNameToSingularPascalCase(string propertyName)
        {
            string singularName = propertyName.Singularize() ?? propertyName;
            return singularName.Pascalize();
        }
    }
}
