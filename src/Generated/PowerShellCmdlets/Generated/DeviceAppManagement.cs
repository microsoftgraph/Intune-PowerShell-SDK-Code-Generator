// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    using System;
    using System.Management.Automation;
    using System.Net.Http;

    [Cmdlet(
        VerbsCommon.Get, "GraphDeviceAppManagement",
        ConfirmImpact = ConfirmImpact.Low)]
    public class GetGraphDeviceAppManagement : ODataPowerShellSDKCmdlet
    {
        internal override string GetResourcePath()
        {
            return "deviceAppManagement";
        }
    }

    //[Cmdlet(
    //    VerbsData.Update, "GraphDeviceAppManagement",
    //    ConfirmImpact = ConfirmImpact.Medium)]
    //public class UpdateDeviceAppManagement : ODataPowerShellSDKCmdlet
    //{
    //    [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
    //    public DateTime microsoftStoreForBusinessLastSuccessfulSyncDateTime { get; set; }

    //    [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
    //    public bool? isEnabledForMicrosoftStoreForBusiness { get; set; }

    //    [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
    //    public string microsoftStoreForBusinessLanguage { get; set; }

    //    [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
    //    public DateTime microsoftStoreForBusinessLastCompletedApplicationSyncTime { get; set; }

    //    internal override HttpMethod GetHttpMethod()
    //    {
    //        return new HttpMethod("PATCH");
    //    }

    //    internal override string GetResourcePath()
    //    {
    //        return "deviceAppManagement";
    //    }

    //    internal override object GetContent()
    //    {
    //        // TODO: Convert parameters to a patch set
    //        throw new NotImplementedException();
    //    }
    //}
}
