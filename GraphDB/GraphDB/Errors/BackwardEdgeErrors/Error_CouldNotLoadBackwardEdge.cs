/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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
