// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class CSharpTypeUtils
    {
        /// <summary>
        /// Gets the underlying type for an array type, and also returns the number of dimensions.
        /// </summary>
        /// <param name="arrayType">The type that represents an array</param>
        /// <param name="dimensions">The number of dimensions - if the provided type is not an array type, this will be zero</param>
        /// <returns>
        /// The underlying type (i.e. the type when all dimensions are removed and it is no longer an array).
        /// If the provided type is not an array type, this method will return the provided type.
        /// </returns>
        public static Type GetArrayUnderlyingType(this Type arrayType, out int dimensions)
        {
            if (arrayType == null)
            {
                throw new ArgumentNullException(nameof(arrayType));
            }

            // Create a temporary type so we don't lose the reference to the passed-in type
            Type tempType = arrayType;

            // Unwrap the array
            int arrayDimensions = 0;
            while (tempType.IsArray)
            {
                tempType = tempType.GetElementType();
                arrayDimensions++;
            }

            dimensions = arrayDimensions;
            return tempType;
        }

        /// <summary>
        /// Converts a type to a C# string that can be compiled in code.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The C# string</returns>
        public static string ToCSharpString(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // Unwrap the array if required
            Type tempType = type.GetArrayUnderlyingType(out int arrayDimensions);

            // Get the C# type name
            string typeName;
            if (!tempType.IsGenericType)
            {
                typeName = tempType.Name;
            }
            else
            {
                // Get the full name of the type without it's generic arguments (i.e. everything before the backtick)
                string typeNameWithoutArguments = tempType.Name.Substring(0, tempType.Name.IndexOf('`'));

                // Get the type arguments
                IEnumerable<string> typeArguments = tempType.GenericTypeArguments.Select(typeArgument => typeArgument.ToCSharpString());
                string typeArgumentsString = string.Join(", ", typeArguments);

                // Create the full type name with arguments
                typeName = $"{typeNameWithoutArguments}<{typeArgumentsString}>";
            }

            // Check if this type is nested inside another type
            if (tempType.DeclaringType != null)
            {
                // Prepend the declaring type
                typeName = $"{tempType.DeclaringType.ToCSharpString()}.{typeName}";
            }
            else if (!string.IsNullOrEmpty(tempType.Namespace))
            {
                // Add the namespace
                typeName = $"{tempType.Namespace}.{typeName}";
            }

            // Add the array brackets back into the type name
            while (arrayDimensions > 0)
            {
                typeName += "[]";
                arrayDimensions--;
            }

            return typeName;
        }
    }
}
