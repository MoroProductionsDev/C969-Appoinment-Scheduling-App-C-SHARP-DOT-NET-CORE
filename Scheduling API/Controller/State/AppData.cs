﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduling_Logic.Model.Data;
using static Scheduling_Logic.Model.Data.ClientScheduleRecord;

namespace Scheduling_API.Controller.State
{
    public sealed class AppData
    {
        public UserRecord UserRecord { get; private set; }
        public AppointmentRecord AppointmentRecord { get; private set; }
        public CustomerRecord CustomerRecord { get; private set; }
        public AddressRecord AddressRecord { get; private set; }
        public CityRecord CityRecord { get; private set; }
        public CountryRecord CountryRecord { get; private set; }
        public AppData()
        {
            this.UserRecord = new UserRecord();
            this.AppointmentRecord = new AppointmentRecord();
            this.CustomerRecord = new CustomerRecord();
            this.AddressRecord = new AddressRecord();
            this.CityRecord = new CityRecord();
            this.CountryRecord = new CountryRecord();
        }
    }
}
