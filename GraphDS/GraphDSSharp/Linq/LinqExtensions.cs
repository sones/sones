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
 * GraphDSSharp - LinqExtensions
 * (c) Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDS.API.CSharp.Linq
{

    /// <summary>
    /// Extension classes for the GraphDSSharp API to provide LINQ queries
    /// </summary>
    public static class LinqExtensions
    {


        #region LinqQuery<T>(this myAGraphDSSharp)

        public static LinqQueryable<T> LinqQuery<T>(this AGraphDSSharp myAGraphDSSharp)
            where T : Vertex, new()
        {
            return myAGraphDSSharp.LinqQuery<T>("");
        }

        #endregion

        #region LinqQuery<T>(this myAGraphDSSharp, myTypeAlias)

        public static LinqQueryable<T> LinqQuery<T>(this AGraphDSSharp myAGraphDSSharp, String myTypeAlias)
            where T : Vertex, new()
        {
            return new LinqQueryable<T>(new LinqQueryProvider(myAGraphDSSharp, typeof(T), myTypeAlias));
        }

        #endregion


    }

}
