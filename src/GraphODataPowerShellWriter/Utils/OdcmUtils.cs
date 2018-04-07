// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Inflector;
    using Vipr.Core.CodeModel;

    /// <summary>
    /// A set of utility methods to simplify operations on ODCM objects.
    /// </summary>
    public static class OdcmUtils
    {
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

            if (!property.IsEnumeration())
            {
                idParameterName = null;
                return false;
            }
            else
            {
                string singularName = property.Name.Singularize() ?? property.Name;
                idParameterName = $"{singularName}Id";
                return true;
            }
        }

        /// <summary>
        /// Determines whether or not a given property represents a collection.
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>True if the property represents a collection, false otherwise.</returns>
        public static bool IsEnumeration(this OdcmProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return property.IsCollection;
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
    }
}
