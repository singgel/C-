using Assistant.DataProviders;
using Assistant.Properties;
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
    public class RuleFileManager : SelectorDataModel
    {
        private List<FillRuleVersion> _FillRuleVersions;
        private List<string> _FillTypes;
        private bool _IsUploading;
        private InfoList<RuleFileInfo> _Items;
        private FillRuleVersion _SelectedFillRuleVersion;
        private string _SelectedFillType;
        private static readonly List<FillRuleVersion> _entNameVersion;
        private static readonly PropertyChangedEventArgs FillRuleVersionsChanged = new PropertyChangedEventArgs("FillRuleVersions");
        private static readonly PropertyChangedEventArgs IsUploadingChanged = new PropertyChangedEventArgs("IsUploading");
        private static readonly PropertyChangedEventArgs SelectedFillRuleVersionChanged = new PropertyChangedEventArgs("SelectedFillRuleVersion");
        private static readonly PropertyChangedEventArgs SelectedFillTypeChanged = new PropertyChangedEventArgs("SelectedFillType");

        public List<FillRuleVersion> FillRuleVersions
        {
            get { return _FillRuleVersions; }
            private set
            {
                if (_FillRuleVersions != value)
                {
                    _FillRuleVersions = value;
                    OnPropertyChanged(FillRuleVersionsChanged);
                }
            }
        }

        public List<string> FillTypes
        {
            get { return _FillTypes; }
        }

        public bool IsUploading
        {
            get { return _IsUploading; }
            private set
            {
                if (_IsUploading != value)
                {
                    _IsUploading = value;
                    OnPropertyChanged(IsUploadingChanged);
                }
            }
        }

        public override System.Collections.IList Items
        {
            get { return _Items; }
        }

        public FillRuleVersion SelectedFillRuleVersion
        {
            get { return _SelectedFillRuleVersion; }
            set
            {
                if (_IsUploading)
                    return;
                if (value != _SelectedFillRuleVersion)
                {
                    _SelectedFillRuleVersion = value;
                    OnPropertyChanged(SelectedFillRuleVersionChanged);
                    OnSelectedFillTypeChanged();
                }
            }
        }

        public string SelectedFillType
        {
            get { return _SelectedFillType; }
            set
            {
                if (_IsUploading)
                    return;
                if (_SelectedFillType != value)
                {
                    _SelectedFillType = value;
                    OnPropertyChanged(SelectedFillTypeChanged);
                    OnSelectedFillTypeChanged();
                }
            }
        }

        public RuleFileManager()
        {
            _Items = new InfoList<RuleFileInfo>();
            this.Init();
        }

        static RuleFileManager()
        {
            _entNameVersion = new List<FillRuleVersion>();
            string entName = FileHelper.GetEntName();
            _entNameVersion.Add(new FillRuleVersion()
            {
                Name = entName,
                UseAppendix = false,
                Value = entName
            });
        }

        private void Init()
        {
            _FillTypes = FileHelper.GetFillTypes();
            _FillRuleVersions = FileHelper.GetFillRuleVersions();
        }

        private void OnSelectedFillTypeChanged()
        {
            string ruleVersion = _SelectedFillRuleVersion == null ? "" : _SelectedFillRuleVersion.Name;
            string basePath = FileHelper.GetFillVersionByName(ruleVersion);
            if (_SelectedFillType == "转换规则" || _SelectedFillType == "应用程序配置")
            {
                FillRuleVersions = _entNameVersion;
                SelectedFillRuleVersion = _entNameVersion[0];
                ruleVersion = Properties.Resources.FillRule;
                basePath = "";
            }
            else if(FillRuleVersions == _entNameVersion)
                FillRuleVersions = FileHelper.GetFillRuleVersions();

            if (string.IsNullOrEmpty(_SelectedFillType) || _SelectedFillRuleVersion == null)
            {
                _Items.Clear();
                return;
            }
            bool hasAppendix;
            List<UpdateRuleParameter> list = FileHelper.GetRuleFileList(ruleVersion, _SelectedFillType, basePath, out hasAppendix);
            List<KeyValuePair<string, string>> appendixes = null;
            if (_SelectedFillRuleVersion.UseAppendix && hasAppendix)
                appendixes = FileHelper.GetAppendixes(_SelectedFillType);
            _Items.Clear();
            if (appendixes == null || appendixes.Count == 0)
                InitFileList(list, "");
            else
            {
                foreach (var appendix in appendixes)
                {
                    InitFileList(list, appendix.Key);
                }
            }
            System.Threading.ThreadPool.QueueUserWorkItem(GetFileStatus, null);
        }

        private void InitFileList(List<UpdateRuleParameter> list, string appendix)
        {
            foreach (var item in list)
            {
                RuleFileInfo file = new RuleFileInfo(item.Type)
                {
                    Appendix = appendix,
                    CarType = item.CarType,
                    Standard = item.Standard,
                    Version = _SelectedFillRuleVersion
                };
                System.IO.FileInfo info = new System.IO.FileInfo(item.FileName);
                file.OriginName = info.Name;
                _Items.Add(file);
            }
        }

        private void GetFileStatus(object state)
        {
            IsUploading = true;
            foreach (var item in _Items)
            {
                try
                {
                    string fileName = FileHelper.GetFillRuleFile(_SelectedFillRuleVersion.Name, item.Type, item.Standard, item.CarType);
                    System.IO.FileInfo info = new System.IO.FileInfo(fileName);
                    int len = fileName.Length > info.Name.Length ? fileName.Length - info.Name.Length - 1 : 0;
                    fileName = string.Format("{0}\\{1}\\{2}", item.Version.Value, fileName.Substring(0, len),
                        string.IsNullOrEmpty(item.Appendix) ? info.Name : string.Format("{0}-{1}", item.Appendix, info.Name));
                    string md5 = ServiceHelper.GetFillRuleMd5(item.Type, item.Version.Name, item.Standard, item.CarType, fileName);
                    if (string.IsNullOrEmpty(md5))
                        item.Status = FileStatus.None;
                    else
                        item.Status = FileStatus.Uploaded;
                }
                catch (Exception ex)
                {
                    item.Note = ex.Message;
                }
            }
            IsUploading = false;
        }

        public void Upload()
        {
            if (_IsUploading)
                return;
            IsUploading = true;
            try
            {
                foreach (var item in _Items)
                {
                    if (item.Status == FileStatus.New || item.Status == FileStatus.Update)
                    {
                        if (System.IO.File.Exists(item.FileName) == false)
                        {
                            item.Note = "文件不存在";
                            continue;
                        }
                        try
                        {
                            ServiceHelper.UploadFillRule(item.Type, item.Version.Name, item.Standard, item.CarType, item.FileName);
                            item.Status = FileStatus.Uploaded;
                        }
                        catch (Exception ex)
                        {
                            item.Note = ex.Message;
                        }
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
