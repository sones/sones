
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
