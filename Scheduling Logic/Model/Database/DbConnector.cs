﻿using MySql.Data.MySqlClient;
using Scheduling_Logic.Model.Config;
using Scheduling_Logic.Model.Data;
using Scheduling_Logic.Model.Factory;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Scheduling_Logic.Model.Database
{
    /*
     * Description: This class is a wrapper class that handles different objects to succesfully fetch, retrieve
     *              and update data from and to the database.
     *              It expects to implement the [IDatabaseConnector] properties and behaviors.
     */
    public sealed class DbConnector
    {
        private readonly DbProviderFactory dbProviderFactory;
        private readonly DbConnection dbConnection;
        private readonly DbDataAdapter dbDataAdapter;
        private bool disposedValue;

        /*
         * Description: Parameterized Constructor
         * 
         * @param       [String] connectionString   A connection string configuration for the specific
         *                                          database type.
         *                                          
         * mutation    It initialize this.dbConnection object based on the connection type.                                         
         */
        internal DbConnector(IDbConfig config)
        {
            // Validate for nulls. (params)
            ValidateForNullParamater(config, nameof(config));

            this.dbProviderFactory = DbFactory.CreateDbProviderFactory(config)!;
            ValidateForNullClassVariable(this.dbProviderFactory, nameof(this.dbProviderFactory));

            this.dbConnection = dbProviderFactory.CreateConnection()!;
            ValidateForNullClassVariable(this.dbConnection, nameof(this.dbConnection));

            this.dbDataAdapter = dbProviderFactory.CreateDataAdapter()!;
            ValidateForNullClassVariable(this.dbDataAdapter, nameof(this.dbDataAdapter));

            // Establish connection and data adapater
            ConfigureInteralConnection(config);
            ConfigureInteralDbDataAdapter();
        }

        /*
         * Description: It attempts to open the connection if the object is not null and if the connection is not open already.
         */
        public void OpenConnection()
        {
            if (!dbConnection.State.Equals(ConnectionState.Open))
            {
                this.dbConnection.Open(); // can throw DbException
                // 'Unable to connect to any of the specficied <ServiceName> host.'
            }
        }

        /*
        * Description: It closes the connection.
        */
        public void CloseConnection()
        {
            this.dbConnection.Close();
            /* ChangedStateEventHandler?.Invoke(this, new DbConnectionEventArgs(_conn));*/
        }

        public ConnectionState ConnState
        {
            get => this.dbConnection.State;
        }

        public bool IsConnOpen()
        {
            return this.dbConnection.State == ConnectionState.Open;
        }

        // https://learn.microsoft.com/en-us/dotnet/api/system.data.dataset?view=net-7.0
        public void MapTableAndColumns(string dbName, string tableName)
        {
            // Validate Database Conn
            if (!this.IsConnOpen())
            {
                this.OpenConnection();
            }

            // DataAdapter
            ITableMapping tableMapping = this.dbDataAdapter.TableMappings.Add(tableName, tableName);

            // Database
            this.dbDataAdapter.SelectCommand!.CommandText = $"SELECT * FROM `{dbName}`.`{tableName}`;"; // can throw DbException
            IDataReader dataReader = dbDataAdapter.SelectCommand.ExecuteReader();   // can throw InvalidArgumentException

            // DataAdapter
            for (var fieldIdx = 0; fieldIdx < dataReader.FieldCount; ++fieldIdx)
            {
                tableMapping.ColumnMappings.Add(dataReader.GetName(fieldIdx), dataReader.GetName(fieldIdx));
            }

            // Database
            if (!dataReader.IsClosed)
            {
                dataReader.Close();         // can throw DbException
            }
        }

        public void FillSchema(DataSet dataSet, string tableName)
        {
            // Validate Database Conn
            if (!IsConnOpen())
            {
                OpenConnection();
            }

            this.dbDataAdapter.FillSchema(dataSet, SchemaType.Mapped, tableName);
        }

        public int Fill(DataSet dataSet, string tableName)
        {
            // Validate Database Conn
            if (!IsConnOpen())
            {
                OpenConnection();
            }

            return this.dbDataAdapter.Fill(dataSet, tableName);
        }

        public Task FillSchemaAsync(IDbDataAdapter dbDataAdapter, in DataSet dataSet)
        {
            // Validate Database Conn
            if (!IsConnOpen())
            {
                OpenConnection();
            }

            TaskCompletionSource<DataTable[]> taskCompletionSource = new();
            try
            {
                DataTable[] result = dbDataAdapter.FillSchema(dataSet, SchemaType.Mapped);
                taskCompletionSource.SetResult(result);
            }
            catch (Exception exception)
            {
                taskCompletionSource.SetException(exception);
            }

            return taskCompletionSource.Task;
        }

        public Task FillAsync(IDbDataAdapter dbDataAdapter, in DataSet dataSet)
        {
            if (!IsConnOpen())
            {
                OpenConnection();
            }

            TaskCompletionSource<int> taskCompletionSource = new();
            try
            {
                int result = dbDataAdapter.Fill(dataSet);
                taskCompletionSource.SetResult(result);
            }
            catch (Exception exception)
            {
                taskCompletionSource.SetException(exception);
            }

            return taskCompletionSource.Task;
        }

        public int Delete(DataSet dataSet, string tableName, string deleteStatement)
        {

            if (IsConnOpen())
            {
                OpenConnection();
            }

            DbCommandBuilder sqlCommandBuilder = this.dbProviderFactory!.CreateCommandBuilder()!;

            // Validate for nulls. (local variable)
            ValidateForNullClassVariable(sqlCommandBuilder, nameof(sqlCommandBuilder));

            this.dbDataAdapter.DeleteCommand!.CommandText = deleteStatement;

            sqlCommandBuilder.DataAdapter = this.dbDataAdapter;
            sqlCommandBuilder.GetDeleteCommand();

            return this.dbDataAdapter.Update(dataSet, tableName);
        }

        public int Update<T>(DataSet dataSet, UpdateDbMetaData<T> updateDatabaseMetaData, string updateStatement)
        {
            if (IsConnOpen())
            {
                OpenConnection();
            }

            DbCommandBuilder sqlCommandBuilder = this.dbProviderFactory!.CreateCommandBuilder()!;

            // Validate for nulls. (local variable)
            ValidateForNullClassVariable(sqlCommandBuilder, nameof(sqlCommandBuilder));

            this.dbDataAdapter.UpdateCommand!.CommandText = updateStatement;

            // Clear all parameters before adding new ones
            this.dbDataAdapter.UpdateCommand!.Parameters.Clear();
            this.dbDataAdapter.UpdateCommand!.Parameters
                        .Add(new MySqlParameter(updateDatabaseMetaData.ValueColumnName, updateDatabaseMetaData.NewValue));

            sqlCommandBuilder.DataAdapter = this.dbDataAdapter;
            sqlCommandBuilder.GetUpdateCommand();

            return this.dbDataAdapter.Update(dataSet, updateDatabaseMetaData.TableName);
        }

        public int Insert(DataSet dataSet, string tableName, string[] columnNames, string insertStatement)
        {
            int lastRow = dataSet.Tables[tableName]!.Rows.Count - 1;
            if (IsConnOpen())
            {
                OpenConnection();
            }

            // Validate for nulls. (params)
            ValidateForNullParamater(dataSet, nameof(dataSet));

            DbCommandBuilder sqlCommandBuilder = this.dbProviderFactory!.CreateCommandBuilder()!;

            // Validate for nulls. (local variable)
            ValidateForNullClassVariable(sqlCommandBuilder, nameof(sqlCommandBuilder));

            this.dbDataAdapter.InsertCommand!.CommandText = insertStatement;

            // Clear all parameters before adding new ones
            this.dbDataAdapter.InsertCommand!.Parameters.Clear();
            for (int columnIdx = 0; columnIdx < columnNames.Length; ++columnIdx)
            {
                this.dbDataAdapter.InsertCommand!.Parameters.Add(new MySqlParameter(columnNames[columnIdx], dataSet.Tables[tableName]!.Rows[lastRow][columnNames[columnIdx]]));
            }

            sqlCommandBuilder.DataAdapter = this.dbDataAdapter;
            sqlCommandBuilder.GetInsertCommand(true);

            return this.dbDataAdapter.Update(dataSet, tableName);
        }

        private static void ValidateForNullParamater(object? param, string paramName, [CallerMemberName] string callerName = "")
        {
            if (param is null)
            {
                throw new DbConnectorNullException("<Scheduling_Logic.Model.Database>(DbConnector)",
                new ArgumentNullException(paramName,
                    $"[{callerName}][{paramName}] cannot be null."));
            }
        }

        private static void ValidateForNullClassVariable(object? variable, string variableName, [CallerMemberName] string callerName = "")
        {
            if (variable is null)
            {
                throw new DbConnectorNullException("<Scheduling_Logic.Model.Database>(DbConnector)\n" +
                    $"[{callerName}][{variableName}] cannot be null.");
            }
        }

        private void ConfigureInteralConnection(IDbConfig config)
        {
            this.dbConnection.ConnectionString = config.ConnectionString;
        }

        private void ConfigureInteralDbDataAdapter()
        {
            this.dbDataAdapter.SelectCommand = this.dbProviderFactory.CreateCommand(); // can throw dbexception
            this.dbDataAdapter.UpdateCommand = this.dbProviderFactory.CreateCommand(); // can throw dbexception
            this.dbDataAdapter.InsertCommand = this.dbProviderFactory.CreateCommand(); // can throw dbexception
            this.dbDataAdapter.DeleteCommand = this.dbProviderFactory.CreateCommand(); // can throw dbexception

            // Validate for nulls. (class member property)
            ValidateForNullClassVariable(this.dbDataAdapter.SelectCommand, nameof(this.dbDataAdapter.SelectCommand));
            ValidateForNullClassVariable(this.dbDataAdapter.UpdateCommand, nameof(this.dbDataAdapter.UpdateCommand));
            ValidateForNullClassVariable(this.dbDataAdapter.InsertCommand, nameof(this.dbDataAdapter.InsertCommand));
            ValidateForNullClassVariable(this.dbDataAdapter.DeleteCommand, nameof(this.dbDataAdapter.DeleteCommand));

            this.dbDataAdapter.SelectCommand!.Connection = this.dbConnection;    // can throw null reference exception
            this.dbDataAdapter.UpdateCommand!.Connection = this.dbConnection;    // can throw null reference exception
            this.dbDataAdapter.InsertCommand!.Connection = this.dbConnection;    // can throw null reference exception
            this.dbDataAdapter.DeleteCommand!.Connection = this.dbConnection;    // can throw null reference exception
        }

        /*
         * Description: Disposes different properties and fields of managed and unmanaged data.
         * 
         * @param:      [Boolean] disposing     N/A
         */
        public void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }
                this.dbConnection.Dispose();

                /*                this.DbCommand = null;*/
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~DbConnector()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: false);
        }

        /*
         * Description: This method will be use by the gargabe collector.
         */
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
        }
    }
}