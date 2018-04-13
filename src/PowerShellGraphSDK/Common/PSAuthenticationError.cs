// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK
{
    using System;
    using System.Management.Automation;

    public class PSAuthenticationError : PSGraphSDKException
    {
        public PSAuthenticationError(Exception innerException, string specificErrorId, ErrorCategory errorCategory, object targetObject)
            : base(innerException, specificErrorId, errorCategory, targetObject)
        {

        }
    }
}
