#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Errors;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement.BasicTypes;

#endregion

namespace sones.GraphDB.TypeManagement.SpecialTypeAttributes
{
    public class SpecialTypeAttribute_STREAMS : ASpecialTypeAttribute
    {

        #region AttributeUUID

        public static AttributeUUID AttributeUUID = new AttributeUUID(11);

        #endregion

        #region Name

        public static String AttributeName = "STREAMS";

        #endregion

        #region ShowSettingName

        public override String ShowSettingName
        {
            get
            {
                return "STREAMS";
            }
        }

        #endregion
        
        #region constructors

        public SpecialTypeAttribute_STREAMS()
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
            EdgeTypeListOfBaseObjects streams = new EdgeTypeListOfBaseObjects();

            foreach (var item in dbObjectStream.ObjectStreams)
                streams.Add(new DBString(item.Key + " " + item.Value.ToString()));

            return new Exceptional<IObject>(streams);
        }

    }

}
