// --------------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Steven.Nguyen" date="2012.10.12">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Core.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class AppSettings
    {
        private static List<string> _excludedControllers;

        private static List<string> _specialIncludedViews;

        private static List<string> _multiAgentLicensses;

        private static List<string> _virtualSportBetTypes = new List<string>();

        private static DateTime _winLossByLicensee_OldTableBaseTime;

        /// <summary>
        /// LiveCasinoFinancialReport_MinDate
        ///     - FDL start operate live casino date: 12/17/2012.
        /// </summary>
        private static DateTime _liveCasinoFinancialReport_MinDate;

        private static List<int> _liveCasinoFinancialReport_Licensee;

        private static List<int> _liveCasinoFinancialReport_Currency;

        private static List<int> _liveCasinoFinancialReport_GameType;

        private static int _topRateMostVisitedPages;

        private static DateTime _mixParlayAdvanceSupportDate;

        private static DateTime _virtualSportsWinLoss_MinDate;
        private static bool _isUsingPort = false;

        static AppSettings()
        {
            LbServiceUrl = ConfigurationManager.AppSettings["LBUrl"];
            IsUMTime = ConfigurationManager.AppSettings["IsUM"] == "1";
            IsLocalTest = ConfigurationManager.AppSettings["IsLocalTest"] == "1";
            _excludedControllers = GetExcludedControllers();
            _specialIncludedViews = GetSpecialIncludedViews();
            _multiAgentLicensses = GetMultiAgentLicensses();
            _winLossByLicensee_OldTableBaseTime = Converter.ToDateTime(
                ConfigurationManager.AppSettings["WinLossByLicensee_OldTableBaseTime"]);
            IntervalGetMostVisitedPage = Converter.ToInt(ConfigurationManager.AppSettings["IntervalGetMostVisitedPage"]);
            GetSportBookBetTypes();
            IPVNReplace = Converter.ToString2(ConfigurationManager.AppSettings["ipvnreplace"]);
            _topRateMostVisitedPages = Converter.ToInt(ConfigurationManager.AppSettings["MostVisitedPages_MaxPages"]);
            _mixParlayAdvanceSupportDate = Converter.ToDateTime(ConfigurationManager.AppSettings["MixParlayAdvanceSupportDate"]);

            _liveCasinoFinancialReport_MinDate = Convert.ToDateTime(ConfigurationManager.AppSettings["LiveCasinoFinancialReport_MinDate"]);
            GetLiveCasinoFinancialReport_Licensee();
            GetLiveCasinoFinancialReport_Currency();
            GetLiveCasinoFinancialReport_GameType();

            _virtualSportsWinLoss_MinDate = Convert.ToDateTime(ConfigurationManager.AppSettings["date_VirtualSport"]);
            _isUsingPort = ConfigurationManager.AppSettings["IsUsingPort"] == "true";
        }

        /// <summary>
        /// Gets the excluded controllers.
        /// </summary>
        /// <value>
        /// The excluded controllers.
        /// </value>
        public static List<string> ExcludedControllers
        {
            get
            {
                return _excludedControllers;
            }
        }

        public static List<string> SpecialIncludedViews
        {
            get
            {
                return _specialIncludedViews;
            }
        }

        public static List<string> MultiAgentLicensses
        {
            get
            {
                return _multiAgentLicensses;
            }
        }

        public static List<string> VirtualSportBetTypes
        {
            get
            {
                return _virtualSportBetTypes;
            }
        }

        public static DateTime WinLossByLicensee_OldTableBaseTime
        {
            get
            {
                return _winLossByLicensee_OldTableBaseTime;
            }
        }

        public static DateTime MixParlayAdvanceSupportDate
        {
            get
            {
                return _mixParlayAdvanceSupportDate;
            }
        }

        public static DateTime LiveCasinoFinancialReport_MinDate
        {
            get
            {
                return _liveCasinoFinancialReport_MinDate;
            }
        }

        public static List<int> LiveCasinoFinancialReport_Licensee
        {
            get
            {
                return _liveCasinoFinancialReport_Licensee;
            }
        }

        public static List<int> LiveCasinoFinancialReport_Currency
        {
            get
            {
                return _liveCasinoFinancialReport_Currency;
            }
        }

        public static List<int> LiveCasinoFinancialReport_GameType
        {
            get
            {
                return _liveCasinoFinancialReport_GameType;
            }
        }

        public static string LbServiceUrl { get; set; }

        public static bool IsUMTime { get; set; }

        public static bool IsLocalTest { get; set; }

        public static int IntervalGetMostVisitedPage { get; private set; }

        public static List<int> SportBookBetTypes { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [is disable most visited page].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is disable most visited page]; otherwise, <c>false</c>.
        /// </value>
        public static bool IsDisableMostvisitedPage
        {
            get
            {
                return Converter.ToBool(ConfigurationManager.AppSettings["IsDisableMostvisitedPage"]);
            }
        }

        public static string IPVNReplace
        {
            get;
            private set;
        }

        public static int TopRateMostVisitedPages
        {
            get
            {
                return _topRateMostVisitedPages;
            }
        }

        public static int MaxPagesOfMostVisitedPage
        {
            get
            {
                return Converter.ToInt(ConfigurationManager.AppSettings["MostVisitedPages_MaxPages"]);
            }
        }

        public static DateTime VirtualSportsWinLoss_MinDate
        {
            get
            {
                return _virtualSportsWinLoss_MinDate;
            }
        }

        public static bool IsUsingPort
        {
            get { return _isUsingPort; }
        }

        private static List<string> GetExcludedControllers()
        {
            List<string> excludedControllers =
                new List<string>(ConfigurationManager
                .AppSettings["PageTracker_ExcludedControllers"]
                .ToString()
                .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            return excludedControllers;
        }

        private static List<string> GetSpecialIncludedViews()
        {
            List<string> specialIncludedViews =
                new List<string>(ConfigurationManager
                .AppSettings["PageTracker_SpecialIncludedViews"]
                .ToString()
                .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            return specialIncludedViews;
        }

        private static List<string> GetMultiAgentLicensses()
        {
            List<string> multiAgentLicensses =
                new List<string>(ConfigurationManager
                .AppSettings["MultiAgentLicensses"]
                .ToString()
                .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            return multiAgentLicensses;
        }

        /// <summary>
        /// Gets the sport book bet types.
        /// </summary>
        private static void GetSportBookBetTypes()
        {
            SportBookBetTypes = new List<int>();
            string[] betTypes = ConfigurationManager
                .AppSettings["SportBookBetType"]
                .ToString()
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string betType in betTypes)
            {
                SportBookBetTypes.Add(Converter.ToInt(betType));
            }
        }

        /// <summary>
        /// Gets the live casino financial report licensee.
        /// </summary>
        private static void GetLiveCasinoFinancialReport_Licensee()
        {
            _liveCasinoFinancialReport_Licensee = new List<int>();
            string[] licensees = ConfigurationManager
                .AppSettings["LiveCasinoFinancialReport_Licensee"]
                .ToString()
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string licensee in licensees)
            {
                _liveCasinoFinancialReport_Licensee.Add(Convert.ToInt32(licensee));
            }
        }

        /// <summary>
        /// Gets the live casino financial report currency.
        /// </summary>
        private static void GetLiveCasinoFinancialReport_Currency()
        {
            _liveCasinoFinancialReport_Currency = new List<int>();
            string[] currencies = ConfigurationManager
                .AppSettings["LiveCasinoFinancialReport_Currency"]
                .ToString()
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string currency in currencies)
            {
                _liveCasinoFinancialReport_Currency.Add(Convert.ToInt32(currency));
            }
        }

        /// <summary>
        /// Gets the type of the live casino financial report game.
        /// </summary>
        private static void GetLiveCasinoFinancialReport_GameType()
        {
            _liveCasinoFinancialReport_GameType = new List<int>();
            string[] gameTypes = ConfigurationManager
                .AppSettings["LiveCasinoFinancialReport_GameType"]
                .ToString()
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string gameType in gameTypes)
            {
                _liveCasinoFinancialReport_GameType.Add(Convert.ToInt32(gameType));
            }
        }        
    }
}