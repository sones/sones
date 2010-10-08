using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;

namespace sones.Lib.Settings
{

    public delegate Exceptional GraphSettingChangingEvent(GraphSettingChangingEventArgs myGraphDSSetting);

    /// <summary>
    /// The arguments of the event which is fired for a changed setting.
    /// </summary>
    public class GraphSettingChangingEventArgs : EventArgs
    {

        /// <summary>
        /// The Setting information containing the default vaule, type and name
        /// </summary>
        public IGraphSetting Setting { get; private set; }

        /// <summary>
        /// The new setting value
        /// </summary>
        public String SettingValue { get; set; }

        public GraphSettingChangingEventArgs(IGraphSetting mySetting, String mySettingValue)
        {
            Setting = mySetting;
            SettingValue = mySettingValue;
        }

    }

}
