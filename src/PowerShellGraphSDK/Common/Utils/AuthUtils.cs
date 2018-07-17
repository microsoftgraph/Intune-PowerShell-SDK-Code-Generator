// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Intune.PowerShellGraphSDK
{
    using System;
    using System.Net.Http.Headers;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    internal static class AuthUtils
    {
        /// <summary>
        /// The query parameter to use when triggering the admin consent flow.
        /// </summary>
        private const string AdminConsentQueryParameter = "prompt=admin_consent";

        /// <summary>
        /// The last successful authentication attempt's result.
        /// </summary>
        private static AuthenticationResult LatestAuthResult { get; set; }

        /// <summary>
        /// The current environment parameters.
        /// </summary>
        internal static EnvironmentParameters CurrentEnvironmentParameters { get; } = EnvironmentParameters.Prod.Copy();

        /// <summary>
        /// True if the user has never logged in, otherwise false.
        /// </summary>
        internal static bool UserHasNeverLoggedIn => LatestAuthResult == null;

        /// <summary>
        /// Authenticates using the device code flow. See here for more information:
        /// https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-deviceprofile/.
        /// </summary>
        /// <param name="displayDeviceCodeMessageToUser">
        /// The action which displays the message from ADAL (containing the retrieved device code) to the user.
        /// The message will instruct the user to enter the device code by navigating to http://aka.ms/devicelogin/.
        /// </param>
        /// <param name="useAdminConsentFlow">
        /// Whether or not to trigger the admin consent flow for this app ID.
        /// </param>
        /// <param name="userInfo">Information about the logged in user if authentication was successful, otherwise null.</param>
        /// <returns>The HTTP header to use when making calls.</returns>
        internal static AuthenticationHeaderValue AuthWithDeviceCode(
            Action<string> displayDeviceCodeMessageToUser,
            out object userInfo,
            bool useAdminConsentFlow = false)
        {
            if (displayDeviceCodeMessageToUser == null)
            {
                throw new ArgumentNullException(nameof(displayDeviceCodeMessageToUser));
            }

            // Get the environment parameters
            EnvironmentParameters environmentParameters = AuthUtils.CurrentEnvironmentParameters;

            // Create auth context that we will use to connect to the AAD endpoint
            AuthenticationContext authContext = new AuthenticationContext(environmentParameters.AuthUrl);

            // Get the device code
            DeviceCodeResult deviceCodeResult = authContext.AcquireDeviceCodeAsync(
                environmentParameters.ResourceId,
                environmentParameters.AppId,
                useAdminConsentFlow ? AuthUtils.AdminConsentQueryParameter : null)
                .GetAwaiter().GetResult();

            // Display the device code
            displayDeviceCodeMessageToUser(deviceCodeResult.Message);

            // Get the auth token
            //TODO: This call hangs and crashes the PowerShell session if the first login was cancelled and the second login times out
            AuthenticationResult authResult = authContext.AcquireTokenByDeviceCodeAsync(deviceCodeResult).GetAwaiter().GetResult();

            // Save the auth result
            AuthUtils.LatestAuthResult = authResult;

            // Set the user info
            userInfo = new
            {
                UPN = authResult.UserInfo.DisplayableId,
                TenantId = authResult.TenantId,
            };

            return new AuthenticationHeaderValue(authResult.AccessTokenType, authResult.AccessToken);
        }

        /// <summary>
        /// Refreshes the access token if required, otherwise returns the most recent still-valid refresh token.
        /// </summary>
        /// <returns>A valid access token.</returns>
        internal static AuthenticationHeaderValue RefreshAuthIfRequired()
        {
            // Make sure there was at least 1 successful login
            if (AuthUtils.UserHasNeverLoggedIn)
            {
                throw new ArgumentException($"No successful login attempts were found. Check for this using the '{nameof(AuthUtils)}.{nameof(UserHasNeverLoggedIn)}' property before calling the '{nameof(RefreshAuthIfRequired)}()' method");
            }

            // Get the environment parameters
            EnvironmentParameters environmentParameters = AuthUtils.CurrentEnvironmentParameters;

            // Create auth context that we will use to connect to the AAD endpoint
            AuthenticationContext authContext = new AuthenticationContext(environmentParameters.AuthUrl);

            // Check if the existing token has expired
            AuthenticationResult authResult = AuthUtils.LatestAuthResult;
            if (authResult.ExpiresOn <= DateTimeOffset.Now)
            {
                // Try to get a new token for the same user
                authResult = authContext.AcquireTokenSilentAsync(
                    environmentParameters.ResourceId,
                    environmentParameters.AppId,
                    new UserIdentifier(AuthUtils.LatestAuthResult.UserInfo.UniqueId, UserIdentifierType.UniqueId))
                    .GetAwaiter().GetResult();

                // Save the result
                AuthUtils.LatestAuthResult = authResult;
            }

            return new AuthenticationHeaderValue(authResult.AccessTokenType, authResult.AccessToken);
        }
    }
}
