// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    public class ObjectFactoryCmdlet : Cmdlet
    {
        public string RelativeFilePath { get; set; }

        public string ResourceTypeFullName { get; set; }

        public ObjectFactoryCmdlet(string verb, string noun) : base(verb, noun) { }

        public ObjectFactoryCmdlet(CmdletName cmdletName) : base(cmdletName) { }
    }
}
