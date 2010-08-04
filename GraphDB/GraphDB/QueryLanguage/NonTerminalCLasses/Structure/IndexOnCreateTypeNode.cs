#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class IndexOnCreateTypeNode : AStructureNode
    {
        #region Data

        private List<Exceptional<IndexDefinition>> _ListOfIndices;

        #endregion

        #region constructors

        public IndexOnCreateTypeNode()
        { }

        #endregion

        public Exceptional GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            _ListOfIndices = new List<Exceptional<IndexDefinition>>();

            if (parseNode.ChildNodes[1].AstNode is Exceptional<IndexOptOnCreateTypeMemberNode>)
            {
                var aIDX = (Exceptional<IndexOptOnCreateTypeMemberNode>)parseNode.ChildNodes[1].AstNode;

                var idx = (parseNode.ChildNodes[1].AstNode as Exceptional<IndexOptOnCreateTypeMemberNode>).ConvertWithFunc<IndexOptOnCreateTypeMemberNode, IndexDefinition>(i => i.IndexDefinition);
                _ListOfIndices.Add(idx);
            }
            else
            {
                var idcs = parseNode.ChildNodes[1].ChildNodes.Select(child => ((Exceptional<IndexOptOnCreateTypeMemberNode>)child.AstNode).ConvertWithFunc<IndexOptOnCreateTypeMemberNode, IndexDefinition>(i => i.IndexDefinition));
                _ListOfIndices.AddRange(idcs);
            }

            return Exceptional.OK;
        }


        #region Accessors

        public List<Exceptional<IndexDefinition>> ListOfIndexDefinitions
        {
            get { return _ListOfIndices; }
        }

        #endregion

    }
}
