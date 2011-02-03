using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.PropertyHyperGraph;

namespace sones.GraphDB.Security
{
    /// <summary>
    /// The interface for the graph element authentication
    /// </summary>
    public interface IGraphElementAuthentication
    {
        /// <summary>
        /// Authentication for graph elements
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myToBeCheckedGraphElement">The graph element that should be authenticated</param>
        /// <param name="myWantedAction">The requested action</param>
        /// <returns>True for successful authentication, otherwise false</returns>
        Boolean Authenticate(SecurityToken mySecurityToken, 
            IGraphElementID myToBeCheckedGraphElement, 
            Right myWantedAction = Right.Traverse);
    }
}
