// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Management.Automation;
    using System.Net.Http.Headers;
    using System.Reflection;

    /// <summary>
    /// The common behavior between all OData PowerShell SDK cmdlets that support
    /// $select, $expand, $filter, $orderBy, $skip and $top query parameters.
    /// </summary>
    public abstract class GetOrSearchCmdlet : GetCmdlet
    {
        /// <summary>
        /// The operation name.
        /// </summary>
        public new const string OperationName = "Search";

        /// <summary>
        /// <para type="description">The "$filter" query option value.</para>
        /// </summary>
        [Parameter(ParameterSetName = GetOrSearchCmdlet.OperationName)]
        public string Filter { get; set; }

        /// <summary>
        /// The list of "$orderBy" query option values (i.e. property names).
        /// 
        /// This value is declared as a dynamic parameter so that values can be validated per cmdlet.
        /// </summary>
        public string[] OrderBy = null;

        /// <summary>
        /// <para type="description">The "$skip" query option value.</para>
        /// </summary>
        [Parameter(ParameterSetName = GetOrSearchCmdlet.OperationName)]
        public int? Skip { get; set; }

        /// <summary>
        /// <para type="description">The "$top" query option value.</para>
        /// </summary>
        [Parameter(ParameterSetName = GetOrSearchCmdlet.OperationName)]
        [Alias("First")] // Required to be compatible with the PowerShell paging parameters
        public int? Top { get; set; }

        /// <summary>
        /// <para type="description">The "Prefer: odata.maxpagesize" header value.</para>
        /// </summary>
        [Parameter(ParameterSetName = GetOrSearchCmdlet.OperationName)]
        public int? MaxPageSize { get; set; }

        /// <summary>
        /// Creates a new <see cref="GetOrSearchCmdlet"/>.
        /// </summary>
        public GetOrSearchCmdlet()
        {
            // Get the properties
            IEnumerable<PropertyInfo> properties = this.GetProperties(false);

            // Create the "OrderBy" parameter
            IEnumerable<string> orderByValidValues = properties
                .Where(param => Attribute.IsDefined(param, typeof(SortableAttribute)))
                .Select(param => param.Name);
            if (orderByValidValues.Any())
            {
                // Create the collection of attributes
                var orderByParameter = new RuntimeDefinedParameter(
                    nameof(this.OrderBy),
                    typeof(string[]),
                    new Collection<Attribute>()
                    {
                        new ParameterAttribute() { ParameterSetName = GetOrSearchCmdlet.OperationName },
                        new ValidateSetAttribute(orderByValidValues.ToArray()),
                    });

                // Add to the dictionary of dynamic parameters
                this.DynamicParameters?.Add(nameof(this.OrderBy), orderByParameter);
            }
        }

        /// <summary>
        /// Set up the dynamic parameters.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (this.DynamicParameters != null)
            {
                // OrderBy
                if (this.DynamicParameters.TryGetValue(nameof(this.OrderBy), out RuntimeDefinedParameter selectParam)
                    && selectParam.IsSet)
                {
                    this.OrderBy = selectParam.Value as string[];
                }
            }
        }

        internal override IDictionary<string, string> GetUrlQueryOptions()
        {
            IDictionary<string, string> queryOptions = base.GetUrlQueryOptions();

            // OrderBy
            if (this.OrderBy != null && this.OrderBy.Any())
            {
                IEnumerable<string> sortable = this.OrderBy.Select(param => this.TypeCastMappings[param]);
                queryOptions.Add(ODataConstants.QueryParameters.OrderBy, string.Join(",", sortable));
            }

            // Filter
            if (!string.IsNullOrEmpty(this.Filter))
            {
                queryOptions.Add(ODataConstants.QueryParameters.Filter, this.Filter);
            }

            // Skip
            if (this.Skip != null)
            {
                queryOptions.Add(ODataConstants.QueryParameters.Skip, this.Skip.ToString());
            }

            // Top
            if (this.Top != null)
            {
                queryOptions.Add(ODataConstants.QueryParameters.Top, this.Top.ToString());
            }

            return queryOptions;
        }

        internal override HttpRequestHeaders GetHeaders()
        {
            HttpRequestHeaders headers = base.GetHeaders();
            if (this.MaxPageSize != null)
            {
                headers.Add("Prefer", $"odata.maxpagesize={this.MaxPageSize}");
            }

            return headers;
        }
    }
}
