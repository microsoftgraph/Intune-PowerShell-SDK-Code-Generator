// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections.Generic;
    using Vipr.Core.CodeModel;

    public class OdcmNode
    {
        public OdcmNode Parent { get; }

        public ICollection<OdcmNode> Children { get; } = new List<OdcmNode>();

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

        public OdcmNode CreateChildNode(OdcmObject childData)
        {
            if (childData == null)
            {
                throw new ArgumentNullException(nameof(childData));
            }

            return new OdcmNode(this, childData);
        }
    }
}
