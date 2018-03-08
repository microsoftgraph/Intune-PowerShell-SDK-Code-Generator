// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    using System.Linq;
    using System.Management.Automation;
    
    [Cmdlet(
        CmdletVerb, CmdletNoun,
        ConfirmImpact = ConfirmImpact.Low)]
    public class GetMobileApp : ODataGetOrSearchPowerShellSDKCmdlet
    {
        public const string CmdletVerb = VerbsCommon.Get;
        public const string CmdletNoun = "IntuneMobileApp";

        [Parameter(ParameterSetName = ParameterSetGet, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string id { get; set; }

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

        internal override PSObject ReadResponse(string content)
        {
            object result = base.ReadResponse(content);
            // If this result is for a SEARCH call and there is only 1 page in the result, return only the result objects
            if (result is PSObject response &&
                this.ParameterSetName == ParameterSetSearch &&
                response.Members.Any(member => member.Name == "value") &&
                !response.Members.Any(member => member.Name == "@odata.nextLink"))
            {
                result = response.Members["value"].Value;
            }

            return PSObject.AsPSObject(result);
        }
    }

    // TODO: Implement POST and PATCH for a few app types

    //[Cmdlet(
    //    CmdletVerb, CmdletNoun,
    //    ConfirmImpact = ConfirmImpact.Medium)]
    //public class UpdateGraphMobileApp : ODataPowerShellSDKCmdlet
    //{
    //    public const string CmdletVerb = VerbsData.Update;
    //    public const string CmdletNoun = "IntuneMobileApp";
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
        CmdletVerb, CmdletNoun,
        ConfirmImpact = ConfirmImpact.High)]
    public class RemoveMobileApp : PowerShellGraphSDK.ODataGetPowerShellSDKCmdlet
    {
        public const string CmdletVerb = VerbsCommon.Remove;
        public const string CmdletNoun = "IntuneMobileApp";

        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string id { get; set; }

        internal override string GetHttpMethod()
        {
            return "DELETE";
        }

        internal override string GetResourcePath()
        {
            return $"/deviceAppManagement/mobileApps/{this.id}";
        }
    }
}
