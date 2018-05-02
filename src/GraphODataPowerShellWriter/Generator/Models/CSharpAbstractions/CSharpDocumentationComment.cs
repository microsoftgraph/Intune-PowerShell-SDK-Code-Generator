// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System.Text;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils;

    /// <summary>
    /// Represents a comment in C#.
    /// </summary>
    public class CSharpDocumentationComment
    {
        private const string LinePrefix = @"/// ";

        /// <summary>
        /// The summary section.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// The notes.
        /// </summary>
        public string RawNotes { get; set; }

        public override string ToString()
        {
            StringBuilder resultBuilder = new StringBuilder();

            // Summary
            if (this.Summary != null)
            {
                resultBuilder.AppendLine(LinePrefix + @"<summary>");
                resultBuilder.AppendLine(CSharpDocumentationComment.PrefixLines(this.Summary.Indent()));
                resultBuilder.AppendLine(LinePrefix + @"</summary>");
            }

            // Misc
            if (this.RawNotes != null)
            {
                resultBuilder.AppendLine(CSharpDocumentationComment.PrefixLines(this.RawNotes));
            }

            return resultBuilder.ToString().Trim();
        }

        private static string PrefixLines(string str)
        {
            return str.Indent(indentToken: LinePrefix);
        }
    }
}
