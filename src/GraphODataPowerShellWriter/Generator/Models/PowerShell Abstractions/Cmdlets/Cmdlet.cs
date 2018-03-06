// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

using System;
using System.Net.Http;

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    /// <summary>
    /// An abstract representation of a PowerShell cmdlet.
    /// </summary>
    public class Cmdlet
    {
        /// <summary>
        /// The name of this cmdlet.
        /// </summary>
        public CmdletName Name { get; }

        /// <summary>
        /// The HTTP method to be used when making the call.
        /// </summary>
        public HttpMethod HttpMethod { get; set; } = HttpMethod.Get;

        /// <summary>
        /// The absolute or relative url to be used when making the call.  For relative URLs, the base
        /// URL will be the OData endpoint.  To use values obtained from parameters, this string should
        /// be formatted like an interpolated string with the parameter name as the variable name.
        /// For example, if this cmdlet had a parameter with the name "id", the CallUrl might look like:
        /// <para>
        /// <code>"/deviceAppManagement/mobileApps/{id}"</code>
        /// </para>
        /// </summary>
        public string CallUrl { get; set; }

        /// <summary>
        /// The parameters that this cmdlet accepts.
        /// </summary>
        public CmdletParameters Parameters { get; } = new CmdletParameters();

        /// <summary>
        /// Creates a new representation of a Graph SDK cmdlet.
        /// </summary>
        /// <param name="parentResource">The parent resource object that this cmdlet exists under</param>
        /// <param name="cmdletName">The name of the cmdlet</param>
        public Cmdlet(CmdletName cmdletName)
        {
            this.Name = cmdletName ?? throw new ArgumentNullException(nameof(cmdletName));
        }
    }
}
