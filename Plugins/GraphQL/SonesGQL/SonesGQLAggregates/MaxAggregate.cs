using System;

namespace sones.Plugins.SonesGQL
{

    /// <summary>
    /// The aggregate MAX
    /// </summary>
    public sealed class MaxAggregate : IGQLAggregate
    {

        #region IGQLAggregate Members

        public string Name
        {
            get { return "MAX"; }
        }

        #endregion
    }

}
