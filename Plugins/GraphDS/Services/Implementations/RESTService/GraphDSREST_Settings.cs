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


#region Usings

using System;
using sones.Library.LanguageExtensions;

#endregion

namespace sones.GraphDS.Services.RESTService
{

    public class GraphDSREST_Settings
    {

        public String Username { get; set; }
        //public String Password { get; set; }
        public String DBName { get; set; }
        public GraphDSREST_OutputFormat OutputFormat { get; set; }

        public override String ToString()
        {
            return String.Format("user={0},password={1},db={2},outputformat={3}", "", "", DBName, OutputFormat);
        }

        public String ToBase64()
        {
            return ToString().ToBase64();
        }

    }

}
