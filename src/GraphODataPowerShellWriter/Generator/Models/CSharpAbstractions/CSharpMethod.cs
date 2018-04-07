// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils;

    public class CSharpMethod
    {
        public string Name { get; }

        public CSharpAccessModifier AccessModifier { get; set; } = CSharpAccessModifier.Public;

        public bool Override { get; set; } = false;

        public Type ReturnType { get; }

        //TODO: Create a special type for IEnumerable<CSharpArgument> so validation can be enforced
        private IEnumerable<CSharpArgument> _arguments = new List<CSharpArgument>();
        public IEnumerable<CSharpArgument> Arguments
        {
            get => this._arguments;
            set => this._arguments = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Body { get; }

        public CSharpMethod(string name, Type returnType, string body)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.ReturnType = returnType ?? throw new ArgumentNullException(nameof(returnType));
            this.Body = body ?? throw new ArgumentNullException(nameof(body));
            this.Name = name;
        }

        public string GetMethodSignature()
        {
            string beforeName = $"{this.AccessModifier.ToCSharpString()} {(this.Override ? "override " : string.Empty)}{this.ReturnType.FullName}";
            string arguments = string.Join(", ", this.Arguments.Select(arg => arg.ToString()));
            string result = $"{beforeName} {this.Name}({arguments})";

            return result;
        }

        public override string ToString()
        {
            StringBuilder resultBuilder = new StringBuilder();

            // Method signature (access modifiers, 
            resultBuilder.AppendLine(this.GetMethodSignature());
            resultBuilder.AppendLine("{");
            resultBuilder.AppendLine(StringUtils.Indent(1, this.Body));
            resultBuilder.AppendLine("}");

            string result = resultBuilder.ToString().Trim();

            return result;
        }
    }
}
