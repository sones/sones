using System.Collections.Generic;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;

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
        /// <param name="myCreatedVertex">The vertex that has been created</param>
        /// <returns>A TResult</returns>
        public delegate TResult InsertResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IVertex myCreatedVertex);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <returns>A TResult</returns>
        public delegate TResult ClearResultConverter<out TResult>(IRequestStatistics myRequestStatistics);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <returns>A TResult</returns>
        public delegate TResult DeleteResultConverter<out TResult>(IRequestStatistics myRequestStatistics);
        
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
        /// <param name="myVertexType">The updated vertex type</param>
        /// <returns>A TResult</returns>
        public delegate TResult UpdateResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IEnumerable<IVertex> myVertexType);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <returns>A TResult</returns>
        public delegate TResult DropVertexTypeResultConverter<out TResult>(IRequestStatistics myRequestStatistics);

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
        public delegate TResult RebuildIndicesResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IEnumerable<IIndexDefinition> myIndexDefinitions);
    }
}
