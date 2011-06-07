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

using System.Security.Principal;
using System.Net;

namespace sones.Library.Network.HttpServer
{
    /// <summary>
    /// An implementation of this interface represents an authentification algorithm.
    /// </summary>
    public interface IServerSecurity
    {
        /// <summary>
        /// Authentificates the identity.
        /// </summary>
        /// <param name="myIdentity">An identity, that will be used for authentification.</param>
        /// <exception cref="System.IdentityModel.Tokens.SecurityTokenValidationException">If the validation fails.</exception>
        /// <exception cref="System.ArgumentNullException">If myIdentity is <c>NULL</c>.</exception>
        void Authentificate(IIdentity myIdentity);

        /// <summary>
        /// Gets the authentification schema, depending on the request.
        /// </summary>
        /// <param name="myRequest">The entire request.</param>
        /// <returns>The authentification schema this implementation needs.</returns>
        AuthenticationSchemes SchemaSelector(HttpListenerRequest myRequest);
    }
}
