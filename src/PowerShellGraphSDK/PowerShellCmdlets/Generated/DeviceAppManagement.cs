// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    using System.Management.Automation;

    [Cmdlet(
        "Get", "IntuneDeviceAppManagement",
        ConfirmImpact = ConfirmImpact.Low)]
    public class GetDeviceAppManagement : ODataGetPowerShellSDKCmdlet
    {
        internal override string GetResourcePath()
        {
            return "deviceAppManagement";
        }
    }

    // TODO: Implement PATCH on deviceAppManagement

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