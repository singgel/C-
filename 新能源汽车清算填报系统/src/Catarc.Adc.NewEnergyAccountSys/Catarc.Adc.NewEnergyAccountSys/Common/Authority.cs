using System;
using System.Collections.Generic;
using System.Linq;
using Catarc.Adc.NewEnergyAccountSys.Properties;
using System.Xml;
using System.Windows.Forms;

namespace Catarc.Adc.NewEnergyAccountSys.Common
{
    class Authority
    {
        public static List<MenuModel> ReadMenusXmlData(string pathName)
        {
            // string path = Utils.installPath + Settings.Default["VehicleMeunUrl"];
            string path = Utils.installPath + Settings.Default[pathName];
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNode xn = xmlDoc.SelectSingleNode("MenuData");
            XmlNodeList xnl = xn.ChildNodes;
            List<MenuModel> listModel = new List<MenuModel>();
            foreach (XmlNode nodel in xnl)
            {
                MenuModel model = new MenuModel();
                XmlElement xe = (XmlElement)nodel;
                XmlNodeList xnll = xe.ChildNodes;
                model.ID = Int32.Parse(xnll.Item(0).InnerText);
                model.MenuName = xnll.Item(1).InnerText;
                model.ParentID = xnll.Item(2).InnerText.ToString();
                model.OrderID = Int32.Parse(xnll.Item(3).InnerText);
                listModel.Add(model);
            }
            return listModel;
        }
        public class MenuModel
        {
            #region 字段属性

            private int id;
            /// <summary>
            /// Gets or sets the menu ID.
            /// </summary>
            public int ID
            {
                get { return id; }
                set { id = value; }
            }

            private string parentID;
            /// <summary>
            /// Gets or sets the parent ID.
            /// </summary>
            /// <value>The parent ID.</value>
            public string ParentID
            {
                get { return parentID; }
                set { parentID = value; }
            }

            private int orderID;
            /// <summary>
            /// Gets or sets the order ID.
            /// </summary>
            /// <value>The order ID.</value>
            public int OrderID
            {
                get { return orderID; }
                set { orderID = value; }
            }

            private string menuName;
            /// <summary>
            /// Gets or sets the name of the menu.
            /// </summary>
            /// <value>The name of the menu.</value>
            public string MenuName
            {
                get { return menuName; }
                set { menuName = value; }
            }

            #endregion

            public MenuModel() { }

            protected MenuModel(MenuModel model)
            {
                this.id = model.id;
                this.menuName = model.menuName;
                this.orderID = model.orderID;
                this.parentID = model.parentID;
            }
        }
    }
}
