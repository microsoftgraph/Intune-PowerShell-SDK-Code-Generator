// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using Vipr.Core.CodeModel;

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
            { "Edm.Boolean", typeof(bool) },

            // String
            { "Edm.String", typeof(string) },

            // Byte
            { "Edm.Byte", typeof(byte) },
            { "Edm.SByte", typeof(sbyte) },
            { "Edm.Stream", typeof(Stream) },

            // Integer
            { "Edm.Int16", typeof(short) },
            { "Edm.UInt16", typeof(UInt16) },
            { "Edm.Int32", typeof(int) },
            { "Edm.UInt32", typeof(UInt32) },
            { "Edm.Int64", typeof(long) },
            { "Edm.UInt64", typeof(UInt64) },

            // Decimal
            { "Edm.Single", typeof(Single) },
            { "Edm.Double", typeof(double) },
            { "Edm.Decimal", typeof(decimal) },

            // Guid
            { "Edm.Guid", typeof(Guid) },

            // Date/Time
            { "Edm.Date", typeof(DateTimeOffset) },
            { "Edm.DateTime", typeof(DateTimeOffset) },
            { "Edm.DateTimeOffset", typeof(DateTimeOffset) },
            { "Edm.TimeOfDay", typeof(TimeSpan) },
            { "Edm.Time", typeof(TimeSpan) },
            { "Edm.Duration", typeof(TimeSpan) },
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

            // Track the properties
            // NOTE: Overridden properties in subtypes is not allowed in OData v4, so we don't need to worry about duplicated properties
            IList<OdcmProperty> properties = new List<OdcmProperty>();

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
                        properties.Add(property);
                    }
                }
            }

            return properties;
        }

        /// <summary>
        /// Gets the immediate base type.  Use <see cref="GetBaseTypes(OdcmType)"/> to get the full chain of base types.
        /// </summary>
        /// <param name="type">The type to evaluate</param>
        /// <returns>The type's immediate base type.</returns>
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

            if (type is OdcmClass @class)
            {
                ICollection<OdcmClass> result = new List<OdcmClass>();
                OdcmClass currentClass = @class.Base;
                while (currentClass != null)
                {
                    result.Add(currentClass);
                    currentClass = currentClass.Base;
                }

                return result;
            }
            else
            {
                return Enumerable.Empty<OdcmClass>();
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
        /// Checks whether the resource at this route represents a reference
        /// (i.e. a navigation property which is a collection and not contained).
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
    }
}
