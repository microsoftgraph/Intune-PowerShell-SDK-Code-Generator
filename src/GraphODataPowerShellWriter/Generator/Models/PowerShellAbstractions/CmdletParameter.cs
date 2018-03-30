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

        public bool Mandatory { get; set; }

        public bool ValueFromPipeline { get; set; }

        public bool ValueFromPipelineByPropertyName { get; set; }

        public bool ValidateNotNull { get; set; }

        public bool ValidateNotNullOrEmpty { get; set; }

        /// <summary>
        /// Creates a new cmdlet parameter.
        /// </summary>
        /// <param name="parameterName">The parameter name</param>
        /// <param name="parameterType">The type of the parameter</param>
        public CmdletParameter(string parameterName, Type parameterType)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentNullException(nameof(parameterName), "Parameter name cannot be null or empty");
            }

            // TODO: Throw ArgumentException if the parameter name is a reserved/common name

            this.Name = parameterName;
            this.Type = parameterType ?? throw new ArgumentNullException(nameof(parameterType));
        }
    }
}
