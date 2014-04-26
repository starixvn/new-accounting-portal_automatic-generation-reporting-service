// --------------------------------------------------------------------------
// <copyright file="BaseRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.22">Created</change>
//   <change who="Ricky.Nguyen" date="2012.12.11">Update Get Data Function</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Core.Repository.Impl
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.Data;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    /// <typeparam name="T">The parameter type.</typeparam>
    public class BaseRepository<T> : ReadonlyRepository<T>
    {
        /// <summary>
        /// Fetches the specified criteria.
        /// </summary>
        /// <typeparam name="S">The parameter type.</typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <param name="mapObjectFunction">The map object function.</param>
        /// <returns>The data value.</returns>
        public virtual List<S> Fetch<S>(
            ICriteria criteria,
            Func<IDataReader, S> mapObjectFunction) where S : class
        {
            List<S> listObject = new List<S>();
            if (!criteria.IsValid())
            {
                return listObject;
            }

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    while (reader.Read())
                    {
                        listObject.Add(mapObjectFunction(reader));
                    }
                }
            }

            return listObject;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The data value.</returns>
        public virtual Hashtable ExecuteCommand(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
                return db.GetOuputParams();
            }
        }

        /// <summary>
        /// Fetches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParams">The output parameters.</param>
        /// <returns>The data value.</returns>
        public virtual DataTable Fetch(ICriteria criteria, ref Hashtable outputParams)
        {
            if (!criteria.IsValid())
            {
                return null;
            }

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                DataSet dataSet = db.ExecuteDataSet(criteria.GetSpParams());
                if (dataSet != null &&
                    dataSet.Tables.Count > 0)
                {
                    outputParams = db.GetOuputParams();
                    return dataSet.Tables[0];
                }
            }

            return null;
        }

        /// <summary>
        /// Fetches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The data value.</returns>
        public new virtual DataTable Fetch(ICriteria criteria)
        {
            if (!criteria.IsValid())
            {
                return null;
            }

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                DataSet dataSet = db.ExecuteDataSet(criteria.GetSpParams());
                if (dataSet != null &&
                    dataSet.Tables.Count > 0)
                {
                    return dataSet.Tables[0];
                }
            }

            return null;
        }

        /// <summary>
        /// Fetches the specified criteria.
        /// </summary>
        /// <typeparam name="S">The parameter type.</typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <param name="mapObjectFunction">The map object function.</param>
        /// <returns>The data value.</returns>
        public virtual IList<S> Fetch<S>(ICriteria criteria, Func<DataRow, S> mapObjectFunction) where S : class
        {
            IList<S> listData = new List<S>();
            if (!criteria.IsValid())
            {
                return listData;
            }

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                DataSet dataSet = db.ExecuteDataSet(criteria.GetSpParams());
                if (dataSet != null &&
                    dataSet.Tables.Count > 0)
                {
                    DataTable currentTable = dataSet.Tables[0];
                    foreach (DataRow row in currentTable.Rows)
                    {
                        listData.Add(mapObjectFunction(row));
                    }
                }
            }

            return listData;
        }

        /// <summary>
        /// Fetches the specified criteria.
        /// </summary>
        /// <typeparam name="S">The parameter type.</typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <param name="mapObjectFunction">The map object function.</param>
        /// <param name="outputParams">The output parameters.</param>
        /// <returns>The data value.</returns>
        public virtual IList<S> Fetch<S>(ICriteria criteria, Func<DataRow, S> mapObjectFunction, ref Hashtable outputParams) where S : class
        {
            IList<S> listData = new List<S>();
            if (!criteria.IsValid())
            {
                return listData;
            }

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                DataSet dataSet = db.ExecuteDataSet(criteria.GetSpParams());
                outputParams = db.GetOuputParams();
                if (dataSet != null &&
                    dataSet.Tables.Count > 0)
                {
                    DataTable currentTable = dataSet.Tables[0];
                    foreach (DataRow row in currentTable.Rows)
                    {
                        listData.Add(mapObjectFunction(row));
                    }
                }
            }

            return listData;
        }
    }
}