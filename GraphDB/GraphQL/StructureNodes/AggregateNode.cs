/* <id name="GraphDB – aggregate node" />
 * <copyright file="AggregateNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of an aggregate statement.</summary>
 */

#region Usings

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of an aggregate statement.
    /// </summary>
    public class AggregateNode : FuncCallNode
    {

        public AggregateDefinition AggregateDefinition { get; private set; }

        #region constructor

        public AggregateNode()
        {
            
        }

        #endregion

        public new void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            
            base.GetContent(context, parseNode);

            AggregateDefinition = new AggregateDefinition(new ChainPartAggregateDefinition(base.FuncDefinition));

        }

    }

}
