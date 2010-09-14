using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.ObjectManagement;
using sones.GraphFS.Session;
using sones.GraphFS.Session;
using sones.Lib.ErrorHandling;

using sones.GraphDB.TypeManagement;
using System.Diagnostics;
using sones.GraphDB.Errors;

namespace sones.GraphDB.TypeManagement.SpecialTypeAttributes
{

    public class SpecialTypeAttribute_REVISION : ASpecialTypeAttribute
    {

        #region AttributeUUID

        public static AttributeUUID AttributeUUID = new AttributeUUID(2);

        #endregion

        #region Name

        public static String AttributeName = "REVISION";

        #endregion

        #region ShowSettingName

        public override String ShowSettingName
        {
            get
            {
                return "REVISION";
            }
        }

        #endregion

        public SpecialTypeAttribute_REVISION()
        {
            Name = AttributeName;
            UUID = AttributeUUID;
        }

        public override Exceptional<IObject> ExtractValue(DBObjectStream myDBObjectStream, GraphDBType myGraphDBType, DBContext myDBContext)
        {
            return new Exceptional<IObject>(new DBObjectRevisionID(myDBObjectStream.ObjectRevisionID));
        }

        public override Exceptional<Object> ApplyTo(DBObjectStream myNewDBObject, object myValue, params object[] myOptionalParameters)
        {
            return new Exceptional<Object>(new Error_NotImplemented(new StackTrace(true)));
        }

    }

}
