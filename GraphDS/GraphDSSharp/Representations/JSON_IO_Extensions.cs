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

/* 
 * JSON_IO_Extensions
 * Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using Newtonsoft.Json.Linq;
using sones.GraphDB.Structures.Result;

#endregion

namespace sones.GraphDS.API.CSharp
{

    /// <summary>
    /// Extension methods to transform a QueryResult and a DBObjectReadout into an
    /// application/json representation an vice versa.
    /// </summary>

    public static class JSON_IO_Extensions
    {

        #region ToJSON(this myQueryResult)

        public static JObject ToJSON(this QueryResult myQueryResult)
        {
            return new JSON_IO().Export(myQueryResult) as JObject;
        }

        #endregion

        #region ToJSON(this myDBObjectReadout)

        private static JObject ToJSON(this DBObjectReadout myDBObjectReadout, Boolean myRecursion = false)
        {
            return new JSON_IO().Export(myDBObjectReadout) as JObject;
        }

        #endregion

        //private static JObject FromJSON(this DBObjectReadout myDBObjectReadout, Boolean myRecursion = false)
        //{
        //    return null;
        //}


    }

}
