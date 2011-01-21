using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index
{
    public sealed class DictionaryIndex : IIndex
    {
        #region IIndex

        public bool IsPersistent()
        {
            return false;
        }

        public string GetName()
        {
            return "Dictionary";
        }

        #endregion
    }
}
