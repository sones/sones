using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Result;
using sones.Library.Internal.Definitions;
using sones.Library.Internal.Security;

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
        /// Transforms the output header into the desired result
        /// </summary>
        private readonly Func<OutputHeader, TResult> _outputConverter = null;

        /// <summary>
        /// The result of the request
        /// </summary>
        private OutputHeader _result = null;

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
        public RequestCreateVertexType(VertexTypeDefinition myVertexTypeDefinition, Func<OutputHeader, TResult> myOutputConverter)
        {
            _outputConverter = myOutputConverter;
            VertexTypeDefinition = myVertexTypeDefinition;
        }

        #endregion

        #region IRequest<TResult> Members

        public TResult GenerateResult()
        {
            return _outputConverter(_result);
        }

        public GraphDBAccessModeEnum AccessMode
        {
            get { return GraphDBAccessModeEnum.TypeChange; }
        }

        #endregion
    }
}
