using Assistant.DataProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Assistant.Manager
{
    public class RuleFileInfo : NotifyPropertyChanged
    {
        private string _Appendix;
        private string _CarType;
        private string _FileName;
        private string _Note;
        private string _Standard;
        private FileStatus _Status;
        private string _Type;
        private string _OriginName;
        private FillRuleVersion _Version;

        private static readonly PropertyChangedEventArgs AppendixChanged = new PropertyChangedEventArgs("Appendix");
        private static readonly PropertyChangedEventArgs CarTypeChanged = new PropertyChangedEventArgs("CarType");
        private static readonly PropertyChangedEventArgs FileNameChanged = new PropertyChangedEventArgs("FileName");
        private static readonly PropertyChangedEventArgs NoteChanged = new PropertyChangedEventArgs("Note");
        private static readonly PropertyChangedEventArgs StandardChanged = new PropertyChangedEventArgs("Standard");
        private static readonly PropertyChangedEventArgs StatusChanged = new PropertyChangedEventArgs("Status");
        private static readonly PropertyChangedEventArgs TypeChanged = new PropertyChangedEventArgs("Type");
        private static readonly PropertyChangedEventArgs VersionChanged = new PropertyChangedEventArgs("Version");
        private static readonly PropertyChangedEventArgs OriginNameChanged = new PropertyChangedEventArgs("OriginName");

        public RuleFileInfo(string type)
        {
            _Type = type;
        }

        public string CarType
        {
            get { return _CarType; }
            set
            {
                if (_CarType != value)
                {
                    _CarType = value;
                    OnPropertyChanged(CarTypeChanged);
                }
            }
        }

        public string Standard
        {
            get { return _Standard; }
            set
            {
                if (_Standard != value)
                {
                    _Standard = value;
                    OnPropertyChanged(StandardChanged);
                }
            }
        }

        public FillRuleVersion Version
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

        public string Type
        {
            get { return _Type; }
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    OnPropertyChanged(TypeChanged);
                }
            }
        }

        public string Appendix
        {
            get { return _Appendix; }
            set
            {
                if (_Appendix != value)
                {
                    _Appendix = value;
                    OnPropertyChanged(AppendixChanged);
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

        public string FileName
        {
            get { return _FileName; }
            set
            {
                if (_FileName != value)
                {
                    _FileName = value;
                    if (_Status == FileStatus.Uploaded)
                        Status = FileStatus.Update;
                    else if (_Status == FileStatus.None)
                        Status = FileStatus.New;
                    OnPropertyChanged(FileNameChanged);
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
    }
}
