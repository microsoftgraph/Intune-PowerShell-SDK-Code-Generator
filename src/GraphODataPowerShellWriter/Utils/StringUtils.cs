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
            #region DeviceAppManagement_AndroidManagedAppProtections
            { "DeviceAppManagement_AndroidManagedAppProtections", "AmAndroidAPP" },
            { "DeviceAppManagement_AndroidManagedAppProtections_Apps", "AmAndroidAPPApps" },
            { "DeviceAppManagement_AndroidManagedAppProtections_Assignments", "AmAndroidAPPAssignments" },
            { "DeviceAppManagement_AndroidManagedAppProtections_DeploymentSummary", "AmAndroidAPPDeploymentSummary" },
            { "DeviceAppManagement_AndroidManagedAppProtections_Assign", "AmAssignAndroidAPP" },
            { "DeviceAppManagement_AndroidManagedAppProtections_TargetApps", "AmTargetAndroidAPP" },
            #endregion
            #region DeviceAppManagement_DefaultManagedAppProtections
            { "DeviceAppManagement_DefaultManagedAppProtections", "AmDefaultAPP" },
            { "DeviceAppManagement_DefaultManagedAppProtections_Apps", "AmDefaultAPPApps" },
            { "DeviceAppManagement_DefaultManagedAppProtections_DeploymentSummary", "AmDefaultAPPDeploymentSummary" },
            { "DeviceAppManagement_DefaultManagedAppProtections_TargetApps", "AmTargetDefaultAPP" },
            #endregion
            #region DeviceAppManagement_IosManagedAppProtections
            { "DeviceAppManagement_IosManagedAppProtections", "AmIosApp" },
            { "DeviceAppManagement_IosManagedAppProtections_Apps", "AmIosAppApps" },
            { "DeviceAppManagement_IosManagedAppProtections_Assignments", "AmIosAppAppsAssignments" },
            { "DeviceAppManagement_IosManagedAppProtections_DeploymentSummary", "AmIosAppAppsDeploymentSummary" },
            { "DeviceAppManagement_IosManagedAppProtections_Assign", "AmAssignIosAppApps" },
            { "DeviceAppManagement_IosManagedAppProtections_TargetApps", "AmTargetIosApp" },
            #endregion      
            #region DeviceAppManagement_ManagedAppPolicies
            { "DeviceAppManagement_ManagedAppPolicies", "AmManagedAP" },
            { "DeviceAppManagement_ManagedAppPolicies_Apps", "AmManagedAPApps" },
            { "DeviceAppManagement_ManagedAppPolicies_Assignments", "AmManagedAPAssignments" },
            { "DeviceAppManagement_ManagedAppPolicies_DeploymentSummary", "AmManagedAPDeploymentSummary" },
            { "DeviceAppManagement_ManagedAppPolicies_ExemptAppLockerFiles", "AmManagedAPExemptAppLockerFiles" },
            { "DeviceAppManagement_ManagedAppPolicies_ProtectedAppLockerFiles", "AmManagedAPProtectedAppLockerFiles" },
            { "DeviceAppManagement_ManagedAppPolicies_TargetApps", "AmTargetManagedAP" },
            #endregion
            #region DeviceAppManagement_ManagedAppRegistrations
            { "DeviceAppManagement_ManagedAppRegistrations", "AmAppRegns" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies", "AmAppRegnPolicies" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_Apps", "AmAppRegnApps" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_Assignments", "AmAppRegnAssignments" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_DeploymentSummary", "AmAppRegnDeploymentSummary" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_ExemptAppLockerFiles", "AmAppRegnExemptAppLockerFiles" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_ProtectedAppLockerFiles", "AmAppRegnProtectedAppLockerFiles" },
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies", "AmAppRegnIntendedPolicies" },
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_Apps", "AmAppRegnIntendedPoliciesApps" },
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_Assignments", "AmAppRegnIntendedPoliciesAsignments" },
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_DeploymentSummary", "AmAppRegnIntendedPoliciesDeploymentSummary"},
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_ExemptAppLockerFiles", "AmAppRegnIntendedPoliciesExemptAppLockerFiles" },
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_ProtectedAppLockerFiles", "AmAppRegnIntendedPoliciesProtectedAppLockerFiles" },            
            { "DeviceAppManagement_ManagedAppRegistrations_Operations", "AmAmAppRegnOps" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_TargetApps", "AmTargetAppRegnAppliedPolicies" },
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_TargetApps", "AmTargetAppRegnIntendedPolicies" },
            { "DeviceAppManagement_ManagedAppRegistrations_GetUserIdsWithFlaggedAppRegistration", "AmGetUserIdsWithFlaggedAppRegistration" },
            #endregion
            #region DeviceAppManagement_ManagedAppStatuses
            { "DeviceAppManagement_ManagedAppStatuses", "AmAppStatuses" },
            #endregion
            #region DeviceAppManagement_ManagedEBooks
            { "DeviceAppManagement_ManagedEBooks", "AmManagedEBooks" },
            { "DeviceAppManagement_ManagedEBooks_Assignments", "AmManagedEBookAssignments"},
            { "DeviceAppManagement_ManagedEBooks_DeviceStates", "ManagedEBooksDeviceStates" },
            { "DeviceAppManagement_ManagedEBooks_InstallSummary", "AmManagedEBooksInstallSummary" },
            { "DeviceAppManagement_ManagedEBooks_UserStateSummary", "AmManagedEBooksUserStateSummary" },
            { "DeviceAppManagement_ManagedEBooks_UserStateSummary_DeviceStates", "AmManagedEBooksUserStateSummaryDeviceStates" },
            { "DeviceAppManagement_ManagedEBooks_Assign", "AssignManagedEBooks" },
            #endregion
            #region DeviceAppManagement_MdmWindowsInformationProtectionPolicies
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies", "AmMdmWinInfoPP" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_Assignments", "AmMdmWinInfoPPAssignments" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_ExemptAppLockerFiles", "AmMdmWinInfoPPExemptAppLockerFiles" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_ProtectedAppLockerFiles", "AmMdmWinInfoPPProtectedAppLockerFiles" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_Assign", "AmAssignMdmWinInfoPP" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_TargetApps", "AmTargetMdmWinInfoPP" },
            #endregion
            #region DeviceAppManagement_MobileAppCategories
            { "DeviceAppManagement_MobileAppCategories", "AmMobileAppCats" },
            #endregion
            #region DeviceAppManagement_MobileAppConfigurations
            { "DeviceAppManagement_MobileAppConfigurations", "AmMobileAppConfigs" },
            { "DeviceAppManagement_MobileAppConfigurations_Assignments", "AmMobileAppConfigAssignments" },
            { "DeviceAppManagement_MobileAppConfigurations_DeviceStatuses", "AmMobileAppConfigDeviceStatuses" },
            { "DeviceAppManagement_MobileAppConfigurations_DeviceStatusSummary", "AmMobileAppConfigDeviceStatusSummary" },
            { "DeviceAppManagement_MobileAppConfigurations_UserStatuses", "AmMobileAppConfigUserStatuses" },
            { "DeviceAppManagement_MobileAppConfigurations_UserStatusSummary", "AmMobileAppConfigUserStatusSummary" },
            #endregion            
            #region DeviceAppManagement_MobileApps
            { "DeviceAppManagement_MobileApps", "AmMobileApps" },            
            { "DeviceAppManagement_MobileApps_Assignments", "AmMobileAppsAssignments" },
            { "DeviceAppManagement_MobileApps_Categories", "AmMobileAppsCategories" },
            { "DeviceAppManagement_MobileApps_CategoriesReferences", "AmMobileAppsCategoriesReferences" },
            { "DeviceAppManagement_MobileApps_ContentVersions", "AmMobileAppsContentVersions" },
            { "DeviceAppManagement_MobileApps_ContentVersions_Files", "AmMobileAppsContentVersionsFiles"},
            { "DeviceAppManagement_MobileApps_Assign", "AmAssignMobileApps" },
            { "DeviceAppManagement_MobileApps_ContentVersions_Files_Commit", "AmCommitMobileAppCurrentFileVersion" },
            { "DeviceAppManagement_MobileApps_ContentVersions_Files_RenewUpload", "AmRenewMobileAppCurrentFileVersionUpload" },
            #endregion
            #region DeviceAppManagement_SyncMicrosoftStoreForBusinessApps
            { "DeviceAppManagement_SyncMicrosoftStoreForBusinessApps", "AmSyncMSFBApps" },
            #endregion
            #region DeviceAppManagement_TargetedManagedAppConfigurations
            { "DeviceAppManagement_TargetedManagedAppConfigurations", "AmTargetedAppConfigs" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_Apps", "AmTargetedAppConfigApps" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_Assignments", "AmTargetedAppConfigAssignments" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_DeploymentSummary", "AmTargetedAppConfigDeploymentSummary" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_Assign", "AmAssignTargetedAppConfig" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_TargetApps", "AmTargetedAppConfigApps" },
            #endregion
            #region DeviceAppManagement_VppTokens
            { "DeviceAppManagement_VppTokens", "AmVppTokens"},
            { "DeviceAppManagement_VppTokens_SyncLicenses", "VppTokensSyncLicenses" },
            #endregion
            #region DeviceAppManagement_WindowsInformationProtectionPolicies
            { "DeviceAppManagement_WindowsInformationProtectionPolicies", "AmWinInfoPP" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_Assignments", "AmWinInfoPPAssignments" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_ExemptAppLockerFiles", "AmWinInfoPPExemptAppLockerFiles" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_ProtectedAppLockerFiles", "AmWinInfoPPProtectedAppLockerFiles" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_Assign", "AmAssignWinInfoPP" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_TargetApps", "AmTargetWinInfoPP"},
            #endregion
            #endregion
            #region DeviceManagement            
            { "DeviceManagement_ApplePushNotificationCertificate", "DmAPNSCert" },
            { "DeviceManagement_ApplePushNotificationCertificate_DownloadApplePushNotificationCertificateSigningRequest", "DmDownloadAPNSCertSigningRequest" },
            { "DeviceManagement_ConditionalAccessSettings", "DmCASettings" },
            { "DeviceManagement_DeviceCategories", "DmDeviceCategories" },
            { "DeviceManagement_DeviceCompliancePolicyDeviceStateSummary", "DmDeviceCompliancePolicyDeviceStateSummary" },
            { "DeviceManagement_DeviceCompliancePolicySettingStateSummaries", "DmDeviceCompliancePolicySettingStateSummaries" },
            { "DeviceManagement_DeviceCompliancePolicySettingStateSummaries_DeviceComplianceSettingStates", "DmDeviceComplianceSettingStates" },
            { "DeviceManagement_DeviceCompliancePolicies_Assign", "DmAssignDeviceCompliancePolicies" },
            { "DeviceManagement_DeviceCompliancePolicies_ScheduleActionsForRules", "DmScheduleActionsForRulesOfDeviceCompliancePolicies" },
            { "DeviceManagement_DeviceConfigurationDeviceStateSummaries", "DmDeviceConfigurationDeviceStateSummaries" },
            { "DeviceManagement_DeviceManagementPartners", "DmDeviceManagementPartners" },
            { "DeviceManagement_ExchangeConnectors", "DmExchangeConnectors" },
            { "DeviceManagement_IosUpdateStatuses", "DmIosUpdateStatuses" },
            { "DeviceManagement_ManagedDeviceOverview", "DmManagedDeviceOverview" },
            { "DeviceManagement_ManagedDeviceOverviewReference", "DmManagedDeviceOverviewReference" }, //BUGBUG: Missing Route
            { "DeviceManagement_MobileThreatDefenseConnectors", "DmMobileThreatDefenseConnectors" },
            { "DeviceManagement_TroubleshootingEvents", "DmTroubleshootingEvents"},
            { "DeviceManagement_WindowsInformationProtectionAppLearningSummaries", "DmWIPAppLearningSummaries" },
            { "DeviceManagement_WindowsInformationProtectionNetworkLearningSummaries", "DmWIPNetworkLearningSummaries" },
            { "DeviceManagement_RemoteAssistancePartners", "DmRemoteAssistancePartners" },
            { "DeviceManagement_ResourceOperations", "DmResourceOperations" },
            { "DeviceManagement_SoftwareUpdateStatusSummary", "DmSoftwareUpdateStatusSummary" },
            { "DeviceManagement_SoftwareUpdateStatusSummaryReference", "DmSoftwareUpdateStatusSummaryReference" }, //BUGBUG: Missing Route
            { "DeviceManagement_TelecomExpenseManagementPartners", "DmTelecomExpenseManagementPartners" },
            { "DeviceManagement_DeviceConfigurations_Assign", "DmAssignDCs" },
            { "DeviceManagement_DeviceEnrollmentConfigurations_Assign", "DmAssignDeviceEnrollmentConfigs" },
            { "DeviceManagement_DeviceEnrollmentConfigurations_SetPriority", "DmSetPriorityOfDeviceEnrollmentConfigs" },
            { "DeviceManagement_ExchangeConnectors_Sync", "DmSyncExchangeConnectors" },
            { "DeviceManagement_GetEffectivePermissions", "DmGetEffectivePermissions" },
            { "DeviceManagement_VerifyWindowsEnrollmentAutoDiscovery", "DmVerifyWindowsEnrollmentAutoDiscovery" },
            #region DeviceManagement_DetectedApps
            { "DeviceManagement_DetectedApps", "DmDetectedApps" },
            { "DeviceManagement_DetectedApps_ManagedDevices", "DmDetectedAppDevices" },
            { "DeviceManagement_DetectedApps_ManagedDevicesReferences", "DmDetectedAppDeviceRefs" },
            #endregion
            #region DeviceManagement_DeviceCompliancePolicies
            { "DeviceManagement_DeviceCompliancePolicies", "DmDeviceCompliancePolicies" },
            { "DeviceManagement_DeviceCompliancePolicies_Assignments", "DmDeviceCompliancePolicyAssignments" },
            { "DeviceManagement_DeviceCompliancePolicies_DeviceSettingStateSummaries", "DmDeviceCompliancePolicyDeviceSettingStateSummaries" },
            { "DeviceManagement_DeviceCompliancePolicies_DeviceStatuses", "DmDeviceCompliancePolicyDeviceStatuses" },
            { "DeviceManagement_DeviceCompliancePolicies_DeviceStatusOverview", "DmDeviceCompliancePolicyDeviceStatusOverview" },
            { "DeviceManagement_DeviceCompliancePolicies_ScheduledActionsForRule", "DmDeviceCompliancePolicyScheduledActionsForRule" },
            { "DeviceManagement_DeviceCompliancePolicies_ScheduledActionsForRule_ScheduledActionConfigurations", "DmDeviceCompliancePolicyScheduledActionsForRuleConfigs" },
            { "DeviceManagement_DeviceCompliancePolicies_UserStatuses", "DmDeviceCompliancePolicyUserStatuses" },
            { "DeviceManagement_DeviceCompliancePolicies_UserStatusOverview", "DmDeviceCompliancePolicyUserStatusOverview" },
            #endregion
            #region DeviceManagement_DeviceConfigurations
            { "DeviceManagement_DeviceConfigurations", "DmDeviceConfigurations" },
            { "DeviceManagement_DeviceConfigurations_Assignments", "DmDCAssignments" },
            { "DeviceManagement_DeviceConfigurations_DeviceStatusOverview", "DmDCDeviceStatusOverview" },
            { "DeviceManagement_DeviceConfigurations_UserStatuses", "DmDCUserStatuses" },
            { "DeviceManagement_DeviceConfigurations_UserStatusOverview", "DmDCUserStatusOverview" },
            { "DeviceManagement_DeviceConfigurations_DeviceSettingStateSummaries", "DmDCDeviceSettingStateSummaries" },
            { "DeviceManagement_DeviceConfigurations_DeviceStatuses", "DmDCDeviceStatuses" },
            #endregion
            #region DeviceManagement_DeviceEnrollmentConfigurations
            { "DeviceManagement_DeviceEnrollmentConfigurations", "DmDeviceEnrollmentConfigs" },
            { "DeviceManagement_DeviceEnrollmentConfigurations_Assignments", "DmDeviceEnrollmentConfigAssignments" },
            #endregion
            #region DeviceManagement_ManagedDevices
            { "DeviceManagement_ManagedDevices", "DmManagedDevices" },
            { "DeviceManagement_ManagedDevices_DeviceCategory", "DmDeviceCategory" },
            { "DeviceManagement_ManagedDevices_DeviceCompliancePolicyStates", "DmDeviceCompliancePolicyStates" },
            { "DeviceManagement_ManagedDevices_DeviceConfigurationStates", "DmDeviceConfigurationStates" },
            { "DeviceManagement_ManagedDevices_BypassActivationLock", "DmBypassActivationLock" },
            { "DeviceManagement_ManagedDevices_CleanWindowsDevice", "DmCleanWindowsDevice" },
            { "DeviceManagement_ManagedDevices_DeleteUserFromSharedAppleDevice", "DmDeleteUserFromSharedAppleDevice" },
            { "DeviceManagement_ManagedDevices_DisableLostMode", "DmDisableLostMode" },
            { "DeviceManagement_ManagedDevices_LocateDevice", "DmLocateDevice" },
            { "DeviceManagement_ManagedDevices_LogoutSharedAppleDeviceActiveUser", "DmLogoutSharedAppleDeviceActiveUser" },
            { "DeviceManagement_ManagedDevices_RebootNow", "DmRebootDeviceNow" },
            { "DeviceManagement_ManagedDevices_RecoverPasscode", "DmRecoverDevicePasscode" },
            { "DeviceManagement_ManagedDevices_RemoteLock", "DmRemoteLockDevice" },
            { "DeviceManagement_ManagedDevices_RequestRemoteAssistance", "DmRequestRA" },
            { "DeviceManagement_ManagedDevices_Retire", "DmRetireDevice" },
            { "DeviceManagement_ManagedDevices_ShutDown", "DmShutDownDevice"},
            { "DeviceManagement_ManagedDevices_SyncDevice", "DmSyncDevice" },
            { "DeviceManagement_ManagedDevices_UpdateWindowsDeviceAccount", "DmUpdateWindowsDeviceAccount"},
            { "DeviceManagement_ManagedDevices_WindowsDefenderScan", "DmWindowsDefenderScan"},
            { "DeviceManagement_ManagedDevices_WindowsDefenderUpdateSignatures", "DmWindowsDefenderUpdateSignatures" },
            { "DeviceManagement_ManagedDevices_Wipe", "DmWipeDevice" },

            #endregion
            #region DeviceManagement_NotificationMessageTemplates
            { "DeviceManagement_NotificationMessageTemplates", "DmNotifMsgTemplates"},
            { "DeviceManagement_NotificationMessageTemplates_LocalizedNotificationMessages", "DmNotifMsgTemplateLocMsgs" },
            { "DeviceManagement_NotificationMessageTemplates_SendTestMessage", "DmSendTestMessage" },
            #endregion
            #region DeviceManagement_RoleAssignments
            { "DeviceManagement_RoleAssignments", "DmRoleAssignments" },
            { "DeviceManagement_RoleAssignments_RoleDefinition", "DmRoleAssignmentDefinition" }, //BUGBUG: Missing Route
            #endregion
            #region DeviceManagement_RoleDefinitions
            { "DeviceManagement_RoleDefinitions", "DmRoleDefinitions" },
            { "DeviceManagement_RoleDefinitions_RoleAssignments", "DmRoleDefinitionAssignments" },  //BUGBUG: Missing Route
            { "DeviceManagement_RoleDefinitions_RoleAssignments_RoleDefinition", "DmRoleAssignmentRoleDefinition" }, //BUGBUG: Missing route
            { "DeviceManagement_RoleDefinitions_RoleAssignments_RoleDefinitionReference", "DmRoleAssignmentDefinitionRef" }, //BUGBUG: Missing route
            #endregion
            #region DeviceManagement_RemoteAssistancePartners_BeginOnboarding
            { "DeviceManagement_RemoteAssistancePartners_BeginOnboarding", "DmBeginRAOnboarding" },
            { "DeviceManagement_RemoteAssistancePartners_Disconnect", "DmDisconnectRA" },
            #endregion
            #region DeviceManagement_TermsAndConditions
            { "DeviceManagement_TermsAndConditions", "DmTnCs" },
            { "DeviceManagement_TermsAndConditions_AcceptanceStatuses", "DmTnCAcceptanceStatuses" },
            { "DeviceManagement_TermsAndConditions_Assignments", "DmTnCAssignments" }
            #endregion
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
