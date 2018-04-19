// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils;

    public class CSharpClass
    {
        public string Name { get; }

        public CSharpAccessModifier AccessModifier { get; set; } = CSharpAccessModifier.Public;

        private string _baseType = null;
        public string BaseType
        {
            get => this._baseType;
            set
            {
                if (value != null && value.Trim() == string.Empty)
                {
                    throw new ArgumentException("Base type name cannot be empty.  Set it to null to remove the base type.", nameof(value));
                }

                this._baseType = value;
            }
        }

        private IEnumerable<string> _interfaces = new HashSet<string>();
        public IEnumerable<string> Interfaces
        {
            get => this._interfaces;
            set => this._interfaces = new HashSet<string>(value ?? throw new ArgumentNullException(nameof(value)));
        }

        //TODO: Create a special type for IEnumerable<CSharpAttribute> so validation can be enforced
        private IEnumerable<CSharpAttribute> _attributes = new List<CSharpAttribute>();
        public IEnumerable<CSharpAttribute> Attributes
        {
            get => this._attributes;
            set => this._attributes = value ?? throw new ArgumentNullException(nameof(value));
        }

        //TODO: Create a special type for IEnumerable<CSharpProperty> so validation can be enforced
        private IEnumerable<CSharpProperty> _properties = new List<CSharpProperty>();
        public IEnumerable<CSharpProperty> Properties
        {
            get => this._properties;
            set => this._properties = value ?? throw new ArgumentNullException(nameof(value));
        }

        //TODO: Create a special type for IEnumerable<CSharpMethod> so validation can be enforced
        private IEnumerable<CSharpMethod> _methods = new List<CSharpMethod>();
        public IEnumerable<CSharpMethod> Methods
        {
            get => this._methods;
            set => this._methods = value ?? throw new ArgumentNullException(nameof(value));
        }

        public CSharpClass(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("C# Class name cannot be null or whitespace", nameof(name));
            }

            this.Name = name;
        }

        public override string ToString()
        {
            StringBuilder resultBuilder = new StringBuilder();

            // Attributes
            foreach (CSharpAttribute attribute in this.Attributes)
            {
                resultBuilder.AppendLine(attribute.ToString());
            }

            // Add the first line of the class definition (access modifiers, class name, inheritance)
            IEnumerable<string> parents = this.BaseType == null
                ? this.Interfaces
                : this.BaseType.SingleObjectAsEnumerable().Concat(this.Interfaces);
            string inheritance = parents.Any() ? $" : {string.Join(", ", parents)}" : string.Empty;
            string nameLine = $"{AccessModifier.ToCSharpString()} class {this.Name}{inheritance}";
            resultBuilder.AppendLine(nameLine);

            // Start body
            resultBuilder.AppendLine("{");

            // Properties
            bool isFirst = true;
            foreach (CSharpProperty property in this.Properties)
            {
                // Add a new line except for the first property
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    resultBuilder.AppendLine();
                }

                resultBuilder.AppendLine(property.ToString().Indent());
            }

            if (this.Properties.Any() && this.Methods.Any())
            {
                resultBuilder.AppendLine();
            }

            // Methods
            isFirst = true;
            foreach (CSharpMethod method in this.Methods)
            {
                // Add a new line except for the first property
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    resultBuilder.AppendLine();
                }

                resultBuilder.AppendLine(method.ToString().Indent());
            }

            // End body
            resultBuilder.AppendLine("}");

            // Compile and return result
            string result = resultBuilder.ToString().Trim();
            return result;
        }
    }
}