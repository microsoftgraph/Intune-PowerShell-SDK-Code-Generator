// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System.Collections.Generic;

    public class CmdletParameterDocumentation
    {
        /// <summary>
        /// The descriptions that appear when looking at the help documentation for this cmdlet.
        /// 
        /// Each description will be in it's own paragraph.
        /// </summary>
        public IEnumerable<string> Descriptions { get; set; }

        /// <summary>
        /// The set of valid values (e.g. for an enum).
        /// </summary>
        public IEnumerable<string> ValidValues { get; set; }
    }
}
