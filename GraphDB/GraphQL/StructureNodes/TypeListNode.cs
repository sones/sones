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

using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class TypeListNode : AStructureNode
    {
        
        #region Properties

        public List<TypeReferenceDefinition> Types { get; private set; }

        #endregion

        #region Constructor

        public TypeListNode()
        {
            Types = new List<TypeReferenceDefinition>();
        }

        #endregion

        #region GetContent(myCompilerContext, myParseTreeNode)

        public void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            if (myParseTreeNode.HasChildNodes())
            {
                foreach (var child in myParseTreeNode.ChildNodes)
                {
                    if (child.AstNode is ATypeNode)
                    {
                        var tr = (child.AstNode as ATypeNode).ReferenceAndType;
                        if (!Types.Contains(tr))
                        {
                            Types.Add(tr);
                        }
                        else
                        {
                            throw new GraphDBException(new Error_DuplicateReferenceOccurence(tr.TypeName));
                        }
                    }
                }
            }

        }

        #endregion

    }

}
