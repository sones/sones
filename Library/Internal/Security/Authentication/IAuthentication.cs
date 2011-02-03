using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Security
{
    /// <summary>
    /// Authentication interface
    /// User and GraphElement authentication
    /// </summary>
    public interface IAuthentication : IUserAuthentication, IGraphElementAuthentication
    {
        
    }
}
