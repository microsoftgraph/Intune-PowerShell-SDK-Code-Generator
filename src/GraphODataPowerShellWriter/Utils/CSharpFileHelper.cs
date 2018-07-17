// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    public static class CSharpFileHelper
    {
        public static string GetLicenseHeader() => @"// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.";

        public static string GetDefaultNamespace() => "Microsoft.Intune.PowerShellGraphSDK.PowerShellCmdlets";

        public static string[] GetDefaultUsings() => defaultUsings;
        private static readonly string[] defaultUsings =
        {
            "System.Management.Automation",
        };
    }
}
