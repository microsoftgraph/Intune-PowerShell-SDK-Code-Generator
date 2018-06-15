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
        public const string GeneratedSDKFilesLocation = @"PowerShellCmdlets\Generated\SDK";
        public const string GeneratedObjectFactoryFilesLocation = @"PowerShellCmdlets\Generated\ObjectFactories";

        /// <summary>
        /// Implementation which is provided to Vipr for transforming an ODCM model into the PowerShell SDK.
        /// </summary>
        /// <param name="model">The ODCM model provided by Vipr</param>
        /// <returns>The TextFile objects representing the generated SDK.</returns>
        public IEnumerable<TextFile> GenerateProxy(OdcmModel model)
        {
            IEnumerable<TextFile> generatedSdk = GeneratePowerShellSDK(model, PowerShellSDKWriter.GeneratedSDKFilesLocation);
            foreach (TextFile file in generatedSdk)
            {
                yield return file;
            }

            IEnumerable<TextFile> generatedObjectFactories = GenerateObjectFactoryCmdlets(model, PowerShellSDKWriter.GeneratedObjectFactoryFilesLocation);
            foreach (TextFile file in generatedObjectFactories)
            {
                yield return file;
            }
        }

        /// <summary>
        /// Generates the PowerShell SDK from the given ODCM model.
        /// </summary>
        /// <param name="model">The ODCM model</param>
        /// <returns>The TextFile objects representing the generated SDK.</returns>
        private static IEnumerable<TextFile> GeneratePowerShellSDK(OdcmModel model, string location)
        {
            // 1. Convert the ODCM model into nodes (i.e. routes)
            foreach (OdcmNode node in model.ConvertToOdcmNodes())
            {
                // 2. Convert the route into an abstract representation of the PowerShell cmdlets
                Resource resource = node.ConvertToResource(location);

                // 3. Convert the resource into an abstract representation of the C# file
                CSharpFile cSharpFile = resource.ToCSharpFile();

                // 4. Generate the text file by converting the abstract representation of the C# file into a string
                TextFile outputFile = cSharpFile.ToTextFile();

                // Return the generated file
                yield return outputFile;
            }
        }

        /// <summary>
        /// Generates factory cmdlets for creating PowerShell objects that can be serialized as types defined
        /// in the given ODCM model.
        /// </summary>
        /// <param name="model">The ODCM model</param>
        /// <param name="location">The filesystem location at which the generated files should be placed</param>
        /// <returns></returns>
        private static IEnumerable<TextFile> GenerateObjectFactoryCmdlets(OdcmModel model, string location)
        {
            // 1. Convert the types in the ODCM model into ObjectFactory objects
            foreach (ObjectFactoryCmdlet objectFactoryCmdlet in model.CreateObjectFactories(location))
            {
                // 2. Convert the ObjectFactory into an abstract representation of the C# file
                CSharpFile cSharpFile = objectFactoryCmdlet.ToCSharpFile();

                // 3. Generate the text file by converting the abstract representation of the C# file into a string
                TextFile outputFile = cSharpFile.ToTextFile();

                // Return the generated file
                yield return outputFile;
            }
        }
    }
}
