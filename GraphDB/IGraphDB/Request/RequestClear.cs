using System;
using System.Collections.Generic;
using sones.GraphDB.Result;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// A request for clearing the whole graphdb
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class RequestClear<TResult> : IRequest<TResult>
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

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request that clears the Graphdb
        /// </summary>
        /// <param name="myOutputConverter">A function that convertes the result into the desired output</param>
        public RequestClear(Func<OutputHeader, TResult> myOutputConverter)
        {
            _outputConverter = myOutputConverter;
        }

        #endregion

        #region IRequest<TResult> Members

        public TResult GenerateResult()
        {
            return _outputConverter(_result);
        }

        #endregion
    }
}
