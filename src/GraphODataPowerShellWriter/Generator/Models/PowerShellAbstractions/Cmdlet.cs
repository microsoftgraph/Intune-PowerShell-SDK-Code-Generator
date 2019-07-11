// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections.Generic;
    using PS = System.Management.Automation;

    /// <summary>
    /// An abstract representation of a PowerShell cmdlet.
    /// </summary>
    public abstract class Cmdlet
    {
        /// <summary>
        /// The name of this cmdlet.
        /// </summary>
        public CmdletName Name { get; }

        /// <summary>
        /// Aliases for the cmdlet.
        /// </summary>
        public IList<string> Aliases { get; } = new List<string>();

        /// <summary>
        /// The name of the default parameter set.  If this is null, PowerShell's default will be used.
        /// </summary>
        public string DefaultParameterSetName { get; set; }

        /// <summary>
        /// The impact level of this cmdlet.
        /// This corresponds to the "ConfirmImpact" enum in the System.Management.Automation assembly.
        /// </summary>
        public PS.ConfirmImpact ImpactLevel { get; set; }

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

        /// <summary>
        /// Creates a new representation of a Graph SDK cmdlet.
        /// </summary>
        /// <param name="verb">The verb part of the cmdlet's name</param>
        /// <param name="noun">The noun part of the cmdlet's name</param>
        public Cmdlet(string verb, string noun)
        {
            this.Name = new CmdletName(verb, noun);
        }
    }
}
