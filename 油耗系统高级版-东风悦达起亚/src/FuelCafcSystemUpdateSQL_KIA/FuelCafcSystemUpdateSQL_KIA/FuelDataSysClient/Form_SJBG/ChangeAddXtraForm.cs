using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using FuelDataSysClient.Model;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Word;
using SData = System.Data;
using System.Data.OleDb;
using FuelDataSysClient.Tool;
using System.IO;
using System.Threading;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;

namespace FuelDataSysClient.Form_SJBG
{
    public partial class ChangeAddXtraForm : DevExpress.XtraEditors.XtraForm
    {
        public string did = string.Empty;
        public string guid = string.Empty;
        public bool import = false;
        private string templatePath = System.Windows.Forms.Application.StartupPath + @"\ExcelHeaderTemplate\Change.docx";
        FuelFileUpload.FileUploadService service = Utils.serviceFiel;

        public ChangeAddXtraForm()
        {
            InitializeComponent();
        }

        public ChangeAddXtraForm(string did)
        {
            InitializeComponent();
            this.did = did;
            LoadData(did);
        }

        private void ChangeAddXtraForm_Load(object sender, EventArgs e)
        {
            QCSCQYMC.Text = Utils.qymc;
            APPLYDATA.Text = DateTime.Now.ToString("yyyy-MM-dd");
            guid = Guid.NewGuid().ToString();
            if (import)
            {
                this.INSERT_O.Enabled = false;
                this.UPDATE.Enabled = false;
                this.DELETE.Enabled = false;
            }
        }

