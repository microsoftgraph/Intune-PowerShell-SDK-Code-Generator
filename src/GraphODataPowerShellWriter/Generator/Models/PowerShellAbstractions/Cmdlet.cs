// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using PS = System.Management.Automation;

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
        /// The name of the default parameter set.  If this is null, PowerShell's default will be used.
        /// </summary>
        public string DefaultParameterSetName { get; set; }

        /// <summary>
        /// The base type of this cmdlet in the generated output.
        /// </summary>
        public CmdletOperationType OperationType { get; set; }

        /// <summary>
        /// The impact level of this cmdlet.
        /// This corresponds to the "ConfirmImpact" enum in the System.Management.Automation assembly.
        /// </summary>
        public PS.ConfirmImpact ImpactLevel { get; set; }

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
        /// <code>"/deviceAppManagement/mobileApps/{id}"</code>
        /// </para>
        /// </summary>
        public string CallUrl { get; set; }

        /// <summary>
        /// The information that will appear when retrieving the documentation for this cmdlet.
        /// </summary>
        public CmdletDocumentation Documentation { get; set; }

        /// <summary>
        /// This cmdlet's parameter sets (including the default parameter set).
        /// </summary>
        public CmdletParameterSets ParameterSets { get; } = new CmdletParameterSets();

        /// <summary>
        /// Convenience property for getting the default parameter set.
        /// </summary>
        public CmdletParameterSet DefaultParameterSet => ParameterSets.DefaultParameterSet;

        /// <summary>
        /// Creates a new representation of a Graph SDK cmdlet.
        /// </summary>
        /// <param name="cmdletName">The name of the cmdlet</param>
        public Cmdlet(CmdletName cmdletName)
        {
            this.Name = cmdletName ?? throw new ArgumentNullException(nameof(cmdletName));
        }
    }
}
