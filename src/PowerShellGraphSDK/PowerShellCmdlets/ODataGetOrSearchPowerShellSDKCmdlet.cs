// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    /// <summary>
    /// The common behavior between all OData PowerShell SDK cmdlets that support
    /// $select, $expand, $filter, $orderBy, $skip and $top query parameters.
    /// </summary>
    public abstract class ODataGetOrSearchPowerShellSDKCmdlet : ODataGetPowerShellSDKCmdlet
    {
        public const string OperationName = "Search";

        [Parameter(ParameterSetName = ODataGetOrSearchPowerShellSDKCmdlet.OperationName)]
        public string Filter { get; set; }

        [Parameter(ParameterSetName = ODataGetOrSearchPowerShellSDKCmdlet.OperationName)]
        public string[] OrderBy { get; set; }

        [Parameter(ParameterSetName = ODataGetOrSearchPowerShellSDKCmdlet.OperationName)]
        public int? Skip { get; set; }

        [Parameter(ParameterSetName = ODataGetOrSearchPowerShellSDKCmdlet.OperationName)]
        [Alias("First")] // Required to be compatible with the PowerShell paging parameters
        public int? Top { get; set; }

        internal override IDictionary<string, string> GetUrlQueryOptions()
        {
            IDictionary<string, string> queryOptions = base.GetUrlQueryOptions();
            if (!string.IsNullOrEmpty(Filter))
            {
                queryOptions.Add("$filter", this.Filter);
            }
            if (OrderBy != null && OrderBy.Any())
            {
                queryOptions.Add("$orderBy", string.Join(",", OrderBy));
            }
            if (Skip != null)
            {
                queryOptions.Add("$skip", Skip.ToString());
            }
            if (Top != null)
            {
                queryOptions.Add("$top", Top.ToString());
            }

            return queryOptions;
        }

        internal override PSObject ReadResponse(string content)
        {
            object result = base.ReadResponse(content);
            // If this result is for a SEARCH call and there is only 1 page in the result, return only the result objects
            if (result is PSObject response &&
                this.ParameterSetName == OperationName &&
                response.Members.Any(member => member.Name == "value") &&
                !response.Members.Any(member => member.Name == "@odata.nextLink"))
            {
                result = response.Members["value"].Value;
            }

            return PSObject.AsPSObject(result);
        }
    }
}
