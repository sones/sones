using System;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// A request for creating a new vertex
    /// </summary>
    /// <typeparam name="TResult">The result type</typeparam>
    public sealed class RequestInsertVertex<TResult> : IRequest<TResult>
    {

        #region data

        /// <summary>
        /// Transforms the statistics into the desired result
        /// </summary>
        private readonly Func<RequestStatistics, TResult> _outputConverter;

        /// <summary>
        /// The stats of the request
        /// </summary>
        private RequestStatistics _stats = null;

        /// <summary>
        /// The definition of the vertex that is going to be inserted
        /// </summary>
        public readonly VertexInsert VertexInsertDefinition = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request that clears the Graphdb
        /// </summary>
        /// <param name="myVertexInsertDefinition">Describes the vertex that is going to be inserted</param>
        /// <param name="myOutputConverter">A function that convertes the result into the desired output</param>
        public RequestInsertVertex(VertexInsert myVertexInsertDefinition, Func<RequestStatistics, TResult> myOutputConverter)
        {
            _outputConverter = myOutputConverter;
            VertexInsertDefinition = myVertexInsertDefinition;
        }

        #endregion

        #region IRequest<TResult> Members

        public TResult GenerateResult()
        {
            return _outputConverter(_stats);
        }

        public GraphDBAccessModeEnum AccessMode
        {
            get { return GraphDBAccessModeEnum.ReadWrite; }
        }

        public void SetStatistics(IRequestStatistics myRequestStatistics)
        {
            _stats = myRequestStatistics as RequestStatistics;
        }

        #endregion
    }
}
