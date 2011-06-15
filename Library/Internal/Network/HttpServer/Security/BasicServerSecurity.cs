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
using System.IdentityModel.Selectors;
using System.Security.Principal;
using System.Net;
using System.IdentityModel.Tokens;

namespace sones.Library.Network.HttpServer.Security
{
    public class BasicServerSecurity: IServerSecurity
    {
        #region Data

        private readonly UserNamePasswordValidator _validator;

        #endregion

        #region c'tor

        public BasicServerSecurity(UserNamePasswordValidator myValidator)
        {
            if (myValidator == null)
                throw new ArgumentNullException("myValidator");
            _validator = myValidator;
        }

        #endregion

        #region IServerSecurity Members

        public AuthenticationSchemes SchemaSelector(HttpListenerRequest myRequest)
        {
            return AuthenticationSchemes.Basic;
        }

        public void Authentificate(IIdentity myIdentity)
        {
            if (myIdentity == null)
                throw new ArgumentNullException("myIdentity");

            var identity = myIdentity as HttpListenerBasicIdentity;

            if (identity == null)
                throw new SecurityTokenValidationException("The authentification scheme was not basic.");

            _validator.Validate(identity.Name, identity.Password);
        }

        #endregion


    }
}
