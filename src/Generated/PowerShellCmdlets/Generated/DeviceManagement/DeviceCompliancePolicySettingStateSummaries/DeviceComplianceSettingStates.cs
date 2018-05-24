// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace PowerShellGraphSDK.PowerShellCmdlets
{
    using System.Management.Automation;

    /// <summary>
    ///     <para type="synopsis">Retrieves &quot;microsoft.graph.deviceComplianceSettingState&quot; objects.</para>
    ///     <para type="description">GET ~/deviceManagement/deviceCompliancePolicySettingStateSummaries/{deviceCompliancePolicySettingStateSummaryId}/deviceComplianceSettingStates</para>
    ///     <para type="description">Retrieves &quot;microsoft.graph.deviceComplianceSettingState&quot; objects in the &quot;deviceComplianceSettingStates&quot; collection.</para>
    /// </summary>
    [Cmdlet("Get", "DeviceManagement_DeviceCompliancePolicySettingStateSummaries_DeviceComplianceSettingStateReferences", DefaultParameterSetName = @"Search")]
    [ODataType("microsoft.graph.deviceComplianceSettingState")]
    public class Get_DeviceManagement_DeviceCompliancePolicySettingStateSummaries_DeviceComplianceSettingStateReferences : GetOrSearchCmdlet
    {
        /// <summary>
        ///     <para type="description">A required ID for referencing a &quot;microsoft.graph.deviceCompliancePolicySettingStateSummary&quot; object in the &quot;deviceCompliancePolicySettingStateSummaries&quot; collection</para>
        /// </summary>
        [Selectable]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = @"A required ID for referencing a &quot;microsoft.graph.deviceCompliancePolicySettingStateSummary&quot; object in the &quot;deviceCompliancePolicySettingStateSummaries&quot; collection")]
        public System.String deviceCompliancePolicySettingStateSummaryId { get; set; }

        /// <summary>
        ///     <para type="description">The ID for a &quot;microsoft.graph.deviceComplianceSettingState&quot; object in the &quot;deviceComplianceSettingStates&quot; collection</para>
        /// </summary>
        [Selectable]
        [Alias("id")]
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = @"Get", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = @"The ID for a &quot;microsoft.graph.deviceComplianceSettingState&quot; object in the &quot;deviceComplianceSettingStates&quot; collection")]
        public System.String deviceComplianceSettingStateId { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;setting&quot; property, of type &quot;Edm.String&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceComplianceSettingState&quot; type.</para>
        ///     <para type="description">The setting class name and property name.</para>
        /// </summary>
        [ODataType("Edm.String")]
        [Selectable]
        [Sortable]
        public System.String setting { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;settingName&quot; property, of type &quot;Edm.String&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceComplianceSettingState&quot; type.</para>
        ///     <para type="description">The Setting Name that is being reported</para>
        /// </summary>
        [ODataType("Edm.String")]
        [Selectable]
        [Sortable]
        public System.String settingName { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;deviceId&quot; property, of type &quot;Edm.String&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceComplianceSettingState&quot; type.</para>
        ///     <para type="description">The Device Id that is being reported</para>
        /// </summary>
        [ODataType("Edm.String")]
        [Selectable]
        [Sortable]
        public System.String deviceId { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;deviceName&quot; property, of type &quot;Edm.String&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceComplianceSettingState&quot; type.</para>
        ///     <para type="description">The Device Name that is being reported</para>
        /// </summary>
        [ODataType("Edm.String")]
        [Selectable]
        [Sortable]
        public System.String deviceName { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;userId&quot; property, of type &quot;Edm.String&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceComplianceSettingState&quot; type.</para>
        ///     <para type="description">The user Id that is being reported</para>
        /// </summary>
        [ODataType("Edm.String")]
        [Selectable]
        [Sortable]
        public System.String userId { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;userEmail&quot; property, of type &quot;Edm.String&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceComplianceSettingState&quot; type.</para>
        ///     <para type="description">The User email address that is being reported</para>
        /// </summary>
        [ODataType("Edm.String")]
        [Selectable]
        [Sortable]
        public System.String userEmail { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;userName&quot; property, of type &quot;Edm.String&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceComplianceSettingState&quot; type.</para>
        ///     <para type="description">The User Name that is being reported</para>
        /// </summary>
        [ODataType("Edm.String")]
        [Selectable]
        [Sortable]
        public System.String userName { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;userPrincipalName&quot; property, of type &quot;Edm.String&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceComplianceSettingState&quot; type.</para>
        ///     <para type="description">The User PrincipalName that is being reported</para>
        /// </summary>
        [ODataType("Edm.String")]
        [Selectable]
        [Sortable]
        public System.String userPrincipalName { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;deviceModel&quot; property, of type &quot;Edm.String&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceComplianceSettingState&quot; type.</para>
        ///     <para type="description">The device model that is being reported</para>
        /// </summary>
        [ODataType("Edm.String")]
        [Selectable]
        [Sortable]
        public System.String deviceModel { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;state&quot; property, of type &quot;microsoft.graph.complianceStatus&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceComplianceSettingState&quot; type.</para>
        ///     <para type="description">The compliance state of the setting</para>
        /// </summary>
        [ODataType("microsoft.graph.complianceStatus")]
        [Selectable]
        [Sortable]
        public System.String state { get; set; }

        /// <summary>
        ///     <para type="description">The &quot;complianceGracePeriodExpirationDateTime&quot; property, of type &quot;Edm.DateTimeOffset&quot;.</para>
        ///     <para type="description">This property is on the &quot;microsoft.graph.deviceComplianceSettingState&quot; type.</para>
        ///     <para type="description">The DateTime when device compliance grace period expires</para>
        /// </summary>
        [ODataType("Edm.DateTimeOffset")]
        [Selectable]
        [Sortable]
        public System.DateTimeOffset complianceGracePeriodExpirationDateTime { get; set; }

        internal override System.String GetResourcePath()
        {
            return $"deviceManagement/deviceCompliancePolicySettingStateSummaries/{deviceCompliancePolicySettingStateSummaryId}/deviceComplianceSettingStates/{deviceComplianceSettingStateId ?? string.Empty}";
        }
    }

    /// <summary>
    ///     <para type="synopsis">Removes a reference from a &quot;deviceCompliancePolicySettingStateSummary&quot; to a &quot;microsoft.graph.deviceComplianceSettingState&quot; object.</para>
    ///     <para type="description">DELETE ~/deviceManagement/deviceCompliancePolicySettingStateSummaries/{deviceCompliancePolicySettingStateSummaryId}/deviceComplianceSettingStates</para>
    ///     <para type="description">Removes a reference from the specified &quot;deviceCompliancePolicySettingStateSummary&quot; object to a &quot;deviceComplianceSettingState&quot;.</para>
    /// </summary>
    [Cmdlet("Remove", "DeviceManagement_DeviceCompliancePolicySettingStateSummaries_DeviceComplianceSettingStateReferences", ConfirmImpact = ConfirmImpact.High)]
    [ODataType("microsoft.graph.deviceComplianceSettingState")]
    public class Remove_DeviceManagement_DeviceCompliancePolicySettingStateSummaries_DeviceComplianceSettingStateReferences : DeleteCmdlet
    {
        /// <summary>
        ///     <para type="description">The ID for a &quot;microsoft.graph.deviceComplianceSettingState&quot; object in the &quot;deviceComplianceSettingStates&quot; collection</para>
        /// </summary>
        [Selectable]
        [Alias("id")]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = @"The ID for a &quot;microsoft.graph.deviceComplianceSettingState&quot; object in the &quot;deviceComplianceSettingStates&quot; collection")]
        public System.String deviceComplianceSettingStateId { get; set; }

        /// <summary>
        ///     <para type="description">A required ID for referencing a &quot;microsoft.graph.deviceCompliancePolicySettingStateSummary&quot; object in the &quot;deviceCompliancePolicySettingStateSummaries&quot; collection</para>
        /// </summary>
        [Selectable]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = @"A required ID for referencing a &quot;microsoft.graph.deviceCompliancePolicySettingStateSummary&quot; object in the &quot;deviceCompliancePolicySettingStateSummaries&quot; collection")]
        public System.String deviceCompliancePolicySettingStateSummaryId { get; set; }

        internal override System.String GetResourcePath()
        {
            return $"deviceManagement/deviceCompliancePolicySettingStateSummaries/{deviceCompliancePolicySettingStateSummaryId}/deviceComplianceSettingStates/{deviceComplianceSettingStateId}/$ref";
        }
    }

    /// <summary>
    ///     <para type="synopsis">Creates a reference from a &quot;deviceCompliancePolicySettingStateSummary&quot; to a &quot;microsoft.graph.deviceComplianceSettingState&quot; object.</para>
    ///     <para type="description">POST ~/deviceManagement/deviceCompliancePolicySettingStateSummaries/{deviceCompliancePolicySettingStateSummaryId}/deviceComplianceSettingStates</para>
    ///     <para type="description">Creates a reference from the specified &quot;deviceCompliancePolicySettingStateSummary&quot; object to a &quot;deviceComplianceSettingState&quot;.</para>
    /// </summary>
    [Cmdlet("New", "DeviceManagement_DeviceCompliancePolicySettingStateSummaries_DeviceComplianceSettingStateReferences", ConfirmImpact = ConfirmImpact.Low)]
    [ODataType("microsoft.graph.deviceComplianceSettingState")]
    public class New_DeviceManagement_DeviceCompliancePolicySettingStateSummaries_DeviceComplianceSettingStateReferences : PostReferenceToCollectionCmdlet
    {
        /// <summary>
        ///     <para type="description">The ID for a &quot;microsoft.graph.deviceComplianceSettingState&quot; object in the &quot;deviceComplianceSettingStates&quot; collection</para>
        /// </summary>
        [Selectable]
        [Alias("id")]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = @"The ID for a &quot;microsoft.graph.deviceComplianceSettingState&quot; object in the &quot;deviceComplianceSettingStates&quot; collection")]
        public System.String deviceComplianceSettingStateId { get; set; }

        /// <summary>
        ///     <para type="description">A required ID for referencing a &quot;microsoft.graph.deviceCompliancePolicySettingStateSummary&quot; object in the &quot;deviceCompliancePolicySettingStateSummaries&quot; collection</para>
        /// </summary>
        [Selectable]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = @"A required ID for referencing a &quot;microsoft.graph.deviceCompliancePolicySettingStateSummary&quot; object in the &quot;deviceCompliancePolicySettingStateSummaries&quot; collection")]
        public System.String deviceCompliancePolicySettingStateSummaryId { get; set; }

        internal override System.String GetResourcePath()
        {
            return $"deviceManagement/deviceCompliancePolicySettingStateSummaries/{deviceCompliancePolicySettingStateSummaryId}/deviceComplianceSettingStates/$ref";
        }

        internal override System.Object GetContent()
        {
            return this.GetODataIdContent(deviceComplianceSettingStateId);
        }
    }
}