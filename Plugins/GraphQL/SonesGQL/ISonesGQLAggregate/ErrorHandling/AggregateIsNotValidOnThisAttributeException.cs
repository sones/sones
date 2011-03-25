using System;
using sones.Library.ErrorHandling;

namespace sones.Plugins.SonesGQL.Aggregates.ErrorHandling
{
    public sealed class AggregateIsNotValidOnThisAttributeException : ASonesQLAggregateException
    {
        public String Info { get; private set; }

        /// <summary>
        /// Creates a new AggregateIsNotValidOnThisAttributeException exception
        /// </summary>
        /// <param name="myInfo"></param>
        public AggregateIsNotValidOnThisAttributeException(String myInfo)
        {
            Info = myInfo;
        }

        public override string ToString()
        {
            return Info;
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.AggregateIsNotValidOnThisAttribute; }
        }  
    }
}
