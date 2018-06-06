// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using PowerShellGraphSDK;
    using PowerShellGraphSDK.ODataConstants;
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
                idParameterName = $"{property.Name.Singularize()}Id";
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
        /// Determines whether the given type represents a stream.
        /// </summary>
        /// <param name="type">The ODCM type</param>
        /// <returns>True if the given type represents a stream, otherwise false.</returns>
        public static bool IsStream(this OdcmType type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // Check if either this type or one of it's base types are streams
            OdcmType currentType = type;
            while (currentType != null)
            {
                if (currentType.FullName == EdmTypeNames.Stream ||
                    currentType is OdcmMediaClass)
                {
                    return true;
                }

                currentType = currentType.GetBaseType();
            }

            return false;
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

            OdcmBooleanCapability capability = obj.TryGetCapability(AnnotationTerms.Computed) as OdcmBooleanCapability;

            return (capability != null && capability.Value == true);
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

            OdcmBooleanCapability capability = obj.TryGetCapability(AnnotationTerms.Immutable) as OdcmBooleanCapability;

            return (capability != null && capability.Value == true);
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
