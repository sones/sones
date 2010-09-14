using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Errors;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;

namespace sones.GraphDB.Errors
{
    public class Error_CouldNotLoadBackwardEdge : GraphDBBackwardEdgeError
    {
        public DBObjectStream DBObject { get; private set; }
        public TypeAttribute Attribute { get; private set; }
        public IEnumerable<IError> Errors { get; private set; }

        public Error_CouldNotLoadBackwardEdge(DBObjectStream myDBObject, TypeAttribute myTypeAttribute, IEnumerable<IError> myErrors)
        {
            DBObject = myDBObject;
            Attribute = myTypeAttribute;
            Errors = myErrors;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var aError in Errors)
	        {
                sb.AppendLine(aError.ToString());
	        }

            return String.Format("It was not possible to load the BackwardEdge for DBObject \"{0}\" on TypeAttribute \"{1}\". The following Errors occourred:" + Environment.NewLine + "{2}", DBObject, Attribute, sb.ToString());
        }
    }
}
