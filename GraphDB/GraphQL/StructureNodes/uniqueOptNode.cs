
#region Usings

using System;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class uniqueOptNode : AStructureNode, IAstNodeInit
    {

        private Boolean _IsUnique;
        public Boolean IsUnique
        {
            get { return _IsUnique; }
        }

        #region GetContent(myCompilerContext, myParseTreeNode)

        private void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            if (myParseTreeNode.HasChildNodes())
                _IsUnique = true;

            else
                _IsUnique = false;

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
