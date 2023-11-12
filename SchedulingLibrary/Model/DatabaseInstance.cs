﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SchedulingLibrary
{
    /*
     * Description: This static class applies the S.O.L.I.[D] specially the "D" for "Dependency Inversion".
     *              It will return instances related to data use for the database objects instatiations.
     *              
     *              S.O.L.I.D. stands for: 
     *              S – Single-Responsibility Principle. 
     *              O – Open-Closed Principle. (Open for extension but close to modifications)
     *              L – Liskov Substitution Principle. 
     *              I – Interface Segregation Principle. 
     *              D – Dependency Inversion
     */

    /* References:
     * How to use switch statements on system's type.
     * https://stackoverflow.com/questions/43080505/how-to-switch-on-system-type
     */
    public static class DatabaseInstance
    {
        /*
         * Description: This function creates and returns a new [IDatabaseConnector] object base on the type passed.
         * 
         * @param       [Type] connectionType       It carries a IDbConnection (Type) that will be use to create the
         *                                          right object type.
         *              [String] connectionString   A connection string configuration for the specific
         *                                          database type.
         *                                      
         * @return      A instance of a [IDatabaseConnector].
         */
        public static IDatabaseConnector CreateDatabaseConnector(Type connectionType, String connectionString)
        {
            IDatabaseConnector dbConnector = null;
            switch(connectionType)
            {
                case Type _ when typeof(MySqlConnection) == connectionType:
                {
                    dbConnector = new MySqlConnector(connectionType, connectionString);
                    break;
                }
            }

            return dbConnector;
        }

        /*
         * Description: This function creates and returns a new [IDbConnection] object base on the type passed.
         * 
         * @param       [Type] connectionType       It carries a IDbConnection (Type) that will be use to create the
         *                                          right object type.
         *              [String] connectionString   A connection string configuration for the specific
         *                                          database type.
         *                                      
         * @return      A instance of a [IDbConnection].
         */
        public static IDbConnection CreateDbConnection(Type connectionType, String connectionString)
        {
            IDbConnection dbConnection = null;
            switch (connectionType)
            {
                case Type _ when typeof(MySqlConnection) == connectionType:
                    dbConnection = new MySqlConnection(connectionString);
                    break;
            }

            return dbConnection;
        }

        /*
         * Description: This function creates and returns a new [IDbCommand] object base on the type passed.
         * 
         * @param       [Type] connectionType       It carries a IDbConnection (Type) that will be use to create the
         *                                          right object type.
         *              [String] connectionString   A connection string configuration for the specific
         *                                          database type.
         *                                      
         * @return      A instance of a [IDbCommand].
         */
        public static IDbCommand CreateDbCommand(Type connectionType, String commandText)
        {
            IDbCommand dbCommand = null;
            switch (connectionType)
            {
                case Type _ when typeof(MySqlConnection) == connectionType:
                    dbCommand = new MySqlCommand(commandText);
                    break;
            }

            return dbCommand;
        }
    }
}