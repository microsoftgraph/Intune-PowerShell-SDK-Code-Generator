// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections.Generic;
    using Vipr.Core.CodeModel;

    public class OdcmNode
    {
        public OdcmNode Parent { get; }

        private List<OdcmNode> _children = new List<OdcmNode>();
        public IReadOnlyCollection<OdcmNode> Children => _children.AsReadOnly();

        public bool IsRootNode => this.Parent == null;

        public OdcmObject OdcmObject { get; }

        /// <summary>
        /// Creates a root node for an ODCM tree.
        /// </summary>
        /// <param name="entityContainer">The entity container for the ODCM model</param>
        public OdcmNode(OdcmClass entityContainer)
        {
            this.OdcmObject = entityContainer ?? throw new ArgumentNullException(nameof(entityContainer));
        }

        /// <summary>
        /// Creates an OdcmNode.
        /// </summary>
        /// <param name="parent">The parent node</param>
        /// <param name="odcmObject">The ODCM object which this node represents</param>
        public OdcmNode(OdcmNode parent, OdcmObject odcmObject)
        {
            this.Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this.OdcmObject = odcmObject ?? throw new ArgumentNullException(nameof(odcmObject));
        }

        /// <summary>
        /// Creates and adds a child to this node.
        /// </summary>
        /// <param name="childData">The ODCM object to be used as the child</param>
        /// <returns>The created child node.</returns>
        public OdcmNode CreateAndAddChildNode(OdcmObject childData)
        {
            if (childData == null)
            {
                throw new ArgumentNullException(nameof(childData));
            }

            OdcmNode childNode = new OdcmNode(this, childData);
            this._children.Add(childNode);

            return childNode;
        }

        /// <summary>
        /// Gets the hash code for the ODCM object in this node.
        /// </summary>
        /// <returns>The hash code for the ODCM object.</returns>
        public override int GetHashCode()
        {
            return OdcmObject == null ? 0 : OdcmObject.GetHashCode();
        }
    }
}
