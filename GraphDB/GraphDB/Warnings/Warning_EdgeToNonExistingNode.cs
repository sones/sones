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
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;

namespace sones.GraphDB.Warnings
{
    public class Warning_EdgeToNonExistingNode : GraphDBWarning
    {
        public DBObjectStream StartingNode { get; set; }
        public IEnumerable<IError> Errors { get; set; }
        public TypeAttribute Edge { get; set; }
        public GraphDBType TypeOfDBO { get; set; }

        public Warning_EdgeToNonExistingNode(DBObjectStream myStartingNode, GraphDBType myTypeOfDBO, TypeAttribute myEdge, IEnumerable<IError> myErrors)
        {
            StartingNode = myStartingNode;
            Errors = myErrors;
            Edge = myEdge;
            TypeOfDBO = myTypeOfDBO;
        }

        public override string ToString()
        {
            String ErrorString = "";
            foreach (var aError in Errors)
            {
                ErrorString += aError.ToString() + Environment.NewLine;
            }

            return String.Format("Error while loading the edge \"{0}\" of the Node with UUID \"{1}\" of type \"{2}\". " + Environment.NewLine + "Errors:" + Environment.NewLine + "{3}", Edge.Name, StartingNode.ObjectUUID, TypeOfDBO.Name, ErrorString);
        }
    }
}
