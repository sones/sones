using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Security
{
    /// <summary>
    /// The interface for all security managers
    /// Authentication & integrity & encryption
    /// </summary>
    public interface ISecurityManager : IAuthentication, IIntegrity, IEncryption
    {
       
    }
}
