using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.TypeSystem;

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
        public delegate TResult InsertResultConverter<TResult>(IRequestStatistics myRequestStatistics, IVertex myCreatedVertex);

        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <returns>A TResult</returns>
        public delegate TResult ClearResultConverter<TResult>(IRequestStatistics myRequestStatistics);
        
        /// <summary>
        /// A converter delegate that produces a generic result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myRequestStatistics">The statistics of the request</param>
        /// <param name="myCreatedVertexType">The vertex type that has been created</param>
        /// <returns>A TResult</returns>
        public delegate TResult CreateVertexTypeResultConverter<TResult>(IRequestStatistics myRequestStatistics, IVertexType myCreatedVertexType);
    }
}
