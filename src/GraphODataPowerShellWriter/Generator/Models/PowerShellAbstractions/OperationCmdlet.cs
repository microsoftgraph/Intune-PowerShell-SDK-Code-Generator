// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System.Collections.Generic;

    public class OperationCmdlet : Cmdlet
    {
        /// <summary>
        /// A reference to the parameter that is used to represent the entity's ID.
        /// Leave this null if there is no ID parameter.
        /// </summary>
        public CmdletParameter IdParameter { get; set; }

        /// <summary>
        /// The name of the OData resource that this cmdlet operates on.
        /// </summary>
        public string ResourceTypeFullName { get; set; }

        /// <summary>
        /// The full names of the derived types of this resource's OData type.
        /// </summary>
        public IEnumerable<string> ResourceSubTypeFullNames { get; set; }

        /// <summary>
        /// The name of the property on output objects whose value should be the object's type name.
        /// </summary>
        public string ResourceTypePropertyName { get; set; }

        /// <summary>
        /// Indicates that this cmdlet retrieves resources that can be referenced by "$ref" requests.
        /// </summary>
        public bool IsReferenceable { get; set; }

        /// <summary>
        /// The base type of this cmdlet in the generated output.
        /// </summary>
        public CmdletOperationType OperationType { get; set; }

        /// <summary>
        /// The HTTP method to be used when making the call.
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// The absolute or relative url to be used when making the call.  For relative URLs, the base
        /// URL will be the OData endpoint.  To use values obtained from parameters, this string should
        /// be formatted like an interpolated string with the parameter name as the variable name.
        /// For example, if this cmdlet had a parameter with the name "id", the CallUrl might look like:
        /// <para>
        /// <code>/deviceAppManagement/mobileApps/{mobileAppId}/categories/{id}</code>
        /// </para>
        /// </summary>
        public string CallUrl { get; set; }

        public OperationCmdlet(string verb, string noun) : base(verb, noun) { }

        public OperationCmdlet(CmdletName cmdletName) : base(cmdletName) { }
    }
}
