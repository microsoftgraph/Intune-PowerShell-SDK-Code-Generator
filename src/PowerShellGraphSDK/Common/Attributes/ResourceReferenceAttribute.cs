// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK
{
    using System;

    /// <summary>
    /// Indicates that the class represents a cmdlet that operates on a resource that can be referenced in a "$ref" cmdlet.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal class ResourceReferenceAttribute : Attribute
    {
        internal string ResourceTypeFullName { get; }

        /// <summary>
        /// Creates a new <see cref="ResourceReferenceAttribute"/>.
        /// </summary>
        /// <param name="resourceUrl">The URL at which the resource can be accessed</param>
        internal ResourceReferenceAttribute(string resourceUrl)
        {
            this.ResourceTypeFullName = resourceUrl ?? throw new ArgumentNullException(nameof(resourceUrl));
        }
    }
}
