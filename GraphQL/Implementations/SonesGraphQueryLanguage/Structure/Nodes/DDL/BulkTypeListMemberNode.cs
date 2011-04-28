using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using System.Collections.Generic;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// This node is requested in case of an BulkTypeListMember node.
    /// </summary>
    public sealed class BulkTypeListMemberNode : AStructureNode, IAstNodeInit
    {
        #region Data

        private String  _TypeName = ""; //the name of the type that should be created
        private String _Extends = ""; //the name of the type that should be extended
        private String _Comment = ""; //the name of the type that should be extended
        private Boolean _IsAbstract = false;
        private Dictionary<AttributeDefinition, String> _Attributes = new Dictionary<AttributeDefinition, String>(); //the dictionayry of attribute definitions
        private List<BackwardEdgeDefinition> _BackwardEdgeInformation;
        private List<IndexDefinition> _Indices;

        #endregion

        #region Accessessors

        public String TypeName { get { return _TypeName; } }
        public String Extends { get { return _Extends; } }
        public String Comment { get { return _Comment; } }
        public Boolean IsAbstract { get { return _IsAbstract; } }
        public Dictionary<AttributeDefinition, String> Attributes { get { return _Attributes; } }
        public List<BackwardEdgeDefinition> BackwardEdges { get { return _BackwardEdgeInformation; } }
        public List<IndexDefinition> Indices { get { return _Indices; } }

        #endregion

        #region constructor

        public BulkTypeListMemberNode()
        {
        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region get Abstract

            if (HasChildNodes(parseNode.ChildNodes[0]))
                _IsAbstract = true;

            #endregion

            var bulkTypeNode = (BulkTypeNode)parseNode.ChildNodes[1].AstNode;

            #region get Name

            _TypeName = bulkTypeNode.TypeName;

            #endregion

            #region get Extends

            _Extends = bulkTypeNode.Extends;

            #endregion

            #region get myAttributes

            _Attributes = bulkTypeNode.Attributes;

            #endregion

            #region get BackwardEdges

            _BackwardEdgeInformation = bulkTypeNode.BackwardEdges;

            #endregion

            #region Get Optional Indices

            _Indices = bulkTypeNode.Indices;

            #endregion

            #region get Comment

            _Comment = bulkTypeNode.Comment;

            #endregion
        }

        #endregion
    }
}
