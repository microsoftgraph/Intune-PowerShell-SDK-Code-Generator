// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK
{
    using System;
    using System.Linq;
    using System.Management.Automation;

    public class ValidateTypeAttribute : ValidateEnumeratedArgumentsAttribute
    {
        private Type[] Types { get; set; }

        public ValidateTypeAttribute(params Type[] types)
        {
            if (types == null || !types.Any())
            {
                throw new ArgumentException("The list of types cannot be null or empty", nameof(types));
            }

            this.Types = types;
        }

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
