// -----------------------------------------------------------------------
// <copyright file="CompanyStatementService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.08">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Service.Implement.CompanyStatement
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Fanex.AccountingReports.ReportingService.Common;
    using Fanex.AccountingReports.ReportingService.Core.Extensions;
    using Fanex.AccountingReports.ReportingService.Core.Services.Impl;
    using Fanex.AccountingReports.ReportingService.Criteria.CompanyStatement;
    using Fanex.AccountingReports.ReportingService.Model.CompanyStatement;
    using Fanex.AccountingReports.ReportingService.Resources;
    using Fanex.AccountingReports.ReportingService.Service.Interface;
    using Fanex.Common;
    using Fanex.Data;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using WebConfigurationManager = System.Web.Configuration;

    /// <summary>
    /// Company statement service.
    /// </summary>
    public class CompanyStatementService : ServiceBase, ICompanyStatementService
    {
        private const short EvenRowColorIndex = 29;

        /// <summary>
        /// Gets the company statement.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Company statement data.</returns>
        public CompanyStatementInfo GetCompanyStatement(GetCompanyStatementCriteria criteria)
        {
            CompanyStatementInfo result = new CompanyStatementInfo();
            criteria.FromDate = criteria.FromDate.Date;

            try
            {
                using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
                {
                    var reader = objectDb.ExecuteReader(criteria.GetSpParams());
                    decimal accumulatedTotal = 0;

                    while (reader.Read())
                    {
                        var total = Converter.ToDecimal(reader["totalamt"]);
                        accumulatedTotal += total;

                        var companyStatementInfo = new CompanyStatementInfo
                        {
                            Date = Converter.ToDateTime(reader["accdate"]).ToShortDateString(),
                            Type = Converter.ToString2(reader["transtype"]),
                            Total = total,
                            AccumulatedTotal = accumulatedTotal
                        };

                        result.ReportRows.Add(companyStatementInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return result;
        }

        /// <summary>
        /// Exports the company statement report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportCompanyStatementReport(GetCompanyStatementCriteria criteria)
        {
            FileStream fileStream = null;

            try
            {
                CompanyStatementInfo report = this.GetCompanyStatement(criteria);

                HSSFWorkbook excelFile = new HSSFWorkbook();
                this.SetColorAtIndex(excelFile);
                string sheetName = ReportingServiceResources.CompanyStatement_SheetName;
                ISheet worksheet = excelFile.CreateSheet(sheetName);
                int currentRow = 0;

                this.ExportReportHeader(excelFile, ref worksheet, ref currentRow, criteria);

                if (report != null && report.ReportRows.Count > 0)
                {
                    this.ExportReportContent(excelFile, ref worksheet, ref currentRow, report);
                }
                else
                {
                    this.ExportNoData(excelFile, ref worksheet, ref currentRow);
                }

                var reportDirectory = this.CreateCompanyStatementReportDirectory(criteria);
                var reportName = string.Format(
                ReportingServiceResources.CompanyStatement_ReportName,
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
                HandleException(ex);
            }
            finally
            {
                fileStream.Close();
                fileStream.Dispose();
            }
        }

        /// <summary>
        /// Exports the report header.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        private void ExportReportHeader(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, GetCompanyStatementCriteria criteria)
        {
            int currentColumn = 0;
            IRow row = worksheet.CreateRow(currentRow);
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);

            for (int i = 0; i < 4; i++)
            {
                worksheet.SetColumnWidth(i, 170 * NpoiExcelExtensions.FontWidth);
            }

            string reportTitle = string.Format(
                    ReportingServiceResources.CompanyStatement_ReportTitle,
                    criteria.FromDate.ToShortFormatedDate());
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + 3, reportTitle, headerStyle);
            currentRow += 2;

            row = worksheet.CreateRow(currentRow);
            currentColumn = 0;
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, "Date", headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, "Type", headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, "Total", headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, "Accummulated Total", headerStyle);
            currentRow++;
        }

        /// <summary>
        /// Exports the no data.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="currentRow">The current row.</param>
        private void ExportNoData(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow)
        {
            int currentColumn = 0;
            IRow row = worksheet.CreateRow(currentRow);
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Left);
            NpoiExcelExtensions.RenderCell(
                ref worksheet,
                ref row,
                currentRow,
                ref currentColumn,
                currentRow,
                currentColumn + 3,
                Labels.There_is_no_data,
                textStyle);
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
        private void ExportReportContent(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, CompanyStatementInfo report)
        {
            int currentColumn = 0;
            IRow row = null;
            ICellStyle textStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle evenRowTextStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle decimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, false);
            ICellStyle evenRowDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, false);

            for (int i = 0; i < report.ReportRows.Count; i++)
            {
                var currentReportRow = report.ReportRows[i];
                row = worksheet.CreateRow(currentRow);
                currentColumn = 0;
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    currentReportRow.Date,
                    i % 2 == 0 ? evenRowTextStyle : textStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    currentReportRow.Type,
                    i % 2 == 0 ? evenRowTextStyle : textStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    currentReportRow.Total,
                    i % 2 == 0 ? evenRowDecimalNumberStyle : decimalNumberStyle);
                NpoiExcelExtensions.RenderCell(
                    ref worksheet,
                    ref row,
                    currentRow,
                    ref currentColumn,
                    currentRow,
                    currentColumn,
                    currentReportRow.AccumulatedTotal,
                    i % 2 == 0 ? evenRowDecimalNumberStyle : decimalNumberStyle);
                currentRow++;
            }
        }

        /// <summary>
        /// Creates the company statement report directory.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Company statement report directory.</returns>
        private string CreateCompanyStatementReportDirectory(GetCompanyStatementCriteria criteria)
        {
            string reportDataDirectory = WebConfigurationManager.WebConfigurationManager.AppSettings["ExportDataDirectory"];
            DirectoryExtensions.CheckAndCreateDirectory(reportDataDirectory);

            string directoryByYearAndMonth = string.Format(
                "{0}{1}",
                criteria.FromDate.ToString("yyyy"),
                criteria.FromDate.ToString("MM"));
            string directoryByYearAndMonthPath = Path.Combine(reportDataDirectory, directoryByYearAndMonth);
            DirectoryExtensions.CheckAndCreateDirectory(directoryByYearAndMonthPath);

            string directoryByReport = "CompanyStatement";
            string directoryByReportPath = Path.Combine(directoryByYearAndMonthPath, directoryByReport);
            DirectoryExtensions.CheckAndCreateDirectory(directoryByReportPath);

            return directoryByReportPath;
        }

        private void SetColorAtIndex(HSSFWorkbook excelFile)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            palette.SetColorAtIndex(EvenRowColorIndex, (byte)228, (byte)242, (byte)253);
        }
    }
}