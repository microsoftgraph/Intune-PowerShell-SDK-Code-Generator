// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using Microsoft.Graph.GraphODataPowerShellSDKWriter.Generator.Models;
    using System.Collections.Generic;
    using Vipr.Core.CodeModel;

    public static class OdcmUtils
    {
        public static string EvaluatePath(this OdcmNode node)
        {
            Stack<string> segments = new Stack<string>();

            segments.Push(node.OdcmProperty.Name);

            OdcmNode currentNode = node;
            while (currentNode.Parent != null)
            {
                currentNode = currentNode.Parent;
                if (currentNode.OdcmProperty.IsEnumeration())
                {
                    segments.Push(currentNode.OdcmProperty.Name + "/{id}");
                }
                else
                {
                    segments.Push(currentNode.OdcmProperty.Name);
                }
            }

            string result = string.Join("/", segments);

            return result;
        }

        public static bool IsEnumeration(this OdcmProperty property)
        {
            return property.IsCollection;
        }
    }
}
