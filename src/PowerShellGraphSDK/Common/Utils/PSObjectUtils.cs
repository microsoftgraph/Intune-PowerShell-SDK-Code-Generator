// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    internal static class PSObjectUtils
    {
        private const string PSStandardMembers = "PSStandardMembers";

        private const string DefaultDisplayPropertySet = "DefaultDisplayPropertySet";

        internal static void SetDefaultProperties(this PSObject psObject, Func<PSPropertyInfo, bool> filter)
        {
            if (psObject == null)
            {
                throw new ArgumentNullException(nameof(psObject));
            }
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            // Apply the filter and get the member names
            IEnumerable<string> defaultPropertyNames = psObject.Properties.Where(filter).Select(prop => prop.Name);

            // Create the PSMemberSet
            PSMemberSet memberSet = new PSMemberSet(PSStandardMembers, new PSMemberInfo[]
            {
                new PSPropertySet(DefaultDisplayPropertySet, defaultPropertyNames),
            });

            // If the PSStandardMembers property already exists, remove it
            psObject.Members.Remove(PSStandardMembers);

            // Add the "PSStandardMembers" member
            psObject.Members.Add(memberSet);
        }
    }
}
