// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    using System.Collections;
    using System.Management.Automation;
    using System.Net.Http;
    using PowerShellGraphSDK.Common;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    [Cmdlet(
        CmdletVerb, CmdletNoun,
        ConfirmImpact = ConfirmImpact.Low)]
    public class Connect : PSCmdlet
    {
        public const string CmdletVerb = VerbsCommunications.Connect;
        public const string CmdletNoun = "MSGraph";

        private const string ParameterSetPSCredential = "PSCredential";
        private const string ParameterSetCertificate = "Certificate";

        [Parameter]
        public bool UsePPE { get; set; }

        //[Parameter(ParameterSetName = ParameterSetPSCredential, Mandatory = true)]
        //[ValidateNotNull]
        //public PSCredential PSCredential { get; set; }

        //[Parameter(ParameterSetName = ParameterSetCertificate, Mandatory = true)]
        //[ValidateNotNull]
        //public IClientAssertionCertificate Cert { get; set; }

        protected override void ProcessRecord()
        {
            // Get the environment parameters
            EnvironmentParameters environmentParameters = UsePPE
                ? EnvironmentParameters.PPE
                : EnvironmentParameters.Prod;

            // Auth
            AuthenticationResult authResult;
            switch (this.ParameterSetName)
            {
                case ParameterSetPSCredential:
                    // TODO: Implement PSCredential auth
                    throw new PSNotImplementedException();
                case ParameterSetCertificate:
                    // TODO: Implement Certificate auth
                    throw new PSNotImplementedException();
                default:
                    authResult = GraphAuthentication.Auth(environmentParameters).GetAwaiter().GetResult();
                    break;
            }
        }
    }

    [Cmdlet(
        CmdletVerb, CmdletNoun,
        ConfirmImpact = ConfirmImpact.Low)]
    public class GetMetadata : ODataGetPowerShellSDKCmdlet
    {
        public const string CmdletVerb = VerbsCommon.Get;
        public const string CmdletNoun = "MSGraphMetadata";

        internal override string GetResourcePath()
        {
            return "$metadata";
        }

        internal override PSObject ReadResponse(string content)
        {
            // Return the raw response body
            return PSObject.AsPSObject(content);
        }
    }

    [Cmdlet(
        CmdletVerb, CmdletNoun,
        ConfirmImpact = ConfirmImpact.Low)]
    public class GetNextPage : ODataPowerShellSDKCmdletBase // Use the base class to not allow any query parameters
    {
        public const string CmdletVerb = VerbsCommon.Get;
        public const string CmdletNoun = "MSGraphNextPage";

        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [Alias("@odata.nextLink")]
        public string nextLink { get; set; }

        internal override string GetResourcePath()
        {
            return this.nextLink;
        }
    }

    [Cmdlet(
        CmdletVerb, CmdletNoun,
        ConfirmImpact = ConfirmImpact.Low)]
    public class InvokeRequest : ODataGetPowerShellSDKCmdlet
    {
        public const string CmdletVerb = VerbsLifecycle.Invoke;
        public const string CmdletNoun = "MSGraphRequest";

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string HttpMethod { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateUrl]
        public string Url { get; set; }

        // TODO: Document that this parameter can be a string, PSObject, Hashtable or an HttpContent object
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateType(typeof(string), typeof(PSObject), typeof(Hashtable), typeof(HttpContent))]
        public object Content { get; set; }

        internal override string GetHttpMethod()
        {
            return this.HttpMethod;
        }

        internal override string GetResourcePath()
        {
            return this.Url;
        }

        internal override object GetContent()
        {
            return this.Content;
        }

        internal override HttpContent WriteContent(object content)
        {
            // If there's no content, return null
            if (content == null)
            {
                return null;
            }

            // HttpContent
            HttpContent contentHttp = content as HttpContent;
            if (contentHttp != null)
            {
                return contentHttp;
            }

            // String
            string contentString = content as string;
            if (contentString != null)
            {
                return new StringContent(contentString);

            }

            // Hashtable or PSObject
            if (content is Hashtable || content is PSObject)
            {
                // Convert the object into JSON
                string contentJson = JsonUtils.WriteJson(content);

                // Return the string as HttpContent
                return new StringContent(contentJson);
            }

            // We should have returned before here
            throw new PSArgumentException($"Unknown content type: '{this.Content.GetType()}'", nameof(this.Content));
        }

        internal override PSObject ReadResponse(string content)
        {
            // Return the raw response body
            return PSObject.AsPSObject(content);
        }
    }
}
