// --------------------------------------------------------------------------
// <copyright file="IServiceBase.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.10.9">Created</change>
//   <change who="Ricky.Nguyen" date="2012.11.6">Move</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Core.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.NAP.Framework.Data;

    public interface IServiceBase
    {
        IEnumerable<T> GetReportData<T>(ICriteria criteria);
    }
}
