using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Internal.Token;
using sones.GraphDB.Request;
using sones.Library.Internal.Security;

namespace sones.GraphDB.Context
{
    /// <summary>
    /// The abstract class for all hg sessions
    /// </summary>
    public abstract class AGraphDBSession : IGraphDBSession
    {
        #region Data

        /// <summary>
        /// The session token for the current session
        /// </summary>
        protected readonly SessionToken _SessionToken;

        /// <summary>
        /// The graphDB corresponding to this session
        /// </summary>
        protected readonly IGraphDB _iGraphDB;

        #endregion

        #region constructor

        /// <summary>
        /// Create a new AGraphDBSession
        /// </summary>
        /// <param name="myCredentials">The credentials of the session</param>
        /// <param name="myHyperGraph">The current graphdb implementation</param>
        protected AGraphDBSession(ICredentials myCredentials, IGraphDB myHyperGraph)
        {
            _SessionToken = new SessionToken(myCredentials);
            _iGraphDB = myHyperGraph;
        }

        #endregion


        #region IGraphDBSession Members

        public abstract TResult CreateVertexType<TResult>(RequestCreateVertexType<TResult> myRequestCreateVertexType, TransactionToken myTransactionToken = null);

        public abstract TResult Clear<TResult>(RequestClear<TResult> myRequestClear, TransactionToken myTransactionToken = null);

        #endregion
    }
}
