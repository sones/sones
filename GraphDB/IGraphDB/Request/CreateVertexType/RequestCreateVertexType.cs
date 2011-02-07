using System;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// A request for creating a new vertex type
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class RequestCreateVertexType<TResult> : IRequest<TResult>
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
        /// The definition of the vertex that is going to be created
        /// </summary>
        public readonly VertexTypeDefinition VertexTypeDefinition = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request that clears the Graphdb
        /// </summary>
        /// <param name="myVertexTypeDefinition">Describes the vertex that is going to be created</param>
        /// <param name="myOutputConverter">A function that convertes the result into the desired output</param>
        public RequestCreateVertexType(VertexTypeDefinition myVertexTypeDefinition, Func<RequestStatistics, TResult> myOutputConverter)
        {
            _outputConverter = myOutputConverter;
            VertexTypeDefinition = myVertexTypeDefinition;
        }

        #endregion

        #region IRequest<TResult> Members

        public TResult GenerateResult()
        {
            return _outputConverter(_stats);
        }

        public GraphDBAccessModeEnum AccessMode
        {
            get { return GraphDBAccessModeEnum.TypeChange; }
        }

        public void SetStatistics(IRequestStatistics myRequestStatistics)
        {
            _stats = myRequestStatistics as RequestStatistics;
        }

        #endregion
    }
}
