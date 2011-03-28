#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.Index.ErrorHandling;
using sones.Library.ErrorHandling;

#endregion

namespace sones.Plugins.Index.ErrorHandling
{
    public sealed class UniqueIndexConstraintException : ASonesIndexException
    {
        #region Data

        public String Info { get; private set; }

        #endregion

        #region Constructor

        public UniqueIndexConstraintException(String myInfo)
        {
            Info = myInfo;
        }

        #endregion

        public override string ToString()
        {
            return Info;
        }

        public override ushort ErrorCode
        {
            get
            {
                return ErrorCodes.UniqueIndexConstraintException;
            }
        }
    }
}
