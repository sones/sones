/* <id name="DescrEdgeNode" />
 * <copyright file="DescrEdgeNode.cs"
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

    public class DescribeEdgeNode : ADescrNode
    {

        #region ADescrNode

        public override ADescribeDefinition DescribeDefinition
        {
            get { return _DescribeEdgeDefinition; }
        }
        private DescribeEdgeDefinition _DescribeEdgeDefinition;

        #endregion

        #region AStructureNode        

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            _DescribeEdgeDefinition = new DescribeEdgeDefinition(parseNode.ChildNodes[1].Token.ValueString);
            
        }        
       
        #endregion

    }

}
