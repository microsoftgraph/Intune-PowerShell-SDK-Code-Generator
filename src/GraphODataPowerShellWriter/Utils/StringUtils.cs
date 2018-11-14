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
            { "DeviceAppManagement_IosManagedAppProtections", "IosAPP" },
            { "DeviceAppManagement_IosManagedAppProtections_Apps", "IosAPPApps" },
            { "DeviceAppManagement_IosManagedAppProtections_Assignments", "IosAPPAppsAssignments" },
            { "DeviceAppManagement_IosManagedAppProtections_DeploymentSummary", "IosAPPAppsDeploymentSummary" },
            { "DeviceAppManagement_IosManagedAppProtections_Assign", "AssignIosAPPApps" },
            { "DeviceAppManagement_IosManagedAppProtections_TargetApps", "TargetIosAPP" },
            #endregion      
            #region DeviceAppManagement_ManagedAppPolicies
            { "DeviceAppManagement_ManagedAppPolicies", "ManagedAP" },
            { "DeviceAppManagement_ManagedAppPolicies_Apps", "ManagedAPApps" },
            { "DeviceAppManagement_ManagedAppPolicies_Assignments", "ManagedAPAssignments" },
            { "DeviceAppManagement_ManagedAppPolicies_DeploymentSummary", "ManagedAPDeploymentSummary" },
            { "DeviceAppManagement_ManagedAppPolicies_ExemptAppLockerFiles", "ManagedAPExemptAppLockerFiles" },
            { "DeviceAppManagement_ManagedAppPolicies_ProtectedAppLockerFiles", "ManagedAPProtectedAppLockerFiles" },
            { "DeviceAppManagement_ManagedAppPolicies_TargetApps", "TargetManagedAP" },
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
            { "DeviceAppManagement_ManagedAppRegistrations_Operations", "AppRegnOps" },
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
            { "DeviceAppManagement_WindowsInformationProtectionPolicies_TargetApps", "TargetWinInfoPP"}
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
