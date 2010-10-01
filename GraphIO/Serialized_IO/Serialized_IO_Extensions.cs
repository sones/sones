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
 * JSON_IO_Extensions
 * Achim 'ahzf' Friedland, 2009-2010
 */

#region Usings

using System;



#endregion

namespace sones.GraphIO.Serialized
{

    /// <summary>
    /// Extension methods to transform a QueryResult and a DBObjectReadout into an
    /// application/json representation an vice versa.
    /// </summary>

    public static class Serialized_IO_Extensions
    {

        //#region ToSerialized(this myQueryResult)

        //public static Object ToSerialized(this QueryResult myQueryResult)
        //{
        //    return new Serialized_IO().ExportQueryResult(myQueryResult) as Object;
        //}

        //#endregion

        //#region ToJSON(this myDBObjectReadout)

        //private static Object ToSerialized(this DBObjectReadout myDBObjectReadout, Boolean myRecursion = false)
        //{
        //    return new Serialized_IO().ExportVertex(myDBObjectReadout) as Object;
        //}

        //#endregion


    }

}
