// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    public static class OdcmObjectTypeExtensions
    {
        public static bool IsClass(this OdcmObjectType type)
        {
            switch (type)
            {
                case OdcmObjectType.Class:
                case OdcmObjectType.ComplexClass:
                case OdcmObjectType.EntityClass:
                case OdcmObjectType.ServiceClass:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsProperty(this OdcmObjectType type)
        {
            switch (type)
            {
                case OdcmObjectType.Property:
                case OdcmObjectType.SingletonProperty:
                case OdcmObjectType.EntitySetProperty:
                    return true;

                default:
                    return false;
            }
        }
    }
}
