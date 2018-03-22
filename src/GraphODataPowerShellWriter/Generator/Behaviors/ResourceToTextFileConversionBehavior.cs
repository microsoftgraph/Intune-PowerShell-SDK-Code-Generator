// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Behaviors
{
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Templates;
    using Vipr.Core;

    /// <summary>
    /// The behavior to convert a Resource into a TextFile.
    /// </summary>
    public static class ResourceToTextFileConversionBehavior
    {
        /// <summary>
        /// Converts a Resource into a TextFile.
        /// </summary>
        /// <param name="resource">The resource</param>
        /// <returns>The generated TextFile.</returns>
        public static TextFile ToTextFile(this Resource resource)
        {
            // Get the T4 template
            ResourceTemplate resourceTemplate = new ResourceTemplate(resource);

            // Generate the output
            string fileContents = resourceTemplate.TransformText();

            // Create the TextFile object which will be sent back to Vipr
            TextFile textFile = new TextFile(resource.OutputFilePath + ".cs", fileContents);

            return textFile;
        }
    }
}
