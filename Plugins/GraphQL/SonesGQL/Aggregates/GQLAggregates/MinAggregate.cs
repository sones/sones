namespace sones.Plugins.SonesGQL.Aggregate
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