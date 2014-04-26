// -----------------------------------------------------------------------
// <copyright file="TaskQueueDetail.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.ReportingService.Model
{
    using System.Collections;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TaskQueueDetail
    {
        public int Id { get; set; }

        public string TaskName { get; set; }

        public string SPName { get; set; }

        public int Status { get; set; }

        public int Retry { get; set; }

        public string CreatedDate { get; set; }

        public Hashtable Parameters { get; set; }
    }
}