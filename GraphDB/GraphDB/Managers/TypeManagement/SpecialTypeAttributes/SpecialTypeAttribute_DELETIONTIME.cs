#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.Session;

using sones.GraphDB.TypeManagement;
using System.Diagnostics;
using sones.GraphDB.Errors;

#endregion

namespace sones.GraphDB.TypeManagement.SpecialTypeAttributes
{
    public class SpecialTypeAttribute_DELETIONTIME : ASpecialTypeAttribute 
    {

        #region AttributeUUID

        public static AttributeUUID AttributeUUID = new AttributeUUID(8);

        #endregion

        #region Name

        public static String AttributeName = "DELETIONTIME";

        #endregion

        #region constructors

        public SpecialTypeAttribute_DELETIONTIME()
        {
            Name = AttributeName;
            UUID = AttributeUUID;
        }
        
        #endregion

        public override string ShowSettingName
        {
            get { return "DELETIONTIME"; }
        }

        public override Exceptional<Object> ApplyTo(DBObjectStream myNewDBObject, object myValue, params object[] myOptionalParameters)
        {
            return new Exceptional<Object>(new Error_NotImplemented(new StackTrace(true)));
        }

        public override Exceptional<IObject> ExtractValue(DBObjectStream dbObjectStream, GraphDBType graphDBType, DBContext dbContext)
        {
            return new Exceptional<IObject>(new DBUInt64(dbObjectStream.INodeReference.DeletionTime));
        }

    }
}
