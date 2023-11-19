﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduling_Library.Model.Structure
{
    public class ClientScheduleDbSchema : DbSchema
    {
        public const string _dbName = Structure.DbName.ClientScheduleDbName;

        private static readonly Dictionary<int, string> _tableNamesIndented = new Dictionary<int, string>()
        {
            {0, ClientScheduleTableName.User},
            {1, ClientScheduleTableName.Appointment},
            {2, ClientScheduleTableName.Customer},
            {3, ClientScheduleTableName.Address},
            {4, ClientScheduleTableName.City},
            {5, ClientScheduleTableName.Country}
        };

        private static readonly Dictionary<string, string[]> _primaryKeys = new Dictionary<string, string[]>()
        {
            {ClientScheduleTableName.User, new[]{UserColumnName.UserId} },
            {ClientScheduleTableName.Appointment, new[]{AppointmentColumnName.AppointmentId} },
            {ClientScheduleTableName.Customer, new[] {CustomerColumnName.CustomerId} },
            {ClientScheduleTableName.Address, new[] {AddressColumnName.AddressId} },
            {ClientScheduleTableName.City, new[] {CityColumnName.CityId} },
            {ClientScheduleTableName.Country, new[] {CountryColumnName.CountryId}}
        };

        private static readonly Dictionary<string, string[]> _foreignKeys = new Dictionary<string, string[]>()
        {
            {ClientScheduleTableName.User, new[]{String.Empty} },
            {ClientScheduleTableName.Appointment, new []{ AppointmentColumnName.CustomerId, AppointmentColumnName .UserID} },
            {ClientScheduleTableName.Customer, new[] {CustomerColumnName.AddressId} },
            {ClientScheduleTableName.Address, new[] {AddressColumnName.CityId} },
            {ClientScheduleTableName.City, new[] {CityColumnName.CountryId} },
            {ClientScheduleTableName.Country, new[] {String.Empty } }
        };

        private static readonly Dictionary<string, string[]> _fkTablesContraints = new Dictionary<string, string[]>()
        {
            {ClientScheduleTableName.User, new[]{String.Empty} },
            {ClientScheduleTableName.Appointment, new []{ClientScheduleTableName.Customer, ClientScheduleTableName.User} },
            {ClientScheduleTableName.Customer, new[] {ClientScheduleTableName.Address} },
            {ClientScheduleTableName.Address, new[] {ClientScheduleTableName.City} },
            {ClientScheduleTableName.City, new[] {ClientScheduleTableName.Country} },
            {ClientScheduleTableName.Country, new[] {String.Empty } }
        };

        public override String DbName { get => _dbName; }
        public override Dictionary<int, string> TableNamesIndented { get => _tableNamesIndented; }
        public override Dictionary<string, string[]> PrimaryKeys { get => _primaryKeys; }
        public override Dictionary<string, string[]> ForeignKeys { get => _foreignKeys; }
        public override Dictionary<string, string[]> FKTables { get => _fkTablesContraints; }

        public static class ClientScheduleTableName
        {
            public const string User = "user";
            public const string Appointment = "appointment";
            public const string Customer = "customer";
            public const string Address = "address";
            public const string City = "city";
            public const string Country = "country";
        }

        public static class UserColumnName
        {
            public const string UserId = "userId";
            public const string UserName = "userName";
            public const string Password = "password";
        }

        public static class AppointmentColumnName
        {
            public const string AppointmentId = "appointmentId";
            public const string CustomerId = "customerId";
            public const string UserID = "userId";
            public const string Title = "title";
            public const string Description = "description";
            public const string Location = "location";
            public const string Contact = "contact";
            public const string Type = "type";
            public const string Url = "url";
            public const string Start = "start";
            public const string End = "end";
        }

        public static class CustomerColumnName
        {
            public const string CustomerId = "customerId";
            public const string CustomerName = "customerName";
            public const string AddressId = "addressId";
            public const string Active = "active";
        }

        public static class AddressColumnName
        {
            public const string AddressId = "addressId";
            public const string Address = "address";
            public const string Address2 = "address2";
            public const string CityId = "cityId";
            public const string PostalCode = "postalCode";
            public const string Phone = "phone";
        }

        public static class CityColumnName
        {
            public const string CityId = "cityId";
            public const string City = "city";
            public const string CountryId = "countryId";
        }

        public static class CountryColumnName
        {
            public const string CountryId = "countryId";
            public const string Country = "country";
        }

        public static class AllInCommonColumns
        {
            public const string CreateDate = "createDate";
            public const string CreatedBy = "createdBy";
            public const string LastUpdate = "lastUpdate";
            public const string LastUpdateBy = "lastUpdateBy";
        }
    }
}