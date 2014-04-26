// -----------------------------------------------------------------------
// <copyright file="IMatchWinLoss2aService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.03.18">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Service.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.AccountingReports.ReportingService.Criteria.MatchWinLoss2a;
    using Fanex.AccountingReports.ReportingService.Model.MatchWinLoss2a;

    public interface IMatchWinLoss2aService
    {
        dynamic GetMainReport(GetMainReportCriteria criteria);

        SummaryReportByBetType GetMainReportByBetType(GetWinLossByBetTypeCriteria criteria);

        void ExportAllDetailReport(GetMainReportCriteria criteria);
    }
}
