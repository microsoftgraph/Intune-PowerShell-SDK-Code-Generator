// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// The common behavior between all OData PowerShell SDK cmdlets that call actions on OData resources.
    /// </summary>
    public abstract class ActionCmdlet : ODataPowerShellSDKCmdletBase
    {
        public const string OperationName = "Action";

        internal override string GetHttpMethod()
        {
            return "POST";
        }

        internal override object GetContent()
        {
            IDictionary<string, object> content = this.CreateDictionaryFromBoundProperties();

            return content;
        }
    }
}
