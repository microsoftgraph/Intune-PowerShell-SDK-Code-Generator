// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK
{
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using System;
    using System.Threading.Tasks;

    internal static class AuthUtils
    {
        internal static EnvironmentParameters EnvironmentParameters { get; private set; } = EnvironmentParameters.Prod.Copy();

        internal static AuthenticationResult AuthResult { get; private set; }

        internal async static Task<AuthenticationResult> Auth()
        {
            // Use the default environment parameters
            return await AuthUtils.Auth(AuthUtils.EnvironmentParameters);
        }

        internal async static Task<AuthenticationResult> Auth(EnvironmentParameters environmentParameters)
        {
            // Create auth context that we will use to connect to the AAD endpoint
            AuthenticationContext authContext = new AuthenticationContext(environmentParameters.AuthUrl);

            // Get the AuthenticationResult from AAD
            AuthResult = await authContext.AcquireTokenAsync(
               environmentParameters.ResourceId,
               environmentParameters.ClientId,
               new Uri(environmentParameters.RedirectLink),
               new PlatformParameters(PromptBehavior.SelectAccount));

            // Update the cached EnvironmentParameters
            EnvironmentParameters = environmentParameters;

            return AuthResult;
        }
    }
}
