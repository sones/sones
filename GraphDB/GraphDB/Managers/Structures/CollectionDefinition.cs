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
 * CollectionDefinition
 * (c) Stefan Licht, 2010
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Errors;

namespace sones.GraphDB.Managers.Structures
{
    public enum CollectionType
    {
        Set,
        List,
        SetOfUUIDs
    }

    public class CollectionDefinition
    {

        #region Properties

        public CollectionType CollectionType { get; private set; }
        public TupleDefinition TupleDefinition { get; private set; }

        #endregion

        #region Ctor

        public CollectionDefinition(CollectionType myCollectionType, TupleDefinition myTupleDefinition)
        {
            CollectionType = myCollectionType;
            TupleDefinition = myTupleDefinition;
        }

        #endregion

        #region GetEdge

        /// <summary>
        /// Get the content of the CollectionDefinition as an edge
        /// </summary>
        /// <param name="myTypeAttribute"></param>
        /// <param name="myGraphDBType"></param>
        /// <param name="myDBContext"></param>
        /// <returns></returns>
        public Exceptional<ASetOfReferencesEdgeType> GetEdge(TypeAttribute myTypeAttribute, GraphDBType myGraphDBType, DBContext myDBContext)
        {

            if (CollectionType == CollectionType.List || !(myTypeAttribute.EdgeType is ASetOfReferencesEdgeType))
            {
                return new Exceptional<ASetOfReferencesEdgeType>(new Error_InvalidAssignOfSet(myTypeAttribute.Name));
            }

            #region The Edge is empty

            if (TupleDefinition == null)
            {
                return new Exceptional<ASetOfReferencesEdgeType>(myTypeAttribute.EdgeType.GetNewInstance() as ASetOfReferencesEdgeType);
            }

            #endregion

            Exceptional<ASetOfReferencesEdgeType> uuids = null;
            if (CollectionType == CollectionType.SetOfUUIDs)
            {
                uuids = TupleDefinition.GetAsUUIDEdge(myDBContext, myTypeAttribute);
                if (uuids.Failed())
                {
                    return new Exceptional<ASetOfReferencesEdgeType>(uuids);
                }
            }
            else
            {
                uuids = TupleDefinition.GetCorrespondigDBObjectUUIDAsList(myGraphDBType, myDBContext, (ASetOfReferencesEdgeType)myTypeAttribute.EdgeType, myGraphDBType);
                if (CollectionType == CollectionType.Set)
                {
                    if (uuids.Success())
                    {
                        uuids.Value.Distinction();
                    }
                }
            }
            return uuids;

        }

        #endregion

    }
}
