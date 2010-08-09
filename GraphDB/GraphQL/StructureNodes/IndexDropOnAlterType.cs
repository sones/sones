
#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class IndexDropOnAlterType : AStructureNode
    {

        #region Data

        private Dictionary<String, String> _DropIndexList;
        
        #endregion

        #region constructors

        public IndexDropOnAlterType()
        { }
        
        #endregion

        public Exceptional GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            _DropIndexList = new Dictionary<String, String>();

            try
            {
                if (parseNode.ChildNodes[1].ChildNodes.Count == 2 && parseNode.ChildNodes[1].ChildNodes.TrueForAll(item => !item.HasChildNodes()))
                {
                    var idxName = parseNode.ChildNodes[1].ChildNodes[0].Token.Text;
                    var idxEdition = ((EditionOptNode)parseNode.ChildNodes[1].ChildNodes[1].AstNode).IndexEdition;

                    if (!_DropIndexList.ContainsKey(idxName))
                    {
                        _DropIndexList.Add(idxName, idxEdition);
                    }
                }
                else
                {
                    foreach (var nodes in parseNode.ChildNodes[1].ChildNodes)
                    { 
                        var idxName = nodes.ChildNodes[0].Token.Text;
                        var idxEdition = ((EditionOptNode)nodes.ChildNodes[1].AstNode).IndexEdition;

                        if (!_DropIndexList.ContainsKey(idxName))
                        {
                            _DropIndexList.Add(idxName, idxEdition);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var error = new Error_UnknownDBError(e);
                
                return new Exceptional(error);
            }
            
            return Exceptional.OK;
        }

        #region Accessors

        public Dictionary<String, String> DropIndexList
        {
            get { return _DropIndexList; }
        }

        #endregion

    }

}
