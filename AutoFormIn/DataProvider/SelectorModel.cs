using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Data.Collection;

namespace Assistant.DataProviders
{
    public class SelectorModel : SelectorDataModel
    {
        private InfoList _items;

        public override System.Collections.IList Items
        {
            get { return _items; }
        }

        public SelectorModel()
        {
            _items = new InfoList();
        }
    }
}
