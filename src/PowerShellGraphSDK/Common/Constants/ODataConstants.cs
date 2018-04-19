// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK
{
    public static class ODataConstants
    {
        public static class QueryParameters
        {
            public const string Select = "$select";
            public const string Expand = "$expand";

            public const string Filter = "$filter";
            public const string OrderBy = "$orderBy";
            public const string Skip = "$skip";
            public const string Top = "$top";
        }

        public static class SearchResultProperties
        {
            public const string Context = "@odata.context";
            public const string Count = "@odata.count";
            public const string NextLink = "@odata.nextLink";
            public const string Value = "value";
        }

        public static class ObjectProperties
        {
            public const string Type = "@odata.type";
        }
    }
}
