// -----------------------------------------------------------------------
// <copyright file="ISuperYesterdayBalanceService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.08">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Service.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.AccountingReports.ReportingService.Criteria.SuperYesterdayBalance;
    using Fanex.AccountingReports.ReportingService.Model.SuperYesterdayBalance;

    public interface ISuperYesterdayBalanceService
    {
        SuperYesterdayBalance GetSuperYesterdayBalance(GetSuperYesterdayBalanceCriteria superYesterdayBalanceCriteria);

        void ExportSuperYesterdayBalanceReport(GetSuperYesterdayBalanceCriteria superYesterdayBalanceCriteria);
    }
}
