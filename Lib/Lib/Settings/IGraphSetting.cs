using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Lib.Settings
{

    public interface IGraphSetting
    {

        String SettingName          { get; }
        String DefaultSettingValue  { get; }
        Type SettingType            { get; }
        Boolean IsValidValue(String myValue);

    }

}
