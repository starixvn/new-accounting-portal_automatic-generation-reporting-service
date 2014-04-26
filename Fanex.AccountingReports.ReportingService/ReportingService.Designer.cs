// -----------------------------------------------------------------------
// <copyright file="ReportingService.Designer.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
namespace Fanex.AccountingReports.ReportingService
{
    /// <summary>
    /// Reporting service.
    /// </summary>
    public partial class ReportingService
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer _components = null;

        /// <summary>
        /// The event log 1.
        /// </summary>
        private System.Diagnostics.EventLog _eventLog1;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this._components != null))
            {
                this._components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent(string serviceName)
        {
            this._eventLog1 = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this._eventLog1)).BeginInit();
            // 
            // Nozomi
            // 
            this.ServiceName = serviceName;
            ((System.ComponentModel.ISupportInitialize)(this._eventLog1)).EndInit();

        }

        #endregion
    }
}
