// -----------------------------------------------------------------------
// <copyright file="ReportingServiceQueue.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.ReportingService.Service
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.ReportingService.Common;
    using Fanex.AccountingReports.ReportingService.Criteria.CompanyStatement;
    using Fanex.AccountingReports.ReportingService.Criteria.DailyTurnOver;
    using Fanex.AccountingReports.ReportingService.Criteria.MatchWinLoss2a;
    using Fanex.AccountingReports.ReportingService.Criteria.SuperYesterdayBalance;
    using Fanex.AccountingReports.ReportingService.Enums;
    using Fanex.AccountingReports.ReportingService.Model;
    using Fanex.AccountingReports.ReportingService.Repositories.Implement;
    using Fanex.AccountingReports.ReportingService.Repositories.Interface;
    using Fanex.AccountingReports.ReportingService.Service.Implement.CompanyStatement;
    using Fanex.AccountingReports.ReportingService.Service.Implement.DailyTurnOver;
    using Fanex.AccountingReports.ReportingService.Service.Implement.MatchWinLoss2a;
    using Fanex.AccountingReports.ReportingService.Service.Implement.SuperYesterdayBalance;
    using Fanex.AccountingReports.ReportingService.Service.Interface;
    using Fanex.Common;
    using WebConfiguration = System.Web.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ReportingServiceQueue : BaseService
    {
        private const string MatchWinLoss2aTaskName = "matchwinloss2a";
        private const string DailyTurnOverTaskName = "dailyturnover";
        private const string SuperYesterdayBalanceTaskName = "superyesterdaybalance";
        private const string CompanyStatementTaskName = "companystatement";
        private IReportingServiceRepository _repository = null;
        private IMatchWinLoss2aService _matchWinLoss2aService = null;
        private IDailyTurnoverService _dailyTurnOverService = null;
        private ISuperYesterdayBalanceService _superYesterdayBalanceService = null;
        private ICompanyStatementService _companyStatementService = null;

        public ReportingServiceQueue()
        {
            _repository = new ReportingServiceRepository();
        }

        public bool IsRunning()
        {
            return _repository.IsRunning();
        }

        /// <summary>
        /// Inserts the specified type.
        /// </summary>
        /// <param name="type">The type of task.</param>
        /// <returns>Identity of task.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public int Insert(TaskType type)
        {
            int mainId = 0;
            int detailId = 0;

            if (!AppSettings.OnProcessing)
            {
                AppSettings.OnProcessing = true;
                mainId = _repository.InsertTaskQueue(type);

                if (mainId > 0)
                {
                    bool isSuccess = true;
                    var status = Status.Completed;
                    var tasks = ReportList(mainId);
                    foreach (TaskQueueDetail task in tasks)
                    {
                        detailId = _repository.InsertTaskQueueDetails(task);

                        try
                        {
                            this.ExportReport(task);
                        }
                        catch (Exception ex)
                        {
                            LogExtensions.ExceptionHandler(ex);
                            isSuccess = false;
                        }

                        status = isSuccess ? Status.Completed : Status.Failed;
                        _repository.UpdateStatusTaskQueueDetails(detailId, status);
                    }

                    if (isSuccess)
                    {
                        _repository.UpdateStatusTaskQueue(mainId, Status.Completed);
                    }
                    else
                    {
                        isSuccess = RetryTasksFailed(mainId);
                        status = isSuccess ? Status.Completed : Status.Failed;
                        _repository.UpdateStatusTaskQueue(mainId, status);
                    }

                    //// Delete directory until 3 lasted month.
                    string reportDataDirectory = WebConfiguration.WebConfigurationManager.AppSettings["ExportDataDirectory"];
                    DirectoryExtensions.DeleteDirectory(reportDataDirectory);
                }

                AppSettings.OnProcessing = false;
            }

            return mainId;
        }

        public bool HasTaskAlreadyRunToday()
        {
            return _repository.HasTaskAlreadyRunToday();
        }

        private List<TaskQueueDetail> ReportList(int id)
        {
            List<TaskQueueDetail> list = new List<TaskQueueDetail>();

            list.Add(GetTaskMatchWinLoss2a(id));
            list.Add(GetTaskDailyTurnOver(id));
            list.Add(GetTaskSuperYesterdayBalance(id));
            list.Add(GetTaskCompanyStatement(id));

            return list;
        }

        /// <summary>
        /// Gets the task match win loss2a.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Match win loss 2A task information.</returns>
        private TaskQueueDetail GetTaskMatchWinLoss2a(int id)
        {
            GetMainReportCriteria criteria = new GetMainReportCriteria();
            criteria.Winlostdate = DateTime.Now.AddDays(-1);
            if (AppSettings.ForceRunning)
            {
                criteria.Winlostdate = AppSettings.WinlostDateForceRunning;
            }

            string taskName = MatchWinLoss2aTaskName;
            TaskQueueDetail queue = GetQueueModel(criteria, id, taskName);
            return queue;
        }

        /// <summary>
        /// Gets the task daily turn over.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Daily turn over task information.</returns>
        private TaskQueueDetail GetTaskDailyTurnOver(int id)
        {
            GetDailyTurnoverCriteria criteria = new GetDailyTurnoverCriteria();
            criteria.FromDate = DateTime.Now.AddDays(-1);
            if (AppSettings.ForceRunning)
            {
                criteria.FromDate = AppSettings.WinlostDateForceRunning;
            }

            string taskName = DailyTurnOverTaskName;
            TaskQueueDetail queue = GetQueueModel(criteria, id, taskName);
            return queue;
        }

        /// <summary>
        /// Gets the task super yesterday balance.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Super yesterday balance task information.</returns>
        private TaskQueueDetail GetTaskSuperYesterdayBalance(int id)
        {
            GetSuperYesterdayBalanceCriteria criteria = new GetSuperYesterdayBalanceCriteria();
            string taskName = SuperYesterdayBalanceTaskName;
            TaskQueueDetail queue = GetQueueModel(criteria, id, taskName);
            return queue;
        }

        /// <summary>
        /// Gets the task company statement.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Company statement task information.</returns>
        private TaskQueueDetail GetTaskCompanyStatement(int id)
        {
            GetCompanyStatementCriteria criteria = new GetCompanyStatementCriteria();
            criteria.FromDate = DateTime.Now.AddDays(-1);
            if (AppSettings.ForceRunning)
            {
                criteria.FromDate = AppSettings.WinlostDateForceRunning;
            }

            string taskName = CompanyStatementTaskName;
            TaskQueueDetail queue = GetQueueModel(criteria, id, taskName);
            return queue;
        }

        /// <summary>
        /// Exports the match win loss 2A report.
        /// </summary>
        /// <param name="task">The task information.</param>
        private void ExportMatchWinLoss2aReport(TaskQueueDetail task)
        {
            var getMatchWinLoss2aCriteria = new GetMainReportCriteria();
            getMatchWinLoss2aCriteria.Winlostdate = Converter.ToDateTime(task.Parameters["winlostdate"]);
            getMatchWinLoss2aCriteria.IsGetMainReport = true;
            getMatchWinLoss2aCriteria.IsGetDetailReport = true;
            _matchWinLoss2aService = new MatchWinLoss2aService();
            _matchWinLoss2aService.ExportAllDetailReport(getMatchWinLoss2aCriteria);
        }

        /// <summary>
        /// Exports the daily turn over report.
        /// </summary>
        /// <param name="task">The task information.</param>
        private void ExportDailyTurnOverReport(TaskQueueDetail task)
        {
            var getDailyTurnOverCriteria = new GetDailyTurnoverCriteria();
            getDailyTurnOverCriteria.FromDate = Converter.ToDateTime(task.Parameters["fromdate"]);
            _dailyTurnOverService = new DailyTurnoverService();
            _dailyTurnOverService.ExportReport(getDailyTurnOverCriteria);
        }

        /// <summary>
        /// Exports the super yesterday balance report.
        /// </summary>
        private void ExportSuperYesterdayBalanceReport()
        {
            var getSuperYesterdayBalance = new GetSuperYesterdayBalanceCriteria();
            _superYesterdayBalanceService = new SuperYesterdayBalanceService();
            _superYesterdayBalanceService.ExportSuperYesterdayBalanceReport(getSuperYesterdayBalance);
        }

        /// <summary>
        /// Exports the company statement report.
        /// </summary>
        /// <param name="task">The task information.</param>
        private void ExportCompanyStatementReport(TaskQueueDetail task)
        {
            var getCompanyStatementCriteria = new GetCompanyStatementCriteria();
            getCompanyStatementCriteria.FromDate = Converter.ToDateTime(task.Parameters["fromdate"]);
            _companyStatementService = new CompanyStatementService();
            _companyStatementService.ExportCompanyStatementReport(getCompanyStatementCriteria);
        }

        /// <summary>
        /// Exports the report.
        /// </summary>
        /// <param name="task">The task information.</param>
        private void ExportReport(TaskQueueDetail task)
        {
            switch (task.TaskName.ToLower())
            {
                case MatchWinLoss2aTaskName:
                    this.ExportMatchWinLoss2aReport(task);
                    break;

                case DailyTurnOverTaskName:
                    this.ExportDailyTurnOverReport(task);
                    break;

                case SuperYesterdayBalanceTaskName:
                    this.ExportSuperYesterdayBalanceReport();
                    break;

                case CompanyStatementTaskName:
                    this.ExportCompanyStatementReport(task);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Retries the tasks failed.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task is completed or not.</returns>
        private bool RetryTasksFailed(int id)
        {
            int retry = AppSettings.Retry;
            bool isSuccess = false;
            for (int i = 0; i < retry; i++)
            {
                var tasks = _repository.GetTaskDetailsFailed(id);
                foreach (var task in tasks)
                {
                    try
                    {
                        this.ExportReport(task);
                    }
                    catch (Exception ex)
                    {
                        _repository.UpdateStatusTaskQueueDetails(task.Id, Status.Failed);
                        LogExtensions.ExceptionHandler(ex);
                    }

                    _repository.UpdateStatusTaskQueueDetails(task.Id, Status.Completed);
                    _repository.UpdateRetryTaskQueueDetails(task.Id, i + 1);
                }
            }

            ////check again
            isSuccess = _repository.GetTaskDetailsFailed(id).Count == 0;

            return isSuccess;
        }
    }
}