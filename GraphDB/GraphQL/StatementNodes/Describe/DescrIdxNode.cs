/* <id name="DescrIdxNode" />
 * <copyright file="DescrIdxNode.cs"
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

    public class DescribeIndexNode : ADescrNode
    {

        #region ADescrNode

        public override ADescribeDefinition DescribeDefinition
        {
            get { return _DescribeIndexDefinition; }
        }
        private DescribeIndexDefinition _DescribeIndexDefinition;

        #endregion

        #region AStructureNode

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            string type = parseNode.ChildNodes[1].ChildNodes[0].Token.ValueString;
            string indexName = parseNode.ChildNodes[1].ChildNodes[1].Token.ValueString;
            string edition = null;
            if (parseNode.ChildNodes[2].HasChildNodes())
            {
                edition = parseNode.ChildNodes[2].ChildNodes[0].Token.ValueString;
            }

            _DescribeIndexDefinition = new DescribeIndexDefinition(type, indexName, edition);
            
        }       
        #endregion

    }

}
