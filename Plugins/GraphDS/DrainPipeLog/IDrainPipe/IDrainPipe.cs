using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.VersionedPluginManager;
using sones.GraphDS;

namespace sones.Plugins.GraphDS
{
    #region IDrainPipeCompatibility

    /// <summary>
    /// A static implementation of the compatible IDrainPipe plugin versions. 
    /// Defines the min and max version for all IDrainPipe implementations which will be activated used this IDrainPipe.
    /// </summary>
    public static class IDrainPipeCompatibility
    {
        public static Version MinVersion
        {
            get { return new Version("2.0.0.0"); }
        }

        public static Version MaxVersion
        {
            get { return new Version("2.0.0.0"); }
        }
    }
    #endregion

    public interface IDrainPipe : IGraphDS,IPluginable
    {

    }
}
