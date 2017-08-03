using System.ComponentModel;
using System.Windows.Data.Collection;

namespace Assistant.DataProviders
{
    public class RuleCompareNode : TreeItemModel
    {
        private int _nullCount;
        private int _CheckedCount;
        private object _Content;
        private bool _HasChange;
        private bool? _IsChecked;
        private static readonly PropertyChangedEventArgs CheckedCountChanged = new PropertyChangedEventArgs("CheckedCount");
        private static readonly PropertyChangedEventArgs ContentChanged = new PropertyChangedEventArgs("Content");
        private static readonly PropertyChangedEventArgs HasChangeChanged = new PropertyChangedEventArgs("HasChange");
        private static readonly PropertyChangedEventArgs IsCheckedChanged = new PropertyChangedEventArgs("IsChecked");

        public int CheckedCount
        {
            get { return _CheckedCount; }
            private set
            {
                if (_CheckedCount != value)
                {
                    _CheckedCount = value;
                    OnPropertyChanged(CheckedCountChanged);
                }
            }
        }

        public object Content
        {
            get { return _Content; }
            set
            {
                if (_Content != value)
                {
                    _Content = value;
                    OnPropertyChanged(ContentChanged);
                }
            }
        }

        public bool HasChange
        {
            get { return _HasChange; }
            set
            {
                if (_HasChange != value)
                {
                    _HasChange = value;
                    RuleCompareNode parent = base.Parent as RuleCompareNode;
                    if (parent != null)
                        parent.HasChange = value;
                    OnPropertyChanged(HasChangeChanged);
                }
            }
        }

        public bool? IsChecked
        {
            get { return _IsChecked; }
            set
            {
                if (_IsChecked != value)
                {
                    RuleCompareNode parent = base.Parent as RuleCompareNode;
                    if (parent != null)
                        parent.ChildNotifyIsChecked(_IsChecked, value);
                    _IsChecked = value;
                    SetChildIsChecked(value);
                    OnPropertyChanged(IsCheckedChanged);
                }
            }
        }

        public RuleCompareNode()
        {
            _IsChecked = false;
        }

        private void SetChildIsChecked(bool? value)
        {
            if (value == null)
                return;
            foreach (RuleCompareNode item in base.Children)
            {
                if (item != null)
                {
                    item._IsChecked = value;
                    item.OnPropertyChanged(IsCheckedChanged);
                    item.SetChildIsChecked(value);
                }
            }
            if (value == true)
                CheckedCount = base.Children.Count;
            else
                CheckedCount = 0;
        }

        private void ChildNotifyIsChecked(bool? oldValue, bool? newValue)
        {
            if (oldValue == false)
                CheckedCount++;
            else if (oldValue == null)
            {
                if (newValue == false)
                    CheckedCount--;
                _nullCount--;
            }
            else
            {
                if (newValue == false)
                    CheckedCount--;
            }
            if (newValue == null)
                _nullCount++;
            bool? value = _nullCount == 0 ? (_CheckedCount == 0 ? false : _CheckedCount == base.Children.Count ? true : (bool?)null) : null;
            if (_IsChecked != value)
            {
                RuleCompareNode node = base.Parent as RuleCompareNode;
                if (node != null)
                    node.ChildNotifyIsChecked(_IsChecked, value);
                _IsChecked = value;
                OnPropertyChanged(IsCheckedChanged);
            }
        }
    }
}
