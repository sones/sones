using sones.Plugins.SonesGQL;

namespace sones.Plugins.SonesGQL.Aggregates
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