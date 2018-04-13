// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ParameterSetSelectorAttribute : Attribute
    {
        public string ParameterSetName { get; }

        public ParameterSetSelectorAttribute(string parameterSetName)
        {
            if (string.IsNullOrWhiteSpace(parameterSetName))
            {
                throw new ArgumentException($"{nameof(parameterSetName)} cannot be null or whitespace", nameof(parameterSetName));
            }

            this.ParameterSetName = parameterSetName;
        }
    }
}
