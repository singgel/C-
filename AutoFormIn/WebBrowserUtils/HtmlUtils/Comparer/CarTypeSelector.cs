using Assistant.DataProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Data.Collection;
using Updater = AssistantUpdater;

namespace WebBrowserUtils.HtmlUtils.Comparer
{
    public class CarTypeSelector : SelectorDataModel
    {
        private TreeModel _Tree;
        private List<string> _Sites;
        private static readonly PropertyChangedEventArgs TreeChanged = new PropertyChangedEventArgs("Tree");

        public override System.Collections.IList Items
        {
            get { return _Sites; }
        }

        public TreeModel Tree
        {
            get { return _Tree; }
            private set
            {
                if (_Tree != value)
                {
                    _Tree = value;
                    OnPropertyChanged(TreeChanged);
                }
            }
        }

        public CarTypeSelector()
        {
            _Sites = FileHelper.GetWebsites();
            base.SelectedItem = _Sites.Count <= 0 ? null : _Sites[0];
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                string website = base.SelectedItem as string;
                if(website != null)
                    Tree = FileHelper.GetCarTypeList(website);
            }
            base.OnPropertyChanged(e);
        }
    }
}
