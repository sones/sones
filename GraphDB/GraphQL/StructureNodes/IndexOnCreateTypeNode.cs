
#region Usings

using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class IndexOnCreateTypeNode : AStructureNode
    {

        #region Properties

        public List<IndexDefinition> ListOfIndexDefinitions { get; private set; }

        #endregion

        #region constructors

        public IndexOnCreateTypeNode()
        { }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            ListOfIndexDefinitions = new List<IndexDefinition>();

            if (parseNode.ChildNodes[1].AstNode is IndexOptOnCreateTypeMemberNode)
            {
                var aIDX = (IndexOptOnCreateTypeMemberNode)parseNode.ChildNodes[1].AstNode;
                ParsingResult.Push(aIDX.ParsingResult);

                ListOfIndexDefinitions.Add(aIDX.IndexDefinition);
            }

            else
            {
                var idcs = parseNode.ChildNodes[1].ChildNodes.Select(child =>
                {
                    ParsingResult.Push(((IndexOptOnCreateTypeMemberNode)child.AstNode).ParsingResult);
                    return ((IndexOptOnCreateTypeMemberNode)child.AstNode).IndexDefinition;
                });
                ListOfIndexDefinitions.AddRange(idcs);
            }

        }


    }

}
