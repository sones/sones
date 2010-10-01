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

/* 
 * GraphDB
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

#endregion

namespace sones.GraphDB.NewAPI
{

    public class DBPath
    {

        #region Properties

        public Vertex              StartVertex    { get; protected set; }
        public Vertex              EndVertex      { get; protected set; }
        public UInt64                Length         { get; protected set; }
        public IEnumerable<Vertex> Vertices       { get; protected set; }
        public IEnumerable<Edge> Edges { get; protected set; }

        #endregion

        public DBPath(Vertex myStartVertex, Vertex myEndVertex, IEnumerable<Vertex> myVertices, IEnumerable<Edge> myEdges)
        {

            #region Initial Checks

            if (myStartVertex == null)
                throw new ArgumentNullException();

            if (myEndVertex == null)
                throw new ArgumentNullException();

            if (myVertices == null)
                throw new ArgumentNullException();

            if (myEdges == null)
                throw new ArgumentNullException();

            if (myVertices.Count() != myEdges.Count() - 1)
                throw new ArgumentException();

            #endregion

            StartVertex = myStartVertex;
            EndVertex   = myEndVertex;
            Vertices    = myVertices;
            Edges       = myEdges;
            Length      = (UInt64) myEdges.LongCount();

        }

    }

}
