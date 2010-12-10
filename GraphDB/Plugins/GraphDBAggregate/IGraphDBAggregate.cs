using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Aggregates
{

    #region IGraphDBAggregateVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IGraphDBAggregate plugin versions. 
    /// Defines the min and max version for all IGraphDBAggregate implementations which will be activated used this IGraphDBAggregate.
    /// </summary>
    public static class IGraphDBAggregateVersionCompatibility
    {
        public static Version MinVersion
        {
            get
            {
                return new Version("1.0.0.1");
            }
        }
        public static Version MaxVersion
        {
            get
            {
                return new Version("1.0.0.1");
            }
        }
    }

    #endregion

    /// <summary>
    /// Since the ABaseAggregate can't be moved out of the GraphDB this interface is used to handle
    /// the version compatibility. The assembly version of this assembly needs to be chagned for any changes at the ABaseAggregate class!
    /// </summary>
    public interface IGraphDBAggregate
    {
    }
}
