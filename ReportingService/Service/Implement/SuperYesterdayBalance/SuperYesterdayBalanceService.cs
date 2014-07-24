// -----------------------------------------------------------------------
// <copyright file="SuperYesterdayBalanceService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.08">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Service.Implement.SuperYesterdayBalance
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Fanex.AccountingReports.ReportingService.Common;
    using Fanex.AccountingReports.ReportingService.Core.Extensions;
    using Fanex.AccountingReports.ReportingService.Core.Services.Impl;
    using Fanex.AccountingReports.ReportingService.Criteria.SuperYesterdayBalance;
    using Fanex.AccountingReports.ReportingService.Model.SuperYesterdayBalance;
    using Fanex.AccountingReports.ReportingService.Repositories.Implement.SuperYesterdayBalance;
    using Fanex.AccountingReports.ReportingService.Repositories.Interface;
    using Fanex.AccountingReports.ReportingService.Resources;
    using Fanex.AccountingReports.ReportingService.Service.Interface;
    using Fanex.Common;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using WebConfigurationManager = System.Web.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SuperYesterdayBalanceService : ServiceBase, ISuperYesterdayBalanceService
    {
        private const short EvenRowColorIndex = 41;
        private const short GrandTotalColorIndex = 51;
        private const short GrandTotalBorderColorIndex = 55;
        private ISuperYesterdayBalanceRepository _superYesterdayBalanceRepository;

        public SuperYesterdayBalanceService()
            : this(null)
        {
        }

        public SuperYesterdayBalanceService(ISuperYesterdayBalanceRepository superYesterdayBalanceRepository)
        {
            this._superYesterdayBalanceRepository = superYesterdayBalanceRepository ?? new SuperYesterdayBalanceRepository();
        }

        /// <summary>
        /// Gets the super balance.
        /// </summary>
        /// <param name="superYesterdayBalanceCriteria">The super yesterday balance criteria.</param>
        /// <returns>Super yesterday balance data.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public SuperYesterdayBalance GetSuperYesterdayBalance(GetSuperYesterdayBalanceCriteria superYesterdayBalanceCriteria)
        {
            var result = new SuperYesterdayBalance
            {
                SuperBalanceList = new List<SuperYesterdayBalance>(),
                BaseBalanceTotal = new SuperYesterdayBalance(),
            };

            var outputParam = new Hashtable();

            try
            {
                var rawData = _superYesterdayBalanceRepository.GetSuperYesterdayBalance(superYesterdayBalanceCriteria, ref outputParam);

                if (rawData == null)
                {
                    return result;
                }

                if (superYesterdayBalanceCriteria.IsViewTotalOnly)
                {
                    var superBalanceInfo = new SuperYesterdayBalance()
                    {
                        CustId = null,
                        Username = null,
                        Currency = null,
                        Balance = 0,
                        BaseBalance = Converter.ToDecimal(rawData[0])
                    };

                    result.BaseBalanceTotal.AddToBaseBalance(superBalanceInfo);
                    result.BaseBalanceTotal = SuperYesterdayBalance.FormatInfo(result.BaseBalanceTotal);
                    result.HasData = !Convert.IsDBNull(rawData[0]);
                }
                else
                {
                    foreach (var row in rawData)
                    {
                        var balance = Converter.ToDecimal(row.Balance);
                        var baseBalance = Converter.ToDecimal(row.BaseBalance);

                        if (Math.Round(balance, 2, MidpointRounding.AwayFromZero) == 0 &&
                            Math.Round(baseBalance, 2, MidpointRounding.AwayFromZero) == 0)
                        {
                            continue;
                        }

                        var superBalanceInfo = new SuperYesterdayBalance()
                        {
                            CustId = row.CustId,
                            Username = row.Username,
                            Currency = row.Currency,
                            Balance = balance,
                            BaseBalance = baseBalance
                        };

                        result.BaseBalanceTotal.AddToBaseBalance(superBalanceInfo);
                        superBalanceInfo = SuperYesterdayBalance.FormatInfo(superBalanceInfo);
                        result.SuperBalanceList.Add(superBalanceInfo);
                    }

                    result.BaseBalanceTotal = SuperYesterdayBalance.FormatInfo(result.BaseBalanceTotal);
                }

                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Exports the super yesterday balance report.
        /// </summary>
        /// <param name="superYesterdayBalanceCriteria">The super yesterday balance criteria.</param>
        public void ExportSuperYesterdayBalanceReport(GetSuperYesterdayBalanceCriteria superYesterdayBalanceCriteria)
        {
            FileStream fileStream = null;

            try
            {
                var report = this.GetSuperYesterdayBalance(superYesterdayBalanceCriteria);
                HSSFWorkbook excelFile = new HSSFWorkbook();
                string sheetName = ReportingServiceResources.SuperYesterdayBalance_SheetName;
                ISheet worksheet = excelFile.CreateSheet(sheetName);
                this.SetColorAtIndex(excelFile);

                int currentRow = 0;

                if (report.SuperBalanceList.Count > 0 && report.BaseBalanceTotal != null)
                {
                    this.ExportReportHeader(excelFile, ref worksheet, ref currentRow);
                    this.ExportReportContent(excelFile, ref worksheet, ref currentRow, report);
                    this.ExportReportTotal(excelFile, ref worksheet, ref currentRow, report);
                }

                var reportDirectory = this.CreateSuperYesterdayBalanceReportDirectory();
                var currentDateTime = DateTime.Now.AddDays(-1);
                var reportName = string.Format(
                    ReportingServiceResources.SuperYesterdayBalance_ReportName,
                    DateTimeExtensions.ToShortFormatedDateWithSlash(currentDateTime).Replace('/', '_'));
                var reportPath = Path.Combine(reportDirectory, reportName);
                var fullReportPath = string.Format("{0}.{1}", reportPath, "xls");

                fileStream = new FileStream(
                    fullReportPath,
                    FileMode.Create,
                    FileAccess.Write);

                excelFile.Write(fileStream);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                fileStream.Close();
                fileStream.Dispose();
            }
        }

        /// <summary>
        /// Creates the super yesterday balance report directory.
        /// </summary>
        /// <returns>Super yesterday balance report directory.</returns>
        private string CreateSuperYesterdayBalanceReportDirectory()
        {
            string reportDataDirectory = WebConfigurationManager.WebConfigurationManager.AppSettings["ExportDataDirectory"];
            DirectoryExtensions.CheckAndCreateDirectory(reportDataDirectory);

            DateTime currentDateTime = DateTime.Now.AddDays(-1);

            string directoryByYearAndMonth = string.Format(
                "{0}{1}",
                currentDateTime.ToString("yyyy"),
                currentDateTime.ToString("MM"));
            string directoryByYearAndMonthPath = Path.Combine(reportDataDirectory, directoryByYearAndMonth);
            DirectoryExtensions.CheckAndCreateDirectory(directoryByYearAndMonthPath);

            string directoryByReport = "SuperYesterdayBalance";
            string directoryByReportPath = Path.Combine(directoryByYearAndMonthPath, directoryByReport);
            DirectoryExtensions.CheckAndCreateDirectory(directoryByReportPath);

            return directoryByReportPath;
        }

        /// <summary>
        /// Exports the report header.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        private void ExportReportHeader(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow)
        {
            IRow row = null;
            int currentColumn = 0;
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, true, NpoiExcelExtensions.Alignment.Center);

            for (var i = 0; i < 5; i++)
            {
                worksheet.SetColumnWidth(i, 170 * NpoiExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(0, 60 * NpoiExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(1, 120 * NpoiExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(2, 100 * NpoiExcelExtensions.FontWidth);

            string reportTitle = ReportingServiceResources.SuperYesterdayBalance_ReportTitle;
            row = worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn + 4,
                reportTitle,
                headerStyle);

            currentRow++;
            row = worksheet.CreateRow(currentRow);
            currentColumn = 0;
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn + 4,
                string.Empty,
                headerStyle);

            currentRow++;
        }

        /// <summary>
        /// Exports the content of the report.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportContent(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, SuperYesterdayBalance report)
        {
            ICellStyle intNumberStyle = NpoiExcelExtensions.IntStyle(excelFile, HSSFColor.White.Index, false);
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Left);
            ICellStyle decimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, false);
            ICellStyle evenRowIntNumberStyle = NpoiExcelExtensions.IntStyle(excelFile, EvenRowColorIndex, false);
            ICellStyle evenRowTextStyle = NpoiExcelExtensions.TextStyle(excelFile, EvenRowColorIndex, false, NpoiExcelExtensions.Alignment.Left);
            ICellStyle evenRowDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, EvenRowColorIndex, false);
            int currentColumn = 0;

            var reportLength = report.SuperBalanceList.Count;
            for (int i = 0; i < reportLength; i++)
            {
                var currentReportRow = report.SuperBalanceList[i];
                IRow row = worksheet.CreateRow(currentRow);

                NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                i + 1,
                i % 2 == 0 ? intNumberStyle : evenRowIntNumberStyle);
                NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                currentReportRow.Username,
                i % 2 == 0 ? textStyle : evenRowTextStyle);
                NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                currentReportRow.Currency,
                i % 2 == 0 ? textStyle : evenRowTextStyle);
                NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                currentReportRow.Balance,
                i % 2 == 0 ? decimalNumberStyle : evenRowDecimalNumberStyle);
                NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                currentReportRow.BaseBalance,
                i % 2 == 0 ? decimalNumberStyle : evenRowDecimalNumberStyle);

                currentRow++;
                currentColumn = 0;
            }
        }

        /// <summary>
        /// Exports the report total.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportTotal(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, SuperYesterdayBalance report)
        {
            ICellStyle grandTotalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, true, NpoiExcelExtensions.Alignment.Right);
            grandTotalTextStyle.BorderTop = BorderStyle.Medium;
            grandTotalTextStyle.TopBorderColor = GrandTotalBorderColorIndex;
            ICellStyle grandTotalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, true);
            grandTotalDecimalNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalDecimalNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;
            int currentColumn = 0;

            IRow row = worksheet.CreateRow(currentRow);
            int startMergeColumn = 0;
            int endMergeColumn = 3;
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref startMergeColumn,
                currentRow,
                endMergeColumn,
                Labels.Grand_Total,
                grandTotalTextStyle);
            currentColumn = 4;
            NpoiExcelExtensions.RenderCell(
                 ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                report.BaseBalanceTotal.BaseBalance,
                grandTotalDecimalNumberStyle);
        }

        /// <summary>
        /// Sets the index of the color at.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        private void SetColorAtIndex(HSSFWorkbook excelFile)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            palette.SetColorAtIndex(GrandTotalColorIndex, (byte)219, (byte)225, (byte)187);
            palette.SetColorAtIndex(GrandTotalBorderColorIndex, (byte)255, (byte)102, (byte)0);
            palette.SetColorAtIndex(EvenRowColorIndex, (byte)228, (byte)242, (byte)253);
        }

        /// <summary>
        /// Renders the merge cell.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row of sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="endRow">The end row.</param>
        /// <param name="endColumn">The end column.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <param name="cellStyle">The cell style.</param>
        private void RenderMergeCell(
            ref ISheet worksheet,
            ref IRow row,
            int currentRow,
            ref int currentColumn,
            int endRow,
            int endColumn,
            object cellValue,
            ICellStyle cellStyle)
        {
            for (var i = currentRow; i <= endRow; i++)
            {
                row = worksheet.GetRow(i) ?? worksheet.CreateRow(i);

                for (var j = currentColumn; j <= endColumn; j++)
                {
                    ICell cell = row.GetCell(j) ?? row.CreateCell(j);
                    if (i == currentRow && j == endColumn)
                    {
                        NpoiExcelExtensions.SetCellValue(cell, cellValue);
                    }

                    if (i == currentRow ||
                        i == endRow ||
                        j == currentColumn ||
                        j == endColumn)
                    {
                        cell.CellStyle = cellStyle;
                    }
                }
            }

            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, endRow, currentColumn, endColumn));
            currentColumn = endColumn + 1;
        }
    }
}