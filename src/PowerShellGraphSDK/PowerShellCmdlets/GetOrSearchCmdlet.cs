// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Management.Automation;
    using System.Reflection;

    /// <summary>
    /// The common behavior between all OData PowerShell SDK cmdlets that support
    /// $select, $expand, $filter, $orderBy, $skip and $top query parameters.
    /// </summary>
    public abstract class GetOrSearchCmdlet : GetCmdlet
    {
        public new const string OperationName = "Search";

        /// <summary>
        /// The $filter query option value.
        /// </summary>
        [Parameter(ParameterSetName = GetOrSearchCmdlet.OperationName)]
        public string Filter { get; set; }

        /// <summary>
        /// The list of $orderBy query option values (i.e. property names).
        /// 
        /// This value is declared as a dynamic parameter so that values can be validated per cmdlet.
        /// </summary>
        public string[] OrderBy = null;

        /// <summary>
        /// The $skip query option value.
        /// </summary>
        [Parameter(ParameterSetName = GetOrSearchCmdlet.OperationName)]
        public int? Skip { get; set; }

        /// <summary>
        /// The $top query option value.
        /// </summary>
        [Parameter(ParameterSetName = GetOrSearchCmdlet.OperationName)]
        [Alias("First")] // Required to be compatible with the PowerShell paging parameters
        public int? Top { get; set; }

        /// <summary>
        /// Set up the dynamic parameters.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (this.DynamicParameters != null)
            {
                // OrderBy
                if (this.DynamicParameters.TryGetValue(nameof(OrderBy), out RuntimeDefinedParameter selectParam)
                    && selectParam.IsSet)
                {
                    this.OrderBy = selectParam.Value as string[];
                }
            }
        }
        
        public override object GetDynamicParameters()
        {
            // Get the dynamic parameters from the base class
            RuntimeDefinedParameterDictionary parameterDictionary = (RuntimeDefinedParameterDictionary)base.GetDynamicParameters();

            // Get the properties
            IEnumerable<PropertyInfo> properties = this.GetProperties(false);

            // Create the "OrderBy" parameter
            var validateSetAttributeOrderBy = new ValidateSetAttribute(properties
                .Where(param => Attribute.IsDefined(param, typeof(SortableAttribute)))
                .Select(param => param.Name)
                .Distinct()
                .ToArray());
            var orderByParameter = new RuntimeDefinedParameter(
                nameof(OrderBy),
                typeof(string[]),
                new Collection<Attribute>()
                {
                    new ParameterAttribute() { ParameterSetName = GetOrSearchCmdlet.OperationName },
                    validateSetAttributeOrderBy,
                });

            // Add to the dictionary of dynamic parameters
            parameterDictionary.Add(nameof(OrderBy), orderByParameter);

            return parameterDictionary;
        }

        internal override IDictionary<string, string> GetUrlQueryOptions()
        {
            IDictionary<string, string> queryOptions = base.GetUrlQueryOptions();
            if (!string.IsNullOrEmpty(Filter))
            {
                queryOptions.Add(ODataConstants.QueryParameters.Filter, this.Filter);
            }
            if (OrderBy != null && OrderBy.Any())
            {
                queryOptions.Add(ODataConstants.QueryParameters.OrderBy, string.Join(",", OrderBy));
            }
            if (Skip != null)
            {
                queryOptions.Add(ODataConstants.QueryParameters.Skip, Skip.ToString());
            }
            if (Top != null)
            {
                queryOptions.Add(ODataConstants.QueryParameters.Top, Top.ToString());
            }

            return queryOptions;
        }

        internal override object ReadResponse(string content)
        {
            // Convert the string content into a C# object
            object result = base.ReadResponse(content);

            // If this is the final page of a SEARCH result, unwrap and return just the values
            if (// Make sure that this result is for a SEARCH call
                this.ParameterSetName == GetOrSearchCmdlet.OperationName
                // Make sure that this is a JSON response
                && result is PSObject response
                // Make sure that the "@odata.context" property exists (to make sure that this is an OData response)
                && response.Members.Any(member => member.Name == ODataConstants.SearchResultProperties.Context)
                // Make sure that there is no nextLink (i.e. there is only 1 page of results)
                && !response.Members.Any(member => member.Name == ODataConstants.SearchResultProperties.NextLink))
            {
                // Check if there were any values in the page of results
                if (response.Members.Any(member => member.Name == ODataConstants.SearchResultProperties.Value))
                {
                    // There were values in the page, so unwrap and return them
                    result = response.Members[ODataConstants.SearchResultProperties.Value].Value;
                    return result;
                }
                else
                {
                    // There were no values in the page
                    return null;
                }
            }
            else
            {
                // If this is a GET result or a SEARCH result with multiple pages, return the result as-is
                return result;
            }
        }
    }
}
