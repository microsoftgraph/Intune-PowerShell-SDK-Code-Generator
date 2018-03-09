// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils;
    using Vipr.Core.CodeModel;

    public static class OdcmModelToTreeConversionBehavior
    {
        /// <summary>
        /// Converts the structure of an ODCM model into a tree and returns the root node of the tree.
        /// </summary>
        /// <param name="obj">The ODCM model to convert</param>
        /// <returns>The root node of the created tree.</returns>
        public static OdcmNode ConvertToOdcmTree(this OdcmModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // The EntityContainer in the model represents all of the data exposed through
            // the OData service, so treat this as the root of the tree
            OdcmNode root = new OdcmNode(model.EntityContainer);

            // Create a stack to allow us to traverse the model
            Stack<OdcmNode> unvisited = new Stack<OdcmNode>();

            // Mark the root node as "to be expanded"
            unvisited.Push(root);
            
            // Continue adding to the tree until there are no more nodes to expand
            while (unvisited.Any())
            {
                // Get the next node to expand
                OdcmNode currentNode = unvisited.Pop();

                // Expand the node
                IEnumerable<OdcmNode> childNodes = currentNode.CreateChildNodes(model);

                // Mark the child nodes as "to be expanded"
                foreach (OdcmNode childNode in childNodes)
                {
                    unvisited.Push(childNode);
                }
            }

            // Return the root node
            return root;
        }

        /// <summary>
        /// Expands a node into child nodes.
        /// </summary>
        /// <param name="node">The node to expand</param>
        /// <param name="model">The ODCM model</param>
        /// <returns>The child nodes if the node can be expanded, otherwise an empty list.</returns>
        private static IEnumerable<OdcmNode> CreateChildNodes(this OdcmNode node, OdcmModel model)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Identify the kind of ODCM element this node represents and expand it if we need to
            OdcmObject obj = node.OdcmObject;
            OdcmObjectType objType = obj.GetOdcmObjectType();
            IEnumerable<OdcmObject> childObjects;
            switch (objType)
            {
                // Classes
                case OdcmObjectType.Class:
                case OdcmObjectType.ComplexClass:
                case OdcmObjectType.EntityClass:
                case OdcmObjectType.ServiceClass:
                    {
                        OdcmClass @class = obj as OdcmClass;
                        childObjects = @class.GetChildObjects(model);
                    }
                    break;

                // Properties
                case OdcmObjectType.Property:
                case OdcmObjectType.SingletonProperty:
                case OdcmObjectType.EntitySetProperty:
                    {
                        OdcmProperty property = obj as OdcmProperty;
                        childObjects = property.GetChildObjects(model);
                    }
                    break;

                // Types that cannot be expanded
                case OdcmObjectType.Method:
                case OdcmObjectType.Enum:
                case OdcmObjectType.PrimitiveType:
                case OdcmObjectType.TypeDefinition:
                    {
                        // Nothing to return
                        return Enumerable.Empty<OdcmNode>();
                    }

                default:
                    {
                        throw new ArgumentException($"Don't know how to handle ODCM object type: {obj.GetType()}", nameof(node));
                    }
            }

            ICollection<string> parents = new HashSet<string>();
            OdcmNode currentNode = node;
            while (currentNode != null)
            {
                parents.Add(currentNode.OdcmObject.CanonicalName());
                currentNode = currentNode.Parent;
            }

            return childObjects
                // Filter out the children we've already seen so we can avoid loops
                .Where(child => !parents.Contains(child.CanonicalName()))
                // Add the child nodes to the current node
                .Select(child => node.CreateAndAddChildNode(child));
        }

        /// <summary>
        /// Gets child ODCM objects for an ODCM class.
        /// </summary>
        /// <param name="class">The ODCM class</param>
        /// <param name="model">The ODCM model</param>
        /// <returns>The child ODCM objects for the given ODCM class.</returns>
        private static IEnumerable<OdcmObject> GetChildObjects(this OdcmClass @class, OdcmModel model)
        {
            // Return the properties of the class
            foreach (OdcmProperty property in @class.Properties)
            {
                yield return property;
            }
        }

        /// <summary>
        /// Gets child ODCM objects for an ODCM property.
        /// </summary>
        /// <param name="class">The ODCM property</param>
        /// <param name="model">The ODCM model</param>
        /// <returns>The child ODCM objects for the given ODCM property.</returns>
        private static IEnumerable<OdcmObject> GetChildObjects(this OdcmProperty property, OdcmModel model)
        {
            // Get the property's type and expand it to get it's properties
            OdcmType propertyType = property.Type;
            if (propertyType is OdcmClass @class)
            {
                // Return this class' properties
                return @class.GetChildObjects(model);
            }
            else
            {
                // Return nothing
                return Enumerable.Empty<OdcmObject>();
            }
        }
    }
}
