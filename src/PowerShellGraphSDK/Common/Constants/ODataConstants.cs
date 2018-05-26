// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace ODataConstants
{
    /// <summary>
    /// The valid OData query parameter names.
    /// </summary>
    public static class QueryParameters
    {
        /// <summary>
        /// $select
        /// </summary>
        public const string Select = "$select";

        /// <summary>
        /// $expand
        /// </summary>
        public const string Expand = "$expand";

        /// <summary>
        /// $filter
        /// </summary>
        public const string Filter = "$filter";

        /// <summary>
        /// $orderBy
        /// </summary>
        public const string OrderBy = "$orderBy";

        /// <summary>
        /// $skip
        /// </summary>
        public const string Skip = "$skip";

        /// <summary>
        /// $top
        /// </summary>
        public const string Top = "$top";
    }

    /// <summary>
    /// The properties that can be returned in a search result (i.e. GET on a collection).
    /// </summary>
    public static class SearchResultProperties
    {
        /// <summary>
        /// @odata.context
        /// </summary>
        public const string ODataContext = "@odata.context";

        /// <summary>
        /// @odata.count
        /// </summary>
        public const string ODataCount = "@odata.count";

        /// <summary>
        /// @odata.nextLink
        /// </summary>
        public const string ODataNextLink = "@odata.nextLink";

        /// <summary>
        /// value
        /// </summary>
        public const string Value = "value";

        /// <summary>
        /// id
        /// </summary>
        public const string Id = ODataConstants.RequestProperties.Id;
    }

    /// <summary>
    /// The properties that can be sent as part of a request body.
    /// </summary>
    public static class RequestProperties
    {
        /// <summary>
        /// @odata.type
        /// </summary>
        public const string ODataType = "@odata.type";

        /// <summary>
        /// @odata.id
        /// </summary>
        public const string ODataId = "@odata.id";

        /// <summary>
        /// id
        /// </summary>
        public const string Id = "id";
    }
}
