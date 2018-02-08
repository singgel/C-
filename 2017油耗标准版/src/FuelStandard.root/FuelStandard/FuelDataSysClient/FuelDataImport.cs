using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using System.Data.OleDb;

namespace FuelDataSysClient
{
    public partial class FuelDataImportForm : Form
    {
        public FuelDataImportForm()
        {
            InitializeComponent();
        }

        private void btnVIN_Click(object sender, EventArgs e)
        {
            //mitsUtils utils = new mitsUtils();
            //FolderDialog openFolder = new FolderDialog();
            //if (openFolder.DisplayDialog() == DialogResult.OK)
            //{
            //    string folderPath = openFolder.Path.ToString();
            //   // string fileName = utils.GetFileName(folderPath, "VIN*.xls");
            //    this.teVIN.Text = fileName;

            //    utils.SaveVinInfo(utils.ReadVINExcel(fileName));
            //}
            //else
            //{
            //    this.teVIN.Text = "你没有选择目录";
            //}
        }

        private void btnCOC_Click(object sender, EventArgs e)
        {
            //mitsUtils utils = new mitsUtils();
            //FolderDialog openFolder = new FolderDialog();
            //if (openFolder.DisplayDialog() == DialogResult.OK)
            //{
            //    string folderPath = openFolder.Path.ToString();
            //  //  string fileName = utils.GetFileName(folderPath, "xCOC*.xls");
            //    this.teCOC.Text = fileName;

            //    utils.ReadCtnyCOCExcel(fileName);
            //}
            //else
            //{
            //    this.teCOC.Text = "你没有选择目录";
            //}
        }
    }
}