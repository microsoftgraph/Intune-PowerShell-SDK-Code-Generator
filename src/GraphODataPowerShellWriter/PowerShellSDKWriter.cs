// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace GraphODataPowerShellTemplateWriter
{
    using System.Collections.Generic;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Behaviors;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using Vipr.Core;
    using Vipr.Core.CodeModel;

    /// <summary>
    /// A writer module for Vipr which generates C# code that can be compiled into a PowerShell Graph SDK module.
    /// </summary>
    public class PowerShellSDKWriter : IOdcmWriter
    {
        public const string CmdletPrefix = "Graph";

        /// <summary>
        /// Implementation which is provided to Vipr for transforming an ODCM model into the PowerShell SDK.
        /// </summary>
        /// <param name="model">The ODCM model provided by Vipr</param>
        /// <returns>The TextFile objects representing the generated SDK.</returns>
        public IEnumerable<TextFile> GenerateProxy(OdcmModel model)
        {
            IEnumerable<TextFile> generated =  GeneratePowerShellSDK(model);

            return generated;
        }

        /// <summary>
        /// Generates the PowerShell SDK from the given ODCM model.
        /// </summary>
        /// <param name="model">The ODCM model</param>
        /// <returns>The TextFile objects representing the generated SDK.</returns>
        public static IEnumerable<TextFile> GeneratePowerShellSDK(OdcmModel model)
        {
            // 1. Convert the ODCM model into nodes (i.e. routes)
            foreach (OdcmNode node in model.ConvertToOdcmNodes())
            {
                // 2. Convert the route into an abstract representation of the PowerShell cmdlets
                Resource resource = node.ConvertToResource(@"PowerShellCmdlets\Generated");

                // 3. Convert the resource into an abstract representation of the C# file
                CSharpFile cSharpFile = resource.ToCSharpFile();

                // 4. Generate the text file by inserting data from the intermediate type into templates
                TextFile outputFile = cSharpFile.ToTextFile();

                // Return the generated file
                yield return outputFile;
            }
        }
    }
}
