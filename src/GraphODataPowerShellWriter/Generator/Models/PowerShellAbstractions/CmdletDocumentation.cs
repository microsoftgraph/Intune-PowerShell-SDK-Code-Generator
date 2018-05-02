// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections.Generic;

    public class CmdletDocumentation
    {
        /// <summary>
        /// The synopsis that will appear when looking at the help documentation for this cmdlet.
        /// </summary>
        public string Synopsis { get; set; }

        /// <summary>
        /// The descriptions that will appear when looking at the help documentation for this cmdlet.
        /// 
        /// Each description will be in it's own paragraph.
        /// </summary>
        public IEnumerable<string> Descriptions { get; set; }

        /// <summary>
        /// The URLs that will appear when looking at the help documentation for this cmdlet.
        /// </summary>
        public IEnumerable<CmdletDocumentationLink> Links { get; set; }
    }
}
