// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    using System;
    using System.Management.Automation;
    using System.Net.Http;
    
    [Cmdlet(
        VerbsCommon.Get, "GraphMobileApp",
        ConfirmImpact = ConfirmImpact.Low)]
    public class GetGraphMobileApp : ODataGetPowerShellSDKCmdlet
    {
        internal override string GetResourcePath()
        {
            if (this.id != null)
            {
                return $"/deviceAppManagement/mobileApps/{this.id}";
            }
            else
            {
                return $"/deviceAppManagement/mobileApps";
            }
        }
    }

    // TODO: Implement app creation and update for a few app types

    //[Cmdlet(
    //    VerbsData.Update, "GraphMobileApp",
    //    ConfirmImpact = ConfirmImpact.Medium)]
    //public class UpdateGraphMobileApp : ODataPowerShellSDKCmdlet
    //{
    //    [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
    //    [ValidateNotNullOrEmpty]
    //    public string id { get; set; }

    //    internal override HttpMethod GetHttpMethod()
    //    {
    //        return new HttpMethod("PATCH");
    //    }

    //    internal override string GetResourcePath()
    //    {
    //        return $"/deviceAppManagement/mobileApps/{this.id}";
    //    }

    //    internal override object GetContent()
    //    {
    //        // TODO: Convert parameters to a patch set
    //        throw new NotImplementedException();
    //    }
    //}

    [Cmdlet(
        VerbsCommon.Remove, "GraphMobileApp",
        ConfirmImpact = ConfirmImpact.High)]
    public class RemoveMobileApp : ODataPowerShellSDKCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string id { get; set; }

        internal override HttpMethod GetHttpMethod()
        {
            return HttpMethod.Delete;
        }

        internal override string GetResourcePath()
        {
            return $"/deviceAppManagement/mobileApps/{this.id}";
        }
    }
}
