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
using sones.GraphDB.Errors;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.DataStructures;
using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.GraphDBInterface.TypeManagement;

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

        public override Exceptional<IObject> ExtractValue(DBObjectStream dbObjectStream, GraphDBType graphDBType, DBContext dbContext)
        {

            //return new Exceptional<ADBBaseObject>(new DBString(dbObjectStream.ObjectUUID.ToString()));
            return new Exceptional<IObject>(new DBReference(dbObjectStream.ObjectUUID));

            //String settingEncoding = (String)graphDBType.GetSettingValue(DBConstants.SettingUUIDEncoding, dbContext.SessionSettings, dbContext.DBTypeManager).Value.Value;

            //if (settingEncoding == null)
            //{
            //    throw new GraphDBException(new Error_ArgumentNullOrEmpty("settingEncoding"));
            //}

            //switch (settingEncoding.ToLower())
            //{
            //    case "utf8":
            //    case "hex":

            //        return new Exceptional<ADBBaseObject>(new DBString((String)Extract(dbObjectStream, settingEncoding)));

            //    case "ulong":

            //        return new Exceptional<ADBBaseObject>(new DBUInt64((UInt64)Extract(dbObjectStream, settingEncoding)));

            //    default:
            //        return new Exceptional<ADBBaseObject>(new Error_InvalidTuple("The parameter " + settingEncoding + " is not valid for the " + DBConstants.SettingUUIDEncoding + " Setting."));
            //}

        }

        private object Extract(DBObjectStream myObject, params object[] myOptionalParameters)
        {
            //if (myOptionalParameters.Count() == 0)

                return myObject.ObjectUUID.ToString();

            //else
            //    return ConvertFromUUID(myObject.ObjectUUID, ((String)myOptionalParameters[0]).ToLower());
        }

        #endregion

        #region ApplyTo

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myNewDBObject"></param>
        /// <param name="myValue"></param>
        /// <param name="myOptionalParameters">the type - 'ulong', 'hex', 'utf8'</param>
        public override Exceptional ApplyTo(DBObjectStream myNewDBObject, Object myValue, params object[] myOptionalParameters)
        {

            #region inputException

            if (myValue.ToString().Length == 0)
            {
                return new Exceptional(new Error_InvalidAttributeValue(this.Name, myValue));
            }

            #endregion


            //if (myOptionalParameters.Count() == 0)
            //{
                //UnitTestHelper.SetPrivateProperty("ObjectUUID", myNewDBObject, new ObjectUUID(ByteArrayHelper.FromUTF8String(myValue.ToString())));
                UnitTestHelper.SetPrivateProperty("ObjectUUID", myNewDBObject, new ObjectUUID(myValue.ToString()));
                myNewDBObject.ObjectLocation = new ObjectLocation(myNewDBObject.ObjectPath, myNewDBObject.ObjectUUID.ToString());
            //}
            //else
            //{

                //String param = ((String)myOptionalParameters[0]).ToLower();

                //switch (param)
                //{

                //    case "utf8":

                //        UnitTestHelper.SetPrivateProperty("ObjectUUID", myNewDBObject, new ObjectUUID(ByteArrayHelper.FromUTF8String(myValue.ToString())));
                //        myNewDBObject.ObjectLocation = new ObjectLocation(myNewDBObject.ObjectPath, myNewDBObject.ObjectUUID.ToString());

                //        break;

                //    case "hex":

                //        Byte[] newUUID;

                //        try
                //        {
                //            newUUID = ByteArrayHelper.FromHexString((String)myValue, true);
                //        }
                //        catch
                //        {
                //            return new Exceptional(new Error_InvalidAttributeValue(this.Name, (String)myValue));
                //        }

                //        UnitTestHelper.SetPrivateProperty("ObjectUUID", myNewDBObject, new ObjectUUID(newUUID));
                //        myNewDBObject.ObjectLocation = new ObjectLocation(myNewDBObject.ObjectPath, myNewDBObject.ObjectUUID.ToString());

                //        break;

                //    case "ulong":

                //        ulong newUlongUUID = 0;

                //        try
                //        {
                //            newUlongUUID = Convert.ToUInt64(myValue);
                //        }
                //        catch (Exception e)
                //        {
                //            return new Exceptional(new Error_UnknownDBError(e));
                //        }

                //        UnitTestHelper.SetPrivateProperty("ObjectUUID", myNewDBObject, new ObjectUUID(newUlongUUID));
                //        myNewDBObject.ObjectLocation = new ObjectLocation(myNewDBObject.ObjectPath, myNewDBObject.ObjectUUID.ToString());

                //        break;

                //    default:
                //        return new Exceptional(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "The parameter " + param + " is not valid for the " + this.Name + " Setting."));
                //}

            //}

            return Exceptional.OK;
        }

        #endregion

    }
}
