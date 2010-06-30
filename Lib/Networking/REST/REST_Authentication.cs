/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/



#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

#endregion

namespace sones.Networking.REST
{

    public class REST_Authentication : UserNamePasswordValidator
    {

        public override void Validate(String myUserName, String myPassword)
        {

            Console.WriteLine("Implement authentication for " + myUserName);

            if (String.IsNullOrEmpty(myUserName) || String.IsNullOrEmpty(myPassword))
                throw new SecurityTokenException("Validation Failed!");

        }

    }

}
