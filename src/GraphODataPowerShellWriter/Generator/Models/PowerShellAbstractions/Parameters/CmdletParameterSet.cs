// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a parameter set for a PowerShell cmdlet.
    /// </summary>
    public class CmdletParameterSet : IEnumerable<CmdletParameter>
    {
        /// <summary>
        /// The name of the parameter set.  This can only be null for the default parameter set.
        /// </summary>
        public string Name { get; private set; }

        private IDictionary<string, CmdletParameter> Parameters { get; } = new Dictionary<string, CmdletParameter>();

        /// <summary>
        /// Creates a new default parameter set (i.e. its name is set to null).
        /// This should only ever be called by the <see cref="CmdletParameterSets"/> class.
        /// </summary>
        public CmdletParameterSet()
        {
            this.Name = null;
        }


        public CmdletParameterSet(string parameterSetName)
        {
            if (string.IsNullOrWhiteSpace(parameterSetName))
            {
                throw new ArgumentNullException(nameof(parameterSetName), "Parameter set name cannot be null or empty");
            }

            this.Name = parameterSetName;
        }

        /// <summary>
        /// An accessor for parameters.
        /// </summary>
        /// <param name="parameterName">The parameter name</param>
        /// <returns>The parameter.</returns>
        public CmdletParameter this[string parameterName]
        {
            get => this.Parameters[parameterName];
            private set => this.Parameters[parameterName] = value;
        }

        /// <summary>
        /// Determines whether this parameter set contains a parameter by the given name.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to check</param>
        /// <returns>True if this parameter set contains a parameter by the given name, otherwise false</returns>
        public bool Contains(string parameterName)
        {
            return this.Parameters.ContainsKey(parameterName);
        }

        /// <summary>
        /// Gets a parameter from the parameter set.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to get</param>
        /// <returns>The parameter if it exists, otherwise null</returns>
        public CmdletParameter Get(string parameterName)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            return this[parameterName];
        }

        /// <summary>
        /// Adds a parameter to the parameter set.
        /// </summary>
        /// <param name="parameter">The parameter to add</param>
        public void Add(CmdletParameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            this[parameter.Name] = parameter;
        }

        /// <summary>
        /// Removes a parameter from this parameter set.
        /// </summary>
        /// <param name="parameterName">The name of the parameter</param>
        /// <returns>True if the parameter was successfully removed, otherwise false</returns>
        public bool Remove(string parameterName)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            return this.Parameters.Remove(parameterName);
        }

        /// <summary>
        /// Returns an enumerator that iterates over Parameters.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<CmdletParameter> GetEnumerator()
        {
            return this.Parameters.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates over Parameters.
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Parameters.Values.GetEnumerator();
        }
    }
}
