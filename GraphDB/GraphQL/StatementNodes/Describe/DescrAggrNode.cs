/* <id name="DescrAggrNode" />
 * <copyright file="DescrAggrNode.cs"
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

    public class DescrAggrNode : ADescrNode
    {

        #region ADescrNode

        public override ADescribeDefinition DescribeDefinition
        {
            get { return _DescrAggrDefinition; }
        }

        private DescribeAggregateDefinition _DescrAggrDefinition;

        #endregion
        
        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
                        
            if (parseNode.HasChildNodes())
            {
                _DescrAggrDefinition = new DescribeAggregateDefinition(parseNode.ChildNodes[1].Token.ValueString.ToUpper());
            }

        }

    }

}
