using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphInfrastructure.Element;

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
