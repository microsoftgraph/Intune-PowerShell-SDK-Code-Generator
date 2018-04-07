// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;

    public static class CSharpClassAttributeHelper
    {
        public static CSharpAttribute CreateCSharpClassAttribute(CmdletName name, CmdletImpactLevel impactLevel)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            ICollection<string> arguments = new List<string>()
            {
                $"\"{name.Verb}\"",
                $"\"{name.Noun}\"",
            };
            if (impactLevel != CmdletImpactLevel.None)
            {
                arguments.Add($"ConfirmImpact = ConfirmImpact.{impactLevel.ToString()}");
            }

            return new CSharpAttribute("Cmdlet", arguments);
        }
    }
}
