using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Catarc.Adc.NewEnergyApproveSys.DBUtils;
using Catarc.Adc.NewEnergyApproveSys.LogUtils;
using Catarc.Adc.NewEnergyApproveSys.Common;
using DevExpress.XtraEditors.Controls;
using System.IO;
using Catarc.Adc.NewEnergyApproveSys.PopForm;

namespace Catarc.Adc.NewEnergyApproveSys.Form_WorkManage
{
    public partial class SingleInfoForm : DevExpress.XtraEditors.XtraForm
    {
        readonly Dictionary<string, Control> mapControl = new Dictionary<string, Control>();//控件列表
        readonly Dictionary<string, string> mapRightData = new Dictionary<string, string>();//与公告比对值不同的列表  需要显示tooltip控件的名称与显示值
        readonly Dictionary<string, string> mapResult = new Dictionary<string, string>();//控件显示值

        public SingleInfoForm()
        {
            InitializeComponent();
        }

        public SingleInfoForm(Dictionary<string, string> mapResult, Dictionary<string, string> mapRightData)
        {
            InitializeComponent();
            this.mapRightData = mapRightData;
            this.mapResult = mapResult;
        }

        private void SingleInfoForm_Load(object sender, EventArgs e)
        {
            //画基本信息tab页
            drawBaseInfomation();
            //画配置信息tab页
            drawConfigInfomation();
            //画运行信息tab页
            drawWorkingInfomation();
            //显示数据
            ShowUpdataData();
            //与公告比对值不同的控件
            drawRightData();
        }

        //画基本信息tab页
        void drawBaseInfomation()
        {
            try
            {
                DataSet ds = new DataSet();
                ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, "SELECT PARAM_CODE, PARAM_NAME, PARAM_REMARK,CONTROL_TYPE,CONTROL_VALUE ,ORDER_RULE,PARENT_PARAM ,PARAM_DEFAULT FROM SYS_CONTROLLERS WHERE STATUS = '1' AND PARAM_TYPE = '车辆基本信息' ORDER BY ORDER_RULE ");
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
                        tb.Properties.ReadOnly = true;
                        tb.MouseEnter += te_MouseEnter;
                        tb.MouseLeave += te_MouseLeave;
                        mapControl.Add(dr["PARAM_CODE"].ToString(), tb);
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
                        cbe.Properties.ReadOnly = true;
                        cbe.Properties.Items.AddRange(dr["CONTROL_VALUE"].ToString().Split('/'));
                        cbe.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                        Label lbll = new Label() { Width = 10, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                        this.tlp_base.Controls.Add(lbl);
                        this.tlp_base.Controls.Add(cbe);
                        this.tlp_base.Controls.Add(lbll);
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
                        DevExpress.XtraEditors.DateEdit cbe = new DateEdit() { Width = 250, Height = 28, Name = dr["PARAM_CODE"].ToString(), Enabled = false };
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
                        this.tlp_base.Controls.Add(lbl);
                        this.tlp_base.Controls.Add(cbe);
                        this.tlp_base.Controls.Add(lbll);
                        cbe.Text = "";
                        //双击打开大图
                        cbe.MouseDoubleClick += pe_MouseDoubClick;

                        mapControl.Add(dr["PARAM_CODE"].ToString(), cbe);
                    }
                    if (dr["CONTROL_TYPE"].ToString() == "BUTTON")
                    {
                        Label lbl = new Label() { Width = 150, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
                        DevExpress.XtraEditors.PanelControl pcl = new DevExpress.XtraEditors.PanelControl() { Width = 250, Height = 30, BorderStyle = BorderStyles.NoBorder };
                        Label lbll = new Label() { Width = 10, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
                        this.tlp_base.Controls.Add(lbl);
                        this.tlp_base.Controls.Add(pcl);
                        this.tlp_base.Controls.Add(lbll);
                    }
                    tlp_base.Location = new Point(10, 15);
                }
            }
            catch (Exception ex)
            {
                LogManager.Log("Error", "SingleInfoForm", "初始化基本信息失败，失败原因：" + ex.Message);
            }
        }

