using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Networking.HTTP
{

    #region NeedsAuthenticationAttribute

    /// <summary>
    /// If set to True, this methods of a web interface definition needs authentication. If the server does not provide any, an exception will be thrown.
    /// If set to False, no authentication is required even if the server expect one.
    /// </summary>
    public class NeedsAuthenticationAttribute : Attribute
    {

        private Boolean _NeedsAuthentication;
        public Boolean NeedsAuthentication
        {
            get { return _NeedsAuthentication; }
        }

        /// <summary>
        /// If set to True, this methods of a web interface definition needs authentication. If the server does not provide any, an exception will be thrown.
        /// If set to False, no authentication is required even if the server expect one.
        /// </summary>
        /// <param name="needsAuthentication">If set to True, this methods of a web interface definition needs authentication. If the server does not provide any, an exception will be thrown. If set to False, no authentication is required even if the server expect one.</param>
        public NeedsAuthenticationAttribute(Boolean needsAuthentication)
        {
            _NeedsAuthentication = needsAuthentication;
        }

    }

    #endregion

    #region NoAuthenticationAttribute

    public class NoAuthenticationAttribute : Attribute
    {
        public NoAuthenticationAttribute()
        {
        }
    }

    #endregion

    #region ForceAuthenticationAttribute

    public class ForceAuthenticationAttribute : Attribute
    {
        public ForceAuthenticationAttribute()
        {
        }
    }

    #endregion

}
