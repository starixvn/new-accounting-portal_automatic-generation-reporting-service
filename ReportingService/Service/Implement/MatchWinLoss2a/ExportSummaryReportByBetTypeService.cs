// -----------------------------------------------------------------------
// <copyright file="ExportSummaryReportByBetTypeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.25">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Service.Implement.MatchWinLoss2a
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.ReportingService.Core.Extensions;
    using Fanex.AccountingReports.ReportingService.Model.MatchWinLoss2a;
    using Fanex.AccountingReports.ReportingService.Resources;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// Export summary report by bet type.
    /// </summary>
    public partial class MatchWinLoss2aService
    {
        private void SummaryReportByBetType_InitColumnSize(
            ref ISheet worksheet)
        {
            for (var i = 0; i < 14; i++)
            {
                worksheet.SetColumnWidth(i, 140 * NpoiExcelExtensions.FontWidth);
            }
        }

        private void ExportSummaryReportByBetTypeTitle(
            ref ISheet worksheet,
            ref int currentRow,
            string reportTitle,
            ICellStyle headerStyle)
        {
            var currentColumn = 0;

            IRow row = worksheet.CreateRow(currentRow);

            this.RenderMergeCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn + 13, reportTitle, headerStyle);
            currentRow += 2;
            worksheet.AddMergedRegion(new CellRangeAddress(currentRow, currentRow, 0, 13));
        }

        /// <summary>
        /// Exports the summary report by bet type header information.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="headerStyle">The header style.</param>
        private void ExportSummaryReportByBetTypeHeaderInfo(
            ref ISheet worksheet,
            ref int currentRow,
            ICellStyle headerStyle)
        {
            var currentColumn = 0;
            IRow row = worksheet.CreateRow(currentRow);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Bet_Type, headerStyle);
            this.RenderChildGroupHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, Labels.Soccer_Live, headerStyle);
            this.RenderChildGroupHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, Labels.Soccer_Non_Live, headerStyle);
            this.RenderChildGroupHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, Labels.Other_Live, headerStyle);
            this.RenderChildGroupHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, Labels.Other_Non_Live, headerStyle);
            this.RenderChildGroupHeader(ref worksheet, ref row, ref currentRow, ref currentColumn, Labels.Line_Total, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Variance, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Turnover, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Margin_Percent, headerStyle);

            currentRow++;
            row = worksheet.CreateRow(currentRow);
            currentColumn = 0;

            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Bet_Type, headerStyle);
            this.RenderChildGroupHeaderDetail(ref worksheet, ref row, ref currentRow, ref currentColumn, headerStyle);
            this.RenderChildGroupHeaderDetail(ref worksheet, ref row, ref currentRow, ref currentColumn, headerStyle);
            this.RenderChildGroupHeaderDetail(ref worksheet, ref row, ref currentRow, ref currentColumn, headerStyle);
            this.RenderChildGroupHeaderDetail(ref worksheet, ref row, ref currentRow, ref currentColumn, headerStyle);
            this.RenderChildGroupHeaderDetail(ref worksheet, ref row, ref currentRow, ref currentColumn, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Variance, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Turnover, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Margin_Percent, headerStyle);

            currentColumn = 0;
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow, currentColumn, Labels.Bet_Type, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow - 1, currentColumn + 1, Labels.Real, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow - 1, currentColumn + 1, Labels.Real, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow - 1, currentColumn + 1, Labels.Real, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow - 1, currentColumn + 1, Labels.Real, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow - 1, currentColumn + 1, Labels.Real, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow, currentColumn, Labels.Variance, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow, currentColumn, Labels.Turnover, headerStyle);
            this.RenderMergeCell(ref worksheet, ref row, currentRow - 1, ref currentColumn, currentRow, currentColumn, Labels.Margin_Percent, headerStyle);
        }

        /// <summary>
        /// Exports the content of the summary report by bet type.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="report">The report.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportSummaryReportByBetTypeContent(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            SummaryReportByBetType report)
        {
            IDataFormat dataFormat = excelFile.CreateDataFormat();
            ICellStyle decimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, false);
            decimalNumberStyle.DataFormat = dataFormat.GetFormat(NegativeNumberFormat);
            ICellStyle leftAlignTextStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Left);
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Right);

            IRow row = worksheet.CreateRow(currentRow);
            var reportLength = report.ChildsInfo.Count;
            var currentColumn = 0;
            for (var rowIndex = 0; rowIndex < reportLength; rowIndex++)
            {
                var rowInfo = report.ChildsInfo[rowIndex];
                var bettypeName = rowInfo.Name +
                    (!string.IsNullOrEmpty(rowInfo.Name_Suffix) ? rowInfo.Name_Suffix : string.Empty);
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, bettypeName, leftAlignTextStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    rowInfo.LineTotal_Soccer_Live_Str,
                    rowInfo.LineTotal_Soccer_Live,
                    textStyle,
                    decimalNumberStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    rowInfo.VirtualLineTotal_Soccer_Live_Str,
                    rowInfo.VirtualLineTotal_Soccer_Live,
                    textStyle,
                    decimalNumberStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    rowInfo.LineTotal_Soccer_Non_Live_Str,
                    rowInfo.LineTotal_Soccer_Non_Live,
                    textStyle,
                    decimalNumberStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    rowInfo.VirtualLineTotal_Soccer_Non_Live_Str,
                    rowInfo.VirtualLineTotal_Soccer_Non_Live,
                    textStyle,
                    decimalNumberStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    rowInfo.LineTotal_Others_Live_Str,
                    rowInfo.LineTotal_Others_Live,
                    textStyle,
                    decimalNumberStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    rowInfo.VirtualLineTotal_Others_Live_Str,
                    rowInfo.VirtualLineTotal_Others_Live,
                    textStyle,
                    decimalNumberStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    rowInfo.LineTotal_Others_Non_Live_Str,
                    rowInfo.LineTotal_Others_Non_Live,
                    textStyle,
                    decimalNumberStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    rowInfo.VirtualLineTotal_Others_Non_Live_Str,
                    rowInfo.VirtualLineTotal_Others_Non_Live,
                    textStyle,
                    decimalNumberStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    rowInfo.LineTotalReal_Str,
                    rowInfo.LineTotalReal,
                    textStyle,
                    decimalNumberStyle);
                this.RenderSpecialCell(
                   ref row,
                   ref currentColumn,
                   rowInfo.LineTotalVirtual_Str,
                   rowInfo.LineTotalVirtual,
                   textStyle,
                   decimalNumberStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    rowInfo.Variance_Str,
                    rowInfo.Variance,
                    textStyle,
                    decimalNumberStyle);
                this.RenderSpecialCell(
                   ref row,
                   ref currentColumn,
                   rowInfo.Turnover_Str,
                   rowInfo.Turnover,
                   textStyle,
                   decimalNumberStyle);
                this.RenderSpecialCell(
                   ref row,
                   ref currentColumn,
                   rowInfo.Margin_Str,
                   rowInfo.Margin,
                   textStyle,
                   decimalNumberStyle);

                currentRow++;
                row = worksheet.CreateRow(currentRow);
                currentColumn = 0;
            }

            this.ExportSummaryReportTotal(
                excelFile,
                ref worksheet,
                ref row,
                ref currentRow,
                ref currentColumn,
                report.GrandTotal);
        }

        /// <summary>
        /// Exports the summary report total.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row of sheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="currentColumn">The current column.</param>
        /// <param name="grandTotal">The grand total.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportSummaryReportTotal(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref IRow row,
            ref int currentRow,
            ref int currentColumn,
            SummaryReportByBetType grandTotal)
        {
            IDataFormat dataFormat = excelFile.CreateDataFormat();
            this.SetColorIndex(excelFile);
            ICellStyle grandTotalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, GrandTotalColorIndex, true);
            grandTotalDecimalNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalDecimalNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;
            grandTotalDecimalNumberStyle.DataFormat = dataFormat.GetFormat(NegativeNumberFormat);
            ICellStyle grandTotalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
            grandTotalTextStyle.BorderTop = BorderStyle.Medium;
            grandTotalTextStyle.TopBorderColor = GrandTotalBorderColorIndex;
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Grand_Total, grandTotalTextStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                grandTotal.LineTotal_Soccer_Live_Str,
                grandTotal.LineTotal_Soccer_Live,
                grandTotalTextStyle,
                grandTotalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                grandTotal.VirtualLineTotal_Soccer_Live_Str,
                grandTotal.VirtualLineTotal_Soccer_Live,
                grandTotalTextStyle,
                grandTotalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                grandTotal.LineTotal_Soccer_Non_Live_Str,
                grandTotal.LineTotal_Soccer_Non_Live,
                grandTotalTextStyle,
                grandTotalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                grandTotal.VirtualLineTotal_Soccer_Non_Live_Str,
                grandTotal.VirtualLineTotal_Soccer_Non_Live,
                grandTotalTextStyle,
                grandTotalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                grandTotal.LineTotal_Others_Live_Str,
                grandTotal.LineTotal_Others_Live,
                grandTotalTextStyle,
                grandTotalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                grandTotal.VirtualLineTotal_Others_Live_Str,
                grandTotal.VirtualLineTotal_Others_Live,
                grandTotalTextStyle,
                grandTotalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                grandTotal.LineTotal_Others_Non_Live_Str,
                grandTotal.LineTotal_Others_Non_Live,
                grandTotalTextStyle,
                grandTotalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                grandTotal.VirtualLineTotal_Others_Non_Live_Str,
                grandTotal.VirtualLineTotal_Others_Non_Live,
                grandTotalTextStyle,
                grandTotalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                grandTotal.LineTotalReal_Str,
                grandTotal.LineTotalReal,
                grandTotalTextStyle,
                grandTotalDecimalNumberStyle);
            this.RenderSpecialCell(
               ref row,
               ref currentColumn,
               grandTotal.LineTotalVirtual_Str,
               grandTotal.LineTotalVirtual,
               grandTotalTextStyle,
               grandTotalDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                grandTotal.Variance_Str,
                grandTotal.Variance,
                grandTotalTextStyle,
                grandTotalDecimalNumberStyle);
            this.RenderSpecialCell(
               ref row,
               ref currentColumn,
               grandTotal.Turnover_Str,
               grandTotal.Turnover,
               grandTotalTextStyle,
               grandTotalDecimalNumberStyle);
            this.RenderSpecialCell(
               ref row,
               ref currentColumn,
               grandTotal.Margin_Str,
               grandTotal.Margin,
               grandTotalTextStyle,
               grandTotalDecimalNumberStyle);
        }

        private void RenderChildGroupHeader(
            ref ISheet worksheet,
            ref IRow row,
            ref int currentRow,
            ref int currentColumn,
            object cellValue,
            ICellStyle headerStyle)
        {
            for (int i = 0; i < 2; i++)
            {
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, cellValue, headerStyle);
            }
        }

        private void RenderChildGroupHeaderDetail(
            ref ISheet worksheet,
            ref IRow row,
            ref int currentRow,
            ref int currentColumn,
            ICellStyle headerStyle)
        {
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Real, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Virtual, headerStyle);
        }
    }
}