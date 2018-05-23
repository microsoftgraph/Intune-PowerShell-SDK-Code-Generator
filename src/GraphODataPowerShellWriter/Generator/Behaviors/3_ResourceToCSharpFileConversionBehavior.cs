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
            foreach (Cmdlet cmdlet in resource.Cmdlets)
            {
                yield return cmdlet.ToCSharpClass();
            }
        }

        private static CSharpClass ToCSharpClass(this Cmdlet cmdlet)
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

        private static IEnumerable<CSharpAttribute> CreateAttributes(this Cmdlet cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            // "[Cmdlet]" attribute
            yield return CSharpClassAttributeHelper.CreateCmdletAttribute(cmdlet.Name, cmdlet.ImpactLevel, cmdlet.DefaultParameterSetName);

            // "[ODataType]" attribute
            yield return CSharpClassAttributeHelper.CreateODataTypeAttribute(cmdlet.ResourceTypeFullName);

            // "[ResourceReference]" attribute
            if (cmdlet.IsReferenceable)
            {
                yield return CSharpClassAttributeHelper.CreateResourceReferenceAttribute(cmdlet.CallUrl);
            }
        }

        private static IEnumerable<CSharpMethod> CreateMethods(this Cmdlet cmdlet)
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

        private static IEnumerable<CSharpProperty> CreateProperties(this Cmdlet cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            // We need a mapping of (parameter -> parameter sets) instead of (parameter set -> parameters)
            IReadOnlyDictionary<CmdletParameter, IEnumerable<CmdletParameterSet>> parameters = cmdlet.ParameterSets.GetParameters();

            // Merge duplicate properties into 1 property
            var dedupedParameters = parameters.Keys
                .GroupBy(key => key.Name)
                .ToDictionary(
                    group => group.Key,
                    group => new
                    {
                        Parameter = group.MergeParameters(group.Key),
                        ParameterSets = parameters
                            .Where(entry => entry.Key.Name == group.Key)
                            .SelectMany(entry => entry.Value),
                    });

            // Create a property per parameter
            foreach (var entry in dedupedParameters)
            {
                CmdletParameter parameter = entry.Value.Parameter;
                IEnumerable<CmdletParameterSet> parameterSets = entry.Value.ParameterSets;

                // Create the property
                yield return new CSharpProperty(parameter.Name, parameter.Type)
                {
                    AccessModifier = CSharpAccessModifier.Public,
                    Attributes = parameter.CreateAttributes(parameterSets),
                    DocumentationComment = parameter.Documentation.ToCSharpDocumentationComment(),
                };
            }
        }

        private static IEnumerable<CSharpAttribute> CreateAttributes(this CmdletParameter parameter, IEnumerable<CmdletParameterSet> parameterSets)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }
            if (parameterSets == null)
            {
                throw new ArgumentNullException(nameof(parameterSets));
            }

            if (parameter.ODataTypeFullName != null)
            {
                yield return CSharpPropertyAttributeHelper.CreateODataTypeAttribute(parameter.ODataTypeFullName);
            }

            // Selectable attribute
            if (parameter.IsSelectable)
            {
                yield return CSharpPropertyAttributeHelper.CreateSelectableAttribute();
            }

            // Expandable attribute
            if (parameter.IsExpandable)
            {
                yield return CSharpPropertyAttributeHelper.CreateExpandableAttribute();
            }

            // Sortable attribute
            if (parameter.IsSortable)
            {
                yield return CSharpPropertyAttributeHelper.CreateSortableAttribute();
            }

            // DerivedType attribute
            if (parameter.DerivedTypeName != null)
            {
                yield return CSharpPropertyAttributeHelper.CreateDerivedTypeAttribute(parameter.DerivedTypeName);
            }

            // ValidateSet attribute
            if (parameter.Documentation?.ValidValues != null && parameter.Documentation.ValidValues.Any())
            {
                yield return CSharpPropertyAttributeHelper.CreateValidateSetAttribute(parameter.Documentation.ValidValues);
            }

            // Parameter attribute
            if (parameter.IsPowerShellParameter)
            {
                // Aliases
                if (parameter.Aliases != null && parameter.Aliases.Any())
                {
                    yield return CSharpPropertyAttributeHelper.CreateAliasAttribute(parameter.Aliases);
                }

                // Validate not null
                if (parameter.ValidateNotNull)
                {
                    yield return CSharpPropertyAttributeHelper.CreateValidateNotNullAttribute();
                }

                // Validate not null or empty
                if (parameter.ValidateNotNullOrEmpty)
                {
                    yield return CSharpPropertyAttributeHelper.CreateValidateNotNullOrEmptyAttribute();
                }

                // ParameterSetSwitch attribute
                if (parameter.ParameterSetSelectorName != null)
                {
                    yield return CSharpPropertyAttributeHelper.CreateParameterSetSwitchAttribute(parameter.ParameterSetSelectorName);
                }

                // AllowEmptyCollection attribute
                if (parameter.Type.IsArray)
                {
                    yield return CSharpPropertyAttributeHelper.CreateAllowEmptyCollectionAttribute();
                }

                foreach (CmdletParameterSet parameterSet in parameterSets)
                {
                    // Parameter attribute
                    yield return CSharpPropertyAttributeHelper.CreateParameterAttribute(
                        parameterSet.Name,
                        parameter.Mandatory,
                        parameter.ValueFromPipeline,
                        parameter.ValueFromPipelineByPropertyName,
                        parameter.Documentation?.Descriptions?.FirstOrDefault());
                }
            }
        }
    }
}
