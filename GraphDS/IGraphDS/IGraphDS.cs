using System;
using sones.GraphFS;
using sones.Library.Internal.Token;
using sones.GraphDB;
using sones.GraphQL;
using sones.Library.Internal.Security;
using sones.GraphQL.Result;

namespace sones.GraphDS
{
    /// <summary>
    /// The interface for all graphDS
    /// </summary>
    public interface IGraphDS : IGraphDB, IQueryableLanguage
    {
        /// <summary>
        /// Shutdown of the current database
        /// </summary>
        /// <param name="mySessionToken"></param>
        void Shutdown(SessionToken mySessionToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myCredentials"></param>
        /// <returns></returns>
        SessionToken Authenticate(ICredentials myCredentials);
    }
}
