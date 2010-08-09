
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
