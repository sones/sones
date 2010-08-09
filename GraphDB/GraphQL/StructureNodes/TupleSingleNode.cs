
#region Usings

using System;
using System.Linq;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class TupleSingleNode : AStructureNode, IAstNodeInit
    {

        TupleElement _TupleElement;
        public TupleElement TupleElement
        {
            get { return _TupleElement; }
        }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var tupleNode = new TupleNode();
            tupleNode.Init(context, parseNode);
            if (tupleNode.TupleDefinition.Count() != 1)
            {
                throw new GraphDBException(new Error_InvalidTuple( "Only 1 element allowed but found " + tupleNode.TupleDefinition.Count().ToString()));
            }

            _TupleElement = tupleNode.TupleDefinition.First();
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}
