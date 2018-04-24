// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DerivedTypeAttribute : Attribute
    {
        public string FullName { get; }

        public DerivedTypeAttribute(string derivedTypeFullName)
        {
            if (string.IsNullOrWhiteSpace(derivedTypeFullName))
            {
                throw new ArgumentNullException(nameof(derivedTypeFullName));
            }

            this.FullName = derivedTypeFullName;
        }
    }
}
