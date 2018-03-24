// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.Common
{
    using System;
    using System.Management.Automation;

    public class PSGraphSDKException : Exception, IContainsErrorRecord
    {
        public const string ErrorPrefix = "PowerShellGraphSDK_";

        public ErrorRecord ErrorRecord { get; private set; }

        public PSGraphSDKException(Exception innerException, string specificErrorId, ErrorCategory errorCategory, object targetObject)
            : base(specificErrorId, innerException)
        {
            this.ErrorRecord = new ErrorRecord(
                innerException,
                ErrorPrefix + specificErrorId,
                errorCategory,
                targetObject);
        }
    }
}
