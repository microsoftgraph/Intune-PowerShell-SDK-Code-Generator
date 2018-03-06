// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models
{
    using System;

    /// <summary>
    /// Represents a PowerShell cmdlet's name.
    /// </summary>
    public class CmdletName
    {
        public string VerbName { get; private set; }
        public string NounName { get; private set; }

        public CmdletName(string verbName, string nounName)
        {
            this.VerbName = verbName ?? throw new ArgumentNullException(nameof(verbName));
            this.NounName = nounName ?? throw new ArgumentNullException(nameof(nounName));
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
                    string verbName = splitString[0];
                    string nounName = splitString[1];

                    if (!string.IsNullOrWhiteSpace(verbName) && !string.IsNullOrWhiteSpace(nounName))
                    {
                        parsedName = new CmdletName(verbName, nounName);
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
            return $"{this.VerbName}-{this.NounName}";
        }
    }
}
