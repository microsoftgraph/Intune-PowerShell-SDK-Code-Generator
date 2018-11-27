// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the MIT License.  See License in the project root for license information.

namespace Microsoft.Graph.GraphODataPowerShellSDKWriter.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Pluralize.NET;
    using Humanizer;

    /// <summary>
    /// String utilities.
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// The pluralizer which can singularize or pluralize words.
        /// </summary>
        private static Pluralizer Pluralizer = new Pluralizer();

        /// <summary>
        /// Maps long noun names to short noun names. 
        /// </summary>
        private static readonly Dictionary<string, string> ShortNounName = new Dictionary<string, string>
        {
            // Keep this list sorted
            #region DeviceAppManagement
            { "DeviceAppManagement", "IntuneDeviceAppManagement" },
            #region DeviceAppManagement_AndroidManagedAppProtections
            { "DeviceAppManagement_AndroidManagedAppProtections", "IntuneAndroidAppProtectionPolicies" },
            { "DeviceAppManagement_AndroidManagedAppProtections_Apps", "IntuneAndroidAppProtectionPoliciesApps" },
            { "DeviceAppManagement_AndroidManagedAppProtections_Assignments", "IntuneAndroidAppProtectionPoliciesAssignment" },
            { "DeviceAppManagement_AndroidManagedAppProtections_DeploymentSummary", "IntuneAndroidAppProtectionPoliciesDeploymentSummary" },
            { "DeviceAppManagement_AndroidManagedAppProtections_Assign", "IntuneAndroidAppProtectionPoliciesAssign" },
            { "DeviceAppManagement_AndroidManagedAppProtections_TargetApps", "IntuneAndroidAppProtectionPoliciesTargetApps" },
            #endregion
            #region DeviceAppManagement_DefaultManagedAppProtections
            { "DeviceAppManagement_DefaultManagedAppProtections", "IntuneDefaultAppProtectionPolicies" },
            { "DeviceAppManagement_DefaultManagedAppProtections_Apps", "IntuneDefaultAppProtectionPoliciesApps" },
            { "DeviceAppManagement_DefaultManagedAppProtections_DeploymentSummary", "IntuneDefaultAppProtectionPoliciesDeploymentSummary" },
            { "DeviceAppManagement_DefaultManagedAppProtections_TargetApps", "IntuneDefaultAppProtectionPoliciesTargetApps" },
            #endregion
            #region DeviceAppManagement_IosManagedAppProtections
            { "DeviceAppManagement_IosManagedAppProtections", "IntuneIosAppProtectionPolicies" },
            { "DeviceAppManagement_IosManagedAppProtections_Apps", "IntuneIosAppProtectionPoliciesApps" },
            { "DeviceAppManagement_IosManagedAppProtections_Assignments", "IntuneIosAppProtectionPoliciesAssignment" },
            { "DeviceAppManagement_IosManagedAppProtections_DeploymentSummary", "IntuneIosAppProtectionPoliciesDeploymentSummary" },
            { "DeviceAppManagement_IosManagedAppProtections_Assign", "IntuneIosAppProtectionPoliciesAssign" },
            { "DeviceAppManagement_IosManagedAppProtections_TargetApps", "IntuneIosAppProtectionPoliciesTargetApps" },
            #endregion      
            #region DeviceAppManagement_ManagedAppPolicies
            { "DeviceAppManagement_ManagedAppPolicies", "IntuneAppProtectionPolicies" },
            { "DeviceAppManagement_ManagedAppPolicies_Apps", "IntuneAppProtectionPoliciesApps" },
            { "DeviceAppManagement_ManagedAppPolicies_Assignments", "IntuneAppProtectionPoliciesAssignments" },
            { "DeviceAppManagement_ManagedAppPolicies_DeploymentSummary", "IntuneAppProtectionPoliciesDeploymentSummary" },
            { "DeviceAppManagement_ManagedAppPolicies_ExemptAppLockerFiles", "IntuneAppProtectionPoliciesExemptAppLockerFiles" },
            { "DeviceAppManagement_ManagedAppPolicies_ProtectedAppLockerFiles", "IntuneAppProtectionPoliciesProtectedAppLockerFiles" },
            { "DeviceAppManagement_ManagedAppPolicies_TargetApps", "IntuneAppProtectionPoliciesTargetApps" },
            #endregion
            #region DeviceAppManagement_ManagedAppRegistrations
            { "DeviceAppManagement_ManagedAppRegistrations", "IntuneManagedAppRegistrations" },            
            #endregion
            #region DeviceAppManagement_ManagedAppStatuses
            { "DeviceAppManagement_ManagedAppStatuses", "IntuneManagedAppStatus" },
            #endregion
            #region DeviceAppManagement_ManagedEBooks
            { "DeviceAppManagement_ManagedEBooks", "IntuneManagedEBooks" },
            { "DeviceAppManagement_ManagedEBooks_Assignments", "IntuneManagedEBooksAssignments"},
            { "DeviceAppManagement_ManagedEBooks_DeviceStates", "IntuneManagedEBooksDeviceStates" },
            { "DeviceAppManagement_ManagedEBooks_InstallSummary", "IntuneManagedEBooksInstallSummary" },
            { "DeviceAppManagement_ManagedEBooks_UserStateSummary", "IntuneManagedEBooksUserStateSummary" },
            { "DeviceAppManagement_ManagedEBooks_UserStateSummary_DeviceStates", "IntuneManagedEBooksUserStateSummaryDeviceStates" },
            { "DeviceAppManagement_ManagedEBooks_Assign", "IntuneManagedEBooksAssign" },
            #endregion
            #region DeviceAppManagement_MdmWindowsInformationProtectionPolicies
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies", "IntuneMdmWindowsInformationProtectionPolicies" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_Assignments", "IntuneMdmWindowsInformationProtectionPoliciesAssignments" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_ExemptAppLockerFiles", "IntuneMdmWindowsInformationProtectionPoliciesExemptAppLockerFiles" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_ProtectedAppLockerFiles", "IntuneMdmWindowsInformationProtectionPoliciesProtectedAppLockerFiles" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_Assign", "IntuneMdmWindowsInformationProtectionPoliciesAssign" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_TargetApps", "IntuneMdmWindowsInformationProtectionPoliciesTargetApps" },
            #endregion
            #region DeviceAppManagement_MobileAppCategories
            { "DeviceAppManagement_MobileAppCategories", "IntuneMobileAppCategories" },
            #endregion
            #region DeviceAppManagement_MobileAppConfigurations
            { "DeviceAppManagement_MobileAppConfigurations", "IntuneMobileAppConfigurations" },
            { "DeviceAppManagement_MobileAppConfigurations_Assignments", "IntuneMobileAppConfigurationsAssignments" },
            { "DeviceAppManagement_MobileAppConfigurations_DeviceStatuses", "IntuneMobileAppConfigurationsDeviceStatuses" },
            { "DeviceAppManagement_MobileAppConfigurations_DeviceStatusSummary", "IntuneMobileAppConfigurationsDeviceStatusSummary" },
            { "DeviceAppManagement_MobileAppConfigurations_UserStatuses", "IntuneMobileAppConfigurationsUserStatuses" },
            { "DeviceAppManagement_MobileAppConfigurations_UserStatusSummary", "IntuneMobileAppConfigurationsUserStatusSummary" },
            #endregion            
            #region DeviceAppManagement_MobileApps
            { "DeviceAppManagement_MobileApps", "IntuneMobileApps" },            
            { "DeviceAppManagement_MobileApps_Assignments", "IntuneMobileAppsAssignments" },
            { "DeviceAppManagement_MobileApps_Categories", "IntuneMobileAppsCategories" },
            { "DeviceAppManagement_MobileApps_CategoriesReferences", "IntuneMobileAppsCategoriesReferences" },
            { "DeviceAppManagement_MobileApps_ContentVersions", "IntuneMobileAppsContentVersions" },
            { "DeviceAppManagement_MobileApps_ContentVersions_Files", "IntuneMobileAppsContentVersionsFiles"},
            { "DeviceAppManagement_MobileApps_Assign", "IntuneMobileAppsAssign" },
            { "DeviceAppManagement_MobileApps_ContentVersions_Files_Commit", "IntuneMobileAppsContentVersionsFileCommit" },
            { "DeviceAppManagement_MobileApps_ContentVersions_Files_RenewUpload", "IntuneMobileAppsContentVersionsRenewUpload" },
            #endregion
            #region DeviceAppManagement_SyncMicrosoftStoreForBusinessApps
            { "DeviceAppManagement_SyncMicrosoftStoreForBusinessApps", "IntuneSyncMicrosoftStoreForBusinessApps" },
            #endregion
            #region DeviceAppManagement_TargetedManagedAppConfigurations
            { "DeviceAppManagement_TargetedManagedAppConfigurations", "IntuneTargetedManagedAppConfigurations" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_Apps", "IntuneTargetedManagedAppConfigurationsApps" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_Assignments", "IntuneTargetedManagedAppConfigurationsAssignments" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_DeploymentSummary", "IntuneTargetedManagedAppConfigurationsDeploymentSummary" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_Assign", "IntuneTargetedManagedAppConfigurationsAssign" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_TargetApps", "IntuneTargetedManagedAppConfigurationsTargetedApps" },
            #endregion
            #region DeviceAppManagement_VppTokens
            { "DeviceAppManagement_VppTokens", "IntuneVppTokens"},
            { "DeviceAppManagement_VppTokens_SyncLicenses", "IntuneVppTokensSyncLicenses" },
            #endregion
            #region DeviceAppManagement_WindowsInformationProtectionPolicies
            { "DeviceAppManagement_WindowsInformationProtectionPolicies", "IntuneWindowsInformationProtectionPolicies" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_Assignments", "IntuneWindowsInformationProtectionPoliciesAssignments" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_ExemptAppLockerFiles", "IntuneWindowsInformationProtectionPoliciesExemptAppLockerFiles" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_ProtectedAppLockerFiles", "IntuneWindowsInformationProtectionPoliciesProtectedAppLockerFiles" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_Assign", "IntuneWindowsInformationProtectionPoliciesAssign" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_TargetApps", "IntuneWindowsInformationProtectionPoliciesTargetApps"},
            #endregion
            #endregion
            #region DeviceManagement  
            { "DeviceManagement", "IntuneDeviceManagement" },
            { "DeviceManagement_ApplePushNotificationCertificate", "IntuneApplePushNotificationCertificate" },
            { "DeviceManagement_ApplePushNotificationCertificate_DownloadApplePushNotificationCertificateSigningRequest", "IntuneDownloadApplePushNotificationCertificateSigningRequest" },
            { "DeviceManagement_ConditionalAccessSettings", "IntuneConditionalAccessSettings" },
            { "DeviceManagement_DeviceCategories", "IntuneDeviceCategories" },
            { "DeviceManagement_DeviceCompliancePolicyDeviceStateSummary", "IntuneDeviceCompliancePolicyDeviceStateSummary" },
            { "DeviceManagement_DeviceCompliancePolicySettingStateSummaries", "IntuneDeviceCompliancePolicySettingStateSummaries" },
            { "DeviceManagement_DeviceCompliancePolicySettingStateSummaries_DeviceComplianceSettingStates", "IntuneDeviceComplianceSettingStates" },
            { "DeviceManagement_DeviceCompliancePolicies_Assign", "IntuneDeviceCompliancePoliciesAssign" },
            { "DeviceManagement_DeviceCompliancePolicies_ScheduleActionsForRules", "IntuneDeviceCompliancePoliciesScheduleActionsForRules" },
            { "DeviceManagement_DeviceConfigurationDeviceStateSummaries", "IntuneDeviceConfigurationDeviceStateSummaries" },
            { "DeviceManagement_DeviceManagementPartners", "IntuneDeviceManagementPartners" },
            { "DeviceManagement_ExchangeConnectors", "IntuneExchangeConnectors" },
            { "DeviceManagement_IosUpdateStatuses", "IntuneIosUpdateStatuses" },
            { "DeviceManagement_ManagedDeviceOverview", "IntuneManagedDeviceOverview" },
            { "DeviceManagement_ManagedDeviceOverviewReference", "IntuneManagedDeviceOverviewReference" }, //BUGBUG: Missing Route
            { "DeviceManagement_MobileThreatDefenseConnectors", "IntuneMobileThreatDefenseConnectors" },
            { "DeviceManagement_TroubleshootingEvents", "IntuneTroubleshootingEvents"},
            { "DeviceManagement_WindowsInformationProtectionAppLearningSummaries", "IntuneWindowsInformationProtectionAppLearningSummaries" },
            { "DeviceManagement_WindowsInformationProtectionNetworkLearningSummaries", "IntuneWindowsInformationProtectionNetworkLearningSummaries" },
            { "DeviceManagement_RemoteAssistancePartners", "IntuneRemoteAssistancePartners" },
            { "DeviceManagement_ResourceOperations", "IntuneResourceOperations" },
            { "DeviceManagement_SoftwareUpdateStatusSummary", "IntuneSoftwareUpdateStatusSummary" },
            { "DeviceManagement_SoftwareUpdateStatusSummaryReference", "IntuneSoftwareUpdateStatusSummaryReference" }, //BUGBUG: Missing Route
            { "DeviceManagement_TelecomExpenseManagementPartners", "IntuneTelecomExpenseManagementPartners" },            
            { "DeviceManagement_DeviceEnrollmentConfigurations_Assign", "IntuneDeviceEnrollmentConfigurationsAssign" },
            { "DeviceManagement_DeviceEnrollmentConfigurations_SetPriority", "IntuneDeviceEnrollmentConfigurationsSetPriority" },
            { "DeviceManagement_ExchangeConnectors_Sync", "IntuneExchangeConnectorsSync" },
            { "DeviceManagement_GetEffectivePermissions", "IntuneGetEffectivePermissions" },
            { "DeviceManagement_VerifyWindowsEnrollmentAutoDiscovery", "IntuneVerifyWindowsEnrollmentAutoDiscovery" },
            #region DeviceManagement_DetectedApps
            { "DeviceManagement_DetectedApps", "IntuneDetectedApps" },
            { "DeviceManagement_DetectedApps_ManagedDevices", "IntuneDetectedAppDevices" },
            { "DeviceManagement_DetectedApps_ManagedDevicesReferences", "IntuneManagedDevicesReferences" },
            #endregion
            #region DeviceManagement_DeviceCompliancePolicies
            { "DeviceManagement_DeviceCompliancePolicies", "IntuneDeviceCompliancePolicies" },
            { "DeviceManagement_DeviceCompliancePolicies_Assignments", "IntuneDeviceCompliancePolicyAssignments" },
            { "DeviceManagement_DeviceCompliancePolicies_DeviceSettingStateSummaries", "IntuneDeviceCompliancePolicyDeviceSettingStateSummaries" },
            { "DeviceManagement_DeviceCompliancePolicies_DeviceStatuses", "IntuneDeviceCompliancePolicyDeviceStatuses" },
            { "DeviceManagement_DeviceCompliancePolicies_DeviceStatusOverview", "IntuneDeviceCompliancePolicyDeviceStatusOverview" },
            { "DeviceManagement_DeviceCompliancePolicies_ScheduledActionsForRule", "IntuneDeviceCompliancePolicyScheduledActionsForRule" },
            { "DeviceManagement_DeviceCompliancePolicies_ScheduledActionsForRule_ScheduledActionConfigurations", "IntuneDeviceCompliancePolicyScheduledActionsForRuleConfigs" },
            { "DeviceManagement_DeviceCompliancePolicies_UserStatuses", "IntuneDeviceCompliancePolicyUserStatuses" },
            { "DeviceManagement_DeviceCompliancePolicies_UserStatusOverview", "IntuneDeviceCompliancePolicyUserStatusOverview" },
            #endregion
            #region DeviceManagement_DeviceConfigurations
            { "DeviceManagement_DeviceConfigurations", "IntuneDeviceConfigurations" },
            { "DeviceManagement_DeviceConfigurations_Assignments", "IntuneDeviceConfigurationsAssignments" },
            { "DeviceManagement_DeviceConfigurations_DeviceStatusOverview", "IntuneDeviceConfigurationsDeviceStatusOverview" },
            { "DeviceManagement_DeviceConfigurations_UserStatuses", "IntuneDeviceConfigurationsUserStatuses" },
            { "DeviceManagement_DeviceConfigurations_UserStatusOverview", "IntuneDeviceConfigurationsUserStatusOverview" },
            { "DeviceManagement_DeviceConfigurations_DeviceSettingStateSummaries", "IntuneDeviceConfigurationsDeviceSettingStateSummaries" },
            { "DeviceManagement_DeviceConfigurations_DeviceStatuses", "IntuneDeviceConfigurationsDeviceStatuses" },
            { "DeviceManagement_DeviceConfigurations_Assign", "IntuneDeviceConfigurationsAssign" },
            #endregion
            #region DeviceManagement_DeviceEnrollmentConfigurations
            { "DeviceManagement_DeviceEnrollmentConfigurations", "IntuneDeviceEnrollmentConfigurations" },
            { "DeviceManagement_DeviceEnrollmentConfigurations_Assignments", "IntuneDeviceEnrollmentConfigurationsAssignments" },
            #endregion
            #region DeviceManagement_ManagedDevices
            { "DeviceManagement_ManagedDevices", "IntuneManagedDevices" },
            { "DeviceManagement_ManagedDevices_DeviceCategory", "IntuneManagedDevicesDeviceCategory" },
            { "DeviceManagement_ManagedDevices_DeviceCompliancePolicyStates", "IntuneManagedDevicesDeviceCompliancePolicyStates" },
            { "DeviceManagement_ManagedDevices_DeviceConfigurationStates", "IntuneManagedDevicesDeviceConfigurationStates" },
            { "DeviceManagement_ManagedDevices_BypassActivationLock", "IntuneManagedDevicesBypassActivationLock" },
            { "DeviceManagement_ManagedDevices_CleanWindowsDevice", "IntuneManagedDevicesCleanWindowsDevice" },
            { "DeviceManagement_ManagedDevices_DeleteUserFromSharedAppleDevice", "IntuneManagedDevicesDeleteUserFromSharedAppleDevice" },
            { "DeviceManagement_ManagedDevices_DisableLostMode", "IntuneManagedDevicesDisableLostMode" },
            { "DeviceManagement_ManagedDevices_LocateDevice", "IntuneManagedDevicesLocateDevice" },
            { "DeviceManagement_ManagedDevices_LogoutSharedAppleDeviceActiveUser", "IntuneLogoutSharedAppleDeviceActiveUser" },
            { "DeviceManagement_ManagedDevices_RebootNow", "IntuneManagedDevicesRebootNow" },
            { "DeviceManagement_ManagedDevices_RecoverPasscode", "IntuneManagedDevicesRecoverPasscode" },
            { "DeviceManagement_ManagedDevices_RemoteLock", "IntuneManagedDevicesRemoteLock" },
            { "DeviceManagement_ManagedDevices_RequestRemoteAssistance", "IntuneMangedDeviceRequestRemoteAssistance" },
            { "DeviceManagement_ManagedDevices_Retire", "IntuneManagedDevicesRetire" },
            { "DeviceManagement_ManagedDevices_ShutDown", "IntuneManagedDevicesShutDownDevice"},
            { "DeviceManagement_ManagedDevices_SyncDevice", "IntuneManagedDevicesSyncDevice" },
            { "DeviceManagement_ManagedDevices_UpdateWindowsDeviceAccount", "IntuneManagedDevicesUpdateWindowsDeviceAccount"},
            { "DeviceManagement_ManagedDevices_WindowsDefenderScan", "IntuneManagedDevicesWindowsDefenderScan"},
            { "DeviceManagement_ManagedDevices_WindowsDefenderUpdateSignatures", "IntuneManagedDevicesWindowsDefenderUpdateSignatures" },
            { "DeviceManagement_ManagedDevices_Wipe", "IntuneManagedDevicesWipeDevice" },

            #endregion
            #region DeviceManagement_NotificationMessageTemplates
            { "DeviceManagement_NotificationMessageTemplates", "IntuneNotificationMessageTemplates"},
            { "DeviceManagement_NotificationMessageTemplates_LocalizedNotificationMessages", "IntuneLocalizedNotificationMessages" },
            { "DeviceManagement_NotificationMessageTemplates_SendTestMessage", "IntuneSendTestMessage" },
            #endregion
            #region DeviceManagement_RoleAssignments
            { "DeviceManagement_RoleAssignments", "IntuneRoleAssignments" },            
            #endregion
            #region DeviceManagement_RoleDefinitions
            { "DeviceManagement_RoleDefinitions", "IntuneRoleDefinitions" },
            #endregion
            #region DeviceManagement_RemoteAssistancePartners_BeginOnboarding
            { "DeviceManagement_RemoteAssistancePartners_BeginOnboarding", "IntuneRemoteAssistancePartnersBeginOnboarding" },
            { "DeviceManagement_RemoteAssistancePartners_Disconnect", "IntuneRemoteAssistancePartnersDisconnect" },
            #endregion
            #region DeviceManagement_TermsAndConditions
            { "DeviceManagement_TermsAndConditions", "IntuneTermsAndConditionss" },
            { "DeviceManagement_TermsAndConditions_AcceptanceStatuses", "IntuneTermsAndConditionsAcceptanceStatuses" },
            { "DeviceManagement_TermsAndConditions_Assignments", "IntuneTermsAndConditionsAssignments" },
            #endregion
            #endregion
            #region Groups
            { "Groups_CreatedOnBehalfOf", "GroupsCreatedOnBehalfOf" },
            { "Groups_CreatedOnBehalfOfReference", "GroupsCreatedOnBehalfOfReference" },
            { "Groups_GroupLifecyclePolicies", "GroupsGroupLifecyclePolicies" },
            { "Groups_MemberOf", "GroupsMemberOf" },
            { "Groups_MemberOfReferences", "GroupsMemberOfReferences" },
            { "Groups_Members", "GroupsMembers" },
            { "Groups_MembersReferences", "GroupsMembersReferences" },
            { "Groups_Owners", "GroupsOwners" },
            { "Groups_OwnersReferences", "GroupsOwnersReferences" },
            { "Groups_Photo", "GroupsPhoto" },
            { "Groups_PhotoData", "GroupsPhotoData" },
            { "Groups_Photos", "GroupsPhotos" },
            { "Groups_PhotosData", "GroupsPhotosData" },
            { "Groups_Settings", "GroupsSettings" },
            { "Groups_AddFavorite", "GroupsAddFavorite" },
            { "Groups_CheckMemberGroups", "GroupsCheckMemberGroups" },
            { "Groups_Delta", "GroupsDelta" },
            { "Groups_GetByIds", "Groups_GetByIds" },
            { "Groups_GetMemberGroups", "GroupsGetMemberGroups" },
            { "Groups_GetMemberObjects", "GroupsGetMemberObjects" },
            { "Groups_GroupLifecyclePolicies_AddGroup", "GroupsGroupLifecyclePoliciesAddGroup" },
            { "Groups_GroupLifecyclePolicies_RemoveGroup", "GroupsGroupLifecyclePoliciesRemoveGroup" },
            { "Groups_RemoveFavorite", "GroupsRemoveFavorite" },
            { "Groups_Renew", "GroupsRenew" },
            { "Groups_ResetUnseenCount", "GroupsResetUnseenCount" },
            { "Groups_Restore", "GroupsRestore" },
            { "Groups_SubscribeByMail", "GroupsSubscribeByMail" },
            { "Groups_UnsubscribeByMail", "GroupsUnsubscribeByMail" }
            #endregion           
        };

        /// <summary>
        /// The default indent string.
        /// </summary>
        public const string DefaultIndentToken = "    ";        

        /// <summary>
        /// Indents a string by the desired amount.
        /// </summary>
        /// <param name="stringToIndent">The string to indent</param>
        /// <param name="indentLevel">The desired indentation level</param>
        /// <param name="indentToken">The string which represents a single indent (defaults to <see cref="DefaultIndentToken"/>)</param>
        /// <returns>The indented string.</returns>
        public static string Indent(this string stringToIndent, int indentLevel = 1, string indentToken = DefaultIndentToken)
        {
            if (stringToIndent == null)
            {
                throw new ArgumentNullException(nameof(stringToIndent));
            }

            // Precalculate the indent string since this won't change
            string indentString = GetIndentPrefix(indentLevel, indentToken);

            // Indent the string using a StringReader and not String.Replace() because
            // we don't know what kind of newline char is used in this string
            using (StringReader reader = new StringReader(stringToIndent))
            {
                IList<string> lines = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        lines.Add(indentString + line);
                    }
                    else
                    {
                        lines.Add(line);
                    }
                }

                string resultString = string.Join(Environment.NewLine, lines);
                return resultString;
            }
        }

        /// <summary>
        /// Returns a string which can be prefixed to a line of text to produce the desired indentation amount.
        /// </summary>
        /// <param name="indentLevel">The desired level of indentation</param>
        /// <param name="indentToken">The string which represents an indent level of 1</param>
        /// <returns>The string to which a line of text should be appended to produce the desired level of indentation.</returns>
        public static string GetIndentPrefix(int indentLevel, string indentToken = DefaultIndentToken)
        {
            if (indentLevel > 0)
            {
                return string.Join(string.Empty, Enumerable.Repeat(indentToken, indentLevel));
            }
            else if (indentLevel == 0)
            {
                return string.Empty;
            }
            else
            {
                throw new ArgumentException("Indent level must be greater than or equal to 0", nameof(indentLevel));
            }
        }

        public static string Pluralize(this string singular)
        {
            if (singular == null)
            {
                throw new ArgumentNullException(nameof(singular));
            }

            return Pluralizer.Pluralize(singular);
        }

        public static string Singularize(this string plural)
        {
            if (plural == null)
            {
                throw new ArgumentNullException(nameof(plural));
            }

            return Pluralizer.Singularize(plural);
        }

        public static string Pascalize(this string identifier)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            return InflectorExtensions.Pascalize(identifier);
        }

        /// <summary>
        /// Shortens the Noun names, otherwise they get too long.
        /// </summary>
        /// <param name="identifier">Name of the noun to shorten.</param>
        /// <returns></returns>
        public static string Shorten(this string identifier)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            return ShortNounName.ContainsKey(identifier) ? ShortNounName[identifier] : identifier;
        }
    }
}
