using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssistantUpdater
{
    public class AppFileInfo
    {
        private string _state;
        public string fileName
        {
            get;
            set;
        }

        public string md5Str
        {
            get;
            set;
        }

        public string state
        {
            get { return _state; }
            set
            {
                _state = value;
                IsDeleted = _state != "1";
            }
        }

        public bool IsDeleted
        {
            get;
            set;
        }
    }
}
