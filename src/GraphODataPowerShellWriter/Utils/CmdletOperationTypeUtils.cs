// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;

    public static class CmdletOperationTypeUtils
    {
        public static string ToCSharpString(this CmdletOperationType operationType)
        {
            switch (operationType)
            {
                case CmdletOperationType.Get: return "ODataGetPowerShellSDKCmdlet";
                case CmdletOperationType.GetOrSearch: return "ODataGetOrSearchPowerShellSDKCmdlet";
                case CmdletOperationType.Post: return "ODataPostPowerShellSDKCmdlet";
                case CmdletOperationType.Patch: return "ODataPatchPowerShellSDKCmdlet";
                case CmdletOperationType.Delete: return "ODataDeletePowerShellSDKCmdlet";
                default: throw new ArgumentException("Unknown operation type", nameof(operationType));
            }
        }
    }
}
