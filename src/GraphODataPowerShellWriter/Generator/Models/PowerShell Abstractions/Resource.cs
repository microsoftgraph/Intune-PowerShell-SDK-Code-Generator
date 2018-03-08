// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Resource : IEnumerable<Cmdlet>
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
        public void Add(Cmdlet cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            this[cmdlet.Name.ToString()] = cmdlet;
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

        public IEnumerator<Cmdlet> GetEnumerator()
        {
            return _cmdlets.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _cmdlets.Values.GetEnumerator();
        }
    }
}
