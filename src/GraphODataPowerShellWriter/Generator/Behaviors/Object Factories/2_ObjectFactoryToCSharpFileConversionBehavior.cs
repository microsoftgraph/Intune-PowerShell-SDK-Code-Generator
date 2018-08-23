// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Behaviors
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils;
    using Microsoft.Intune.PowerShellGraphSDK.PowerShellCmdlets;

    public static class ObjectFactoryToCSharpFileConversionBehavior
    {
        /// <summary>
        /// Converts an ObjectFactoryCmdlet into a CSharpFile.
        /// </summary>
        /// <param name="cmdlet">The object factory cmdlet</param>
        /// <returns>The converted CSharpFile.</returns>
        public static CSharpFile ToCSharpFile(this ObjectFactoryCmdlet cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            // Get the C# file details
            CSharpFile cSharpFile = new CSharpFile(cmdlet.RelativeFilePath + ".cs")
            {
                Usings = CSharpFileHelper.GetDefaultUsings(),
                Classes = cmdlet.ToCSharpClass().SingleObjectAsEnumerable(),
            };

            return cSharpFile;
        }

        #region Helpers

        private static CSharpClass ToCSharpClass(this ObjectFactoryCmdlet cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            // Create the result object
            CSharpClass result = new CSharpClass($"{cmdlet.Name.Verb}_{cmdlet.Name.Noun}")
            {
                DocumentationComment = cmdlet.Documentation.ToCSharpDocumentationComment(),
                AccessModifier = CSharpAccessModifier.Public,
                BaseType = nameof(ObjectFactoryCmdletBase),
                Attributes = cmdlet.CreateAttributes(),
                Properties = cmdlet.CreateProperties(),
            };

            return result;
        }

        private static IEnumerable<CSharpAttribute> CreateAttributes(this ObjectFactoryCmdlet cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            // Cmdlet attribute
            yield return CSharpClassAttributeHelper.CreateCmdletAttribute(cmdlet.Name, cmdlet.ImpactLevel, cmdlet.DefaultParameterSetName);

            // ODataType attribute
            yield return CSharpClassAttributeHelper.CreateODataTypeAttribute(cmdlet.ResourceTypeFullName);
        }

        #endregion Helpers
    }
}
