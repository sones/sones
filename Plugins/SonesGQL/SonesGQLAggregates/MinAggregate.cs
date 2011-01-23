using System;

namespace sones.Plugins.SonesGQL
{

    /// <summary>
    /// The aggregate MIN
    /// </summary>
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
