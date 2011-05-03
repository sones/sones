using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Nodes.Misc;
namespace sones.GraphQL.Structure.Nodes.Expressions
{
    /// <summary>
    /// This node is requested in case of an aggregate statement.
    /// </summary>
    public sealed class AggregateNode : FuncCallNode
    {
        public AggregateDefinition AggregateDefinition { get; private set; }

        #region constructor

        public AggregateNode()
        {

        }

        #endregion

        public void Aggregate_Init(ParsingContext context, ParseTreeNode parseNode)
        {
            base.Init(context, parseNode);

            AggregateDefinition = new AggregateDefinition(new ChainPartAggregateDefinition(base.FuncDefinition));
        }
    }
}
