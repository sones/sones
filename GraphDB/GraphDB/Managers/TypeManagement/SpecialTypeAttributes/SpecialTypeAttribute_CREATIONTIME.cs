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
using sones.GraphDB.Errors;
using System.Diagnostics;

#endregion

namespace sones.GraphDB.TypeManagement.SpecialTypeAttributes
{
    public class SpecialTypeAttribute_CREATIONTIME : ASpecialTypeAttribute 
    {

        #region AttributeUUID

        public static AttributeUUID AttributeUUID = new AttributeUUID(5);

        #endregion

        #region Name

        public static String AttributeName = "CREATIONTIME";

        #endregion
        
        #region constructors

        public SpecialTypeAttribute_CREATIONTIME()
        {
            Name = AttributeName;
            UUID = AttributeUUID;
        }

        #endregion


        public override string ShowSettingName
        {
            get { return "CREATIONTIME"; }
        }

        public override Exceptional<Object> ApplyTo(DBObjectStream myNewDBObject, object myValue, params object[] myOptionalParameters)
        {
            return new Exceptional<Object>(new Error_NotImplemented(new StackTrace(true)));
        }

        public override Exceptional<IObject> ExtractValue(DBObjectStream dbObjectStream, GraphDBType graphDBType, DBContext dbContext)
        {
            return new Exceptional<IObject>(new DBUInt64(dbObjectStream.INodeReference.CreationTime));
        }

    }
}
