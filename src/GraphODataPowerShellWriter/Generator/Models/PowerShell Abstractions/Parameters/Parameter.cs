// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;

    public class Parameter
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
        public bool IsMandatory { get; set; } = true;

        public Parameter(string parameterName, Type parameterType)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentNullException(nameof(parameterName), "Parameter name cannot be null or empty");
            }
            if (parameterType == null)
            {
                throw new ArgumentNullException(nameof(parameterType));
            }

            // TODO: Throw ArgumentException if the parameter name is a reserved/common name

            this.Name = parameterName;
            this.Type = parameterType;
        }
    }
}
