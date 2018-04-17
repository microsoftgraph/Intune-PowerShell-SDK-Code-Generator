// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

using System.Collections.Generic;
using System.Reflection;

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    /// <summary>
    /// The common behavior between all OData PowerShell SDK cmdlets that call actions on OData resources.
    /// </summary>
    public abstract class ODataActionPowerShellSDKCmdlet : ODataPowerShellSDKCmdletBase
    {
        public const string OperationName = "Action";

        internal override string GetHttpMethod()
        {
            return "POST";
        }

        internal override object GetContent()
        {
            // Get the properties that were set by the user in this invocation of the PowerShell cmdlet
            IEnumerable<PropertyInfo> boundProperties = this.GetBoundProperties(includeInherited: false);

            // Create a dictionary of the values for these properties
            IDictionary<string, object> content = new Dictionary<string, object>();
            foreach (PropertyInfo propInfo in boundProperties)
            {
                content.Add(propInfo.Name, propInfo.GetValue(this));
            }

            return content;
        }
    }
}
