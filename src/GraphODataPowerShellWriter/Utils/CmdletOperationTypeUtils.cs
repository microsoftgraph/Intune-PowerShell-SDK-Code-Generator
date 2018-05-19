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
                case CmdletOperationType.Get: return nameof(GetCmdlet);
                case CmdletOperationType.GetOrSearch: return nameof(GetOrSearchCmdlet);
                case CmdletOperationType.Post: return nameof(PostCmdlet);
                case CmdletOperationType.PutRefToSingleEntity: return nameof(PutReferenceToEntityCmdlet);
                case CmdletOperationType.PostRefToCollection: return nameof(PostReferenceToCollectionCmdlet);
                case CmdletOperationType.Patch: return nameof(PatchCmdlet);
                case CmdletOperationType.Delete: return nameof(DeleteCmdlet);
                case CmdletOperationType.FunctionReturningEntity: return nameof(FunctionReturningEntityCmdlet);
                case CmdletOperationType.FunctionReturningCollection: return nameof(FunctionReturningCollectionCmdlet);
                case CmdletOperationType.Action: return nameof(ActionCmdlet);
                default: throw new ArgumentException("Unknown operation type", nameof(operationType));
            }
        }
    }
}
