using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Security
{
    /// <summary>
    /// Authentication interface
    /// User and GraphElement authentication
    /// </summary>
    public interface IAuthentication : IUserAuthentication, IGraphElementAuthentication
    {
        
    }
}
