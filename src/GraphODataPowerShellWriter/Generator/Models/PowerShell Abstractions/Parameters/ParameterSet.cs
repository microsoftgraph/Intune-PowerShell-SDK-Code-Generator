// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Management.Automation;

    /// <summary>
    /// Represents a parameter set for a PowerShell cmdlet.
    /// </summary>
    public class CmdletParameterSet : IEnumerable<CmdletParameter>
    {
        /// <summary>
        /// The name of the parameter set.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The default parameter set name.  Parameters in this parameter set will be available in all parameter sets.
        /// </summary>
        public const string DefaultParameterSetName = ParameterAttribute.AllParameterSets;

        private IDictionary<string, CmdletParameter> Parameters { get; } = new Dictionary<string, CmdletParameter>();

        /// <summary>
        /// Creates a new parameter set for the default parameter set (i.e. with the name defined in <see cref="DefaultParameterSetName"/>).
        /// </summary>
        public CmdletParameterSet()
        {
            this.Name = DefaultParameterSetName;
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
        /// A safe accessor for parameters.
        /// </summary>
        /// <param name="parameterName">The parameter name</param>
        /// <returns>The parameter if it exists, otherwise null.</returns>
        public CmdletParameter this[string parameterName]
        {
            get
            {
                CmdletParameter result;
                if (!string.IsNullOrWhiteSpace(parameterName) && this.Parameters.TryGetValue(parameterName, out result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }

            private set
            {
                if (parameterName == null)
                {
                    throw new ArgumentNullException(nameof(parameterName));
                }

                this.Parameters[parameterName] = value;
            }
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
