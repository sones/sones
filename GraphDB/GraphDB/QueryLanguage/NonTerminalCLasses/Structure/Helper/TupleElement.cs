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
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;


namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{

    public class TupleElement
    {

        #region Data

        Object _Value = null;
        TypesOfOperatorResult _TypeOfValue;
        public HashSet<ObjectUUID> CorrespondingDBObjectUUIDS { get; set; }
        public AEdgeType Edges { get; set; }
        public List<ADBBaseObject> Parameters { get; set; }

        #endregion

        public TupleElement(TypesOfOperatorResult TypeOfValue, Object Value)
        {
            _TypeOfValue = TypeOfValue;
            _Value = Value;
            Parameters = new List<ADBBaseObject>();
        }

        public Object Value { get { return _Value; } }

        public TypesOfOperatorResult TypeOfValue { get { return _TypeOfValue; } }

        public override string ToString()
        {
            return String.Concat("[", _TypeOfValue.ToString(), "] ", _Value.ToString(), "{", _Value.GetType().Name, "}");
        }

    }
}
