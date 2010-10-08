#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using sones.GraphDB.ObjectManagement;

using sones.GraphDB.Errors;
using sones.GraphFS.DataStructures;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.TypeManagement;
using System.Diagnostics;

#endregion

namespace sones.GraphDB.TypeManagement.SpecialTypeAttributes
{
    public class SpecialTypeAttribute_PARENTREVISIONS : ASpecialTypeAttribute
    {

        #region AttributeUUID

        public static AttributeUUID AttributeUUID = new AttributeUUID(14);

        #endregion

        #region Name

        public static String AttributeName = "PARENT_REVISIONS";

        #endregion

        #region constructors

        public SpecialTypeAttribute_PARENTREVISIONS()
        {
            Name = AttributeName;
            UUID = AttributeUUID;
        }

        #endregion

        public override string ShowSettingName
        {
            get
            {
                return "PARENT_REVISIONS";
            }
        }

        public override Exceptional<Object> ApplyTo(DBObjectStream myNewDBObject, object myValue, params object[] myOptionalParameters)
        {
            return new Exceptional<Object>(new Error_NotImplemented(new StackTrace(true)));
        }

        public override Exceptional<IObject> ExtractValue(DBObjectStream dbObjectStream, GraphDBType graphDBType, DBContext dbContext)
        {
        
            EdgeTypeListOfBaseObjects parentRevisions = new EdgeTypeListOfBaseObjects();

            foreach (var item in dbObjectStream.ParentRevisionIDs)
                parentRevisions.Add(new DBString(item.ToString()));

            return new Exceptional<IObject>(parentRevisions);

        }

    }

}
