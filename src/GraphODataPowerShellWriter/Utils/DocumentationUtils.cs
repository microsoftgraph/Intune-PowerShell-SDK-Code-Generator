// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System;
    using System.Linq;
    using System.Security;
    using System.Text;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;

    public static class DocumentationUtils
    {
        public static CSharpDocumentationComment ToCSharpDocumentationComment(this CmdletDocumentation cmdletDoc)
        {
            if (cmdletDoc == null)
            {
                throw new ArgumentNullException(nameof(cmdletDoc));
            }

            StringBuilder summaryBuilder = new StringBuilder();

            // Synopsis
            if (cmdletDoc.Synopsis != null)
            {
                summaryBuilder.AppendLine($"<para type=\"synopsis\">{cmdletDoc.Synopsis.EscapeForXml()}</para>");
            }

            // Descriptions
            if (cmdletDoc.Descriptions != null)
            {
                foreach (string description in cmdletDoc.Descriptions.Where(desc => !string.IsNullOrWhiteSpace(desc)))
                {
                    summaryBuilder.AppendLine($"<para type=\"description\">{description.EscapeForXml()}</para>");
                }
            }

            StringBuilder notesBuilder = new StringBuilder();

            // URLs
            if (cmdletDoc.Links != null)
            {
                foreach (CmdletDocumentationLink link in cmdletDoc.Links)
                {
                    notesBuilder.AppendLine($@"<para type=""link"" uri=""{link.Url}"">{link.Name.EscapeForXml()}</para>");
                }
            }

            // Compile the comments
            string summary = summaryBuilder.ToString().Trim();
            string notes = notesBuilder.ToString().Trim();

            // Create the result
            CSharpDocumentationComment result = new CSharpDocumentationComment()
            {
                Summary = string.IsNullOrWhiteSpace(summary) ? null : summary,
                RawNotes = string.IsNullOrWhiteSpace(notes) ? null : notes,
            };

            return result;
        }

        public static CSharpDocumentationComment ToCSharpDocumentationComment(this CmdletParameterDocumentation paramDoc)
        {
            if (paramDoc == null)
            {
                throw new ArgumentNullException(nameof(paramDoc));
            }

            StringBuilder summaryBuilder = new StringBuilder();

            // Descriptions
            if (paramDoc.Descriptions != null)
            {
                foreach (string description in paramDoc.Descriptions.Where(desc => !string.IsNullOrWhiteSpace(desc)))
                {
                    summaryBuilder.AppendLine($"<para type=\"description\">{description.EscapeForXml()}</para>");
                }
            }

            // Valid values
            if (paramDoc.ValidValues != null && paramDoc.ValidValues.Any())
            {
                string valueList = string.Join(", ", paramDoc.ValidValues.Select(val => $"'{val}'".EscapeForXml()));
                summaryBuilder.AppendLine("<para type=\"description\">");
                summaryBuilder.AppendLine($" Valid values: {valueList}".Indent());
                summaryBuilder.AppendLine("</para>");
            }

            // Compile the comments
            string summary = summaryBuilder.ToString().Trim();

            // Create the result
            CSharpDocumentationComment result = new CSharpDocumentationComment()
            {
                Summary = string.IsNullOrWhiteSpace(summary) ? null : summary,
            };

            return result;
        }

        public static string EscapeForXml(this string unescaped)
        {
            return SecurityElement.Escape(unescaped);
        }
    }
}
