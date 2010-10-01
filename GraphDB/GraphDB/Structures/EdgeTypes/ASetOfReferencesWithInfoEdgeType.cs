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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphFS.DataStructures;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.ObjectManagement;
using sones.Lib.ErrorHandling;


namespace sones.GraphDB.Structures.EdgeTypes
{
    public abstract class ASetOfReferencesWithInfoEdgeType : ASetOfReferencesEdgeType
    {

        /// <summary>
        /// Get all uuids and their edge infos
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Tuple<ObjectUUID, ADBBaseObject>> GetAllReferenceIDsWeighted();

        /// <summary>
        /// Get all weighted destinations of an edge
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Tuple<Exceptional<DBObjectStream>, ADBBaseObject>> GetAllEdgeDestinationsWeighted(DBObjectCache dbObjectCache);


        protected ulong GetBaseSize()
        {
            return base.GetBaseSize();
        }
    }
}
