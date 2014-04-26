// --------------------------------------------------------------------------
// <copyright file="CacheRef.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.17">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Core.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.ReportingService.Core.Constants;
    using Fanex.Common;
    using RemotingRef;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class CacheRef
    {
        public static string GetBettype(object bettypeId)
        {
            return GetBetType(bettypeId, null);
        }

        /// <summary>
        /// Gets the type of the bet.
        /// </summary>
        /// <param name="betTypeId">The bet type identifier.</param>
        /// <param name="betId">The bet identifier.</param>
        /// <returns>The type of the bet.</returns>
        public static string GetBetType(object betTypeId, object betId)
        {
            // resourceId remove first character
            string resourceId = string.Empty;
            if (betId != null && betId.ToString().Length > 1)
            {
                resourceId = betId.ToString().Substring(1);
            }

            return RefData.GetBetTypeName(
                betTypeId.ToString(),
                GetSimpleLanguage(LanguageTypeConstants.EnUS),
                string.Empty,
                resourceId);
        }

        public static string GetTeamName(object teamId)
        {
            return RefData.GetTeamName(
                Converter.ToString2(teamId),
                GetSimpleLanguage(LanguageTypeConstants.EnUS),
                string.Empty);
        }

        public static string GetLeagueName(object leagueId)
        {
            return RefData.GetLeagueName(
                Converter.ToString2(leagueId),
                GetSimpleLanguage(LanguageTypeConstants.EnUS),
                string.Empty);
        }

        /// <summary>
        /// Saves the cache reference response.
        /// </summary>
        /// <param name="identityList">The identifier list.</param>
        /// <param name="cacheRefFunction">The cache reference function.</param>
        /// <param name="currentStorage">The current storage.</param>
        /// <returns>The cache reference response.</returns>
        public static Dictionary<object, string> SaveCacheRefResponse(
            List<object> identityList,
            Func<object, string> cacheRefFunction,
            ref Dictionary<object, string> currentStorage)
        {
            currentStorage = currentStorage ?? new Dictionary<object, string>();
            foreach (var identityInfo in identityList)
            {
                if (!currentStorage.ContainsKey(identityInfo))
                {
                    currentStorage.Add(identityInfo, cacheRefFunction(identityInfo).ToString());
                }
            }

            return currentStorage;
        }

        /// <summary>
        /// Gets the simple language.
        /// </summary>
        /// <param name="lang">The language.</param>
        /// <returns>The simple language.</returns>
        private static string GetSimpleLanguage(string lang)
        {
            switch (lang)
            {
                case LanguageTypeConstants.EnUS:
                    return LanguageTypeConstants.En;
                case LanguageTypeConstants.ZhTW:
                    return LanguageTypeConstants.Ch; // Traditional
                case LanguageTypeConstants.ZhCn:
                    return LanguageTypeConstants.Cs; // Simplified
                case LanguageTypeConstants.JaJp:
                    return LanguageTypeConstants.Jp;
                case LanguageTypeConstants.ThTh:
                    return LanguageTypeConstants.Th; // Thai
                default:
                    return LanguageTypeConstants.En;
            }
        }
    }
}