        //画配置信息tab页
        void drawConfigInfomation()
        {
            try
            {
                List<CheckEdit> lsCheckEdit = new List<CheckEdit>();//记录checkEdit控件
                //初始位置
                int x_1 = 15;
                int x_2 = 450;
                int y_1 = 15;
                int y_2 = 15;
                //查询控件
                DataSet ds = new DataSet();
                ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, "SELECT PARAM_CODE, PARAM_NAME, PARAM_REMARK,CONTROL_TYPE,CONTROL_VALUE ,ORDER_RULE,PARENT_PARAM  ,PARENT_CONTROL ,PARAM_DEFAULT FROM SYS_CONTROLLERS WHERE  (STATUS = '1' AND PARENT_PARAM is null AND PARAM_TYPE = '车辆配置信息' ) ORDER BY ORDER_RULE ");
                DataTable dt = ds.Tables[0];
                //遍历控件
                foreach (DataRow dr in dt.Rows)
                {
                    //check类型
                    if (dr["CONTROL_TYPE"].ToString() == "CHECKBOX")
                    {
                        DevExpress.XtraEditors.CheckEdit cbe = new CheckEdit() { Width = 250, Height = 24, Name = dr["PARAM_CODE"].ToString(), Text = dr["PARAM_NAME"].ToString() };
                        cbe.Properties.ReadOnly = true;
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
                        dsGroup = OracleHelper.ExecuteDataSet(OracleHelper.conn, String.Format("SELECT PARAM_CODE, PARAM_NAME, PARAM_REMARK,CONTROL_TYPE,CONTROL_VALUE ,ORDER_RULE,PARENT_PARAM ,PARAM_DEFAULT  FROM SYS_CONTROLLERS WHERE  (STATUS = '1' AND PARENT_PARAM='{0}' AND PARAM_TYPE = '车辆配置信息' ) ORDER BY ORDER_RULE ", dr["PARAM_CODE"]));
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
                                tb.Properties.ReadOnly = true;
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
                                tb.MouseEnter += te_MouseEnter;
                                tb.MouseLeave += te_MouseLeave;
                                mapControl.Add(drGroup["PARAM_CODE"].ToString(), tb);
                            }
                            // OPTION类型
                            if (drGroup["CONTROL_TYPE"].ToString() == "OPTION")
                            {
                                Label lbl = new Label() { Width = 120, Height = 25, Name = "lbl" + drGroup["PARAM_CODE"], Text = drGroup["PARAM_NAME"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleRight };
                                DevExpress.XtraEditors.ComboBoxEdit cbe = new ComboBoxEdit() { Width = 250, Height = 24, Name = drGroup["PARAM_CODE"].ToString() };
                                cbe.Properties.ReadOnly = true;
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
            }
            catch (Exception ex)
            {
                LogManager.Log("Error", "SingleInfoForm", "初始化配置信息失败，失败原因：" + ex.Message);
            }
        }

        //画运行信息tab页
        void drawWorkingInfomation()
        {
            try
            {
                List<CheckEdit> lsCheckEdit = new List<CheckEdit>();
                //获取运行信息界面控件
                DataSet ds = new DataSet();
                ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, "SELECT PARAM_CODE, PARAM_NAME, PARAM_REMARK,CONTROL_TYPE,CONTROL_VALUE ,ORDER_RULE,PARENT_PARAM ,PARAM_DEFAULT FROM SYS_CONTROLLERS WHERE    STATUS = '1' AND PARAM_TYPE = '车辆运行信息' ORDER BY ORDER_RULE ");
                DataTable dt = ds.Tables[0];
                //遍历控件
                foreach (DataRow dr in dt.Rows)
                {
                    //如果lab类型
                    if (dr["CONTROL_TYPE"].ToString() == "LAB")
                    {
                        Label lbl1 = new Label() { Width = 240, Height = 30, Name = dr["PARAM_CODE"].ToString(), Text = "截止至2016年底", TextAlign = System.Drawing.ContentAlignment.MiddleRight };
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
                        mapControl.Add(dr["PARAM_CODE"].ToString(), lbl1);
                    }
                    //Text类型
                    if (dr["CONTROL_TYPE"].ToString() == "TEXT")
                    {
                        Label lbl = new Label() { Width = 240, Height = 30, Name = "lbl" + dr["PARAM_CODE"], Text = dr["PARAM_NAME"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleRight };
                        TextEdit tb = new TextEdit() { Width = 180, Height = 28, Name = dr["PARAM_CODE"].ToString() };
                        tb.Properties.ReadOnly = true;
                        Label lbll = new Label() { Width = 10, Height = 30, Text = dr["PARAM_REMARK"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                        this.tlp.Controls.Add(lbl);
                        this.tlp.Controls.Add(tb);
                        this.tlp.Controls.Add(lbll);
                        mapControl.Add(dr["PARAM_CODE"].ToString(), tb);
                        tb.MouseEnter += te_MouseEnter;
                        tb.MouseLeave += te_MouseLeave;
                    }
                    // OPTION类型
                    if (dr["CONTROL_TYPE"].ToString() == "OPTION")
                    {
                        Label lbl = new Label() { Width = 240, Height = 30, Name = "lbl" + dr["PARAM_CODE"], Text = dr["PARAM_NAME"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleRight };
                        DevExpress.XtraEditors.ComboBoxEdit cbe = new ComboBoxEdit() { Width = 180, Height = 28, Name = dr["PARAM_CODE"].ToString() };
                        cbe.Properties.ReadOnly = true;
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
                        cbe.Properties.ReadOnly = true;
                        cbe.Dock = System.Windows.Forms.DockStyle.Right;
                        Label lbll = new Label() { Width = 10, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                        this.tlp.Controls.Add(cbe);
                        this.tlp.Controls.Add(lbl);
                        this.tlp.Controls.Add(lbll);
                        lsCheckEdit.Add(cbe);
                        mapControl.Add(dr["PARAM_CODE"].ToString(), cbe);
                    }
                    tlp.Location = new Point(10, 15);
                }
            }
            catch (Exception ex)
            {
                LogManager.Log("Error", "SingleInfoForm", "初始化运行信息失败，失败原因：" + ex.Message);
            }
        }

        //根据数据显示控件 bRet true 新增 
        private void ShowUpdataData()
        {
            try
            {
                foreach (var val in mapControl)
                {
                    Control c = val.Value;
                    if (c is Label)
                    {
                        if (c.Name.Equals("JZNF"))
                        {
                            c.Text = string.Format("截止至{0}", mapResult[c.Name]);
                        }
                    }
                    else if (c is DevExpress.XtraEditors.DateEdit)
                    {
                        if (mapResult.ContainsKey(c.Name))
                        {
                            c.Text = Convert.ToDateTime(mapResult[c.Name]).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            c.Text = "";
                        }
                    }
                    else if (c is TextEdit || c is DevExpress.XtraEditors.ComboBoxEdit)
                    {
                        if (mapResult.ContainsKey(c.Name))
                        {
                            c.Text = mapResult[c.Name];
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
                    else if (c is PictureEdit)
                    {
                        if (mapResult.ContainsKey(c.Name))
                        {
                            string path = "";
                            if (c.Name == "FPTP_PICTURE")
                            {
                                path = String.Format("{0}\\{1}", ApplicationFolder.billImage, mapResult[c.Name]);
                            }
                            else
                            {
                                path = String.Format("{0}\\{1}", ApplicationFolder.driveImage, mapResult[c.Name]);
                            }
                            try
                            {
                                if (!File.Exists(path))
                                {
                                    if (c.Name == "FPTP_PICTURE")
                                    {
                                        ApplicationFolder.FtpDownload(String.Format("/IMAGEBill/{0}/{1}", mapResult["CLSCQY"], mapResult[c.Name]), path, true);
                                    }
                                    else
                                    {
                                        ApplicationFolder.FtpDownload(String.Format("/IMAGEDrive/{0}/{1}", mapResult["CLSCQY"], mapResult[c.Name]), path, true);
                                    }
                                }

                                Image image = Image.FromFile(path);
                                Image cloneImage = new Bitmap(image, c.Width, c.Height);
                                ((PictureEdit)c).Image = cloneImage;
                                image.Dispose();
                            }
                            catch (Exception ex)
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
            catch (Exception ex)
            {
                LogManager.Log("Error", "SingleInfoForm", "根据数据显示控件失败，失败原因：" + ex.Message);
            }
        }
        
        //与公告比对值不同的控件
        void drawRightData()
        {
            try
            {
                foreach (var val in mapRightData)
                {
                    if (mapControl.ContainsKey(val.Key))
                    {
                        mapControl[val.Key].BackColor = Color.Pink;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Log("Error", "SingleInfoForm", "初始化公告比对值失败，失败原因：" + ex.Message);
            }

        }

        //进入Text控件 显示tooltip
        private void te_MouseEnter(object sender, EventArgs e)
        {
            if (mapRightData.ContainsKey(((Control)sender).Name))
            {
                toolTipController1.ShowHint(mapRightData[((Control)sender).Name]);
            }
        }

        //鼠标离开text控件
        private void te_MouseLeave(object sender, EventArgs e)
        {
            toolTipController1.HideHint();
        }

        //关闭
        private void sbt_cancle_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        //图片双击打开大图
        private void pe_MouseDoubClick(object sender, EventArgs e)
        {
            string name = ((Control)sender).Name;
            string path = string.Empty;
            if (name == "FPTP_PICTURE")
            {
                path = String.Format("{0}\\{1}", ApplicationFolder.billImage, mapResult[name]);
            }
            else
            {
                path = String.Format("{0}\\{1}", ApplicationFolder.driveImage, mapResult[name]);
            }
            try
            {
                Image image = Image.FromFile(path);
                Image cloneImage = new Bitmap(image, image.Width, image.Height);
                using (var mf = new BigPictureForm(image))
                {
                    mf.ShowDialog();
                }
                image.Dispose();
            }
            catch (Exception ex)
            {
                LogManager.Log("Error", "SingleInfoForm", String.Format("双击放大图片{0}", ex.Message));
            }
        }
    }
}