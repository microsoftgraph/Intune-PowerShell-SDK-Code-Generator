// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using PowerShellGraphSDK.PowerShellCmdlets;

    public static class CmdletOperationTypeUtils
    {
        public static string ToCSharpString(this CmdletOperationType operationType)
        {
            switch (operationType)
            {
                case CmdletOperationType.Get: return nameof(ODataGetPowerShellSDKCmdlet);
                case CmdletOperationType.GetOrSearch: return nameof(ODataGetOrSearchPowerShellSDKCmdlet);
                case CmdletOperationType.Post: return nameof(ODataPostPowerShellSDKCmdlet);
                case CmdletOperationType.Patch: return nameof(ODataPatchPowerShellSDKCmdlet);
                case CmdletOperationType.Delete: return nameof(ODataDeletePowerShellSDKCmdlet);
                case CmdletOperationType.Function: return nameof(ODataFunctionPowerShellSDKCmdlet);
                case CmdletOperationType.Action: return nameof(ODataActionPowerShellSDKCmdlet);
                default: throw new ArgumentException("Unknown operation type", nameof(operationType));
            }
        }
    }
}
