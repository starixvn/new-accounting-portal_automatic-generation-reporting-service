// -----------------------------------------------------------------------
// <copyright file="SqLite.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2014.04.25">Created</change>
// </history>
namespace Fanex.AccountingReports.SqLiteDatabase
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;

    /// <summary>
    /// SQLITE Database.
    /// </summary>
    public class SqLite
    {
        /// <summary>
        /// The connection string.
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqLite" /> class.
        /// </summary>
        /// <param name="connection">Connection string.</param>
        public SqLite(string connection)
        {
            this._connectionString = connection;
        }

        /// <summary>
        /// Allows the programmer to run a query against the Database.
        /// </summary>
        /// <param name="sql">The SQL to run.</param>
        /// <returns>A DataTable containing the result set.</returns>
        public DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                SQLiteConnection cnn = new SQLiteConnection(this._connectionString);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = sql;
                SQLiteDataReader reader = mycommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                cnn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
        }

        /// <summary>
        /// Allows the programmer to interact with the database for purposes other than a query.
        /// </summary>
        /// <param name="sql">The SQL to be run.</param>
        /// <returns>An Integer containing the number of rows updated.</returns>
        public int ExecuteNonQuery(string sql)
        {
            int rowsUpdated = 0;
            try
            {
                SQLiteConnection cnn = new SQLiteConnection(this._connectionString);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = sql;
                rowsUpdated = mycommand.ExecuteNonQuery();
                cnn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

            return rowsUpdated;
        }

        /// <summary>
        /// Allows the programmer to interact with the database for purposes other than a query.
        /// </summary>
        /// <param name="sql">The SQL to be run.</param>
        /// <returns>An Integer containing the number of rows updated.</returns>
        public long ExecuteScalar(string sql)
        {
            long rowsUpdated = 0;
            try
            {
                SQLiteConnection cnn = new SQLiteConnection(this._connectionString);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = sql;
                rowsUpdated = (long)mycommand.ExecuteScalar();
                cnn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

            return rowsUpdated;
        }

        /// <summary>
        /// Allows the programmer to easily update rows in the DB.
        /// </summary>
        /// <param name="tableName">The table to update.</param>
        /// <param name="data">A dictionary containing Column names and their new values.</param>
        /// <param name="where">The where clause for the update statement.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Update(string tableName, Dictionary<string, string> data, string where)
        {
            string vals = string.Empty;
            bool isSuccess = true;
            if (data.Count >= 1)
            {
                foreach (KeyValuePair<string, string> val in data)
                {
                    vals += string.Format(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString());
                }

                vals = vals.Substring(0, vals.Length - 1);
                try
                {
                    this.ExecuteNonQuery(string.Format("update {0} set {1} where {2};", tableName, vals, where));
                }
                catch
                {
                    isSuccess = false;
                }
            }

            return isSuccess;
        }

        /// <summary>
        /// Allows the programmer to easily delete rows from the DB.
        /// </summary>
        /// <param name="tableName">The table from which to delete.</param>
        /// <param name="where">The where clause for the delete.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Delete(string tableName, string where)
        {
            bool isSuccess = true;
            try
            {
                this.ExecuteNonQuery(string.Format("delete from {0} where {1};", tableName, where));
            }
            catch (Exception fail)
            {
                isSuccess = false;
                throw fail;
            }

            return isSuccess;
        }

        /// <summary>
        /// Allows the programmer to easily insert into the DB.
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="data">A dictionary containing the column names and data for the insert.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public long Insert(string tableName, Dictionary<string, string> data)
        {
            string columns = string.Empty;
            string values = string.Empty;
            long lastId = -1;
            if (data.Count == 0)
            {
                return lastId;
            }

            foreach (KeyValuePair<string, string> val in data)
            {
                columns += string.Format(" {0},", val.Key.ToString());
                values += string.Format(" '{0}',", val.Value);
            }

            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            try
            {
                this.ExecuteNonQuery(string.Format("insert into {0}({1}) values({2});", tableName, columns, values));

                // Get lastest inserted row id
                string sql = @"select max(ID) from " + tableName;
                lastId = (long)this.ExecuteScalar(sql);
            }
            catch (Exception e)
            {
                throw e;
            }

            return lastId;
        }

        /// <summary>
        /// Allows the programmer to easily insert into the DB.
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="columnsName">List columns which we insert the data.</param>
        /// <param name="data">Data table containing the column names and data for the insert.</param>
        /// <returns>true: Success; false: Failed.</returns>
        public bool Insert(
            string tableName,
            List<string> columnsName,
            DataTable data)
        {
            bool isSuccess = true;
            try
            {
                SQLiteConnection conn = new SQLiteConnection(this._connectionString);
                conn.Open();

                using (SQLiteTransaction mytransaction = conn.BeginTransaction())
                {
                    using (SQLiteCommand mycommand = new SQLiteCommand(conn))
                    {
                        List<string> values = new List<string>();

                        List<SQLiteParameter> param = new List<SQLiteParameter>();
                        for (int i = 0; i < columnsName.Count; i++)
                        {
                            SQLiteParameter p = new SQLiteParameter();
                            mycommand.Parameters.Add(p);

                            values.Add("?");
                        }

                        mycommand.CommandText = string.Format(
                            "INSERT INTO [{0}] ({1}) VALUES({2})",
                            tableName,
                            string.Join(", ", columnsName),
                            string.Join(", ", values));
                        var rows = data.Rows;
                        foreach (DataRow row in rows)
                        {
                            for (int i = 0; i < columnsName.Count; i++)
                            {
                                mycommand.Parameters[i].Value = row[columnsName[i]];
                            }

                            mycommand.ExecuteNonQuery();
                        }
                    }

                    mytransaction.Commit();
                }
            }
            catch (Exception e)
            {
                isSuccess = false;
                throw e;
            }

            return isSuccess;
        }

        /// <summary>
        /// Allows the user to easily clear all data from a specific table.
        /// </summary>
        /// <param name="table">The name of the table to clear.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearTable(string table)
        {
            try
            {
                this.ExecuteNonQuery(string.Format("delete from {0};", table));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
