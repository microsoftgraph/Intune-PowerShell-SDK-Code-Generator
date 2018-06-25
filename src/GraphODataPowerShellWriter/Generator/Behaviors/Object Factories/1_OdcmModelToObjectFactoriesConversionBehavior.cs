// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils;
    using Vipr.Core.CodeModel;

    public static class OdcmModelToObjectFactoriesConversionBehavior
    {
        public static IEnumerable<ObjectFactoryCmdlet> CreateObjectFactories(this OdcmModel model, string pathPrefix = "")
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (pathPrefix == null)
            {
                throw new ArgumentNullException(nameof(pathPrefix));
            }

            // Iterate over each namespace in the model except for the "Edm" namespace
            foreach (OdcmNamespace @namespace in model.Namespaces.Where(n => n.Name != "Edm"))
            {
                // Iterate over each type in the namespace
                foreach (OdcmClass @class in @namespace.Classes.Where(c =>
                    // Make sure we ignore the "Service" entity (i.e. the root)
                    c.Kind != OdcmClassKind.Service
                    // Ignore any types that inherit from other custom types
                    && (c.Base == null || c.Base.Namespace.Name == "Edm" || c.Base.FullName.Equals("microsoft.graph.entity", StringComparison.OrdinalIgnoreCase))))
                {
                    // Convert the ODCM class into an ObjectFactoryCmdlet (each derived type will be represented by a parameter set)
                    ObjectFactoryCmdlet objectFactory = @class.ToObjectFactory(pathPrefix);

                    yield return objectFactory;
                }
            }
        }

        private static ObjectFactoryCmdlet ToObjectFactory(this OdcmClass @class, string pathPrefix)
        {
            if (@class == null)
            {
                throw new ArgumentNullException(nameof(@class));
            }
            if (pathPrefix == null)
            {
                throw new ArgumentNullException(nameof(pathPrefix));
            }

            // Create the cmdlet
            ObjectFactoryCmdlet cmdlet = new ObjectFactoryCmdlet(@class.GetObjectFactoryCmdletName())
            {
                RelativeFilePath = $"{pathPrefix.TrimEnd('\\')}\\{@class.GetObjectFactoryCmdletName().Noun}",
                ResourceTypeFullName = @class.FullName,
                ImpactLevel = ConfirmImpact.None,
                Documentation = new CmdletDocumentation()
                {
                    Synopsis = $"Creates a new object which represents a \"{@class.FullName}\" (or one of it's derived types).",
                    Descriptions = new string[]
                    {
                        $"Creates a new object which represents a \"{@class.FullName}\" (or one of it's derived types).",
                        @class.Description,
                        @class.LongDescription,
                    },
                },
            };

            // Add the parameters for the entity's properties - there should be 1 parameter set per derived type
            cmdlet.AddParametersForEntityProperties(
                @class,
                property => property.IsComputed() || property.IsImmutable(),
                sharedParameterSetName: null,
                addSwitchParameters: true,
                markAsPowerShellParameter: true,
                setBaseTypeParameterSetAsDefault: true);

            return cmdlet;
        }
    }
}
