using sones.Plugins.SonesGQL;

namespace GQLAggregates
{
    public sealed class MinAggregate : IGQLAggregate
    {
        #region IGQLAggregate Members

        public string Name
        {
            get { return "MIN"; }
        }

        #endregion
    }
}