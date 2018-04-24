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
    /// The common behavior between all OData PowerShell SDK cmdlets that support $select and $expand query parameters.
    /// </summary>
    public abstract class GetCmdlet : ODataPowerShellSDKCmdletBase, IDynamicParameters
    {
        public const string OperationName = "Get";

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
                    this.Select = expandParam.Value as string[];
                }
            }
        }

        /// <summary>
        /// The parameters that are added at runtime.
        /// </summary>
        /// <returns>A <see cref="RuntimeDefinedParameterDictionary"/>.</returns>
        public override object GetDynamicParameters()
        {
            RuntimeDefinedParameterDictionary parameterDictionary = (RuntimeDefinedParameterDictionary)base.GetDynamicParameters();

            // Get the properties
            IEnumerable<PropertyInfo> properties = this.GetProperties(false);

            // Create the "Select" parameter
            var validateSetAttributeSelect = new ValidateSetAttribute(properties
                .Select(param => param.Name)
                .Distinct()
                .ToArray());
            var selectParameter = new RuntimeDefinedParameter(
                nameof(this.Select),
                typeof(string[]),
                new Collection<Attribute>()
                {
                    new ParameterAttribute() { ParameterSetName = GetCmdlet.OperationName },
                    new ParameterAttribute() { ParameterSetName = GetOrSearchCmdlet.OperationName },
                    validateSetAttributeSelect,
                });

            // Create the "Expand" parameter
            var validateSetAttributeExpand = new ValidateSetAttribute(properties
                .Where(param => Attribute.IsDefined(param, typeof(ExpandableAttribute)))
                .Select(param => param.Name)
                .Distinct()
                .ToArray());
            var expandParameter = new RuntimeDefinedParameter(
                nameof(this.Expand),
                typeof(string[]),
                new Collection<Attribute>()
                {
                    new ParameterAttribute() { ParameterSetName = GetCmdlet.OperationName },
                    new ParameterAttribute() { ParameterSetName = GetOrSearchCmdlet.OperationName },
                    validateSetAttributeExpand,
                }
            );

            // Create the dictionary of dynamic parameters
            parameterDictionary.Add(nameof(this.Select), selectParameter);
            parameterDictionary.Add(nameof(this.Expand), expandParameter);

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
