using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
//using System.Windows.Input;

namespace Assistant.DataProviders.HCBM
{
    /// <summary>
    /// 华晨宝马国环数据提供类。
    /// </summary>
    public class GHDataProvider : NotifyPropertyChanged, IDataProvider
    {
        private string _connectionString;
        private OleDbConnection conn;
        private SelectorModel _productList;
        private string _SearchString;

        private static readonly PropertyChangedEventArgs ProductsChanged = new PropertyChangedEventArgs("Products");
        private static readonly PropertyChangedEventArgs SearchStringChanged = new PropertyChangedEventArgs("SearchString");

        public string SearchString
        {
            get { return _SearchString; }
            set
            {
                if (_SearchString != value)
                {
                    _SearchString = value;
                    OnPropertyChanged(SearchStringChanged);
                }
            }
        }

        public SelectorModel Products
        {
            get { return _productList; }
        }

        string IDataProvider.DataSourceFile
        {
            get { return _connectionString; }
            set
            {
                if (conn != null)
                    conn.Dispose();
                conn = null;
                if (value != null)
                {
                    conn = new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};", value));
                    conn.Open();
                }
                _connectionString = value;
            }
        }

        bool IDataProvider.AllowAlternately
        {
            get { return true; }
        }

        public GHDataProvider()
        {
            _productList = new SelectorModel();
        }

        void IDataProvider.Clean()
        {
        }

        ValueConverter IDataProvider.GetConverter()
        {
            return new WebValueConverter();
        }

        object IDataProvider.ProvideData(object state)
        {
            if (state != null)
                return null;
            if (conn == null)
                throw new ArgumentException("未指定有效的数据源！");
            Hashtable result = new Hashtable();
            Product selected = _productList.SelectedItem as Product;
            using (OleDbCommand cmd = new OleDbCommand("", conn))
            {
                cmd.CommandText = string.Format(@"select ba.seq, ba.cname, cb.fieldvalue from CPSB_BACS cb inner join BACSXM ba on cb.fieldname = ba.ename
                where cb.cp_id = '{0}'
                union 
                select gg.seq, gg.cname, cj.fieldvalue from CPSB_JSHCSH cj inner join GGCSXM gg on cj.fieldename = gg.ename
                where cj.cp_id = '{0}'", selected.Id);
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string key = reader["seq"] as string;
                        result.Add(key, reader["fieldvalue"]);
                    }
                }
            }
            result.Add("Model", selected.Model);
            result.Add("ModelName", selected.Name);
            result.Add("Brand", selected.Brand);
            return result;
        }

        private void InitProductList()
        {
            if (conn == null)
                throw new ArgumentException("未指定有效的数据源！");
            _productList.Items.Clear();
            using (OleDbCommand cmd = new OleDbCommand("", conn))
            {
                if (string.IsNullOrEmpty(_SearchString))
                    cmd.CommandText = @"select cp_id, cpxh, Cpsb, Cpmc from cpsb_chpxx";
                else
                    cmd.CommandText = string.Format(@"select cp_id, cpxh, Cpsb, Cpmc from cpsb_chpxx where cpxh like '%{0}%' or Cpmc like '%{0}%' or Cpsb like '%{0}%'", _SearchString);
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _productList.Items.Add(new Product() { Id = reader["cp_id"] as string, Model = reader["cpxh"] as string, Brand = reader["Cpsb"] as string, Name = reader["Cpmc"] as string, ApplicantType = "新产品" });
                    }
                }
            }
        }

        bool IDataProvider.ShowWindow()
        {
            //this.InitProductList();
            //Window window = new Window();
            //ResourceDictionary res = new ResourceDictionary();
            //res.BeginInit();
            //res.Source = new Uri("pack://application:,,,/DataProvider;component/Template/HCBM.xaml");
            //res.EndInit();
            //window.ContentTemplate = res["hcbm"] as DataTemplate;
            //window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //window.ResizeMode = ResizeMode.NoResize;
            //window.SizeToContent = SizeToContent.WidthAndHeight;
            ////window.InputBindings.Add(new InputBinding(ApplicationCommands.Open, new System.Windows.Input.KeyGesture(Key.Enter, ModifierKeys.Shift)));
            //window.DataContext = this;
            //window.Content = this;
            //window.CommandBindings.Add(new System.Windows.Input.CommandBinding(ApplicationCommands.Find, OnFindProduct, CanFindProduct));
            //window.CommandBindings.Add(new System.Windows.Input.CommandBinding(ApplicationCommands.Open, OnConfirmProduct, CanConfirmProduct));
            //window.CommandBindings.Add(new System.Windows.Input.CommandBinding(ApplicationCommands.Stop, OnCancel));
            //return window.ShowDialog() == true;
            HCBMForm form = new HCBMForm();
            form.Connection = conn;
            return form.ShowDialog() == System.Windows.Forms.DialogResult.OK;
        }

        //private void OnCancel(object sender, ExecutedRoutedEventArgs e)
        //{
        //    Window window = (Window)sender;
        //    window.DialogResult = false;
        //}

        //private void OnFindProduct(object sender, ExecutedRoutedEventArgs e)
        //{
        //    InitProductList();
        //}

        //private void CanFindProduct(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = true;
        //}

        //private void CanConfirmProduct(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = _productList != null && _productList.SelectedItem != null;
        //}

        //private void OnConfirmProduct(object sender, ExecutedRoutedEventArgs e)
        //{
        //    Window window = (Window)sender;
        //    window.DialogResult = true;
        //}

        bool IDataProvider.CanValidation
        {
            get { return false; }
        }

        bool IDataProvider.Validate()
        {
            return true;
        }
    }
}
