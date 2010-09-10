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
 * AAttributeAssignOrUpdateOrRemove
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.ObjectManagement;

using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using sones.GraphDBInterface.TypeManagement;

#endregion

namespace sones.GraphDB.Managers.Structures
{
    public abstract class AAttributeAssignOrUpdateOrRemove
    {

        #region Properties

        public IDChainDefinition AttributeIDChain { get; protected set; }

        #endregion

        #region abstract Update

        public abstract Exceptional<Dictionary<String, Tuple<TypeAttribute, IObject>>> Update(DBContext myDBContext, DBObjectStream myDBObjectStream, GraphDBType myGraphDBType);

        #endregion

        #region Protected methods

        #region BackwardEdges (Remove)

        protected Exceptional RemoveBackwardEdges(TypeUUID myTypeUUID, Dictionary<AttributeUUID, object> myUserdefinedAttributes, ObjectUUID myObjectUUIDReference, DBContext myDBContext)
        {

            #region get type that carries the attributes

            var aType = myDBContext.DBTypeManager.GetTypeByUUID(myTypeUUID);

            #endregion

            #region process attributes

            foreach (var aUserDefinedAttribute in myUserdefinedAttributes)
            {

                #region Data

                GraphDBType typeOFAttribute = null;
                TypeAttribute attributesOfType = null;

                #endregion

                #region get GraphType of Attribute

                attributesOfType = aType.Attributes[aUserDefinedAttribute.Key];

                typeOFAttribute = myDBContext.DBTypeManager.GetTypeByUUID(attributesOfType.DBTypeUUID);

                #endregion


                IEnumerable<Exceptional<DBObjectStream>> listOfObjects;

                if (aUserDefinedAttribute.Value is IReferenceEdge)
                {
                    listOfObjects = ((IReferenceEdge)aUserDefinedAttribute.Value).GetAllEdgeDestinations(myDBContext.DBObjectCache);
                }
                else
                {
                    listOfObjects = myDBContext.DBObjectCache.LoadListOfDBObjectStreams(typeOFAttribute, (HashSet<ObjectUUID>)aUserDefinedAttribute.Value);
                }

                foreach (var aDBObject in listOfObjects)
                {
                    if (aDBObject.Failed())
                    {
                        return new Exceptional(aDBObject);
                    }

                    var removeExcept = myDBContext.DBObjectManager.RemoveBackwardEdge(aDBObject.Value, myTypeUUID, aUserDefinedAttribute.Key, myObjectUUIDReference);

                    if (removeExcept.Failed())
                    {
                        return new Exceptional(removeExcept);
                    }
                }

            }

            #endregion

            return Exceptional.OK;
        }

        #endregion

        #region Load undefined attributes

        protected Exceptional<IDictionary<String, IObject>> LoadUndefAttributes(String myName, DBContext dbContext, DBObjectStream myObjStream)
        {

            var loadExcept = myObjStream.GetUndefinedAttributes(dbContext.DBObjectManager);

            if (loadExcept.Failed())
                return new Exceptional<IDictionary<string, IObject>>(loadExcept);

            return new Exceptional<IDictionary<string, IObject>>(loadExcept.Value);

        }

        #endregion

        #endregion

        #region IsUndefinedAttributeAssign

        public bool IsUndefinedAttributeAssign
        {
            get
            {
                return AttributeIDChain == null || AttributeIDChain.IsUndefinedAttribute;
            }
        }
        
        #endregion

    }

    
}
