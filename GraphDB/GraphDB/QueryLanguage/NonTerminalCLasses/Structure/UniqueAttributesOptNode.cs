/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.Lib.Frameworks.Irony.Parsing;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class UniqueAttributesOptNode : AStructureNode, IAstNodeInit
    {

        private List<String> _UniqueAttributes;
        public List<String> UniqueAttributes
        {
            get { return _UniqueAttributes; }
        }
       
        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            if (parseNode.HasChildNodes())
            {

                _UniqueAttributes = new List<string>();

                if (parseNode.ChildNodes[1].HasChildNodes())
                {
                    _UniqueAttributes = (from c in parseNode.ChildNodes[1].ChildNodes select c.Token.ValueString).ToList();
                }
            }
        }
        
        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion
    }
}
