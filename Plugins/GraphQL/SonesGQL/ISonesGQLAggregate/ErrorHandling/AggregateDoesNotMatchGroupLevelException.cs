using System;
using sones.Library.ErrorHandling;

namespace sones.Plugins.SonesGQL.Aggregates.ErrorHandling
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AggregateDoesNotMatchGroupLevelException : ASonesQLAggregateException
    {
        #region data

        public String Info { get; private set; }

        #endregion

        #region constructor

        public AggregateDoesNotMatchGroupLevelException(String myInfo)
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
            get { return ErrorCodes.AggregateDoesNotMatchGroupLevel; }
        }   
    }
}
