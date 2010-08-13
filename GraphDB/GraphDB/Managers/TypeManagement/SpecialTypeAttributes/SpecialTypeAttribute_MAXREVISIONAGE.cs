#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Errors;
using sones.GraphDB.TypeManagement.BasicTypes;

#endregion

namespace sones.GraphDB.TypeManagement.SpecialTypeAttributes
{
    public class SpecialTypeAttribute_MAXREVISIONAGE : ASpecialTypeAttribute
    {

        #region AttributeUUID

        public static AttributeUUID AttributeUUID = new AttributeUUID(15);

        #endregion

        #region Name

        public static String AttributeName = "MAX_REVISION_AGE";

        #endregion

        #region constructors

        public SpecialTypeAttribute_MAXREVISIONAGE()
        {
            Name = AttributeName;
            UUID = AttributeUUID;
        }

        #endregion

        public override string ShowSettingName
        {
            get
            {
                return "MAX_REVISION_AGE";
            }
        }        

        public override Exceptional ApplyTo(DBObjectStream myNewDBObject, object myValue, params object[] myOptionalParameters)
        {
            return new Exceptional(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        public override Exceptional<IObject> ExtractValue(DBObjectStream dbObjectStream, GraphDBType graphDBType, DBContext dbContext)
        {
            return new Exceptional<IObject>(new DBUInt64(dbObjectStream.MaxRevisionAge));
        }

    }
}
