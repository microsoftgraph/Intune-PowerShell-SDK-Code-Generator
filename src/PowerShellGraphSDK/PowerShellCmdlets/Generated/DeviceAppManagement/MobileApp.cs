// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    using System.Management.Automation;

    [Cmdlet(
        "Get", "MobileApp",
        ConfirmImpact = ConfirmImpact.Low)]
    public class GetMobileApp : ODataGetOrSearchPowerShellSDKCmdlet
    {
        [Parameter(ParameterSetName = ParameterSetGet, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string id { get; set; }

        internal override string GetResourcePath()
        {
            return $"/deviceAppManagement/mobileApps/{id ?? string.Empty}";
        }
    }

    // TODO: Implement POST and PATCH for a few app types

    [Cmdlet(
        CmdletVerb, CmdletNoun,
        ConfirmImpact = ConfirmImpact.Medium)]
    public class UpdateGraphMobileApp : ODataPatchPowerShellSDKCmdlet
    {
        public const string CmdletVerb = VerbsData.Update;
        public const string CmdletNoun = "MobileApp";
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string id { get; set; }

        internal override string GetResourcePath()
        {
            return $"/deviceAppManagement/mobileApps/{this.id}";
        }
    }

    [Cmdlet(
        "Get", "MobileApp",
        ConfirmImpact = ConfirmImpact.High)]
    public class RemoveMobileApp : ODataDeletePowerShellSDKCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string id { get; set; }

        internal override string GetResourcePath()
        {
            return $"/deviceAppManagement/mobileApps/{this.id}";
        }
    }
}