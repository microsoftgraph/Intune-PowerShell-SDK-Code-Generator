// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Behaviors
{
    using System;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using Vipr.Core;

    /// <summary>
    /// The behavior to convert a Resource into a TextFile.
    /// </summary>
    public static class TextFileGenerationBehavior
    {
        /// <summary>
        /// Converts a Resource into a TextFile.
        /// </summary>
        /// <param name="resource">The resource</param>
        /// <returns>The generated TextFile.</returns>
        public static TextFile ToTextFile(this Resource resource)
        {
            // For each cmdlet in the resource object, get the appropriate template

            // Populate each applicable cmdlet template using the cmdlet information

            // Get the generic template for a resource

            // Populate the generic template using the resource information and the populated cmdlet templates

            throw new NotImplementedException();
        }
    }
}
