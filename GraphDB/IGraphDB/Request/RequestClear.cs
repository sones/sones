using System;
using System.Collections.Generic;

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
        private readonly Func<RequestStatistics, TResult> _outputGenerator;

        /// <summary>
        /// The request stats
        /// </summary>
        private RequestStatistics _stats = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request that clears the Graphdb
        /// </summary>
        public RequestClear(Func<RequestStatistics, TResult> myOutputGenerator)
        {
            _outputGenerator = myOutputGenerator;
        }

        #endregion

        #region IRequest<TResult> Members

        public TResult GenerateResult()
        {
            return _outputGenerator(_stats);
        }

        public GraphDBAccessModeEnum AccessMode
        {
            get { return GraphDBAccessModeEnum.WriteOnly; }
        }

        public void SetStatistics(IRequestStatistics myRequestStatistics)
        {
            _stats = myRequestStatistics as RequestStatistics;
        }

        #endregion
    }
}
