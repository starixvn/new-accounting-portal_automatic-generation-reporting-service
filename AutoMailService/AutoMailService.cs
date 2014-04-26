// -----------------------------------------------------------------------
// <copyright file="AutoMailService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.21">Created</change>
// </history>

namespace AutoMailService
{
    using System;
    using System.IO;
    using System.ServiceProcess;
    using System.Timers;
    using Fanex.Data.Configuration;
    using WinLossByProducts;
    using WinLossByProducts.Common;
    using WinLossByProducts.Service;

    /// <summary>
    /// Auto mail service.
    /// </summary>
    public partial class AutoMailService : ServiceBase
    {
        /// <summary>
        /// The timer.
        /// </summary>
        private static Timer timer;

        private static WinLossByProductsService service;

        public AutoMailService()
        {
            InitializeComponent(AppSettings.ServiceName);
        }

        /// <summary>
        /// On start service.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            DbSettingProviderManager.Current.Start();
            timer = new Timer();
            timer.Interval = AppSettings.Interval;
            timer.Elapsed += new ElapsedEventHandler(OnInterval);
            timer.AutoReset = true;
            timer.Enabled = true;

            AppSettings.OnProcessing = false;
            AppSettings.ForceRunningOnProcessing = false;

            service = new WinLossByProductsService();
        }

        protected override void OnStop()
        {
            timer.Stop();
            timer = null;
            service = null;

            AppSettings.OnProcessing = false;
            AppSettings.ForceRunningOnProcessing = false;

            GC.Collect();
        }

        /// <summary>
        /// Called when [interval].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private static void OnInterval(object source, ElapsedEventArgs e)
        {
            try
            {
                TimeSpan executeTime = new TimeSpan(AppSettings.RunAtHours, AppSettings.RunAtMinutes, 0);
                var currentTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);

                int compareResult = TimeSpan.Compare(currentTime, executeTime);
                if (compareResult >= 0)
                {
                    service.OnRunningDaily();
                }

                if (!AppSettings.ForceRunningOnProcessing && AppSettings.ForceRunning)
                {
                    AppSettings.ForceRunningOnProcessing = true;
                    DbSettingProviderManager.Current.Restart();

                    service.OnRunningUrgently();
                }
            }
            catch (Exception ex)
            {
                LogExtensions.ExceptionHandler(ex);
            }
        }
    }
}