using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.UUID;

namespace sones.GraphDB.Structures.ExpressionGraph
{
    public class ExpressionLevelEntry : IExpressionLevelEntry
    {

        Dictionary<ObjectUUID, IExpressionNode> _Objects;
        LevelKey _CorrespondingLevelKey;

        public ExpressionLevelEntry(LevelKey myCorrespondingLevelKey, Dictionary<ObjectUUID, IExpressionNode> myObjects)
        {
            _Objects = myObjects;
            _CorrespondingLevelKey = myCorrespondingLevelKey;
        }

        public ExpressionLevelEntry(LevelKey myCorrespondingLevelKey)
            : this(myCorrespondingLevelKey, new Dictionary<ObjectUUID, IExpressionNode>())
        { }

        #region IExpressionLevelEntry Members

        public Dictionary<ObjectUUID, IExpressionNode> Nodes
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
