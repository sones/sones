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
    public class SpecialTypeAttribute_NUMBEROFCOPIES : ASpecialTypeAttribute
    {

        #region AttributeUUID

        public static AttributeUUID AttributeUUID = new AttributeUUID(13);

        #endregion

        #region Name

        public static String AttributeName = "NUMBER_OF_COPIES";

        #endregion

        public override string ShowSettingName
        {
            get
            {
                return "NUMBER_OF_COPIES";
            }
        }

        #region constructors 

        public SpecialTypeAttribute_NUMBEROFCOPIES()
        {
            Name = AttributeName;
            UUID = AttributeUUID;
        }

        #endregion

        public override Exceptional ApplyTo(DBObjectStream myNewDBObject, object myValue, params object[] myOptionalParameters)
        {
            return new Exceptional(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        public override Exceptional<AObject> ExtractValue(DBObjectStream dbObjectStream, GraphDBType graphDBType, DBContext dbContext)
        {
            return new Exceptional<AObject>(new DBUInt64(dbObjectStream.NumberOfCopies));
        }

    }
}
