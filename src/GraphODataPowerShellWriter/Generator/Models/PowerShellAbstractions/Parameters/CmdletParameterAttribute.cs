// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections.Generic;

    public class CmdletParameterAttribute
    {
        public static readonly CmdletParameterAttribute ValidateNotNull = new CmdletParameterAttribute("ValidateNotNull");
        public static readonly CmdletParameterAttribute ValidateNotNullOrEmpty = new CmdletParameterAttribute("ValidateNotNullOrEmpty");

        public string Name { get; }

        public ICollection<string> Arguments { get; }

        public CmdletParameterAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Parameter name cannot be null or whitespace", nameof(name));
            }

            this.Name = name;
            this.Arguments = new List<string>();
        }

        public CmdletParameterAttribute(string name, ICollection<string> arguments)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Parameter name cannot be null or whitespace", nameof(name));
            }

            this.Name = name;
            this.Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        public override string ToString()
        {
            return $"[{this.Name}({string.Join(", ", this.Arguments)})]";
        }
    }
}
