#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Errors;
using sones.GraphDB.TypeManagement.PandoraTypes;

#endregion

namespace sones.GraphDB.TypeManagement.SpecialTypeAttributes
{
    public class SpecialTypeAttribute_NUMBEROFREVISIONS : ASpecialTypeAttribute
    {
        #region AttributeUUID

        public static AttributeUUID AttributeUUID = new AttributeUUID(12);

        #endregion

        #region Name

        public static String AttributeName = "NUMBER_OF_REVISIONS";

        #endregion

        public override string ShowSettingName
        {
            get
            {
                return "NUMBER_OF_REVISIONS";
            }
        }

        #region constructors

        public SpecialTypeAttribute_NUMBEROFREVISIONS()
        {
            _Name = AttributeName;
            _UUID = AttributeUUID;
        }

        #endregion
        
        public override Exceptional ApplyTo(DBObjectStream myNewDBObject, object myValue, params object[] myOptionalParameters)
        {
            return new Exceptional(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        public override Exceptional<AObject> ExtractValue(DBObjectStream dbObjectStream, GraphDBType graphDBType, DBContext dbContext)
        {            
            return new Exceptional<AObject>(new DBUInt64(dbObjectStream.NumberOfRevisions));
        }
    }
}
