using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.ExpressionGraph;
using sones.GraphDB.ObjectManagement;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidLevelKeyOperation : GraphDBError
    {
        public LevelKey LevelKeyA { get; private set; }
        public LevelKey LevelKeyB { get; private set; }
        public EdgeKey EdgeKeyA { get; private set; }
        public EdgeKey EdgeKeyB { get; private set; }
        public String Operation { get; private set; }

        public Error_InvalidLevelKeyOperation(LevelKey myLevelKey, EdgeKey myEdgeKey, String myOperation)
        {
            LevelKeyA = myLevelKey;
            EdgeKeyA = myEdgeKey;
            Operation = myOperation;
        }

        public Error_InvalidLevelKeyOperation(LevelKey myLevelKeyA, LevelKey myLevelKeyB, String myOperation)
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
