// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CSharpAttribute
    {
        public string Name { get; }

        private IEnumerable<string> _arguments = new HashSet<string>();
        public IEnumerable<string> Arguments
        {
            get => this._arguments;
            set => this._arguments = new HashSet<string>(value ?? throw new ArgumentNullException(nameof(value)));
        }

        public CSharpAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("C# Property name cannot be null or whitespace", nameof(name));
            }

            this.Name = name;
            this.Arguments = new List<string>();
        }

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
            string argumentString = this.Arguments.Any() ? $"({string.Join(", ", this.Arguments)})" : string.Empty;
            return $"[{this.Name}{argumentString}]";
        }
    }
}
