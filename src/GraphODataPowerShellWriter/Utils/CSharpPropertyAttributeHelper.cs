// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using PowerShellGraphSDK;
    using PS = System.Management.Automation;

    public static class CSharpPropertyAttributeHelper
    {
        private static readonly CSharpAttribute _validateNotNullAttribute = new CSharpAttribute(nameof(PS.ValidateNotNullAttribute));
        public static CSharpAttribute CreateValidateNotNullAttribute() => _validateNotNullAttribute;

        private static readonly CSharpAttribute _validateNotNullOrEmptyAttribute = new CSharpAttribute(nameof(PS.ValidateNotNullOrEmptyAttribute));
        public static CSharpAttribute CreateValidateNotNullOrEmptyAttribute() => _validateNotNullOrEmptyAttribute;

        private static readonly CSharpAttribute _allowEmptyCollectionAttribute = new CSharpAttribute(nameof(PS.AllowEmptyCollectionAttribute));
        public static CSharpAttribute CreateAllowEmptyCollectionAttribute() => _allowEmptyCollectionAttribute;

        public static CSharpAttribute CreateParameterAttribute(
            string parameterSetName = null,
            bool mandatory = false,
            bool valueFromPipeline = false,
            bool valueFromPipelineByPropertyName = false)
        {
            ICollection<string> arguments = new List<string>();
            if (parameterSetName != null)
            {
                arguments.Add($"{nameof(PS.ParameterAttribute.ParameterSetName)} = \"{parameterSetName}\"");
            }
            if (mandatory)
            {
                arguments.Add($"{nameof(PS.ParameterAttribute.Mandatory)} = true");
            }
            if (valueFromPipeline)
            {
                arguments.Add($"{nameof(PS.ParameterAttribute.ValueFromPipeline)} = true");
            }
            if (valueFromPipelineByPropertyName)
            {
                arguments.Add($"{nameof(PS.ParameterAttribute.ValueFromPipelineByPropertyName)} = true");
            }

            return new CSharpAttribute(nameof(PS.ParameterAttribute), arguments);
        }

        public static CSharpAttribute CreateParameterSetSwitchAttribute(string parameterSetName)
        {
            if (parameterSetName == null)
            {
                throw new ArgumentNullException(nameof(parameterSetName));
            }

            return new CSharpAttribute(nameof(ParameterSetSelectorAttribute), new string[] { $"\"{parameterSetName}\"" });
        }
    }
}
