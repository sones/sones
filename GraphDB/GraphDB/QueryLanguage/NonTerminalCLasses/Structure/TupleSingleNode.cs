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
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
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
            if (tupleNode.Tuple.Count != 1)
            {
                throw new GraphDBException(new Error_InvalidTuple( "Only 1 element allowed but found " + tupleNode.Tuple.Count.ToString()));
            }

            _TupleElement = tupleNode.Tuple[0];
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
