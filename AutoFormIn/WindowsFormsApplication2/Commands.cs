using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Assistant
{
    public class Commands
    {
        private static readonly RoutedCommand _ok, _add, _delete, _browser, _remove, _view;

        public static RoutedCommand OK
        {
            get { return _ok; }
        }

        public static RoutedCommand Add
        {
            get { return _add; }
        }

        public static RoutedCommand Delete
        {
            get { return _delete; }
        }

        public static RoutedCommand Browser
        {
            get { return _browser; }
        }

        public static RoutedCommand Remove
        {
            get { return _remove; }
        }

        public static RoutedCommand View
        {
            get { return _view; }
        }

        static Commands()
        {
            Type typeofThis = typeof(Commands);
            _ok = new RoutedCommand("OK", typeofThis);
            _add = new RoutedCommand("Add", typeofThis);
            _delete = new RoutedCommand("Delete", typeofThis);
            _browser = new RoutedCommand("Browser", typeofThis);
            _remove = new RoutedCommand("Remove", typeofThis);
            _view = new RoutedCommand("View", typeofThis);
        }
    }
}
