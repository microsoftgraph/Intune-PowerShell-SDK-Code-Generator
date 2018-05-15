// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;

    public static class CmdletParameterUtils
    {
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
    }
}
