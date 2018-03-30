// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;

    /// <summary>
    /// Represents a PowerShell cmdlet's name.
    /// </summary>
    public class CmdletName
    {
        public string Verb { get; }
        public string Noun { get; }

        private readonly string _compiledString;

        public CmdletName(string verb, string noun)
        {
            this.Verb = verb ?? throw new ArgumentNullException(nameof(verb));
            this.Noun = noun ?? throw new ArgumentNullException(nameof(noun));

            this._compiledString = $"{this.Verb}-{this.Noun}";
        }

        /// <summary>
        /// Safely parses a string cmdlet name into a <see cref="CmdletName"/> object.
        /// </summary>
        /// <param name="stringCmdletName">The string cmdlet name to parse</param>
        /// <param name="parsedName">The CmdletName object</param>
        /// <returns>True if the string cmdlet name could be parsed, otherwise false.</returns>
        public static bool TryParse(string stringCmdletName, out CmdletName parsedName)
        {
            if (!string.IsNullOrWhiteSpace(stringCmdletName))
            {
                string[] splitString = stringCmdletName.Split('-');
                if (splitString.Length == 2)
                {
                    string verb = splitString[0];
                    string noun = splitString[1];

                    if (!string.IsNullOrWhiteSpace(verb) && !string.IsNullOrWhiteSpace(noun))
                    {
                        parsedName = new CmdletName(verb, noun);
                        return true;
                    }
                }
            }

            parsedName = null;
            return false;
        }

        /// <summary>
        /// Outputs the cmdlet name as a string in the format "VerbName-NounName".
        /// </summary>
        /// <returns>The string cmdlet name</returns>
        public override string ToString()
        {
            return this._compiledString;
        }

        public override bool Equals(object obj)
        {
            if (obj != null
                && obj is CmdletName other
                && other.ToString() == this.ToString())
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public static bool operator ==(CmdletName name1, CmdletName name2)
        {
            if (name1 is null && name2 is null)
            {
                return true;
            }
            else if (!(name1 is null) && !(name2 is null)
                && name1.ToString() == name2.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(CmdletName name1, CmdletName name2)
        {
            return !(name1 == name2);
        }

        public static bool operator ==(CmdletName name1, string name2)
        {
            if (name1 is null && name2 is null)
            {
                return true;
            }
            else if (!(name1 is null) && !(name2 is null)
                && name1.ToString() == name2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(CmdletName name1, string name2)
        {
            return !(name1 == name2);
        }

        public static bool operator ==(string name1, CmdletName name2)
        {
            return name2 == name1;
        }

        public static bool operator !=(string name1, CmdletName name2)
        {
            return !(name1 == name2);
        }

        public static implicit operator string(CmdletName name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return name.ToString();
        }
    }
}
