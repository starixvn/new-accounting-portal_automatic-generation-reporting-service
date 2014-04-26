// -----------------------------------------------------------------------
// <copyright file="BaseService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.ReportingService.Service
{
    using System;
    using Fanex.AccountingReports.ReportingService.Enums;
    using Fanex.AccountingReports.ReportingService.Model;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BaseService
    {
        /// <summary>
        /// Gets the queue model.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="taskName">Name of the task.</param>
        /// <returns>Queue model.</returns>
        protected TaskQueueDetail GetQueueModel(CriteriaBase criteria, int id, string taskName)
        {
            TaskQueueDetail model = new TaskQueueDetail();
            model.Id = id;
            model.TaskName = taskName;
            model.Parameters = criteria.GetSpParams();
            model.SPName = criteria.GetSettingKey();
            model.Retry = 0;
            model.Status = (int)Status.Running;
            model.CreatedDate = DateTime.Now.ToString();

            return model;
        }
    }
}