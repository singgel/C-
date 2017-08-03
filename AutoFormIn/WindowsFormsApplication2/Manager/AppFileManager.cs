using Assistant.DataProviders;
using AssistantUpdater;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Data.Collection;

namespace Assistant.Manager
{
    public class AppFileManager : SelectorDataModel
    {
        private InfoList<AppFileInfo> _AppFiles;
        private List<string> _Enterprises;
        private bool _IsUploading;
        private string _SelectedEnterprise;
        private static readonly PropertyChangedEventArgs EnterprisesChanged = new PropertyChangedEventArgs("Enterprises");
        private static readonly PropertyChangedEventArgs IsUploadingChanged = new PropertyChangedEventArgs("IsUploading");
        private static readonly PropertyChangedEventArgs SelectedEnterpriseChanged = new PropertyChangedEventArgs("SelectedEnterprise");
        
        public List<string> Enterprises
        {
            get { return _Enterprises; }
            private set
            {
                if (_Enterprises != value)
                {
                    _Enterprises = value;
                    OnPropertyChanged(EnterprisesChanged);
                }
            }
        }
        
        public bool IsUploading
        {
            get { return _IsUploading; }
            set
            {
                if (_IsUploading != value)
                {
                    _IsUploading = value;
                    OnPropertyChanged(IsUploadingChanged);
                }
            }
        }

        public string SelectedEnterprise
        {
            get { return _SelectedEnterprise; }
            set
            {
                if (_SelectedEnterprise != value)
                {
                    _SelectedEnterprise = value;
                    OnPropertyChanged(SelectedEnterpriseChanged);
                    _AppFiles.Clear();
                    System.Threading.ThreadPool.QueueUserWorkItem(ChangeEnterprise);
                }
            }
        }

        public override System.Collections.IList Items
        {
            get { return _AppFiles; }
        }

        public AppFileManager()
        {
            _AppFiles = new InfoList<AppFileInfo>();
            System.Threading.ThreadPool.QueueUserWorkItem((state) =>
            {
                Enterprises = ServiceHelper.GetAllEnterprises();
            });
        }

        private void ChangeEnterprise(object state)
        {
            IsUploading = true;
            _AppFiles.CloseNotifycation();
            try
            {
                Version serverVersion = new Version(ServiceHelper.GetAppVersion(_SelectedEnterprise));
                List<AssistantUpdater.AppFileInfo> list = ServiceHelper.GetAllFiles(_SelectedEnterprise, serverVersion.ToString());
                Version version = FileHelper.GetCurrentVersion();

                foreach (var item in list)
                {
                    AppFileInfo file = new AppFileInfo()
                    {
                        ServerVersion = serverVersion,
                        Version = version,
                        Enterprise = _SelectedEnterprise,
                        Status = item.IsDeleted ? FileStatus.Deleted : FileStatus.Uploaded,
                        OriginName = item.fileName
                    };
                    _AppFiles.Add(file);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.Application.OpenForms[0].Invoke((Action)(() =>
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "错误", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }));
            }
            finally
            {
                IsUploading = false;
                System.Windows.Forms.Application.OpenForms[0].Invoke((Action)(() =>
                {
                    _AppFiles.OpenNotifycation(true);
                }));
            }
        }

        public void SaveChange()
        {
            if (_IsUploading)
                return;
            IsUploading = true;
            string entName = this._SelectedEnterprise;
            try
            {
                string version = FileHelper.GetCurrentVersion().ToString();
                foreach (var item in _AppFiles)
                {
                    item.Note = "";
                    try
                    {
                        if (item.Status == FileStatus.DeleteFlag)
                        {
                            ServiceHelper.DeleteAppFile(entName, item.OriginName);
                            item.Status = FileStatus.Deleted;
                        }
                        else if (item.Status == FileStatus.New || item.Status == FileStatus.Update)
                        {
                            if (System.IO.File.Exists(item.FileName) == false)
                            {
                                item.Note = "文件不存在！";
                                continue;
                            }
                            ServiceHelper.UploadAppFile(entName, version, item.FileName);
                            item.Status = FileStatus.Uploaded;
                        }
                    }
                    catch (Exception ex)
                    {
                        item.Note = ex.Message;
                    }
                }
            }
            finally
            {
                IsUploading = false;
            }
        }
    }
}
