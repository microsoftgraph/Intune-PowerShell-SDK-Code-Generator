// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    public enum OdcmObjectType
    {
        // Objects
        Class,
        ServiceClass,
        ComplexClass,
        EntityClass,

        // Properties
        Property,
        EntitySetProperty,
        SingletonProperty,

        // Primitives
        PrimitiveType,
        Enum,
        Method,
        TypeDefinition,
    }
}
