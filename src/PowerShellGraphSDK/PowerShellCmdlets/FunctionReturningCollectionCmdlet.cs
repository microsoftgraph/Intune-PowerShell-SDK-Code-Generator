// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    /// <summary>
    /// The common behavior between all OData PowerShell SDK cmdlets that call OData functions that return a collection of entities.
    /// </summary>
    public abstract class FunctionReturningCollectionCmdlet : GetOrSearchCmdlet
    {
        // TODO: Allow dynamic parameters once the generator supports them
        public override object GetDynamicParameters()
        {
            return null;
        }
    }
}