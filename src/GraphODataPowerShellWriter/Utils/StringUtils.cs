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
            { "DeviceAppManagement_AndroidManagedAppProtections", "AndroidAPP" },
            { "DeviceAppManagement_AndroidManagedAppProtections_Apps", "AndroidAPPApps" },
            { "DeviceAppManagement_AndroidManagedAppProtections_Assignments", "AndroidAPPAssignments" },
            { "DeviceAppManagement_AndroidManagedAppProtections_DeploymentSummary", "AndroidAPPDeploymentSummary" },
            { "DeviceAppManagement_AndroidManagedAppProtections_Assign", "AssignAndroidAPP" },
            { "DeviceAppManagement_AndroidManagedAppProtections_TargetApps", "TargetAndroidAPP" },
            #endregion
            #region DeviceAppManagement_DefaultManagedAppProtections
            { "DeviceAppManagement_DefaultManagedAppProtections", "DefaultAPP" },
            { "DeviceAppManagement_DefaultManagedAppProtections_Apps", "DefaultAPPApps" },
            { "DeviceAppManagement_DefaultManagedAppProtections_DeploymentSummary", "DefaultAPPDeploymentSummary" },
            { "DeviceAppManagement_DefaultManagedAppProtections_TargetApps", "TargetDefaultAPP" },
            #endregion
            #region DeviceAppManagement_IosManagedAppProtections
            { "DeviceAppManagement_IosManagedAppProtections", "IosAPPs" },
            { "DeviceAppManagement_IosManagedAppProtections_Apps", "IosAPPsApps" },
            { "DeviceAppManagement_IosManagedAppProtections_Assignments", "IosAPPsAppsAssignments" },
            { "DeviceAppManagement_IosManagedAppProtections_DeploymentSummary", "IosAPPsAppsDeploymentSummary" },
            { "DeviceAppManagement_IosManagedAppProtections_Assign", "AssignIosAPPsApps" },
            { "DeviceAppManagement_IosManagedAppProtections_TargetApps", "TargetIosAPPs" },
            #endregion      
            #region DeviceAppManagement_ManagedAppPolicies
            { "DeviceAppManagement_ManagedAppPolicies", "ManagedAppPolicies" },
            { "DeviceAppManagement_ManagedAppPolicies_Apps", "ManagedAppPoliciesApps" },
            { "DeviceAppManagement_ManagedAppPolicies_Assignments", "ManagedAppPoliciesAssignments" },
            { "DeviceAppManagement_ManagedAppPolicies_DeploymentSummary", "ManagedAppPoliciesDeploymentSummary" },
            { "DeviceAppManagement_ManagedAppPolicies_ExemptAppLockerFiles", "ManagedAppPoliciesExemptAppLockerFiles" },
            { "DeviceAppManagement_ManagedAppPolicies_ProtectedAppLockerFiles", "ManagedAppPoliciesProtectedAppLockerFiles" },
            { "DeviceAppManagement_ManagedAppPolicies_TargetApps", "TargetManagedAppPolicies" },
            #endregion
            #region DeviceAppManagement_ManagedAppRegistrations
            { "DeviceAppManagement_ManagedAppRegistrations", "AppRegns" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies", "AppRegnPolicies" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_Apps", "AppRegnApps" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_Assignments", "AppRegnAssignments" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_DeploymentSummary", "AppRegnDeploymentSummary" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_ExemptAppLockerFiles", "AppRegnExemptAppLockerFiles" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_ProtectedAppLockerFiles", "AppRegnProtectedAppLockerFiles" },
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies", "AppRegnIntendedPolicies" },
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_Apps", "AppRegnIntendedPoliciesApps" },
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_Assignments", "AppRegnIntendedPoliciesAsignments" },
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_DeploymentSummary", "AppRegnIntendedPoliciesDeploymentSummary"},
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_ExemptAppLockerFiles", "AppRegnIntendedPoliciesExemptAppLockerFiles" },
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_ProtectedAppLockerFiles", "AppRegnIntendedPoliciesProtectedAppLockerFiles" },            
            { "DeviceAppManagement_ManagedAppRegistrations_Operations", "AmAppRegnOps" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_TargetApps", "TargetAppRegnAppliedPolicies" },
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_TargetApps", "TargetAppRegnIntendedPolicies" },
            { "DeviceAppManagement_ManagedAppRegistrations_GetUserIdsWithFlaggedAppRegistration", "GetUserIdsWithFlaggedAppRegistration" },
            #endregion
            #region DeviceAppManagement_ManagedAppStatuses
            { "DeviceAppManagement_ManagedAppStatuses", "AppStatuses" },
            #endregion
            #region DeviceAppManagement_ManagedEBooks
            { "DeviceAppManagement_ManagedEBooks", "ManagedEBooks" },
            { "DeviceAppManagement_ManagedEBooks_Assignments", "ManagedEBookAssignments"},
            { "DeviceAppManagement_ManagedEBooks_DeviceStates", "ManagedEBooksDeviceStates" },
            { "DeviceAppManagement_ManagedEBooks_InstallSummary", "ManagedEBooksInstallSummary" },
            { "DeviceAppManagement_ManagedEBooks_UserStateSummary", "ManagedEBooksUserStateSummary" },
            { "DeviceAppManagement_ManagedEBooks_UserStateSummary_DeviceStates", "ManagedEBooksUserStateSummaryDeviceStates" },
            { "DeviceAppManagement_ManagedEBooks_Assign", "AssignManagedEBooks" },
            #endregion
            #region DeviceAppManagement_MdmWindowsInformationProtectionPolicies
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies", "MdmWinInfoPP" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_Assignments", "MdmWinInfoPPAssignments" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_ExemptAppLockerFiles", "MdmWinInfoPPExemptAppLockerFiles" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_ProtectedAppLockerFiles", "MdmWinInfoPPProtectedAppLockerFiles" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_Assign", "AssignMdmWinInfoPP" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_TargetApps", "TargetMdmWinInfoPP" },
            #endregion
            #region DeviceAppManagement_MobileAppCategories
            { "DeviceAppManagement_MobileAppCategories", "MobileAppCats" },
            #endregion
            #region DeviceAppManagement_MobileAppConfigurations
            { "DeviceAppManagement_MobileAppConfigurations", "MobileAppConfigs" },
            { "DeviceAppManagement_MobileAppConfigurations_Assignments", "MobileAppConfigAssignments" },
            { "DeviceAppManagement_MobileAppConfigurations_DeviceStatuses", "MobileAppConfigDeviceStatuses" },
            { "DeviceAppManagement_MobileAppConfigurations_DeviceStatusSummary", "MobileAppConfigDeviceStatusSummary" },
            { "DeviceAppManagement_MobileAppConfigurations_UserStatuses", "MobileAppConfigUserStatuses" },
            { "DeviceAppManagement_MobileAppConfigurations_UserStatusSummary", "MobileAppConfigUserStatusSummary" },
            #endregion            
            #region DeviceAppManagement_MobileApps
            { "DeviceAppManagement_MobileApps", "MobileApps" },            
            { "DeviceAppManagement_MobileApps_Assignments", "MobileAppsAssignments" },
            { "DeviceAppManagement_MobileApps_Categories", "MobileAppsCategories" },
            { "DeviceAppManagement_MobileApps_CategoriesReferences", "MobileAppsCategoriesReferences" },
            { "DeviceAppManagement_MobileApps_ContentVersions", "MobileAppsContentVersions" },
            { "DeviceAppManagement_MobileApps_ContentVersions_Files", "MobileAppsContentVersionsFiles"},
            { "DeviceAppManagement_MobileApps_Assign", "AssignMobileApps" },
            { "DeviceAppManagement_MobileApps_ContentVersions_Files_Commit", "CommitMobileAppCurrentFileVersion" },
            { "DeviceAppManagement_MobileApps_ContentVersions_Files_RenewUpload", "RenewMobileAppCurrentFileVersionUpload" },
            #endregion
            #region DeviceAppManagement_SyncMicrosoftStoreForBusinessApps
            { "DeviceAppManagement_SyncMicrosoftStoreForBusinessApps", "SyncMSFBApps" },
            #endregion
            #region DeviceAppManagement_TargetedManagedAppConfigurations
            { "DeviceAppManagement_TargetedManagedAppConfigurations", "TargetedAppConfigs" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_Apps", "TargetedAppConfigApps" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_Assignments", "TargetedAppConfigAssignments" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_DeploymentSummary", "TargetedAppConfigDeploymentSummary" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_Assign", "AssignTargetedAppConfig" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_TargetApps", "TargetedAppConfigApps" },
            #endregion
            #region DeviceAppManagement_VppTokens
            { "DeviceAppManagement_VppTokens", "VppTokens"},
            { "DeviceAppManagement_VppTokens_SyncLicenses", "VppTokensSyncLicenses" },
            #endregion
            #region DeviceAppManagement_WindowsInformationProtectionPolicies
            { "DeviceAppManagement_WindowsInformationProtectionPolicies", "WinInfoPP" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_Assignments", "WinInfoPPAssignments" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_ExemptAppLockerFiles", "WinInfoPPExemptAppLockerFiles" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_ProtectedAppLockerFiles", "WinInfoPPProtectedAppLockerFiles" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_Assign", "AssignWinInfoPP" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_TargetApps", "TargetWinInfoPP"},
            #endregion
            #endregion
            #region DeviceManagement            
            { "DeviceManagement_ApplePushNotificationCertificate", "APNSCert" },
            { "DeviceManagement_ApplePushNotificationCertificate_DownloadApplePushNotificationCertificateSigningRequest", "DownloadAPNSCertSigningRequest" },
            { "DeviceManagement_ConditionalAccessSettings", "CASettings" },
            { "DeviceManagement_DeviceCategories", "DeviceCategories" },
            { "DeviceManagement_DeviceCompliancePolicyDeviceStateSummary", "DeviceCompliancePolicyDeviceStateSummary" },
            { "DeviceManagement_DeviceCompliancePolicySettingStateSummaries", "DeviceCompliancePolicySettingStateSummaries" },
            { "DeviceManagement_DeviceCompliancePolicySettingStateSummaries_DeviceComplianceSettingStates", "DeviceComplianceSettingStates" },
            { "DeviceManagement_DeviceCompliancePolicies_Assign", "AssignDeviceCompliancePolicies" },
            { "DeviceManagement_DeviceCompliancePolicies_ScheduleActionsForRules", "ScheduleActionsForRulesOfDeviceCompliancePolicies" },
            { "DeviceManagement_DeviceConfigurationDeviceStateSummaries", "DeviceConfigurationDeviceStateSummaries" },
            { "DeviceManagement_DeviceManagementPartners", "DeviceManagementPartners" },
            { "DeviceManagement_ExchangeConnectors", "ExchangeConnectors" },
            { "DeviceManagement_IosUpdateStatuses", "IosUpdateStatuses" },
            { "DeviceManagement_ManagedDeviceOverview", "ManagedDeviceOverview" },
            { "DeviceManagement_ManagedDeviceOverviewReference", "ManagedDeviceOverviewReference" }, //BUGBUG: Missing Route
            { "DeviceManagement_MobileThreatDefenseConnectors", "MobileThreatDefenseConnectors" },
            { "DeviceManagement_TroubleshootingEvents", "TroubleshootingEvents"},
            { "DeviceManagement_WindowsInformationProtectionAppLearningSummaries", "WIPAppLearningSummaries" },
            { "DeviceManagement_WindowsInformationProtectionNetworkLearningSummaries", "WIPNetworkLearningSummaries" },
            { "DeviceManagement_RemoteAssistancePartners", "RemoteAssistancePartners" },
            { "DeviceManagement_ResourceOperations", "ResourceOperations" },
            { "DeviceManagement_SoftwareUpdateStatusSummary", "SoftwareUpdateStatusSummary" },
            { "DeviceManagement_SoftwareUpdateStatusSummaryReference", "SoftwareUpdateStatusSummaryReference" }, //BUGBUG: Missing Route
            { "DeviceManagement_TelecomExpenseManagementPartners", "TelecomExpenseManagementPartners" },
            { "DeviceManagement_DeviceConfigurations_Assign", "AssignDCs" },
            { "DeviceManagement_DeviceEnrollmentConfigurations_Assign", "AssignDeviceEnrollmentConfigs" },
            { "DeviceManagement_DeviceEnrollmentConfigurations_SetPriority", "SetPriorityOfDeviceEnrollmentConfigs" },
            { "DeviceManagement_ExchangeConnectors_Sync", "SyncExchangeConnectors" },
            { "DeviceManagement_GetEffectivePermissions", "GetEffectivePermissions" },
            { "DeviceManagement_VerifyWindowsEnrollmentAutoDiscovery", "VerifyWindowsEnrollmentAutoDiscovery" },
            #region DeviceManagement_DetectedApps
            { "DeviceManagement_DetectedApps", "DetectedApps" },
            { "DeviceManagement_DetectedApps_ManagedDevices", "DetectedAppDevices" },
            { "DeviceManagement_DetectedApps_ManagedDevicesReferences", "DetectedAppDeviceRefs" },
            #endregion
            #region DeviceManagement_DeviceCompliancePolicies
            { "DeviceManagement_DeviceCompliancePolicies", "DeviceCompliancePolicies" },
            { "DeviceManagement_DeviceCompliancePolicies_Assignments", "DeviceCompliancePolicyAssignments" },
            { "DeviceManagement_DeviceCompliancePolicies_DeviceSettingStateSummaries", "DeviceCompliancePolicyDeviceSettingStateSummaries" },
            { "DeviceManagement_DeviceCompliancePolicies_DeviceStatuses", "DeviceCompliancePolicyDeviceStatuses" },
            { "DeviceManagement_DeviceCompliancePolicies_DeviceStatusOverview", "DeviceCompliancePolicyDeviceStatusOverview" },
            { "DeviceManagement_DeviceCompliancePolicies_ScheduledActionsForRule", "DeviceCompliancePolicyScheduledActionsForRule" },
            { "DeviceManagement_DeviceCompliancePolicies_ScheduledActionsForRule_ScheduledActionConfigurations", "DeviceCompliancePolicyScheduledActionsForRuleConfigs" },
            { "DeviceManagement_DeviceCompliancePolicies_UserStatuses", "DeviceCompliancePolicyUserStatuses" },
            { "DeviceManagement_DeviceCompliancePolicies_UserStatusOverview", "DeviceCompliancePolicyUserStatusOverview" },
            #endregion
            #region DeviceManagement_DeviceConfigurations
            { "DeviceManagement_DeviceConfigurations", "DeviceConfigurations" },
            { "DeviceManagement_DeviceConfigurations_Assignments", "DCAssignments" },
            { "DeviceManagement_DeviceConfigurations_DeviceStatusOverview", "DCDeviceStatusOverview" },
            { "DeviceManagement_DeviceConfigurations_UserStatuses", "DCUserStatuses" },
            { "DeviceManagement_DeviceConfigurations_UserStatusOverview", "DCUserStatusOverview" },
            { "DeviceManagement_DeviceConfigurations_DeviceSettingStateSummaries", "DCDeviceSettingStateSummaries" },
            { "DeviceManagement_DeviceConfigurations_DeviceStatuses", "DCDeviceStatuses" },
            #endregion
            #region DeviceManagement_DeviceEnrollmentConfigurations
            { "DeviceManagement_DeviceEnrollmentConfigurations", "DeviceEnrollmentConfigs" },
            { "DeviceManagement_DeviceEnrollmentConfigurations_Assignments", "DeviceEnrollmentConfigAssignments" },
            #endregion
            #region DeviceManagement_ManagedDevices
            { "DeviceManagement_ManagedDevices", "ManagedDevices" },
            { "DeviceManagement_ManagedDevices_DeviceCategory", "DeviceCategory" },
            { "DeviceManagement_ManagedDevices_DeviceCompliancePolicyStates", "DeviceCompliancePolicyStates" },
            { "DeviceManagement_ManagedDevices_DeviceConfigurationStates", "DeviceConfigurationStates" },
            { "DeviceManagement_ManagedDevices_BypassActivationLock", "BypassActivationLock" },
            { "DeviceManagement_ManagedDevices_CleanWindowsDevice", "CleanWindowsDevice" },
            { "DeviceManagement_ManagedDevices_DeleteUserFromSharedAppleDevice", "DeleteUserFromSharedAppleDevice" },
            { "DeviceManagement_ManagedDevices_DisableLostMode", "DisableLostMode" },
            { "DeviceManagement_ManagedDevices_LocateDevice", "LocateDevice" },
            { "DeviceManagement_ManagedDevices_LogoutSharedAppleDeviceActiveUser", "LogoutSharedAppleDeviceActiveUser" },
            { "DeviceManagement_ManagedDevices_RebootNow", "RebootDeviceNow" },
            { "DeviceManagement_ManagedDevices_RecoverPasscode", "RecoverDevicePasscode" },
            { "DeviceManagement_ManagedDevices_RemoteLock", "RemoteLockDevice" },
            { "DeviceManagement_ManagedDevices_RequestRemoteAssistance", "RequestRA" },
            { "DeviceManagement_ManagedDevices_Retire", "RetireDevice" },
            { "DeviceManagement_ManagedDevices_ShutDown", "ShutDownDevice"},
            { "DeviceManagement_ManagedDevices_SyncDevice", "SyncDevice" },
            { "DeviceManagement_ManagedDevices_UpdateWindowsDeviceAccount", "UpdateWindowsDeviceAccount"},
            { "DeviceManagement_ManagedDevices_WindowsDefenderScan", "WindowsDefenderScan"},
            { "DeviceManagement_ManagedDevices_WindowsDefenderUpdateSignatures", "WindowsDefenderUpdateSignatures" },
            { "DeviceManagement_ManagedDevices_Wipe", "WipeDevice" },

            #endregion
            #region DeviceManagement_NotificationMessageTemplates
            { "DeviceManagement_NotificationMessageTemplates", "NotifMsgTemplates"},
            { "DeviceManagement_NotificationMessageTemplates_LocalizedNotificationMessages", "NotifMsgTemplateLocMsgs" },
            { "DeviceManagement_NotificationMessageTemplates_SendTestMessage", "SendTestMessage" },
            #endregion
            #region DeviceManagement_RoleAssignments
            { "DeviceManagement_RoleAssignments", "RoleAssignments" },
            { "DeviceManagement_RoleAssignments_RoleDefinition", "RoleAssignmentDefinition" }, //BUGBUG: Missing Route
            #endregion
            #region DeviceManagement_RoleDefinitions
            { "DeviceManagement_RoleDefinitions", "RoleDefinitions" },
            { "DeviceManagement_RoleDefinitions_RoleAssignments", "RoleDefinitionAssignments" },  //BUGBUG: Missing Route
            { "DeviceManagement_RoleDefinitions_RoleAssignments_RoleDefinition", "RoleAssignmentRoleDefinition" }, //BUGBUG: Missing route
            { "DeviceManagement_RoleDefinitions_RoleAssignments_RoleDefinitionReference", "RoleAssignmentDefinitionRef" }, //BUGBUG: Missing route
            #endregion
            #region DeviceManagement_RemoteAssistancePartners_BeginOnboarding
            { "DeviceManagement_RemoteAssistancePartners_BeginOnboarding", "BeginRAOnboarding" },
            { "DeviceManagement_RemoteAssistancePartners_Disconnect", "DisconnectRA" },
            #endregion
            #region DeviceManagement_TermsAndConditions
            { "DeviceManagement_TermsAndConditions", "TnCs" },
            { "DeviceManagement_TermsAndConditions_AcceptanceStatuses", "TnCAcceptanceStatuses" },
            { "DeviceManagement_TermsAndConditions_Assignments", "TnCAssignments" }
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
