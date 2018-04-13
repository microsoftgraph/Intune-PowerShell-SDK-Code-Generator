// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils;

    public class CSharpFile
    {
        public string RelativeFilePath { get; }

        private string _namespace = CSharpFileHelper.GetDefaultNamespace();
        public string Namespace
        {
            get => this._namespace;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Namespace name cannot be null or whitespace", nameof(value));
                }

                this._namespace = value;
            }
        }

        private IEnumerable<string> _usings = new HashSet<string>();
        public IEnumerable<string> Usings
        {
            get => this._usings;
            set => this._usings = new HashSet<string>(value ?? throw new ArgumentNullException(nameof(value)));
        }

        //TODO: Create a special type for IEnumerable<CSharpClass> so validation can be enforced
        private IEnumerable<CSharpClass> _classes = new List<CSharpClass>();
        public IEnumerable<CSharpClass> Classes
        {
            get => this._classes;
            set => this._classes = value ?? throw new ArgumentNullException(nameof(value));
        }

        public CSharpFile(string relativeFilePath)
        {
            if (string.IsNullOrWhiteSpace(relativeFilePath))
            {
                throw new ArgumentException("Relative file path cannot be null or whitespace", nameof(relativeFilePath));
            }

            this.RelativeFilePath = relativeFilePath;
        }

        public override string ToString()
        {
            StringBuilder resultBuilder = new StringBuilder();

            // License header
            resultBuilder.AppendLine(CSharpFileHelper.GetLicenseHeader());
            resultBuilder.AppendLine();

            // Namespace
            resultBuilder.AppendLine($"namespace {this.Namespace}");

            // Start body
            resultBuilder.AppendLine("{");

            // Usings
            foreach (string @using in this.Usings)
            {
                resultBuilder.AppendLine(StringUtils.Indent(1, $"using {@using};"));
            }

            if (this.Usings.Any() && this.Classes.Any())
            {
                resultBuilder.AppendLine();
            }

            // Classes
            bool isFirst = true;
            foreach (CSharpClass @class in this.Classes)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    resultBuilder.AppendLine();
                }

                resultBuilder.AppendLine(StringUtils.Indent(1, @class.ToString()));
            }

            // End body
            resultBuilder.AppendLine("}");

            // Compile and return result
            string result = resultBuilder.ToString().Trim();
            return result;
        }
    }
}
