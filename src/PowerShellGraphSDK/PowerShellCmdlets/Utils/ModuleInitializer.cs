// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Reflection;

    /// <summary>
    /// Initializes this PowerShell module.
    /// </summary>
    public class ModuleInitializer : IModuleAssemblyInitializer
    {
        /// <summary>
        /// The initialization logic for this PowerShell module.
        /// </summary>
        public void OnImport()
        {
            InitReferenceUrlGeneratorCache();
        }

        /// <summary>
        /// Creates the reference URL generator cache.
        /// </summary>
        private void InitReferenceUrlGeneratorCache()
        {
            // Get all the types in all assemblies
            IEnumerable<Type> allTypes = AppDomain.CurrentDomain.GetAssemblies()
                // Get all types in all assemblies
                .SelectMany(assembly => assembly.GetTypes());

            // Get all the "$ref" cmdlets
            IEnumerable<Type> referenceCmdletTypes = allTypes
                // Select only the types that represent "$ref" cmdlets
                .Where(type => type.IsClass && !type.IsAbstract
                    // Namespace is "PowerShellGraphSDK.PowerShellCmdlets"
                    && type.Namespace == $"{nameof(PowerShellGraphSDK)}.{nameof(PowerShellGraphSDK.PowerShellCmdlets)}"
                    // Is a cmdlet for a GET call on a resource
                    && (typeof(PostReferenceToCollectionCmdlet).IsAssignableFrom(type) || typeof(PutReferenceToEntityCmdlet).IsAssignableFrom(type)));

            // For each "$ref" cmdlet, create a mapping to a ReferencePathGenerator
            foreach (Type refCmdletType in referenceCmdletTypes)
            {
                // The cmdlet should have the "[ODataType]" attribute
                if (refCmdletType.GetODataResourceTypeName() == null)
                {
                    throw new PSArgumentException($"Could not find the '[{nameof(ODataTypeAttribute)}]' attribute on type '{refCmdletType.Name}'");
                }

                // Get the cmdlet that represents the resource we want to reference
                Type referenceCmdletType = allTypes
                    // Filter them down to the correct cmdlet classes
                    .Where(
                        type => type.IsClass
                        // Namespace is "PowerShellGraphSDK.PowerShellCmdlets"
                        && type.Namespace == $"{nameof(PowerShellGraphSDK)}.{nameof(PowerShellGraphSDK.PowerShellCmdlets)}"
                        // Is a cmdlet for a GET call on a resource
                        && typeof(GetCmdlet).IsAssignableFrom(type)
                        // The resource types match
                        && type.GetODataResourceTypeName() == refCmdletType.GetODataResourceTypeName()
                        // Has the "ResourceReference" attribute
                        && type.GetCustomAttribute<ResourceReferenceDepthAttribute>() != null)
                    // Pick the shortest route possible (i.e. with the least number of segments)
                    .OrderBy(type => type.GetCustomAttribute<ResourceReferenceDepthAttribute>().NumSegments)
                    .FirstOrDefault();

                // Make sure we found a cmdlet which operates on the resource we'd like to reference
                if (referenceCmdletType != null)
                {
                    // Construct an instance of the cmdlet
                    ODataCmdlet cmdlet = referenceCmdletType
                        .GetConstructor(Array.Empty<Type>())?
                        .Invoke(Array.Empty<object>())
                        as ODataCmdlet;

                    // Make sure we were able to construct an instance of the cmdlet
                    if (cmdlet != null)
                    {
                        // Create the URL generator
                        ReferencePathGenerator urlGenerator = new ReferencePathGenerator(cmdlet);

                        // Add the mapping
                        ReferencePathGenerator.Cache.Add(refCmdletType, urlGenerator);
                    }
                }
            }
        }
    }
}
