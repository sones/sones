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
using System.Collections.Generic;

using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class UniqueAttributesOptNode : AStructureNode, IAstNodeInit
    {

        private List<String> _UniqueAttributes;
        public List<String> UniqueAttributes
        {
            get { return _UniqueAttributes; }
        }

        #region GetContent(myCompilerContext, myParseTreeNode)

        private void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            if (myParseTreeNode.HasChildNodes())
            {

                _UniqueAttributes = new List<String>();

                if (myParseTreeNode.ChildNodes[1].HasChildNodes())
                {
                    _UniqueAttributes = (from c in myParseTreeNode.ChildNodes[1].ChildNodes select c.Token.ValueString).ToList();
                }

            }

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

    }

}
