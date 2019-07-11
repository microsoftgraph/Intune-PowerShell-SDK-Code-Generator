// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils;

    /// <summary>
    /// The behavior to convert a Resource into a TextFile.
    /// </summary>
    public static class ResourceToCSharpFileConversionBehavior
    {
        /// <summary>
        /// Converts a Resource into a CSharpFile.
        /// </summary>
        /// <param name="resource">The resource</param>
        /// <returns>The converted CSharpFile.</returns>
        public static CSharpFile ToCSharpFile(this Resource resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            // Get the C# file details
            CSharpFile cSharpFile = new CSharpFile(resource.RelativeFilePath + ".cs")
            {
                Usings = CSharpFileHelper.GetDefaultUsings(),
                Classes = resource.CreateClasses(),
            };

            return cSharpFile;
        }

        private static IEnumerable<CSharpClass> CreateClasses(this Resource resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            // Convert each cmdlet to a C# class
            foreach (OperationCmdlet cmdlet in resource.Cmdlets)
            {
                yield return cmdlet.ToCSharpClass();
            }
        }

        private static CSharpClass ToCSharpClass(this OperationCmdlet cmdlet)
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
                BaseType = cmdlet.OperationType.ToCSharpString(),
                Attributes = cmdlet.CreateAttributes(),
                Properties = cmdlet.CreateProperties(),
                Methods = cmdlet.CreateMethods(),
            };

            return result;
        }

        private static IEnumerable<CSharpAttribute> CreateAttributes(this OperationCmdlet cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            // Cmdlet attribute
            yield return CSharpClassAttributeHelper.CreateCmdletAttribute(cmdlet.Name, cmdlet.ImpactLevel, cmdlet.DefaultParameterSetName);

            // ODataType attribute
            yield return CSharpClassAttributeHelper.CreateODataTypeAttribute(cmdlet.ResourceTypeFullName, cmdlet.ResourceSubTypeFullNames);

            // ResourceTypePropertyName attribute
            yield return CSharpClassAttributeHelper.CreateResourceTypePropertyNameAttribute(cmdlet.ResourceTypePropertyName);

            // ResourceReference attribute
            if (cmdlet.IsReferenceable)
            {
                yield return CSharpClassAttributeHelper.CreateResourceReferenceAttribute();
            }

            // Alias attribute
            if (cmdlet.Aliases != null && cmdlet.Aliases.Any())
            {
                yield return CSharpClassAttributeHelper.CreateAliasAttribute(cmdlet.Aliases);
            }
        }

        private static IEnumerable<CSharpMethod> CreateMethods(this OperationCmdlet cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            // "GetHttpMethod()" method override
            if (cmdlet.HttpMethod != null)
            {
                yield return CSharpMethodHelper.CreateGetHttpMethodMethod(cmdlet.HttpMethod);
            }

            // Determine whether this cmdlet calls a function
            bool isFunction =
                cmdlet.OperationType == CmdletOperationType.FunctionReturningCollection
                || cmdlet.OperationType == CmdletOperationType.FunctionReturningEntity;

            // "GetResourcePath()" method override
            yield return CSharpMethodHelper.CreateGetResourcePathMethod(cmdlet.CallUrl, isFunction);

            // Determine whether this cmdlet creates a reference (i.e. is a POST/PUT "$ref" call)
            if (cmdlet.OperationType == CmdletOperationType.PostRefToCollection)
            {
                // "GetContent()" method override
                yield return CSharpMethodHelper.CreateGetContentMethodForCreatingReference(cmdlet.IdParameter.Name);
            }
        }
    }
}
