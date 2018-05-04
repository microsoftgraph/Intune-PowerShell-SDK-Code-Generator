// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK
{
    using System;
    using System.Linq;
    using System.Management.Automation;

    /// <summary>
    /// Validates that the parameter is set to a value which is one of the specified types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal class ValidateTypeAttribute : ValidateEnumeratedArgumentsAttribute
    {
        /// <summary>
        /// The list of valid types.
        /// </summary>
        private Type[] Types { get; set; }

        /// <summary>
        /// Creates a new <see cref="ValidateTypeAttribute"/>.
        /// </summary>
        /// <param name="types">The list of valid types</param>
        public ValidateTypeAttribute(params Type[] types)
        {
            if (types == null || !types.Any())
            {
                throw new ArgumentException("The list of types cannot be null or empty", nameof(types));
            }

            this.Types = types;
        }

        /// <summary>
        /// Ensures that the value provided is a valid type.
        /// </summary>
        /// <param name="param"></param>
        protected override void ValidateElement(object param)
        {
            Type type = param.GetType();
            if (!Types.Contains(type))
            {
                string typesString = string.Join(", ", this.Types.Select((t) => $"'{t.ToString()}'"));
                throw new ValidationMetadataException($"The provided parameter of type '{type}' is not a valid type.  Accepted types are: [{typesString}].");
            }
        }
    }
}
