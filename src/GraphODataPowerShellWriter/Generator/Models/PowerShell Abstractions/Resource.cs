// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An abstract representation of a C# file which contains a set of Graph PowerShell SDK cmdlets.
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// The relative path on the file system where cmdlet will be written.
        /// </summary>
        public string OutputFilePath { get; }

        /// <summary>
        /// The set of cmdlets in a Dictionary format for fast lookup.
        /// </summary>
        private IDictionary<string, Cmdlet> _cmdlets { get; set; } = new Dictionary<string, Cmdlet>();

        /// <summary>
        /// The cmdlets that expose operations for this resource.
        /// </summary>
        public IEnumerable<Cmdlet> Cmdlets => _cmdlets.Values;

        /// <summary>
        /// Creates a new Resource object.
        /// </summary>
        /// <param name="outputFilePath">The relative file path to use when writing this Resource as a file to disk</param>
        public Resource(string outputFilePath)
        {
            if (string.IsNullOrWhiteSpace(outputFilePath))
            {
                throw new ArgumentException("Output file path cannot be null or whitespace", nameof(outputFilePath));
            }

            this.OutputFilePath = outputFilePath;
        }

        /// <summary>
        /// A safe accessor for cmdlets.  Returns null if a cmdlet with the given name does not exist.
        /// </summary>
        /// <param name="cmdletName">The cmdlet name</param>
        /// <returns>The cmdlet if it exists, otherwise null.</returns>
        public Cmdlet this[string cmdletName]
        {
            get
            {
                Cmdlet result;
                if (!string.IsNullOrWhiteSpace(cmdletName) && this._cmdlets.TryGetValue(cmdletName, out result))
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
                if (cmdletName == null)
                {
                    throw new ArgumentNullException(nameof(cmdletName));
                }

                this._cmdlets[cmdletName] = value;
            }
        }

        /// <summary>
        /// A safe accessor for cmdlets.
        /// </summary>
        /// <param name="cmdletName">The cmdlet name</param>
        /// <returns>The cmdlet if it exists, otherwise null.</returns>
        public Cmdlet this[CmdletName cmdletName]
        {
            get
            {
                Cmdlet result;
                if (cmdletName != null && this._cmdlets.TryGetValue(cmdletName.ToString(), out result))
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
                if (cmdletName == null)
                {
                    throw new ArgumentNullException(nameof(cmdletName));
                }

                this._cmdlets[cmdletName.ToString()] = value;
            }
        }

        /// <summary>
        /// Determines whether this resource contains a cmdlet by the given name.
        /// </summary>
        /// <param name="cmdletName">The name of the cmdlet to check</param>
        /// <returns>True if this resource contains a cmdlet by the given name, otherwise false</returns>
        public bool Contains(string cmdletName)
        {
            return this._cmdlets.ContainsKey(cmdletName);
        }

        /// <summary>
        /// Determines whether this resource contains a cmdlet by the given name.
        /// </summary>
        /// <param name="cmdletName">The name of the cmdlet to check</param>
        /// <returns>True if this resource contains a cmdlet by the given name, otherwise false</returns>
        public bool Contains(CmdletName cmdletName)
        {
            if (cmdletName == null)
            {
                throw new ArgumentNullException(nameof(cmdletName));
            }

            return this._cmdlets.ContainsKey(cmdletName.ToString());
        }

        /// <summary>
        /// Gets a cmdlet from this resource.
        /// </summary>
        /// <param name="cmdletName">The name of the cmdlet to get</param>
        /// <returns>The cmdlet if it exists, otherwise null</returns>
        public Cmdlet Get(string cmdletName)
        {
            if (cmdletName == null)
            {
                throw new ArgumentNullException(nameof(cmdletName));
            }

            return this[cmdletName];
        }

        /// <summary>
        /// Gets a cmdlet from this resource.
        /// </summary>
        /// <param name="cmdletName">The name of the cmdlet to get</param>
        /// <returns>The cmdlet if it exists, otherwise null</returns>
        public Cmdlet Get(CmdletName cmdletName)
        {
            if (cmdletName == null)
            {
                throw new ArgumentNullException(nameof(cmdletName));
            }

            return this[cmdletName.ToString()];
        }

        /// <summary>
        /// Adds a cmdlet to this resource.
        /// </summary>
        /// <param name="cmdlet">The cmdlet to add</param>
        /// <exception cref="ArgumentException">If a cmdlet with the same name already exists on this resource.</exception>
        public void Add(Cmdlet cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            // Don't allow duplicates
            if (this._cmdlets.Any(c => c.Key == cmdlet.Name))
            {
                throw new ArgumentException($"Cmdlet with the name {cmdlet.Name.ToString()} already exists", nameof(cmdlet));
            }

            this[cmdlet.Name.ToString()] = cmdlet;
        }

        /// <summary>
        /// Adds a collection of cmdlets to this resource.
        /// </summary>
        /// <param name="cmdlets">The cmdlets to add</param>
        /// <exception cref="ArgumentException">If adding the given cmdlets would result in more than one cmdlet with the same name.</exception>
        public void AddAll(IEnumerable<Cmdlet> cmdlets)
        {
            if (cmdlets == null)
            {
                throw new ArgumentNullException(nameof(cmdlets));
            }

            // Check for cmdlets that would result in duplicates
            IEnumerable<string> duplicates = cmdlets.Concat(this._cmdlets.Values)
                .GroupBy(cmdlet => cmdlet.Name)
                .Where(group => group.Count() > 1)
                .Select(group => $"'{group.Key}' ({group.Count()})");
            // Throw if there are duplicates
            if (duplicates.Any())
            {
                throw new ArgumentException($"Adding the given cmdlets would result in duplicates with the name(s): {string.Join(", ", duplicates)}", nameof(cmdlets));
            }

            // Add all the cmdlets
            foreach (Cmdlet cmdlet in cmdlets)
            {
                this.Add(cmdlet);
            }
        }

        /// <summary>
        /// Removes a cmdlet from this resource.
        /// </summary>
        /// <param name="cmdletName">The name of the cmdlet</param>
        /// <returns>True if the cmdlet was successfully removed, otherwise false</returns>
        public bool Remove(string cmdletName)
        {
            if (cmdletName == null)
            {
                throw new ArgumentNullException(nameof(cmdletName));
            }

            return this._cmdlets.Remove(cmdletName);
        }
    }
}
