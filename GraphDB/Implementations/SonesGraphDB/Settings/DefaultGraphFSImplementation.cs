using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Settings;

namespace sones.GraphDB.Settings
{
    /// <summary>
    /// setting that defines the default IGraphFS implementation
    /// </summary>
    public sealed class DefaultGraphFSImplementation : IGraphSetting
    {
        #region IGraphSetting Members

        public string SettingName
        {
            get { return "DefaultGraphFSImplementation"; }
        }

        public string DefaultSettingValue
        {
            get { return "InMemoryNonRevisionedFS"; }
        }

        public Type SettingType
        {
            get { return typeof(String); }
        }

        public bool IsValidValue(string myValue)
        {
            return true;
        }

        #endregion
    }
}
