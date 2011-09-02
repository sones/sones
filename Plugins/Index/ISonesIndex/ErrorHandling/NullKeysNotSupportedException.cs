using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index.ErrorHandling
{
    public class NullKeysNotSupportedException : ASonesIndexException
    {
         #region Data

        public String Info { get; private set; }

        #endregion

        #region Constructor

        public NullKeysNotSupportedException(String myInfo)
        {
            Info = myInfo;
        }

        #endregion

        public override string ToString()
        {
            return Info;
        }
    }
}
