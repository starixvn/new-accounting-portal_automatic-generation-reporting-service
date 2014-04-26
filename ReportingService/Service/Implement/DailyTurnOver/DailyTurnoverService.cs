// -----------------------------------------------------------------------
// <copyright file="DailyTurnoverService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.08">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Service.Implement.DailyTurnOver
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Fanex.AccountingReports.ReportingService.Common;
    using Fanex.AccountingReports.ReportingService.Core.Extensions;
    using Fanex.AccountingReports.ReportingService.Core.Services.Impl;
    using Fanex.AccountingReports.ReportingService.Criteria.DailyTurnOver;
    using Fanex.AccountingReports.ReportingService.Model.DailyTurnOver;
    using Fanex.AccountingReports.ReportingService.Resources;
    using Fanex.AccountingReports.ReportingService.Service.Interface;
    using Fanex.Common;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using WebConfigurationManager = System.Web.Configuration;

    /// <summary>
    /// Daily turn over service.
    /// </summary>
    public class DailyTurnoverService : ServiceBase, IDailyTurnoverService
    {
        private const short SubTotalColorIndex = 29;
        private readonly string _intNumberFormat = "#,##0";
        private ICellStyle _headerStyle;
        private ICellStyle _textStyle;
        private ICellStyle _currencyTextStyle;
        private ICellStyle _decimalNumberStyle;

        /// <summary>
        /// Get report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list DailyTurnover.</returns>
        public List<DailyTurnover> GetReport(GetDailyTurnoverCriteria criteria)
        {
            try
            {
                var report = new List<DailyTurnover>();

                if (!criteria.IsValid())
                {
                    return report;
                }

                report = this.ConvertReportData(criteria);

                return report;
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Export Report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportReport(GetDailyTurnoverCriteria criteria)
        {
            FileStream fileStream = null;
            try
            {
                var report = this.GetReport(criteria);

                HSSFWorkbook excelFile = new HSSFWorkbook();
                this.InitStyle(excelFile);
                this.SetColorIndex(excelFile);

                string sheetName = ReportingServiceResources.DailyTurnover_SheetName;
                ISheet worksheet = excelFile.CreateSheet(sheetName);

                int currentRow = 0;

                if (report.Count > 0)
                {
                    this.ExportReportHeader(ref worksheet, ref currentRow, criteria);
                    this.ExportReportContent(ref worksheet, ref currentRow, report);
                }

                var reportDirectory = this.CreateDailyTurnOverReportDirectory(criteria);
                var reportName = string.Format(
                    ReportingServiceResources.DailyTurnover_ReportName,
                    criteria.FromDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
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
                this.HandleException(ex);
            }
            finally
            {
                fileStream.Close();
                fileStream.Dispose();
            }
        }

        /// <summary>
        /// Creates the daily turn over report directory.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Daily turn over report directory.</returns>
        private string CreateDailyTurnOverReportDirectory(GetDailyTurnoverCriteria criteria)
        {
            string reportDataDirectory = WebConfigurationManager.WebConfigurationManager.AppSettings["ExportDataDirectory"];
            DirectoryExtensions.CheckAndCreateDirectory(reportDataDirectory);

            string directoryByYearAndMonth = string.Format(
                "{0}{1}",
                criteria.FromDate.ToString("yyyy"),
                criteria.FromDate.ToString("MM"));
            string directoryByYearAndMonthPath = Path.Combine(reportDataDirectory, directoryByYearAndMonth);
            DirectoryExtensions.CheckAndCreateDirectory(directoryByYearAndMonthPath);

            string directoryByReport = "DailyTurnOver";
            string directoryByReportPath = Path.Combine(directoryByYearAndMonthPath, directoryByReport);
            DirectoryExtensions.CheckAndCreateDirectory(directoryByReportPath);

            return directoryByReportPath;
        }

        /// <summary>
        /// Convert and put data into report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list Daily Turnover.</returns>
        private List<DailyTurnover> ConvertReportData(GetDailyTurnoverCriteria criteria)
        {
            var report = new List<DailyTurnover>();

            var virtualCurrency = this.GetCurrency(criteria, false);
            var actualCurrency = this.GetCurrency(criteria, true);

            if (virtualCurrency != null && actualCurrency != null)
            {
                report.Add(virtualCurrency);
                report.Add(actualCurrency);
            }

            return report;
        }

        /// <summary>
        /// Get information of currency.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="isActual">The parameter to identify type of currency.</param>
        /// <returns>The Daily Turnover.</returns>
        private DailyTurnover GetCurrency(GetDailyTurnoverCriteria criteria, bool isActual)
        {
            criteria.IsActual = isActual;
            var rowsData = GetReportData<dynamic>(criteria);
            DailyTurnover dailyTurnover = null;

            foreach (var row in rowsData)
            {
                DateTime eventDate = Converter.ToDateTime(row.eventdate);
                int ticketCount = Converter.ToInt(row.TicketCNT);
                int normalCount = Converter.ToInt(row.NormalCNT);
                int rejectCount = Converter.ToInt(row.RejectCNT);

                var turnover = new TurnoverInformation
                {
                    TotalTurnover = Converter.ToDecimal(row.total),
                    SuperTurnover = Converter.ToDecimal(row.super),
                    MasterTurnover = Converter.ToDecimal(row.master),
                    AgentTurnover = Converter.ToDecimal(row.agent),
                    IBCTurnover = Converter.ToDecimal(row.after),
                    IsActual = isActual
                };

                dailyTurnover = new DailyTurnover
                {
                    Date = eventDate.ToShortFormatedDate(),
                    Turnover = turnover,
                    TicketNormalRejectCount = string.Format("{0:n0}", ticketCount)
                                                + "/" + string.Format("{0:n0}", normalCount)
                                                + "/" + string.Format("{0:n0}", rejectCount)
                };
            }

            return dailyTurnover;
        }

        /// <summary>
        /// Initial the common style.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        private void InitStyle(HSSFWorkbook excelFile)
        {
            IDataFormat dataFormat = excelFile.CreateDataFormat();
            this._headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
            this._textStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            this._currencyTextStyle = NpoiExcelExtensions.TextStyle(excelFile, SubTotalColorIndex, true, NpoiExcelExtensions.Alignment.Left);
            this._decimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, false);
            _decimalNumberStyle.DataFormat = dataFormat.GetFormat(_intNumberFormat);
        }

        /// <summary>
        /// Export report header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        private void ExportReportHeader(
            ref ISheet worksheet,
            ref int currentRow,
            GetDailyTurnoverCriteria criteria)
        {
            var currentColumn = 0;

            for (var i = 0; i < 7; i++)
            {
                worksheet.SetColumnWidth(i, 170 * NpoiExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(6, 300 * NpoiExcelExtensions.FontWidth);

            var reportTitle = string.Format(
                    ReportingServiceResources.DailyTurnover_ReportTitle,
                    criteria.FromDate.ToShortFormatedDate());
            IRow row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 6, reportTitle, _headerStyle);
            currentRow += 2;
            currentColumn = 0;

            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, currentColumn, 6));
            currentRow++;
            currentColumn = 0;

            row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Date, _headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Total_Turnover, _headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Super_Turnover, _headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Master_Turnover, _headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.Agent_Turnover, _headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.IBC_Turnover, _headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Labels.CNT, _headerStyle);

            currentRow++;
        }

        /// <summary>
        /// Export report content.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        private void ExportReportContent(
            ref ISheet worksheet,
            ref int currentRow,
            List<DailyTurnover> report)
        {
            this.ExportReportCurrency(ref worksheet, ref currentRow, report[0], true);
            this.ExportReportCurrency(ref worksheet, ref currentRow, report[1], false);
        }

        /// <summary>
        /// Export report by currency.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentCurrency">The current currency.</param>
        /// <param name="isVirtual">The boolean isVirtual.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportCurrency(
            ref ISheet worksheet,
            ref int currentRow,
            DailyTurnover currentCurrency,
            bool isVirtual)
        {
            var currentColumn = 0;
            IRow row = worksheet.CreateRow(currentRow);

            if (isVirtual)
            {
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn + 6,
                    Labels.Virtual_Currency,
                    _currencyTextStyle);
            }
            else
            {
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn + 6,
                    Labels.Actual_Currency,
                    _currencyTextStyle);
            }

            currentRow++;
            row = worksheet.GetRow(currentRow) ?? worksheet.CreateRow(currentRow);
            currentColumn = 0;

            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                currentCurrency.Date,
                _textStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                currentCurrency.Turnover.TotalTurnover,
                _decimalNumberStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                currentCurrency.Turnover.SuperTurnover,
                _decimalNumberStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                currentCurrency.Turnover.MasterTurnover,
                _decimalNumberStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                currentCurrency.Turnover.AgentTurnover,
                _decimalNumberStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                currentCurrency.Turnover.IBCTurnover,
                _decimalNumberStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                currentCurrency.TicketNormalRejectCount,
                _textStyle);

            currentRow++;
        }

        private void SetColorIndex(HSSFWorkbook excelFile)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            palette.SetColorAtIndex(SubTotalColorIndex, (byte)244, (byte)244, (byte)190);
        }
    }
}