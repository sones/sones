/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.ObjectManagement;
using sones.GraphFS.Session;
using sones.Lib.Session;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.GraphDB.QueryLanguage.Result;

namespace sones.GraphDB.TypeManagement.SpecialTypeAttributes
{
    public class SpecialTypeAttribute_TYPE : ASpecialTypeAttribute
    {

        #region AttributeUUID

        public static AttributeUUID AttributeUUID = new AttributeUUID(1);

        #endregion

        #region Name

        public static String AttributeName = "TYPE";

        #endregion

        #region ShowSettingName

        public override String ShowSettingName
        {
            get
            {
                return "TYPE";
            }
        }

        #endregion

        public SpecialTypeAttribute_TYPE()
        {
            Name = AttributeName;
            UUID = AttributeUUID;
        }

        public override Exceptional<AObject> ExtractValue(DBObjectStream dbObjectStream, GraphDBType graphDBType, DBContext dbContext)
        {
            var myType = dbContext.DBTypeManager.GetTypeByUUID(dbObjectStream.TypeUUID);
            if (myType != null)
            {
                return new Exceptional<AObject>(new DBString(myType.Name));
            }
            else
            {
                return new Exceptional<AObject>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }
        }

        public override Exceptional ApplyTo(DBObjectStream myNewDBObject, object myValue, params object[] myOptionalParameters)
        {
            return new Exceptional(new Errors.Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

    }
}
