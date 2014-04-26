// -----------------------------------------------------------------------
// <copyright file="CompanyStatementInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.08">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Model.CompanyStatement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Company statement model.
    /// </summary>
    public class CompanyStatementInfo
    {
        public CompanyStatementInfo()
        {
            ReportRows = new List<CompanyStatementInfo>();
        }

        public string Date { get; set; }

        public string Type { get; set; }

        public decimal Total { get; set; }

        public decimal AccumulatedTotal { get; set; }

        public List<CompanyStatementInfo> ReportRows { get; set; }
    }
}
