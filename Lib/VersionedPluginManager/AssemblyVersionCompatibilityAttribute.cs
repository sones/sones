using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace sones.Lib.VersionedPluginManager
{

    /// <summary>
    /// Defines the min and max version of the plugin.
    /// This must be added to the assembly which activates the plugins.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    [ComVisible(true)]
    public sealed class AssemblyVersionCompatibilityAttribute : Attribute
    {

        public AssemblyVersionCompatibilityAttribute(String myPluginName, String myMinVersion, String myMaxVersion)
        {
            PluginName = myPluginName;
            MinVersion = new Version(myMinVersion);
            if (!String.IsNullOrEmpty(myMaxVersion))
            {
                MaxVersion = new Version(myMaxVersion);
            }
        }


        public Version MinVersion { get; private set; }
        public Version MaxVersion { get; private set; }
        public String PluginName { get; private set; }
    }

}

