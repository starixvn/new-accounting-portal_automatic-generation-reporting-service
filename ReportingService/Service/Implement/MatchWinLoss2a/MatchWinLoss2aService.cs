// -----------------------------------------------------------------------
// <copyright file="MatchWinLoss2aService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.03.18">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Service.Implement.MatchWinLoss2a
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.AccountingReports.ReportingService.Core.Configurations;
    using Fanex.AccountingReports.ReportingService.Core.Extensions;
    using Fanex.AccountingReports.ReportingService.Core.Services.Impl;
    using Fanex.AccountingReports.ReportingService.Criteria.MatchWinLoss2a;
    using Fanex.AccountingReports.ReportingService.Model.MatchWinLoss2a;
    using Fanex.AccountingReports.ReportingService.Repositories.Implement.MatchWinLoss2a;
    using Fanex.AccountingReports.ReportingService.Repositories.Interface;
    using Fanex.AccountingReports.ReportingService.Resources;
    using Fanex.AccountingReports.ReportingService.Service.Interface;
    using Fanex.Common;
    using Fanex.NAP.Framework.Data;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using CoreConstants = Fanex.AccountingReports.ReportingService.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class MatchWinLoss2aService : ServiceBase, IMatchWinLoss2aService
    {
        private const string NegativeNumberFormat = "#,##0.00;(#,##0.00)";
        private const short SubTotalColorIndex = 29;
        private const short TotalRowColorIndex = 41;
        private const short GrandTotalColorIndex = 32;
        private const short GrandTotalBorderColorIndex = 55;
        private RMElement _8rmConfig;
        private RMElement _barmConfig;
        private Dictionary<object, string> _betTypeNameCache;
        private Dictionary<object, string> _teamNameCache;
        private Dictionary<object, string> _betTypeNameSuffixCache;
        private IMatchWinLoss2aRepository _reportRepository;

        public MatchWinLoss2aService()
            : this(null)
        {
        }

        public MatchWinLoss2aService(IMatchWinLoss2aRepository reportRepository)
        {
            this._8rmConfig = RMSection.Instance.Report[CoreConstants.ReportName.LeagueWinLoss.ToString()]
                                                 .ReportConfigs[RMSection.RMType.EightRM.ToString()];
            this._barmConfig = RMSection.Instance.Report[CoreConstants.ReportName.LeagueWinLoss.ToString()]
                                                 .ReportConfigs[RMSection.RMType.BARM.ToString()];
            this._betTypeNameSuffixCache = new Dictionary<object, string>();
            this._reportRepository = reportRepository ?? new MatchWinLoss2aRepository();
        }

        /// <summary>
        /// ReportRowType: use to export excel.
        /// </summary>
        private enum ReportRowType
        {
            /// <summary>
            /// The normal.
            /// </summary>
            Normal = 1,

            /// <summary>
            /// The sub total.
            /// </summary>
            SubTotal = 2,

            /// <summary>
            /// The total.
            /// </summary>
            Total = 3,

            /// <summary>
            /// The grand total.
            /// </summary>
            GrandTotal = 4,

            /// <summary>
            /// The eight RM.
            /// </summary>
            EightRM = 5,

            /// <summary>
            /// The BARM row.
            /// </summary>
            BARM = 6,

            /// <summary>
            /// The third party.
            /// </summary>
            ThirdParty = 7,

            /// <summary>
            /// The bet type total.
            /// </summary>
            BetTypeTotal = 8
        }

        /// <summary>
        /// Gets the main report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The main report.</returns>
        public dynamic GetMainReport(GetMainReportCriteria criteria)
        {
            try
            {
                Hashtable outParam = new Hashtable();
                var rawData = this._reportRepository.GetMainReport(criteria, ref outParam);

                var non3rdPartyWinLossReport = rawData[0] as List<WinLossInfo>;
                var _3rdPartyWinLossReport = rawData[1] as List<WinLossInfo>;

                if (rawData != null)
                {
                    return new
                    {
                        SummaryReportByBetType = this.GetSummaryReportByBetType(
                            criteria,
                            rawData[2] as List<dynamic>),
                        SummaryReportBySportType = this.GetSummaryReportBySportType(
                            criteria,
                            non3rdPartyWinLossReport,
                            _3rdPartyWinLossReport,
                            outParam)
                    };
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return null;
        }

        /// <summary>
        /// Gets the type of the main report by bet.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Main win loss report by bet type.</returns>
        public SummaryReportByBetType GetMainReportByBetType(GetWinLossByBetTypeCriteria criteria)
        {
            var summaryReportByBetType = new SummaryReportByBetType();
            try
            {
                var winLossByBetType = this._reportRepository.GetWinLossByBetType(criteria);
                summaryReportByBetType = this.GetSummaryReportByBetType(criteria, winLossByBetType);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return summaryReportByBetType;
        }

        private bool IsVirtualSport(int betType)
        {
            var result = false;
            if (CoreConstants.BetTypeConstants.MinVirtualSportBetType <= betType &&
                  betType <= CoreConstants.BetTypeConstants.MaxVirtualSportBetType)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Determines whether [is exclude virtual line total] [the specified bet type].
        /// </summary>
        /// <param name="betType">Type of the bet.</param>
        /// <returns>Is exclude virtual line total.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private bool IsExcludeVirtualLineTotal(int betType)
        {
            var result = false;

            switch (betType)
            {
                case CoreConstants.BetTypeConstants._5MinOU:
                case CoreConstants.BetTypeConstants._15MinOU:
                case CoreConstants.BetTypeConstants._30MinOU:
                case CoreConstants.BetTypeConstants._45MinOU:
                case CoreConstants.BetTypeConstants._60MinOU:
                case CoreConstants.BetTypeConstants.EndOfDayOU:

                case CoreConstants.BetTypeConstants._5MinOE:
                case CoreConstants.BetTypeConstants._15MinOE:
                case CoreConstants.BetTypeConstants._30MinOE:
                case CoreConstants.BetTypeConstants._45MinOE:
                case CoreConstants.BetTypeConstants._60MinOE:
                case CoreConstants.BetTypeConstants.EndOfDayOE:

                case CoreConstants.BetTypeConstants.WinToteOdds:
                case CoreConstants.BetTypeConstants.PlateToteOdds:
                case CoreConstants.BetTypeConstants.WinPlaceToteOdds:

                case CoreConstants.BetTypeConstants.WinFixedOdds:
                case CoreConstants.BetTypeConstants.PlaceFixedOdds:
                case CoreConstants.BetTypeConstants.WinPlaceFixedOdds:

                case CoreConstants.BetTypeConstants.CasinoGames:
                case CoreConstants.BetTypeConstants.P2P:
                case CoreConstants.BetTypeConstants.P2PGames:
                case CoreConstants.BetTypeConstants.Baccarat:
                case CoreConstants.BetTypeConstants.Roulette:
                case CoreConstants.BetTypeConstants.VirtualSportHandicap:
                case CoreConstants.BetTypeConstants.VirtualSportOverUnder:
                case CoreConstants.BetTypeConstants.VirtualSportCorrectScore:
                case CoreConstants.BetTypeConstants._1X2VirtualSport:
                case CoreConstants.BetTypeConstants.VirtualSportTotalGoal:
                case CoreConstants.BetTypeConstants.VirtualSportDoubleChance:
                case CoreConstants.BetTypeConstants.Bingo:
                case CoreConstants.BetTypeConstants.LiveBaccarat:
                case CoreConstants.BetTypeConstants.LiveRoulette:
                case RowType.ThirdParty:
                    result = true;
                    break;

                default:
                    result = false;
                    break;
            }

            if (this.IsVirtualSport(betType))
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Gets the name of the bet type.
        /// </summary>
        /// <param name="betTypeId">The bet type identifier.</param>
        /// <returns>The name of the bet type.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private string GetBetTypeName(int betTypeId)
        {
            string betTypeName = string.Empty;
            var nameSuffix = string.Empty;
            switch (betTypeId)
            {
                case RowType.ThirdParty:
                    betTypeName = ReportingServiceResources.MatchWinLoss2a_ThirdPartyBetTypeName;
                    break;

                case RowType.EightRM:
                    betTypeName = Labels.EightRM;
                    break;

                case RowType.BARM:
                    betTypeName = Labels.BARM;
                    break;

                case CoreConstants.BetTypeConstants.CasinoGames:
                    betTypeName = Labels.Casino.ToUpper();
                    break;

                case CoreConstants.BetTypeConstants.KickOff:
                    betTypeName = Labels.Kick_Off;
                    break;

                case CoreConstants.BetTypeConstants.P2PGames:
                    betTypeName = Labels.P2P;
                    break;

                case CoreConstants.BetTypeConstants.SystemParlay:
                    betTypeName = Labels.System_Parlay;
                    break;

                case CoreConstants.BetTypeConstants.OddEven:
                case CoreConstants.BetTypeConstants.OverUnder:
                    betTypeName = CacheRef.GetBettype(betTypeId);
                    nameSuffix = string.Format("({0})", Labels.NumberGame);
                    break;

                default:
                    betTypeName = CacheRef.GetBettype(betTypeId);
                    break;
            }

            if (this.IsVirtualSport(betTypeId))
            {
                nameSuffix = string.Format("({0})", Labels.Virtual_Sports);
            }

            if (!this._betTypeNameSuffixCache.ContainsKey(betTypeId))
            {
                this._betTypeNameSuffixCache.Add(betTypeId, nameSuffix);
            }

            return betTypeName;
        }

        /// <summary>
        /// Gets the name of the third party bet type.
        /// </summary>
        /// <param name="betType">Type of the bet.</param>
        /// <returns>The name of the third party bet type.</returns>
        private string GetThirdPartyBetTypeName(int betType)
        {
            var result = string.Empty;
            switch (betType)
            {
                case CoreConstants.BetTypeConstants.CasinoGames:
                    result = Labels.Casino;
                    break;

                case CoreConstants.BetTypeConstants.P2P:
                    result = Labels.P2P;
                    break;

                default:
                    result = string.Empty;
                    break;
            }

            return result;
        }

        private bool IsExcludeVirtualLineTotal(string betTypeName)
        {
            return !string.IsNullOrEmpty(betTypeName) &&
                   (string.Equals(betTypeName, Labels.Casino, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(betTypeName, Labels.Racing, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(betTypeName, Labels.LiveCasino, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(betTypeName, Labels.Virtual_Sports, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(betTypeName, Labels.Bingo, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(betTypeName, Labels.P2P, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(betTypeName, Labels.Keno, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Saves the bet type cache reference.
        /// </summary>
        /// <param name="betTypeIdList">The identifier list.</param>
        /// <param name="currentStorage">The current storage.</param>
        /// <returns>The bet type cache reference.</returns>
        private Dictionary<object, string> SaveBetTypeCacheRef(
            List<dynamic> betTypeIdList,
            ref Dictionary<object, string> currentStorage)
        {
            currentStorage = currentStorage ?? new Dictionary<object, string>();
            foreach (var betTypeIdInfo in betTypeIdList)
            {
                if (!currentStorage.ContainsKey(betTypeIdInfo))
                {
                    currentStorage.Add(betTypeIdInfo, this.GetBetTypeName(Converter.ToInt(betTypeIdInfo)));
                }
            }

            return currentStorage;
        }

        /// <summary>
        /// Cache the bet type reference.
        /// </summary>
        /// <param name="betTypeIdList">The bet type identifier list.</param>
        /// <param name="currentStorage">The current storage.</param>
        /// <returns>The bet type cache reference.</returns>
        private Dictionary<object, string> CachedBetTypeRef(
            List<int> betTypeIdList,
            ref Dictionary<object, string> currentStorage)
        {
            currentStorage = currentStorage ?? new Dictionary<object, string>();
            foreach (var betTypeIdInfo in betTypeIdList)
            {
                if (!currentStorage.ContainsKey(betTypeIdInfo))
                {
                    currentStorage.Add(betTypeIdInfo, this.GetBetTypeName(Converter.ToInt(betTypeIdInfo)));
                }
            }

            return currentStorage;
        }

        /// <summary>
        /// Formats the summary report by bet type.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        /// <param name="isExcludeVirtualLineTotal">If set to <c>true</c> [is exclude virtual line total].</param>
        /// <returns>The summary report by bet type.</returns>
        private SummaryReportByBetType FormatSummaryReportByBetType(
            SummaryReportByBetType winLoss,
            bool isExcludeVirtualLineTotal)
        {
            winLoss.LineTotal_Soccer_Live_Str = winLoss.LineTotal_Soccer_Live.Format(2);
            winLoss.LineTotal_Soccer_Non_Live_Str = winLoss.LineTotal_Soccer_Non_Live.Format(2);
            winLoss.LineTotal_Others_Live_Str = winLoss.LineTotal_Others_Live.Format(2);
            winLoss.LineTotal_Others_Non_Live_Str = winLoss.LineTotal_Others_Non_Live.Format(2);
            winLoss.LineTotalReal_Str = winLoss.LineTotalReal.Format(2);
            if (string.IsNullOrEmpty(winLoss.Turnover_Str))
            {
                winLoss.Turnover_Str = winLoss.Turnover.Format(2);
            }

            winLoss.Margin_Str = winLoss.Turnover != 0 ?
                winLoss.Margin.Format(2) : Labels.NotAvailable;
            if (!isExcludeVirtualLineTotal)
            {
                winLoss.VirtualLineTotal_Soccer_Live_Str = winLoss.VirtualLineTotal_Soccer_Live.Format(2);
                winLoss.VirtualLineTotal_Soccer_Non_Live_Str = winLoss.VirtualLineTotal_Soccer_Non_Live.Format(2);
                winLoss.VirtualLineTotal_Others_Live_Str = winLoss.VirtualLineTotal_Others_Live.Format(2);
                winLoss.VirtualLineTotal_Others_Non_Live_Str = winLoss.VirtualLineTotal_Others_Non_Live.Format(2);

                winLoss.LineTotalReal_Str = winLoss.LineTotalReal.Format(2);
                winLoss.LineTotalVirtual_Str = winLoss.LineTotalVirtual.Format(2);
                winLoss.Variance_Str = winLoss.Variance.Format(2);
            }

            return winLoss;
        }

        /// <summary>
        /// Formats the win loss information.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        /// <returns>The win loss information.</returns>
        private WinLossInfo FormatWinLossInfo(WinLossInfo winLoss)
        {
            winLoss.WinLoss_Str = winLoss.WinLoss.Format(2);
            winLoss.Position_Str = winLoss.Position.Format(2);
            winLoss.Turnover_Str = winLoss.Turnover.Format(2);
            winLoss.Commission_Str = winLoss.Commission.Format(2);
            winLoss.LineTotal_Str = winLoss.LineTotal.Format(2);
            winLoss.Margin_Str = winLoss.Turnover != 0 ?
                winLoss.Margin.Format(2) : Labels.NotAvailable;

            if (string.IsNullOrEmpty(winLoss.Turnover_Str))
            {
                winLoss.Turnover_Str = winLoss.Turnover.Format(2);
            }

            if (string.IsNullOrEmpty(winLoss.VirtualComm_Str))
            {
                winLoss.VirtualComm_Str = winLoss.VirtualComm.Format(2);
            }

            if (string.IsNullOrEmpty(winLoss.VirtualLineTotal_Str))
            {
                winLoss.VirtualLineTotal_Str = winLoss.VirtualLineTotal.Format(2);
            }

            if (string.IsNullOrEmpty(winLoss.Variance_Str))
            {
                winLoss.Variance_Str = winLoss.Variance.Format(2);
            }

            return winLoss;
        }

        /// <summary>
        /// Gets the summary report by bet type.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="rawData">The raw data.</param>
        /// <returns>The summary report by bet type.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private SummaryReportByBetType GetSummaryReportByBetType(
            ICriteria criteria,
            List<dynamic> rawData)
        {
            try
            {
                int mixParlayRowIndex = -1;
                int currentRowIndex = 0;

                var betTypeList = rawData.Select(info => Converter.ToInt(info.bettype)).Distinct().ToList();
                this.SaveBetTypeCacheRef(betTypeList, ref this._betTypeNameCache);

                var summaryReport = new SummaryReportByBetType
                {
                    ChildsInfo = new List<SummaryReportByBetType>(),
                    GrandTotal = new SummaryReportByBetType
                    {
                        Name = Labels.Grand_Total
                    }
                };

                foreach (var info in rawData)
                {
                    var betType = Converter.ToInt(info.bettype);
                    if ((betType == RowType.BARM && !this._barmConfig.IsActive) ||
                        (betType == RowType.EightRM && !this._8rmConfig.IsActive))
                    {
                        continue;
                    }

                    SummaryReportByBetType winLoss = new SummaryReportByBetType
                    {
                        Id = betType,
                        LineTotal_Soccer_Live = Converter.ToDecimal(info.LineTotal_soccer_live),
                        LineTotal_Soccer_Non_Live = Converter.ToDecimal(info.LineTotal_soccer),
                        LineTotal_Others_Live = Converter.ToDecimal(info.LineTotal_others_live),
                        LineTotal_Others_Non_Live = Converter.ToDecimal(info.LineTotal_others),
                        Hedge = info.Hedge
                    };

                    bool isKenoBetType = this.IsKenoBetType(winLoss.Id);

                    if (winLoss.Id == CoreConstants.SportContants.Bingo ||
                        winLoss.Id == CoreConstants.SportContants.Casino)
                    {
                        winLoss.Turnover = 0;
                    }
                    else
                    {
                        winLoss.Turnover = Converter.ToDecimal(info.IBCTurnover);
                    }

                    winLoss.Name = isKenoBetType ?
                        Labels.Keno :
                        this._betTypeNameCache[winLoss.Id] +
                        (!string.IsNullOrEmpty(winLoss.Hedge) ? " " + winLoss.Hedge : string.Empty);
                    winLoss.Name_Suffix = isKenoBetType ?
                        string.Empty :
                        this._betTypeNameSuffixCache[winLoss.Id];

                    winLoss.LineTotalReal = winLoss.LineTotal_Soccer_Live +
                        winLoss.LineTotal_Soccer_Non_Live +
                        winLoss.LineTotal_Others_Live +
                        winLoss.LineTotal_Others_Non_Live;
                    winLoss.Margin = this.CalculateMargin(winLoss.LineTotalReal, winLoss.Turnover);

                    summaryReport.GrandTotal.LineTotal_Soccer_Live += winLoss.LineTotal_Soccer_Live;
                    summaryReport.GrandTotal.LineTotal_Soccer_Non_Live += winLoss.LineTotal_Soccer_Non_Live;
                    summaryReport.GrandTotal.LineTotal_Others_Live += winLoss.LineTotal_Others_Live;
                    summaryReport.GrandTotal.LineTotal_Others_Non_Live += winLoss.LineTotal_Others_Non_Live;
                    summaryReport.GrandTotal.LineTotalReal += winLoss.LineTotalReal;
                    summaryReport.GrandTotal.Turnover += winLoss.Turnover;

                    if (this.IsExcludeVirtualLineTotal(betType) || isKenoBetType)
                    {
                        winLoss.VirtualLineTotal_Soccer_Live_Str = Labels.NotAvailable;
                        winLoss.VirtualLineTotal_Soccer_Non_Live_Str = Labels.NotAvailable;
                        winLoss.VirtualLineTotal_Others_Live_Str = Labels.NotAvailable;
                        winLoss.VirtualLineTotal_Others_Non_Live_Str = Labels.NotAvailable;
                        winLoss.LineTotalVirtual_Str = Labels.NotAvailable;
                        winLoss.Variance_Str = Labels.NotAvailable;
                        winLoss = this.FormatSummaryReportByBetType(winLoss, true);
                    }
                    else
                    {
                        winLoss.VirtualLineTotal_Soccer_Live =
                            Converter.ToDecimal(info.VirtualLineTotal_soccer_live);
                        winLoss.VirtualLineTotal_Soccer_Non_Live =
                            Converter.ToDecimal(info.VirtualLineTotal_soccer);
                        winLoss.VirtualLineTotal_Others_Live =
                            Converter.ToDecimal(info.VirtualLineTotal_others_live);
                        winLoss.VirtualLineTotal_Others_Non_Live =
                            Converter.ToDecimal(info.VirtualLineTotal_others);

                        winLoss.LineTotalVirtual = winLoss.VirtualLineTotal_Soccer_Live +
                            winLoss.VirtualLineTotal_Soccer_Non_Live +
                            winLoss.VirtualLineTotal_Others_Live +
                            winLoss.VirtualLineTotal_Others_Non_Live;
                        winLoss.Variance = winLoss.LineTotalReal - winLoss.LineTotalVirtual;

                        summaryReport.GrandTotal.VirtualLineTotal_Soccer_Live += winLoss.VirtualLineTotal_Soccer_Live;
                        summaryReport.GrandTotal.VirtualLineTotal_Soccer_Non_Live += winLoss.VirtualLineTotal_Soccer_Non_Live;
                        summaryReport.GrandTotal.VirtualLineTotal_Others_Live += winLoss.VirtualLineTotal_Others_Live;
                        summaryReport.GrandTotal.VirtualLineTotal_Others_Non_Live += winLoss.VirtualLineTotal_Others_Non_Live;
                        summaryReport.GrandTotal.LineTotalVirtual += winLoss.LineTotalVirtual;
                        summaryReport.GrandTotal.Variance += winLoss.Variance;
                        winLoss = this.FormatSummaryReportByBetType(winLoss, false);
                    }

                    //// InsertReportDetail

                    if (winLoss.Id == CoreConstants.BetTypeConstants.SystemParlay && mixParlayRowIndex != -1)
                    {
                        summaryReport.ChildsInfo.Insert(mixParlayRowIndex, winLoss);
                    }
                    else
                    {
                        if (betType == CoreConstants.BetTypeConstants.MixParlay)
                        {
                            mixParlayRowIndex = currentRowIndex + 1;
                        }

                        summaryReport.ChildsInfo.Add(winLoss);
                    }

                    currentRowIndex++;

                    //// InsertReportDetail
                }

                summaryReport.GrandTotal.Margin = this.CalculateMargin(summaryReport.GrandTotal.LineTotalReal, summaryReport.GrandTotal.Turnover);
                summaryReport.GrandTotal = this.FormatSummaryReportByBetType(summaryReport.GrandTotal, false);
                return summaryReport;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets the summary report by sport type.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="non3rdPartyWinLossReportRaw">The non3rd party win loss report raw.</param>
        /// <param name="_3rdPartyWinLossReportRaw">The _3RD party win loss report raw.</param>
        /// <param name="outParams">The out parameters.</param>
        /// <returns>The summary report by sport type.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private WinLossInfo GetSummaryReportBySportType(
            GetMainReportCriteria criteria,
            List<WinLossInfo> non3rdPartyWinLossReportRaw,
            List<WinLossInfo> _3rdPartyWinLossReportRaw,
            Hashtable outParams)
        {
            try
            {
                WinLossInfo report = new WinLossInfo
                {
                    ThirdPartyInfo = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo()
                    },

                    NonThirdPartyInfo = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.Grand_Total
                        }
                    },

                    BARMTotal = new WinLossInfo
                    {
                        Name = Labels.Total + " " + Labels.BARM
                    },

                    EightRMTotal = new WinLossInfo
                    {
                        Name = Labels.Total + " " + Labels.EightRM
                    },

                    GrandTotal = new WinLossInfo
                    {
                        Name = Labels.Grand_Total
                    }
                };

                //// Non3rdPartyWinLoss

                var betTypeList = non3rdPartyWinLossReportRaw.Select(info => info.Id as dynamic).Distinct().ToList();
                this.SaveBetTypeCacheRef(betTypeList, ref this._betTypeNameCache);
                var sportGroups = non3rdPartyWinLossReportRaw.GroupBy(rawWinLossInfo => rawWinLossInfo.LeagueName);

                foreach (var sportGroup in sportGroups)
                {
                    var sportWinLoss = new WinLossInfo
                    {
                        Name = Converter.ToString2(sportGroup.Key),
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo()
                    };

                    var betTypeGroups = sportGroup.GroupBy(rawWinLossInfo =>
                        new
                        {
                            rawWinLossInfo.Id,
                            rawWinLossInfo.LiveIndicatorId
                        });

                    foreach (var betTypeGroup in betTypeGroups)
                    {
                        var betTypeWinLoss = new WinLossInfo
                        {
                            Id = Converter.ToInt(betTypeGroup.Key.Id),
                            GrandTotal = new WinLossInfo()
                        };

                        var betTypeWinLossList = betTypeGroup.ToList();
                        var isFirstRowOfBetTypeGroup = true;
                        foreach (var info in betTypeWinLossList)
                        {
                            var winLoss = new WinLossInfo
                            {
                                WinLoss = Converter.ToDecimal(info.WinLoss),
                                Position = Converter.ToDecimal(info.Position),
                                Commission = Converter.ToDecimal(info.Commission),
                                VirtualComm = Converter.ToDecimal(info.VirtualComm),
                                LineTotal = Converter.ToDecimal(info.LineTotal),
                                VirtualLineTotal = Converter.ToDecimal(info.VirtualLineTotal),
                                Turnover = Converter.ToDecimal(info.Turnover)
                            };

                            winLoss.Variance = winLoss.LineTotal - winLoss.VirtualLineTotal;
                            if (isFirstRowOfBetTypeGroup)
                            {
                                betTypeWinLoss.Id = Converter.ToInt(info.Id);
                                betTypeWinLoss.Name = this._betTypeNameCache[betTypeWinLoss.Id];
                                betTypeWinLoss.LiveIndicatorId = Converter.ToBool(info.LiveIndicatorId);
                                betTypeWinLoss.MatchCode = Converter.ToString2(info.MatchCode);
                                betTypeWinLoss.KickOffTime = Converter.ToString2(info.KickOffTime);
                                betTypeWinLoss.HomeTeamId = Converter.ToInt(info.HomeTeamId);
                                betTypeWinLoss.AwayTeamId = Converter.ToInt(info.AwayTeamId);
                                betTypeWinLoss.IsNeutral = Converter.ToBool(info.IsNeutral);

                                isFirstRowOfBetTypeGroup = false;
                            }

                            betTypeWinLoss.Add(winLoss);
                        }

                        betTypeWinLoss.Margin = this.CalculateMargin(
                            betTypeWinLoss.LineTotal,
                            betTypeWinLoss.Turnover);
                        sportWinLoss.AddToGrandTotal(betTypeWinLoss);
                        betTypeWinLoss = this.FormatWinLossInfo(betTypeWinLoss);
                        sportWinLoss.ChildsInfo.Add(betTypeWinLoss);
                    }

                    sportWinLoss.GrandTotal.Margin = this.CalculateMargin(
                        sportWinLoss.GrandTotal.LineTotal,
                        sportWinLoss.GrandTotal.Turnover);
                    report.NonThirdPartyInfo.AddToGrandTotal(sportWinLoss.GrandTotal);
                    sportWinLoss.GrandTotal = this.FormatWinLossInfo(sportWinLoss.GrandTotal);
                    report.NonThirdPartyInfo.ChildsInfo.Add(sportWinLoss);
                }

                report.NonThirdPartyInfo.GrandTotal.Margin = this.CalculateMargin(
                    report.NonThirdPartyInfo.GrandTotal.LineTotal,
                    report.NonThirdPartyInfo.GrandTotal.Turnover);
                report.AddToGrandTotal(report.NonThirdPartyInfo.GrandTotal);
                report.NonThirdPartyInfo.GrandTotal = this.FormatWinLossInfo(report.NonThirdPartyInfo.GrandTotal);

                //// Non3rdPartyWinLoss
                //// _3rdPartyWinLoss

                betTypeList = _3rdPartyWinLossReportRaw.Select(info => info.Id as dynamic).Distinct().ToList();
                this.SaveBetTypeCacheRef(betTypeList, ref this._betTypeNameCache);
                foreach (var info in _3rdPartyWinLossReportRaw)
                {
                    var winLoss = new WinLossInfo
                    {
                        WinLoss = Converter.ToDecimal(info.WinLoss),
                        Position = Converter.ToDecimal(info.Position),
                        Commission = Converter.ToDecimal(info.Commission),
                        VirtualComm = Converter.ToDecimal(info.VirtualComm),
                        LineTotal = Converter.ToDecimal(info.LineTotal),
                        VirtualLineTotal = Converter.ToDecimal(info.VirtualLineTotal),
                        Name = Converter.ToString2(info.Name),
                        EventDate = Converter.ToString2(info.EventDate),
                        Turnover = Converter.ToDecimal(info.Turnover)
                    };

                    if (this.IsExcludeVirtualLineTotal(winLoss.Name))
                    {
                        winLoss.VirtualComm = 0;
                        winLoss.VirtualLineTotal = 0;
                        winLoss.Variance = 0;
                        winLoss.VirtualComm_Str = Labels.NotAvailable;
                        winLoss.VirtualLineTotal_Str = Labels.NotAvailable;
                        winLoss.Variance_Str = Labels.NotAvailable;
                    }
                    else
                    {
                        winLoss.Variance = winLoss.LineTotal - winLoss.VirtualLineTotal;
                    }

                    if (string.Equals(winLoss.Name, Labels.Bingo, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(winLoss.Name, Labels.Casino, StringComparison.OrdinalIgnoreCase))
                    {
                        winLoss.Turnover = 0;
                    }

                    winLoss.Margin = this.CalculateMargin(winLoss.LineTotal, winLoss.Turnover);
                    report.ThirdPartyInfo.AddToGrandTotal(winLoss);
                    winLoss = this.FormatWinLossInfo(winLoss);
                    report.ThirdPartyInfo.ChildsInfo.Add(winLoss);
                }

                report.ThirdPartyInfo.GrandTotal.Margin = this.CalculateMargin(
                    report.ThirdPartyInfo.GrandTotal.LineTotal,
                    report.ThirdPartyInfo.GrandTotal.Turnover);
                report.AddToGrandTotal(report.ThirdPartyInfo.GrandTotal);
                report.ThirdPartyInfo.GrandTotal = this.FormatWinLossInfo(report.ThirdPartyInfo.GrandTotal);

                //// _3rdPartyWinLoss

                if (this._8rmConfig.IsActive)
                {
                    report.EightRMTotal.LineTotal = Converter.ToDecimal(outParams["winlost_8rm"]);
                    report.EightRMTotal.VirtualLineTotal = report.EightRMTotal.LineTotal;
                    report.EightRMTotal.Turnover = Converter.ToDecimal(outParams["ibcturnover_8rm"]);
                    report.EightRMTotal.Margin = this.CalculateMargin(
                        report.EightRMTotal.LineTotal,
                        report.EightRMTotal.Turnover);
                    report.AddToGrandTotal(report.EightRMTotal);
                    report.EightRMTotal = this.FormatWinLossInfo(report.EightRMTotal);
                }

                if (this._barmConfig.IsActive)
                {
                    report.BARMTotal.LineTotal = Converter.ToDecimal(outParams["winlost_barm"]);
                    report.BARMTotal.VirtualLineTotal = report.BARMTotal.LineTotal;
                    report.BARMTotal.Turnover = Converter.ToDecimal(outParams["ibcturnover_barm"]);
                    report.BARMTotal.Margin = this.CalculateMargin(
                        report.BARMTotal.LineTotal,
                        report.BARMTotal.Turnover);
                    report.AddToGrandTotal(report.BARMTotal);
                    report.BARMTotal = this.FormatWinLossInfo(report.BARMTotal);
                }

                report.GrandTotal.Margin = this.CalculateMargin(
                    report.GrandTotal.LineTotal,
                    report.GrandTotal.Turnover);
                report.GrandTotal = this.FormatWinLossInfo(report.GrandTotal);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets all detail report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>All detail report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private WinLossInfo GetAllDetailReport(GetMainReportCriteria criteria)
        {
            try
            {
                Hashtable outputParam = new Hashtable();

                WinLossInfo report = new WinLossInfo
                {
                    NonThirdPartyInfo = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.Grand_Total
                        }
                    },
                    ThirdPartyInfo = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.Grand_Total
                        }
                    },
                    GrandTotal = new WinLossInfo
                    {
                        Name = Labels.Grand_Total
                    },
                    EightRMTotal = new WinLossInfo
                    {
                        Name = Labels.Total + " " + Labels.EightRM
                    },
                    BARMTotal = new WinLossInfo
                    {
                        Name = Labels.Total + " " + Labels.BARM
                    },
                };

                var rawData = this._reportRepository.GetMainReport(criteria, ref outputParam);
                var nonThirdPartyData = rawData[0] as List<WinLossInfo>;
                var _3rdPartyWinLossReport = rawData[1] as List<WinLossInfo>;

                report = this.GetAllDetailReportInfo(
                    nonThirdPartyData,
                    _3rdPartyWinLossReport,
                    report);

                if (this._8rmConfig.IsActive)
                {
                    report.EightRMTotal.LineTotal = Converter.ToDecimal(outputParam["winlost_8rm"]);
                    report.EightRMTotal.VirtualLineTotal = report.EightRMTotal.LineTotal;
                    report.EightRMTotal.Turnover = Converter.ToDecimal(outputParam["ibcturnover_8rm"]);
                    report.EightRMTotal.Margin = this.CalculateMargin(
                        report.EightRMTotal.LineTotal,
                        report.EightRMTotal.Turnover);
                    report.AddToGrandTotal(report.EightRMTotal);
                    report.EightRMTotal = this.FormatWinLossInfo(report.EightRMTotal);
                }

                if (this._barmConfig.IsActive)
                {
                    report.BARMTotal.LineTotal = Converter.ToDecimal(outputParam["winlost_barm"]);
                    report.BARMTotal.VirtualLineTotal = report.BARMTotal.LineTotal;
                    report.BARMTotal.Turnover = Converter.ToDecimal(outputParam["ibcturnover_barm"]);
                    report.BARMTotal.Margin = this.CalculateMargin(
                        report.BARMTotal.LineTotal,
                        report.BARMTotal.Turnover);
                    report.AddToGrandTotal(report.BARMTotal);
                    report.BARMTotal = this.FormatWinLossInfo(report.BARMTotal);
                }

                report.GrandTotal.Margin = this.CalculateMargin(
                    report.GrandTotal.LineTotal,
                    report.GrandTotal.Turnover);
                report.GrandTotal = this.FormatWinLossInfo(report.GrandTotal);

                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets all detail report before balance up day.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Detail win loss report before balance up day.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private WinLossInfo GetAllDetailReportBeforeBalanceUpDay(GetAllDetailReportBySportsCriteria criteria)
        {
            try
            {
                Hashtable outputParam = new Hashtable();
                var rawData = this._reportRepository.GetAllDetailReportBySportsBeforeBalanceUpDay(criteria, ref outputParam);
                WinLossInfo report = new WinLossInfo
                {
                    NonThirdPartyInfo = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.Grand_Total
                        }
                    },
                    ThirdPartyInfo = new WinLossInfo
                    {
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.Grand_Total
                        }
                    },
                    GrandTotal = new WinLossInfo
                    {
                        Name = Labels.Grand_Total
                    },
                    EightRMTotal = new WinLossInfo
                    {
                        Name = Labels.Total + " " + Labels.EightRM
                    },
                    BARMTotal = new WinLossInfo
                    {
                        Name = Labels.Total + " " + Labels.BARM
                    },
                };

                var nonThirdPartyData = rawData[0] as List<WinLossInfo>;
                var _3rdPartyWinLossReport = rawData[1] as List<WinLossInfo>;

                report = this.GetAllDetailReportInfo(
                    nonThirdPartyData,
                    _3rdPartyWinLossReport,
                    report);

                if (this._8rmConfig.IsActive)
                {
                    report.EightRMTotal.LineTotal = Converter.ToDecimal(outputParam["winlost_8rm"]);
                    report.EightRMTotal.VirtualLineTotal = report.EightRMTotal.LineTotal;
                    report.EightRMTotal.Turnover = Converter.ToDecimal(outputParam["ibcturnover_8rm"]);
                    report.EightRMTotal.Margin = this.CalculateMargin(
                        report.EightRMTotal.LineTotal,
                        report.EightRMTotal.Turnover);
                    report.AddToGrandTotal(report.EightRMTotal);
                    report.EightRMTotal = this.FormatWinLossInfo(report.EightRMTotal);
                }

                if (this._barmConfig.IsActive)
                {
                    report.BARMTotal.LineTotal = Converter.ToDecimal(outputParam["winlost_barm"]);
                    report.BARMTotal.VirtualLineTotal = report.BARMTotal.LineTotal;
                    report.BARMTotal.Turnover = Converter.ToDecimal(outputParam["ibcturnover_barm"]);
                    report.BARMTotal.Margin = this.CalculateMargin(
                        report.BARMTotal.LineTotal,
                        report.BARMTotal.Turnover);
                    report.AddToGrandTotal(report.BARMTotal);
                    report.BARMTotal = this.FormatWinLossInfo(report.BARMTotal);
                }

                report.GrandTotal.Margin = this.CalculateMargin(
                    report.GrandTotal.LineTotal,
                    report.GrandTotal.Turnover);
                report.GrandTotal = this.FormatWinLossInfo(report.GrandTotal);

                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets all detail report information.
        /// </summary>
        /// <param name="nonThirdPartyData">The non third party data.</param>
        /// <param name="_3rdPartyWinLossReport">The _3RD party win loss report.</param>
        /// <param name="report">The report.</param>
        /// <returns>All detail win loss data.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private WinLossInfo GetAllDetailReportInfo(
            List<WinLossInfo> nonThirdPartyData,
            List<WinLossInfo> _3rdPartyWinLossReport,
            WinLossInfo report)
        {
            var betTypeList = nonThirdPartyData.Select(info => info.Id as dynamic).Distinct().ToList();
            this.SaveBetTypeCacheRef(betTypeList, ref this._betTypeNameCache);
            var homeTeamIdList = nonThirdPartyData.Select(info => info.HomeTeamId as dynamic).Distinct().ToList();
            var awayTeamIdList = nonThirdPartyData.Select(info => info.AwayTeamId as dynamic).Distinct().ToList();
            CacheRef.SaveCacheRefResponse(homeTeamIdList, CacheRef.GetTeamName, ref this._teamNameCache);
            CacheRef.SaveCacheRefResponse(awayTeamIdList, CacheRef.GetTeamName, ref this._teamNameCache);

            var sportGroups = nonThirdPartyData.GroupBy(rawWinLossInfo => rawWinLossInfo.LeagueName);
            foreach (var sportGroup in sportGroups)
            {
                var sportWinLoss = new WinLossInfo
                {
                    Name = Converter.ToString2(sportGroup.Key),
                    ChildsInfo = new List<WinLossInfo>(),
                    GrandTotal = new WinLossInfo()
                };

                var betTypeGroups = sportGroup.GroupBy(rawWinLossInfo => new
                {
                    rawWinLossInfo.Id,
                    rawWinLossInfo.LiveIndicatorId
                });

                foreach (var betTypeGroup in betTypeGroups)
                {
                    var betTypeWinLoss = new WinLossInfo
                    {
                        Id = Converter.ToInt(betTypeGroup.Key.Id),
                        ChildsInfo = new List<WinLossInfo>(),
                        GrandTotal = new WinLossInfo
                        {
                            Name = Labels.Total.ToUpper()
                        }
                    };

                    betTypeWinLoss.Name = this._betTypeNameCache[betTypeWinLoss.Id];
                    betTypeWinLoss.LiveIndicatorId = betTypeGroup.Key.LiveIndicatorId;
                    var betTypeWinLossList = betTypeGroup.ToList();
                    foreach (var info in betTypeWinLossList)
                    {
                        var winLoss = new WinLossInfo
                        {
                            Id = Converter.ToInt(info.Id),
                            Commission = Converter.ToDecimal(info.Commission),
                            VirtualComm = Converter.ToDecimal(info.VirtualComm),
                            LineTotal = Converter.ToDecimal(info.LineTotal),
                            VirtualLineTotal = Converter.ToDecimal(info.VirtualLineTotal),
                            HomeTeamId = Converter.ToInt(info.HomeTeamId),
                            AwayTeamId = Converter.ToInt(info.AwayTeamId),
                            Position = Converter.ToDecimal(info.Position),
                            WinLoss = Converter.ToDecimal(info.WinLoss),
                            Turnover = Converter.ToDecimal(info.Turnover),

                            LiveIndicatorId = Converter.ToBool(info.LiveIndicatorId),
                            MatchCode = Converter.ToString2(info.MatchCode),
                            EventDate = ((DateTime)Converter.ToDateTime(info.KickOffTime)).ToShortFormatedDateWithSlash(),
                            KickOffTime = Converter.ToDateTime(info.KickOffTime).ToShortTimeString(),
                            IsNeutral = Converter.ToBool(info.IsNeutral)
                        };

                        winLoss.Variance = winLoss.LineTotal - winLoss.VirtualLineTotal;
                        winLoss.Margin = this.CalculateMargin(winLoss.LineTotal, winLoss.Turnover);
                        winLoss.Name = this._betTypeNameCache[winLoss.Id];
                        winLoss.HomeTeamName = this._teamNameCache[winLoss.HomeTeamId];
                        winLoss.AwayTeamName = this._teamNameCache[winLoss.AwayTeamId];
                        betTypeWinLoss.AddToGrandTotal(winLoss);
                        winLoss = this.FormatWinLossInfo(winLoss);
                        betTypeWinLoss.ChildsInfo.Add(winLoss);
                    }

                    betTypeWinLoss.GrandTotal.Margin = this.CalculateMargin(
                        betTypeWinLoss.GrandTotal.LineTotal,
                        betTypeWinLoss.GrandTotal.Turnover);
                    sportWinLoss.AddToGrandTotal(betTypeWinLoss.GrandTotal);
                    betTypeWinLoss.GrandTotal = this.FormatWinLossInfo(betTypeWinLoss.GrandTotal);
                    sportWinLoss.ChildsInfo.Add(betTypeWinLoss);
                }

                sportWinLoss.GrandTotal.Margin = this.CalculateMargin(
                    sportWinLoss.GrandTotal.LineTotal,
                    sportWinLoss.GrandTotal.Turnover);
                report.NonThirdPartyInfo.AddToGrandTotal(sportWinLoss.GrandTotal);
                sportWinLoss.GrandTotal = this.FormatWinLossInfo(sportWinLoss.GrandTotal);
                report.NonThirdPartyInfo.ChildsInfo.Add(sportWinLoss);
            }

            report.NonThirdPartyInfo.GrandTotal.Margin = this.CalculateMargin(
                report.NonThirdPartyInfo.GrandTotal.LineTotal,
                report.NonThirdPartyInfo.GrandTotal.Turnover);
            report.AddToGrandTotal(report.NonThirdPartyInfo.GrandTotal);
            report.NonThirdPartyInfo.GrandTotal = this.FormatWinLossInfo(report.NonThirdPartyInfo.GrandTotal);

            //// Non3rdPartyWinLoss
            //// _3rdPartyWinLoss

            betTypeList = _3rdPartyWinLossReport.Select(info => info.Id as dynamic).Distinct().ToList();
            this.SaveBetTypeCacheRef(betTypeList, ref this._betTypeNameCache);
            foreach (var info in _3rdPartyWinLossReport)
            {
                var winLoss = new WinLossInfo
                {
                    Name = Converter.ToString2(info.Name),
                    Commission = Converter.ToDecimal(info.Commission),
                    VirtualComm = Converter.ToDecimal(info.VirtualComm),
                    LineTotal = Converter.ToDecimal(info.LineTotal),
                    VirtualLineTotal = Converter.ToDecimal(info.VirtualLineTotal),
                    Position = Converter.ToDecimal(info.Position),
                    WinLoss = Converter.ToDecimal(info.WinLoss),
                    Turnover = Converter.ToDecimal(info.Turnover),

                    EventDate = ((DateTime)Converter.ToDateTime(info.EventDate)).ToShortFormatedDateWithSlash()
                };

                if (this.IsExcludeVirtualLineTotal(winLoss.Name))
                {
                    winLoss.VirtualComm = 0;
                    winLoss.VirtualLineTotal = 0;
                    winLoss.Variance = 0;
                    winLoss.VirtualComm_Str = Labels.NotAvailable;
                    winLoss.VirtualLineTotal_Str = Labels.NotAvailable;
                    winLoss.Variance_Str = Labels.NotAvailable;
                }
                else
                {
                    winLoss.Variance = winLoss.LineTotal - winLoss.VirtualLineTotal;
                }

                if (string.Equals(winLoss.Name, Labels.Bingo, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(winLoss.Name, Labels.Casino, StringComparison.OrdinalIgnoreCase))
                {
                    winLoss.Turnover = 0;
                }

                report.ThirdPartyInfo.GrandTotal.Margin = this.CalculateMargin(
                    report.ThirdPartyInfo.GrandTotal.LineTotal,
                    report.ThirdPartyInfo.GrandTotal.Turnover);
                report.ThirdPartyInfo.AddToGrandTotal(winLoss);
                winLoss = this.FormatWinLossInfo(winLoss);
                report.ThirdPartyInfo.ChildsInfo.Add(winLoss);
            }

            report.AddToGrandTotal(report.ThirdPartyInfo.GrandTotal);
            report.ThirdPartyInfo.GrandTotal = this.FormatWinLossInfo(report.ThirdPartyInfo.GrandTotal);

            //// _3rdPartyWinLoss

            return report;
        }

        /// <summary>
        /// Calculates the margin.
        /// </summary>
        /// <param name="lineTotal">The line total.</param>
        /// <param name="turnover">The turnover.</param>
        /// <returns>Margin value.</returns>
        private decimal CalculateMargin(decimal lineTotal, decimal turnover)
        {
            decimal result = 0;
            if (turnover != 0)
            {
                result = (lineTotal / turnover) * 100;
            }

            return result;
        }

        /// <summary>
        /// Compares the match information.
        /// </summary>
        /// <param name="winLossInfo">The win loss information.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>True: There is data changes; False: No change.</returns>
        private bool CompareMatchInfo(WinLossInfo winLossInfo, GetDetailReportByBetTypeCriteria criteria)
        {
            if (string.Compare(criteria.Sporttype, winLossInfo.LeagueName, true) != 0 ||
                    (criteria.Bettype != 0 && (criteria.Bettype != winLossInfo.Id ||
                                               criteria.LiveIndicator != winLossInfo.LiveIndicatorId)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Compares the match information before balance up day.
        /// </summary>
        /// <param name="winLossInfo">The win loss information.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>True: There is data changes; False: No change.</returns>
        private bool CompareMatchInfoBeforeBalanceUpDay(WinLossInfo winLossInfo, GetDetailReportByBetTypeCriteria criteria)
        {
            if (criteria.Bettype != 0 && (criteria.Bettype != winLossInfo.Id ||
                                               criteria.LiveIndicator != winLossInfo.LiveIndicatorId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether [is keno bet type] [the specified bet type identifier].
        /// </summary>
        /// <param name="betTypeId">The bet type identifier.</param>
        /// <returns>Bet type is KENO.</returns>
        private bool IsKenoBetType(int betTypeId)
        {
            if (betTypeId < CoreConstants.BetTypeConstants.MinKenoBetType ||
                betTypeId > CoreConstants.BetTypeConstants.MaxKenoBetType)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Renders the merge cell.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row of sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="endRow">The end row.</param>
        /// <param name="endColumn">The end column.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <param name="cellStyle">The cell style.</param>
        private void RenderMergeCell(
            ref ISheet worksheet,
            ref IRow row,
            int currentRow,
            ref int currentColumn,
            int endRow,
            int endColumn,
            object cellValue,
            ICellStyle cellStyle)
        {
            ICell cell = row.CreateCell(currentColumn);

            if (cellValue is string)
            {
                cell.SetCellValue((string)cellValue);
            }
            else if (cellValue is decimal)
            {
                cell.SetCellValue(Convert.ToDouble(cellValue));
            }
            else if (cellValue is int)
            {
                cell.SetCellValue(Convert.ToInt32(cellValue));
            }

            cell.CellStyle = cellStyle;
            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, endRow, currentColumn, endColumn));
            currentColumn = endColumn + 1;
        }

        /// <summary>
        /// Renders the cell.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row of report.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <param name="cellStyle">The cell style.</param>
        private void RenderCell(
            ref ISheet worksheet,
            ref IRow row,
            int currentRow,
            ref int currentColumn,
            object cellValue,
            ICellStyle cellStyle)
        {
            ICell cell = row.CreateCell(currentColumn);

            if (cellValue is string)
            {
                cell.SetCellValue((string)cellValue);
            }
            else if (cellValue is decimal)
            {
                cell.SetCellValue(Convert.ToDouble(cellValue));
            }
            else if (cellValue is int)
            {
                cell.SetCellValue(Convert.ToInt32(cellValue));
            }

            cell.CellStyle = cellStyle;
            currentColumn += 1;
        }

        /// <summary>
        /// Renders the special cell.
        /// </summary>
        /// <param name="row">The row of sheet.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="value_Str">The value_ string.</param>
        /// <param name="value">The value of cell.</param>
        /// <param name="cellTextStyle">The cell text style.</param>
        /// <param name="cellNumberStyle">The cell number style.</param>
        private void RenderSpecialCell(
            ref IRow row,
            ref int currentColumn,
            string value_Str,
            decimal value,
            ICellStyle cellTextStyle,
            ICellStyle cellNumberStyle)
        {
            ICell cell = row.CreateCell(currentColumn);
            object cellValue = null;
            if (value_Str != Labels.NotAvailable)
            {
                cellValue = value;
                cell.CellStyle = cellNumberStyle;
            }
            else
            {
                cellValue = value_Str;
                cellTextStyle.Alignment = HorizontalAlignment.Right;
                cell.CellStyle = cellTextStyle;
            }

            if (cellValue is string)
            {
                cell.SetCellValue((string)cellValue);
            }
            else if (cellValue is decimal)
            {
                cell.SetCellValue(Convert.ToDouble(cellValue));
            }
            else if (cellValue is int)
            {
                cell.SetCellValue(Convert.ToInt32(cellValue));
            }

            currentColumn += 1;
        }

        /// <summary>
        /// Sets the index of the color.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        private void SetColorIndex(HSSFWorkbook excelFile)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            palette.SetColorAtIndex(SubTotalColorIndex, (byte)244, (byte)244, (byte)190);
            palette.SetColorAtIndex(TotalRowColorIndex, (byte)194, (byte)217, (byte)236);
            palette.SetColorAtIndex(GrandTotalColorIndex, (byte)219, (byte)225, (byte)187);
            palette.SetColorAtIndex(GrandTotalBorderColorIndex, (byte)255, (byte)102, (byte)0);
        }

        public struct ReportCellStyle
        {
            public ICellStyle TextStyle;
            public ICellStyle RightTextStyle;
            public ICellStyle CenterTextStyle;
            public ICellStyle BoldTextStyle;
            public ICellStyle DecimalNumberStyle;
            public ICellStyle BoldDecimalNumberStyle;

            public ReportCellStyle(ref HSSFWorkbook excelFile)
            {
                IDataFormat dataFormat = excelFile.CreateDataFormat();
                TextStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Left);
                RightTextStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Right);
                CenterTextStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
                BoldTextStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, true, NpoiExcelExtensions.Alignment.Right);
                DecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, false);
                DecimalNumberStyle.DataFormat = dataFormat.GetFormat(NegativeNumberFormat);
                BoldDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, true);
                BoldDecimalNumberStyle.DataFormat = dataFormat.GetFormat(NegativeNumberFormat);
            }
        }
    }
}