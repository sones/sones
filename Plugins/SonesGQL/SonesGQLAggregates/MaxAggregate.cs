using System;

namespace sones.Plugins.SonesGQL
{

    /// <summary>
    /// The aggregate MAX
    /// </summary>
    public sealed class MaxAggregate : IGQLAggregate
    {
        #region IGQLAggregate
        
        public string GetAggregateName()
        {
            return "MAX";
        }

        #endregion
    }

}
