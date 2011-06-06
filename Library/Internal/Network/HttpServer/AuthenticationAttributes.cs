/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;

namespace sones.Library.Network.HttpServer
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
