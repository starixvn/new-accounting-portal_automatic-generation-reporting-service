// -----------------------------------------------------------------------
// <copyright file="ISuperYesterdayBalanceRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.08">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Repositories.Interface
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.AccountingReports.ReportingService.Criteria.SuperYesterdayBalance;
    using Fanex.AccountingReports.ReportingService.Model.SuperYesterdayBalance;

    public interface ISuperYesterdayBalanceRepository
    {
        List<dynamic> GetSuperYesterdayBalance(GetSuperYesterdayBalanceCriteria criteria, ref Hashtable outputParam);
    }
}