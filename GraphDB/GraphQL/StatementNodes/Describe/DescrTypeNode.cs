/* <id name="DescrTypeNode" />
 * <copyright file="DescrTypeNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using sones.GraphDB.Managers.Structures.Describe;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class DescribeTypeNode : ADescrNode
    {

        #region ADescrNode

        public override ADescribeDefinition DescribeDefinition
        {
            get { return _DescribeTypeDefinition; }
        }
        private DescribeTypeDefinition _DescribeTypeDefinition;

        #endregion

        #region ASructureNode
        
        public void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            _DescribeTypeDefinition = new DescribeTypeDefinition(myParseTreeNode.ChildNodes[1].Token.ValueString);
            
        }        

        #endregion

    }

}
