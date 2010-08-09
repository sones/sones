/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


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
