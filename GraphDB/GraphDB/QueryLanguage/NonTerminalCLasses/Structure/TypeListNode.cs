using System;
using System.Collections.Generic;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.Managers.Structures;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class TypeListNode : AStructureNode
    {
        
        #region Properties

        public List<TypeReferenceDefinition> Types { get; private set; }

        #endregion

        public TypeListNode()
        {
            Types = new List<TypeReferenceDefinition>();
        }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            if (parseNode.HasChildNodes())
            {
                foreach (var child in parseNode.ChildNodes)
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

    }
}
