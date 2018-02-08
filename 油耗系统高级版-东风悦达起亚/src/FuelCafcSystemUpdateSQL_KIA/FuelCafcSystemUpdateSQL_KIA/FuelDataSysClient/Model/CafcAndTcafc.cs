using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelDataSysClient.Model
{
    public class CafcAndTcafc : FuelDataService_CQCA.FuelCafcAndTcafc
    {
        private decimal tg_tcafcField;

        /// <remarks/>
        public decimal Tg_tcafc
        {
            get
            {
                return this.tg_tcafcField;
            }
            set
            {
                this.tg_tcafcField = value;
            }
        }
    }
}
