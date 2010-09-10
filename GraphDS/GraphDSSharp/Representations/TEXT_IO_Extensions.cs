/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* 
 * TEXT_IO_Extensions
 * Achim 'ahzf' Friedland, 2009-2010
 */

#region Usings

using System;
using System.Text;

#endregion

namespace sones.GraphDB.QueryLanguage.Result
{

    /// <summary>
    /// Extension methods to transform a QueryResult and a DBObjectReadout into a text/plain representation
    /// </summary>

    public static class TEXT_IO_Extensions
    {

        #region ToTEXT(this myQueryResult)

        public static String ToTEXT(this QueryResult myQueryResult)
        {
            return (new TEXT_IO().Export(myQueryResult) as StringBuilder).ToString();
        }

        #endregion

        #region ToTEXT(this myQueryResult, myStringBuilder)

        public static String ToTEXT(this QueryResult myQueryResult, StringBuilder myStringBuilder)
        {
            return new TEXT_IO().Export(myQueryResult, myStringBuilder).ToString();
        }

        #endregion


        #region ToTEXT(this myDBObjectReadout)

        private static String ToTEXT(this DBObjectReadout myDBObjectReadout)
        {
            return new TEXT_IO().Export(myDBObjectReadout) as String;
        }

        #endregion

        #region ToTEXT(this myDBObjectReadout, myStringBuilder)

        private static String ToTEXT(this DBObjectReadout myDBObjectReadout, StringBuilder myStringBuilder)
        {
            return (new TEXT_IO().Export(myDBObjectReadout, myStringBuilder) as StringBuilder).ToString();
        }

        #endregion

    }

}
