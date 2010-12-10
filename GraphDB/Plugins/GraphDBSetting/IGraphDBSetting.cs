using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Settings
{


    #region IGraphDBSettingVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IGraphDBSetting plugin versions. 
    /// Defines the min and max version for all IGraphDBSetting implementations which will be activated used this IGraphDBSetting.
    /// </summary>
    public static class IGraphDBSettingVersionCompatibility
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
    /// Since the ADBSettingsBase can't be moved out of the GraphDB this interface is used to handle
    /// the version compatibility. The assembly version of this assembly needs to be chagned for any changes at the ADBSettingsBase class!
    /// </summary>
    public interface IGraphDBSetting
    {
    }


}
