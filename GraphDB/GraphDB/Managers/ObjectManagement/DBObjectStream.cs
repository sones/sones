/* <id Name=”GraphDB – database object” />
 * <copyright file=”DBObjectStream.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>The DBObject carries the myAttributes of a database object.<summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using sones.GraphDB.Errors;
using sones.GraphDB.Errors.DBObjectErrors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Result;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.GraphFS.Session;
using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.DataStructures;
using sones.Lib.DataStructures.Indices;
using sones.Lib.ErrorHandling;
using sones.Lib.NewFastSerializer;
using sones.Lib;

#endregion

namespace sones.GraphDB.ObjectManagement
{

    /// <summary>
    /// The DBObject carries the myAttributes of a database object.
    /// </summary>
    public class DBObjectStream : ADictionaryObject<AttributeUUID, IObject>
    {
        #region Properties

        public TypeUUID                  TypeUUID        { get; private set; }
        
        #endregion


        #region Constructor

        #region DBObjectStream()

        /// <summary>
        /// This will create an empty DBObject
        /// </summary>
        public DBObjectStream()
        {

            // Members of AGraphStructure
            _StructureVersion = 1;

            // Members of AGraphObject
            _ObjectStream = "DBOBJECTSTREAM";

            // Object specific data...

            // Set ObjectUUID
            if (ObjectUUID.Length == 0)
                ObjectUUID = base.ObjectUUID;

        }

        #endregion

        #region DBObjectStream(myObjectUUID, myDBTypeStream, myDBObjectAttributes)

        /// <summary>
        /// This will create an DBObject
        /// </summary>
        /// <param name="myObjectLocation">the location (object myPath and Name) of the requested DBObject within the file system</param>
        public DBObjectStream(ObjectUUID myObjectUUID, GraphDBType myDBTypeStream, Dictionary<AttributeUUID, IObject> myDBObjectAttributes, ObjectLocation myObjectLocation)
            :this()
        {

            Debug.Assert(myObjectUUID != null);

            if (myDBObjectAttributes == null)
                throw new ArgumentNullException("The parameter myDBObjectAttributes must not be null or empty!");

            base.Set(myDBObjectAttributes, IndexSetStrategy.REPLACE);

            TypeUUID = myDBTypeStream.UUID;

            #region estimated size

            _estimatedSize += EstimatedSizeConstants.CalcUUIDSize(TypeUUID);

            #endregion

            ObjectLocation = myObjectLocation;

            ObjectUUID = myObjectUUID;
        }

        #endregion

        #region DBObjectStream(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="myObjectLocation">the location (object myPath and Name) of the requested file within the file system</param>
        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized myAttributes of an DBObject</param>
        public DBObjectStream(ObjectLocation myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
        {

            if (mySerializedData == null || mySerializedData.Length == 0)
                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, false);
            _isNew = false;

            this.ObjectLocation = myObjectLocation;

        }

        #endregion

        #endregion


        #region Members of AGraphObject

        #region Clone()

        public override AFSObject Clone()
        {

            var newT = new DBObjectStream();
            newT.Deserialize(Serialize(null, null, false), null, null, this);

            return newT;

        }

        #endregion

        #endregion


        #region DBObject related methods

        #region AddAttribute(myAttributeName, myAttributeValue)

        public Exceptional<ResultType> AddAttribute(AttributeUUID myAttributeUUID, IObject myAttributeValue)
        {

            #region Input validation

            if (myAttributeUUID == null)
                return new Exceptional<ResultType>(new Error_ArgumentNullOrEmpty("myAttributeUUID"));

            if (myAttributeValue == null)
                return new Exceptional<ResultType>(new Error_ArgumentNullOrEmpty("myAttributeValue"));

            #endregion

            if (base.ContainsKey(myAttributeUUID) == Trinary.TRUE)
            {
                return new Exceptional<ResultType>(new Error_AttributeAlreadyExists(myAttributeUUID.ToString()));
            }

            else
            {
                base.Add(myAttributeUUID, myAttributeValue);
                return new Exceptional<ResultType>(ResultType.Successful);
            }

        }

        #endregion

        #region GetAttribute(myAttributeName)

        public Exceptional<IObject> GetAttribute(TypeAttribute myAttribute, GraphDBType myType, DBContext dbContext)
        {
            IObject RetVal = null;

            if (myAttribute is UndefinedTypeAttribute)
            {
                return GetUndefinedAttributeValue(myAttribute.Name, dbContext.DBObjectManager);
            }

            if (base.ContainsKey(myAttribute.UUID) == Trinary.TRUE)
                return new Exceptional<IObject>(base[myAttribute.UUID]);

            if (myAttribute is ASpecialTypeAttribute)
            {
                var extrVal = (myAttribute as ASpecialTypeAttribute).ExtractValue(this, myType, dbContext);
                return extrVal;
            }

            return new Exceptional<IObject>(RetVal);
        }

        /// <summary>
        /// Returns the value of the <paramref name="myAttributeName"/> Attribute
        /// 
        /// </summary>
        /// <param name="myAttributeName">The AttributeUUID</param>
        /// <param name="myType">The Type</param>
        /// <param name="mySession">CurrentSession</param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <returns>The attribute value</returns>        
        public IObject GetAttribute(AttributeUUID myAttributeUUID, GraphDBType myType, DBContext dbContext)
        {
            IObject RetVal = null;

            if (base.ContainsKey(myAttributeUUID) == Trinary.TRUE)
                return base[myAttributeUUID];

            var typeAttribute = myType.GetTypeAttributeByUUID(myAttributeUUID);

            if (typeAttribute is ASpecialTypeAttribute)
            {
                var extrVal = (typeAttribute as ASpecialTypeAttribute).ExtractValue(this, myType, dbContext);
                if (extrVal.Failed())
                {
                    throw new GraphDBException(extrVal.IErrors);
                }
                return extrVal.Value;
            }

            return RetVal;
        }

        public IObject GetAttribute(AttributeUUID myAttributeName)
        {
            if (base.ContainsKey(myAttributeName) == Trinary.TRUE)
                return base[myAttributeName];

            return null;
        }

        #endregion

        #region GetAttributes()

        /// <summary>
        /// Returns the value of the <paramref name="myAttributeName"/> Attribute
        /// 
        /// </summary>
        /// <param name="myAttributeName">The AttributeUUID</param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <returns>The attribute value</returns>
        public IDictionary<AttributeUUID, IObject> GetAttributes()
        {
            return (IDictionary<AttributeUUID, IObject>)this.GetIDictionary();
        }

        #endregion

        #region get undefined attributes

        /// <summary>
        /// return all undefined attributes of the dbobject
        /// </summary>
        /// <param name="myTypeManager">the typemanger</param>
        /// <returns></returns>
        public Exceptional<IDictionary<String, IObject>> GetUndefinedAttributePayload(DBObjectManager objectManager)
        {
            var retVal = LoadUndefAttributes(this.ObjectLocation, objectManager);

            if (retVal.Success())
            {
                return new Exceptional<IDictionary<String, IObject>>(retVal.Value.GetAllAttributes());
            }
            
            return new Exceptional<IDictionary<String, IObject>>(retVal);
        }

        /// <summary>
        /// return true if the type has an undefined attribute with this name
        /// </summary>
        /// <param name="myName">the attribute name</param>
        /// <param name="myTypeManager">the typemanager</param>
        /// <returns></returns>
        public Boolean ContainsUndefinedAttribute(String myName, DBObjectManager objectManager)
        {
            var retVal = LoadUndefAttributes(this.ObjectLocation, objectManager);

            if (retVal.Success())
            {
                return retVal.Value.ContainsAttribute(myName);
            }
            else
            {
                //Hack: should return an exceptional
                return false;
            }
        }

        /// <summary>
        /// return the value for the undefined attribute
        /// </summary>
        /// <param name="myName">the attribute name</param>
        /// <param name="myTypeManager">the typemanager</param>
        /// <returns>the value for the attribute</returns>
        public Exceptional<IObject> GetUndefinedAttributeValue(String myName, DBObjectManager objectManager)
        {
            var retVal = LoadUndefAttributes(this.ObjectLocation, objectManager);

            if (retVal.Success())
            {
                if (!retVal.Value.ContainsAttribute(myName))
                {
                    return new Exceptional<IObject>(new Error_UndefinedAttributeNotFound(myName));
                }
                else
                {
                    return new Exceptional<IObject>(retVal.Value[myName]);
                }
            }

            return new Exceptional<IObject>(retVal);
        }

        /// <summary>
        /// if the stream for undefined attributes is null then the undefined attributes are load
        /// </summary>
        /// <param name="myLocation">object location for this type</param>
        /// <param name="myTypeManager">the typemanager</param>
        /// <returns></returns>
        private Exceptional<UndefinedAttributesStream> LoadUndefAttributes(ObjectLocation myLocation, DBObjectManager objectManager)
        {

            return objectManager.LoadUndefinedAttributes(myLocation);
        }

        public Exceptional<Boolean> AddUndefinedAttribute(String myName, IObject myValue, DBObjectManager myDBObjectManager)
        {

            var retVal = LoadUndefAttributes(ObjectLocation, myDBObjectManager);

            if (retVal.Success())
            {
                retVal.Value.AddAttribute(myName, myValue);
            }
            else
            {
                return new Exceptional<Boolean>(retVal);
            }

            var storeExcepts = myDBObjectManager.StoreUndefinedAttributes(retVal.Value);

            if (storeExcepts.Failed())
                return new Exceptional<bool>(storeExcepts);

            return new Exceptional<bool>(true);

        }

        public Exceptional<Boolean> RemoveUndefinedAttribute(string myName, DBObjectManager objectManager)
        {
            var retVal = LoadUndefAttributes(this.ObjectLocation, objectManager);

            if (retVal.Success())
            {
                retVal.Value.RemoveAttribute(myName);
            }
            else
            {
                return new Exceptional<bool>(retVal);
            }

            var storeExcepts = objectManager.StoreUndefinedAttributes(retVal.Value);

            if (storeExcepts.Failed())
                return new Exceptional<bool>(storeExcepts);

            return new Exceptional<bool>(true);
        }
    

        #endregion

        #region HasAttribute(myTypeAttribute)

        public Boolean HasAttribute(TypeAttribute myTypeAttribute, DBContext myDBContext)
        {

            if (myTypeAttribute is UndefinedTypeAttribute)
            {
                return ContainsUndefinedAttribute(myTypeAttribute.Name, myDBContext.DBObjectManager);
            }

            if (base.ContainsKey(myTypeAttribute.UUID) == Trinary.TRUE)
            {
                return true;
            }

            if (myTypeAttribute is ASpecialTypeAttribute)
            {
                return true;
            }

            return false;
        }


        #endregion

        #region HasAttribute(myAttributeName)

        public Boolean HasAttribute(AttributeUUID myAttributeUUID, GraphDBType myType)
        {
            if (base.ContainsKey(myAttributeUUID) == Trinary.TRUE)
                return true;

            var typeAttr = myType.GetTypeAttributeByUUID(myAttributeUUID);
            if (typeAttr is ASpecialTypeAttribute)
                return true;

            return false;
        }


        #endregion

        #region HasAttribute(myAttributeChain)

        public Boolean HasAtLeastOneAttribute(IEnumerable<AttributeUUID> myInterestingUUIDs, GraphDBType myType, SessionSettings mySessionToken)
        {
            foreach (var aInterestingUUID in myInterestingUUIDs)
            {
                if (HasAttribute(aInterestingUUID, myType))
                {
                    return true;
                }
            }

            return false;
        }

        public Boolean HasAttribute(List<AttributeUUID> myAttributes)
        {
            foreach (var aAttributeUUD in myAttributes)
            {
                if (!base.ContainsKey(aAttributeUUD) == Trinary.TRUE)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region RemoveAttribute(myAttributeName)

        public Boolean RemoveAttribute(AttributeUUID myAttributeName)
        {
            return base.Remove(myAttributeName);
        }

        #endregion

        #region AlterAttribute(myAttributeName, myAttributeValue)

        public Exceptional<Boolean> AlterAttribute(AttributeUUID myAttributeName, IObject myAttributeValue)
        {

            #region input exceptions

            if (myAttributeName == null)
            {
                return new Exceptional<bool>(new Error_ArgumentNullOrEmpty("Attribute name should not be null."));
            }

            if (myAttributeValue == null)
            {
                return new Exceptional<bool>(new Error_ArgumentNullOrEmpty("Attribute value should not be null."));
            }

            #endregion

            IObject value = null;

            base.TryGetValue(myAttributeName, out value);

            if (value != null)
            {
                base[myAttributeName] = myAttributeValue;

                #region estimated size

                _estimatedSize -= value.GetEstimatedSize();
                _estimatedSize += myAttributeValue.GetEstimatedSize();

                #endregion

                return new Exceptional<Boolean>(true);
            }

            else
                return new Exceptional<Boolean>(false);

        }

        #endregion

        public UInt64 AttributesCount
        {
            get
            {
                return base.KeyCount();
            }
        }

        #endregion


        #region ToString()

        public override String ToString()
        {
            StringBuilder retVal = new StringBuilder("Attributes[" + base.GetIDictionary().Count + "]");
            foreach (KeyValuePair<AttributeUUID, IObject> attr in base.GetIDictionary())
            {
                retVal.AppendLine("\t  #" + attr.Key + " = " + attr.Value);
            }
            return retVal.ToString();
        }

        #endregion

        #region (de)serialize overrides
        
        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {
            base.Serialize(ref mySerializationWriter);
            TypeUUID.Serialize(ref mySerializationWriter);
        }

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {
            base.Deserialize(ref mySerializationReader);
            TypeUUID = new TypeUUID();
            TypeUUID.Deserialize(ref mySerializationReader);

            _estimatedSize += EstimatedSizeConstants.CalcUUIDSize(TypeUUID);
        }
        
        #endregion        

        #region BackwardEdges

        public Exceptional<Boolean> ContainsBackwardEdge(BackwardEdgeStream myBackwardEdgeStream, EdgeKey myEdgeKey)
        {
            return new Exceptional<bool>(myBackwardEdgeStream.ContainsBackwardEdge(myEdgeKey));
        }

        public Exceptional<Boolean> ContainsBackwardEdge(EdgeKey myEdgeKey, DBContext dbContext, DBObjectCache myObjectCache, GraphDBType myTypeOfDBObject)
        {
            var loadExcept = myObjectCache.LoadDBBackwardEdgeStream(dbContext.DBTypeManager.GetTypeAttributeByEdge(myEdgeKey).GetDBType(dbContext.DBTypeManager), this.ObjectUUID);

            if (loadExcept.Failed())
                return new Exceptional<Boolean>(loadExcept);

            return new Exceptional<bool>(loadExcept.Value.ContainsBackwardEdge(myEdgeKey));
        }

        public Exceptional<ASetOfReferencesEdgeType> GetBackwardEdges(EdgeKey myEdgeKey, DBContext dbContext, DBObjectCache myObjectCache, GraphDBType myTypeOfDBObject)
        {
            var loadExcept = myObjectCache.LoadDBBackwardEdgeStream(dbContext.DBTypeManager.GetTypeAttributeByEdge(myEdgeKey).GetDBType(dbContext.DBTypeManager), this.ObjectUUID);
            if (loadExcept.Failed())
            {
                return new Exceptional<ASetOfReferencesEdgeType>(loadExcept);
            }

            var contBackwardExcept = ContainsBackwardEdge(loadExcept.Value, myEdgeKey);

            if (contBackwardExcept.Failed())
                return new Exceptional<ASetOfReferencesEdgeType>(contBackwardExcept);
            
            if (!contBackwardExcept.Value)
                return new Exceptional<ASetOfReferencesEdgeType>(null as ASetOfReferencesEdgeType);

            return new Exceptional<ASetOfReferencesEdgeType>(loadExcept.Value.GetBackwardEdges(myEdgeKey));
        }
        
        /// <summary>
        /// Adds a BackwardEdge to the DBObject and flush them
        /// Use the TypeManager method instead of this!
        /// </summary>
        /// <param name="uUIDofType"></param>
        /// <param name="uUIDofAttribute"></param>
        /// <param name="reference"></param>
        /// <param name="myTypeManager"></param>
        /// <param name="myGraphFS2Session"></param>
        public Exceptional<Boolean> AddBackwardEdge(TypeUUID uUIDofType, AttributeUUID uUIDofAttribute, ObjectUUID reference, DBObjectManager objectManager)
        {

            var loadExcept = objectManager.LoadBackwardEdge(this.ObjectLocation);

            if (loadExcept.Failed())
                return new Exceptional<Boolean>(loadExcept);

            EdgeKey tempKey = new EdgeKey(uUIDofType, uUIDofAttribute);

            loadExcept.Value.AddBackwardEdge(tempKey, reference, objectManager);

            var storeExcept = StoreBackwardEdges(_IGraphFSSessionReference.Value, loadExcept.Value);

            if (storeExcept.Failed())
                return new Exceptional<bool>(storeExcept);

            return new Exceptional<bool>(true);

        }

        /// <summary>
        /// Remove a backwardEdge from the DBObject and flush them.
        /// Use the TypeManager method instead of this!
        /// </summary>
        /// <param name="myTypeUUID"></param>
        /// <param name="myAttributeUUID"></param>
        /// <param name="myObjectUUID"></param>
        /// <param name="myTypeManager"></param>
        /// <param name="myGraphFS2Session"></param>
        public Exceptional<Boolean> RemoveBackwardEdge(TypeUUID myTypeUUID, AttributeUUID myAttributeUUID, ObjectUUID myObjectUUID, DBObjectManager myDBObjectManager)
        {
            var loadExcept = myDBObjectManager.LoadBackwardEdge(this.ObjectLocation);

            if (loadExcept.Failed())
                return new Exceptional<Boolean>(loadExcept);

            var BackwardEdges = loadExcept.Value;

            var tempKey = new EdgeKey(myTypeUUID, myAttributeUUID);

            if (!BackwardEdges.RemoveBackwardEdge(tempKey, myObjectUUID))
                return new Exceptional<Boolean>(new Error_BackwardEdgeDestinationIsInvalid(myTypeUUID.ToString(), myAttributeUUID.ToString()));

            var storeExcept = StoreBackwardEdges(_IGraphFSSessionReference.Value, BackwardEdges);

            if (storeExcept.Failed())
                return new Exceptional<bool>(storeExcept);

            return new Exceptional<bool>(true);

        }

        private Exceptional<Boolean> StoreBackwardEdges(IGraphFSSession myIGraphFS2Session, BackwardEdgeStream myBackwardEdge)
        {

            var storeExcept = myIGraphFS2Session.StoreFSObject(myBackwardEdge, true);

            if (storeExcept.Failed())
                return new Exceptional<Boolean>(storeExcept);

            return new Exceptional<bool>(true);

        }

        #endregion

        #region Undefined Attributes

        public Exceptional<UndefinedAttributesStream> GetUndefinedAttributeStream(DBObjectManager dBObjectManager)
        {
            return LoadUndefAttributes(this.ObjectLocation, dBObjectManager);
        }

        #endregion

        #region IEstimable Members

        public override ulong GetEstimatedSize()
        {
            return _estimatedSize;
        }

        public override ulong GetEstimatedSizeOfKey(AttributeUUID myTKey)
        {
            return myTKey.GetEstimatedSize();
        }

        public override ulong GetEstimatedSizeOfValue(IObject myTValue)
        {
            return myTValue.GetEstimatedSize();
        }

        #endregion
    }

}   
       
