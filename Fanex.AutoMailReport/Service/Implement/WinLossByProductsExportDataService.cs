// -----------------------------------------------------------------------
// <copyright file="WinLossByProductsExportDataService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.21">Created</change>
// </history>

namespace WinLossByProducts.Service.Implement
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using WinLossByProducts.Common;
    using WinLossByProducts.Criteria;
    using WinLossByProducts.Extensions;
    using WinLossByProducts.Model;
    using WinLossByProducts.Resources;
    using WinLossByProducts.Service.Interface;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossByProductsExportDataService : IWinLossByProductsExportDataService
    {
        private const short SubTotalColorIndex = 29;
        private const short GrandTotalColorIndex = 32;
        private const short GrandTotalBorderColorIndex = 55;
        private const short RacingDueToPartnersColorIndex = 47;
        private const string FontName = "Tahoma";

        /// <summary>
        /// Exports the report to excel.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Export excel file.</returns>
        public string ExportReportToExcel(WinLossByProductsInfo report, GetWinLossByProductsCriteria criteria)
        {
            FileStream fileStream = null;
            HSSFWorkbook excelFile = new HSSFWorkbook();
            string sheetName = WinLossByProductsResource.WinLossByProducts_SheetName;
            ISheet worksheet = excelFile.CreateSheet(sheetName);
            this.SetColorIndex(excelFile);
            int currentRow = 0;
            if (report.Sportbook.Count > 0 || report.ThirdPartySport.Count > 0)
            {
                this.ExportReportHeader(excelFile, ref worksheet, ref currentRow);
                this.ExportSportBookContent(excelFile, ref worksheet, ref currentRow, report);
                this.ExportSportbookTotal(excelFile, ref worksheet, ref currentRow, report);
                this.ExportThirdPartySport(excelFile, ref worksheet, ref currentRow, report);
            }

            if (report.Total != null)
            {
                this.ExportReportTotal(excelFile, ref worksheet, ref currentRow, report);
            }

            var reportDirectory = this.CreateWinLossByProductsReportDirectory(criteria);
            string reportName = string.Format(
                WinLossByProductsResource.WinLossByProducts_ReportName,
                criteria.WinLossDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
            var reportPath = Path.Combine(reportDirectory, reportName);
            var fullReportPath = string.Format("{0}.{1}", reportPath, "xls");

            fileStream = new FileStream(
                    fullReportPath,
                    FileMode.Create,
                    FileAccess.Write);

            excelFile.Write(fileStream);
            fileStream.Close();
            fileStream.Dispose();

            return fullReportPath;
        }

        /// <summary>
        /// Exports the report header.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        private void ExportReportHeader(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow)
        {
            worksheet.SetColumnWidth(0, 200 * NpoiExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(1, 220 * NpoiExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(2, 220 * NpoiExcelExtensions.FontWidth);

            IRow row = worksheet.CreateRow(currentRow);
            int currentColumn = 0;
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);

            string reportTile = string.Format(WinLossByProductsResource.WinLossByProducts_ReportTitle, DateTime.Now.AddDays(-1).ToShortFormatedDate());
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn + 2,
                reportTile,
                headerStyle);
            currentRow += 2;
            currentColumn = 0;
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                WinLossByProductsLabel.Product,
                headerStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                WinLossByProductsLabel.Win_Loss,
                headerStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                WinLossByProductsLabel.MonthtoDate,
                headerStyle);
            currentRow++;
        }

        /// <summary>
        /// Exports the content of the sport book.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        private void ExportSportBookContent(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, WinLossByProductsInfo report)
        {
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Left);
            ICellStyle decimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, false);

            int currentColumn = 0;
            IRow row = null;
            foreach (var sportbook in report.Sportbook)
            {
                row = worksheet.CreateRow(currentRow);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    sportbook.ProductName,
                    textStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    sportbook.WinLoss,
                    decimalNumberStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    sportbook.MonthToDateWinLoss,
                    decimalNumberStyle);
                currentColumn = 0;
                currentRow++;
            }
        }

        /// <summary>
        /// Exports the third party sport.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportThirdPartySport(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, WinLossByProductsInfo report)
        {
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Left);
            ICellStyle decimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, false);

            IFont font = excelFile.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Normal;
            font.Color = RacingDueToPartnersColorIndex;
            font.FontHeightInPoints = NpoiExcelExtensions.FontSize;
            font.FontName = FontName;

            ICellStyle racingDueToPartnersTextStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Left);
            racingDueToPartnersTextStyle.SetFont(font);
            ICellStyle racingDueToPartnersDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, false);
            racingDueToPartnersDecimalNumberStyle.SetFont(font);

            int currentColumn = 0;
            IRow row = null;

            foreach (var thirdPartySport in report.ThirdPartySport)
            {
                bool isRacingDueToPartners = thirdPartySport.GroupName == "partners" ? true : false;
                row = worksheet.CreateRow(currentRow);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    thirdPartySport.ProductName,
                    isRacingDueToPartners ? racingDueToPartnersTextStyle : textStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    thirdPartySport.WinLoss,
                    isRacingDueToPartners ? racingDueToPartnersDecimalNumberStyle : decimalNumberStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    thirdPartySport.MonthToDateWinLoss,
                    isRacingDueToPartners ? racingDueToPartnersDecimalNumberStyle : decimalNumberStyle);
                currentColumn = 0;
                currentRow++;
            }
        }

        /// <summary>
        /// Exports the report total.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        private void ExportReportTotal(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, WinLossByProductsInfo report)
        {
            ICellStyle grandTotalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Left);
            grandTotalTextStyle.BorderTop = BorderStyle.Medium;
            grandTotalTextStyle.TopBorderColor = GrandTotalBorderColorIndex;
            ICellStyle grandTotalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, GrandTotalColorIndex, true);
            grandTotalDecimalNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalDecimalNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;
            int currentColumn = 0;
            IRow row = worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                report.Total.ProductName,
                grandTotalTextStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                report.Total.WinLoss,
                grandTotalDecimalNumberStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                report.Total.MonthToDateWinLoss,
                grandTotalDecimalNumberStyle);
        }

        /// <summary>
        /// Exports the sport book total.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        private void ExportSportbookTotal(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, WinLossByProductsInfo report)
        {
            ICellStyle totalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, SubTotalColorIndex, true, NpoiExcelExtensions.Alignment.Left);
            ICellStyle totalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, SubTotalColorIndex, true);
            int currentColumn = 0;
            IRow row = worksheet.CreateRow(currentRow);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                report.SportbookTotal.ProductName,
                totalTextStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                report.SportbookTotal.WinLoss,
                totalDecimalNumberStyle);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn,
                report.SportbookTotal.MonthToDateWinLoss,
                totalDecimalNumberStyle);
            currentRow++;
        }

        /// <summary>
        /// Sets the index of the color.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        private void SetColorIndex(HSSFWorkbook excelFile)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            palette.SetColorAtIndex(SubTotalColorIndex, (byte)244, (byte)244, (byte)190);
            palette.SetColorAtIndex(GrandTotalColorIndex, (byte)219, (byte)225, (byte)187);
            palette.SetColorAtIndex(GrandTotalBorderColorIndex, (byte)255, (byte)102, (byte)0);
            palette.SetColorAtIndex(RacingDueToPartnersColorIndex, (byte)192, (byte)192, (byte)192);
        }

        /// <summary>
        /// Creates the win loss by products report directory.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Directory of win loss by products report.</returns>
        private string CreateWinLossByProductsReportDirectory(GetWinLossByProductsCriteria criteria)
        {
            string reportDataDirectory = AppSettings.ExportDataDirectory;

            DirectoryExtensions.CheckAndCreateDirectory(reportDataDirectory);

            string directoryByYearAndMonth = string.Format(
                "{0}{1}",
                criteria.WinLossDate.ToString("yyyy"),
                criteria.WinLossDate.ToString("MM"));
            string directoryByYearAndMonthPath = Path.Combine(reportDataDirectory, directoryByYearAndMonth);
            DirectoryExtensions.CheckAndCreateDirectory(directoryByYearAndMonthPath);

            string directoryByReport = "WinLossByProducts";
            string directoryByReportPath = Path.Combine(directoryByYearAndMonthPath, directoryByReport);
            DirectoryExtensions.CheckAndCreateDirectory(directoryByReportPath);

            return directoryByReportPath;
        }
    }
}