using System;
using sones.Library.ErrorHandling;

namespace sones.Plugins.SonesGQL.Aggregates.ErrorHandling
{
    /// <summary>
    /// An aggregate is on multi attributes not allowed
    /// </summary>
    public sealed class AggregateOnMultiAttributesNotAllowedException : ASonesQLAggregateException
    {
        public String Info { get; private set; }

        public AggregateOnMultiAttributesNotAllowedException(String myInfo)
        {
            Info = myInfo;
        }

        public override string ToString()
        {
            return Info;
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.AggregateOnMultiAttributesNotAllowed; }
        } 
    }
}
