// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;

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
        public Type Type
        {
            get => this._type;
            set => this._type = value ?? throw new ArgumentNullException(nameof(value));
        }
        private Type _type;

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
        /// </summary>
        public bool ValidateNotNullOrEmpty { get; set; } = false;

        /// <summary>
        /// If not null, adds the [<see cref="PowerShellGraphSDK.ParameterSetSelectorAttribute"/>] to the parameter with the given name.
        /// </summary>
        public string ParameterSetSelectorName { get; set; } = null;

        /// <summary>
        /// Whether or not to add the [<see cref="PowerShellGraphSDK.ExpandableAttribute"/>] to the parameter.
        /// </summary>
        public bool IsExpandable { get; set; } = false;

        /// <summary>
        /// Whether or not to add the [<see cref="PowerShellGraphSDK.SortableAttribute"/>] to the parameter.
        /// </summary>
        public bool IsSortable { get; set; } = false;

        /// <summary>
        /// If this is not null, the [<see cref="PowerShellGraphSDK.DerivedTypeAttributeAttribute"/>] will be added to the parameter.
        /// 
        /// This property is not allowed to be empty or whitespace - it must either be null or a valid type name (including the namespace).
        /// </summary>
        public string DerivedTypeName { get; set; } = null;

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
