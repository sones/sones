#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Errors;

using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.TypeManagement;
using System.Diagnostics;

#endregion

namespace sones.GraphDB.TypeManagement.SpecialTypeAttributes
{
    public class SpecialTypeAttribute_MAXNUMBEROFCOPIES : ASpecialTypeAttribute
    {

        #region AttributeUUID

        public static AttributeUUID AttributeUUID = new AttributeUUID(18);

        #endregion

        #region Name

        public static String AttributeName = "MAX_NUMBER_OF_COPIES";

        #endregion
        
        #region constructors

        public SpecialTypeAttribute_MAXNUMBEROFCOPIES()
        {
            Name = AttributeName;
            UUID = AttributeUUID;
        }

        #endregion
        
        public override string ShowSettingName
        {
            get
            {
                return "MAX_NUMBER_OF_COPIES";
            }
        }

        public override Exceptional<Object> ApplyTo(DBObjectStream myNewDBObject, object myValue, params object[] myOptionalParameters)
        {
            return new Exceptional<Object>(new Error_NotImplemented(new StackTrace(true)));
        }

        public override Exceptional<IObject> ExtractValue(DBObjectStream dbObjectStream, GraphDBType graphDBType, DBContext dbContext)
        {
            return new Exceptional<IObject>(new DBUInt64(dbObjectStream.MaxNumberOfCopies));
        }

    }
}
