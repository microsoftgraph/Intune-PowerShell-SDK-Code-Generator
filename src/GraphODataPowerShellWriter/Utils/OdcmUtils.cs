// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Vipr.Core.CodeModel;

    public static class OdcmUtils
    {
        /// <summary>
        /// Evaluates the OData route which leads to the given node.
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The OData path.</returns>
        public static string EvaluateODataRoute(this OdcmNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            // Track the segments in the route - use a stack so we don't have to reverse the list later
            Stack<string> segments = new Stack<string>();

            // Iterate over the node's parents
            OdcmNode currentNode = node;
            while (currentNode != null)
            {
                // If this node represents a collection, add a segment for the ID
                if (currentNode.OdcmProperty.IsEnumeration())
                {
                    segments.Push("{id}");
                }

                // Add this node to the route
                segments.Push(currentNode.OdcmProperty.Name);

                // Get the parent node
                currentNode = currentNode.Parent;
            }

            // Join the route segments
            string result = string.Join("/", segments);

            return result;
        }

        /// <summary>
        /// Determines whether or not a given property represents a collection.
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>True if the property represents a collection, false otherwise.</returns>
        public static bool IsEnumeration(this OdcmProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return property.IsCollection;
        }

        /// <summary>
        /// Gets all properties for a type, including those inherited from base types.
        /// The values are the properties and the keys are the types they came from.
        /// </summary>
        /// <param name="type">The type to evaluate</param>
        /// <returns>The type's immediate and inherited properties.  If the type is not a class, it will return an empty result.</returns>
        public static IEnumerable<OdcmProperty> EvaluateProperties(this OdcmType type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // Track the properties
            // NOTE: Overridden properties in subtypes is not allowed in OData v4, so we don't need to worry about duplicated properties
            IList<OdcmProperty> properties = new List<OdcmProperty>();

            // If the type is not a class, there are no properties to return
            if (type is OdcmClass @class)
            {
                List<OdcmClass> classes = new List<OdcmClass>();
                classes.Add(@class);
                classes.AddRange(@class.GetBaseTypes());

                // Iterate over the type and its base types
                foreach (OdcmClass currentClass in classes)
                {
                    // Add the immediate properties
                    foreach (OdcmProperty property in currentClass.Properties)
                    {
                        properties.Add(property);
                    }
                }
            }

            return properties;
        }

        /// <summary>
        /// Gets the immediate base type.  Use <see cref="GetBaseTypes(OdcmType)"/> to get the full chain of base types.
        /// </summary>
        /// <param name="type">The type to evaluate</param>
        /// <returns>The type's immediate base type.</returns>
        public static OdcmClass GetBaseType(this OdcmType type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type is OdcmClass @class)
            {
                return @class.Base;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the chain of base types for a given type.
        /// The first item in the chain is the type that the given type directly inherits from.
        /// </summary>
        /// <param name="type">The type to evaluate</param>
        /// <returns>The chain of base types.</returns>
        public static IEnumerable<OdcmClass> GetBaseTypes(this OdcmType type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type is OdcmClass @class)
            {
                ICollection<OdcmClass> result = new List<OdcmClass>();
                OdcmClass currentClass = @class.Base;
                while (currentClass != null)
                {
                    result.Add(currentClass);
                    currentClass = currentClass.Base;
                }

                return result;
            }
            else
            {
                return Enumerable.Empty<OdcmClass>();
            }
        }

        /// <summary>
        /// Gets the given type's derived types.
        /// </summary>
        /// <param name="type">The type to evaluate</param>
        /// <returns>The derived types.</returns>
        public static IEnumerable<OdcmClass> GetDerivedTypes(this OdcmType type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type is OdcmClass @class)
            {
                return @class.Derived;
            }
            else
            {
                return Enumerable.Empty<OdcmClass>();
            }
        }
    }
}
