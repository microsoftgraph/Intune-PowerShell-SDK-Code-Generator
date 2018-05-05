// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System;
    using Microsoft.CSharp;

    public static class CSharpNamingUtils
    {
        /// <summary>
        /// C# code provider.
        /// </summary>
        private static CSharpCodeProvider CSharpCodeProvider = new CSharpCodeProvider();

        /// <summary>
        /// Checks whether the provided string is a reserved C# keyword.
        /// </summary>
        /// <param name="identifier">The identifier to check</param>
        /// <returns>True if the string is a reserved keyword, otherwise false.</returns>
        public static bool IsValidIdentifier(this string identifier)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            return CSharpCodeProvider.IsValidIdentifier(identifier);
        }

        /// <summary>
        /// Sanitizes a C# string if required, otherwise returns the original string.
        /// </summary>
        /// <param name="identifier">The identifier to sanitize</param>
        /// <returns>The sanitized C# keyword which is safe to use as an identifier.</returns>
        public static string SanitizeIdentifier(this string identifier)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            if (identifier.IsValidIdentifier())
            {
                return identifier;
            }
            else
            {
                // Add an "@" to escape keywords
                string result = $"@{identifier}";

                // Make sure that it is now valid - if it isn't, it was never a C# keyword to begin with.
                // It was just an invalid identifier, probably with special characters or numbers.
                if (!result.IsValidIdentifier())
                {
                    throw new ArgumentException($"Invalid characters found in identifier '{identifier}'", nameof(identifier));
                }

                return result;
            }
        }
    }
}
