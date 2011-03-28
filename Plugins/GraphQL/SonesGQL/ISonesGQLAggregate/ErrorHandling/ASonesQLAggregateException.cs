using sones.Library.ErrorHandling;

namespace sones.Plugins.SonesGQL.Aggregate.ErrorHandling
{
    public class ASonesQLAggregateException : ASonesException
    {
        public override ushort ErrorCode
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}
