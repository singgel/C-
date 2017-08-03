using System;
using System.Collections.Generic;
using System.Text;

namespace OAUS.Core
{
    public class LastUpdateTimeContract
    {
        #region ClientVersion
        private int clientVersion = 0;
        public int ClientVersion
        {
            get { return clientVersion; }
            set { clientVersion = value; }
        }
        #endregion

        public LastUpdateTimeContract()
        {
        }

        public LastUpdateTimeContract(int version)
        {           
            this.clientVersion = version;
        }
    }
}
