// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace PowerShellGraphSDK
{
    /// <summary>
    /// Keeps track of the parameters used by an AAD environment.
    /// </summary>
    internal class EnvironmentParameters
    {
        private const string powerShellClientId = "d1ddf0e4-d672-4dae-b554-9d5bdfd93547"; // PowerShell-Intune
        //private const string powerShellClientId = "ae9acab0-7171-4115-83d9-18b4d80969b8"; // PowerShell
        private const string applicationRedirectLink = "urn:ietf:wg:oauth:2.0:oob";

        /// <summary>
        /// Location of the auth endpoint.
        /// </summary>
        public string AuthUrl { get; set; }

        /// <summary>
        /// The ID of the resource.
        /// </summary>
        public string ResourceId { get; set; }

        /// <summary>
        /// The location of the resource.
        /// </summary>
        public string ResourceBaseAddress { get; set; }

        /// <summary>
        /// The client ID to use when authenticating.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The redirect link to use when authenticating.
        /// </summary>
        public string RedirectLink { get; set; }

        /// <summary>
        /// Authentication result.
        /// </summary>
        public AuthenticationResult AuthResult { get; set; }

        /// <summary>
        /// Creates a new EnvironmentParameters object.
        /// </summary>
        public EnvironmentParameters() { }

        /// <summary>
        /// Copies an existing EnvironmentParameters object.
        /// </summary>
        /// <param name="toCopy">The EnvironmentParameters object to copy</param>
        public EnvironmentParameters(EnvironmentParameters toCopy)
        {
            this.AuthUrl = toCopy.AuthUrl;
            this.ResourceId = toCopy.ResourceId;
            this.ClientId = toCopy.ClientId;
            this.RedirectLink = toCopy.RedirectLink;
        }

        public EnvironmentParameters Copy()
        {
            return new EnvironmentParameters(this);
        }

        internal static EnvironmentParameters Prod { get; } = new EnvironmentParameters()
        {
            AuthUrl = "https://login.microsoftonline.com/common",
            ResourceId = "https://graph.microsoft.com",
            ResourceBaseAddress = "https://graph.microsoft.com",
            ClientId = powerShellClientId,
            RedirectLink = applicationRedirectLink,
        };

        internal static EnvironmentParameters PPE { get; } = new EnvironmentParameters()
        {
            AuthUrl = "https://login.windows-ppe.net/common",
            ResourceId = "https://graph.microsoft-ppe.com",
            ResourceBaseAddress = "https://graph.microsoft-ppe.com",
            ClientId = powerShellClientId,
            RedirectLink = applicationRedirectLink,
        };
    }
}
