#region Usings

using System;

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

    }
}
