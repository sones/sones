/* <id name="GraphDB – AAttributeIndex" />
 * <copyright file="AAttributeIndex.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>A abstract class for AttributeIndex and UUIDIdx</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.Indices;
using sones.Lib.ErrorHandling;
using sones.GraphFS.Objects;
using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Structures.EdgeTypes;


#endregion

namespace sones.GraphDB.Indices
{
    /// <summary>
    /// A abstract class for AttributeIndex and UUIDIdx
    /// </summary>
    public abstract class AAttributeIndex : IFastSerializationTypeSurrogate, IFastSerialize, IDisposable// : IAttributeIndex
    {

        #region Properties

        #region IndexName

        /// <summary>
        /// The name of the index
        /// </summary>
        public virtual String IndexName { get; protected set; }

        #endregion

        #region IndexKeyDefinition

        /// <summary>
        /// Definition of the AttributeIndex.
        /// </summary>
        public virtual IndexKeyDefinition IndexKeyDefinition { get; set; }

        #endregion

        #region IndexEdition

        /// <summary>
        /// You may have different versions of an attribute index, e.g. HashMap,
        /// BTree to speed up different database operations
        /// </summary>
        public virtual String IndexEdition { get; protected set; }

        #endregion

        #region IsListOfBaseObjectsIndex
        
        /// <summary>
        /// This is a special handling for indices on list or set of baseobjects cause we can't use the standard indexOperation on this type of index
        /// </summary>
        public virtual Boolean IsListOfBaseObjectsIndex { get; protected set; }
        
        #endregion

        #region IsUUIDIndex

        /// <summary>
        /// This is the UUID index which contains ALL DBOs 
        /// </summary>
        public abstract Boolean IsUUIDIndex { get; }

        #endregion

        #region IsUniqueIndex

        /// <summary>
        /// Determines whether this index is an unique index
        /// </summary>
        public virtual Boolean IsUniqueIndex { get; set; }

        #endregion

        #region IsUniqueAttributeIndex

        /// <summary>
        /// Determines whether this index is an special index for unique attributes definition
        /// </summary>
        public virtual Boolean IsUniqueAttributeIndex
        {
            get
            {
                return IndexEdition == DBConstants.UNIQUEATTRIBUTESINDEX;
            }
        }

        #endregion

        #region IndexObjectType

        /// <summary>
        /// The IndexType e.g. HashMap, BTree of this AttributeIndex
        /// </summary>
        public abstract String IndexType { get; }
        
        #endregion

        #region FileSystemLocation

        /// <summary>
        /// The ObjectLocation of the IndexObject within a Graph file system
        /// </summary>
        public virtual ObjectLocation FileSystemLocation { get; protected set; }
        
        #endregion

        #region IndexRelatedTypeUUID

        /// <summary>
        /// The name of the index
        /// </summary>
        public virtual TypeUUID IndexRelatedTypeUUID { get; protected set; }

        #endregion

        #endregion

        #region IAttributeIndex methods

        /// <summary>
        /// This method updates the idx corresponding to an DBObject
        /// </summary>
        /// <param name="myDBObject">The DBObject that should be updated</param>
        /// <param name="myTypeOfDBObject">The type of the DBObject</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns>an exceptional</returns>
        public abstract Exceptional Update(DBObjectStream myDBObject, GraphDBType myTypeOfDBObject, DBContext myDBContext);

        /// <summary>
        /// This method inserts the given DBObject into the index
        /// </summary>
        /// <param name="myDBObject">The DBObject that should be inserted</param>
        /// <param name="myTypeOfDBobject">The type of the DBObject</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns>an exceptional</returns>
        public abstract Exceptional Insert(DBObjectStream myDBObject, GraphDBType myTypeOfDBobject, DBContext myDBContext);

        /// <summary>
        /// This method inserts the given DBObject into the index
        /// </summary>
        /// <param name="myDBObject">The DBObject that should be inserted</param>
        /// <param name="myIndexSetStrategy">The index merge strategy</param>
        /// <param name="myTypeOfDBObject">The type of the DBObject</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns>an exceptional</returns>
        public abstract Exceptional Insert(DBObjectStream myDBObject, IndexSetStrategy myIndexSetStrategy, GraphDBType myTypeOfDBObject, DBContext myDBContext);

        /// <summary>
        /// This method checks if the current attribute index contains a DBObject
        /// </summary>
        /// <param name="myDBObject">The DBObject that should be checked</param>
        /// <param name="myTypeOfDBObject">The Type of the DBObject</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns>boolean</returns>
        public abstract bool Contains(DBObjectStream myDBObject, GraphDBType myTypeOfDBObject, DBContext myDBContext);

        /// <summary>
        /// This method checks if the current attribute index contains a DBObject
        /// </summary>
        /// <param name="myIndexKey">The DBObject that should be checked</param>
        /// <param name="myTypeOfDBObject">The Type of the DBObject</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns>boolean</returns>
        public abstract bool Contains(IndexKey myIndexKey, GraphDBType myTypeOfDBObject, DBContext myDBContext);

        /// <summary>
        /// This method removes a given DBObject from the index
        /// </summary>
        /// <param name="myDBObject">The DBObject that should be removed</param>
        /// <param name="myTypeOfDBObjects">The type of the DBObject</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns>An exceptional</returns>
        public abstract Exceptional Remove(DBObjectStream myDBObject, GraphDBType myTypeOfDBObjects, DBContext myDBContext);

        /// <summary>
        /// Clear and clean up all indices
        /// </summary>
        /// <param name="myIndexManager">The DBIndexManager</param>
        /// <returns>An exceptional</returns>
        public abstract Exceptional ClearAndRemoveFromDisc(DBContext myDBContext);

        /// <summary>
        /// Get the index keys for a type
        /// </summary>
        /// <param name="myTypeOfDBObject">The db type</param>
        /// <paramr name="myDBContext">The db context</param>
        /// <returns>An enumerable of index keys</returns>
        public abstract IEnumerable<IndexKey> GetKeys(GraphDBType myTypeOfDBObject, DBContext myDBContext);

        /// <summary>
        /// Return all object uuid's of an db type
        /// </summary>
        /// <param name="myTypeOfDBObject">The db type</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns>An enumerable of an enumerable that contain all object uuid's</returns>
        public abstract IEnumerable<IEnumerable<ObjectUUID>> GetAllValues(GraphDBType myTypeOfDBObject, DBContext myDBContext);

        /// <summary>
        /// Return all object uuid's of an index with the given index key
        /// </summary>
        /// <param name="myIndeyKey">The index key</param>
        /// <param name="myTypeOfDBObject">The db type</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns>An enumerable of object uuid's</returns>
        public abstract IEnumerable<ObjectUUID> GetValues(IndexKey myIndeyKey, GraphDBType myTypeOfDBObject, DBContext myDBContext);

        /// <summary>
        /// Return the keys and values of this index
        /// this is an key value pair with the index key and the corresponding object uuid's 
        /// </summary>
        /// <param name="myTypeOfDBObject">The db type</param>
        /// <param name="myDBContext">The db context</param>
        /// <returns>an enumerable of key value pairs</returns>
        public abstract IEnumerable<KeyValuePair<IndexKey, HashSet<ObjectUUID>>> GetKeyValues(GraphDBType myTypeOfDBObject, DBContext myDBContext);

        /// <summary>
        /// Return the number of objects for this index
        /// </summary>
        /// <returns>The number of values</returns>
        public abstract UInt64 GetValueCount(DBContext myDBContext, GraphDBType myTypeOfDBObject);

        /// <summary>
        /// Return the number of keys for this index
        /// </summary>
        /// <returns>The number of keys</returns>
        public abstract UInt64 GetKeyCount(DBContext myDBContext, GraphDBType myTypeOfDBObject);

        /// <summary>
        /// This is an in range operator
        /// returns all objects that are in range from an index key to an index key
        /// </summary>
        /// <param name="fromIndexKey">Start of the region</param>
        /// <param name="toIndexKey">End of region</param>
        /// <param name="myOrEqualFromKey">Currently not used</param>
        /// <param name="myOrEqualToKey">Currently not used</param>
        /// <param name="myTypeOfDBObject">The type of the db object</param>
        /// <param name="myDBContext"/>The db context
        /// <returns></returns>
        public abstract IEnumerable<ObjectUUID> InRange(IndexKey fromIndexKey, IndexKey toIndexKey, bool myOrEqualFromKey, bool myOrEqualToKey, GraphDBType myTypeOfDBObject, DBContext myDBContext);

        public abstract AAttributeIndex GetNewInstance();

        public abstract Exceptional Initialize(DBContext myDBContext, string indexName, IndexKeyDefinition idxKey, GraphDBType correspondingType, string indexEdition = DBConstants.DEFAULTINDEX);

        #endregion

        #region IFastSerializationTypeSurrogate Members

        public abstract bool SupportsType(Type type);

        public void Serialize(Lib.NewFastSerializer.SerializationWriter writer, object value)
        {
            (value as AAttributeIndex).Serialize(ref writer);
        }

        public object Deserialize(Lib.NewFastSerializer.SerializationReader reader, Type type)
        {
            AAttributeIndex thisObject = (AAttributeIndex)Activator.CreateInstance(type);
            thisObject.Deserialize(ref reader);
            return thisObject;
        }

        public abstract uint TypeCode
        {
            get;
        }

        #endregion

        #region IFastSerialize Members

        public bool isDirty
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime ModificationTime
        {
            get { throw new NotImplementedException(); }
        }

        public abstract void Serialize(ref Lib.NewFastSerializer.SerializationWriter mySerializationWriter);

        public abstract void Deserialize(ref Lib.NewFastSerializer.SerializationReader mySerializationReader);

        #endregion

        //public abstract Exceptional Initialize(DBContext myDBContext);

        public abstract Exceptional Clear(DBContext myDBContext, GraphDBType myTypeOfDBObject);

        #region IDisposable Members

        public virtual void Dispose() { }

        #endregion

        #region GetIndexkeysFromDBObject

        /// <summary>
        /// Creates IndexKeys from a DBObject.
        /// </summary>
        /// <param name="myDBObject">The DBObject reference for the resulting IndexKeys</param>
        /// <param name="myTypeOfDBObject">The Type of the DBObject</param>
        /// <param name="myToken">The SessionInfos</param>
        /// <returns>A HashSet of IndexKeys</returns>
        protected HashSet<IndexKey> GetIndexkeysFromDBObject(DBObjectStream myDBObject, GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            HashSet<IndexKey> result = new HashSet<IndexKey>();
            TypeAttribute currentAttribute;

            foreach (var aIndexAttributeUUID in IndexKeyDefinition.IndexKeyAttributeUUIDs)
            {
                currentAttribute = myTypeOfDBObject.GetTypeAttributeByUUID(aIndexAttributeUUID);

                if (!currentAttribute.GetDBType(dbContext.DBTypeManager).IsUserDefined)
                {
                    #region base attribute

                    if (myDBObject.HasAttribute(aIndexAttributeUUID, myTypeOfDBObject))
                    {
                        ADBBaseObject newIndexKeyItem = null;

                        switch (currentAttribute.KindOfType)
                        {
                            #region List/Set

                            case KindsOfType.ListOfNoneReferences:
                            case KindsOfType.SetOfNoneReferences:

                                var helperSet = new List<ADBBaseObject>();

                                foreach (var aBaseObject in ((IBaseEdge)myDBObject.GetAttribute(aIndexAttributeUUID, myTypeOfDBObject, dbContext)).GetBaseObjects())
                                {
                                    helperSet.Add((ADBBaseObject)aBaseObject);
                                }

                                if (result.Count != 0)
                                {
                                    #region update

                                    HashSet<IndexKey> helperResultSet = new HashSet<IndexKey>();

                                    foreach (var aNewItem in helperSet)
                                    {
                                        foreach (var aReturnVal in result)
                                        {
                                            helperResultSet.Add(new IndexKey(aReturnVal, aIndexAttributeUUID, aNewItem, this.IndexKeyDefinition));
                                        }
                                    }

                                    result = helperResultSet;

                                    #endregion
                                }
                                else
                                {
                                    #region create new

                                    foreach (var aNewItem in helperSet)
                                    {
                                        result.Add(new IndexKey(aIndexAttributeUUID, aNewItem, this.IndexKeyDefinition));
                                    }

                                    #endregion
                                }

                                break;

                            #endregion

                            #region single/special

                            case KindsOfType.SingleReference:
                            case KindsOfType.SingleNoneReference:
                            case KindsOfType.SpecialAttribute:

                                newIndexKeyItem = (ADBBaseObject)myDBObject.GetAttribute(aIndexAttributeUUID, myTypeOfDBObject, dbContext);

                                if (result.Count != 0)
                                {
                                    #region update

                                    foreach (var aResultItem in result)
                                    {
                                        aResultItem.AddAADBBAseObject(aIndexAttributeUUID, newIndexKeyItem);
                                    }

                                    #endregion
                                }
                                else
                                {
                                    #region create new

                                    result.Add(new IndexKey(aIndexAttributeUUID, newIndexKeyItem, this.IndexKeyDefinition));

                                    #endregion
                                }

                                break;

                            #endregion

                            #region not implemented

                            case KindsOfType.SetOfReferences:
                            default:

                                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "Currently its not implemented to insert anything else than a List/Set/Single of base types"));

                            #endregion
                        }
                    }
                    else
                    {
                        //add default value

                        var defaultADBBAseObject = GraphDBTypeMapper.GetADBBaseObjectFromUUID(currentAttribute.DBTypeUUID);
                        defaultADBBAseObject.SetValue(DBObjectInitializeType.Default);

                        if (result.Count != 0)
                        {
                            #region update

                            foreach (var aResultItem in result)
                            {
                                aResultItem.AddAADBBAseObject(aIndexAttributeUUID, defaultADBBAseObject);
                            }

                            #endregion
                        }
                        else
                        {
                            #region create new

                            result.Add(new IndexKey(aIndexAttributeUUID, defaultADBBAseObject, this.IndexKeyDefinition));

                            #endregion
                        }

                    }
                    #endregion
                }
                else
                {
                    #region reference attribute

                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

                    #endregion
                }
            }

            return result;
        }

        #endregion

    }
}
