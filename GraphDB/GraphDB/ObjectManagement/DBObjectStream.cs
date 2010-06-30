/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


/* <id Name=”GraphDB – database object” />
 * <copyright file=”DBObjectStream.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>The DBObject carries the myAttributes of a database object.<summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.GraphFS.Session;
using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.DataStructures;
using sones.Lib.ErrorHandling;
using sones.Lib.NewFastSerializer;
using sones.Lib.Session;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones.GraphDB.ObjectManagement
{


    /// <summary>
    /// The DBObject carries the myAttributes of a database object.
    /// </summary>
    public class DBObjectStream : ADictionaryObject<AttributeUUID, AObject>
    {

        public static ObjectLocation GetObjectLocation(GraphDBType myGraphDBType, ObjectUUID myObjectUUID)
        {
            return new ObjectLocation(myGraphDBType.ObjectLocation, DBConstants.DBObjectsLocation, myObjectUUID.ToString());
        }

        #region Properties

        #region type uuid

        public TypeUUID TypeUUID { get; set; }

        #endregion

        #region back edge

        public BackwardEdgeStream BackwardEdges { get; set; }

        #endregion

        #region undefined attributes

        public UndefinedAttributesStream UndefAttributes { get; set; }

        #endregion

        #endregion


        #region Constructor

        #region DBObject()

        /// <summary>
        /// This will create an empty DBObject
        /// </summary>
        public DBObjectStream()
        {

            // Members of APandoraStructure
            _StructureVersion = 1;

            // Members of APandoraObject
            _ObjectStream = "DBOBJECTSTREAM";

            // Object specific data...

            // Set ObjectUUID
            if (ObjectUUID.Length == 0)
                ObjectUUID = base.ObjectUUID;

        }

        #endregion

        #region DBObject(myObjectLocation)

        /// <summary>
        /// This will create an em
        /// </summary>
        /// <param name="myObjectLocation">the location (object myPath and Name) of the requested DBObject within the file system</param>
        private DBObjectStream(GraphDBType myDBTypeStream)
            : this()
        {

            ObjectLocation = DBObjectStream.GetObjectLocation(myDBTypeStream, ObjectUUID);

            if (ObjectLocation == null || ObjectLocation.Length < FSPathConstants.PathDelimiter.Length)
                throw new ArgumentNullException("Invalid ObjectLocation!");

        }

        #endregion

        #region DBObject(myDBTypeStream, myDBObjectAttributes)

        /// <summary>
        /// This will create an DBObject
        /// </summary>
        /// <param name="myObjectLocation">the location (object myPath and Name) of the requested DBObject within the file system</param>
        public DBObjectStream(GraphDBType myDBTypeStream, Dictionary<AttributeUUID, AObject> myDBObjectAttributes)
            : this(myDBTypeStream)
        {
            if (myDBObjectAttributes == null)
                throw new ArgumentNullException("The parameter myDBObjectAttributes must not be null or empty!");

            base.Set(myDBObjectAttributes, IndexSetStrategy.REPLACE);

            TypeUUID = myDBTypeStream.UUID;

            #warning: "Add the GUID AttributeUUID"
            //base.Add(DBReference.UUID, ObjectUUID);

        }

        #endregion

        #region DBObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

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


        #region Members of APandoraObject

        #region Clone()

        public override AFSObject Clone()
        {

            var newT = new DBObjectStream();
            newT.Deserialize(Serialize(null, null, false), null, null, this);
            newT.BackwardEdges = this.BackwardEdges;
            return newT;

        }

        #endregion

        #endregion


        #region DBObject related methods

        #region AddAttribute(myAttributeName, myAttributeValue)

        public Exceptional<ResultType> AddAttribute(AttributeUUID myAttributeUUID, AObject myAttributeValue)
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

        public AObject GetAttribute(TypeAttribute myAttribute, GraphDBType myType, DBContext dbContext)
        {
            AObject RetVal = null;

            if (base.ContainsKey(myAttribute.UUID) == Trinary.TRUE)
                return base[myAttribute.UUID];

            if (myAttribute is ASpecialTypeAttribute)
            {
                var extrVal = (myAttribute as ASpecialTypeAttribute).ExtractValue(this, myType, dbContext);
                if (extrVal.Failed)
                {
                    throw new GraphDBException(extrVal.Errors);
                }
                return extrVal.Value;
            }

            return RetVal;
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
        public AObject GetAttribute(AttributeUUID myAttributeUUID, GraphDBType myType, DBContext dbContext)
        {
            AObject RetVal = null;

            if (base.ContainsKey(myAttributeUUID) == Trinary.TRUE)
                return base[myAttributeUUID];

            var typeAttribute = myType.GetTypeAttributeByUUID(myAttributeUUID);

            if (typeAttribute is ASpecialTypeAttribute)
            {
                var extrVal = (typeAttribute as ASpecialTypeAttribute).ExtractValue(this, myType, dbContext);
                if (extrVal.Failed)
                {
                    throw new GraphDBException(extrVal.Errors);
                }
                return extrVal.Value;
            }

            return RetVal;
        }

        public AObject GetAttribute(AttributeUUID myAttributeName)
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
        public IDictionary<AttributeUUID, AObject> GetAttributes()
        {
            return (IDictionary<AttributeUUID, AObject>)this.GetIDictionary();
        }

        #endregion

        #region get undefined attributes

        /// <summary>
        /// return all undefined attributes of the dbobject
        /// </summary>
        /// <param name="myTypeManager">the typemanger</param>
        /// <returns></returns>
        public Exceptional<IDictionary<String, AObject>> GetUndefinedAttributes(DBObjectManager objectManager)
        {
            var retVal = LoadUndefAttributes(this.ObjectLocation, objectManager);
            
            if (retVal.Success)
                return new Exceptional<IDictionary<String, AObject>>(UndefAttributes.GetAllAttributes());

            return new Exceptional<IDictionary<String, AObject>>(retVal);
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
            
            if (retVal.Success)
                return UndefAttributes.ContainsAttribute(myName);

            return new Exceptional<Boolean>(retVal).Value;
        }

        /// <summary>
        /// return the value for the undefined attribute
        /// </summary>
        /// <param name="myName">the attribute name</param>
        /// <param name="myTypeManager">the typemanager</param>
        /// <returns>the value for the attribute</returns>
        public Exceptional<AObject> GetUndefinedAttributeValue(String myName, DBObjectManager objectManager)
        {
            var retVal = LoadUndefAttributes(this.ObjectLocation, objectManager);
            
            if (retVal.Success)
                return new Exceptional<AObject>(UndefAttributes.GetAttributeValue(myName));

            return new Exceptional<AObject>(retVal);
        }

        /// <summary>
        /// if the stream for undefined attributes is null then the undefined attributes are load
        /// </summary>
        /// <param name="myLocation">object location for this type</param>
        /// <param name="myTypeManager">the typemanager</param>
        /// <returns></returns>
        private Exceptional<UndefinedAttributesStream> LoadUndefAttributes(ObjectLocation myLocation, DBObjectManager objectManager)
        {
            Exceptional<UndefinedAttributesStream> retVal;

            if (UndefAttributes == null)
            {
                retVal = objectManager.LoadUndefinedAttributes(myLocation);

                if (retVal.Success)
                    UndefAttributes = retVal.Value;
                else
                    return new Exceptional<UndefinedAttributesStream>(retVal);

                return retVal;
            }
            else
                return new Exceptional<UndefinedAttributesStream>(UndefAttributes);
        }

        public Exceptional<Boolean> AddUndefinedAttribute(string myName, AObject myValue, DBObjectManager objectManager)
        {
            var retVal = LoadUndefAttributes(this.ObjectLocation, objectManager);

            if (retVal.Success)
                UndefAttributes.AddAttribute(myName, myValue);

            var storeExcepts = objectManager.StoreUndefinedAttributes(UndefAttributes);

            if (storeExcepts.Failed)
                return new Exceptional<bool>(storeExcepts);

            return new Exceptional<bool>(true);
        }

        public Exceptional<Boolean> RemoveUndefinedAttribute(string myName, DBObjectManager objectManager)
        {
            var retVal = LoadUndefAttributes(this.ObjectLocation, objectManager);

            if (retVal.Success)
                UndefAttributes.RemoveAttribute(myName);

            var storeExcepts = objectManager.StoreUndefinedAttributes(UndefAttributes);

            if (storeExcepts.Failed)
                return new Exceptional<bool>(storeExcepts);

            return new Exceptional<bool>(true);
        }
    

        #endregion

        #region HasAttribute(myAttributeName)

        public Boolean HasAttribute(AttributeUUID myAttributeUUID, GraphDBType myType, SessionSettings mySessionSettings)
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
                if (HasAttribute(aInterestingUUID, myType, mySessionToken))
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

        public Exceptional<Boolean> AlterAttribute(AttributeUUID myAttributeName, AObject myAttributeValue)
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

            if (base.ContainsKey(myAttributeName) == Trinary.TRUE)
            {
                base[myAttributeName] = myAttributeValue;
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
            foreach (KeyValuePair<AttributeUUID, AObject> attr in base.GetIDictionary())
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
        }
        
        #endregion        

        #region BackwardEdges

        public Exceptional<Boolean> ContainsBackwardEdge(EdgeKey myEdgeKey, DBContext dbContext, DBObjectCache myObjectCache, GraphDBType myTypeOfDBObject)
        {
            if (BackwardEdges == null)
            {
                var loadExcept = myObjectCache.LoadDBBackwardEdgeStream(dbContext.DBTypeManager.GetTypeAttributeByEdge(myEdgeKey).GetDBType(dbContext.DBTypeManager), this.ObjectUUID);

                if (loadExcept.Failed)
                    return new Exceptional<Boolean>(loadExcept);

                BackwardEdges = loadExcept.Value;
            }

            return new Exceptional<bool>(BackwardEdges.ContainsBackwardEdge(myEdgeKey));
        }

        public Exceptional<ASetReferenceEdgeType> GetBackwardEdges(EdgeKey myEdgeKey, DBContext dbContext, DBObjectCache myObjectCache, GraphDBType myTypeOfDBObject)
        {
            var contBackwardExcept = ContainsBackwardEdge(myEdgeKey, dbContext, myObjectCache, myTypeOfDBObject);

            if (contBackwardExcept.Failed)
                return new Exceptional<ASetReferenceEdgeType>(contBackwardExcept);
            
            if (!contBackwardExcept.Value)
                return new Exceptional<ASetReferenceEdgeType>(null as ASetReferenceEdgeType);

            return new Exceptional<ASetReferenceEdgeType>(BackwardEdges.GetBackwardEdges(myEdgeKey));
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
            if (BackwardEdges == null)
            {
                var loadExcept = objectManager.LoadBackwardEdge(this.ObjectLocation);

                if (loadExcept.Failed)
                    return new Exceptional<Boolean>(loadExcept);

                BackwardEdges = loadExcept.Value;
            }

            EdgeKey tempKey = new EdgeKey(uUIDofType, uUIDofAttribute);

            BackwardEdges.AddBackwardEdge(tempKey, reference, objectManager);

            var storeExcept = StoreBackwardEdges(_IGraphFSSessionReference.Value);

            if (storeExcept.Failed)
                return new Exceptional<bool>(storeExcept);

            return new Exceptional<bool>(true);

        }

        /// <summary>
        /// Remove a backwardEdge from the DBObject and flush them.
        /// Use the TypeManager method instead of this!
        /// </summary>
        /// <param name="uUIDofType"></param>
        /// <param name="uUIDofAttribute"></param>
        /// <param name="reference"></param>
        /// <param name="myTypeManager"></param>
        /// <param name="myGraphFS2Session"></param>
        public Exceptional<Boolean> RemoveBackwardEdge(TypeUUID uUIDofType, AttributeUUID uUIDofAttribute, ObjectUUID reference, DBObjectManager objectManager)
        {

            if (BackwardEdges == null)
            {
                var loadExcept = objectManager.LoadBackwardEdge(this.ObjectLocation);

                if (loadExcept.Failed)
                    return new Exceptional<Boolean>(loadExcept);

                BackwardEdges = loadExcept.Value;
            }

            EdgeKey tempKey = new EdgeKey(uUIDofType, uUIDofAttribute);

            if (!BackwardEdges.RemoveBackwardEdge(tempKey, reference))
                return new Exceptional<Boolean>(new Error_BackwardEdgeDestinationIsInvalid(uUIDofType.ToHexString(), uUIDofAttribute.ToString()));

            var storeExcept = StoreBackwardEdges(_IGraphFSSessionReference.Value);

            if (storeExcept.Failed)
                return new Exceptional<bool>(storeExcept);

            return new Exceptional<bool>(true);

        }

        private Exceptional<Boolean> StoreBackwardEdges(IGraphFSSession myIGraphFS2Session)
        {

            if (BackwardEdges != null && BackwardEdges.isDirty)
            {
                var storeExcept = myIGraphFS2Session.StoreFSObject(BackwardEdges, true);

                if (storeExcept.Failed)
                    return new Exceptional<Boolean>(storeExcept);
            }

            return new Exceptional<bool>(true);

        }        

        #endregion
    }

}   
       
