/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Errors;
using sones.GraphDB.Managers.Select;
using sones.GraphDB.ObjectManagement;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidEdgeListOperation : GraphDBError
    {
        private EdgeList _EdgeList1;
        private EdgeList _EdgeList2;
        private EdgeKey _EdgeKey;
        private string _Operation;

        public Error_InvalidEdgeListOperation(EdgeList myEdgeList, ObjectManagement.EdgeKey myEdgeKey, string myOperation)
        {
            _EdgeList1 = myEdgeList;
            this._EdgeKey = myEdgeKey;
            this._Operation = myOperation;
        }

        public Error_InvalidEdgeListOperation(EdgeList myEdgeList1, EdgeList myEdgeList2, string myOperation)
        {
            _EdgeList1 = myEdgeList1;
            _EdgeList2 = myEdgeList2;
            this._Operation = myOperation;
        }

        public override string ToString()
        {
            if (_EdgeKey != null)
            {
                return String.Format("EdgeList operation [{0}] is not valid for [{1}] and [{2}] ", _Operation, _EdgeList1, _EdgeKey);
            }
            else
            {
                return String.Format("EdgeList operation [{0}] is not valid for [{1}] and [{2}] ", _Operation, _EdgeList1, _EdgeList2);
            }
        }

    }
}
