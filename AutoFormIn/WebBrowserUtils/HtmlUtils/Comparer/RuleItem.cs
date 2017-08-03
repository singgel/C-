using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace WebBrowserUtils.HtmlUtils.Comparer
{
    public class RuleItem : NotifyPropertyChanged, IEquatable<RuleItem>
    {
        private string _Id;
        private string _Name;
        private string _OnClick;
        private string _Type;
        private string _Value;
        private RuleItemStatus _Status;
        public static RuleItem Empty;
        private static readonly PropertyChangedEventArgs TypeChanged = new PropertyChangedEventArgs("Type");
        private static readonly PropertyChangedEventArgs IdChanged = new PropertyChangedEventArgs("Id");
        private static readonly PropertyChangedEventArgs NameChanged = new PropertyChangedEventArgs("Name");
        private static readonly PropertyChangedEventArgs ValueChanged = new PropertyChangedEventArgs("Value");
        private static readonly PropertyChangedEventArgs OnClickChanged = new PropertyChangedEventArgs("OnClick");
        private static readonly PropertyChangedEventArgs StatusChanged = new PropertyChangedEventArgs("Status");

        public string Type
        {
            get { return _Type; }
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    OnPropertyChanged(TypeChanged);
                }
            }
        }

        public string Id
        {
            get { return _Id; }
            set
            {
                if (_Id != value)
                {
                    _Id = value;
                    OnPropertyChanged(IdChanged);
                }
            }
        }

        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged(NameChanged);
                }
            }
        }

        public string Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    OnPropertyChanged(ValueChanged);
                }
            }
        }

        public string OnClick
        {
            get { return _OnClick; }
            set
            {
                if (_OnClick != value)
                {
                    _OnClick = value;
                    OnPropertyChanged(OnClickChanged);
                }
            }
        }

        public RuleItemStatus Status
        {
            get { return _Status; }
            internal set
            {
                if (_Status != value)
                {
                    _Status = value;
                    OnPropertyChanged(StatusChanged);
                }
            }
        }

        static RuleItem()
        {
            Empty = new RuleItem();
        }

        public override int GetHashCode()
        {
            return string.Format("{0}{1}{2}{3}{4}", _Type, _Id, _Name, _Value, _OnClick).GetHashCode();
        }

        public bool Equals(RuleItem other)
        {
            if (other == null)
                return false;
            return this.Type == other.Type && this._Id == other._Id && this._Name == other._Name && this._Value == other._Value
                && this._OnClick == other._OnClick;
        }

        public static bool operator ==(RuleItem a, RuleItem b)
        {
            return (object.Equals(a, null) ? (object.Equals(b, null) ? true : false) : a.Equals(b));
        }

        public static bool operator !=(RuleItem a, RuleItem b)
        {
            return !(a == b);
        }
    }
}
