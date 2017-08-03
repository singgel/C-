using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Assistant.DataProviders.HCBM
{
    public class Product : NotifyPropertyChanged
    {
        private bool _ApplicantCatalyticConverter;

        private static readonly PropertyChangedEventArgs ApplicantCatalyticConverterChanged = new PropertyChangedEventArgs("ApplicantCatalyticConverter");

        public bool ApplicantCatalyticConverter
        {
            get { return _ApplicantCatalyticConverter; }
            set
            {
                if (_ApplicantCatalyticConverter != value)
                {
                    _ApplicantCatalyticConverter = value;
                    OnPropertyChanged(ApplicantCatalyticConverterChanged);
                }
            }
        }

        public string ApplicantType { get; set; }

        public string Brand { get; set; }

        public string Id { get; set; }

        public string Model { get; set; }

        public string Name { get; set; }
    }
}
