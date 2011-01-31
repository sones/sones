using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphInfrastructure.Element;

namespace sones.Library.Internal.Security
{
    /// <summary>
    /// Authentication interface
    /// User and GraphElement authentication
    /// </summary>
    public interface IAuthentication : IUserAuthentication, IGraphElementAuthentication
    {
        
    }
}
