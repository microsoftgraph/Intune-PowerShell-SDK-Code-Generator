// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    public abstract class ODataPowerShellSDKCmdlet : ODataPowerShellSDKCmdletBase
    {
        [Parameter]
        public string[] Select { get; set; }

        [Parameter]
        public string[] Expand { get; set; }

        internal override IDictionary<string, string> GetUrlQueryOptions()
        {
            IDictionary<string, string> queryOptions = base.GetUrlQueryOptions();
            if (Select != null && Select.Any())
            {
                queryOptions.Add("$select", string.Join(",", Select));
            }
            if (Expand != null && Expand.Any())
            {
                queryOptions.Add("$expand", string.Join(",", Expand));
            }

            return queryOptions;
        }
    }
}
