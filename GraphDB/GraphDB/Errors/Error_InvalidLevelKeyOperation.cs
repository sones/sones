/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


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
