// -----------------------------------------------------------------------
// <copyright file="IMatchWinLoss2aRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.03.18">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Repositories.Interface
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.AccountingReports.ReportingService.Criteria.MatchWinLoss2a;
    using Fanex.AccountingReports.ReportingService.Model.MatchWinLoss2a;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IMatchWinLoss2aRepository
    {
        List<dynamic> GetMainReport(ICriteria criteria, ref Hashtable outputParam);

        List<dynamic> GetWinLossByBetType(GetWinLossByBetTypeCriteria criteria);

        List<dynamic> GetWinLossBySportType(GetMainReportCriteria criteria, ref Hashtable outputParam);

        WinLossInfo GetWinLossDetailBeforeBalanceUpDay(GetDetailReportByBetTypeCriteria criteria);

        List<dynamic> GetAllDetailReport(GetMainReportCriteria criteria, ref Hashtable outputParam);

        List<dynamic> GetAllDetailReportBySportsBeforeBalanceUpDay(GetAllDetailReportBySportsCriteria criteria, ref Hashtable outputParam);
    }
}