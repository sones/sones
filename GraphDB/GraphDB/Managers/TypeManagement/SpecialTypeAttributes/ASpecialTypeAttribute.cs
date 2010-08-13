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
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.ObjectManagement;
using sones.GraphFS.Session;
using sones.Lib.Session;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Structures.Result;

namespace sones.GraphDB.TypeManagement.SpecialTypeAttributes
{
    public abstract class ASpecialTypeAttribute : TypeAttribute
    {

        /// <summary>
        /// The ShowSetting name
        /// </summary>
        public abstract String ShowSettingName { get; }

        /// <summary>
        /// This will change the <paramref name="myNewDBObject"/> dependend on the SpecialTypeAttribute implementation
        /// </summary>
        /// <param name="myNewDBObject">The DBObject</param>
        /// <param name="myValue">The values which should be assigned to the <paramref name="myNewDBObject"/></param>
        /// <param name="myOptionalParameters">Some optional parameters</param>
        public abstract Exceptional ApplyTo(DBObjectStream myNewDBObject, Object myValue, params object[] myOptionalParameters);

        /// <summary>
        /// Extracts the value dependend on the SpecialTypeAttribute from the <paramref name="dbObjectStream"/>
        /// </summary>
        /// <param name="dbObjectStream">The dbObjectStream</param>
        /// <param name="graphDBType"></param>
        /// <param name="sessionInfos"></param>
        /// <returns>The extracted value</returns>
        public abstract Exceptional<IObject> ExtractValue(DBObjectStream dbObjectStream, GraphDBType graphDBType, DBContext dbContext);
    }
}
