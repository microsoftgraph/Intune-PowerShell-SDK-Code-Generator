// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System;
    using Vipr.Core.CodeModel;

    public static class OdcmUtils
    {
        /// <summary>
        /// Gets the type of an OdcmObject.
        /// </summary>
        /// <param name="obj">The OdcmObject</param>
        /// <returns>The type of the OdcmObject.</returns>
        public static OdcmObjectType GetOdcmObjectType(this OdcmObject obj)
        {
            if (obj is OdcmType type)
            {
                if (type is OdcmPrimitiveType primitiveType)
                {
                    return OdcmObjectType.PrimitiveType;
                }
                else if (type is OdcmClass @class)
                {
                    return OdcmObjectType.Class;
                }
                else if (type is OdcmEnum @enum)
                {
                    return OdcmObjectType.Enum;
                }
                else if (type is OdcmMethod method)
                {
                    return OdcmObjectType.Method;
                }
                else if (type is OdcmTypeDefinition typeDefinition)
                {
                    return OdcmObjectType.TypeDefinition;
                }
                else
                {
                    throw new ArgumentException($"Unknown {typeof(OdcmType)} type: {type.GetType()}", nameof(obj));
                }
            }
            else if (obj is OdcmProperty property)
            {
                if (property is OdcmEntitySet entitySet)
                {
                    return OdcmObjectType.EntitySetProperty;
                }
                else if (property is OdcmSingleton singleton)
                {
                    return OdcmObjectType.SingletonProperty;
                }
                else
                {
                    throw new ArgumentException($"Unknown {typeof(OdcmProperty)} type: {property.GetType()}", nameof(obj));
                }
            }
            else
            {
                throw new ArgumentException($"Unknown {typeof(OdcmObject)} type: {obj.GetType()}", nameof(obj));
            }
        }
    }
}
