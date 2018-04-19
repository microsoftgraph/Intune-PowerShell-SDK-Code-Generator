// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using PS = System.Management.Automation;

    public static class CSharpClassAttributeHelper
    {
        public static CSharpAttribute CreateCmdletAttribute(CmdletName name, PS.ConfirmImpact impactLevel, string defaultParameterSetName = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            // Cmdlet name
            ICollection<string> arguments = new List<string>()
            {
                $"\"{name.Verb}\"",
                $"\"{name.Noun}\"",
            };

            // Impact level
            if (impactLevel != PS.ConfirmImpact.None)
            {
                arguments.Add($"{nameof(PS.ConfirmImpact)} = {nameof(PS.ConfirmImpact)}.{impactLevel.ToString()}");
            }

            // Default parameter set
            if (defaultParameterSetName != null)
            {
                arguments.Add($"DefaultParameterSetName = \"{defaultParameterSetName}\"");
            }

            return new CSharpAttribute(nameof(PS.CmdletAttribute), arguments);
        }
    }
}
