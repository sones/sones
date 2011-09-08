using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index.ErrorHandling
{
    public class IndexAddFailedException : ASonesIndexException
    {
        #region Data

        public String Info { get; private set; }

        #endregion

        #region Constructor

        public IndexAddFailedException(String myInfo)
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
