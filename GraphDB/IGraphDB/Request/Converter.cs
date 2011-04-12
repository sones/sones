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
        /// <param name="myCreatedVertexType">The vertex type that has been created</param>
        /// <returns>A TResult</returns>
        public delegate TResult CreateVertexTypeResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IEnumerable<IVertexType> myCreatedVertexType);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myVertices">The vertices that have been fetched from the GraphDB</param>
        /// <returns>A TResult</returns>
        public delegate TResult GetVerticesResultConverter<out TResult>(IRequestStatistics myRequestStatistics, IEnumerable<IVertex> myVertices);
    }
}
