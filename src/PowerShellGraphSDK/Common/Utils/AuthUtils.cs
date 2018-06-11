// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK
{
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using System;
    using System.Threading.Tasks;

    internal static class AuthUtils
    {
        internal static EnvironmentParameters DefaultEnvironmentParameters { get; private set; } = null;

        internal static AuthenticationResult LatestAuthResult { get; private set; }

        /// <summary>
        /// Authenticates with the default environment parameters.
        /// </summary>
        /// <param name="promptBehavior">The ADAL prompt behavior</param>
        /// <returns>The authentication result.</returns>
        /// <exception cref="AdalException">If authentication fails</exception>
        internal static AuthenticationResult Auth(PromptBehavior promptBehavior = PromptBehavior.Auto)
        {
            if (AuthUtils.DefaultEnvironmentParameters == null)
            {
                promptBehavior = PromptBehavior.SelectAccount;
                AuthUtils.DefaultEnvironmentParameters = EnvironmentParameters.Prod.Copy();
            }

            // Use the default environment parameters
            return AuthUtils.Auth(AuthUtils.DefaultEnvironmentParameters, promptBehavior: promptBehavior);
        }

        /// <summary>
        /// Authenticates with the given environment parameters.
        /// </summary>
        /// <param name="environmentParameters">The environment parameters</param>
        /// <param name="promptBehavior">The ADAL prompt behavior</param>
        /// <returns>The authentication result.</returns>
        /// <exception cref="AdalException">If authentication fails</exception>
        internal static AuthenticationResult Auth(EnvironmentParameters environmentParameters, PromptBehavior promptBehavior = PromptBehavior.Auto)
        {
            if (environmentParameters == null)
            {
                throw new ArgumentNullException(nameof(environmentParameters));
            }

            // Create auth context that we will use to connect to the AAD endpoint
            AuthenticationContext authContext = new AuthenticationContext(environmentParameters.AuthUrl);

            // Get the AuthenticationResult from AAD
            LatestAuthResult = authContext.AcquireTokenAsync(
               environmentParameters.ResourceId,
               environmentParameters.ClientId,
               new Uri(environmentParameters.RedirectLink),
               new PlatformParameters(promptBehavior))
               .GetAwaiter().GetResult();

            // Update the cached EnvironmentParameters
            DefaultEnvironmentParameters = environmentParameters;

            return LatestAuthResult;
        }
    }
}
