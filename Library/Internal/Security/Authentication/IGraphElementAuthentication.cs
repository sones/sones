using System;

namespace sones.Library.Security
{
    /// <summary>
    /// The interface for the graph element authentication
    /// </summary>
    public interface IGraphElementAuthentication
    {
        /// <summary>
        /// Authentication for vertices
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myToBeCheckedVertexID">The vertex that should be authenticated</param>
        /// <param name="myCorrespondingVertexTypeID">The vertex that should be authenticated</param>
        /// <param name="myWantedAction">The requested action</param>
        /// <returns>True for successful authentication, otherwise false</returns>
        Boolean Authenticate(SecurityToken mySecurityToken,
                             UInt64 myToBeCheckedVertexID,
                             UInt64 myCorrespondingVertexTypeID,
                             Right myWantedAction = Right.Traverse);
    }
}