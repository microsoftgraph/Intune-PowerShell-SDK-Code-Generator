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
    }
}
