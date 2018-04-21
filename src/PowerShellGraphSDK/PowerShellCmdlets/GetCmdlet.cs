// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Management.Automation;

    /// <summary>
    /// The common behavior between all OData PowerShell SDK cmdlets that support $select and $expand query parameters.
    /// </summary>
    public abstract class GetCmdlet : ODataPowerShellSDKCmdletBase, IDynamicParameters
    {
        public const string OperationName = "Get";

        private const string SelectParameterName = "Select";

        private RuntimeDefinedParameterDictionary DynamicParameters => this.GetDynamicParameters() as RuntimeDefinedParameterDictionary;

        /// <summary>
        /// The list of $select query option values (i.e. property names).
        /// 
        /// This value is declared as a dynamic parameter so that values can be validated per cmdlet.
        /// </summary>
        private string[] Select = null;

        /// <summary>
        /// The list of $expand query option values (i.e. property names).
        /// </summary>
        [Parameter(ParameterSetName = GetCmdlet.OperationName)]
        [Parameter(ParameterSetName = GetOrSearchCmdlet.OperationName)]
        public string[] Expand { get; set; }

        /// <summary>
        /// Set up the dynamic parameters.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (this.DynamicParameters?.ContainsKey(SelectParameterName) == true
                    && this.DynamicParameters[SelectParameterName].IsSet)
            {
                this.Select = this.DynamicParameters[SelectParameterName].Value as string[];
            }
        }

        /// <summary>
        /// The parameters that are added at runtime.
        /// </summary>
        /// <returns>A <see cref="RuntimeDefinedParameterDictionary"/>.</returns>
        public virtual object GetDynamicParameters()
        {
            // Get the names of the properties on this cmdlet
            IEnumerable<string> properties = this.GetProperties(false, null).Select(param => param.Name).Distinct();

            // Create the "Select" parameter
            var validateSetAttribute = new ValidateSetAttribute(properties.ToArray());
            var selectParameter = new RuntimeDefinedParameter(
                SelectParameterName,
                typeof(string[]),
                new Collection<Attribute>()
                {
                    new ParameterAttribute() { ParameterSetName = GetCmdlet.OperationName },
                    new ParameterAttribute() { ParameterSetName = GetOrSearchCmdlet.OperationName },
                    validateSetAttribute,
                });

            // Create the dictionary of dynamic parameters
            var parameterDictionary = new RuntimeDefinedParameterDictionary()
            {
                { SelectParameterName, selectParameter },
            };

            return parameterDictionary;
        }

        internal override string GetHttpMethod()
        {
            return "GET";
        }

        internal override IDictionary<string, string> GetUrlQueryOptions()
        {
            IDictionary<string, string> queryOptions = base.GetUrlQueryOptions();
            if (Select != null && Select.Any())
            {
                queryOptions.Add(ODataConstants.QueryParameters.Select, string.Join(",", Select));
            }
            if (Expand != null && Expand.Any())
            {
                queryOptions.Add(ODataConstants.QueryParameters.Expand, string.Join(",", Expand));
            }

            return queryOptions;
        }
    }
}
