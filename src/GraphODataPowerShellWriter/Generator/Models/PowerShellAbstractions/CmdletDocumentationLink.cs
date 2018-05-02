// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;

    public class CmdletDocumentationLink
    {
        /// <summary>
        /// A descriptive name for the link.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The URL.
        /// </summary>
        public string Url { get; }

        public CmdletDocumentationLink(string name, string url)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Url = url ?? throw new ArgumentNullException(nameof(url));
        }
    }
}
