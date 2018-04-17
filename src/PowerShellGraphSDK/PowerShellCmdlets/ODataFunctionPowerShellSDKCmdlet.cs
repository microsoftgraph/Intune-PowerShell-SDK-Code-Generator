// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    /// <summary>
    /// The common behavior between all OData PowerShell SDK cmdlets that call actions on OData resources.
    /// </summary>
    public abstract class ODataFunctionPowerShellSDKCmdlet : ODataGetPowerShellSDKCmdlet
    {
        public const string OperationName = "Function";
    }
}
