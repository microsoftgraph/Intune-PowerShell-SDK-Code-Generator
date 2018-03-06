// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.Common
{
    using System;
    using System.Management.Automation;

    public class ValidateUrlAttribute : ValidateEnumeratedArgumentsAttribute
    {
        private UriKind UriKind { get; set; }

        public ValidateUrlAttribute(UriKind uriKind = UriKind.Absolute)
        {
            this.UriKind = uriKind;
        }

        protected override void ValidateElement(object url)
        {
            if (url == null)
            {
                throw new ValidationMetadataException("The provided URL cannot be null");
            }

            string stringUrl = url as string;
            if (stringUrl == null)
            {
                throw new ValidationMetadataException("The provided URL must be a string");
            }

            if (!Uri.IsWellFormedUriString(stringUrl, UriKind))
            {
                throw new ValidationMetadataException("The provided URL is not valid");
            }
        }
    }
}
