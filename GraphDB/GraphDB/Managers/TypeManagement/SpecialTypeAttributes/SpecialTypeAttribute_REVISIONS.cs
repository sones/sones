#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.Errors;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Structures.EdgeTypes;

#endregion

namespace sones.GraphDB.TypeManagement.SpecialTypeAttributes
{
    public class SpecialTypeAttribute_REVISIONS : ASpecialTypeAttribute
    {

        #region AttributeUUID

        public static AttributeUUID AttributeUUID = new AttributeUUID(10);

        #endregion

        #region Name

        public static String AttributeName = "REVISIONS";

        #endregion

        public override string ShowSettingName
        {
            get
            {
                return "REVISIONS";
            }
        }

        #region constructors 

        public SpecialTypeAttribute_REVISIONS()
        {
            Name = AttributeName;
            UUID = AttributeUUID;
        }

        #endregion

        public override Exceptional ApplyTo(DBObjectStream myNewDBObject, object myValue, params object[] myOptionalParameters)
        {
            return new Exceptional(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        public override Exceptional<IObject> ExtractValue(DBObjectStream dbObjectStream, GraphDBType graphDBType, DBContext dbContext)
        {
            EdgeTypeListOfBaseObjects revisions = new EdgeTypeListOfBaseObjects();
            
            foreach (var item in dbObjectStream.ObjectRevisions)
                revisions.Add(new DBString(item.Value));

            return new Exceptional<IObject>(revisions);
        }

    }

}
