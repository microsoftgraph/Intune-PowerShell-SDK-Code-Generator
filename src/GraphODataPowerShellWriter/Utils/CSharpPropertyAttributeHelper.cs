// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System.Collections.Generic;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;

    public static class CSharpPropertyAttributeHelper
    {
        private static readonly CSharpAttribute _validateNotNullPropertyAttribute = new CSharpAttribute("ValidateNotNull");
        public static CSharpAttribute ValidateNotNullPropertyAttribute() => _validateNotNullPropertyAttribute;

        private static readonly CSharpAttribute _validateNotNullOrEmptyPropertyAttribute = new CSharpAttribute("ValidateNotNullOrEmpty");
        public static CSharpAttribute ValidateNotNullOrEmptyPropertyAttribute() => _validateNotNullOrEmptyPropertyAttribute;

        public static CSharpAttribute ParameterPropertyAttribute(string parameterSetName = null, bool mandatory = false, bool valueFromPipeline = false, bool valueFromPipelineByPropertyName = false)
        {
            ICollection<string> arguments = new List<string>();
            if (parameterSetName != null)
            {
                arguments.Add($"ParameterSetName = \"{parameterSetName}\"");
            }
            if (mandatory)
            {
                arguments.Add("Mandatory = true");
            }
            if (valueFromPipeline)
            {
                arguments.Add("ValueFromPipeline = true");
            }
            if (valueFromPipelineByPropertyName)
            {
                arguments.Add("ValueFromPipelineByPropertyName = true");
            }

            return new CSharpAttribute("Parameter", arguments);
        }
    }
}
