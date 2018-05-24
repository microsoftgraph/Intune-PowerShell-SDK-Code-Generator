// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    using System.Management.Automation;

    /// <summary>
    ///     <para type="synopsis">Retrieves &quot;microsoft.graph.deviceInstallState&quot; objects.</para>
    ///     <para type="description">GET ~/deviceAppManagement/managedEBooks/{managedEBookId}/deviceStates</para>
    ///     <para type="description">Retrieves &quot;microsoft.graph.deviceInstallState&quot; objects in the &quot;deviceStates&quot; collection.</para>
    ///     <para type="description">The list of installation states for this eBook.</para>
    /// </summary>
    [Cmdlet("Get", "DeviceAppManagement_ManagedEBooks_DeviceStateReferences", DefaultParameterSetName = @"Search")]
    [ODataType("microsoft.graph.deviceInstallState")]
    public class Get_DeviceAppManagement_ManagedEBooks_DeviceStateReferences : GetOrSearchCmdlet
    {
        /// <summary>
        ///     <para type="description">A required ID for referencing a &quot;microsoft.graph.managedEBook&quot; object in the &quot;managedEBooks&quot; collection</para>
        /// </summary>
        [Selectable]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = @"A required ID for referencing a &quot;microsoft.graph.managedEBook&quot; object in the &quot;managedEBooks&quot; collection")]
        public System.String managedEBookId { get; set; }

        /// <summary>
        ///     <para type="description">The ID for a &quot;microsoft.graph.deviceInstallState&quot; object in the &quot;deviceStates&quot; collection</para>
        /// </summary>
        [Selectable]
        [Alias("id")]
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = @"Get", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = @"The ID for a &quot;microsoft.graph.deviceInstallState&quot; object in the &quot;deviceStates&quot; collection")]
        public System.String deviceStateId { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;deviceName&quot; property, of type &quot;Edm.String&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceInstallState&quot; type.</para>
        ///     <para type="description">Device name.</para>
        /// </summary>
        [ODataType("Edm.String")]
        [Selectable]
        [Sortable]
        public System.String deviceName { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;deviceId&quot; property, of type &quot;Edm.String&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceInstallState&quot; type.</para>
        ///     <para type="description">Device Id.</para>
        /// </summary>
        [ODataType("Edm.String")]
        [Selectable]
        [Sortable]
        public System.String deviceId { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;lastSyncDateTime&quot; property, of type &quot;Edm.DateTimeOffset&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceInstallState&quot; type.</para>
        ///     <para type="description">Last sync date and time.</para>
        /// </summary>
        [ODataType("Edm.DateTimeOffset")]
        [Selectable]
        [Sortable]
        public System.DateTimeOffset lastSyncDateTime { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;installState&quot; property, of type &quot;microsoft.graph.installState&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceInstallState&quot; type.</para>
        ///     <para type="description">The install state of the eBook.</para>
        /// </summary>
        [ODataType("microsoft.graph.installState")]
        [Selectable]
        [Sortable]
        public System.String installState { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;errorCode&quot; property, of type &quot;Edm.String&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceInstallState&quot; type.</para>
        ///     <para type="description">The error code for install failures.</para>
        /// </summary>
        [ODataType("Edm.String")]
        [Selectable]
        [Sortable]
        public System.String errorCode { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;osVersion&quot; property, of type &quot;Edm.String&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceInstallState&quot; type.</para>
        ///     <para type="description">OS Version.</para>
        /// </summary>
        [ODataType("Edm.String")]
        [Selectable]
        [Sortable]
        public System.String osVersion { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;osDescription&quot; property, of type &quot;Edm.String&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceInstallState&quot; type.</para>
        ///     <para type="description">OS Description.</para>
        /// </summary>
        [ODataType("Edm.String")]
        [Selectable]
        [Sortable]
        public System.String osDescription { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;userName&quot; property, of type &quot;Edm.String&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceInstallState&quot; type.</para>
        ///     <para type="description">Device User Name.</para>
        /// </summary>
        [ODataType("Edm.String")]
        [Selectable]
        [Sortable]
        public System.String userName { get; set; }

        internal override System.String GetResourcePath()
        {
            return $"deviceAppManagement/managedEBooks/{managedEBookId}/deviceStates/{deviceStateId ?? string.Empty}";
        }
    }

    /// <summary>
    ///     <para type="synopsis">Removes a reference from a &quot;managedEBook&quot; to a &quot;microsoft.graph.deviceInstallState&quot; object.</para>
    ///     <para type="description">DELETE ~/deviceAppManagement/managedEBooks/{managedEBookId}/deviceStates</para>
    ///     <para type="description">Removes a reference from the specified &quot;managedEBook&quot; object to a &quot;deviceState&quot;.</para>
    ///     <para type="description">The list of installation states for this eBook.</para>
    /// </summary>
    [Cmdlet("Remove", "DeviceAppManagement_ManagedEBooks_DeviceStateReferences", ConfirmImpact = ConfirmImpact.High)]
    [ODataType("microsoft.graph.deviceInstallState")]
    public class Remove_DeviceAppManagement_ManagedEBooks_DeviceStateReferences : DeleteCmdlet
    {
        /// <summary>
        ///     <para type="description">The ID for a &quot;microsoft.graph.deviceInstallState&quot; object in the &quot;deviceStates&quot; collection</para>
        /// </summary>
        [Selectable]
        [Alias("id")]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = @"The ID for a &quot;microsoft.graph.deviceInstallState&quot; object in the &quot;deviceStates&quot; collection")]
        public System.String deviceStateId { get; set; }

        /// <summary>
        ///     <para type="description">A required ID for referencing a &quot;microsoft.graph.managedEBook&quot; object in the &quot;managedEBooks&quot; collection</para>
        /// </summary>
        [Selectable]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = @"A required ID for referencing a &quot;microsoft.graph.managedEBook&quot; object in the &quot;managedEBooks&quot; collection")]
        public System.String managedEBookId { get; set; }

        internal override System.String GetResourcePath()
        {
            return $"deviceAppManagement/managedEBooks/{managedEBookId}/deviceStates/{deviceStateId}/$ref";
        }
    }

    /// <summary>
    ///     <para type="synopsis">Creates a reference from a &quot;managedEBook&quot; to a &quot;microsoft.graph.deviceInstallState&quot; object.</para>
    ///     <para type="description">POST ~/deviceAppManagement/managedEBooks/{managedEBookId}/deviceStates</para>
    ///     <para type="description">Creates a reference from the specified &quot;managedEBook&quot; object to a &quot;deviceState&quot;.</para>
    ///     <para type="description">The list of installation states for this eBook.</para>
    /// </summary>
    [Cmdlet("New", "DeviceAppManagement_ManagedEBooks_DeviceStateReferences", ConfirmImpact = ConfirmImpact.Low)]
    [ODataType("microsoft.graph.deviceInstallState")]
    public class New_DeviceAppManagement_ManagedEBooks_DeviceStateReferences : PostReferenceToCollectionCmdlet
    {
        /// <summary>
        ///     <para type="description">The ID for a &quot;microsoft.graph.deviceInstallState&quot; object in the &quot;deviceStates&quot; collection</para>
        /// </summary>
        [Selectable]
        [Alias("id")]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = @"The ID for a &quot;microsoft.graph.deviceInstallState&quot; object in the &quot;deviceStates&quot; collection")]
        public System.String deviceStateId { get; set; }

        /// <summary>
        ///     <para type="description">A required ID for referencing a &quot;microsoft.graph.managedEBook&quot; object in the &quot;managedEBooks&quot; collection</para>
        /// </summary>
        [Selectable]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = @"A required ID for referencing a &quot;microsoft.graph.managedEBook&quot; object in the &quot;managedEBooks&quot; collection")]
        public System.String managedEBookId { get; set; }

        internal override System.String GetResourcePath()
        {
            return $"deviceAppManagement/managedEBooks/{managedEBookId}/deviceStates/$ref";
        }

        internal override System.Object GetContent()
        {
            return this.GetODataIdContent(deviceStateId);
        }
    }
}