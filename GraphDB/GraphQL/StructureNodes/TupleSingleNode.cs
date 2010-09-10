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


#region Usings

using System;
using System.Linq;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class TupleSingleNode : AStructureNode, IAstNodeInit
    {

        TupleElement _TupleElement;
        public TupleElement TupleElement
        {
            get { return _TupleElement; }
        }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var tupleNode = new TupleNode();
            tupleNode.Init(context, parseNode);
            if (tupleNode.TupleDefinition.Count() != 1)
            {
                throw new GraphDBException(new Error_InvalidTuple( "Only 1 element allowed but found " + tupleNode.TupleDefinition.Count().ToString()));
            }

            _TupleElement = tupleNode.TupleDefinition.First();
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}
