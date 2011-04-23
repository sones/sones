using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.GQL.Structure.Helper.ExpressionGraph
{
    public sealed class ExpressionLevelEntry : IExpressionLevelEntry
    {

        Dictionary<Int64, IExpressionNode> _Objects;
        LevelKey _CorrespondingLevelKey;

        public ExpressionLevelEntry(LevelKey myCorrespondingLevelKey, Dictionary<Int64, IExpressionNode> myObjects)
        {
            _Objects = myObjects;
            _CorrespondingLevelKey = myCorrespondingLevelKey;
        }

        public ExpressionLevelEntry(LevelKey myCorrespondingLevelKey)
            : this(myCorrespondingLevelKey, new Dictionary<Int64, IExpressionNode>())
        { }

        #region IExpressionLevelEntry Members

        public Dictionary<Int64, IExpressionNode> Nodes
        {
            get { return _Objects; }
        }

        public LevelKey CorrespondingLevelKey
        {
            get { return _CorrespondingLevelKey; }
        }

        #endregion

    }
}
