using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_CouldNotSetSetting : GraphDBSettingError
    {
        public ADBSettingsBase Setting { get; private set; }
        public TypesSettingScope Scope { get; private set; }
        public GraphDBType Type { get; private set; }
        public TypeAttribute Attribute { get; private set; }


        public Error_CouldNotSetSetting(ADBSettingsBase mySetting, TypesSettingScope myScope, GraphDBType myType = null, TypeAttribute myAttribute = null)
        {
            Setting = mySetting;
            Scope = myScope;
            Type = myType;
            Attribute = myAttribute;
        }

        public override string ToString()
        {
            return String.Format("Could not set the setting {0} for scope {1}.", Setting.Name, Scope.ToString());
        }
    }
}
