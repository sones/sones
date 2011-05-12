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
using System.Linq;
using System.Text;

namespace sones.Library.Commons.VertexStore.Definitions.Update
{
    /// <summary>
    /// The definition of the direction of a single edge
    /// </summary>
    public sealed class SingleEdgeDeleteDefinition
    {
        #region data

        /// <summary>
        /// The source vertex.
        /// </summary>
        public readonly VertexInformation SourceVertex;
        
        /// <summary>
        /// The target vertex.
        /// </summary>
        public readonly VertexInformation TargetVertex;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new instance of SingleEdgeDeleteDefinition.
        /// </summary>
        /// <param name="mySourceVertex">The vertex where the edge begins.</param>
        /// <param name="myTargetVertex">The vertex where the edge ends.</param>
        public SingleEdgeDeleteDefinition(VertexInformation mySourceVertex, VertexInformation myTargetVertex)
        {
            SourceVertex = mySourceVertex;
            TargetVertex = myTargetVertex;
        }

        #endregion
    }
}
