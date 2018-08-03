// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using PS = System.Management.Automation;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using Microsoft.Intune.PowerShellGraphSDK;
    using Microsoft.Intune.PowerShellGraphSDK.ODataConstants;
    using Vipr.Core.CodeModel;
    using Vipr.Core.CodeModel.Vocabularies.Capabilities;

    /// <summary>
    /// A set of utility methods to simplify operations on ODCM objects.
    /// </summary>
    public static class OdcmUtils
    {
        /// <summary>
        /// The mapping of Edm types to CLR types.
        /// </summary>
        /// <remarks>
        /// See the spec for details on primitive types:
        /// http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part3-csdl/odata-v4.0-errata03-os-part3-csdl-complete.html#_The_edm:Documentation_Element
        /// </remarks>
        private static IReadOnlyDictionary<string, Type> EdmTypeMappings = new Dictionary<string, Type>(
            EnumerableUtils.CreateEqualityComparer<string>(
                equalsFunction: (s1, s2) => s1.Equals(s2, StringComparison.InvariantCultureIgnoreCase),
                getHashCodeFunction: str => str.ToLowerInvariant().GetHashCode()))
        {
            // Boolean
            { EdmTypeNames.Boolean, typeof(bool) },

            // String
            { EdmTypeNames.String, typeof(string) },

            // Byte
            { EdmTypeNames.Byte, typeof(byte) },
            { EdmTypeNames.SByte, typeof(sbyte) },
            { EdmTypeNames.Stream, typeof(Stream) },
            { EdmTypeNames.Binary, typeof(byte[]) },

            // Integer
            { EdmTypeNames.Int16, typeof(short) },
            { EdmTypeNames.UInt16, typeof(UInt16) },
            { EdmTypeNames.Int32, typeof(int) },
            { EdmTypeNames.UInt32, typeof(UInt32) },
            { EdmTypeNames.Int64, typeof(long) },
            { EdmTypeNames.UInt64, typeof(UInt64) },

            // Decimal
            { EdmTypeNames.Single, typeof(Single) },
            { EdmTypeNames.Double, typeof(double) },
            { EdmTypeNames.Decimal, typeof(decimal) },

            // Guid
            { EdmTypeNames.Guid, typeof(Guid) },

            // Date/Time
            { EdmTypeNames.Date, typeof(DateTimeOffset) },
            { EdmTypeNames.DateTime, typeof(DateTimeOffset) },
            { EdmTypeNames.DateTimeOffset, typeof(DateTimeOffset) },
            { EdmTypeNames.TimeOfDay, typeof(TimeSpan) },
            { EdmTypeNames.Time, typeof(TimeSpan) },
            { EdmTypeNames.Duration, typeof(TimeSpan) },
        };

        /// <summary>
        /// Converts an ODCM type to a CLR type.
        /// </summary>
        /// <param name="type">The ODCM type</param>
        /// <returns>The CLR type if a conversion exists, otherwise the <see cref="Object"/> type.</returns>
        public static Type ToDotNetType(this OdcmType type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type is OdcmEnum)
            {
                // If it's an enum, accept the enum member name as a string
                return typeof(string);
            }
            else if (EdmTypeMappings.TryGetValue(type.FullName, out Type foundType))
            {
                return foundType;
            }
            else
            {
                return typeof(object);
            }
        }

        /// <summary>
        /// Tries to get the name of the parameter that represents the ID of this property.
        /// </summary>
        /// <param name="property">The property</param>
        /// <param name="idParameterName">The ID parameter's name if this property represents an enumeration, otherwise null</param>
        /// <returns>True if successful, otherwise false.</returns>
        public static bool TryGetIdParameterName(this OdcmProperty property, out string idParameterName)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }
            if (property.Name == null)
            {
                throw new ArgumentException("Property name cannot be null", nameof(property));
            }

            // If it's a collection, it needs an ID
            if (property.IsCollection)
            {
                idParameterName = $"{property.Name.Singularize()}Id".ToPascalCase();
                return true;
            }
            else
            {
                idParameterName = null;
                return false;
            }
        }

        /// <summary>
        /// Gets the name of a parameter which accepts a resource URL.
        /// </summary>
        /// <param name="type">The ODCM type of the object that will be referenced</param>
        /// <returns>The name of the parameter which accepts a resource URL.</returns>
        public static string GetResourceUrlParameterName(this OdcmType type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return ODataTypeUtils.GetReferenceUrlParameterName(type.FullName);
        }

        /// <summary>
        /// Gets all properties for a type.
        /// The values are the properties and the keys are the types they came from.
        /// </summary>
        /// <param name="type">The type to evaluate</param>
        /// <param name="includeInherited">Whether or not to include inherited properties</param>
        /// <returns>The type's immediate and inherited properties.  If the type is not a class, it will return an empty result.</returns>
        public static IEnumerable<OdcmProperty> EvaluateProperties(this OdcmType type, bool includeInherited = true)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // NOTE: Overridden properties in subtypes is not allowed in OData v4, so we don't need to worry about duplicated properties
            // If the type is not a class, there are no properties to return
            if (type is OdcmClass @class)
            {
                // Get the list of classes that we want to get properties from (including the class which is this property's type)
                List<OdcmClass> classes = new List<OdcmClass>()
                {
                    @class,
                };

                // Only add base types if we want to include inherited properties
                if (includeInherited)
                {
                    classes.AddRange(@class.GetBaseTypes());
                }

                // Iterate over the type and its base types
                foreach (OdcmClass currentClass in classes)
                {
                    // Add the immediate properties
                    foreach (OdcmProperty property in currentClass.Properties)
                    {
                        yield return property;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the immediate base type.  Use <see cref="GetBaseTypes(OdcmType)"/> to get the full chain of base types.
        /// </summary>
        /// <param name="type">The type to evaluate</param>
        /// <returns>The type's immediate base type if it has one, otherwise null.</returns>
        public static OdcmClass GetBaseType(this OdcmType type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type is OdcmClass @class)
            {
                return @class.Base;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the chain of base types for a given type.
        /// The first item in the chain is the type that the given type directly inherits from.
        /// </summary>
        /// <param name="type">The type to evaluate</param>
        /// <returns>The chain of base types.</returns>
        public static IEnumerable<OdcmClass> GetBaseTypes(this OdcmType type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            OdcmClass currentType = type.GetBaseType();
            while (currentType != null)
            {
                yield return currentType;
                currentType = currentType.GetBaseType();
            }
        }

        /// <summary>
        /// Gets the given type's derived types.
        /// </summary>
        /// <param name="type">The type to evaluate</param>
        /// <returns>The derived types.</returns>
        public static IEnumerable<OdcmClass> GetDerivedTypes(this OdcmType type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type is OdcmClass @class)
            {
                return @class.Derived;
            }
            else
            {
                return Enumerable.Empty<OdcmClass>();
            }
        }

        /// <summary>
        /// Determines whether the given property's type represents a media entity (which has an associated stream).
        /// </summary>
        /// <param name="property">The ODCM property</param>
        /// <returns>True if the given property's type represents a media entity, otherwise false.</returns>
        public static bool HasStream(this OdcmProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }
            if (property.Type == null)
            {
                throw new ArgumentNullException($"{nameof(property)}.{nameof(OdcmProperty.Type)}");
            }

            return property.Type.HasStream();
        }

        /// <summary>
        /// Determines whether the given type represents a media entity (which has an associated stream).
        /// </summary>
        /// <param name="type">The ODCM type</param>
        /// <returns>True if the given type represents a media entity, otherwise false.</returns>
        public static bool HasStream(this OdcmType type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // Check if this is a media entity (i.e. has an associated stream)
            return type is OdcmMediaClass;
        }

        /// <summary>
        /// Determines whether the given property's type represents a data stream.
        /// </summary>
        /// <param name="type">The ODCM property</param>
        /// <returns>True if the given property's type represents a data stream, otherwise false.</returns>
        public static bool IsStream(this OdcmProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }
            if (property.Type == null)
            {
                throw new ArgumentNullException($"{nameof(property)}.{nameof(OdcmProperty.Type)}");
            }

            return property.Type.IsStream();
        }

        /// <summary>
        /// Determines whether the given type represents a data stream.
        /// </summary>
        /// <param name="type">The ODCM type</param>
        /// <returns>True if the given type represents a data stream, otherwise false.</returns>
        public static bool IsStream(this OdcmType type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // Check if this is an "Edm.Stream" type
            return type.FullName == "Edm.Stream";
        }

        /// <summary>
        /// Determines whether the resource at this route represents a reference
        /// (i.e. a navigation property which is not contained).
        /// If so, it outputs this resource's parent.
        /// </summary>
        /// <param name="parentProperty">This resource's parent - if null, this method returns false</param>
        /// <returns>True if this property is a reference, otherwise false.</returns>
        public static bool IsReference(this OdcmProperty property, OdcmProperty parentProperty)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            bool isReference =
                // This has a parent property (i.e. not a top-level entity)
                parentProperty != null
                // Navigation property
                && property.IsLink
                // Not contained
                && !property.ContainsTarget;

            return isReference;
        }

        /// <summary>
        /// Determines whether or not a given ODCM object is marked with a "Computed" annotation.
        /// </summary>
        /// <param name="obj">The ODCM object</param>
        /// <returns>True if the ODCM object is marked as "Computed", otherwise false.</returns>
        public static bool IsComputed(this OdcmObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return (obj.TryGetCapability(AnnotationTerms.Computed) is OdcmBooleanCapability capability && capability.Value == true);
        }

        /// <summary>
        /// Determines whether or not a given ODCM object is marked with a "Immutable" annotation.
        /// </summary>
        /// <param name="obj">The ODCM object</param>
        /// <returns>True if the ODCM object is marked as "Immutable", otherwise false.</returns>
        public static bool IsImmutable(this OdcmObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return (obj.TryGetCapability(AnnotationTerms.Immutable) is OdcmBooleanCapability capability && capability.Value == true);
        }

        /// <summary>
        /// Gets the name of the ObjectFactoryCmdlet that can be generated from the given OdcmType.
        /// </summary>
        /// <param name="type">The ODCM type</param>
        /// <returns>The name of the ObjectFactoryCmdlet.</returns>
        public static CmdletName GetObjectFactoryCmdletName(this OdcmType type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return new CmdletName(PS.VerbsCommon.New, $"{type.Name.ToPascalCase()}Object");
        }

        /// <summary>
        /// Converts an ODCM type into a PowerShell acceptable type.
        /// </summary>
        /// <param name="odcmType">The ODCM type</param>
        /// <param name="isCollection">Whether or not the type is the type of object in a collection (e.g. an array)</param>
        /// <returns>The PowerShell type.</returns>
        public static Type ToPowerShellType(this OdcmType odcmType, bool isCollection = false)
        {
            if (odcmType == null)
            {
                throw new ArgumentNullException(nameof(odcmType));
            }

            // Convert the type (default to System.Object if we can't convert the type)
            Type result = odcmType.ToDotNetType();

            // Make it an array type if necessary
            if (isCollection)
            {
                result = result.MakeArrayType();
            }

            return result;
        }

        /// <summary>
        /// Traverses the tree of a type and its subtypes. 
        /// </summary>
        /// <param name="baseType">The base type (i.e. root of the tree)</param>
        /// <param name="typeProcessor">The processor for handling each type</param>
        /// <remarks>
        /// Types are guaranteed to be processed before the types that derive from them.
        /// In other words, when a type is visited, it is guaranteed that all of its base types have been processed.
        /// </remarks>
        public static void VisitDerivedTypes(this OdcmType baseType, Action<OdcmType> typeProcessor)
        {
            if (baseType == null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }
            if (typeProcessor == null)
            {
                throw new ArgumentNullException(nameof(typeProcessor));
            }

            Stack<OdcmType> unvisited = new Stack<OdcmType>();
            unvisited.Push(baseType);
            while (unvisited.Any())
            {
                // Get the next type
                OdcmType type = unvisited.Pop();

                // Add derived types to the unvisited list
                foreach (OdcmType derivedType in type.GetDerivedTypes())
                {
                    unvisited.Push(derivedType);
                }

                // Process the type
                typeProcessor(type);
            }
        }

        /// <summary>
        /// Create the processor for adding parameters to a cmdlet.
        /// </summary>
        /// <param name="cmdlet">The cmdlet</param>
        /// <param name="baseType">The type for which we should add parameters</param>
        /// <param name="addSwitchParametersForAbstractTypes">Whether or not to create a switch parameter for abstract types</param>
        /// <param name="sharedParameterSetName">
        /// The name of the shared parameter set, or null if parameters shouldn't be added to the shared parameter set
        /// </param>
        public static void AddParametersForEntityProperties(
            this Cmdlet cmdlet,
            OdcmType baseType,
            Func<OdcmProperty, bool> isReadOnlyFunc,
            string sharedParameterSetName = null,
            bool addSwitchParameters = true,
            bool markAsPowerShellParameter = true,
            bool setBaseTypeParameterSetAsDefault = false)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }
            if (baseType == null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }

            // Don't try to add parameters for Edm types
            if (baseType.Namespace.Name.StartsWith("Edm"))
            {
                return;
            }

            // Track parameters as we visit each type
            IDictionary<OdcmType, IEnumerable<string>> parameterNameLookup = new Dictionary<OdcmType, IEnumerable<string>>();
            IDictionary<string, CmdletParameter> parameterLookup = new Dictionary<string, CmdletParameter>();

            // Visit all derived types
            baseType.VisitDerivedTypes((OdcmType type) =>
            {
                string parameterName = type.Name;
                string parameterSetName = "#" + type.FullName;

                // Determine if this is the only entity type for this cmdlet
                bool isTheOnlyType = (type == baseType && !type.GetDerivedTypes().Any());

                // Create the parameter set for this type if it doesn't already exist
                CmdletParameterSet parameterSet = cmdlet.GetOrCreateParameterSet(parameterSetName);

                // Set this as the default parameter set if it's the only type
                if ((setBaseTypeParameterSetAsDefault && type == baseType)
                    || (isTheOnlyType && markAsPowerShellParameter))
                {
                    cmdlet.DefaultParameterSetName = parameterSet.Name;
                }

                // Add a switch parameter for this type if required
                if (addSwitchParameters
                    && !(type is OdcmClass @class && @class.IsAbstract) // don't add a switch for abstract types
                    && !isTheOnlyType) // if there is only 1 type, don't add a switch parameter for it
                {
                    // Add the switch parameter
                    parameterSet.Add(new CmdletParameter(parameterName, typeof(PS.SwitchParameter))
                    {
                        Mandatory = true,
                        ParameterSetSelectorName = parameterSetName,
                        ValueFromPipelineByPropertyName = false,
                        Documentation = new CmdletParameterDocumentation()
                        {
                            Descriptions = new string[]
                            {
                                $"A switch parameter for selecting the parameter set which corresponds to the \"{type.FullName}\" type.",
                            },
                        },
                    });
                }

                // Evaluate the properties on this type
                IEnumerable<OdcmProperty> properties = type.EvaluateProperties(type == baseType);

                if (markAsPowerShellParameter)
                {
                    properties = properties.Where(prop => prop.Name != RequestProperties.Id);
                }

                // Add this type into the parmeter name lookup table
                parameterNameLookup.Add(type, properties
                    //.Where(prop => !prop.ReadOnly && !prop.IsCollection && !prop.IsLink)
                    .Select(prop => prop.Name)
                    .Distinct());

                // Add the base types' properties as parameters to this parameter set
                // NOTE: Safe lookups are not necessary since all base types are guaranteed to have already been processed
                // by the VisitDerivedTypes() method
                OdcmType currentType = type;
                while (currentType != baseType)
                {
                    // Get the next type
                    currentType = currentType.GetBaseType();

                    // Lookup the properties for this type
                    IEnumerable<string> parameterNames = parameterNameLookup[currentType];

                    // Lookup the CmdletParameter objects for each parameter name
                    IEnumerable<CmdletParameter> parameters = parameterNames.Select(paramName => parameterLookup[paramName]);

                    // Add the parameters to the parameter set
                    parameterSet.AddAll(parameters);
                }

                // Iterate over properties
                foreach (OdcmProperty property in properties)
                {
                    // Get the PowerShell type for this property from the Edm type
                    Type propertyType = property.Type.ToPowerShellType(property.IsCollection);

                    // Create the parameter for this property if it doesn't already exist
                    if (!parameterLookup.TryGetValue(property.Name, out CmdletParameter parameter))
                    {
                        // Get the valid values if it is an enum
                        IEnumerable<string> enumMembers = null;
                        if (markAsPowerShellParameter && property.Type is OdcmEnum @enum)
                        {
                            enumMembers = @enum.Members.Select(enumMember => enumMember.Name);
                        }

                        parameter = CreateEntityParameter(
                            property,
                            propertyType,
                            markAsPowerShellParameter,
                            type == baseType,
                            type.FullName,
                            isReadOnly: isReadOnlyFunc(property),
                            enumValues: enumMembers);

                        parameterLookup.Add(property.Name, parameter);
                    }
                    else if (propertyType != parameter.Type)
                    {
                        // If all uses of this parameter don't use the same type, default to System.Object
                        parameter = CreateEntityParameter(
                            property,
                            typeof(object),
                            markAsPowerShellParameter,
                            type == baseType,
                            type.FullName,
                            isReadOnly: isReadOnlyFunc(property));

                        parameterLookup[property.Name] = parameter;
                    }

                    // Save the original OData type name
                    parameter.ODataTypeFullName = property.Type.FullName;

                    // Add this type's properties as parameters to this parameter set
                    parameterSet.Add(parameter);
                }
            });

            // Get/create the shared parameter set if required (e.g. the "Post" parameter set for the "Create" cmdlet)
            if (sharedParameterSetName != null)
            {
                // Create the parameter set if it doesn't already exist
                CmdletParameterSet sharedParameterSet = cmdlet.GetOrCreateParameterSet(sharedParameterSetName);

                // All properties should be part of the shared parameter set
                sharedParameterSet.AddAll(parameterLookup.Values);
            }
        }

        private static CmdletParameter CreateEntityParameter(
            OdcmProperty property,
            Type powerShellType,
            bool markAsPowerShellParameter,
            bool isBaseType,
            string entityTypeFullName,
            bool isReadOnly = false,
            IEnumerable<string> enumValues = null)
        {
            var result = new CmdletParameter(property.Name, powerShellType)
            {
                Mandatory = property.IsRequired,
                ValueFromPipelineByPropertyName = false,
                IsPowerShellParameter = markAsPowerShellParameter && !isReadOnly,

                DerivedTypeName = markAsPowerShellParameter || isBaseType
                                ? null
                                : entityTypeFullName,
                IsExpandable = !markAsPowerShellParameter && property.IsLink, // TODO: use the annotations in the schema to determine whether the property is expandable
                IsSortable = !markAsPowerShellParameter && !property.IsCollection,
                Documentation = new CmdletParameterDocumentation()
                {
                    Descriptions = new string[] {
                        $"The \"{property.Name}\" property, of type \"{property.Type.FullName}\".",
                        $"This property is on the \"{entityTypeFullName}\" type.",
                        property.Description,
                    },
                    ValidValues = enumValues,
                }
            };

            return result;
        }

        private static OdcmCapability TryGetCapability(this OdcmObject obj, string termName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return obj.Projection?.Capabilities?
                .Where(c => c.TermName == termName)?
                .SingleOrDefault();
        }
    }
}
