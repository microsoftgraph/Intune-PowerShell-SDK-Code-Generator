// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;

    public static class CmdletParameterUtils
    {
        /// <summary>
        /// Merges multiple parameters with the same name (but potentially different types) into a single parameter.
        /// </summary>
        /// <param name="parameters">The parameters to merge</param>
        /// <param name="parameterName">The name of the resulting parameter</param>
        /// <returns>The merged parameter.</returns>
        public static CmdletParameter MergeParameters(this IEnumerable<CmdletParameter> parameters, string parameterName)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            if (!parameters.Any())
            {
                throw new ArgumentException("The provided list of CmdletParameter objects cannot be empty.", nameof(parameters));
            }
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentException("The parameter name cannot be null or whitespace", nameof(parameterName));
            }

            if (parameters.Count() == 1)
            {
                // There was only 1 parameter, so no need to merge
                return parameters.Single();
            }
            else
            {
                // Get the parameter type
                Type parameterType;
                IEnumerable<Type> types = parameters.GroupBy(param => param.Type).Select(g => g.Key);
                if (types.Count() == 1)
                {
                    // There was only 1 type specified for the parameter
                    parameterType = types.Single();
                }
                else
                {
                    parameterType = typeof(object);
                }

                // Create the parameter and set it's properties
                IEnumerable<CmdletParameter> powerShellParameters = parameters.Where(param => param.IsPowerShellParameter);
                CmdletParameter firstDupe = powerShellParameters.FirstOrDefault() ?? parameters.First();
                CmdletParameter result = new CmdletParameter(parameterName, parameterType)
                {
                    IsPowerShellParameter = powerShellParameters.Any(),
                    Mandatory = powerShellParameters.All(param => param.Mandatory),

                    ParameterSetSelectorName = powerShellParameters.FirstOrDefault(param => param.ParameterSetSelectorName != null)?.ParameterSetSelectorName,
                    DerivedTypeName = firstDupe.DerivedTypeName,

                    ValidateNotNull = powerShellParameters.All(param => param.ValidateNotNull),
                    ValidateNotNullOrEmpty = powerShellParameters.All(param => param.ValidateNotNullOrEmpty),
                    ValueFromPipeline = powerShellParameters.All(param => param.ValueFromPipeline),
                    ValueFromPipelineByPropertyName = powerShellParameters.All(param => param.ValueFromPipelineByPropertyName),

                    IsExpandable = parameters.Any(param => param.IsExpandable),
                    IsSelectable = parameters.Any(param => param.IsSelectable),
                    IsSortable = parameters.Any(param => param.IsSortable),

                    Documentation = firstDupe.Documentation,
                };

                return result;
            }
        }

        /// <summary>
        /// Gets the parameter set with the given name if it exists on the cmdlet,
        /// otherwise creates a new parameter set and adds it to the cmdlet.
        /// </summary>
        /// <param name="cmdlet">The cmdlet</param>
        /// <param name="parameterSetName">The name of the parameter set</param>
        /// <returns>The parameter set that was either retrieved or created</returns>
        public static CmdletParameterSet GetOrCreateParameterSet(this Cmdlet cmdlet, string parameterSetName)
        {
            if (parameterSetName == null)
            {
                throw new ArgumentNullException(nameof(parameterSetName));
            }

            // Create the parameter set if it doesn't already exist
            CmdletParameterSet parameterSet = cmdlet.ParameterSets.Get(parameterSetName);
            if (parameterSet == null)
            {
                parameterSet = new CmdletParameterSet(parameterSetName);
                cmdlet.ParameterSets.Add(parameterSet);
            }

            return parameterSet;
        }

        /// <summary>
        /// Gets the C# properties from the parameters defined on the given cmdlet.
        /// </summary>
        /// <param name="cmdlet">The cmdlet</param>
        /// <returns>The C# properties.</returns>
        public static IEnumerable<CSharpProperty> CreateProperties(this Cmdlet cmdlet)
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

        /// <summary>
        /// Creates C# attributes from the information on a given cmdlet parameter
        /// </summary>
        /// <param name="parameter">The cmdlet parameter</param>
        /// <param name="parameterSets">The parameter sets that this parameter is a part of</param>
        /// <returns>The C# attributes.</returns>
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

            // ODataType attribute
            if (parameter.ODataTypeFullName != null)
            {
                yield return CSharpPropertyAttributeHelper.CreateODataTypeAttribute(parameter.ODataTypeFullName, parameter.ODataSubTypeFullNames);
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

            // ID attribute
            if (parameter.IsIdParameter)
            {
                yield return CSharpPropertyAttributeHelper.CreateIdParameterAttribute();
            }

            // TypeCastParameter attribute
            if (parameter.IsTypeCastParameter)
            {
                yield return CSharpPropertyAttributeHelper.CreateTypeCastParameterAttribute();
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
