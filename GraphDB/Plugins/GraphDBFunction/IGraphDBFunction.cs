using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Functions
{

    #region IGraphDBFunctionVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IGraphDBFunction plugin versions. 
    /// Defines the min and max version for all IGraphDBFunction implementations which will be activated used this IGraphDBFunction.
    /// </summary>
    public static class IGraphDBFunctionVersionCompatibility
    {
        public static Version MinVersion
        {
            get
            {
                return new Version("1.0.0.0");
            }
        }
        public static Version MaxVersion
        {
            get
            {
                return new Version("1.0.0.0");
            }
        }
    }

    #endregion

    /// <summary>
    /// Since the ABaseFunction can't be moved out of the GraphDB this interface is used to handle
    /// the version compatibility. The assembly version of this assembly needs to be chagned for any changes at the ABaseFunction class!
    /// </summary>
    public interface IGraphDBFunction
    {
    }
}
