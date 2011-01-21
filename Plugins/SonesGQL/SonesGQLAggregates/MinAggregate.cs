using System;

namespace sones.Plugins.SonesGQL
{

    /// <summary>
    /// The aggregate MIN
    /// </summary>
    public sealed class MinAggregate : IGQLAggregate
    {
        #region IGQLAggregate
        
        public string GetAggregateName()
        {
            return "MIN";
        }

        #endregion
    }

}
