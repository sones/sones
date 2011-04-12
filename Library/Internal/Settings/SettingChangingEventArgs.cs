using System;

namespace sones.Library.Settings
{
    public delegate void GraphSettingChangingEvent(SettingChangingEventArgs myGraphDSSetting);

    /// <summary>
    /// The arguments of the event which is fired for a changed setting.
    /// </summary>
    public sealed class SettingChangingEventArgs : EventArgs
    {

        /// <summary>
        /// The Setting information containing the default vaule, type and name
        /// </summary>
        public IGraphSetting Setting { get; private set; }

        /// <summary>
        /// The new setting value
        /// </summary>
        public String SettingValue { get; set; }

        public SettingChangingEventArgs(IGraphSetting mySetting, String mySettingValue)
        {
            Setting = mySetting;
            SettingValue = mySettingValue;
        }

    }
}
