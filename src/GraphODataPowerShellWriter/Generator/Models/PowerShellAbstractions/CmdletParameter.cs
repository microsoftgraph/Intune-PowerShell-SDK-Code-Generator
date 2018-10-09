// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a PowerShell cmdlet's parameter.
    /// </summary>
    public class CmdletParameter
    {
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The type of the parameter.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The alternative names for this parameter, which will appear as aliases.
        /// </summary>
        public IEnumerable<string> Aliases { get; set; } = null;

        /// <summary>
        /// Whether or not the parameter is mandatory.
        /// </summary>
        public bool Mandatory { get; set; } = false;

        /// <summary>
        /// This property allows a property to be created in the C# class without actually exposing it through PowerShell.
        /// </summary>
        public bool IsPowerShellParameter { get; set; } = true;

        /// <summary>
        /// Whether or not to get the value from piped objects.
        /// </summary>
        public bool ValueFromPipeline { get; set; } = false;

        /// <summary>
        /// Whether or not to get the value from piped objects based on property name.
        /// </summary>
        public bool ValueFromPipelineByPropertyName { get; set; } = true;

        /// <summary>
        /// Whether or not to add the [<see cref="System.Management.Automation.ValidateNotNullAttribute"/>] to the parameter.
        /// </summary>
        public bool ValidateNotNull { get; set; } = false;

        /// <summary>
        /// Whether or not to add the [<see cref="System.Management.Automation.ValidateNotNullOrEmptyAttribute"/>] to the parameter.
        /// This should only be applied to string or array parameters.
        /// </summary>
        public bool ValidateNotNullOrEmpty { get; set; } = false;

        /// <summary>
        /// If not null, adds the [<see cref="Microsoft.Intune.PowerShellGraphSDK.ValidateUrlAttribute"/>] to the parameter.
        /// This should be applied to string parameters that only accept valid URLs.
        /// If this value is set to true, the parameter will only accept absolute URLs.
        /// If this value is set to false, the parameter will accept both absolute and relative URLs.
        /// </summary>
        public bool? ValidateUrlIsAbsolute { get; set; } = null;

        /// <summary>
        /// If not null, adds the [<see cref="Microsoft.Intune.PowerShellGraphSDK.ParameterSetSelectorAttribute"/>] to the parameter with the given name.
        /// </summary>
        public string ParameterSetSelectorName { get; set; } = null;

        /// <summary>
        /// Whether or not to add the [<see cref="Microsoft.Intune.PowerShellGraphSDK.TypeCastParameterAttribute"/>] to the parameter.
        /// </summary>
        public bool IsTypeCastParameter { get; set; } = false;

        /// <summary>
        /// Whether or not to add the [<see cref="Microsoft.Intune.PowerShellGraphSDK.IdParameterAttribute"/>] to the parameter.
        /// </summary>
        public bool IsIdParameter { get; set; } = false;

        /// <summary>
        /// Whether or not to add the [<see cref="Microsoft.Intune.PowerShellGraphSDK.ResourceIdParameterAttribute"/>] to the parameter.
        /// </summary>
        public bool IsResourceIdParameter { get; set; } = false;

        /// <summary>
        /// Whether or not to add the [<see cref="Microsoft.Intune.PowerShellGraphSDK.SelectableAttribute"/>] to the parameter.
        /// </summary>
        public bool IsSelectable { get; set; } = true;

        /// <summary>
        /// Whether or not to add the [<see cref="Microsoft.Intune.PowerShellGraphSDK.ExpandableAttribute"/>] to the parameter.
        /// </summary>
        public bool IsExpandable { get; set; } = true;

        /// <summary>
        /// Whether or not to add the [<see cref="Microsoft.Intune.PowerShellGraphSDK.SortableAttribute"/>] to the parameter.
        /// </summary>
        public bool IsSortable { get; set; } = false;

        /// <summary>
        /// If this is not null, the [<see cref="Microsoft.Intune.PowerShellGraphSDK.DerivedTypeAttribute"/>] will be added to the parameter.
        /// 
        /// This property is not allowed to be empty or whitespace - it must either be null or a valid type name (including the namespace).
        /// </summary>
        public string DerivedTypeName { get; set; } = null;

        /// <summary>
        /// The full name of the original OData type defined in the schema for the property that this parameter represents.
        /// </summary>
        public string ODataTypeFullName { get; set; } = null;

        /// <summary>
        /// The full names of the subtypes of the property's OData type.
        /// </summary>
        public IEnumerable<string> ODataSubTypeFullNames { get; set; }

        /// <summary>
        /// If this is not null, the provided documentation will be added to the parameter.
        /// </summary>
        public CmdletParameterDocumentation Documentation { get; set; } = null;

        /// <summary>
        /// Creates a new cmdlet parameter.
        /// </summary>
        /// <param name="parameterName">The parameter name</param>
        /// <param name="parameterType">The type of the parameter</param>
        public CmdletParameter(string parameterName, Type parameterType)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentException("Parameter name cannot be null or empty", nameof(parameterName));
            }

            // TODO: Throw ArgumentException if the parameter name is a reserved/common name

            this.Name = parameterName;
            this.Type = parameterType ?? throw new ArgumentNullException(nameof(parameterType));
        }
    }
}
