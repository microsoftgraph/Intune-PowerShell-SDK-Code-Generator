// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Reflection;

    public static class CmdletUtils
    {
        public static IEnumerable<PropertyInfo> GetBoundProperties(this PSCmdlet cmdlet, bool includeInherited = true)
        {
            if (cmdlet == null)
            {
                throw new PSArgumentNullException(nameof(cmdlet));
            }

            // Create the binding flags
            BindingFlags bindingFlags = 
                BindingFlags.Instance | // ignore static/const properties
                BindingFlags.Public; // only include public properties
            if (!includeInherited)
            {
                bindingFlags |= BindingFlags.DeclaredOnly; // ignore inherited properties
            }

            // Get the cmdlet's properties
            IEnumerable<PropertyInfo> cmdletProperties = cmdlet.GetType().GetProperties(bindingFlags);

            // Get the properties that were set from PowerShell
            IEnumerable<string> boundParameterNames = cmdlet.MyInvocation.BoundParameters.Keys;
            IEnumerable<PropertyInfo> boundProperties = cmdletProperties.Where(prop => boundParameterNames.Contains(prop.Name));

            return boundProperties;
        }
    }
}
