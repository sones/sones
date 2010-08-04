/*
 * AAttributeAssignOrUpdateOrRemove
 * (c) Stefan Licht, 2010
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.GraphFS.DataStructures;
using System.Diagnostics;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Result;

namespace sones.GraphDB.Managers.Structures
{
    public abstract class AAttributeAssignOrUpdateOrRemove
    {

        #region Properties

        public IDChainDefinition AttributeIDChain { get; protected set; }

        #endregion

        #region abstract Update

        public abstract Exceptional<Dictionary<String, Tuple<TypeAttribute, AObject>>> Update(DBContext myDBContext, DBObjectStream myDBObjectStream, GraphDBType myGraphDBType);

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

                #region get PandoraType of Attribute

                attributesOfType = aType.Attributes[aUserDefinedAttribute.Key];

                typeOFAttribute = myDBContext.DBTypeManager.GetTypeByUUID(attributesOfType.DBTypeUUID);

                #endregion


                IEnumerable<Exceptional<DBObjectStream>> listOfObjects;

                if (aUserDefinedAttribute.Value is IReferenceEdge)
                {
                    listOfObjects = myDBContext.DBObjectCache.LoadListOfDBObjectStreams(typeOFAttribute, ((IReferenceEdge)aUserDefinedAttribute.Value).GetAllReferenceIDs());
                }
                else
                {
                    listOfObjects = myDBContext.DBObjectCache.LoadListOfDBObjectStreams(typeOFAttribute, (HashSet<ObjectUUID>)aUserDefinedAttribute.Value);
                }

                foreach (var aDBObject in listOfObjects)
                {
                    if (aDBObject.Failed)
                    {
                        return new Exceptional(aDBObject);
                    }

                    var removeExcept = myDBContext.DBObjectManager.RemoveBackwardEdge(aDBObject.Value, myTypeUUID, aUserDefinedAttribute.Key, myObjectUUIDReference);

                    if (removeExcept.Failed)
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

        protected Exceptional<IDictionary<String, AObject>> LoadUndefAttributes(String myName, DBContext dbContext, DBObjectStream myObjStream)
        {
            var loadExcept = myObjStream.GetUndefinedAttributes(dbContext.DBObjectManager);

            if (loadExcept.Failed)
                return new Exceptional<IDictionary<string, AObject>>(loadExcept);

            return new Exceptional<IDictionary<string, AObject>>(loadExcept.Value);
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
