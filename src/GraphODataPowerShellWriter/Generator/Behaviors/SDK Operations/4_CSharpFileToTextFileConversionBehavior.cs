// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Behaviors
{
    using System;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using Vipr.Core;

    /// <summary>
    /// The behavior to convert a C# file into a TextFile.
    /// </summary>
    public static class CSharpFileToTextFileConversionBehavior
    {
        /// <summary>
        /// Converts a CSharpFile into a TextFile.
        /// </summary>
        /// <param name="cSharpFile">The CSharpFile</param>
        /// <returns>The generated TextFile.</returns>
        public static TextFile ToTextFile(this CSharpFile cSharpFile)
        {
            if (cSharpFile == null)
            {
                throw new ArgumentNullException(nameof(cSharpFile));
            }

            // Generate the output
            string fileContents = cSharpFile.ToString();

            // Create the TextFile object which will be sent back to Vipr
            TextFile textFile = new TextFile(cSharpFile.RelativeFilePath, fileContents);

            return textFile;
        }
    }
}
