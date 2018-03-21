// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using Vipr.Core.CodeModel;

    /// <summary>
    /// Represents the final segment of the path to an ODCM property from the entity container.
    /// Use the <see cref="Parent"/> property to access the previous segment.
    /// </summary>
    public class OdcmNode
    {
        /// <summary>
        /// The parent node.
        /// </summary>
        public OdcmNode Parent { get; }

        /// <summary>
        /// The ODCM property that this node represents.
        /// </summary>
        public OdcmProperty OdcmProperty { get; }

        /// <summary>
        /// Creates an ODCM node with no parent.
        /// </summary>
        /// <param name="entityContainer">The ODCM property that this node represents</param>
        public OdcmNode(OdcmProperty property)
        {
            this.OdcmProperty = property ?? throw new ArgumentNullException(nameof(property));
        }

        /// <summary>
        /// Creates an OdcmNode.
        /// </summary>
        /// <param name="parent">The parent node</param>
        /// <param name="odcmProperty">The ODCM object which this node represents</param>
        private OdcmNode(OdcmNode parent, OdcmProperty odcmProperty)
        {
            this.Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this.OdcmProperty = odcmProperty ?? throw new ArgumentNullException(nameof(odcmProperty));
        }

        /// <summary>
        /// Creates a child for this node.
        /// </summary>
        /// <param name="childData">The ODCM object to be used as the child</param>
        /// <returns>The created child node.</returns>
        public OdcmNode CreateChildNode(OdcmProperty childData)
        {
            if (childData == null)
            {
                throw new ArgumentNullException(nameof(childData));
            }

            OdcmNode childNode = new OdcmNode(this, childData);

            return childNode;
        }

        /// <summary>
        /// Gets the hash code for the ODCM object in this node.
        /// </summary>
        /// <returns>The hash code for the ODCM object.</returns>
        public override int GetHashCode()
        {
            return OdcmProperty == null ? 0 : OdcmProperty.GetHashCode();
        }
    }
}
