// -----------------------------------------------------------------------
// <copyright file="IWinLossByProductsExportDataService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.21">Created</change>
// </history>

namespace WinLossByProducts.Service.Interface
{
    using WinLossByProducts.Criteria;
    using WinLossByProducts.Model;

    public interface IWinLossByProductsExportDataService
    {
        string ExportReportToExcel(WinLossByProductsInfo report, GetWinLossByProductsCriteria criteria);
    }
}