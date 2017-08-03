using Assistant.DataProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Assistant.Manager
{
    public class AppFileInfo : NotifyPropertyChanged, IEquatable<AppFileInfo>
    {
        private string _Enterprise;
        private string _FileName;
        private string _Note;
        private string _OriginName;
        private Version _ServerVersion;
        private FileStatus _Status;
        private Version _Version;
        private static readonly PropertyChangedEventArgs EnterpriseChanged = new PropertyChangedEventArgs("Enterprise");
        private static readonly PropertyChangedEventArgs FileNameChanged = new PropertyChangedEventArgs("FileName");
        private static readonly PropertyChangedEventArgs NoteChanged = new PropertyChangedEventArgs("Note");
        private static readonly PropertyChangedEventArgs StatusChanged = new PropertyChangedEventArgs("Status");
        private static readonly PropertyChangedEventArgs ServerVersionChanged = new PropertyChangedEventArgs("ServerVersion");
        private static readonly PropertyChangedEventArgs VersionChanged = new PropertyChangedEventArgs("Version");
        private static readonly PropertyChangedEventArgs OriginNameChanged = new PropertyChangedEventArgs("OriginName");

        public string Enterprise
        {
            get { return _Enterprise; }
            set
            {
                if (_Enterprise != value)
                {
                    _Enterprise = value;
                    OnPropertyChanged(EnterpriseChanged);
                }
            }
        }

        public string FileName
        {
            get { return _FileName; }
            set
            {
                if (_FileName != value)
                {
                    _FileName = value;
                    OnPropertyChanged(FileNameChanged);
                    if (_Status == FileStatus.Uploaded || _Status == FileStatus.DeleteFlag || _Status == FileStatus.Deleted)
                        Status = FileStatus.Update;
                }
            }
        }

        public FileStatus Status
        {
            get { return _Status; }
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    OnPropertyChanged(StatusChanged);
                }
            }
        }

        public Version Version
        {
            get { return _Version; }
            set
            {
                if (_Version != value)
                {
                    _Version = value;
                    OnPropertyChanged(VersionChanged);
                }
            }
        }

        public string OriginName
        {
            get { return _OriginName; }
            set
            {
                if (_OriginName != value)
                {
                    _OriginName = value;
                    OnPropertyChanged(OriginNameChanged);
                }
            }
        }

        public Version ServerVersion
        {
            get { return _ServerVersion; }
            set
            {
                if (_ServerVersion != value)
                {
                    _ServerVersion = value;
                    OnPropertyChanged(ServerVersionChanged);
                }
            }
        }

        public string Note
        {
            get { return _Note; }
            set
            {
                if (_Note != value)
                {
                    _Note = value;
                    OnPropertyChanged(NoteChanged);
                }
            }
        }

        public AppFileInfo()
        {
            _Version = FileHelper.GetCurrentVersion();
        }

        public void Delete()
        {
            if(_Status == FileStatus.Uploaded || _Status == FileStatus.Update)
                Status = FileStatus.DeleteFlag;
        }

        public bool Equals(AppFileInfo other)
        {
            if (object.Equals(other, null))
                return false;
            string name1, name2;
            if (string.IsNullOrEmpty(this._FileName))
                name1 = this._OriginName;
            else
            {
                int index = this._FileName.LastIndexOf("\\");
                name1 = this.FileName.Substring(index + 1);
            }
            if (string.IsNullOrEmpty(other._FileName))
                name2 = other.OriginName;
            else
            {
                int index = other._FileName.LastIndexOf("\\");
                name2 = other._FileName.Substring(index + 1);
            }
            return name1 == name2;
        }

        public static bool operator ==(AppFileInfo file1, AppFileInfo file2)
        {
            return object.Equals(file1, null) ? (object.Equals(file2, null) ? true : false) : file1.Equals(file2);
        }

        public static bool operator !=(AppFileInfo file1, AppFileInfo file2)
        {
            return !(file1 == file2);
        }
    }
}
