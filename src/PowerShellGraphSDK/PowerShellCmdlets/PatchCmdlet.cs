﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    /// <summary>
    /// The common behavior between all OData PowerShell SDK cmdlets that update OData resources.
    /// </summary>
    public abstract class PatchCmdlet : PostOrPatchCmdlet
    {
        /// <summary>
        /// The operation name.
        /// </summary>
        public const string OperationName = "Patch";

        internal override string GetHttpMethod()
        {
            return "PATCH";
        }
    }
}
