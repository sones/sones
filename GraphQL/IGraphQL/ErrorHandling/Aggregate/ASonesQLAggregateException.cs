using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    public class ASonesQLAggregateException : ASonesException
    {
        public override ushort ErrorCode
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}
