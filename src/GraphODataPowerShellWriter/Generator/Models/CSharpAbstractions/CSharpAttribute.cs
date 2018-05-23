// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils;

    public class CSharpAttribute
    {
        public string Name { get; }

        public bool MultiLineArguments { get; set; } = false;

        private IEnumerable<string> _arguments = new HashSet<string>();
        public IEnumerable<string> Arguments
        {
            get => this._arguments;
            set => this._arguments = new HashSet<string>(value ?? throw new ArgumentNullException(nameof(value)));
        }

        public CSharpAttribute(string name, params string[] arguments) : this(name, arguments.AsEnumerable()) { }

        public CSharpAttribute(string name, IEnumerable<string> arguments)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("C# Property name cannot be null or whitespace", nameof(name));
            }

            this.Name = name;
            this.Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        public override string ToString()
        {
            string attributeName = this.Name.EndsWith("Attribute")
                ? this.Name.Substring(0, this.Name.LastIndexOf("Attribute"))
                : this.Name;

            string argumentString;
            if (this.Arguments.Any())
            {
                if (this.MultiLineArguments)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine();
                    string lastArgument = this.Arguments.Last();
                    foreach (string argument in this.Arguments)
                    {
                        if (argument != lastArgument)
                        {
                            stringBuilder.AppendLine($"{argument},".Indent());
                        }
                        else
                        {
                            stringBuilder.AppendLine(argument.Indent());
                        }
                    }

                    argumentString = $"({stringBuilder.ToString()})";
                }
                else
                {
                    argumentString = $"({string.Join(", ", this.Arguments)})";
                }
            }
            else
            {
                argumentString = string.Empty;
            }

            return $"[{attributeName}{argumentString}]";
        }
    }
}
