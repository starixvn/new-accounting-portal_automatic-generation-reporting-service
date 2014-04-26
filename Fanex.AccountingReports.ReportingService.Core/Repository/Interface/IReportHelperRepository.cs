// --------------------------------------------------------------------------
// <copyright file="IReportHelperRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Core.Repository.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.ReportingService.Core.Criteria;

    public interface IReportHelperRepository
    {
        List<dynamic> GetCurrencyByLicensee(GetCurrencyByLicenseeCriteria criteria);
    }
}