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


#region Usings

using System;

using sones.GraphFS.DataStructures;

using sones.GraphDB.Errors;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement.BasicTypes;

using sones.Lib;
using sones.Lib.ErrorHandling;
using System.Diagnostics;

#endregion

namespace sones.GraphDB.TypeManagement.SpecialTypeAttributes
{

    public class SpecialTypeAttribute_UUID : ASpecialTypeAttribute
    {

        #region AttributeUUID

        public static AttributeUUID AttributeUUID = new AttributeUUID(0);

        #endregion
        
        #region Name

        public static String AttributeName = "UUID";

        #endregion

        #region ShowSettingName

        public override String ShowSettingName
        {
            get
            {
                return "UUID";
            }
        }

        #endregion

        public SpecialTypeAttribute_UUID()
        {
            Name = AttributeName;
            UUID = AttributeUUID;
        }

        #region Extract

        public override Exceptional<IObject> ExtractValue(DBObjectStream myDBObjectStream, GraphDBType myGraphDBType, DBContext myDBContext)
        {
            return new Exceptional<IObject>(new DBReference(myDBObjectStream.ObjectUUID));
        }

        private Object Extract(DBObjectStream myDBObjectStream, params object[] myOptionalParameters)
        {
            return myDBObjectStream.ObjectUUID;
        }

        #endregion

        #region ApplyTo

        public override Exceptional<Object> ApplyTo(DBObjectStream myNewDBObject, Object myValue, params object[] myOptionalParameters)
        {
            return new Exceptional<Object>(new Error_NotImplemented(new StackTrace(true)));
        }

        #endregion

    }

}
