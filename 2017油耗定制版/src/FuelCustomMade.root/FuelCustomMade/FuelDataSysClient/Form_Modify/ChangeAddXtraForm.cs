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
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.DevForm;

namespace FuelDataSysClient.Form_Modify
{
    public partial class ChangeAddXtraForm : DevExpress.XtraEditors.XtraForm
    {
        public string did = string.Empty;
        public string guid = string.Empty;
        public bool import = false;
        private const string Change = "\\ExcelHeaderTemplate\\Change.docx";
        DefaultDirectory defaultDirectory = new DefaultDirectory();
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



        public void LoadData(string did)
        {
            if (!string.IsNullOrEmpty(did))
            {
                this.simpleButton1.Visible = false;
                this.simpleButton2.Visible = false;
                this.simpleButton3.Visible = false;
                this.simpleButton4.Visible = false;
            }
            string sql = "select * from DATA_CHANGE_BASE where ID='" + did + "'";
            DataSet dsBase = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
            if (dsBase != null && dsBase.Tables[0].Rows.Count>0)
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

            string sqlParam = "select * from DATA_CHANGE where DID='" + did + "'";
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlParam, null);
                
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                System.Data.DataTable dt =ds.Tables[0];
                dt.Columns["RLZL"].ColumnName = "RLLX";
                dt.Columns["BSQSX"].ColumnName = "CT_BSQXS";
                this.gcChangeData.DataSource = dt;
                this.gvChangeData.BestFitColumns();
                var insert_o = ds.Tables[0].Select("APPLYTYPE='补传'");
                var update = ds.Tables[0].Select("APPLYTYPE='修改'");
                var delete = ds.Tables[0].Select("APPLYTYPE='撤销'");
                INSERT_O_SL.Text = insert_o.Length.ToString();
                UPDATE_SL.Text = update.Length.ToString();
                DELETE_SL.Text = delete.Length.ToString();
            }

        }


        public void LoadDataByVin(DataView vins)
        {
            //string sql = "select * from DATA_CHANGE where vin in ("+vins+")";
            //DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
            try
            {
                if (vins != null && vins.Table.Rows.Count>0)
                {
                    System.Data.DataTable dt = (System.Data.DataTable)gcChangeData.DataSource;
                    if (dt != null)
                    {
                        dt.Merge(vins.Table);
                        string[] strComuns = { "VIN", "CLXH", "TYMC", "RLLX", "CT_ZHGKRLXHL", "ZCZBZL", "CT_BSQXS", "ZWPS", "UPDATEFIELD", "FIELDOLD", "FIELDNEW", "APPLYTYPE" };
                        dt = dt.AsDataView().ToTable(true, strComuns);
                        gcChangeData.DataSource = dt;
                    }
                    else
                    {
                        string[] strComuns = { "VIN", "CLXH", "TYMC", "RLLX", "CT_ZHGKRLXHL", "ZCZBZL", "CT_BSQXS", "ZWPS", "UPDATEFIELD", "FIELDOLD", "FIELDNEW", "APPLYTYPE" };
                        System.Data.DataTable dtVins = vins.ToTable(true, strComuns);
                        gcChangeData.DataSource = dtVins;
                    }
                    System.Data.DataView dv = ((System.Data.DataTable)gcChangeData.DataSource).AsDataView();
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


        private void simpleButton3_Click(object sender, EventArgs e)
        {
            ChangeRibbonForm crf = new ChangeRibbonForm();
            this.Close();
            crf.LoadData();
        }

        private void Show(CheckEdit ck,string type,string text,string applyType)
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
                    if (this.gcChangeData.DataSource == null) return;
                    string deleteSql = "delete from DATA_CHANGE where DID='" + guid + "' and APPLYTYPE='" + applyType + "'";
                    AccessHelper.ExecuteNonQuery(AccessHelper.conn, deleteSql, null);
                    System.Data.DataView dv = ((System.Data.DataTable)gcChangeData.DataSource).AsDataView();
                    if (dv != null)
                    {
                        var dvRow = dv.Table.Select("APPLYTYPE='" + applyType + "'");
                        foreach (var r in dvRow)
                        {
                            dv.Table.Rows.Remove(r);
                        }
                        if (dv.Table.Rows.Count > 0)
                        {
                            gcChangeData.DataSource = dv.Table;
                        }
                        else
                        {
                            gcChangeData.DataSource = null;
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

        private void INSERT_O_CheckedChanged(object sender, EventArgs e)
        {
            var check = (CheckEdit)sender;
            Show(check,"1","筛选补传数据","补传");
           
        }

        private void UPDATE_CheckedChanged(object sender, EventArgs e)
        {
            var check = (CheckEdit)sender;
            Show(check, "2", "筛选修改数据", "修改");
        }

        private void DELETE_CheckedChanged(object sender, EventArgs e)
        {
            var check = (CheckEdit)sender;
            Show(check,"0", "筛选撤销数据","撤销");
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

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (gcChangeData.DataSource == null)
            {
                MessageBox.Show("没有要保存的数据","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
                return;
            }
            string applyType = string.Empty;
            if(INSERT_O.Checked)
            {
                applyType = INSERT_O.Text+"-";
            }
            if(UPDATE.Checked)
            {
                applyType += UPDATE.Text+"-";
            }
            if(DELETE.Checked)
            {
                applyType += DELETE.Text+"-";
            }
            applyType = applyType.TrimEnd('-');
            System.Data.DataView dv = ((System.Data.DataTable)gcChangeData.DataSource).AsDataView();
            if (dv == null || dv.Table.Select("VIN not is null or VIN <> ''").Length < 1)
            {
                MessageBox.Show("没有要保存的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            using (OleDbConnection con = new OleDbConnection(AccessHelper.conn))
            {
                con.Open();
                OleDbTransaction tra = con.BeginTransaction(); //创建事务，开始执行事务
                try
                {
                    string delsql = string.Format("delete from DATA_CHANGE_BASE where ID='{0}'",guid);
                    int val = AccessHelper.ExecuteNonQuery(tra, delsql, null);
                    string sql = string.Format("insert into DATA_CHANGE_BASE values('{11}', '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}' )", this.QCSCQYMC.Text, this.APPLYDATA.Text, applyType, INSERT_O_RANGE.Text, UPDATE_RANGE.Text, DELETE_RANGE.Text, INSERT_O_SL.Text, UPDATE_SL.Text, DELETE_SL.Text, REASON.Text, REMARK.Text, guid);
                     AccessHelper.ExecuteNonQuery(tra, sql, null);


                    string delsqldata = string.Format("delete from DATA_CHANGE where DID='{0}'",guid);
                    AccessHelper.ExecuteNonQuery(tra, delsqldata, null);
                    foreach (DataRow r in dv.Table.Rows)
                    {
                        string sqlData = "insert into DATA_CHANGE values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')";
                        string sqlDataStr = string.Format(sqlData, guid, r["VIN"], r["CLXH"], r["TYMC"], r["rllx"], r["CT_ZHGKRLXHL"], r["ZCZBZL"], r["CT_BSQXS"], r["ZWPS"], r["UPDATEFIELD"], r["FIELDOLD"], r["FIELDNEW"], r["APPLYTYPE"]);
                        AccessHelper.ExecuteNonQuery(tra, sqlDataStr, null);
                    }
                    tra.Commit();
                    MessageBox.Show("操作成功,可以生成WORD或发送管理员", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    tra.Rollback();
                    MessageBox.Show("操作失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public void simpleButton2_Click(object sender, EventArgs e)
        {
            var data = InitDatas();
            if (data != null)
            {
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        saveFileDialog.AddExtension = true;
                        saveFileDialog.RestoreDirectory = true;
                        System.Data.DataTable dt = (System.Data.DataTable)this.gcChangeData.DataSource;
                        if (!dt.Columns.Contains("DID"))
                        {
                            dt.Columns.Add("DID");
                            dt.Columns["DID"].SetOrdinal(0);
                        }
                        InitRead(Change, data, saveFileDialog.FileName, dt.AsDataView());
                        if (MessageBox.Show("保存成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog.FileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
            }
            else 
            {
                MessageBox.Show("请您确认是否已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void simpleButton4_Click(object sender, EventArgs e)
        {
            //是否保存
            var data = InitDatas();
            if (data != null)
            {
                //是否插入加密狗
                string dogKey = DogAction.DogHelper.ReadData();
                if (dogKey.Length > 20)
                {
                    //加密狗是否通过
                    string strKey = Utils.userId;
                    if (strKey.Equals(dogKey.Substring(0, 13)))
                    {
                        //电子签章是否通过
                        string sigName = downLoadSignature();
                        if (!string.IsNullOrEmpty(sigName))
                        {
                            //其他申请是否通过
                            bool flg = checkOtherApply();
                            if (flg)
                            {
                                bool flag = sendChangeDoc(data, sigName);
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

        private Dictionary<string, string> InitDatas()
        {
            string sql = string.Empty;
            if (string.IsNullOrEmpty(did))
            {
                sql = "select * from DATA_CHANGE_BASE where ID='" + guid + "'";
            }
            else
            {
                sql = "select * from DATA_CHANGE_BASE where ID='" + did + "'";
            }

            var ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
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

        public void InitRead(string Template, Dictionary<string, string> datas, string saveFileName, DataView dv)
        {
            Microsoft.Office.Interop.Word.Application app = null;
            Microsoft.Office.Interop.Word.Document doc = null;
            
            try
            {
                app = new Microsoft.Office.Interop.Word.Application();//创建word应用程序
                object fileName = System.Windows.Forms.Application.StartupPath + Template;//Year1;//模板文件
                app.Visible = false ;
                //打开模板文件
                object oMissing = System.Reflection.Missing.Value;
                doc = app.Documents.Open(ref fileName,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                object replace = Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll;
                foreach (var item in datas)
                {
                    app.Selection.Find.Replacement.ClearFormatting();
                    app.Selection.Find.ClearFormatting();
                    app.Selection.Find.Text = item.Key;//需要被替换的文本
                    app.Selection.Find.Replacement.Text = item.Value;//替换文本 

                    //执行替换操作
                    app.Selection.Find.Execute(
                    ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref replace,
                    ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing);
                }
                string imageName = downLoadSignature();
                //电子签章是否存在
                if (!string.IsNullOrEmpty(imageName))
                {
                    //标签
                    object bookMark = "SIGNATURE";
                    if (app.ActiveDocument.Bookmarks.Exists(Convert.ToString(bookMark)) == true)
                    {
                        //图片
                        string replacePic = String.Format(@"{0}\{1}", defaultDirectory.Signature, imageName);
                        //定义该插入图片是否为外部链接
                        object linkToFile = true;
                        //定义插入图片是否随word文档一起保存
                        object saveWithDocument = true;
                        object Nothing = System.Reflection.Missing.Value;
                        //查找书签
                        app.ActiveDocument.Bookmarks.get_Item(ref bookMark).Select();
                        //设置图片位置
                        app.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                        //在书签的位置添加图片
                        InlineShape inlineShape = app.Selection.InlineShapes.AddPicture(replacePic, ref linkToFile, ref saveWithDocument, ref Nothing);
                        //设置图片大小
                        inlineShape.Width = 100;
                        inlineShape.Height = 100;
                    }
                }
                System.Data.DataView dt = dv;


                object tmp = "BT_CREATE";  //WORD中创建标签
                Word.Range range = app.ActiveDocument.Bookmarks.get_Item(ref tmp).Range;  //查找标签位置
                Microsoft.Office.Interop.Word.Table table = app.Selection.Tables.Add(range, dt.Table.Rows.Count+1, 10, ref oMissing, ref oMissing);

                //设置表格的字体大小粗细
                table.Range.Font.Size = 10;
                table.Range.Font.Bold = 0;

                //设置表格标题
                int rowIndex = 1;
                table.Cell(rowIndex, 1).Range.Text = "序号";
                table.Cell(rowIndex, 2).Range.Text = "备案号（VIN）";
                table.Cell(rowIndex, 3).Range.Text = "车辆型号";
                table.Cell(rowIndex, 4).Range.Text = "通用名称";
                table.Cell(rowIndex, 5).Range.Text = "燃料种类";
                table.Cell(rowIndex, 6).Range.Text = "综合工况燃料消耗量";
                table.Cell(rowIndex, 7).Range.Text = "整车整备质量";
                table.Cell(rowIndex, 8).Range.Text = "变速器型式";
                table.Cell(rowIndex, 9).Range.Text = "座位排数";
                table.Cell(rowIndex, 10).Range.Text = "操作类型";


                //循环数据创建数据行
                
                foreach (DataRow r in dt.Table.Rows)
                {
                    rowIndex++;
                    table.Cell(rowIndex, 1).Range.Text = (rowIndex-1).ToString();
                    for (int i = 2; i < 10; i++)
                    {
                        table.Cell(rowIndex, i).Range.Text = Convert.ToString(r[i-1]);
                    }
                    table.Cell(rowIndex, 10).Range.Text = Convert.ToString(r["APPLYTYPE"]);
                }

                //为表格划线
                range.Tables[1].Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                range.Tables[1].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                range.Tables[1].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                range.Tables[1].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                range.Tables[1].Borders[WdBorderType.wdBorderHorizontal].LineStyle = WdLineStyle.wdLineStyleSingle;
                range.Tables[1].Borders[WdBorderType.wdBorderVertical].LineStyle = WdLineStyle.wdLineStyleSingle;


                //对替换好的word模板另存为一个新的word文档
                doc.SaveAs(saveFileName,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

                //准备导出word

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close();//关闭word文档
                }
                if (app != null)
                {
                    app.Quit();//退出word应用程序
                }
            }
        }

        //电子签章验证
        private string downLoadSignature()
        {
            var ds = service.QuerySignatureByQymc(Utils.userId, Utils.password, Utils.qymc);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["STATUS"].ToString().Equals("2"))
            {
                string imageName = ds.Tables[0].Rows[0]["IMG_NEW_NAME"].ToString();
                string imagePath = String.Format(@"{0}\{1}", defaultDirectory.Signature, imageName);
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
                return imageName;
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
        public bool sendChangeDoc(Dictionary<string, string> data, string sigName)
        {
            string fileName = String.Format("{0}_{1}", Utils.userId, DateTime.Now.ToFileTime());
            string fileNameExtension = String.Format("{0}_{1}.docx", Utils.userId, DateTime.Now.ToFileTime());
            string filePath = String.Format(@"{0}\{1}", defaultDirectory.ChangeDoc, fileName);
            string filePathExtension = String.Format(@"{0}\{1}.docx", defaultDirectory.ChangeDoc, fileName);
            try
            {
                System.Data.DataTable dt = (System.Data.DataTable)this.gcChangeData.DataSource;
                InitRead(Change, data, filePath, dt.AsDataView());
                FileStream fs = new FileStream(filePathExtension, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                byte[] bytes = br.ReadBytes((int)fs.Length);
                fs.Flush();
                fs.Close();
                int flg = service.UpLoadChangeDoc(bytes, Utils.userId, Utils.password, Utils.qymc, fileNameExtension, fileNameExtension, DateTime.Now, DateTime.Now, 0);
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