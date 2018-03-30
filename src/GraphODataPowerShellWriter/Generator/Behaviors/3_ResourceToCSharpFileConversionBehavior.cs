// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Behaviors
{
    using System;
    using System.Collections.Generic;
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
                Classes = resource.GetClasses(),
            };

            return cSharpFile;
        }

        private static IEnumerable<CSharpClass> GetClasses(this Resource resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

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
                AccessModifier = CSharpAccessModifier.Public,
                BaseType = cmdlet.BaseType.ToCSharpString(),
                Attributes = cmdlet.GetAttributes(),
                Properties = cmdlet.GetProperties(),
                Methods = cmdlet.GetMethods(),
            };

            return result;
        }

        private static IEnumerable<CSharpAttribute> GetAttributes(this Cmdlet cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            // "Cmdlet" attribute
            yield return CSharpClassAttributeHelper.CmdletClassAttribute(cmdlet.Name, cmdlet.ImpactLevel);
        }

        private static IEnumerable<CSharpMethod> GetMethods(this Cmdlet cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            // "GetResourcePath" method override
            yield return CSharpMethodHelper.GetResourcePath(cmdlet.CallUrl);
        }

        private static IEnumerable<CSharpProperty> GetProperties(this Cmdlet cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            // We need a mapping of (parameter -> parameter sets) instead of (parameter set -> parameters)
            IReadOnlyDictionary<CmdletParameter, IEnumerable<CmdletParameterSet>> parameters = cmdlet.ParameterSets.GetParameters();

            // Create a property per parameter
            foreach (var entry in parameters)
            {
                CmdletParameter parameter = entry.Key;
                IEnumerable<CmdletParameterSet> parameterSets = entry.Value;

                // Create the property
                yield return new CSharpProperty(parameter.Name, parameter.Type)
                {
                    AccessModifier = CSharpAccessModifier.Public,
                    Attributes = parameter.GetAttributes(parameterSets),
                };
            }
        }

        private static IEnumerable<CSharpAttribute> GetAttributes(this CmdletParameter parameter, IEnumerable<CmdletParameterSet> parameterSets)
        {
            bool firstIteration = true;
            foreach (CmdletParameterSet parameterSet in parameterSets)
            {
                // Only set parameter options (other than parameter set name) on one parameter attribute
                if (firstIteration)
                {
                    yield return CSharpPropertyAttributeHelper.ParameterPropertyAttribute(
                        parameterSet.Name,
                        parameter.Mandatory,
                        parameter.ValueFromPipeline,
                        parameter.ValueFromPipelineByPropertyName);
                }
                else
                {
                    yield return CSharpPropertyAttributeHelper.ParameterPropertyAttribute(parameterSet.Name);
                }

                firstIteration = false;
            }
        }
    }
}
