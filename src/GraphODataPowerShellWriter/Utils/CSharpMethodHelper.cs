// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;

    public static class CSharpMethodHelper
    {
        public static CSharpMethod GetResourcePath(string url)
        {
            CSharpMethod result = new CSharpMethod("GetResourcePath", typeof(string), $"return $\"{url}\";")
            {
                Override = true,
                AccessModifier = CSharpAccessModifier.Internal,
            };

            return result;
        }
    }
}
