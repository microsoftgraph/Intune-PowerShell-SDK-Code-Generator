// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// The common behavior between all OData PowerShell SDK cmdlets that call OData functions.
    /// </summary>
    public abstract class FunctionCmdlet : GetCmdlet
    {
        internal override object GetContent()
        {
            return this.CreateDictionaryFromBoundProperties();
        }
    }
}
