// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK
{
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using System;
    using System.Threading.Tasks;

    internal static class GraphAuthentication
    {
        public static EnvironmentParameters EnvironmentParameters { get; private set; }

        public async static Task<AuthenticationResult> Auth(EnvironmentParameters environmentParameters)
        {
            AuthenticationContext authContext = new AuthenticationContext(environmentParameters.AuthUrl);

            environmentParameters.AuthResult = await authContext.AcquireTokenAsync(
               environmentParameters.ResourceId,
               environmentParameters.ClientId,
               new Uri(environmentParameters.RedirectLink),
               new PlatformParameters(PromptBehavior.Always));
            EnvironmentParameters = environmentParameters;

            return EnvironmentParameters.AuthResult;
        }
    }
}
