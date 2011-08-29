/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;

namespace sones.GraphDB.Manager.TypeManagement
{
    public class IDManager
    {
        private readonly Dictionary<Int64, UniqueID> _vertexIDs;
        private readonly Dictionary<Int64, UniqueID> _edgeIDs;

        public IDManager()
        {
            VertexTypeID = new UniqueID();
            EdgeTypeID = new UniqueID();

            _vertexIDs = new Dictionary<long, UniqueID>();
            _edgeIDs = new Dictionary<long, UniqueID>();
        }

        public UniqueID VertexTypeID { get; private set; }
        public UniqueID EdgeTypeID { get; private set; }

        public UniqueID GetVertexTypeUniqeID(long myVertexTypeID)
        {
            if (!_vertexIDs.ContainsKey(myVertexTypeID))

                lock (_vertexIDs)
                {
                    if (!_vertexIDs.ContainsKey(myVertexTypeID))
                    {
                        _vertexIDs[myVertexTypeID] = new UniqueID();
                    }
                }

            return _vertexIDs[myVertexTypeID];
        }

        public UniqueID GetEdgeTypeUniqeID(long myEdgeTypeID)
        {
            if (!_edgeIDs.ContainsKey(myEdgeTypeID))

                lock (_edgeIDs)
                {
                    if (!_edgeIDs.ContainsKey(myEdgeTypeID))
                    {
                        _edgeIDs[myEdgeTypeID] = new UniqueID();
                    }
                }

            return _edgeIDs[myEdgeTypeID];
        }
    }
}
