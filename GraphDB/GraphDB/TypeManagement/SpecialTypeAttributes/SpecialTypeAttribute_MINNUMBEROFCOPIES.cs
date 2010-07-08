using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Errors;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement.PandoraTypes;

namespace sones.GraphDB.TypeManagement.SpecialTypeAttributes
{
    public class SpecialTypeAttribute_MINNUMBEROFCOPIES : ASpecialTypeAttribute 
    {
        #region AttributeUUID

        public static AttributeUUID AttributeUUID = new AttributeUUID(19);

        #endregion

        #region Name

        public static String AttributeName = "MIN_NUMBER_OF_COPIES";

        #endregion
        
        #region construtors

        public SpecialTypeAttribute_MINNUMBEROFCOPIES()
        {
            _Name = AttributeName;
            _UUID = AttributeUUID;
        }

        #endregion

        public override string ShowSettingName
        {
            get
            {
                return "MIN_NUMBER_OF_COPIES";
            }
        }

        public override Exceptional ApplyTo(DBObjectStream myNewDBObject, object myValue, params object[] myOptionalParameters)
        {
            return new Exceptional(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        public override Exceptional<AObject> ExtractValue(DBObjectStream dbObjectStream, GraphDBType graphDBType, DBContext dbContext)
        {
            return new Exceptional<AObject>(new DBUInt64(dbObjectStream.MinNumberOfCopies));
        }
    }
}
