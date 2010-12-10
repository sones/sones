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
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.Managers.Structures
{
    /// <summary>
    /// This is the abstract base class for all attribute manipulations
    /// </summary>
    public abstract class AAttributeAssignOrUpdateOrRemove
    {

        #region Properties

        /// <summary>
        /// The attribute chain definition
        /// </summary>
        public IDChainDefinition AttributeIDChain { get; protected set; }

        #endregion

        #region abstract Update

        /// <summary>
        /// Abstract definition for an attribute update
        /// </summary>
        /// <param name="myDBContext">The db context</param>
        /// <param name="myDBObjectStream">The db objectstream</param>
        /// <param name="myGraphDBType">The type of the db objectstream</param>
        /// <returns>An exceptional with the update result</returns>
        public abstract Exceptional<Dictionary<String, Tuple<TypeAttribute, IObject>>> Update(DBContext myDBContext, DBObjectStream myDBObjectStream, GraphDBType myGraphDBType);

        #endregion

        #region Protected methods

        #region BackwardEdges (Remove)

        /// <summary>
        /// Abstract class to remove all references to a deleted db object
        /// </summary>
        /// <param name="myTypeUUID">Type type UUID of the db object</param>
        /// <param name="myUserdefinedAttributes">The userdefined attributes of the db object</param>
        /// <param name="myObjectUUIDReference">The object uuid of the db object</param>
        /// <pararm name="myDBContext">The db context</pararm>
        /// <returns>An exceptional</returns>
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

        /// <summary>
        /// Load undefined attributes from a db objectstream
        /// </summary>
        /// <param name="myName">Name of the undefined attribute</param>
        /// <param name="myDBContext">The db context</param>
        /// <param name="myDBObjStream">The DBObjectstream that contains the undefined attribute</param>
        /// <returns>An excpetional with a dictionary of the undefined attributes</returns>
        protected Exceptional<IDictionary<String, IObject>> LoadUndefAttributes(String myName, DBContext myDBContext, DBObjectStream myDBObjStream)
        {

            var loadExcept = myDBObjStream.GetUndefinedAttributePayload(myDBContext.DBObjectManager);

            if (loadExcept.Failed())
                return new Exceptional<IDictionary<string, IObject>>(loadExcept);

            return new Exceptional<IDictionary<string, IObject>>(loadExcept.Value);

        }

        #endregion

        #endregion

        #region IsUndefinedAttributeAssign

        /// <summary>
        /// Return true if the attribute is undefined
        /// </summary>
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
