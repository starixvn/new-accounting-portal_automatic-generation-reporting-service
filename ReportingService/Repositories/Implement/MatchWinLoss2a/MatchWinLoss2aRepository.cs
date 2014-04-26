// -----------------------------------------------------------------------
// <copyright file="MatchWinLoss2aRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.03.18">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Repositories.Implement.MatchWinLoss2a
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.ReportingService.Core.Services.Impl;
    using Fanex.AccountingReports.ReportingService.Criteria.MatchWinLoss2a;
    using Fanex.AccountingReports.ReportingService.Model.MatchWinLoss2a;
    using Fanex.AccountingReports.ReportingService.Repositories.Interface;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MatchWinLoss2aRepository : IMatchWinLoss2aRepository
    {
        /// <summary>
        /// Gets the main report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output parameter.</param>
        /// <returns>List data reports.</returns>
        public List<dynamic> GetMainReport(ICriteria criteria, ref Hashtable outputParam)
        {
            List<dynamic> report = new List<dynamic>();
            DataSet result = null;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria.GetSpParams());
                outputParam = db.GetOuputParams();
            }

            if (result != null && result.Tables.Count < 3)
            {
                return report;
            }

            var winLossTable = result.Tables[0];
            List<WinLossInfo> non3rdPartyWinLossReport = AssignDataNon3rdPartyWinLossReport(winLossTable);
            report.Add(non3rdPartyWinLossReport);

            var _3rdWinLossTable = result.Tables[1];
            List<WinLossInfo> _3rdPartyWinLossReport = AssignData3rdPartyWinLossReport(_3rdWinLossTable);
            report.Add(_3rdPartyWinLossReport);

            var _summaryByBetTypeTable = result.Tables[2];
            var summaryByBetTypeReport = AssignDataSummaryByBetTypeReportWinLossReport(_summaryByBetTypeTable);
            report.Add(summaryByBetTypeReport);

            return report;
        }

        /// <summary>
        /// Gets all detail report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output parameter.</param>
        /// <returns>All detail report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<dynamic> GetAllDetailReport(GetMainReportCriteria criteria, ref Hashtable outputParam)
        {
            DataSet result = null;
            List<dynamic> report = new List<dynamic>();
            var settingKey = criteria.GetSettingKey();
            using (var objDb = new ObjectDb(settingKey))
            {
                if (settingKey.EndsWith(ReportService.WarehouseSPSuffix))
                {
                    objDb.AddInParameter("fromdate", DbType.DateTime, criteria.Winlostdate);
                    objDb.AddInParameter("todate", DbType.DateTime, criteria.Winlostdate);
                }
                else
                {
                    objDb.AddInParameter("winlostdate", DbType.DateTime, criteria.Winlostdate);
                }

                objDb.AddOutParameter("winlost_8rm", DbType.Double, 8);
                objDb.AddOutParameter("winlost_barm", DbType.Double, 8);
                result = objDb.ExecuteDataSet();
                outputParam = objDb.GetOuputParams();
            }

            var rawData = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var nonThirdPartyData = result.Tables[0];
                foreach (DataRow row in nonThirdPartyData.Rows)
                {
                    rawData.Add(new
                    {
                        leaguenametc = Converter.ToString2(row["leaguenametc"]),
                        bettype = Converter.ToInt(row["bettype"]),
                        comm = Converter.ToDecimal(row["comm"]),
                        VirtualComm = Converter.ToDecimal(row["VirtualComm"]),
                        LineTotal = Converter.ToDecimal(row["LineTotal"]),
                        VirtualLineTotal = Converter.ToDecimal(row["VirtualLineTotal"]),
                        homeid = Converter.ToInt(row["homeid"]),
                        awayid = Converter.ToInt(row["awayid"]),
                        pos = Converter.ToDecimal(row["pos"]),
                        winlost = Converter.ToDecimal(row["winlost"]),
                        liveindicator = Converter.ToBool(row["liveindicator"]),
                        matchcode = Converter.ToString2(row["matchcode"]),
                        eventdate = Converter.ToDateTime(row["eventdate"]),
                        kickofftime = Converter.ToDateTime(row["kickofftime"]),
                        IsNeutral = Converter.ToBool(row["IsNeutral"])
                    });
                }
            }

            report.Add(rawData);
            rawData = new List<dynamic>();
            if (result != null && result.Tables.Count >= 2)
            {
                var thirdPartyData = result.Tables[1];
                foreach (DataRow row in thirdPartyData.Rows)
                {
                    rawData.Add(new
                    {
                        bettype = Converter.ToString2(row["bettype"]),
                        comm = Converter.ToDecimal(row["comm"]),
                        VirtualComm = Converter.ToDecimal(row["VirtualComm"]),
                        LineTotal = Converter.ToDecimal(row["LineTotal"]),
                        VirtualLineTotal = Converter.ToDecimal(row["VirtualLineTotal"]),
                        pos = Converter.ToDecimal(row["pos"]),
                        winlost = Converter.ToDecimal(row["winlost"]),
                        winlostdate = Converter.ToDateTime(row["winlostdate"])
                    });
                }
            }

            report.Add(rawData);
            return report;
        }

        /// <summary>
        /// Gets all detail report by sports before balance up day.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output parameter.</param>
        /// <returns>Detail report by sport before balance up day.</returns>
        public List<dynamic> GetAllDetailReportBySportsBeforeBalanceUpDay(GetAllDetailReportBySportsCriteria criteria, ref Hashtable outputParam)
        {
            DataSet result = null;
            List<dynamic> report = new List<dynamic>();
            var settingKey = criteria.GetSettingKey();
            using (var objDb = new ObjectDb(settingKey))
            {
                if (settingKey.EndsWith(ReportService.WarehouseSPSuffix))
                {
                    objDb.AddInParameter("fromdate", DbType.DateTime, criteria.Winlostdate);
                    objDb.AddInParameter("todate", DbType.DateTime, criteria.Winlostdate);
                }
                else
                {
                    objDb.AddInParameter("winlostdate", DbType.DateTime, criteria.Winlostdate);
                }

                objDb.AddOutParameter("winlost_8rm", DbType.Double, 8);
                objDb.AddOutParameter("winlost_barm", DbType.Double, 8);
                objDb.AddOutParameter("ibcturnover_8rm", DbType.Double, 8);
                objDb.AddOutParameter("ibcturnover_barm", DbType.Double, 8);
                result = objDb.ExecuteDataSet();
                outputParam = objDb.GetOuputParams();
            }

            var winLossTable = result.Tables[0];
            List<WinLossInfo> non3rdPartyWinLossReport = AssignDataNon3rdPartyWinLossReport(winLossTable);
            report.Add(non3rdPartyWinLossReport);

            var _3rdWinLossTable = result.Tables[1];
            List<WinLossInfo> _3rdPartyWinLossReport = AssignData3rdPartyWinLossReport(_3rdWinLossTable);
            report.Add(_3rdPartyWinLossReport);

            return report;
        }

        /// <summary>
        /// Gets the type of the win loss by bet.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List win loss by bet type.</returns>
        public List<dynamic> GetWinLossByBetType(GetWinLossByBetTypeCriteria criteria)
        {
            DataSet result = null;

            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            var winLossByBetTypeTable = result.Tables[0];
            var winLossByBetType = AssignDataSummaryByBetTypeReportWinLossReport(winLossByBetTypeTable);

            return winLossByBetType;
        }

        /// <summary>
        /// Gets the type of the win loss by sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output parameter.</param>
        /// <returns>List win loss by sport type.</returns>
        public List<dynamic> GetWinLossBySportType(GetMainReportCriteria criteria, ref Hashtable outputParam)
        {
            var winLossBySportType = new List<dynamic>();
            DataSet result = null;

            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
                outputParam = objectDb.GetOuputParams();
            }

            if (result != null && result.Tables.Count < 2)
            {
                return winLossBySportType;
            }

            var winLossTable = result.Tables[0];
            List<WinLossInfo> non3rdPartyWinLossReport = AssignNon3rdPartyWinLossReportDataBeforeBalanceUpDay(winLossTable);
            winLossBySportType.Add(non3rdPartyWinLossReport);

            var _3rdWinLossTable = result.Tables[1];
            List<WinLossInfo> _3rdPartyWinLossReport = AssignData3rdPartyWinLossReport(_3rdWinLossTable);
            winLossBySportType.Add(_3rdPartyWinLossReport);

            return winLossBySportType;
        }

        /// <summary>
        /// Gets the win loss detail before balance up day.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Win loss detail before balance up day.</returns>
        public WinLossInfo GetWinLossDetailBeforeBalanceUpDay(GetDetailReportByBetTypeCriteria criteria)
        {
            var result = new WinLossInfo
            {
                ChildsInfo = new List<WinLossInfo>()
            };

            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var reader = objectDb.ExecuteReader(criteria.GetSpParams());

                while (reader.Read())
                {
                    var winLossInfo = new WinLossInfo
                    {
                        MatchCode = Converter.ToString2(reader["matchcode"]),
                        KickOffTime = Converter.ToString2(reader["kickofftime"]),
                        LiveIndicatorId = Converter.ToBool(reader["liveindicator"]),
                        HomeTeamId = Converter.ToInt(reader["homeid"]),
                        AwayTeamId = Converter.ToInt(reader["awayid"]),
                        IsNeutral = Converter.ToBool(reader["isneutral"]),
                        WinLoss = Converter.ToDecimal(reader["winlost"]),
                        Position = Converter.ToDecimal(reader["pos"]),
                        Commission = Converter.ToDecimal(reader["comm"]),
                        VirtualComm = Converter.ToDecimal(reader["virtualcomm"]),
                        LineTotal = Converter.ToDecimal(reader["linetotal"]),
                        VirtualLineTotal = Converter.ToDecimal(reader["virtuallinetotal"]),
                        Turnover = Converter.ToDecimal(reader["IBCTurnOver"]),
                        Id = Converter.ToInt(reader["bettype"])
                    };

                    result.ChildsInfo.Add(winLossInfo);
                }
            }

            return result;
        }

        /// <summary>
        /// Assigns the data non3rd party win loss report.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <returns>None third party win loss data.</returns>
        private List<WinLossInfo> AssignDataNon3rdPartyWinLossReport(DataTable dataTable)
        {
            List<WinLossInfo> lstWinLossInfo = new List<WinLossInfo>();

            if (dataTable == null && dataTable.Rows.Count == 0)
            {
                return lstWinLossInfo;
            }

            DataRow[] rows = dataTable.Select();
            foreach (DataRow row in rows)
            {
                lstWinLossInfo.Add(new WinLossInfo()
                {
                    WinLoss = Converter.ToDecimal(row["winlost"]),
                    Position = Converter.ToDecimal(row["pos"]),
                    Commission = Converter.ToDecimal(row["comm"]),
                    VirtualComm = Converter.ToDecimal(row["VirtualComm"]),
                    LineTotal = Converter.ToDecimal(row["LineTotal"]),
                    VirtualLineTotal = Converter.ToDecimal(row["VirtualLineTotal"]),
                    Id = Converter.ToInt(row["bettype"]),
                    LeagueName = Converter.ToString2(row["leaguenametc"]),
                    LiveIndicatorId = Converter.ToBool(row["liveindicator"]),
                    Turnover = Converter.ToDecimal(row["IBCTurnover"]),
                    MatchCode = Converter.ToString2(row["matchcode"]),
                    KickOffTime = Converter.ToString2(row["kickofftime"]),
                    HomeTeamId = Converter.ToInt(row["homeid"]),
                    AwayTeamId = Converter.ToInt(row["awayid"]),
                    IsNeutral = Converter.ToBool(row["IsNeutral"])
                });
            }

            return lstWinLossInfo;
        }

        /// <summary>
        /// Assigns the non3rd party win loss report data before balance up day.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <returns>None third party data before balance up day.</returns>
        private List<WinLossInfo> AssignNon3rdPartyWinLossReportDataBeforeBalanceUpDay(DataTable dataTable)
        {
            List<WinLossInfo> lstWinLossInfo = new List<WinLossInfo>();

            if (dataTable == null && dataTable.Rows.Count == 0)
            {
                return lstWinLossInfo;
            }

            DataRow[] rows = dataTable.Select();
            foreach (DataRow row in rows)
            {
                lstWinLossInfo.Add(new WinLossInfo()
                {
                    Commission = Converter.ToDecimal(row["comm"]),
                    VirtualComm = Converter.ToDecimal(row["VirtualComm"]),
                    LineTotal = Converter.ToDecimal(row["LineTotal"]),
                    VirtualLineTotal = Converter.ToDecimal(row["VirtualLineTotal"]),
                    Id = Converter.ToInt(row["bettype"]),
                    LeagueName = Converter.ToString2(row["leaguenametc"]),
                    LiveIndicatorId = Converter.ToBool(row["liveindicator"]),
                    Turnover = Converter.ToDecimal(row["IBCTurnover"])
                });
            }

            return lstWinLossInfo;
        }

        /// <summary>
        /// Assigns the data3rd party win loss report.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <returns>Third party win loss data.</returns>
        private List<WinLossInfo> AssignData3rdPartyWinLossReport(DataTable dataTable)
        {
            List<WinLossInfo> lstWinLossInfo = new List<WinLossInfo>();

            if (dataTable == null && dataTable.Rows.Count == 0)
            {
                return lstWinLossInfo;
            }

            DataRow[] rows = dataTable.Select();
            foreach (DataRow row in rows)
            {
                lstWinLossInfo.Add(new WinLossInfo()
                {
                    WinLoss = Converter.ToDecimal(row["winlost"]),
                    Position = Converter.ToDecimal(row["pos"]),
                    Commission = Converter.ToDecimal(row["comm"]),
                    VirtualComm = Converter.ToDecimal(row["virtualComm"]),
                    LineTotal = Converter.ToDecimal(row["LineTotal"]),
                    VirtualLineTotal = Converter.ToDecimal(row["VirtualLineTotal"]),
                    Turnover = Converter.ToDecimal(row["IBCTurnover"]),

                    Name = Converter.ToString2(row["bettype"]),
                    EventDate = Converter.ToString2(row["winlostdate"])
                });
            }

            return lstWinLossInfo;
        }

        /// <summary>
        /// Assigns the data summary by bet type report win loss report.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <returns>List dynamic.</returns>
        private List<dynamic> AssignDataSummaryByBetTypeReportWinLossReport(DataTable dataTable)
        {
            List<dynamic> lstWinLossInfo = new List<dynamic>();

            if (dataTable == null && dataTable.Rows.Count == 0)
            {
                return lstWinLossInfo;
            }

            DataRow[] rows = dataTable.Select();
            foreach (DataRow row in rows)
            {
                lstWinLossInfo.Add(new
                {
                    bettype = Converter.ToString2(row["bettype"]),
                    IBCTurnover = Converter.ToDecimal(row["IBCTurnover"]),
                    Hedge = Converter.ToString2(row["Hedge"]),
                    LineTotal_soccer_live = Converter.ToDecimal(row["LineTotal_soccer_live"]),
                    VirtualLineTotal_soccer_live = Converter.ToDecimal(row["VirtualLineTotal_soccer_live"]),
                    LineTotal_soccer = Converter.ToDecimal(row["LineTotal_soccer"]),
                    VirtualLineTotal_soccer = Converter.ToDecimal(row["VirtualLineTotal_soccer"]),
                    LineTotal_others_live = Converter.ToDecimal(row["LineTotal_others_live"]),
                    VirtualLineTotal_others_live = Converter.ToDecimal(row["VirtualLineTotal_others_live"]),
                    LineTotal_others = Converter.ToDecimal(row["LineTotal_others"]),
                    VirtualLineTotal_others = Converter.ToDecimal(row["VirtualLineTotal_others"])
                });
            }

            return lstWinLossInfo;
        }
    }
}