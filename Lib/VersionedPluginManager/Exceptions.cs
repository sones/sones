using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace sones.Lib.VersionedPluginManager.Exceptions
{

    #region PluginManagerException

    public class PluginManagerException : Exception
    {

        public PluginManagerException()
            : base()
        { }

        public PluginManagerException(string myMessage)
            : base(myMessage)
        { }

        public PluginManagerException(string myMessage, Exception myInnerException)
            : base(myMessage, myInnerException)
        { }

    }
    
    #endregion

    #region PluginActivatorException

    public class PluginActivatorException : PluginManagerException
    {

        public PluginActivatorException()
            : base()
        { }

        public PluginActivatorException(string myMessage)
            : base(myMessage)
        { }

        public PluginActivatorException(string myMessage, Exception myInnerException)
            : base(myMessage, myInnerException)
        { }

    }
    
    #endregion

    #region IncompatiblePluginVersion

    public class IncompatiblePluginVersionException : PluginActivatorException
    {

        public IncompatiblePluginVersionException(Assembly myPluginAssembly, Version myCurrentVersion, Version myMinVersion)
            : base(String.Format("The plugin '{0}' at '{1}' is of a not supported version {2}. The minimum version is '{3}'!", myPluginAssembly, myPluginAssembly.Location, myCurrentVersion, myMinVersion))
        {

        }

        public IncompatiblePluginVersionException(Assembly myPluginAssembly, Version myCurrentVersion, Version myMinVersion, Version myMaxVersion)
            : base(String.Format("The plugin '{0}' at '{1}' is of a not supported version {2}. The minimum version is '{3}' and the maximum version is '{4}'!", myPluginAssembly, myPluginAssembly.Location, myCurrentVersion, myMinVersion, myMaxVersion))
        {

        }

    }

    #endregion

}
