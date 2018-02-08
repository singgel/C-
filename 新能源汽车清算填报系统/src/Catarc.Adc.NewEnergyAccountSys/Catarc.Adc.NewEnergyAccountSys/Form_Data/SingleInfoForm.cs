using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Catarc.Adc.NewEnergyAccountSys.DBUtils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System.Data.OleDb;
using Catarc.Adc.NewEnergyAccountSys.OfficeHelper;
using Catarc.Adc.NewEnergyAccountSys.Common;
using Catarc.Adc.NewEnergyAccountSys.PopForm;
using Catarc.Adc.NewEnergyAccountSys.Properties;
using Catarc.Adc.NewEnergyAccountSys.LogUtils;

namespace Catarc.Adc.NewEnergyAccountSys.Form_Data
{
    public partial class SingleInfoForm : Form
    {
        bool bMode = true;//true 新增   false 更新
        bool bVesion = true;
        string clpz_Final = string.Empty;
        string clearYear = Catarc.Adc.NewEnergyAccountSys.Properties.Settings.Default.ClearYear;//清算年份
        string strCompany = Catarc.Adc.NewEnergyAccountSys.Properties.Settings.Default.Vehicle_MFCS;//生产企业

        Dictionary<string, string> checkControl = new Dictionary<string, string>();//check控件列表
        Dictionary<string, string> tempPictureName = new Dictionary<string, string>();//临时文件名
        Dictionary<string, string> mapPictureName = new Dictionary<string, string>();//
        Dictionary<string, string> mapRightData = new Dictionary<string, string>();//与公告比对值不同的列表  需要显示tooltip控件的名称与显示值
        Dictionary<string,Control> mapControl = new Dictionary<string,Control>();//控件列表
        Dictionary<string, string> mapResult = new Dictionary<string, string>();
        public SingleInfoForm()
        {
            InitializeComponent();
            this.bMode = true;
            //商业版与免费版区分
            if (!Settings.Default.AuthorityUrl.Equals(@"\Template\Authority.XML"))
            {
                this.bVesion = false;
            }
            //画基本信息tab页
            drawBaseInfomation();
            //画配置信息tab页
            drawConfigInfomation();
            //画运行信息tab页
            drawWorkingInfomation();
            bt_view.Visible = false;

        }
        public SingleInfoForm(Dictionary<string, string> mapData,Dictionary<string,string>mapRightData ,bool bRet, bool bMode)//bRet  true 显示所有值   false  显示部分值  bMode  true 新增   false 更新
        {
            InitializeComponent();
            this.bMode = bMode;
            this.mapRightData = mapRightData;
            //商业版与免费版区分
            if (!Settings.Default.AuthorityUrl.Equals(@"\Template\Authority.XML"))
            {
                this.bVesion = false;
            }
            this.mapResult = mapData;
            //画基本信息tab页
            drawBaseInfomation();
            //画配置信息tab页
            drawConfigInfomation();
            //画运行信息tab页
            drawWorkingInfomation();
            //显示数据
            ShowUpdataData( bRet);
            //与公告比对值不同的控件
            drawRightData();
            if (bMode == false)
            {
                sbt_save_add.Visible = false;
                if (mapControl.ContainsKey("VIN"))
                {
                    ((TextEdit)mapControl["VIN"]).Properties.ReadOnly = true;
                }
                if (mapData.ContainsKey("CLPZ"))
                {
                    clpz_Final = mapData["CLPZ"];
                }
            }
            else
            {
                sbt_save_add.Visible = true;
                ((TextEdit)mapControl["VIN"]).Properties.ReadOnly = false;
            }
            mapResult.Clear();
            bt_view.Visible = false;
        }
        //画运行信息tab页
        void drawWorkingInfomation()
        {
            try
            {
                List<CheckEdit> lsCheckEdit = new List<CheckEdit>();
                //获取运行信息界面控件
                DataSet ds = new DataSet();
                ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, String.Format("SELECT PARAM_CODE, PARAM_NAME, PARAM_REMARK,CONTROL_TYPE,CONTROL_VALUE ,ORDER_RULE,PARENT_PARAM ,PARAM_DEFAULT FROM WORKING_INFOMATION WHERE    STATUS = '1' ORDER BY ORDER_RULE "), null);
                DataTable dt = ds.Tables[0];
          
                //遍历控件
                foreach (DataRow dr in dt.Rows)
                {
                    //如果lab类型
                    if (dr["CONTROL_TYPE"].ToString() == "LAB")
                    {
                        Label lbl1 = new Label() { Width = 240, Height = 30, Name = dr["PARAM_CODE"].ToString(), Text = "截止至" + clearYear + "年底", TextAlign = System.Drawing.ContentAlignment.MiddleRight };
                        Label lbl2 = new Label() { Width = 180, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                        Label lbl3 = new Label() { Width = 10, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                        Label lbl4 = new Label() { Width = 240, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                        Label lbl5 = new Label() { Width = 180, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                        Label lbl6 = new Label() { Width = 10, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                        this.tlp.Controls.Add(lbl1);
                        this.tlp.Controls.Add(lbl2);
                        this.tlp.Controls.Add(lbl3);
                        this.tlp.Controls.Add(lbl4);
                        this.tlp.Controls.Add(lbl5);
                        this.tlp.Controls.Add(lbl6);
                    }
                    //Text类型
                    if (dr["CONTROL_TYPE"].ToString() == "TEXT")
                    {
                        Label lbl = new Label() { Width = 240, Height = 30, Name = "lbl" + dr["PARAM_CODE"], Text = dr["PARAM_NAME"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleRight };
                        TextEdit tb = new TextEdit() { Width = 180, Height = 28, Name = dr["PARAM_CODE"].ToString() };
                        Label lbll = new Label() { Width = 10, Height = 30, Text = dr["PARAM_REMARK"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                        this.tlp.Controls.Add(lbl);
                        this.tlp.Controls.Add(tb);
                        this.tlp.Controls.Add(lbll);
                        mapControl.Add(dr["PARAM_CODE"].ToString(), tb);
                        if (bVesion)
                        {
                            DevTextEditControl.SetWatermark(tb, dr["PARAM_DEFAULT"].ToString());
                        }
                        
                    }
                    // OPTION类型
                    if (dr["CONTROL_TYPE"].ToString() == "OPTION")
                    {
                        Label lbl = new Label() { Width = 240, Height = 30, Name = "lbl" + dr["PARAM_CODE"], Text = dr["PARAM_NAME"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleRight };
                        DevExpress.XtraEditors.ComboBoxEdit cbe = new ComboBoxEdit() { Width = 180, Height = 28, Name = dr["PARAM_CODE"].ToString() };
                        cbe.Properties.Items.AddRange(dr["CONTROL_VALUE"].ToString().Split('/'));
                        cbe.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                        Label lbll = new Label() { Width = 10, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                        this.tlp.Controls.Add(lbl);
                        this.tlp.Controls.Add(cbe);
                        this.tlp.Controls.Add(lbll);
                        mapControl.Add(dr["PARAM_CODE"].ToString(), cbe);
                    }
                    //CHECKBOX类型
                    if (dr["CONTROL_TYPE"].ToString() == "CHECKBOX")
                    {
                       Label lbl = new Label() { Width = 110, Height = 30, Name = "lbl" + dr["PARAM_CODE"] };
                       DevExpress.XtraEditors.CheckEdit cbe = new CheckEdit() { Width = 120, Height = 28, Name = dr["PARAM_CODE"].ToString(), Text = dr["PARAM_NAME"].ToString() };
                       cbe.Dock = System.Windows.Forms.DockStyle.Right;
                        Label lbll = new Label() { Width = 10, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                        this.tlp.Controls.Add(cbe);
                        this.tlp.Controls.Add(lbl);
                        
                        this.tlp.Controls.Add(lbll);
                        checkControl.Add(dr["PARAM_CODE"].ToString(), dr["CONTROL_VALUE"].ToString());
                        cbe.CheckedChanged += new System.EventHandler(checkChange);
                        lsCheckEdit.Add(cbe);
                        mapControl.Add(dr["PARAM_CODE"].ToString(), cbe);
                    }
                    tlp.Location = new Point(10, 15);
                }
                //处理与CHECKBOX所有关联控件状态
                foreach (CheckEdit val in lsCheckEdit)
                {
                    SetCheckChildStatue(val);
                }
            }
            catch (System.Exception ex)
            {
                LogManager.Log("Error","error","初始化运行信息失败，失败原因："+ex.Message);
            }
           
        }
        //画配置信息tab页
        void drawConfigInfomation()
        {
            try
            {
                List<CheckEdit> lsCheckEdit = new List<CheckEdit>();//记录checkEdit控件
                //初始位置
                int x_1 = 15;//
                int x_2 = 450;
                int y_1 = 15;
                int y_2 = 15;
                //查询控件
                DataSet ds = new DataSet();
                ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, String.Format("SELECT PARAM_CODE, PARAM_NAME, PARAM_REMARK,CONTROL_TYPE,CONTROL_VALUE ,ORDER_RULE,PARENT_PARAM  ,PARENT_CONTROL ,PARAM_DEFAULT FROM CONFIG_INFOMATION WHERE  (STATUS = '1' AND PARENT_PARAM is null)   ORDER BY ORDER_RULE "), null);
                DataTable dt = ds.Tables[0];
                //遍历控件
                foreach (DataRow dr in dt.Rows)
                {
                    //check类型
                    if (dr["CONTROL_TYPE"].ToString() == "CHECKBOX")
                    {
                        DevExpress.XtraEditors.CheckEdit cbe = new CheckEdit() { Width = 250, Height = 24, Name = dr["PARAM_CODE"].ToString(), Text = dr["PARAM_NAME"].ToString() };
                        checkControl.Add(dr["PARAM_CODE"].ToString(), dr["CONTROL_VALUE"].ToString());
                        //添加check事件
                        cbe.CheckedChanged += new System.EventHandler(checkChange);
                        panelControl3.Controls.Add(cbe);
                        lsCheckEdit.Add(cbe);
                        //判断放在左右边
                        if (dr["PARENT_CONTROL"].ToString() == "1")
                        {
                            cbe.Top = y_1;
                            cbe.Left = x_1;
                            y_1 += 25;
                        }
                        else
                        {
                            cbe.Top = y_2;
                            cbe.Left = x_2;
                            y_2 += 25;
                        }
                        mapControl.Add(dr["PARAM_CODE"].ToString(), cbe);
                    }
                    //group类型
                    if (dr["CONTROL_TYPE"].ToString() == "GROUP")
                    {
                        //查询group所有子控件
                        DataSet dsGroup = new DataSet();
                        dsGroup = AccessHelper.ExecuteDataSet(AccessHelper.conn, String.Format("SELECT PARAM_CODE, PARAM_NAME, PARAM_REMARK,CONTROL_TYPE,CONTROL_VALUE ,ORDER_RULE,PARENT_PARAM ,PARAM_DEFAULT  FROM CONFIG_INFOMATION WHERE  (STATUS = '1' AND PARENT_PARAM='{0}')   ORDER BY ORDER_RULE ", dr["PARAM_CODE"]), null);
                        DataTable dtGroup = dsGroup.Tables[0];
                        GroupBox gb = new GroupBox() { Width = 420, Height = 25 * dtGroup.Rows.Count + 16, Name = dr["PARAM_CODE"].ToString(), Text = dr["PARAM_NAME"].ToString() };
                        int i = 0;
                        //遍历子控件
                        foreach (DataRow drGroup in dtGroup.Rows)
                        {
                            if (drGroup["CONTROL_TYPE"].ToString() == "TEXT")
                            {
                                Label lbl = new Label() { Width = 120, Height = 25, Name = "lbl" + drGroup["PARAM_CODE"], Text = drGroup["PARAM_NAME"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleRight };
                                TextEdit tb = new TextEdit() { Width = 250, Height = 24, Name = drGroup["PARAM_CODE"].ToString() };
                                Label lbll = new Label() { Width = 10, Height = 25, Text = drGroup["PARAM_REMARK"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                                gb.Controls.Add(lbl);
                                gb.Controls.Add(tb);
                                gb.Controls.Add(lbll);
                                //控件位置
                                lbl.Top = 15 + i * 25;
                                lbl.Left = 5;
                                tb.Top = 15 + i * 25;
                                tb.Left = 5 + 120;
                                lbll.Top = 15 + i * 25;
                                lbll.Left = 5 + 120 + 250;
                                tb.MouseEnter += new System.EventHandler(te_MouseEnter);
                                tb.MouseLeave += new System.EventHandler(te_MouseLeave);
                                mapControl.Add(drGroup["PARAM_CODE"].ToString(), tb);
                                if (bVesion)
                                {
                                    DevTextEditControl.SetWatermark(tb, drGroup["PARAM_DEFAULT"].ToString());

                                }
                            }
                            // OPTION类型
                            if (drGroup["CONTROL_TYPE"].ToString() == "OPTION")
                            {
                                Label lbl = new Label() { Width = 120, Height = 25, Name = "lbl" + drGroup["PARAM_CODE"], Text = drGroup["PARAM_NAME"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleRight };
                                DevExpress.XtraEditors.ComboBoxEdit cbe = new ComboBoxEdit() { Width = 250, Height = 24, Name = drGroup["PARAM_CODE"].ToString() };
                                cbe.Properties.Items.AddRange(drGroup["CONTROL_VALUE"].ToString().Split('/'));
                                cbe.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                                Label lbll = new Label() { Width = 10, Height = 25, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                                gb.Controls.Add(lbl);
                                gb.Controls.Add(cbe);
                                gb.Controls.Add(lbll);
                                //控件位置
                                lbl.Top = 15 + i * 25;
                                lbl.Left = 5;
                                cbe.Top = 15 + i * 25;
                                cbe.Left = 5 + 120;
                                lbll.Top = 15 + i * 25;
                                lbll.Left = 5 + 120 + 250;
                                mapControl.Add(drGroup["PARAM_CODE"].ToString(), cbe);
                            }
                            i++;
                        }
                        panelControl3.Controls.Add(gb);
                        //判断放在左右边
                        if (dr["PARENT_CONTROL"].ToString() == "1")
                        {
                            gb.Top = y_1;
                            gb.Left = x_1;
                            y_1 += 25 * dtGroup.Rows.Count + 25;
                        }
                        else
                        {
                            gb.Top = y_2;
                            gb.Left = x_2;
                            y_2 += 25 * dtGroup.Rows.Count + 25;
                        }
                        mapControl.Add(dr["PARAM_CODE"].ToString(), gb);
                    }

                }
                //处理与CHECKBOX所有关联控件状态
                foreach (CheckEdit val in lsCheckEdit)
                {
                    SetCheckChildStatue(val);
                }
            }
            catch (System.Exception ex)
            {
                LogManager.Log("Error", "error", "初始化配置信息失败，失败原因：" + ex.Message);
            }
            
           
        }
        //画基本信息tab页
        void drawBaseInfomation()
        {
            try
            {
                DataSet ds = new DataSet();
                ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, String.Format("SELECT PARAM_CODE, PARAM_NAME, PARAM_REMARK,CONTROL_TYPE,CONTROL_VALUE ,ORDER_RULE,PARENT_PARAM ,PARAM_DEFAULT FROM BASE_INFOMATION WHERE    STATUS = '1' ORDER BY ORDER_RULE "), null);
                DataTable dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["CONTROL_TYPE"].ToString() == "TEXT")
                    {
                        Label lbl = new Label() { Width = 150, Height = 30, Name = "lbl" + dr["PARAM_CODE"], Text = dr["PARAM_NAME"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleRight };
                        TextEdit tb = new TextEdit() { Width = 250, Height = 28, Name = dr["PARAM_CODE"].ToString() };
                        Label lbll = new Label() { Width = 10, Height = 30, Text = dr["PARAM_REMARK"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                        this.tlp_base.Controls.Add(lbl);
                        this.tlp_base.Controls.Add(tb);
                        this.tlp_base.Controls.Add(lbll);
                        if (dr["PARAM_CODE"].ToString() == "FPTP" || dr["PARAM_CODE"].ToString() == "XSZTP")
                        {
                            tb.Properties.ReadOnly = true;

                        }
                        tb.MouseEnter += new System.EventHandler(te_MouseEnter);
                        tb.MouseLeave += new System.EventHandler(te_MouseLeave);
                        mapControl.Add(dr["PARAM_CODE"].ToString(), tb);
                        if (bVesion)
                        {
                            DevTextEditControl.SetWatermark(tb, dr["PARAM_DEFAULT"].ToString());
                        }
                        if (dr["PARAM_CODE"].ToString() == "VIN")
                        {
                            tb.Properties.MaxLength = 17;
                        }
                        
                    }
                    // OPTION类型
                    if (dr["CONTROL_TYPE"].ToString() == "OPTION")
                    {
                        Label lbl = new Label() { Width = 150, Height = 30, Name = "lbl" + dr["PARAM_CODE"], Text = dr["PARAM_NAME"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleRight };
                        DevExpress.XtraEditors.ComboBoxEdit cbe = new ComboBoxEdit() { Width = 250, Height = 28, Name = dr["PARAM_CODE"].ToString() };
                        cbe.Properties.Items.AddRange(dr["CONTROL_VALUE"].ToString().Split('/'));
                        cbe.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                        Label lbll = new Label() { Width = 10, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                        this.tlp_base.Controls.Add(lbl);
                        this.tlp_base.Controls.Add(cbe);
                        this.tlp_base.Controls.Add(lbll);
                        if (dr["PARAM_CODE"].ToString() == "CLZL")
                        {
                            cbe.EditValueChanged += new System.EventHandler(CLZLValueChange);
                        }
                        if (dr["PARAM_CODE"].ToString() == "GCSF")
                        {
                            cbe.EditValueChanged += new System.EventHandler(GCSFValueChange);
                        }
                        if (dr["PARAM_CODE"].ToString() == "CLYT")
                        {
                            cbe.EditValueChanged += new System.EventHandler(CLYTValueChange);
                        }
                        mapControl.Add(dr["PARAM_CODE"].ToString(), cbe);
                    }
                    if (dr["CONTROL_TYPE"].ToString() == "RESERVE")
                    {
                        Label lbl1 = new Label();
                        Label lbl2 = new Label();
                        Label lbl3 = new Label();
                        this.tlp_base.Controls.Add(lbl1);
                        this.tlp_base.Controls.Add(lbl2);
                        this.tlp_base.Controls.Add(lbl3);
                    }
                    if (dr["CONTROL_TYPE"].ToString() == "TIME")
                    {
                        Label lbl = new Label() { Width = 150, Height = 30, Name = "lbl" + dr["PARAM_CODE"], Text = dr["PARAM_NAME"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleRight };
                        DevExpress.XtraEditors.DateEdit cbe = new DateEdit() { Width = 250, Height = 28, Name = dr["PARAM_CODE"].ToString() };
                        Label lbll = new Label() { Width = 10, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                        this.tlp_base.Controls.Add(lbl);
                        this.tlp_base.Controls.Add(cbe);
                        this.tlp_base.Controls.Add(lbll);
                        mapControl.Add(dr["PARAM_CODE"].ToString(), cbe);
                    }
                    if (dr["CONTROL_TYPE"].ToString() == "PICTURE")
                    {
                        Label lbl = new Label() { Width = 150, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
                        DevExpress.XtraEditors.PictureEdit cbe = new PictureEdit() { Width = 250, Height = 150, Name = dr["PARAM_CODE"].ToString() };
                        Label lbll = new Label() { Width = 10, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                       // cbe.Properties.SizeMode = PictureSizeMode.Zoom;
                        this.tlp_base.Controls.Add(lbl);
                        this.tlp_base.Controls.Add(cbe);
                        this.tlp_base.Controls.Add(lbll);
                        cbe.Text = "";
                        mapControl.Add(dr["PARAM_CODE"].ToString(), cbe);
                    }
                    if (dr["CONTROL_TYPE"].ToString() == "BUTTON")
                    {
                        Label lbl = new Label() { Width = 150, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
                        DevExpress.XtraEditors.PanelControl pcl = new DevExpress.XtraEditors.PanelControl() { Width = 250, Height = 30 };
                        // DevExpress.XtraEditors.PictureEdit cbe = new PictureEdit() { Width = 250, Height = 150, Name = dr["PARAM_CODE"].ToString() };
                        pcl.BorderStyle = BorderStyles.NoBorder;
                        string[] paramName = dr["PARAM_NAME"].ToString().Split('/');
                        int paraNameNum = paramName.Count();
                        int width = 210 / paraNameNum;
                        for (int i = 0; i < paraNameNum; i++)
                        {
                            DevExpress.XtraEditors.SimpleButton bte = new DevExpress.XtraEditors.SimpleButton() { Width = width, Height = 30, Name = paramName[i], Text = paramName[i] };
                            pcl.Controls.Add(bte);
                            bte.Left = i * width;
                            bte.Click += new System.EventHandler(buttonClick);
                            mapControl.Add(paramName[i].ToString(), bte);
                        }
                        Label lbll = new Label() { Width = 10, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
                        this.tlp_base.Controls.Add(lbl);
                        this.tlp_base.Controls.Add(pcl);
                        this.tlp_base.Controls.Add(lbll);
                        
                    }
                    tlp_base.Location = new Point(10, 15);
                }
            }
            catch (System.Exception ex)
            {
                LogManager.Log("Error", "error", "初始化基本信息失败，失败原因：" + ex.Message);
            }
            
        }
        //与公告比对值不同的控件
        void drawRightData()
        {
            try
            {
                foreach(var val in mapRightData)
                {
                    if (mapControl.ContainsKey(val.Key))
                    {
                        mapControl[val.Key].BackColor = Color.Pink;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogManager.Log("Error", "error", "初始化公告比对值失败，失败原因：" + ex.Message);
            }

        }
        //check值改变事件
        private void checkChange(object sender, System.EventArgs e)
        {
            SetCheckChildStatue((CheckEdit)sender);
        }

        //根据check控件状态 处理其他控件状态
        private void SetCheckChildStatue(CheckEdit c)
        {
            try
            {
                if (checkControl.ContainsKey(c.Name))
                {
                    if (mapControl.ContainsKey(checkControl[c.Name]))
                    {
                        if (c.Checked)
                        {
                            mapControl[checkControl[c.Name]].Enabled = true;
                        }
                        else
                        {
                            mapControl[checkControl[c.Name]].Enabled = false;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogManager.Log("Error", "error", "根据check控件状态 处理其他控件状态失败，失败原因：" + ex.Message);
            }
        }
        //处理button点击事件
        private void buttonClick(object sender, System.EventArgs e)
        {
            try
            {
                if (((DevExpress.XtraEditors.SimpleButton)sender).Name.IndexOf("CHOOSE") != -1)
                {
                    //选择图片事件
                    ChoosePicture((DevExpress.XtraEditors.SimpleButton)sender);
                }
                else if (((DevExpress.XtraEditors.SimpleButton)sender).Name.IndexOf("VIEW") != -1)
                {
                    //预览pdf事件
                    ViewPicture((DevExpress.XtraEditors.SimpleButton)sender);
                }
            }
            catch (System.Exception ex)
            {
                LogManager.Log("Error", "error", "处理button点击事件失败，失败原因：" + ex.Message);
            }
            
        }
        //选择图片事件
        private void ChoosePicture(DevExpress.XtraEditors.SimpleButton btn)
        {
            OpenFileDialog ofd = new OpenFileDialog();
           // ofd.Filter = "Excel|*.xls;*.xlsx|All Files|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string forldername = btn.Name.Substring(0, btn.Name.LastIndexOf("BT"));
                    Control textControl = GetControl(forldername);
                    Control pictureControl = GetControl(forldername + "_PICTURE");
                    if (textControl != null && pictureControl!= null)
                    {
                  
                        //获取缩率图
                        String PictureFileName = ImageTool.MakeThumbnail(ofd.FileName, Utils.tempctreatepath + forldername + "\\");
                        if (!String.IsNullOrEmpty(PictureFileName))
                        {
                            //设置文件名称
                            textControl.Text = Utils.tempctreatepath + forldername + "\\" + PictureFileName;
                            //加载图片
                            Image img = Image.FromFile(Utils.tempctreatepath + forldername + "\\" + PictureFileName);
                            Image cloneImage = new Bitmap(img, pictureControl.Width, pictureControl.Height);
                            img.Dispose();
                            ((PictureEdit)pictureControl).Image = cloneImage;
                            //记录临时文件名称
                            if (tempPictureName.ContainsKey(forldername))
                            {
                                tempPictureName.Remove(forldername);
                            }
                            tempPictureName.Add(forldername, Utils.tempctreatepath + forldername + "\\" + PictureFileName);
                        }
                        else
                        {
                            MessageBox.Show("加载文件失败");
                        }
                      
                    }
                    
                }
                catch (System.Exception ex)
                {
                    LogManager.Log("Error", "error", "拷贝文件失败，失败原因：" + ex.Message);
                    MessageBox.Show("加载文件失败，失败原因："+ex.Message);
                }
                
            }
        }
        //预览PDF事件
        private void ViewPicture(DevExpress.XtraEditors.SimpleButton btn)
        {
            try
            {
                string forldername = btn.Name.Substring(0, btn.Name.LastIndexOf("BT"));
                
                    //获取控件
                    Control FPHMControl = GetControl("FPHM");
                    Control VinControl = GetControl("VIN");
                    Control textControl = GetControl(forldername);
                    if (FPHMControl!= null && VinControl != null&& textControl != null)
                    {
                        if (textControl.Text == "")
                        {
                            MessageBox.Show("请先选择图片后预览");
                            return;
                        }
                        string tempPicture = "";
                        if (tempPictureName.ContainsKey(forldername))
                        {
                            tempPicture = tempPictureName[forldername];
                        }
                        else
                        {
                            if (mapPictureName.ContainsKey(forldername+"_PICTURE"))
                            {
                                tempPicture = mapPictureName[forldername + "_PICTURE"];
                                if (forldername == "FPTP")
                                {
                                    tempPicture = String.Format("{0}\\{1}", Utils.billImage, tempPicture);
                                }
                                else if (forldername == "XSZTP")
                                {
                                    tempPicture = String.Format("{0}\\{1}", Utils.driveImage, tempPicture);
                                }
                            }
                        }
                        //生成PDF
                        Tool tool = new Tool();
                        tool.ImageConvertPDF(String.Format("{0}{1}\\{2}", Utils.tempviewpath, forldername, Utils.temppdfname), tempPicture, strCompany, FPHMControl.Text, VinControl.Text);
                        //打开PFD
                        System.Diagnostics.Process.Start(String.Format("{0}{1}\\{2}", Utils.tempviewpath, forldername, Utils.temppdfname));
                    }
            }
            catch (System.Exception ex)
            {
                LogManager.Log("Error", "error", "预览pdf失败，失败原因：" + ex.Message);
                MessageBox.Show("预览pdf失败，失败原因：" + ex.Message);
            }
            
        }
        //车辆种类更改
        private void CLZLValueChange(object sender, System.EventArgs e)
        {
            try
            {
                Control c_CLZL = GetControl("CLZL");
                Control c_CLYT = GetControl("CLYT");
                if (c_CLZL !=null&& c_CLYT!= null)
                {
                    ComboBoxEdit cbe_CLZL = (ComboBoxEdit)c_CLZL;
                    ComboBoxEdit cbe_CLYT = (ComboBoxEdit)c_CLYT;
                    cbe_CLYT.Properties.Items.Clear();
                    cbe_CLYT.Text = "";
                    if (cbe_CLZL.SelectedItem.ToString() == "插电式混合动力客车" || cbe_CLZL.SelectedItem.ToString() == "纯电动客车" || cbe_CLZL.SelectedItem.ToString() == "燃料电池客车")
                    {
                        cbe_CLYT.Properties.Items.Add((object)"公交");
                        cbe_CLYT.Properties.Items.Add((object)"通勤");
                        cbe_CLYT.Properties.Items.Add((object)"旅游");
                        cbe_CLYT.Properties.Items.Add((object)"公路");
                    }
                    else if (cbe_CLZL.SelectedItem.ToString() == "插电式混合动力乘用车" || cbe_CLZL.SelectedItem.ToString() == "纯电动乘用车" || cbe_CLZL.SelectedItem.ToString() == "燃料电池乘用车")
                    {
                        cbe_CLYT.Properties.Items.Add((object)"公务");
                        cbe_CLYT.Properties.Items.Add((object)"出租");
                        cbe_CLYT.Properties.Items.Add((object)"租赁");
                        cbe_CLYT.Properties.Items.Add((object)"私人");
                    }
                    else
                    {
                        if (!(cbe_CLZL.SelectedItem.ToString() == "纯电动特种车") && !(cbe_CLZL.SelectedItem.ToString() == "燃料电池货车"))
                            return;
                        cbe_CLYT.Properties.Items.Add((object)"邮政");
                        cbe_CLYT.Properties.Items.Add((object)"物流");
                        cbe_CLYT.Properties.Items.Add((object)"环卫");
                        cbe_CLYT.Properties.Items.Add((object)"工程");
                    }
                    CLYTValueOperate();
                }
                
            }
            catch (System.Exception ex)
            {
                LogManager.Log("Error", "error", "车辆种类更改失败，失败原因：" + ex.Message);
            }
           
        }
        //购车省份更改
        private void GCSFValueChange(object sender, System.EventArgs e)
        {
            GCSFValueOperate();
        }
        //选择省份 对应城市
        private void GCSFValueOperate()
        {
            try
            {

                Control c_GCSF = GetControl("GCSF");// (ComboBoxEdit)(this.tlp_base.Controls.Find("GCSF", false)[0]);
                Control c_GCCS = GetControl("GCCS"); //(ComboBoxEdit)(this.tlp_base.Controls.Find("GCCS", false)[0]);
                if (c_GCCS!= null && c_GCSF!= null)
                {
                    ComboBoxEdit cbe_GCSF = (ComboBoxEdit)c_GCSF;
                    ComboBoxEdit cbe_GCCS = (ComboBoxEdit)c_GCCS;
                    cbe_GCCS.Properties.Items.Clear();
                    cbe_GCCS.Text = "";
                    DataSet ds = new DataSet();
                    ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, String.Format("SELECT CITY_NAME FROM CITY WHERE    COUNTY_NAME = '{0}' ORDER BY CITY_ORDER ", cbe_GCSF.SelectedItem.ToString()), null);
                    DataTable dt = ds.Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        cbe_GCCS.Properties.Items.Add(dr["CITY_NAME"].ToString());
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogManager.Log("Error", "error", "省份更改失败，失败原因：" + ex.Message);
            }
           
        }
        //根据数据显示控件 bRet true 新增 
        private void ShowUpdataData(bool bRet)
        {
            try
            {
                
                foreach (var val in mapControl)
                {
                    Control c = val.Value;
                    if (c is TextEdit || c is DevExpress.XtraEditors.ComboBoxEdit || c is DevExpress.XtraEditors.DateEdit)
                    {
                        if (mapResult.ContainsKey(c.Name))
                        {
                            if (bRet || c.Name == "CLXZ" || c.Name == "CLZL" || c.Name == "GCSF" || c.Name == "GCCS" || c.Name == "CLYT" || c.Name == "FPSJ" || c.Name == "XSZSJ" || c.Name == "GMJG" 
                                || c.Name == "SQBZBZ" || c.Name == "CLXH" || c.Name == "CLYXDW")
                            {
                                c.Text = mapResult[c.Name];
                            }
                            else if (c.Name == "CJDRXX_CXXH" || c.Name == "CJDRXX_DRZRL" || c.Name == "CJDRXX_DRZSCQY" || c.Name == "CJDRXX_DTSCQY" || c.Name == "CJDRXX_DTXH" || c.Name == "CJDRXX_XTJG" || c.Name == "CJDRXX_ZBNX")
                            {
                                c.Text = mapResult[c.Name];
                            }
                            else if (c.Name == "DCDTXX_SCQY" || c.Name == "DCDTXX_XH" || c.Name == "DCZXX_SCQY" || c.Name == "DCZXX_XH" || c.Name == "DCZXX_XTJG" || c.Name == "DCZXX_ZBNX" || c.Name == "DCZXX_ZRL")
                            {
                                c.Text = mapResult[c.Name];
                            }
                            else if (c.Name == "QDDJXX_EDGL_1" || c.Name == "QDDJXX_EDGL_2" || c.Name == "QDDJXX_SCQY_1" || c.Name == "QDDJXX_SCQY_2" || c.Name == "QDDJXX_XH_1" || c.Name == "QDDJXX_XH_2" || c.Name == "QDDJXX_XTJG_1")
                            {
                                c.Text = mapResult[c.Name];
                            }
                            else if (c.Name == "QDDJXX_XTJG_2" || c.Name == "RLDCXX_EDGL" || c.Name == "RLDCXX_GMJG" || c.Name == "RLDCXX_SCQY" || c.Name == "RLDCXX_XH" || c.Name == "RLDCXX_ZBNX" )
                            {
                                c.Text = mapResult[c.Name];
                            } 
                            else
                            {
                                c.Text = "";
                            }
                        }
                        else
                        {
                            c.Text = "";
                        }
                    }
                    else if (c is CheckEdit)
                    {
                        if (mapResult.ContainsKey(c.Name))
                        {
                            if (bRet || c.Name == "CLSFYCJDR" || c.Name == "CLSFYRLDC"|| c.Name == "CLSFYQDDJ2")
                            {
                                if (mapResult[c.Name] == "是")
                                {
                                    ((CheckEdit)c).Checked = true;
                                }
                                else
                                {
                                    ((CheckEdit)c).Checked = false;
                                }
                            }
                            else
                            {
                                ((CheckEdit)c).Checked = false;
                            }
                        }
                        else
                        {
                            ((CheckEdit)c).Checked = false;
                        }
                        SetCheckChildStatue((CheckEdit)c);
                    }
                    else if (c is PictureEdit)
                    {
                        if (mapResult.ContainsKey(c.Name))
                        {
                            if (bRet)
                            {
                                string path = "";
                                if (c.Name == "FPTP_PICTURE")
                                {
                                    path = Utils.billImage + "\\" + mapResult[c.Name];
                                }
                                else
                                {
                                    path = Utils.driveImage + "\\" + mapResult[c.Name];
                                }
                                mapPictureName.Add(c.Name, mapResult[c.Name]);
                                try
                                {
                                    Image image = Image.FromFile(path);
                                    Image cloneImage = new Bitmap(image, c.Width, c.Height);
                                    ((PictureEdit)c).Image = cloneImage;
                                    image.Dispose();
                                }
                                catch (System.Exception ex)
                                {
                                    ((PictureEdit)c).Image = null;
                                }
                            }
                            else
                            {
                                ((PictureEdit)c).Image = null;
                            }
                        }
                        else
                        {
                            ((PictureEdit)c).Image = null;
                        }
                    }

                }
            }
            catch (System.Exception ex)
            {
                LogManager.Log("Error", "error", "根据数据显示控件失败，失败原因：" + ex.Message);
            }
            
        }
        //车辆用途更改
        private void CLYTValueChange(object sender, System.EventArgs e)
        {
            CLYTValueOperate();
        }
        //当车辆用途选择为私人时，车辆运行单位为私人
        private void CLYTValueOperate()
        {
            try
            {
                Control c_CLYT = GetControl("CLYT");
                Control c_CLYXDW = GetControl("CLYXDW");
                if (c_CLYT != null && c_CLYXDW != null)
                {
                    if (((ComboBoxEdit)c_CLYT).SelectedItem.ToString() == "私人")
                    {
                        c_CLYXDW.Text = "私人";
                        c_CLYXDW.Enabled = false;
                    }
                    else
                    {
                        c_CLYXDW.Text = string.Empty;
                        c_CLYXDW.Enabled = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogManager.Log("Error", "error", "车辆用途选择失败，失败原因：" + ex.Message);
            }
            
           
        }

        //根据控件名查询控件
        private Control GetControl(string controlName)
        {
            Control c = null;
            try
            {
                
                if (mapControl.ContainsKey(controlName))
                {
                    c = mapControl[controlName];
                }
            }
            catch (System.Exception ex)
            {
            	
            }
            
            return c;
        }
        //保存数据
        private string SaveData()
        {
            string msg = "";
            try
            {
                Control cvin = GetControl("VIN");
                if (cvin != null)
                {
                    string vin = ((TextEdit)cvin).Text.Trim().ToUpper();
                    if (vin != "" && !String.IsNullOrEmpty(vin))
                    {
                        //检查vin是否重复
                        if (AccessHelper.Exists(AccessHelper.conn, String.Format("SELECT count(*) FROM INFOMATION_ENTITIES WHERE VIN='{0}'", vin)))
                        {
                            throw new Exception("已存在该vin，无法新增");
                        }
                        //检查车牌是否重复
                        if (mapControl.ContainsKey("CLPZ"))
                        {
                            if (AccessHelper.Exists(AccessHelper.conn, String.Format("SELECT count(*) FROM INFOMATION_ENTITIES WHERE CLPZ='{0}'", mapControl["CLPZ"].Text.Trim())))
                            {
                                throw new Exception("已存在该车牌号，无法新增");
                            }
                        }

                        using (OleDbConnection con = new OleDbConnection(AccessHelper.conn))
                        {
                            con.Open();
                            OleDbTransaction tra = con.BeginTransaction(); //创建事务，开始执行事务

                            try
                            {
                                AccessHelper.ExecuteNonQuery(tra, String.Format("INSERT INTO INFOMATION_ENTITIES (VIN,JZNF) VALUES('{0}','{1}')", vin, clearYear));
                                foreach (var val in mapResult)
                                {
                                    if (val.Key != "FPTP" && val.Key != "XSZTP")
                                    {
                                        AccessHelper.ExecuteNonQuery(tra, String.Format("UPDATE INFOMATION_ENTITIES SET {0} = '{1}' WHERE VIN = '{2}'", val.Key, val.Value, vin));
                                    }
                                    else
                                    {
                                        if (tempPictureName.ContainsKey(val.Key))
                                        {
                                            string path = "";
                                            string fileName = "";
                                            string suffix = "";
                                            string strPictureName = "";
                                            SysIOHelper.GetFileNameInfo(tempPictureName[val.Key], ref  path, ref  fileName, ref  suffix);
                                            string strPicturePath = Utils.installPath;
                                            if (val.Key == "FPTP")
                                            {
                                                strPicturePath = Utils.billImage + "\\";
                                                strPictureName = String.Format("车辆发票VIN-{0}.{1}", vin, suffix);
                                            }
                                            else if (val.Key == "XSZTP")
                                            {
                                                strPicturePath = Utils.driveImage + "\\";
                                                strPictureName = String.Format("行驶证VIN-{0}.{1}", vin, suffix);
                                            }
                                            SysIOHelper.CopyFile(tempPictureName[val.Key], strPicturePath, strPictureName);
                                            AccessHelper.ExecuteNonQuery(tra, String.Format("UPDATE INFOMATION_ENTITIES SET {0}_PICTURE = '{1}' WHERE VIN = '{2}'", val.Key, strPictureName, vin));
                                            AccessHelper.ExecuteNonQuery(tra, String.Format("UPDATE INFOMATION_ENTITIES SET {0} = '{1}' WHERE VIN = '{2}'", val.Key, strPictureName, vin));
                                        }
                                    }
                                }
                                tra.Commit();
                            }
                            catch (System.Exception ex)
                            {
                                tra.Rollback();
                                throw ex;
                            }
                        }

                    }
                    else 
                    {
                        throw new Exception("vin不可为空");
                    }
                }
                else
                {
                    throw new Exception("未查找到vin控件");
                }
            }
            catch (System.Exception ex)
            {
                msg+= "保存失败，失败原因："+ex.Message;
            }
            return msg;
        }
        //更新数据
        private string UpdateData()
        {
            string msg = "";
            try
            {
                Control cvin = GetControl("VIN");
                if (cvin != null)
                {
                    string vin = ((TextEdit)cvin).Text.Trim().ToUpper();
                    if (vin != "" && !String.IsNullOrEmpty(vin))
                    {
                        //检查vin是否重复
                        if (!AccessHelper.Exists(AccessHelper.conn, String.Format("SELECT count(*) FROM INFOMATION_ENTITIES WHERE VIN='{0}'", vin)))
                        {
                            throw new Exception("不存在该vin，无法更新");
                        }
                        //检查车牌是否重复
                        if (mapControl.ContainsKey("CLPZ") && !mapControl["CLPZ"].Text.Trim().Equals(clpz_Final))
                        {
                            if (AccessHelper.Exists(AccessHelper.conn, String.Format("SELECT count(*) FROM INFOMATION_ENTITIES WHERE CLPZ='{0}'", mapControl["CLPZ"].Text.Trim())))
                            {
                                throw new Exception("已存在该车牌号，无法更新");
                            }
                        }

                        using (OleDbConnection con = new OleDbConnection(AccessHelper.conn))
                        {
                            con.Open();
                            OleDbTransaction tra = con.BeginTransaction(); //创建事务，开始执行事务

                            try
                            {
                                AccessHelper.ExecuteNonQuery(tra, String.Format("DELETE FROM INFOMATION_ENTITIES WHERE VIN = '{0}'", vin), null);
                                AccessHelper.ExecuteNonQuery(tra, String.Format("INSERT INTO INFOMATION_ENTITIES (VIN,JZNF) VALUES('{0}','{1}')", vin, clearYear));
                                foreach (var val in mapResult)
                                {
                                    if (val.Key != "FPTP" && val.Key != "XSZTP")
                                    {
                                        AccessHelper.ExecuteNonQuery(tra, String.Format("UPDATE INFOMATION_ENTITIES SET {0} = '{1}' WHERE VIN = '{2}'", val.Key, val.Value, vin));
                                    }
                                    else
                                    {
                                        if (tempPictureName.ContainsKey(val.Key))
                                        {
                                            string path = "";
                                            string fileName = "";
                                            string suffix = "";
                                            string strPictureName = "";
                                            SysIOHelper.GetFileNameInfo(tempPictureName[val.Key], ref  path, ref  fileName, ref  suffix);
                                            string strPicturePath = Utils.installPath;
                                            if (val.Key == "FPTP")
                                            {
                                                strPicturePath = Utils.billImage + "\\";
                                                strPictureName = String.Format("车辆发票VIN-{0}.{1}", vin, suffix);
                                            }
                                            else if (val.Key == "XSZTP")
                                            {
                                                strPicturePath = Utils.driveImage + "\\";
                                                strPictureName = String.Format("行驶证VIN-{0}.{1}", vin, suffix);
                                            }
                                            SysIOHelper.CopyFile(tempPictureName[val.Key], strPicturePath, strPictureName);
                                            AccessHelper.ExecuteNonQuery(tra, String.Format("UPDATE INFOMATION_ENTITIES SET {0}_PICTURE = '{1}' WHERE VIN = '{2}'", val.Key, strPictureName, vin));
                                            AccessHelper.ExecuteNonQuery(tra, String.Format("UPDATE INFOMATION_ENTITIES SET {0} = '{1}' WHERE VIN = '{2}'", val.Key, strPictureName, vin));
                                        }
                                        else if (!string.IsNullOrEmpty(val.Value))
                                        {
                                            AccessHelper.ExecuteNonQuery(tra, String.Format("UPDATE INFOMATION_ENTITIES SET {0}_PICTURE = '{1}' WHERE VIN = '{2}'", val.Key, val.Value, vin));
                                            AccessHelper.ExecuteNonQuery(tra, String.Format("UPDATE INFOMATION_ENTITIES SET {0} = '{1}' WHERE VIN = '{2}'", val.Key, val.Value, vin));
                                        }
                                    }
                                }
                                tra.Commit();
                            }
                            catch (System.Exception ex)
                            {
                                tra.Rollback();
                                throw ex;
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("vin不可为空");
                    }
                }
                else
                {
                    throw new Exception("未查找到vin控件");
                }

            }
            catch (System.Exception ex)
            {
                msg += "保存失败，失败原因：" + ex.Message;
            }
            return msg;
        }
        //保存并添加
        private void sbt_save_add_Click(object sender, EventArgs e)
        {
            string msg = "";
            GetValue();
            msg = ValidateParam.CheckParameter(mapResult);
            if (msg != "")
            {
                using (var mf = new MessageForm(msg) { Text = "校验结果" })
                {
                    mf.ShowDialog();
                }       
            }
            else
            {
                String strMsg = "";
                if (bMode)
                {
                    strMsg = SaveData();
                }
                else
                {
                    strMsg = UpdateData();
                }
                if (strMsg == "")
                {
                    MessageBox.Show("保存成功");
                    //显示保留参数
                    ShowUpdataData(false);
                    mapPictureName.Clear();

                }
                else
                {
                    MessageBox.Show(strMsg);
                }
                
               
            }
            mapResult.Clear();
        }

        private void sbt_save_exit_Click(object sender, EventArgs e)
        {
            string msg = "";
            GetValue();
            msg = ValidateParam.CheckParameter(mapResult);
            if (msg != "")
            {
                using (var mf = new MessageForm(msg) { Text = "校验结果" })
                {
                    mf.ShowDialog();
                } 
            }
            else
            {
                String strMsg = "";
                if (bMode)
                {
                    strMsg = SaveData();
                }
                else
                {
                    strMsg = UpdateData();
                }
                if (strMsg == "")
                {
                    MessageBox.Show("保存成功");
                    mapPictureName.Clear();
                    this.Close();
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show(strMsg);
                }
               
               
            }
            mapResult.Clear();
        }

        private void sbt_cancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void GetValue()
        {
            List<String> lsCheckControl = new List<String>();
            foreach(var val in mapControl)
            {
                Control c = val.Value;
                if (c is TextEdit || c is DevExpress.XtraEditors.ComboBoxEdit || c is DevExpress.XtraEditors.DateEdit)
                {
                    mapResult.Add(c.Name, c.Text.Trim());
                    
                }
                else if (val.Value is CheckEdit)
                {
                    string strCheck = "否";
                    if (((CheckEdit)c).Checked)
                    {
                        strCheck = "是";
                    }
                    else
                    {
                        lsCheckControl.Add(c.Name);
                    }
                    mapResult.Add(c.Name, strCheck);
                }
                
            }
            foreach (string checkname in lsCheckControl)
            {
                DataSet dsChild = new DataSet();
                if (checkControl.ContainsKey(checkname))
                {
                    if (mapControl.ContainsKey(checkControl[checkname]))
                    {
                        Control c = mapControl[checkControl[checkname]];
                        if (c is TextEdit)
                        {
                            if (mapResult.ContainsKey(c.Name))
                            {
                                mapResult.Remove(c.Name);
                            }
                        }
                        else if (c is GroupBox)
                        {
                            DataSet dsGroup = new DataSet();
                            dsGroup = AccessHelper.ExecuteDataSet(AccessHelper.conn, String.Format("SELECT PARAM_CODE, PARAM_NAME, PARAM_REMARK,CONTROL_TYPE,CONTROL_VALUE ,ORDER_RULE,PARENT_PARAM ,PARAM_DEFAULT  FROM CONFIG_INFOMATION WHERE  (STATUS = '1' AND PARENT_PARAM='{0}')   ORDER BY ORDER_RULE ", c.Name), null);
                            DataTable dtGroup = dsGroup.Tables[0];
                            foreach (DataRow drGroup in dtGroup.Rows)
                            {
                                if (mapResult.ContainsKey(drGroup["PARAM_CODE"].ToString()))
                                {
                                    mapResult.Remove(drGroup["PARAM_CODE"].ToString());
                                }
                            }
                        }
                    }


                }
            }
        }
        //鼠标离开text控件
        private void te_MouseLeave(object sender, EventArgs e)
        {
            toolTipController1.HideHint();
        }

        //进入Text控件 显示tooltip
        private void te_MouseEnter(object sender, EventArgs e)
        {
            
            if (mapRightData.ContainsKey(((Control)sender).Name))
            {
                toolTipController1.ShowHint(mapRightData[((Control)sender).Name]);
            }

        }

        private void bt_view_Click(object sender, EventArgs e)
        {
            try
            {
                if (mapControl.ContainsKey("CLXH"))
                {
                    if (!String.IsNullOrEmpty(mapControl["CLXH"].Text))
                    {
                        DataSet ds = new DataSet();
                        ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, String.Format("SELECT MODEL_VEHICLE,DATASOURCE,BATCH,MODEL_SINGLE,MFRS_SINGLE,MODEL_WHOLE,CAPACITY_BAT,MFRS_BAT,MODEL_DRIVE,RATEPOW_DRIVE,MFRS_DRIVE,MDEL_FUEL,RATEPOW_FUEL,MFRS_FUEL FROM ANNOUNCE_PARAM WHERE MODEL_VEHICLE='{0}' ", mapControl["CLXH"].Text), null);
                        if (ds.Tables[0].Rows.Count != 0)
                        {
                            ChooseConfigInfoForm dlg = new ChooseConfigInfoForm(ds.Tables[0]);
                            if (dlg.ShowDialog() == DialogResult.OK)
                            {
                                ShowCLXHInfomation(dlg.mapCLXHData);
                            }
                        }
                        else
                        {
                            MessageBox.Show("未找到该车型信息");
                        }
                    }
                    else
                    {
                        MessageBox.Show("车型信息未填写，无法引用申报配置信息");

                    }
                }
                else
                {

                }
            }
            catch (System.Exception ex)
            {
            	
            }
            
        }
        private void ShowCLXHInfomation(Dictionary<string,string> mapCLXHData)
        {
            foreach (var val in mapCLXHData)
            {
                if (mapControl.ContainsKey(val.Key))
                {
                    if (val.Key == "CLSFYRLDC")
                    {
                        if (val.Value == "是")
                        {
                            ((CheckEdit)mapControl[val.Key]).Checked = true;
                        }
                        else
                        {
                            ((CheckEdit)mapControl[val.Key]).Checked = false;
                        }
                    }
                    else
                    {
                        mapControl[val.Key].Text = val.Value;
                    }
                    
                }
            }
        }

        private void tc_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (Settings.Default.AuthorityUrl.Equals(@"\Template\Authority.XML"))
            {
                if (e.Page.Text == "车辆配置信息")
                {
                    bt_view.Visible = true;
                }
                else
                {
                    bt_view.Visible = false;
                }
            }
            
        }
        

    }
}
