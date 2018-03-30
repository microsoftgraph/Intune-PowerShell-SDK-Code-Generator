// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;

    public static class CSharpAccessModifierUtils
    {
        public static string ToCSharpString(this CSharpAccessModifier accessModifier)
        {
            switch (accessModifier)
            {
                case CSharpAccessModifier.Public: return "public";
                case CSharpAccessModifier.Protected: return "protected";
                case CSharpAccessModifier.Internal: return "internal";
                case CSharpAccessModifier.Private: return "private";
                default: throw new ArgumentException("Unknown access modifier", nameof(accessModifier));
            }
        }
    }
}
