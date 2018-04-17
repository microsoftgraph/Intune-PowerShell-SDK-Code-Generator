// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Reflection;

    /// <summary>
    /// The common behavior between cmdlets that create or update OData resources.
    /// </summary>
    public abstract class ODataPostOrPatchPowerShellSDKCmdlet : ODataPowerShellSDKCmdletBase
    {
        [Parameter(
            ParameterSetName = ODataPatchPowerShellSDKCmdlet.OperationName,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        [Alias("@odata.type")]
        public string ODataType { get; set; }

        internal override object GetContent()
        {
            // Get the properties that were set by the user in this invocation of the PowerShell cmdlet
            IEnumerable<PropertyInfo> boundProperties = this.GetBoundProperties(includeInherited: false);

            // Get the OData type based on which parameters were bound
            string selectedODataType = this.GetODataType();

            // Make sure that the selected type is not null or whitespace
            if (string.IsNullOrWhiteSpace(selectedODataType))
            {
                throw new PSArgumentException("Either the ODataType parameter or a type switch parameter must be set");
            }

            // Create the content
            IDictionary<string, object> content = new Dictionary<string, object>();

            // If the type is missing the leading "#", add it
            if (!selectedODataType.StartsWith("#"))
            {
                selectedODataType = "#" + selectedODataType;
                this.WriteWarning($"The ODataType should start with a '#' character.  Prepending ODataType with '#': '{selectedODataType}'");
            }

            // Add the OData type to the request body
            content.Add("@odata.type", selectedODataType);

            // Get the rest of the properties that will be serialized into the request body
            IEnumerable<PropertyInfo> typeProperties = boundProperties.Where(prop =>
                prop.Name != nameof(this.ODataType) // don't include the ODataType parameter since we already got it
                && !this.GetParameterSetSelectorProperties().Contains(prop)); // don't include the switch parameters

            // Add the parameters to the content
            foreach (PropertyInfo property in typeProperties)
            {
                string propertyName = property.Name;
                object propertyValue = property.GetValue(this); // get the value for the given property on this instance of the cmdlet

                if (propertyValue is PSObject psObj)
                {
                    propertyValue = psObj.BaseObject;
                }

                content.Add(propertyName, propertyValue);
            }

            // Return the content which can be serialized by the "WriteContent" method into a JSON string
            return content;
        }

        #region Helper Methods

        /// <summary>
        /// Gets the currently selected OData type.
        /// </summary>
        /// <returns>The OData type.</returns>
        /// <exception cref="PSArgumentException">If neither the ODataType property nor any of the type switches are set.</exception>
        private string GetODataType()
        {
            string result;

            // Get the bound properties
            IEnumerable<PropertyInfo> boundProperties = this.GetBoundProperties();

            // If ODataType was not set, pick the appropriate value based on the parameter set selector that was set
            bool isODataTypeSet = boundProperties.Any(prop => prop.Name == nameof(this.ODataType));
            if (!isODataTypeSet)
            {
                // Try to get the switch parameter which represents the OData type
                IEnumerable<PropertyInfo> typeSelectorPropertyInfos = this.GetParameterSetSelectorProperties();

                // If no parameter set selector was set, throw an exception
                if (!typeSelectorPropertyInfos.Any())
                {
                    throw new PSArgumentException("Either the ODataType parameter or one of the type switches must be set");
                }

                // If more than 1 parameter set selector was set, throw an exception
                PropertyInfo typeSelectorPropertyInfo = typeSelectorPropertyInfos.SingleOrDefault();
                if (typeSelectorPropertyInfo == null)
                {
                    throw new PSArgumentException($"Multiple type switches were set, but only 1 type switch is allowed per invocation of this cmdlet - these are the type switches that were set: [{string.Join(", ", "'" + typeSelectorPropertyInfos.Select(info => info.Name) + "'")}]");
                }

                // Get the ParameterSetSelector attribute
                ParameterSetSelectorAttribute typeSelectorSwitchAttribute = typeSelectorPropertyInfo
                    .GetCustomAttributes<ParameterSetSelectorAttribute>()
                    .SingleOrDefault();

                // Get the OData type name from the "ParameterSetSelector" attribute (parameter set name is the OData type name)
                result = typeSelectorSwitchAttribute.ParameterSetName;
            }
            else
            {
                // If ODataType was set, make sure that no parameter set selector was set
                if (this.GetParameterSetSelectorProperties().Any())
                {
                    throw new PSArgumentException($"Type switches cannot be used if the '{nameof(this.ODataType)}' parameter is set");
                }

                // Set the result to the value of the ODataType parameter
                result = this.ODataType;
            }

            return result;
        }

        private IEnumerable<PropertyInfo> _parameterSetSelectorProperties = null;
        private IEnumerable<PropertyInfo> GetParameterSetSelectorProperties()
        {
            if (this._parameterSetSelectorProperties == null)
            {
                IEnumerable<PropertyInfo> parameterSetSelectorProperties = this.GetBoundProperties()
                    .Where(prop =>
                        prop.PropertyType == typeof(SwitchParameter) // get the switch parameters
                        && prop.GetCustomAttributes<ParameterSetSelectorAttribute>().Any() // which have the "ParameterSetSelector" attribute
                        && ((SwitchParameter)prop.GetValue(this)).IsPresent); // and are also set to true

                this._parameterSetSelectorProperties = parameterSetSelectorProperties;
            }

            return this._parameterSetSelectorProperties;
        }

        #endregion Helper Methods
    }
}
