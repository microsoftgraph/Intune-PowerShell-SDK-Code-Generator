// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Management.Automation;
    using System.Net;
    using System.Reflection;

    /// <summary>
    /// The common behavior between all OData PowerShell SDK cmdlets that support $select and $expand query parameters.
    /// </summary>
    public abstract class GetCmdlet : ODataPowerShellSDKCmdletBase
    {
        /// <summary>
        /// The operation name.
        /// </summary>
        public const string OperationName = "Get";

        /// <summary>
        /// Mapping between parameter names and their type cast.
        /// </summary>
        protected IDictionary<string, string> TypeCastMappings = new Dictionary<string, string>();

        /// <summary>
        /// The list of $select query option values (i.e. property names).
        /// 
        /// This value is declared as a dynamic parameter so that values can be validated per cmdlet.
        /// </summary>
        private string[] Select = null;

        /// <summary>
        /// The list of $expand query option values (i.e. property names).
        /// 
        /// This value is declared as a dynamic parameter so that values can be validated per cmdlet.
        /// </summary>
        public string[] Expand = null;

        #region Setup

        /// <summary>
        /// Creates a new instance of <see cref="GetCmdlet"/>.
        /// </summary>
        public GetCmdlet()
        {
            // Get the properties
            IEnumerable<PropertyInfo> properties = this.GetProperties(false);

            // Set up type casts
            foreach (PropertyInfo prop in properties)
            {
                // Store the mapping between the parameter name and its type cast if necessary
                this.TypeCastMappings.Add(prop.Name, prop.Name);
                if (Attribute.IsDefined(prop, typeof(DerivedTypeAttribute)))
                {
                    DerivedTypeAttribute selectableAttr = prop.GetCustomAttribute<DerivedTypeAttribute>(false);
                    if (!string.IsNullOrWhiteSpace(selectableAttr.FullName))
                    {
                        this.TypeCastMappings[prop.Name] = $"{selectableAttr.FullName}/{prop.Name}";
                    }
                }
            }

            // Create the "Select" parameter if required
            IEnumerable<string> selectValidValues = properties
                .Where(param => Attribute.IsDefined(param, typeof(SelectableAttribute)))
                .Select(param => param.Name);
            if (selectValidValues.Any())
            {
                var selectParameter = new RuntimeDefinedParameter(
                    nameof(this.Select),
                    typeof(string[]),
                    new Collection<Attribute>()
                    {
                        new ParameterAttribute() { ParameterSetName = GetCmdlet.OperationName },
                        new ParameterAttribute() { ParameterSetName = GetOrSearchCmdlet.OperationName },
                        new ValidateSetAttribute(selectValidValues.ToArray()),
                    });

                // Add to the dictionary of dynamic parameters
                this.DynamicParameters?.Add(nameof(this.Select), selectParameter);
            }

            // Create the "Expand" parameter if required
            IEnumerable<string> expandValidValues = properties
                .Where(param => Attribute.IsDefined(param, typeof(ExpandableAttribute)))
                .Select(param => param.Name);
            if (expandValidValues.Any())
            {
                var expandParameter = new RuntimeDefinedParameter(
                    nameof(this.Expand),
                    typeof(string[]),
                    new Collection<Attribute>()
                    {
                        new ParameterAttribute() { ParameterSetName = GetCmdlet.OperationName },
                        new ParameterAttribute() { ParameterSetName = GetOrSearchCmdlet.OperationName },
                        new ValidateSetAttribute(expandValidValues.ToArray()),
                    });

                // Add to the dictionary of dynamic parameters
                this.DynamicParameters?.Add(nameof(this.Expand), expandParameter);
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
                // Select
                if (this.DynamicParameters.TryGetValue(nameof(Select), out RuntimeDefinedParameter selectParam)
                    && selectParam.IsSet)
                {
                    this.Select = selectParam.Value as string[];
                }

                // Expand
                if (this.DynamicParameters.TryGetValue(nameof(Expand), out RuntimeDefinedParameter expandParam)
                    && expandParam.IsSet)
                {
                    this.Expand = expandParam.Value as string[];
                }
            }
        }

        #endregion Setup

        internal override string GetHttpMethod()
        {
            return "GET";
        }

        internal override IDictionary<string, string> GetUrlQueryOptions()
        {
            IDictionary<string, string> queryOptions = base.GetUrlQueryOptions();

            // Select
            if (this.Select != null && this.Select.Any())
            {
                IEnumerable<string> selectable = this.Select.Select(param => this.TypeCastMappings[param]);
                queryOptions.Add(ODataConstants.QueryParameters.Select, string.Join(",", selectable));
            }

            // Expand
            if (Expand != null && Expand.Any())
            {
                IEnumerable<string> selectable = this.Expand.Select(param => this.TypeCastMappings[param]);
                queryOptions.Add(ODataConstants.QueryParameters.Expand, string.Join(",", this.Expand));
            }

            return queryOptions;
        }

        internal override object ReadResponse(string content)
        {
            // Convert the string content into a C# object
            object result = base.ReadResponse(content);

            // If this is the final page of a SEARCH result, unwrap and return just the values
            if (// Make sure that this is a JSON response
                result is PSObject response
                // Make sure that the "@odata.context" property exists (to make sure that this is an OData response)
                && response.Members.Any(member => member.Name == ODataConstants.SearchResultProperties.Context)
                // Make sure that there is no nextLink (i.e. there is only 1 page of results)
                && !response.Members.Any(member => member.Name == ODataConstants.SearchResultProperties.NextLink)
                // Make sure that this is for a collection result
                && ((this is GetOrSearchCmdlet && this.ParameterSetName == GetOrSearchCmdlet.OperationName) || this is FunctionReturningCollectionCmdlet))
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

        #region Helpers

        /// <summary>
        /// Creates the URL segment containing the function name and arguments.
        /// </summary>
        /// <returns>The URL segment containing the function name and arguments.</returns>
        public string GetFunctionUrlSegment()
        {
            Type cmdletType = this.GetType();
            if (!(this is FunctionReturningEntityCmdlet) &&
                !(this is FunctionReturningCollectionCmdlet))
            {
                throw new PSArgumentException($"Cannot call method '{nameof(GetFunctionUrlSegment)}()' on a cmdlet of type '{cmdletType}'.");
            }

            // Create the list of arguments
            IEnumerable<string> paramArguments = this.GetBoundProperties(false).Select(param =>
            {
                object paramValue = param.GetValue(this);
                Type paramType = param.PropertyType;
                string oDataType = param.GetCustomAttribute<ODataTypeAttribute>()?.FullName;

                // Check if we need special handling of the value based on the parameter type
                string paramArgumentValue = paramValue.ToODataString(oDataType, isArray: paramType.IsArray, isUrlValue: true);

                // Create the parameter mapping
                return $"{param.Name}={WebUtility.UrlEncode(paramArgumentValue)}";
            });

            // Join the list of arguments
            string result = string.Join(",", paramArguments);

            return result;
        }

        #endregion Helpers
    }
}
