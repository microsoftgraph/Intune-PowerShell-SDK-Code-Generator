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

            // Create a stack to allow us to traverse the model in a depth-first manner.
            // Change this to a queue to traverse in a breadth-first manner.
            Stack<OdcmNode> unvisited = new Stack<OdcmNode>();
            unvisited.Push(root);
            
            // Continue adding to the tree until there are no more nodes to expand
            while (unvisited.Any())
            {
                // Get the next node to expand
                OdcmNode currentNode = unvisited.Pop();

                // Expand the node
                IEnumerable<OdcmNode> childNodes = currentNode.CreateChildNodes(model);

                // Mark the child nodes to be expanded
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
        /// <param name="currentNode">The node to expand</param>
        /// <param name="model">The ODCM model</param>
        /// <returns>The child nodes if the node can be expanded, otherwise an empty list.</returns>
        private static IEnumerable<OdcmNode> CreateChildNodes(this OdcmNode currentNode, OdcmModel model)
        {
            if (currentNode == null)
            {
                throw new ArgumentNullException(nameof(currentNode));
            }

            // Identify the kind of ODCM element this node represents and expand it if we need to
            OdcmObject obj = currentNode.OdcmObject;
            OdcmObjectType objType = obj.GetOdcmObjectType();
            switch (objType)
            {
                case OdcmObjectType.Class:
                    {
                        OdcmClass @class = obj as OdcmClass;

                        // Get the properties and wrap each one in an OdcmNode
                        foreach (OdcmProperty property in @class.Properties)
                        {
                            OdcmNode childNode = currentNode.CreateChildNode(property);
                            currentNode.Children.Add(childNode);
                            yield return childNode;
                        }
                    }
                    break;

                case OdcmObjectType.SingletonProperty:
                case OdcmObjectType.EntitySetProperty:
                    {
                        OdcmNode childNode = currentNode.CreateChildNode(obj);
                        currentNode.Children.Add(childNode);
                        yield return childNode;
                    }
                    break;

                case OdcmObjectType.Method:
                case OdcmObjectType.Enum:
                case OdcmObjectType.PrimitiveType:
                case OdcmObjectType.TypeDefinition:
                    {
                        // Nothing to return
                    }
                    break;

                default:
                    {
                        throw new ArgumentException($"Don't know how to handle ODCM object type: {obj.GetType()}", nameof(currentNode));
                    }
            }
        }
    }
}
