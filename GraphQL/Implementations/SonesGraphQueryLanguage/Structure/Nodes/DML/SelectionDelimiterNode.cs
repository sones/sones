using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class SelectionDelimiterNode : AStructureNode, IAstNodeInit
    {
        #region Data

        private KindOfDelimiter _KindOfDelimiter;

        #endregion

        #region constructor

        public SelectionDelimiterNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
        }

        #endregion

        #region public methods

        internal void SetDelimiter(KindOfDelimiter myKindOfDelimiter)
        {
            _KindOfDelimiter = myKindOfDelimiter;
        }

        public KindOfDelimiter GetKindOfDelimiter()
        {
            return _KindOfDelimiter;
        }

        public String GetDelimiterString()
        {
            switch (_KindOfDelimiter)
            {
                case KindOfDelimiter.Dot:
                    return SonesGQLConstants.EdgeTraversalDelimiterSymbol;

                case KindOfDelimiter.EdgeInformationDelimiter:
                    return SonesGQLConstants.EdgeInformationDelimiterSymbol;

                default:
                    throw new NotImplementedQLException("");
            }
        }

        #endregion
    }
}
