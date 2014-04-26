// -----------------------------------------------------------------------
// <copyright file="ICompanyStatementService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.08">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Service.Interface
{
    using Fanex.AccountingReports.ReportingService.Criteria.CompanyStatement;
    using Fanex.AccountingReports.ReportingService.Model.CompanyStatement;

    public interface ICompanyStatementService
    {
        CompanyStatementInfo GetCompanyStatement(GetCompanyStatementCriteria criteria);

        void ExportCompanyStatementReport(GetCompanyStatementCriteria criteria);
    }
}