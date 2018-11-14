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
            { "DeviceAppManagement_DefaultManagedAppProtections_TargetApps", "TargetAppsToDefaultAPP" },
            #endregion
            #region DeviceAppManagement_IosManagedAppProtections
            { "DeviceAppManagement_IosManagedAppProtections", "IosAPP" },
            { "DeviceAppManagement_IosManagedAppProtections_Apps", "IosAPPApps" },
            { "DeviceAppManagement_IosManagedAppProtections_Assignments", "IosAPPAppsAssignments" },
            { "DeviceAppManagement_IosManagedAppProtections_DeploymentSummary", "IosAPPAppsDeploymentSummary" },
            { "DeviceAppManagement_IosManagedAppProtections_Assign", "AssignIosAPPApps" },
            #endregion      
            #region DeviceAppManagement_ManagedAppPolicies
            { "DeviceAppManagement_ManagedAppPolicies", "ManagedAppPolicies" },
            { "DeviceAppManagement_ManagedAppPolicies_Apps", "ManagedAppPoliciesApps" },
            { "DeviceAppManagement_ManagedAppPolicies_Assignments", "ManagedAppPoliciesAssignments" },
            { "DeviceAppManagement_ManagedAppPolicies_DeploymentSummary", "ManagedAppPoliciesDeploymentSummary" },
            { "DeviceAppManagement_ManagedAppPolicies_ExemptAppLockerFiles", "ManagedAppPoliciesExemptAppLockerFiles" },
            { "DeviceAppManagement_ManagedAppPolicies_ProtectedAppLockerFiles", "ManagedAppPoliciesProtectedAppLockerFiles" },
            #endregion
            #region DeviceAppManagement_ManagedAppRegistrations
            { "DeviceAppManagement_ManagedAppRegistrations", "AppRegistrations" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies", "AppRegistrationPolicies" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_Apps", "AppRegistrationApps" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_Assignments", "AppRegistrationAssignments" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_DeploymentSummary", "AppRegistrationDeploymentSummary" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_ExemptAppLockerFiles", "AppRegistrationExemptAppLockerFiles" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_ProtectedAppLockerFiles", "AppRegistrationProtectedAppLockerFiles" },
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies", "IntendedPoliciesForAppRegistrations" },
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_Apps", "AppsWithIntendedPoliciesForAppRegistrations" },
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_Assignments", "AssignmentsOfIntendedPoliciesForAppRegistrations" },
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_DeploymentSummary", "DeploymentSummaryOfIntendedPoliciesForAppRegistrations"},
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_ExemptAppLockerFiles", "ExemptAppLockerFilesOfIntendedPoliciesForAppRegistrations" },
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_ProtectedAppLockerFiles", "ProtectedAppLockerFilesOfIntendedPoliciesForAppRegistrations" },            
            { "DeviceAppManagement_ManagedAppRegistrations_Operations", "ManagedAppRegistrationsOperations" },
            { "DeviceAppManagement_ManagedAppRegistrations_AppliedPolicies_TargetApps", "TargetAppsToAppRegistrationPolicies" },
            { "DeviceAppManagement_ManagedAppRegistrations_IntendedPolicies_TargetApps", "TargetAppsToAppRegistrationIntendedPolicies" },
            { "DeviceAppManagement_ManagedAppRegistrations_GetUserIdsWithFlaggedAppRegistration", "GetUserIdsWithFlaggedAppRegistration" },
            #endregion
            #region DeviceAppManagement_ManagedAppStatuses
            { "DeviceAppManagement_ManagedAppStatuses", "ManagedAppStatuses" },
            #endregion
            #region DeviceAppManagement_ManagedEBooks
            { "DeviceAppManagement_ManagedEBooks", "ManagedEBooks" },
            { "DeviceAppManagement_ManagedEBooks_Assignments", "ManagedEBookAssignments"},
            { "DeviceAppManagement_ManagedEBooks_DeviceStates", "ManagedEBooksDeviceStates" },
            { "DeviceAppManagement_ManagedEBooks_InstallSummary", "ManagedEBooksInstallSummary" },
            { "DeviceAppManagement_ManagedEBooks_UserStateSummary", "ManagedEBooksUserStateSummary" },
            { "DeviceAppManagement_ManagedEBooks_UserStateSummary_DeviceStates", "ManagedEBooksUserStateSummaryDeviceStates" },
            { "DeviceAppManagement_ManagedEBooks_Assign", "ManagedEBooksAssign" },
            #endregion
            #region DeviceAppManagement_MdmWindowsInformationProtectionPolicies
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies", "MdmWindowsInformationProtectionPolicies" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_Assignments", "MdmWindowsInformationProtectionPoliciesAssignments" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_ExemptAppLockerFiles", "MdmWindowsInformationProtectionPoliciesExemptAppLockerFiles" },
            { "DeviceAppManagement_MdmWindowsInformationProtectionPolicies_ProtectedAppLockerFiles", "MdmWindowsInformationProtectionPoliciesProtectedAppLockerFiles" },
            #endregion
            #region DeviceAppManagement_MobileAppCategories
            { "DeviceAppManagement_MobileAppCategories", "MobileAppCategories" },
            #endregion
            #region DeviceAppManagement_MobileAppConfigurations
            { "DeviceAppManagement_MobileAppConfigurations", "MobileAppConfigurations" },
            { "DeviceAppManagement_MobileAppConfigurations_Assignments", "MobileAppConfigurationAssignments" },
            { "DeviceAppManagement_MobileAppConfigurations_DeviceStatuses", "MobileAppConfigurationDeviceStatuses" },
            { "DeviceAppManagement_MobileAppConfigurations_DeviceStatusSummary", "MobileAppConfigurationDeviceStatusSummary" },
            { "DeviceAppManagement_MobileAppConfigurations_UserStatuses", "MobileAppConfigurationsUserStatuses" },
            { "DeviceAppManagement_MobileAppConfigurations_UserStatusSummary", "MobileAppConfigurationsUserStatusSummary" },
            #endregion
            #region DeviceAppManagement_VppTokens
            { "DeviceAppManagement_VppTokens", "VppTokens"},
            #endregion
            #region DeviceAppManagement_MobileApps
            { "DeviceAppManagement_MobileApps", "MobileApps" },            
            { "DeviceAppManagement_MobileApps_Assignments", "MobileAppsAssignments" },
            { "DeviceAppManagement_MobileApps_Categories", "MobileAppsCategories" },
            { "DeviceAppManagement_MobileApps_CategoriesReferences", "MobileAppsCategoriesReferences" },
            { "DeviceAppManagement_MobileApps_ContentVersions", "MobileAppsContentVersions" },
            { "DeviceAppManagement_MobileApps_ContentVersions_Files", "MobileAppsContentVersionsFiles"},
            #endregion
            #region DeviceAppManagement_TargetedManagedAppConfigurations
            { "DeviceAppManagement_TargetedManagedAppConfigurations", "TargetedManagedAppConfigurations" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_Apps", "TargetedManagedAppConfigurationsApps" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_Assignments", "TargetedManagedAppConfigurationsAssignments" },
            { "DeviceAppManagement_TargetedManagedAppConfigurations_DeploymentSummary", "TargetedManagedAppConfigurationsDeploymentSummary" }
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
