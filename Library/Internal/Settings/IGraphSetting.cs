using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Library.Settings
{
    /// <summary>
    /// The interface for all graph application settings
    /// </summary>
    public interface IGraphSetting
    {
        /// <summary>
        /// The name of the settiong
        /// </summary>
        String SettingName { get; }

        /// <summary>
        /// The default value of the setting
        /// </summary>
        String DefaultSettingValue { get; }

        /// <summary>
        /// The type of the setting
        /// </summary>
        Type SettingType { get; }

        /// <summary>
        /// Is some kind of value valid for this setting
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        Boolean IsValidValue(String myValue);
    }
}
