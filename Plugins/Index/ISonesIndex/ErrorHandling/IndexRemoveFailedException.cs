using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index.ErrorHandling
{
    public class IndexRemoveFailedException : ASonesIndexException
    {
        #region Data

        public String Info { get; private set; }

        #endregion

        #region Constructor

        public IndexRemoveFailedException(String myInfo)
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
