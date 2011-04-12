using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The aggregate does not match the group level
    /// </summary>
    public sealed class AggregateDoesNotMatchGroupLevelException : ASonesQLAggregateException
    {
        #region data

        public String Info { get; private set; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new AggregateDoesNotMatchGroupLevelException exception
        /// </summary>
        /// <param name="myInfo"></param>
        public AggregateDoesNotMatchGroupLevelException(String myInfo)
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
