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
            { "DeviceAppManagement_AndroidManagedAppProtections", "IntuneAppProtectionPolicyAndroid" },
            { "DeviceAppManagement_AndroidManagedAppProtections_Apps", "IntuneAppProtectionPolicyAndroidApp" },
            { "DeviceAppManagement_AndroidManagedAppProtections_Assignments", "IntuneAppProtectionPolicyAndroidAssignment" },
            { "DeviceAppManagement_AndroidManagedAppProtections_DeploymentSummary", "IntuneAppProtectionPolicyAndroidDeploymentSummary" },
            { "DeviceAppManagement_AndroidManagedAppProtections_Assign", "IntuneAppProtectionPolicyAndroidAssign" },
            { "DeviceAppManagement_AndroidManagedAppProtections_TargetApps", "IntuneAppProtectionPolicyAndroidTargetApp" },
            #endregion
            #region DeviceAppManagement_DefaultManagedAppProtections
            { "DeviceAppManagement_DefaultManagedAppProtections", "IntuneAppProtectionPolicyDefault" },
            { "DeviceAppManagement_DefaultManagedAppProtections_Apps", "IntuneAppProtectionPolicyDefaultApp" },
            { "DeviceAppManagement_DefaultManagedAppProtections_DeploymentSummary", "IntuneAppProtectionPolicyDefaultDeploymentSummary" },
            { "DeviceAppManagement_DefaultManagedAppProtections_TargetApps", "IntuneAppProtectionPolicyDefaultTargetApp" },
            #endregion
            #region DeviceAppManagement_IosManagedAppProtections
            { "DeviceAppManagement_IosManagedAppProtections", "IntuneAppProtectionPolicyIos" },
            { "DeviceAppManagement_IosManagedAppProtections_Apps", "IntuneAppProtectionPolicyIosApp" },
            { "DeviceAppManagement_IosManagedAppProtections_Assignments", "IntuneAppProtectionPolicyIosAssignment" },
            { "DeviceAppManagement_IosManagedAppProtections_DeploymentSummary", "IntuneAppProtectionPolicyIosDeploymentSummary" },
            { "DeviceAppManagement_IosManagedAppProtections_Assign", "IntuneAppProtectionPolicyIosAssign" },
            { "DeviceAppManagement_IosManagedAppProtections_TargetApps", "IntuneAppProtectionPolicyIosTargetApp" },
            #endregion      
            #region DeviceAppManagement_ManagedAppPolicies
            { "DeviceAppManagement_ManagedAppPolicies", "IntuneAppProtectionPolicy" },
            { "DeviceAppManagement_ManagedAppPolicies_Apps", "IntuneAppProtectionPolicyApp" },
            { "DeviceAppManagement_ManagedAppPolicies_Assignments", "IntuneAppProtectionPolicyAssignment" },
            { "DeviceAppManagement_ManagedAppPolicies_DeploymentSummary", "IntuneAppProtectionPolicyDeploymentSummary" },
            { "DeviceAppManagement_ManagedAppPolicies_ExemptAppLockerFiles", "IntuneAppProtectionPolicyExemptAppLockerFile" },
            { "DeviceAppManagement_ManagedAppPolicies_ProtectedAppLockerFiles", "IntuneAppProtectionPolicyProtectedAppLockerFile" },
            { "DeviceAppManagement_ManagedAppPolicies_TargetApps", "IntuneAppProtectionPolicyTargetApp" },
            #endregion
            #region DeviceAppManagement_ManagedAppRegistrations
            { "DeviceAppManagement_ManagedAppRegistrations", "IntuneManagedAppRegistration" },            
            #endregion
            #region DeviceAppManagement_ManagedAppStatuses
            { "DeviceAppManagement_ManagedAppStatuses", "IntuneManagedAppStatus" },
            #endregion
            #region DeviceAppManagement_ManagedEBooks
            { "DeviceAppManagement_ManagedEBooks", "IntuneManagedEBook" },
            { "DeviceAppManagement_ManagedEBooks_Assignments", "IntuneManagedEBookAssignment"},
            { "DeviceAppManagement_ManagedEBooks_DeviceStates", "IntuneManagedEBookDeviceState" },
            { "DeviceAppManagement_ManagedEBooks_InstallSummary", "IntuneManagedEBookInstallSummary" },
            { "DeviceAppManagement_ManagedEBooks_UserStateSummary", "IntuneManagedEBookUserStateSummary" },
            { "DeviceAppManagement_ManagedEBooks_UserStateSummary_DeviceStates", "IntuneManagedEBookUserStateSummaryDeviceState" },
            { "DeviceAppManagement_ManagedEBooks_Assign", "IntuneManagedEBookAssign" },
            #endregion
            #region DeviceAppManagement_MdmWindowsInformationProtectionPolicies
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies", "IntuneMdmWindowsInformationProtectionPolicy" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_Assignments", "IntuneMdmWindowsInformationProtectionPolicyAssignment" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_ExemptAppLockerFiles", "IntuneMdmWindowsInformationProtectionPolicyExemptAppLockerFile" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_ProtectedAppLockerFiles", "IntuneMdmWindowsInformationProtectionPolicyProtectedAppLockerFile" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_Assign", "IntuneMdmWindowsInformationProtectionPolicyAssign" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_TargetApps", "IntuneMdmWindowsInformationProtectionPolicyTargetApp" },
            #endregion
            #region DeviceAppManagement_MobileAppCategories
            { "DeviceAppManagement_MobileAppCategories", "IntuneMobileAppCategory" },
            #endregion
            #region DeviceAppManagement_MobileAppConfigurations
            { "DeviceAppManagement_MobileAppConfigurations", "IntuneMobileAppConfigurationPolicy" },
            { "DeviceAppManagement_MobileAppConfigurations_Assignments", "IntuneMobileAppConfigurationPolicyAssignment" },
            { "DeviceAppManagement_MobileAppConfigurations_DeviceStatuses", "IntuneMobileAppConfigurationPolicyDeviceStatus" },
            { "DeviceAppManagement_MobileAppConfigurations_DeviceStatusSummary", "IntuneMobileAppConfigurationPolicyDeviceStatusSummary" },
            { "DeviceAppManagement_MobileAppConfigurations_UserStatuses", "IntuneMobileAppConfigurationPolicyUserStatus" },
            { "DeviceAppManagement_MobileAppConfigurations_UserStatusSummary", "IntuneMobileAppConfigurationPolicyUserStatusSummary" },
            #endregion            
            #region DeviceAppManagement_MobileApps
            { "DeviceAppManagement_MobileApps", "IntuneMobileApp" },            
            { "DeviceAppManagement_MobileApps_Assignments", "IntuneMobileAppAssignment" },
            { "DeviceAppManagement_MobileApps_Categories", "IntuneMobileAppCategorySet" },
            { "DeviceAppManagement_MobileApps_CategoriesReferences", "IntuneMobileAppCategoryReference" },
            { "DeviceAppManagement_MobileApps_ContentVersions", "IntuneMobileAppContentVersion" },
            { "DeviceAppManagement_MobileApps_ContentVersions_Files", "IntuneMobileAppContentVersionFile"},
            { "DeviceAppManagement_MobileApps_Assign", "IntuneMobileAppAssign" },
            { "DeviceAppManagement_MobileApps_ContentVersions_Files_Commit", "IntuneMobileAppContentVersionFileCommit" },
            { "DeviceAppManagement_MobileApps_ContentVersions_Files_RenewUpload", "IntuneMobileAppContentVersionRenewUpload" },
            #endregion
            #region DeviceAppManagement_SyncMicrosoftStoreForBusinessApps
            { "DeviceAppManagement_SyncMicrosoftStoreForBusinessApps", "IntuneSyncMicrosoftStoreForBusinessApp" },
            #endregion
            #region DeviceAppManagement_TargetedManagedAppConfigurations
            { "DeviceAppManagement_TargetedManagedAppConfigurations", "IntuneAppConfigurationPolicyTargeted" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_Apps", "IntuneAppConfigurationPolicyTargetedApp" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_Assignments", "IntuneAppConfigurationPolicyTargetedAssignment" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_DeploymentSummary", "IntuneAppConfigurationPolicyTargetedDeploymentSummary" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_Assign", "IntuneAppConfigurationPolicyTargetedAssign" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_TargetApps", "IntuneAppConfigurationPolicyTargetedTargetedApp" },
            #endregion
            #region DeviceAppManagement_VppTokens
            { "DeviceAppManagement_VppTokens", "IntuneVppToken"},
            { "DeviceAppManagement_VppTokens_SyncLicenses", "IntuneVppTokenSyncLicense" },
            #endregion
            #region DeviceAppManagement_WindowsInformationProtectionPolicies
            { "DeviceAppManagement_WindowsInformationProtectionPolicies", "IntuneWindowsInformationProtectionPolicy" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_Assignments", "IntuneWindowsInformationProtectionPolicyAssignment" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_ExemptAppLockerFiles", "IntuneWindowsInformationProtectionPolicyExemptAppLockerFile" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_ProtectedAppLockerFiles", "IntuneWindowsInformationProtectionPolicyProtectedAppLockerFile" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_Assign", "IntuneWindowsInformationProtectionPolicyAssign" },
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_TargetApps", "IntuneWindowsInformationProtectionPolicyTargetApp"},
            #endregion
            #endregion
            #region DeviceManagement  
            { "DeviceManagement", "IntuneDeviceManagement" },
            { "DeviceManagement_ApplePushNotificationCertificate", "IntuneApplePushNotificationCertificate" },
            { "DeviceManagement_ApplePushNotificationCertificate_DownloadApplePushNotificationCertificateSigningRequest", "IntuneDownloadApplePushNotificationCertificateSigningRequest" },
            { "DeviceManagement_ConditionalAccessSettings", "IntuneConditionalAccessSetting" },
            { "DeviceManagement_DeviceCategories", "IntuneDeviceCategory" },
            { "DeviceManagement_DeviceCompliancePolicyDeviceStateSummary", "IntuneDeviceCompliancePolicyDeviceStateSummary" },            
            { "DeviceManagement_DeviceCompliancePolicySettingStateSummaries_DeviceComplianceSettingStates", "IntuneDeviceComplianceSettingState" },
            { "DeviceManagement_DeviceCompliancePolicies_Assign", "IntuneDeviceCompliancePolicyAssign" },
            { "DeviceManagement_DeviceCompliancePolicies_ScheduleActionsForRules", "IntuneDeviceCompliancePolicyScheduleActionsForRules" },
            { "DeviceManagement_DeviceConfigurationDeviceStateSummaries", "IntuneDeviceConfigurationDeviceStateSummary" },
            { "DeviceManagement_DeviceManagementPartners", "IntuneDeviceManagementPartner" },
            { "DeviceManagement_ExchangeConnectors", "IntuneExchangeConnector" },
            { "DeviceManagement_IosUpdateStatuses", "IntuneIosUpdateStatus" },
            { "DeviceManagement_ManagedDeviceOverview", "IntuneManagedDeviceOverview" },
            { "DeviceManagement_ManagedDeviceOverviewReference", "IntuneManagedDeviceOverviewReference" }, //BUGBUG: Missing Route
            { "DeviceManagement_MobileThreatDefenseConnectors", "IntuneMobileThreatDefenseConnector" },
            { "DeviceManagement_TroubleshootingEvents", "IntuneTroubleshootingEvent"},
            { "DeviceManagement_WindowsInformationProtectionAppLearningSummaries", "IntuneWindowsInformationProtectionAppLearningSummary" },
            { "DeviceManagement_WindowsInformationProtectionNetworkLearningSummaries", "IntuneWindowsInformationProtectionNetworkLearningSummary" },
            { "DeviceManagement_RemoteAssistancePartners", "IntuneRemoteAssistancePartner" },
            { "DeviceManagement_ResourceOperations", "IntuneResourceOperation" },
            { "DeviceManagement_SoftwareUpdateStatusSummary", "IntuneSoftwareUpdateStatusSummary" },
            { "DeviceManagement_SoftwareUpdateStatusSummaryReference", "IntuneSoftwareUpdateStatusSummaryReference" }, //BUGBUG: Missing Route
            { "DeviceManagement_TelecomExpenseManagementPartners", "IntuneTelecomExpenseManagementPartner" },            
            { "DeviceManagement_DeviceEnrollmentConfigurations_Assign", "IntuneDeviceEnrollmentConfigurationAssign" },
            { "DeviceManagement_DeviceEnrollmentConfigurations_SetPriority", "IntuneDeviceEnrollmentConfigurationSetPriority" },
            { "DeviceManagement_ExchangeConnectors_Sync", "IntuneExchangeConnectorSync" },
            { "DeviceManagement_GetEffectivePermissions", "IntuneGetEffectivePermission" },
            { "DeviceManagement_VerifyWindowsEnrollmentAutoDiscovery", "IntuneVerifyWindowsEnrollmentAutoDiscovery" },
            #region DeviceManagement_DetectedApps
            { "DeviceManagement_DetectedApps", "IntuneDetectedApp" },
            { "DeviceManagement_DetectedApps_ManagedDevices", "IntuneDetectedAppDevice" },
            { "DeviceManagement_DetectedApps_ManagedDevicesReferences", "IntuneManagedDeviceReference" },
            #endregion
            #region DeviceManagement_DeviceCompliancePolicies
            { "DeviceManagement_DeviceCompliancePolicies", "IntuneDeviceCompliancePolicy" },
            { "DeviceManagement_DeviceCompliancePolicies_Assignments", "IntuneDeviceCompliancePolicyAssignment" },
            { "DeviceManagement_DeviceCompliancePolicies_DeviceSettingStateSummaries", "IntuneDeviceCompliancePolicyDeviceSettingStateSummary" },
            { "DeviceManagement_DeviceCompliancePolicies_DeviceStatuses", "IntuneDeviceCompliancePolicyDeviceStatus" },
            { "DeviceManagement_DeviceCompliancePolicies_DeviceStatusOverview", "IntuneDeviceCompliancePolicyDeviceStatusOverview" },
            { "DeviceManagement_DeviceCompliancePolicies_ScheduledActionsForRule", "IntuneDeviceCompliancePolicyScheduledActionsForRule" },
            { "DeviceManagement_DeviceCompliancePolicies_ScheduledActionsForRule_ScheduledActionConfigurations", "IntuneDeviceCompliancePolicyScheduledActionsForRuleConfiguration" },
            { "DeviceManagement_DeviceCompliancePolicies_UserStatuses", "IntuneDeviceCompliancePolicyUserStatus" },
            { "DeviceManagement_DeviceCompliancePolicies_UserStatusOverview", "IntuneDeviceCompliancePolicyUserStatusOverview" },
            #endregion
            #region DeviceManagement_DeviceConfigurations
            { "DeviceManagement_DeviceConfigurations", "IntuneDeviceConfigurationPolicy" },
            { "DeviceManagement_DeviceConfigurations_Assignments", "IntuneDeviceConfigurationPolicyAssignment" },
            { "DeviceManagement_DeviceConfigurations_DeviceStatusOverview", "IntuneDeviceConfigurationPolicyDeviceStatusOverview" },
            { "DeviceManagement_DeviceConfigurations_UserStatuses", "IntuneDeviceConfigurationPolicyUserStatus" },
            { "DeviceManagement_DeviceConfigurations_UserStatusOverview", "IntuneDeviceConfigurationPolicyUserStatusOverview" },
            { "DeviceManagement_DeviceConfigurations_DeviceSettingStateSummaries", "IntuneDeviceConfigurationPolicyDeviceSettingStateSummary" },
            { "DeviceManagement_DeviceConfigurations_DeviceStatuses", "IntuneDeviceConfigurationPolicyDeviceStatus" },
            { "DeviceManagement_DeviceConfigurations_Assign", "IntuneDeviceConfigurationPolicyAssign" },
            #endregion
            #region DeviceManagement_DeviceEnrollmentConfigurations
            { "DeviceManagement_DeviceEnrollmentConfigurations", "IntuneDeviceEnrollmentConfiguration" },
            { "DeviceManagement_DeviceEnrollmentConfigurations_Assignments", "IntuneDeviceEnrollmentConfigurationAssignment" },
            #endregion
            #region DeviceManagement_ManagedDevices
            { "DeviceManagement_ManagedDevices", "IntuneManagedDevice" },
            { "DeviceManagement_ManagedDevices_DeviceCategory", "IntuneManagedDeviceDeviceCategory" },
            { "DeviceManagement_ManagedDevices_DeviceCompliancePolicyStates", "IntuneManagedDeviceDeviceCompliancePolicyState" },
            { "DeviceManagement_ManagedDevices_DeviceConfigurationStates", "IntuneManagedDeviceDeviceConfigurationState" },
            { "DeviceManagement_ManagedDevices_BypassActivationLock", "IntuneManagedDeviceBypassActivationLock" },
            { "DeviceManagement_ManagedDevices_CleanWindowsDevice", "IntuneManagedDeviceCleanWindowsDevice" },
            { "DeviceManagement_ManagedDevices_DeleteUserFromSharedAppleDevice", "IntuneManagedDeviceDeleteUserFromSharedAppleDevice" },
            { "DeviceManagement_ManagedDevices_DisableLostMode", "IntuneManagedDeviceDisableLostMode" },
            { "DeviceManagement_ManagedDevices_LocateDevice", "IntuneManagedDeviceLocateDevice" },
            { "DeviceManagement_ManagedDevices_LogoutSharedAppleDeviceActiveUser", "IntuneLogoutSharedAppleDeviceActiveUser" },
            { "DeviceManagement_ManagedDevices_RebootNow", "IntuneManagedDeviceRebootNow" },
            { "DeviceManagement_ManagedDevices_RecoverPasscode", "IntuneManagedDeviceRecoverPasscode" },
            { "DeviceManagement_ManagedDevices_RemoteLock", "IntuneManagedDeviceRemoteLock" },
            { "DeviceManagement_ManagedDevices_RequestRemoteAssistance", "IntuneMangedDeviceRequestRemoteAssistance" },
            { "DeviceManagement_ManagedDevices_Retire", "IntuneManagedDeviceRetire" },
            { "DeviceManagement_ManagedDevices_ShutDown", "IntuneManagedDeviceShutDownDevice"},
            { "DeviceManagement_ManagedDevices_SyncDevice", "IntuneManagedDeviceSyncDevice" },
            { "DeviceManagement_ManagedDevices_UpdateWindowsDeviceAccount", "IntuneManagedDeviceUpdateWindowsDeviceAccount"},
            { "DeviceManagement_ManagedDevices_WindowsDefenderScan", "IntuneManagedDeviceWindowsDefenderScan"},
            { "DeviceManagement_ManagedDevices_WindowsDefenderUpdateSignatures", "IntuneManagedDeviceWindowsDefenderUpdateSignature" },
            { "DeviceManagement_ManagedDevices_Wipe", "IntuneManagedDeviceWipeDevice" },

            #endregion
            #region DeviceManagement_NotificationMessageTemplates
            { "DeviceManagement_NotificationMessageTemplates", "IntuneNotificationMessageTemplate"},
            { "DeviceManagement_NotificationMessageTemplates_LocalizedNotificationMessages", "IntuneLocalizedNotificationMessage" },
            { "DeviceManagement_NotificationMessageTemplates_SendTestMessage", "IntuneSendTestMessage" },
            #endregion
            #region DeviceManagement_RoleAssignments
            { "DeviceManagement_RoleAssignments", "IntuneRoleAssignment" },            
            #endregion
            #region DeviceManagement_RoleDefinitions
            { "DeviceManagement_RoleDefinitions", "IntuneRoleDefinition" },
            #endregion
            #region DeviceManagement_RemoteAssistancePartners_BeginOnboarding
            { "DeviceManagement_RemoteAssistancePartners_BeginOnboarding", "IntuneRemoteAssistancePartnerBeginOnboarding" },
            { "DeviceManagement_RemoteAssistancePartners_Disconnect", "IntuneRemoteAssistancePartnerDisconnect" },
            #endregion
            #region DeviceManagement_TermsAndConditions
            { "DeviceManagement_TermsAndConditions", "IntuneTermsAndConditions" },
            { "DeviceManagement_TermsAndConditions_AcceptanceStatuses", "IntuneTermsAndConditionsAcceptanceStatus" },
            { "DeviceManagement_TermsAndConditions_Assignments", "IntuneTermsAndConditionsAssignment" },
            #endregion
            #endregion
            #region Groups
            { "Groups_CreatedOnBehalfOf", "GroupsCreatedOnBehalfOf" },
            { "Groups_CreatedOnBehalfOfReference", "GroupsCreatedOnBehalfOfReference" },
            { "Groups_GroupLifecyclePolicies", "GroupsGroupLifecyclePolicy" },
            { "Groups_MemberOf", "GroupsMemberOf" },
            { "Groups_MemberOfReferences", "GroupsMemberOfReference" },
            { "Groups_Members", "GroupsMember" },
            { "Groups_MembersReferences", "GroupsMemberReference" },
            { "Groups_Owners", "GroupsOwners" },
            { "Groups_OwnersReferences", "GroupsOwnersReference" },
            { "Groups_Photo", "GroupsPhoto" },
            { "Groups_PhotoData", "GroupsPhotoData" },
            { "Groups_Photos", "GroupsPhotos" },
            { "Groups_PhotosData", "GroupsPhotosData" },
            { "Groups_Settings", "GroupsSettings" },
            { "Groups_AddFavorite", "GroupsAddFavorite" },
            { "Groups_CheckMemberGroups", "GroupsCheckMemberGroup" },
            { "Groups_Delta", "GroupsDelta" },
            { "Groups_GetById", "Groups_GetById" },
            { "Groups_GetMemberGroups", "GroupsGetMemberGroup" },
            { "Groups_GetMemberObjects", "GroupsGetMemberObject" },
            { "Groups_GroupLifecyclePolicies_AddGroup", "GroupsGroupLifecyclePolicyAddGroup" },
            { "Groups_GroupLifecyclePolicies_RemoveGroup", "GroupsGroupLifecyclePolicyRemoveGroup" },
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
