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

using System.Collections.Generic;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;
using System;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// Static converter class
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myVertexCount">The count of vertices</param>
        /// <returns>A TResult</returns>
        public delegate TResult GetVertexCountResultConverter<out TResult>(IRequestStatistics myRequestStatistics, UInt64 myVertexCount);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myCreatedVertex">The vertex that has been created</param>
        /// <returns>A TResult</returns>
        public delegate TResult InsertResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IVertex myCreatedVertex);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <returns>A TResult</returns>
        public delegate TResult ClearResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IEnumerable<long> myDeletetVertexTypes);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <returns>A TResult</returns>
        public delegate TResult DeleteResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IEnumerable<IComparable> myDeletedAttributes, IEnumerable<IComparable> myDeletedVertices);
        
        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myCreatedVertexTypes">The vertex types that have been created</param>
        /// <returns>A TResult</returns>
        public delegate TResult CreateVertexTypesResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IEnumerable<IVertexType> myCreatedVertexTypes);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myCreatedVertexTypes">The vertex type that has been altered</param>
        /// <returns>A TResult</returns>
        public delegate TResult AlterVertexTypeResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IVertexType myAlteredVertexType);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myCreatedVertexTypes">The edge type that has been altered</param>
        /// <returns>A TResult</returns>
        public delegate TResult AlterEdgeTypeResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IEdgeType myAlteredEdgeType);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myCreatedVertexType">The vertex type that has been created</param>
        /// <returns>A TResult</returns>
        public delegate TResult CreateVertexTypeResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IVertexType myCreatedVertexType);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myCreatedVertexType">The edge type that has been created</param>
        /// <returns>A TResult</returns>
        public delegate TResult CreateEdgeTypeResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IEdgeType myCreatedVertexType);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myVertices">The vertices that have been fetched from the GraphDB</param>
        /// <returns>A TResult</returns>
        public delegate TResult GetVerticesResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IEnumerable<IVertex> myVertices);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myVertices">The vertices that have been collected from the GraphDB</param>
        /// <returns>A TResult</returns>
        public delegate TResult TraverseVertexResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IEnumerable<IVertex> myVertices);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myVertexType">The VertexType that has been fetched from the GraphDB</param>
        /// <returns>A TResult</returns>
        public delegate TResult GetVertexTypeResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IVertexType myVertexType);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myVertexType">The VertexType that has been fetched from the GraphDB</param>
        /// <returns>A TResult</returns>
        public delegate TResult GetAllVertexTypesResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IEnumerable<IVertexType> myVertexTypes);
        
        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myEdgeType">The EdgeType that has been fetched from the GraphDB</param>
        /// <returns>A TResult</returns>
        public delegate TResult GetEdgeTypeResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IEdgeType myEdgeType);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myEdgeType">The EdgeType that has been fetched from the GraphDB</param>
        /// <returns>A TResult</returns>
        public delegate TResult GetAllEdgeTypesResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IEnumerable<IEdgeType> myEdgeTypes);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myVertex">The vertex that has been fetched from the GraphDB</param>
        /// <returns>A TResult</returns>
        public delegate TResult GetVertexResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IVertex myVertex);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myVertex">The vertex that has been truncated</param>
        /// <returns>A TResult</returns>
        public delegate TResult TruncateResultConverter<out TResult>(IRequestStatistics myRequestStatistics);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myIndexDefinitons">The describing index definitions</param>
        /// <returns>A TResult</returns>
        public delegate TResult DescribeIndexResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IEnumerable<IIndexDefinition> myIndexDefinitons);
        
        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myIndexDefinitons">The describing index definitions</param>
        /// <returns>A TResult</returns>
        public delegate TResult DescribeIndicesResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IEnumerable<IIndexDefinition> myIndexDefinitons);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myVertexType">The updated vertex type</param>
        /// <returns>A TResult</returns>
        public delegate TResult UpdateResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IEnumerable<IVertex> myVertexType);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <returns>A TResult</returns>
        public delegate TResult DropVertexTypeResultConverter<out TResult>(IRequestStatistics myRequestStatistics, Dictionary<Int64, String> myDeletedTypeIDs);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <returns>A TResult</returns>
        public delegate TResult DropIndexResultConverter<out TResult>(IRequestStatistics myRequestStatistics);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <returns>A TResult</returns>
        public delegate TResult CreateIndexResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IIndexDefinition myIndexDefinition);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <returns>A TResult</returns>
        public delegate TResult RebuildIndicesResultConverter<out TResult>(IRequestStatistics myRequestStatistics);
    }
}
