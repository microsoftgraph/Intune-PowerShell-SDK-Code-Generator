// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils;
    using Vipr.Core.CodeModel;

    /// <summary>
    /// Represents the route to an OData resource.
    /// </summary>
    public class ODataRoute
    {
        /// <summary>
        /// The segments in the route (not including IDs).
        /// </summary>
        public IList<OdcmProperty> Segments { get; }

        /// <summary>
        /// The ODCM Property that represents this route's resource.
        /// </summary>
        public OdcmProperty Property { get; }

        /// <summary>
        /// The ODCM property that represents the parent resource.
        /// </summary>
        public OdcmProperty ParentProperty { get; }

        /// <summary>
        /// The parameters for the IDs of entities in the route.
        /// </summary>
        private readonly IDictionary<OdcmProperty, string> _idParameters = new Dictionary<OdcmProperty, string>();

        /// <summary>
        /// The ODCM properties which require an ID to build this OData route.
        /// </summary>
        public IEnumerable<OdcmProperty> IdParameterProperties => this._idParameters.Keys;

        /// <summary>
        /// The parameters for the IDs of entities in the route.
        /// </summary>
        private readonly IDictionary<OdcmProperty, string> _typeCastParameters = new Dictionary<OdcmProperty, string>();

        /// <summary>
        /// The ODCM properties which require an ID to build this OData route.
        /// </summary>
        public IEnumerable<OdcmProperty> TypeCastParameterProperties => this._typeCastParameters.Keys;

        /// <summary>
        /// Creates an OData route from an ODCM node.
        /// </summary>
        /// <param name="node">The ODCM node</param>
        public ODataRoute(OdcmNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            // Store the node's ODCM property
            this.Property = node.OdcmProperty;

            // Store the parent node's ODCM property if it exists
            this.ParentProperty = node.Parent?.OdcmProperty;

            // Use a stack to add segments so we don't have to reverse them later
            Stack<OdcmProperty> segments = new Stack<OdcmProperty>();

            // Keep track of the ID parameters
            ICollection<string> idParameters = new List<string>();

            // Iterate over the node's parents (i.e. traverse the route in reverse)
            OdcmNode currentNode = node;
            OdcmNode childNode = null;
            while (currentNode != null)
            {
                // Get the current node's property
                OdcmProperty property = currentNode.OdcmProperty;

                // Add this node to the route
                segments.Push(property);

                // Ignore this logic for the last segment in the route
                bool isLastSegment = childNode == null;
                if (!isLastSegment)
                {
                    // If the property this node represents is an enumeration, track its ID as a parameter
                    if (property.TryGetIdParameterName(out string idParameterName))
                    {
                        this._idParameters.Add(property, idParameterName);
                    }

                    // If the property's type is not the same as the class that the child property is defined in, the child class must be a subtype
                    if (property.Type != childNode.OdcmProperty.Class)
                    {
                        this._typeCastParameters.Add(property, property.GetResourceTypeParameterName());
                    }
                }

                // Update pointers
                childNode = currentNode;
                currentNode = currentNode.Parent;
            }

            this.Segments = segments.ToList();
        }

        /// <summary>
        /// Gets the name of the ID parameter given a property.
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>The ID parameter name if the property represents a collection, otherwise null.</returns>
        public string GetIdParameterName(OdcmProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return this._idParameters.TryGetValue(property, out string idParameterName)
                ? idParameterName
                : null;
        }

        /// <summary>
        /// Gets the name of the typecast parameter given a property.
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>The typecast parameter name</returns>
        public string GetTypeCastParameterName(OdcmProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return this._typeCastParameters.TryGetValue(property, out string typeCastParameterName)
                ? typeCastParameterName
                : null;
        }

        /// <summary>
        /// Generates the OData route string with ID and typecast placeholders.
        /// It will have neither a leading slash nor trailing slash.
        /// </summary>
        /// <param name="includeEntityIdAndTypeCast">Whether or not to include the entity ID and typecast placeholders if required</param>
        /// <returns>The OData route.</returns>
        public string ToODataRouteString(bool includeEntityIdAndTypeCast = true)
        {
            IList<string> segments = new List<string>();
            OdcmProperty lastSegment = this.Segments.LastOrDefault();
            foreach (OdcmProperty property in this.Segments)
            {
                // Add this node to the route
                segments.Add(property.Name);

                // Add the ID and typecast after the final segment only if the caller wants to include them
                if (property != lastSegment || includeEntityIdAndTypeCast)
                {
                    // If this segment requires an ID, add it to the route
                    if (this._idParameters.TryGetValue(property, out string idParameterName))
                    {
                        segments.Add($"{{{idParameterName}}}");
                    }

                    // If this segment requires a typecast, add it to the route
                    if (this._typeCastParameters.TryGetValue(property, out string typeCastParameterName))
                    {
                        segments.Add($"{{{typeCastParameterName}}}");
                    }
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
                .Select(property => property.Name.Pascalize());

            string result = string.Join("\\", segments);

            return result;
        }

        /// <summary>
        /// Generates a cmdlet name's noun for the resource that can be found at the given ODataRoute.
        /// </summary>
        /// <returns>The cmdlet name.</returns>
        public string ToCmdletNameNounString(params string[] postfixSegments)
        {
            // Get all the segments in order
            string lastSegment = this.Segments.Last().Name;
            IEnumerable<string> segments = this.Segments
                // Convert name to pascal case
                .Select(property => property.Name.Pascalize())
                // Convert the postfix segments to pascal case and append them
                .Concat(postfixSegments.Select(segment => segment.Pascalize()));

            // Join the segments with underscores
            string result = string.Concat(segments);

            return result;
        }

        /// <summary>
        /// Generates a cmdlet name's noun for the referenced resource that can be found at the given ODataRoute.
        /// </summary>
        /// <returns>The cmdlet name.</returns>
        public string ToCmdletNameNounStringForReference(params string[] postfixSegments)
        {
            string result = this.ToCmdletNameNounString(postfixSegments);
            if (this.Segments.Last().IsCollection)
            {
                result += "References";
            }
            else
            {
                result += "Reference";
            }

            return result;
        }

        /// <summary>
        /// Generates a cmdlet name's noun for the referenced resource that can be found at the given ODataRoute.
        /// </summary>
        /// <returns>The cmdlet name.</returns>
        public string ToCmdletNameNounStringForStream(params string[] postfixSegments)
        {
            string result = this.ToCmdletNameNounString(postfixSegments);
            result += "Data";

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
    }
}
