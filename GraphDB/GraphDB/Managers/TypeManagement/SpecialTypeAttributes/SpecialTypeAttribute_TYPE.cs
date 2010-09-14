using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.ObjectManagement;
using sones.GraphFS.Session;
using sones.GraphFS.Session;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;

using sones.GraphDB.TypeManagement;
using System.Diagnostics;

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

        public override Exceptional<IObject> ExtractValue(DBObjectStream dbObjectStream, GraphDBType graphDBType, DBContext dbContext)
        {
            var myType = dbContext.DBTypeManager.GetTypeByUUID(dbObjectStream.TypeUUID);
            if (myType != null)
            {
                return new Exceptional<IObject>(new DBString(myType.Name));
            }
            else
            {
                return new Exceptional<IObject>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
            }
        }

        public override Exceptional<Object> ApplyTo(DBObjectStream myNewDBObject, object myValue, params object[] myOptionalParameters)
        {
            return new Exceptional<Object>(new Error_NotImplemented(new StackTrace(true)));
        }

    }
}
