// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    using System.Collections;
    using System.Management.Automation;
    using System.Net.Http;
    using PowerShellGraphSDK.Common;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    [Cmdlet(
        VerbsCommunications.Connect, "Intune",
        ConfirmImpact = ConfirmImpact.Low)]
    public class GetGraphAccessToken : PSCmdlet
    {
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
                    throw new PSNotImplementedException();
                case ParameterSetCertificate:
                    throw new PSNotImplementedException();
                default:
                    authResult = GraphAuthentication.Auth(environmentParameters).GetAwaiter().GetResult();
                    break;
            }

            // Write token to pipeline
            this.WriteObject(authResult.AccessToken);
        }
    }

    [Cmdlet(
        VerbsCommon.Get, "GraphMetadata",
        ConfirmImpact = ConfirmImpact.Low)]
    public class GetGraphMetadata : ODataPowerShellSDKCmdlet
    {
        internal override string GetResourcePath()
        {
            return "$metadata";
        }

        internal override object ReadResponse(string content)
        {
            return content;
        }
    }

    [Cmdlet(
        VerbsCommon.Get, "GraphNextPage",
        ConfirmImpact = ConfirmImpact.Low)]
    public class GetGraphNextPage : ODataPowerShellSDKCmdletBase // Use the base class to not allow any query parameters
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [Alias("@odata.nextLink")]
        public string nextLink { get; set; }

        internal override string GetResourcePath()
        {
            return this.nextLink;
        }
    }

    [Cmdlet(
        VerbsLifecycle.Invoke, "GraphRequest",
        ConfirmImpact = ConfirmImpact.Low)]
    public class InvokeGraphRequest : ODataPowerShellSDKCmdlet
    {
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

        // TODO: document that this parameter can be a string, a PSObject or an HttpContent object
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateType(typeof(string), typeof(PSObject), typeof(Hashtable), typeof(HttpContent))]
        public object Content { get; set; }

        internal override HttpMethod GetHttpMethod()
        {
            return new HttpMethod(this.HttpMethod);
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

            // PSObject
            if (content is Hashtable || content is PSObject)
            {
                // Convert the object into JSON
                string contentJson = JsonUtils.WriteJson(content);

                // Return the string as HttpContent
                return new StringContent(contentJson);
            }

            // We should have returned before here
            throw new PSArgumentException($"Unknown content type: '{this.Content.GetType()}'");
        }

        internal override object ReadResponse(string content)
        {
            return content;
        }
    }
}
