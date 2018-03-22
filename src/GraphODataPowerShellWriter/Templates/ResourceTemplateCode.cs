// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Templates
{
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;

    public partial class ResourceTemplate
    {
        private Resource resource;

        public ResourceTemplate(Resource resource)
        {
            this.resource = resource;
        }
    }
}
