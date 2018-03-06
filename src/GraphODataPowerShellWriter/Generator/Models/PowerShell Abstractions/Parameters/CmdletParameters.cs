// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class CmdletParameters : IEnumerable<ParameterSet>
    {
        private IDictionary<string, ParameterSet> ParameterSets { get; } = new Dictionary<string, ParameterSet>();

        public ParameterSet DefaultParameterSet => this.Get(ParameterSet.DefaultParameterSetName);

        /// <summary>
        /// Creates a new CmdletParameters instance.
        /// </summary>
        public CmdletParameters()
        {
            // All Cmdlets must have the default parameter set
            this.Add(new ParameterSet());
        }

        /// <summary>
        /// A safe accessor for parameter sets.
        /// </summary>
        /// <param name="parameterSetName">The parameter set name</param>
        /// <returns>The parameter set if it exists, otherwise null.</returns>
        public ParameterSet this[string parameterSetName] => this.Get(parameterSetName);

        /// <summary>
        /// Determines whether this object contains a parameter set by the given name.
        /// </summary>
        /// <param name="parameterSetName">The name of the parameter set to check</param>
        /// <returns>True if this contains a parameter set by the given name, otherwise false</returns>
        public bool Contains(string parameterSetName)
        {
            return this.ParameterSets.ContainsKey(parameterSetName);
        }

        /// <summary>
        /// Gets a parameter from the parameter set.
        /// </summary>
        /// <param name="parameterSetName">The name of the parameter set to get</param>
        /// <returns>The parameter if it exists, otherwise null</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="parameterSetName"/> is null</exception>
        public ParameterSet Get(string parameterSetName)
        {
            if (parameterSetName == null)
            {
                throw new ArgumentNullException(nameof(parameterSetName));
            }
            ParameterSet result;
            if (!string.IsNullOrWhiteSpace(parameterSetName) && this.ParameterSets.TryGetValue(parameterSetName, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Adds a parameter to the parameter set.
        /// </summary>
        /// <param name="parameterSet">The parameter set to add</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="parameterSet"/> is null</exception>
        /// <exception cref="ArgumentException">If the <paramref name="parameterSet"/>'s name is null, empty or whitespace</exception>
        /// <exception cref="ArgumentException">If the <paramref name="parameterSet"/>'s name already exists</exception>
        public void Add(ParameterSet parameterSet)
        {
            if (parameterSet == null)
            {
                throw new ArgumentNullException(nameof(parameterSet));
            }
            if (string.IsNullOrWhiteSpace(parameterSet.Name))
            {
                throw new ArgumentException("The parameter set's name cannot be null, empty or whitespace", nameof(parameterSet));
            }
            if (this.ParameterSets.ContainsKey(parameterSet.Name))
            {
                throw new ArgumentException($"A parameter set with the name '{parameterSet.Name}' already exists", nameof(parameterSet));
            }

            this.ParameterSets.Add(parameterSet.Name, parameterSet);
        }

        /// <summary>
        /// Removes a parameter from this parameter set.
        /// </summary>
        /// <param name="parameterSetName">The name of the parameter set</param>
        /// <returns>True if the parameter set was successfully removed, otherwise false</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="parameterSetName"/> is null</exception>
        /// <exception cref="ArgumentException">If the <paramref name="parameterSetName"/> is equal to <see cref="ParameterSet.DefaultParameterSetName"/></exception>
        public bool Remove(string parameterSetName)
        {
            if (parameterSetName == null)
            {
                throw new ArgumentNullException(nameof(parameterSetName));
            }
            if (parameterSetName == ParameterSet.DefaultParameterSetName)
            {
                throw new ArgumentException($"Cannot remove the default parameter set '{ParameterSet.DefaultParameterSetName}'");
            }

            return this.ParameterSets.Remove(parameterSetName);
        }

        /// <summary>
        /// Returns an enumerator that iterates over ParameterSets.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<ParameterSet> GetEnumerator()
        {
            return this.ParameterSets.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates over ParameterSets.
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.ParameterSets.Values.GetEnumerator();
        }
    }
}
