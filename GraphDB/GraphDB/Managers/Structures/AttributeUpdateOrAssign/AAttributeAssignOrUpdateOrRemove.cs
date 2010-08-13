/*
 * AAttributeAssignOrUpdateOrRemove
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;

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
