// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Reflection;

    /// <summary>
    /// The common behavior between all OData PowerShell SDK cmdlets that update OData resources.
    /// </summary>
    public abstract class ODataPatchPowerShellSDKCmdlet : ODataPowerShellSDKCmdletBase
    {
        [Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [Alias("@odata.type")]
        public string ODataType { get; set; }

        internal override string GetHttpMethod()
        {
            return "PATCH";
        }

        internal override object GetContent()
        {
            // Get this cmdlet's properties
            IEnumerable<PropertyInfo> propertyInfos = this.GetType().GetProperties(
                BindingFlags.DeclaredOnly |
                BindingFlags.Instance |
                BindingFlags.Public);

            // Get the properties for the selected parameter set
            IEnumerable<PropertyInfo> patchProperties = propertyInfos
                .Where(prop => prop.GetCustomAttributes<ParameterAttribute>()
                    .Any(parameterAttribute => parameterAttribute.ParameterSetName == this.ParameterSetName));

            // Create the patch set
            IDictionary<string, object> patchSet = new Dictionary<string, object>();

            // Add the type name if it was provided
            if (string.IsNullOrWhiteSpace(this.ODataType))
            {
                this.WriteDebug("Ignoring ODataType parameter value as it is null or white space");
            }
            else
            {
                // If the type is missing the leading "#", add it
                if (!this.ODataType.StartsWith("#"))
                {
                    this.WriteWarning("The ODataType should start with a '#' character.  Prepending ODataType with '#'...");
                    this.ODataType = "#" + this.ODataType;
                }

                patchSet.Add("@odata.type", this.ODataType);
            }

            // Add the parameters to the patch set
            foreach (PropertyInfo property in patchProperties)
            {
                string propertyName = property.Name;
                object propertyValue = property.GetValue(this); // get the value for the given property on this cmdlet

                if (propertyValue != null)
                {
                    patchSet.Add(propertyName, propertyValue);
                }
            }

            // Return the patch set which can be serialized by the "WriteContent" method into a JSON string
            return patchSet;
        }
    }
}
