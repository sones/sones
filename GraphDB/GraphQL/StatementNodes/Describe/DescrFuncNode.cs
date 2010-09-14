/* <id name="DescrFuncNode" />
 * <copyright file="DescrFuncNode.cs"
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

    public class DescrFuncNode : ADescrNode
    {

        #region ADescrNode

        public override ADescribeDefinition DescribeDefinition
        {
            get { return _DescribeFuncDefinition; }
        }
        private DescribeFuncDefinition _DescribeFuncDefinition;

        #endregion

        #region AStructureNode

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            _DescribeFuncDefinition = new DescribeFuncDefinition(parseNode.ChildNodes[1].Token.ValueString.ToUpper());
        }
       
        #endregion
    
    }

}