        //初始化数据
        public void LoadData(string did)
        {
            if (!string.IsNullOrEmpty(did))
            {
                this.simpleButton1.Visible = false;
                this.simpleButton2.Visible = false;
                this.simpleButton3.Visible = false;
                this.simpleButton4.Visible = false;
            }
            DataSet dsBase = OracleHelper.ExecuteDataSet(OracleHelper.conn, String.Format("select * from DATA_CHANGE_BASE where ID='{0}'", did), null);
            if (dsBase != null && dsBase.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow r in dsBase.Tables[0].Rows)
                {
                    this.QCSCQYMC.Text = Convert.ToString(r["QCSCQY"]);
                    APPLYDATA.Text = Convert.ToString(r["APPLYDATA"]);
                    string applyType = Convert.ToString(r["APPLYTYPE"]);
                    var temp = applyType.Split('-');
                    foreach (string str in temp)
                    {
                        if (str == "补传")
                        {
                            this.INSERT_O.Checked = true;
                        }
                        else if (str == "修改")
                        {
                            this.UPDATE.Checked = true;
                        }
                        else
                        {
                            this.DELETE.Checked = true;
                        }
                    }
                    INSERT_O_RANGE.Text = Convert.ToString(r["INSERT_O_DATARANGE"]);
                    UPDATE_RANGE.Text = Convert.ToString(r["UPDATE_DATARANGE"]);
                    DELETE_RANGE.Text = Convert.ToString(r["DELETE_DATARANGE"]);
                    INSERT_O_SL.Text = Convert.ToString(r["INSERT_O_SL"]);
                    UPDATE_SL.Text = Convert.ToString(r["UPDATE_SL"]);
                    DELETE_SL.Text = Convert.ToString(r["DELETE_SL"]);
                    REASON.Text = Convert.ToString(r["REASON"]);
                    REMARK.Text = Convert.ToString(r["REMARK"]);
                }
            }
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, String.Format("select * from DATA_CHANGE where DID='{0}'", did), null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                System.Data.DataTable dt = ds.Tables[0];
                dt.Columns["RLZL"].ColumnName = "RLLX";
                dt.Columns["BSQSX"].ColumnName = "CT_BSQXS";
                gridControl1.DataSource = dt;
                var insert_o = ds.Tables[0].Select("APPLYTYPE='补传'");
                var update = ds.Tables[0].Select("APPLYTYPE='修改'");
                var delete = ds.Tables[0].Select("APPLYTYPE='撤销'");
                INSERT_O_SL.Text = insert_o.Length.ToString();
                UPDATE_SL.Text = update.Length.ToString();
                DELETE_SL.Text = delete.Length.ToString();
            }

        }

        //查询窗口添加数据调用
        public void LoadDataByVin(DataView vins)
        {
            try
            {
                if (vins != null && vins.Table.Rows.Count > 0)
                {
                    System.Data.DataTable dt = (System.Data.DataTable)gridControl1.DataSource;
                    if (dt != null)
                    {
                        dt.Merge(vins.Table);
                        string[] strComuns = { "VIN", "CLXH", "TYMC", "RLLX", "CT_ZHGKRLXHL", "ZCZBZL", "CT_BSQXS", "ZWPS", "UPDATEFIELD", "FIELDOLD", "FIELDNEW", "APPLYTYPE" };
                        dt = dt.AsDataView().ToTable(true, strComuns);
                        gridControl1.DataSource = dt;
                    }
                    else
                    {
                        gridControl1.DataSource = vins.ToTable();
                    }
                    System.Data.DataView dv = ((System.Data.DataTable)gridControl1.DataSource).AsDataView();
                    var insert_o = dv.ToTable(true, "VIN", "APPLYTYPE").Select("APPLYTYPE='补传'");
                    var update = dv.ToTable(true, "VIN", "APPLYTYPE").Select("APPLYTYPE='修改'");
                    var delete = dv.ToTable(true, "VIN", "APPLYTYPE").Select("APPLYTYPE='撤销'");
                    INSERT_O_SL.Text = insert_o.Length.ToString();
                    UPDATE_SL.Text = update.Length.ToString();
                    DELETE_SL.Text = delete.Length.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //关闭
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            ChangeRibbonForm crf = new ChangeRibbonForm();
            this.Close();
            crf.LoadData();
        }

        //调用查询窗体-传递初始化变量
        private void Show(CheckEdit ck, string type, string text, string applyType)
        {
            if (string.IsNullOrEmpty(did))
            {
                var check = ck;
                if (check.Checked)
                {
                    ChangeQuery cq = new ChangeQuery(type, guid);
                    cq.Owner = this;
                    cq.Text = text;
                    cq.Show();
                }
                else
                {
                    if (this.gridControl1.DataSource == null) return;
                    OracleHelper.ExecuteNonQuery(OracleHelper.conn, String.Format("delete from DATA_CHANGE where DID='{0}' and APPLYTYPE='{1}'", guid, applyType), null);
                    System.Data.DataView dv = ((System.Data.DataTable)gridControl1.DataSource).AsDataView();
                    if (dv != null)
                    {
                        var dvRow = dv.Table.Select(String.Format("APPLYTYPE='{0}'", applyType));
                        foreach (var r in dvRow)
                        {
                            dv.Table.Rows.Remove(r);
                        }
                        if (dv.Table.Rows.Count > 0)
                        {
                            gridControl1.DataSource = dv.Table;
                        }
                        else
                        {
                            gridControl1.DataSource = null;
                        }
                        var insert_o = dv.Table.Select("APPLYTYPE='补传'");
                        var update = dv.Table.Select("APPLYTYPE='修改'");
                        var delete = dv.Table.Select("APPLYTYPE='撤销'");
                        INSERT_O_SL.Text = insert_o.Length.ToString();
                        UPDATE_SL.Text = update.Length.ToString();
                        DELETE_SL.Text = delete.Length.ToString();
                    }

                }

            }
        }

        //补传数据
        private void INSERT_O_CheckedChanged(object sender, EventArgs e)
        {
            var check = (CheckEdit)sender;
            Show(check, "1", "筛选补传数据", "补传");
        }

        //修改数据
        private void UPDATE_CheckedChanged(object sender, EventArgs e)
        {
            var check = (CheckEdit)sender;
            Show(check, "2", "筛选修改数据", "修改");
        }

        //撤销数据
        private void DELETE_CheckedChanged(object sender, EventArgs e)
        {
            var check = (CheckEdit)sender;
            Show(check, "0", "筛选撤销数据", "撤销");
        }

        //保存变更申请单
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            string applyType = string.Empty;
            if (INSERT_O.Checked)
            {
                applyType = INSERT_O.Text + "-";
            }
            if (UPDATE.Checked)
            {
                applyType += UPDATE.Text + "-";
            }
            if (DELETE.Checked)
            {
                applyType += DELETE.Text + "-";
            }
            applyType = applyType.TrimEnd('-');
            if (gridControl1.DataSource == null)
            {
                MessageBox.Show("没有要保存的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            System.Data.DataView dv = ((System.Data.DataTable)gridControl1.DataSource).AsDataView();
            if (dv == null || dv.Table.Select("VIN not is null or VIN <> ''").Length < 1)
            {
                MessageBox.Show("没有要保存的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (OracleConnection con = new OracleConnection(OracleHelper.conn))
            {
                con.Open();
                OracleTransaction tra = con.BeginTransaction();
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    string delsql = string.Format("delete from DATA_CHANGE_BASE where ID='{0}'", guid);
                    int val = OracleHelper.ExecuteNonQuery(tra, delsql, null);
                    string sql = string.Format("insert into DATA_CHANGE_BASE values('{11}','{0}',to_date('{1}','yyyy-mm-dd hh24:mi:ss'),'{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}' )", this.QCSCQYMC.Text, this.APPLYDATA.Text, applyType, INSERT_O_RANGE.Text, UPDATE_RANGE.Text, DELETE_RANGE.Text, INSERT_O_SL.Text, UPDATE_SL.Text, DELETE_SL.Text, REASON.Text, REMARK.Text, guid);
                    OracleHelper.ExecuteNonQuery(tra, sql, null);

                    string delsqldata = string.Format("delete from DATA_CHANGE where DID='{0}'", guid);
                    OracleHelper.ExecuteNonQuery(tra, delsqldata, null);
                    foreach (DataRow r in dv.Table.Rows)
                    {
                        string sqlData = "insert into DATA_CHANGE values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')";
                        string sqlDataStr = string.Format(sqlData, guid, r["VIN"], r["CLXH"], r["TYMC"], r["rllx"], r["CT_ZHGKRLXHL"], r["ZCZBZL"], r["CT_BSQXS"], r["ZWPS"], r["UPDATEFIELD"], r["FIELDOLD"], r["FIELDNEW"], r["APPLYTYPE"]);
                        OracleHelper.ExecuteNonQuery(tra, sqlDataStr, null);
                    }
                    tra.Commit();
                    MessageBox.Show("操作成功,可以生成WORD或发送管理员", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    tra.Rollback();
                    MessageBox.Show("操作失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
            }
        }

        //生成Word
        public void simpleButton2_Click(object sender, EventArgs e)
        {
            var data = InitDatas();
            if (data != null)
            {
                using (SaveFileDialog sfd = new SaveFileDialog() { FileName = "变更申请表 " + data["{APPLYDATA}"], Title = "下载表更申请表Word", Filter = "word files(*.docx)|*.docx|word files(*.doc)|*.doc" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            SplashScreenManager.ShowForm(typeof(DevWaitForm));
                            System.Data.DataTable dt = (System.Data.DataTable)this.gridControl1.DataSource;
                            string imagePath = this.downLoadSignature();
                            createWord(templatePath, imagePath, data, sfd.FileName, dt);
                            if (MessageBox.Show("是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            {
                                System.Diagnostics.Process.Start(sfd.FileName);
                            }
                        }
                        catch (Exception msg)
                        {
                            MessageBox.Show(msg.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        finally
                        {
                            SplashScreenManager.CloseForm();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("请您确认是否已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //发送管理员
        public void simpleButton4_Click(object sender, EventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                //是否保存
                var data = InitDatas();
                if (data != null)
                {
                    //是否插入加密狗
                    string dogKey = DogAction.DogHelper.ReadData();
                    if (dogKey.Length > 20)
                    {
                        //加密狗是否通过
                        string strKey = Utils.userId;// +Utils.password;
                        if (strKey.Equals(dogKey.Substring(0, 13)))
                        {
                            //电子签章是否通过
                            string imagePath = downLoadSignature();
                            if (!string.IsNullOrEmpty(imagePath))
                            {
                                //其他申请是否通过
                                bool flg = checkOtherApply();
                                if (flg)
                                {
                                    bool flag = sendChangeDoc(data, imagePath);
                                    if (flag)
                                    {
                                        MessageBox.Show("发送成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    else
                                    {
                                        MessageBox.Show("发送失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("有变更申请单尚未通过！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                            else
                            {
                                MessageBox.Show("电子签章未通过审核！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("加密狗错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show(dogKey, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("请先保存！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        // 初始化表头数据
        private Dictionary<string, string> InitDatas()
        {
            string sql = string.Empty;
            if (string.IsNullOrEmpty(did))
            {
                sql = String.Format("select * from DATA_CHANGE_BASE where ID='{0}'", guid);
            }
            else
            {
                sql = String.Format("select * from DATA_CHANGE_BASE where ID='{0}'", did);
            }
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                Dictionary<string, string> datas = new Dictionary<string, string>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    datas.Add("{QCSCQY}", r["QCSCQY"].ToString());
                    datas.Add("{APPLYDATA}", Convert.ToDateTime(r["APPLYDATA"].ToString()).ToString("yyyy-MM-dd"));
                    datas.Add("{INSERT_O_SL}", r["INSERT_O_SL"].ToString());
                    datas.Add("{INSERT_O_DATARANGE}", r["INSERT_O_DATARANGE"].ToString());
                    datas.Add("{UPDATE_SL}", r["UPDATE_SL"].ToString());
                    datas.Add("{UPDATE_DATARANGE}", r["UPDATE_DATARANGE"].ToString());
                    datas.Add("{DELETE_SL}", r["DELETE_SL"].ToString());
                    datas.Add("{DELETE_DATARANGE}", r["DELETE_DATARANGE"].ToString());
                    datas.Add("{REASON}", r["REASON"].ToString());
                    datas.Add("{REMARK}", r["REMARK"].ToString());
                }
                return datas;
            }
            else
            {
                return null;
            }
        }

        //生成Word
        private static void createWord(string TemplatePath, string imagePath, Dictionary<string, string> dic, string SavePath, System.Data.DataTable dt)
        {
            WordHelper wordBuilder = new WordHelper();
            wordBuilder.CreateNewDocument(TemplatePath);
            foreach (var item in dic)
            {
                wordBuilder.InsertReplaceText(item.Key, item.Value);
            }
            if (!string.IsNullOrEmpty(imagePath))
            {
                wordBuilder.InsertPicture("BOOKMARK_SIGNATURE", imagePath, 100, 100);
            }
            Microsoft.Office.Interop.Word.Table table = wordBuilder.InsertTable("BOOKMARK_TABLE", dt.Rows.Count + 1, 10, 0);
            wordBuilder.SetParagraph_Table(table, 0, 0);
            wordBuilder.SetFont_Table(table, string.Empty, 10, 0);
            table.Cell(1, 1).Range.Text = "序号";
            table.Cell(1, 2).Range.Text = "备案号（VIN）";
            table.Cell(1, 3).Range.Text = "车辆型号";
            table.Cell(1, 4).Range.Text = "通用名称";
            table.Cell(1, 5).Range.Text = "燃料种类";
            table.Cell(1, 6).Range.Text = "综合工况燃料消耗量";
            table.Cell(1, 7).Range.Text = "整车整备质量";
            table.Cell(1, 8).Range.Text = "变速器型式";
            table.Cell(1, 9).Range.Text = "座位排数";
            table.Cell(1, 10).Range.Text = "操作类型";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                wordBuilder.InsertCell(table, i + 2, 1, (i + 1).ToString());
                for (int j = 2; j < 10; j++)
                {
                    wordBuilder.InsertCell(table, i + 2, j, dt.Rows[i][j - 1].ToString());
                }
                wordBuilder.InsertCell(table, i + 2, 10, dt.Rows[i][12].ToString());
            }
            wordBuilder.SaveDocument(SavePath);
        }

        //电子签章验证
        private string downLoadSignature()
        {
            var ds = service.QuerySignatureByQymc(Utils.userId, Utils.password, Utils.qymc);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["STATUS"].ToString().Equals("2"))
            {
                string imageName = ds.Tables[0].Rows[0]["IMG_NEW_NAME"].ToString();
                string imagePath = String.Format(@"{0}\{1}", DefaultDirectory.Signature, imageName);
                if (!File.Exists(imagePath))
                {
                    byte[] bs = service.DownloadFile(Utils.userId, Utils.password, imageName, "signature");
                    if (bs.Length > 0)
                    {
                        FileStream stream = new FileStream(imagePath, FileMode.Create);
                        stream.Write(bs, 0, bs.Length);
                        stream.Flush();
                        stream.Close();
                    }
                }
                return imagePath;
            }
            return null;
        }

        //验证是否存在其他申请单
        private bool checkOtherApply()
        {
            var ds = service.QueryChangeDocByUid(Utils.userId, Utils.password, Utils.qymc);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i]["STATUS"].ToString().Equals("0"))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //发送申请单
        public bool sendChangeDoc(Dictionary<string, string> data, string imagePath)
        {
            string fileName = String.Format("{0}_{1}.docx", Utils.userId, DateTime.Now.ToFileTime());
            string filePath = String.Format(@"{0}\{1}", DefaultDirectory.ChangeDoc, fileName);
            try
            {
                System.Data.DataTable dt = (System.Data.DataTable)this.gridControl1.DataSource;
                createWord(templatePath, imagePath, data, filePath, dt);
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                byte[] bytes = br.ReadBytes((int)fs.Length);
                fs.Flush();
                fs.Close();
                int flg = service.UpLoadChangeDoc(bytes, Utils.userId, Utils.password, Utils.qymc, fileName, fileName, DateTime.Now, DateTime.Now, 0);
                if (flg > 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}