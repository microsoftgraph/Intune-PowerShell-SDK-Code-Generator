// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils;

    /// <summary>
    /// A collection of parameter sets for a PowerShell cmdlet.
    /// </summary>
    public class CmdletParameterSets : IEnumerable<CmdletParameterSet>
    {
        private IDictionary<string, CmdletParameterSet> ParameterSets { get; } = new Dictionary<string, CmdletParameterSet>();

        public CmdletParameterSet DefaultParameterSet { get; }

        /// <summary>
        /// Creates a new CmdletParameters instance.
        /// </summary>
        public CmdletParameterSets()
        {
            // All Cmdlets must have the default parameter set
            this.DefaultParameterSet = new CmdletParameterSet();
        }

        /// <summary>
        /// An accessor for parameter sets.
        /// </summary>
        /// <param name="parameterSetName">The parameter set name</param>
        /// <returns>The parameter set.</returns>
        public CmdletParameterSet this[string parameterSetName] => this.Get(parameterSetName);

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
        public CmdletParameterSet Get(string parameterSetName)
        {
            if (parameterSetName == null)
            {
                throw new ArgumentNullException(nameof(parameterSetName));
            }
            CmdletParameterSet result;
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
        public void Add(CmdletParameterSet parameterSet)
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
        public bool Remove(string parameterSetName)
        {
            if (parameterSetName == null)
            {
                throw new ArgumentNullException(nameof(parameterSetName));
            }

            return this.ParameterSets.Remove(parameterSetName);
        }

        /// <summary>
        /// Maps each parameter to the list of parameter sets it belongs to.
        /// </summary>
        /// <returns>The mapping between parameters and parameter sets.</returns>
        public IReadOnlyDictionary<CmdletParameter, IEnumerable<CmdletParameterSet>> GetParameters()
        {
            IDictionary<CmdletParameter, ICollection<CmdletParameterSet>> dictionary = new Dictionary<CmdletParameter, ICollection<CmdletParameterSet>>();

            foreach (CmdletParameterSet parameterSet in this)
            {
                foreach (CmdletParameter parameter in parameterSet)
                {
                    if (!dictionary.ContainsKey(parameter))
                    {
                        dictionary.Add(parameter, new List<CmdletParameterSet>());
                    }

                    dictionary[parameter].Add(parameterSet);
                }
            }

            // Make the parameter set lists read-only by using IEnumerable instead of ICollection
            IReadOnlyDictionary<CmdletParameter, IEnumerable<CmdletParameterSet>> result = dictionary.ToDictionary(
                entry => entry.Key,
                entry => entry.Value.AsEnumerable());
            return result;
        }

        /// <summary>
        /// Returns an enumerator that iterates over all ParameterSets, including the default parameter set.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<CmdletParameterSet> GetEnumerator()
        {
            return this.DefaultParameterSet.SingleObjectAsEnumerable()
                .Concat(this.ParameterSets.Values)
                .GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates over ParameterSets.
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
