using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph.Helper;

namespace sones.GraphQL.GQL.ErrorHandling
{
    public sealed class InvalidLevelKeyOperationException : AGraphQLException
    {
        public LevelKey LevelKeyA { get; private set; }
        public LevelKey LevelKeyB { get; private set; }
        public EdgeKey EdgeKeyA { get; private set; }
        public EdgeKey EdgeKeyB { get; private set; }
        public String Operation { get; private set; }

        public InvalidLevelKeyOperationException(LevelKey myLevelKey, EdgeKey myEdgeKey, String myOperation)
        {
            LevelKeyA = myLevelKey;
            EdgeKeyA = myEdgeKey;
            Operation = myOperation;
        }

        public InvalidLevelKeyOperationException(LevelKey myLevelKeyA, LevelKey myLevelKeyB, String myOperation)
        {
            LevelKeyA = myLevelKeyA;
            LevelKeyB = myLevelKeyB;
            Operation = myOperation;
        }

        public override string ToString()
        {
            if (LevelKeyA != null && LevelKeyB != null)
                return String.Format("Invalid Operation '{0}' {1} '{2}'", LevelKeyA, Operation, LevelKeyB);
            if (EdgeKeyA != null && EdgeKeyB != null)
                return String.Format("Invalid Operation '{0}' {1} '{2}'", EdgeKeyA, Operation, EdgeKeyB);

            return String.Format("Invalid Operation '{0}' {1} '{2}'", LevelKeyA, Operation, EdgeKeyA);
        }
    }
}
