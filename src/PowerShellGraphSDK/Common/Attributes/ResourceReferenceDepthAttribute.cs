// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK
{
    using System;

    /// <summary>
    /// Indicates that the class represents a cmdlet that operates on a resource that can be referenced in a "$ref" cmdlet.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal class ResourceReferenceDepthAttribute : Attribute
    {
        internal int NumSegments { get; }

        /// <summary>
        /// Creates a new <see cref="ResourceReferenceDepthAttribute"/>.
        /// </summary>
        /// <param name="numSegments">The number of segments (not including IDs) in the OData route that references this resource</param>
        internal ResourceReferenceDepthAttribute(int numSegments)
        {
            if (numSegments <= 0)
            {
                throw new ArgumentNullException("There must be 1 or more segments in the OData route.", nameof(numSegments));
            }

            this.NumSegments = numSegments;
        }
    }
}
