// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a parameter set for a PowerShell cmdlet.
    /// </summary>
    public class CmdletParameterSet : ICollection<CmdletParameter>
    {
        /// <summary>
        /// The name of the parameter set.  This can only be null for the default parameter set.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The number of parameters in this parameter set.
        /// </summary>
        public int Count => this._parameters.Count;

        /// <summary>
        /// Whether or not this parameter set is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// The cmdlet parameters in this parameter set as a dictionary for fast lookup.
        /// </summary>
        private IDictionary<string, CmdletParameter> _parameters { get; } = new Dictionary<string, CmdletParameter>();

        /// <summary>
        /// Creates a new default parameter set (i.e. its name is set to null).
        /// This should only ever be called by the <see cref="CmdletParameterSets"/> class.
        /// </summary>
        public CmdletParameterSet()
        {
            this.Name = null;
        }

        /// <summary>
        /// Creates a new parameter set with the given name.
        /// </summary>
        /// <param name="parameterSetName">The parameter set's name</param>
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
            get => this._parameters[parameterName];
            private set => this._parameters[parameterName] = value;
        }

        /// <summary>
        /// Determines whether this parameter set contains a parameter by the given name.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to check</param>
        /// <returns>True if this parameter set contains a parameter by the given name, otherwise false</returns>
        public bool Contains(string parameterName)
        {
            return this._parameters.ContainsKey(parameterName);
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
        /// Adds all given parameters to the parameter set.
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <exception cref="ArgumentNullException">If <paramref name="parameters"/> is null</exception>
        /// <exception cref="ArgumentException">If adding the given CmdletParameter objects would result in duplicate parameter names</exception>
        public void AddAll(IEnumerable<CmdletParameter> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            // Check for cmdlets that would result in duplicates
            IEnumerable<string> duplicates = parameters.Concat(this._parameters.Values)
                .GroupBy(param => param.Name)
                .Where(group => group.Count() > 1)
                .Select(group => $"'{group.Key}' ({group.Count()})");

            // Throw if there are duplicates
            if (duplicates.Any())
            {
                throw new ArgumentException($"Adding the given parameters would result in duplicates with the name(s): {string.Join(", ", duplicates)}", nameof(parameters));
            }

            // Add all the cmdlets
            foreach (CmdletParameter parameter in parameters)
            {
                this.Add(parameter);
            }
        }

        /// <summary>
        /// Removes a parameter from this parameter set.
        /// </summary>
        /// <param name="parameterName">The name of the parameter</param>
        /// <returns>True if the parameter was successfully removed, otherwise false</returns>
        public bool Remove(string parameterName)
        {
            return this._parameters.Remove(parameterName);
        }

        /// <summary>
        /// Clears all parameters from this parameter set.
        /// </summary>
        public void Clear()
        {
            this._parameters.Clear();
        }

        /// <summary>
        /// Checks whether this pararmeter set contains the given parameter.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if this parameter set contains the given parameter, otherwise false.</returns>
        public bool Contains(CmdletParameter parameter)
        {
            return this._parameters.ContainsKey(parameter?.Name)
                && this._parameters[parameter.Name].Equals(parameter);
        }

        /// <summary>
        /// Copies the parameters in this parameter set to the given array, starting at the given array index.
        /// </summary>
        /// <param name="array">The destination array</param>
        /// <param name="arrayIndex">The array index</param>
        public void CopyTo(CmdletParameter[] array, int arrayIndex)
        {
            this._parameters.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the given parameter if it exists in this parameter set.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if the parameter was removed, otherwise false.</returns>
        public bool Remove(CmdletParameter parameter)
        {
            return this._parameters.ContainsKey(parameter?.Name)
                && this._parameters[parameter.Name].Equals(parameter)
                && this._parameters.Remove(parameter.Name);
        }

        /// <summary>
        /// Returns an enumerator that iterates over Parameters.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<CmdletParameter> GetEnumerator()
        {
            return this._parameters.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates over Parameters.
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._parameters.Values.GetEnumerator();
        }
    }
}
