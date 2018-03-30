// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;

    public class CSharpArgument
    {
        public string Name { get; }
        public Type Type { get; }

        public CSharpArgument(string name, Type type)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.Name = name;
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
        }
    }
}
