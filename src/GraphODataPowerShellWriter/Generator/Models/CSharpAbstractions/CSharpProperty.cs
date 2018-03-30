// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils;

    public class CSharpProperty
    {
        public string Name { get; }

        public Type Type { get; }

        public CSharpAccessModifier AccessModifier { get; set; } = CSharpAccessModifier.Public;

        private IEnumerable<CSharpAttribute> _attributes = new List<CSharpAttribute>();
        public IEnumerable<CSharpAttribute> Attributes
        {
            get => this._attributes;
            set => this._attributes = value ?? throw new ArgumentNullException(nameof(value));
        }

        public CSharpProperty(string name, Type type)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("C# Property name cannot be null or whitespace", nameof(name));
            }

            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.Name = name;
        }

        public override string ToString()
        {
            // Create a string builder
            StringBuilder resultBuilder = new StringBuilder();

            // Loop through attributes
            foreach (CSharpAttribute attribute in this.Attributes)
            {
                resultBuilder.AppendLine(attribute.ToString());
            }

            // Add the property definition itself
            resultBuilder.AppendLine($"{this.AccessModifier.ToCSharpString()} {this.Type.FullName} {this.Name} {{ get; set; }}");

            // Compile the string
            string result = resultBuilder.ToString().Trim();

            return result;
        }
    }
}
