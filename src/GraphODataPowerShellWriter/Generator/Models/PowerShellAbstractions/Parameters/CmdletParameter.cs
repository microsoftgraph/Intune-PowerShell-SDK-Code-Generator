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
        /// Whether or not this is a required parameter.
        /// </summary>
        public bool IsMandatory { get; }

        public ICollection<CmdletParameterAttribute> Attributes { get; } = new List<CmdletParameterAttribute>();

        /// <summary>
        /// Creates a new cmdlet parameter.
        /// </summary>
        /// <param name="parameterName">The parameter name</param>
        /// <param name="parameterType">The type of the parameter</param>
        /// <param name="isMandatory">Whether or not this parameter is mandatory for the parameter set it is in</param>
        public CmdletParameter(string parameterName, Type parameterType, bool isMandatory = true)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentNullException(nameof(parameterName), "Parameter name cannot be null or empty");
            }

            // TODO: Throw ArgumentException if the parameter name is a reserved/common name

            this.Name = parameterName;
            this.Type = parameterType ?? throw new ArgumentNullException(nameof(parameterType));
            this.IsMandatory = isMandatory;
        }
    }
}
