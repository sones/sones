using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index
{
    public sealed class DictionaryIndex : IIndex
    {

        #region IIndex Members

        public bool IsPersistent
        {
            get { return false; }
        }

        public string Name
        {
            get { return "Dictionary"; }
        }

        #endregion
    }
}
