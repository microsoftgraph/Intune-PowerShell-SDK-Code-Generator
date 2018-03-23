// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System.Collections.Generic;

    /// <summary>
    /// A set of utility methods to simplify operations related to enumerables.
    /// </summary>
    public static class EnumerableUtils
    {
        /// <summary>
        /// Creates an IEnumerable which contains the given object.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="obj">The object</param>
        /// <returns>The IEnumerable.</returns>
        public static IEnumerable<T> SingleObjectAsEnumerable<T>(this T obj)
        {
            yield return obj;
        }
    }
}
