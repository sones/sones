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
 * AAlterTypeCommand
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Diagnostics;
using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.NewAPI;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures.Enums;

using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Managers.AlterType
{

    public abstract class AAlterTypeCommand
    {

        public abstract TypesOfAlterCmd AlterType { get; }

        public virtual Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return new Exceptional(new Error_NotImplemented(new StackTrace(true), GetType().Name));
        }

        /// <summary>
        /// Create a command specific readout. May return null if no readout applies.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="graphDBType"></param>
        /// <returns>A readout or null</returns>
        public virtual IEnumerable<Vertex> CreateVertex(DBContext dbContext, GraphDBType graphDBType)
        {
            //throw new GraphDBException(new Errors.Error_NotImplemented(new System.Diagnostics.StackTrace(true), GetType().Name));
            return null;
        }

        protected IEnumerable<Vertex> CreateRenameResult(String myAlterAction, String myFromString, String myToString, GraphDBType myType)
        {

            var payload = new Dictionary<String, Object>();

            payload.Add("TYPE",   myType);
            payload.Add("ACTION", myAlterAction);
            payload.Add("FROM",   myFromString);
            payload.Add("TO",     myToString);

            return new List<Vertex>(){new Vertex(payload)};

        }

    }

}
