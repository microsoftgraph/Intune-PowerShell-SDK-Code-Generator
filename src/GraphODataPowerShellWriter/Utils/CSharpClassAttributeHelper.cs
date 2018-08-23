// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using Microsoft.Intune.PowerShellGraphSDK;
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
                arguments.Add($"{nameof(PS.CmdletAttribute.ConfirmImpact)} = {nameof(PS.ConfirmImpact)}.{impactLevel.ToString()}");
            }

            // Default parameter set
            if (defaultParameterSetName != null)
            {
                arguments.Add($"{nameof(PS.CmdletAttribute.DefaultParameterSetName)} = @\"{defaultParameterSetName}\"");
            }

            return new CSharpAttribute(nameof(PS.CmdletAttribute), arguments: arguments.ToArray());
        }

        public static CSharpAttribute CreateODataTypeAttribute(string oDataTypeFullName, IEnumerable<string> subTypeFullNames = null)
        {
            if (oDataTypeFullName == null)
            {
                throw new ArgumentNullException(nameof(oDataTypeFullName));
            }

            return CSharpPropertyAttributeHelper.CreateODataTypeAttribute(oDataTypeFullName, subTypeFullNames ?? Array.Empty<string>());
        }

        public static CSharpAttribute CreateResourceIdPropertyNameAttribute(string idParameterName)
        {
            if (idParameterName == null)
            {
                throw new ArgumentNullException(nameof(idParameterName));
            }

            return new CSharpAttribute(nameof(ResourceIdPropertyNameAttribute), arguments: $"\"{idParameterName}\"");
        }

        public static CSharpAttribute CreateResourceReferenceAttribute()
        {
            return new CSharpAttribute(nameof(ResourceReferenceAttribute));
        }

        public static CSharpAttribute CreateResourceTypePropertyNameAttribute(string resourceTypePropertyName)
        {
            if (resourceTypePropertyName == null)
            {
                throw new ArgumentNullException(nameof(resourceTypePropertyName));
            }

            return new CSharpAttribute(nameof(ResourceTypePropertyNameAttribute), $"\"{resourceTypePropertyName}\"");
        }
    }
}
