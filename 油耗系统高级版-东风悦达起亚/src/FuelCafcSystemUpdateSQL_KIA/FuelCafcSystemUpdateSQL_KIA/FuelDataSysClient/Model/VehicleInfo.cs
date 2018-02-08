using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelDataSysClient.Model
{
    public class VehicleInfo : FuelDataModel.VehicleBasicInfo
    {
        private string baseIdField;

        /// <remarks/>
        public string BaseId
        {
            get
            {
                return this.baseIdField;
            }
            set
            {
                this.baseIdField = value;
            }
        }
    }
}
