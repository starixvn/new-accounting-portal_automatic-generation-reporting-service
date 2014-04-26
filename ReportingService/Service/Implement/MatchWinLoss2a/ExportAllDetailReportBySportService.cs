// -----------------------------------------------------------------------
// <copyright file="ExportAllDetailReportBySportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Service.Implement.MatchWinLoss2a
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.ReportingService.Core.Extensions;
    using Fanex.AccountingReports.ReportingService.Core.Services.Impl;
    using Fanex.AccountingReports.ReportingService.Criteria.MatchWinLoss2a;
    using Fanex.AccountingReports.ReportingService.Model.MatchWinLoss2a;
    using Fanex.AccountingReports.ReportingService.Resources;
    using Fanex.AccountingReports.ReportingService.Service.Interface;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;

    /// <summary>
    /// Export all detail report by sport.
    /// </summary>
    public partial class MatchWinLoss2aService : ServiceBase, IMatchWinLoss2aService
    {
        /// <summary>
        /// Exports all detail report by sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="allDetailReport">All detail report.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportAllDetailReportBySport(
            GetMainReportCriteria criteria,
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            WinLossInfo allDetailReport)
        {
            this.SetColorIndex(excelFile);
            IDataFormat dataFormat = excelFile.CreateDataFormat();
            ICellStyle ringgitMalaysiaTextStyle = NpoiExcelExtensions.TextStyle(excelFile, SubTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
            ICellStyle ringgitMalaysiaDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, SubTotalColorIndex, true);
            ringgitMalaysiaDecimalNumberStyle.DataFormat = dataFormat.GetFormat(NegativeNumberFormat);
            ICellStyle grandTotalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
            grandTotalTextStyle.BorderTop = BorderStyle.Medium;
            grandTotalTextStyle.TopBorderColor = GrandTotalBorderColorIndex;
            ICellStyle grandTotalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, GrandTotalColorIndex, true);
            grandTotalDecimalNumberStyle.BorderTop = BorderStyle.Medium;
            grandTotalDecimalNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;
            grandTotalDecimalNumberStyle.DataFormat = dataFormat.GetFormat(NegativeNumberFormat);
            ICellStyle emptyDataTextStyle = NpoiExcelExtensions.TextStyle(
                excelFile,
                HSSFColor.White.Index,
                false,
                NpoiExcelExtensions.Alignment.Center,
                HSSFColor.Red.Index);

            this.ExportAllDetailReportBySportHeader(excelFile, ref worksheet, ref currentRow);

            currentRow++;

            if (allDetailReport != null)
            {
                WinLossInfo winLossInfo = null;
                winLossInfo = allDetailReport.NonThirdPartyInfo;

                if (allDetailReport.NonThirdPartyInfo.ChildsInfo.Count > 0)
                {
                    this.ExportAll_NonThirdPartyDetailReport(excelFile, ref worksheet, ref currentRow, winLossInfo);
                    currentRow++;
                }

                winLossInfo = allDetailReport.EightRMTotal;
                this.ExportRow_AllDetailReport(
                    ref worksheet,
                    ref currentRow,
                    ringgitMalaysiaTextStyle,
                    ringgitMalaysiaDecimalNumberStyle,
                    ReportRowType.EightRM,
                    winLossInfo.Name,
                    winLossInfo);

                currentRow++;
                winLossInfo = allDetailReport.BARMTotal;
                this.ExportRow_AllDetailReport(
                    ref worksheet,
                    ref currentRow,
                    ringgitMalaysiaTextStyle,
                    ringgitMalaysiaDecimalNumberStyle,
                    ReportRowType.BARM,
                    winLossInfo.Name,
                    winLossInfo);

                currentRow++;
                if (allDetailReport.ThirdPartyInfo.ChildsInfo.Count > 0)
                {
                    winLossInfo = allDetailReport.ThirdPartyInfo;
                    this.ExportAll_ThirdPartyDetailReport(
                        excelFile,
                        ref worksheet,
                        ref currentRow,
                        winLossInfo);
                }

                winLossInfo = allDetailReport.GrandTotal;
                this.ExportRow_AllDetailReport(
                    ref worksheet,
                    ref currentRow,
                    grandTotalTextStyle,
                    grandTotalDecimalNumberStyle,
                    ReportRowType.GrandTotal,
                    winLossInfo.Name,
                    winLossInfo);
            }
            else
            {
                this.ExportAllDetailReportBySportTypeNoData(ref worksheet, ref currentRow, emptyDataTextStyle);
            }
        }

        /// <summary>
        /// Exports all detail report by sport type_ no data.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="textStyle">The text style.</param>
        private void ExportAllDetailReportBySportTypeNoData(
            ref ISheet worksheet,
            ref int currentRow,
            ICellStyle textStyle)
        {
            IRow row = worksheet.CreateRow(currentRow);
            var currentColumn = 0;
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.There_is_no_data, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.There_is_no_data, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.There_is_no_data, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.There_is_no_data, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.There_is_no_data, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.There_is_no_data, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.There_is_no_data, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.There_is_no_data, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.There_is_no_data, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.There_is_no_data, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.There_is_no_data, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.There_is_no_data, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.There_is_no_data, textStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.There_is_no_data, textStyle);
            var startMergeColumn = currentColumn - 14;
            this.RenderMergeCell(
                ref worksheet,
                ref row,
                currentRow,
                ref startMergeColumn,
                currentRow,
                currentColumn - 1,
                Labels.There_is_no_data,
                textStyle);
        }

        /// <summary>
        /// Exports all detail report by sport header.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        private void ExportAllDetailReportBySportHeader(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow)
        {
            IRow row = worksheet.CreateRow(currentRow);

            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
            var currentColumn = 0;

            for (var i = 0; i < 14; i++)
            {
                worksheet.SetColumnWidth(i, 140 * NpoiExcelExtensions.FontWidth);
            }

            worksheet.SetColumnWidth(0, 200 * NpoiExcelExtensions.FontWidth);
            worksheet.SetColumnWidth(1, 200 * NpoiExcelExtensions.FontWidth);

            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Bet_Type, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Date, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Match, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Match, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Match, headerStyle);
            var startMergeColumn = currentColumn - 3;
            this.RenderMergeCell(ref worksheet, ref row, currentRow, ref startMergeColumn, currentRow, currentColumn - 1, Labels.Match, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Win_Loss, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Position, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Comm, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Virtual_Comm, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Line_Total, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Virtual_Line_Total, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Variance, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Turnover, headerStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, Labels.Margin_Percent, headerStyle);
        }

        /// <summary>
        /// Exports the all_ non third party detail report.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="nonThirdPartyReport">The non third party report.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportAll_NonThirdPartyDetailReport(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            WinLossInfo nonThirdPartyReport)
        {
            IDataFormat dataFormat = excelFile.CreateDataFormat();
            ICellStyle totalRowTextStyle = NpoiExcelExtensions.TextStyle(excelFile, TotalRowColorIndex, true, NpoiExcelExtensions.Alignment.Right);
            ICellStyle totalRowDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, TotalRowColorIndex, true);
            totalRowDecimalNumberStyle.DataFormat = dataFormat.GetFormat(NegativeNumberFormat);
            ICellStyle subTotalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, SubTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
            ICellStyle subTotalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, SubTotalColorIndex, true);
            subTotalDecimalNumberStyle.DataFormat = dataFormat.GetFormat(NegativeNumberFormat);
            ICellStyle grandTotalBySportTextStyle = NpoiExcelExtensions.TextStyle(excelFile, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
            ICellStyle grandTotalBySportDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, GrandTotalColorIndex, true);
            grandTotalBySportDecimalNumberStyle.DataFormat = dataFormat.GetFormat(NegativeNumberFormat);
            ReportCellStyle reportCellStyle = new ReportCellStyle(ref excelFile);

            foreach (var sportType in nonThirdPartyReport.ChildsInfo)
            {
                foreach (var betType in sportType.ChildsInfo)
                {
                    foreach (var winLossDetailByBetType in betType.ChildsInfo)
                    {
                        this.ExportNoneThirdPartyDetailReport_Row(
                            ref worksheet,
                            ref currentRow,
                            winLossDetailByBetType,
                            reportCellStyle);

                        currentRow++;
                    }

                    string betTypeTotalTitle = string.Format("{0} {1} {2}", betType.LiveIndicatorId ? Labels.Live : string.Empty, betType.Name, Labels.Total);

                    this.ExportRow_AllDetailReport(
                        ref worksheet,
                        ref currentRow,
                        totalRowTextStyle,
                        totalRowDecimalNumberStyle,
                        ReportRowType.Total,
                        betTypeTotalTitle.ToUpper(),
                        betType.GrandTotal);

                    currentRow++;
                }

                this.ExportRow_AllDetailReport(
                        ref worksheet,
                        ref currentRow,
                        subTotalTextStyle,
                        subTotalDecimalNumberStyle,
                        ReportRowType.SubTotal,
                        (sportType.Name + " " + Labels.Total).ToUpper(),
                        sportType.GrandTotal);

                currentRow++;
            }

            this.ExportRow_AllDetailReport(
                        ref worksheet,
                        ref currentRow,
                        grandTotalBySportTextStyle,
                        grandTotalBySportDecimalNumberStyle,
                        ReportRowType.GrandTotal,
                        nonThirdPartyReport.GrandTotal.Name,
                        nonThirdPartyReport.GrandTotal);
        }

        /// <summary>
        /// Exports the all_ third party detail report.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="thirdPartyReport">The third party report.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportAll_ThirdPartyDetailReport(
            HSSFWorkbook excelFile,
            ref ISheet worksheet,
            ref int currentRow,
            WinLossInfo thirdPartyReport)
        {
            IDataFormat dataFormat = excelFile.CreateDataFormat();
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Left);
            ICellStyle decimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, false);
            decimalNumberStyle.DataFormat = dataFormat.GetFormat(NegativeNumberFormat);

            ICellStyle boldTextStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, true, NpoiExcelExtensions.Alignment.Left);
            ICellStyle boldDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, true);
            boldDecimalNumberStyle.DataFormat = dataFormat.GetFormat(NegativeNumberFormat);

            foreach (var sportType in thirdPartyReport.ChildsInfo)
            {
                IRow row = worksheet.CreateRow(currentRow);
                var currentColumn = 0;
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, sportType.Name, textStyle);
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, sportType.EventDate, textStyle);
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, string.Empty, textStyle);
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, string.Empty, textStyle);
                this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, string.Empty, textStyle);
                var startMergeColumn = currentColumn - 3;
                this.RenderMergeCell(ref worksheet, ref row, currentRow, ref startMergeColumn, currentRow, currentColumn - 1, string.Empty, textStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    sportType.WinLoss_Str,
                    sportType.WinLoss,
                    textStyle,
                    decimalNumberStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    sportType.Position_Str,
                    sportType.Position,
                    textStyle,
                    decimalNumberStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    sportType.Commission_Str,
                    sportType.Commission,
                    textStyle,
                    decimalNumberStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    sportType.VirtualComm_Str,
                    sportType.VirtualComm,
                    textStyle,
                    decimalNumberStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    sportType.LineTotal_Str,
                    sportType.LineTotal,
                    boldTextStyle,
                    boldDecimalNumberStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    sportType.VirtualLineTotal_Str,
                    sportType.VirtualLineTotal,
                    boldTextStyle,
                    boldDecimalNumberStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    sportType.Variance_Str,
                    sportType.Variance,
                    textStyle,
                    decimalNumberStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    sportType.Turnover_Str,
                    sportType.Turnover,
                    textStyle,
                    decimalNumberStyle);
                this.RenderSpecialCell(
                    ref row,
                    ref currentColumn,
                    sportType.Margin_Str,
                    sportType.Margin,
                    textStyle,
                    decimalNumberStyle);

                currentRow++;
            }
        }

        /// <summary>
        /// Exports the none third party detail report_ row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="winLossInfo">The win loss information.</param>
        /// <param name="reportCellStyle">The report cell style.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportNoneThirdPartyDetailReport_Row(
            ref ISheet worksheet,
            ref int currentRow,
            WinLossInfo winLossInfo,
            ReportCellStyle reportCellStyle)
        {
            IRow row = worksheet.CreateRow(currentRow);
            var currentColumn = 0;
            string title = (winLossInfo.LiveIndicatorId ? "!" + Labels.Live + " - " : string.Empty) + winLossInfo.Name + " " + winLossInfo.MatchCode;
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, title, reportCellStyle.TextStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, winLossInfo.EventDate, reportCellStyle.TextStyle);
            string matchInfo = winLossInfo.IsNeutral ?
                 string.Format("{0} {1}-vs-{2}", winLossInfo.HomeTeamName, Labels.IsNeutral, winLossInfo.AwayTeamName) :
                 string.Format("{0}-vs-{1}", winLossInfo.HomeTeamName, winLossInfo.AwayTeamName);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, matchInfo, reportCellStyle.TextStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, matchInfo, reportCellStyle.TextStyle);
            this.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, matchInfo, reportCellStyle.TextStyle);
            var startMergeColumn = currentColumn - 3;
            this.RenderMergeCell(ref worksheet, ref row, currentRow, ref startMergeColumn, currentRow, currentColumn - 1, matchInfo, reportCellStyle.CenterTextStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.WinLoss_Str,
                winLossInfo.WinLoss,
                reportCellStyle.RightTextStyle,
                reportCellStyle.DecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Position_Str,
                winLossInfo.Position,
                 reportCellStyle.RightTextStyle,
                reportCellStyle.DecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Commission_Str,
                winLossInfo.Commission,
                 reportCellStyle.RightTextStyle,
                reportCellStyle.DecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.VirtualComm_Str,
                winLossInfo.VirtualComm,
                 reportCellStyle.RightTextStyle,
                reportCellStyle.DecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.LineTotal_Str,
                winLossInfo.LineTotal,
                reportCellStyle.BoldTextStyle,
                reportCellStyle.BoldDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.VirtualLineTotal_Str,
                winLossInfo.VirtualLineTotal,
                reportCellStyle.BoldTextStyle,
                reportCellStyle.BoldDecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Variance_Str,
                winLossInfo.Variance,
                 reportCellStyle.RightTextStyle,
                reportCellStyle.DecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Turnover_Str,
                winLossInfo.Turnover,
                 reportCellStyle.RightTextStyle,
                reportCellStyle.DecimalNumberStyle);
            this.RenderSpecialCell(
                ref row,
                ref currentColumn,
                winLossInfo.Margin_Str,
                winLossInfo.Margin,
                reportCellStyle.RightTextStyle,
                reportCellStyle.DecimalNumberStyle);
        }
    }
}