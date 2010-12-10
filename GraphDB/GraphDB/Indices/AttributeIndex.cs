/* <id name="GraphDB – AttributeIndex" />
 * <copyright file="AttributeIndex.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Achim Friedland</developer>
 * <developer>Henning Rauch</developer>
 * <summary>This datastructure contains information concerning a single attribute index</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;

using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.Lib.DataStructures.Indices;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement.BasicTypes;

#endregion

namespace sones.GraphDB.Indices
{
    /// <summary>
    /// This datastructure contains information concerning a single attribute index
    /// </summary>
    public class AttributeIndex : AAttributeIndex
    {
        #region Properties

        Object _lockObject = new object();

        UInt64 _keyCount = 0;
        UInt64 _valueCount = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new AttributeIndex object
        /// </summary>
        /// <param name="myIndexName">The user-defined name of this index</param>
        /// <param name="myIndexEdition">You may have different versions of an attribute index, e.g. HashMap, BTree to speed up different database operations</param>
        /// <param name="myAttributes">The list of attributes that is needed for the creation of a IndexKeyDefinition</param>
        /// <param name="myIndexType">The IndexType e.g. HashMap, BTree of this AttributeIndex</param>
        /// <param name="correspondingType">The corresponding type of this index, used to get the file system location</param>
        /// <param name="myFileSystemLocation">The location oif the index. If null it will be generated based on the <paramref name="correspondingType"/>.</param>
        public AttributeIndex(String myIndexName, String myIndexEdition, List<AttributeUUID> myAttributes, GraphDBType correspondingType, UInt16 myObjectDirectoryShards, String myIndexType = null, UInt64 myKeyCount = 0, UInt64 myValueCount = 0)
            : this(myIndexName, new IndexKeyDefinition(myAttributes), correspondingType, myObjectDirectoryShards, myIndexType, myIndexEdition, myKeyCount, myValueCount)
        { }

        public AttributeIndex(string indexName, IndexKeyDefinition idxKey, GraphDBType correspondingType, UInt16 myAttributeIdxShards, string indexType = null, string indexEdition = DBConstants.DEFAULTINDEX, UInt64 myKeyCount = 0, UInt64 myValueCount = 0)
        {
            IndexName          = indexName;
            IndexEdition       = indexEdition;
            IndexKeyDefinition = idxKey;
            IndexRelatedTypeUUID = correspondingType.UUID;
            AttributeIdxShards = myAttributeIdxShards;

            _keyCount = myKeyCount;
            _valueCount = myValueCount;

            if (indexEdition == null)
            {
                IndexEdition = DBConstants.DEFAULTINDEX;
            }
            else
            {
                IndexEdition = indexEdition;
            }

            if (String.IsNullOrEmpty(indexType))
            {
                IndexType = VersionedHashIndexObject.Name;
            }
            else
            {
                IndexType = indexType;
            }

            #region Workaround for current IndexOperation of InOperator - just follow the IsListOfBaseObjectsIndex property

            // better approach, use a special index key for a set of base objects
            if (idxKey.IndexKeyAttributeUUIDs.Any(a =>
            {
                var typeAttr = correspondingType.GetTypeAttributeByUUID(a);
                if (typeAttr != null && (typeAttr.EdgeType is IBaseEdge))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }))
            {
                IsListOfBaseObjectsIndex = true;
            }
            else
            {
                IsListOfBaseObjectsIndex = false;
            }

            #endregion

            FileSystemLocation = (correspondingType.ObjectLocation + "Indices") + (IndexName + "#" + IndexEdition);
           
        }

        #endregion

        #region public methods

        #region Update

        /// <summary>
        /// <seealso cref=" AAtributeIndex"/>
        /// </summary>        
        public override Exceptional Update(DBObjectStream myDBObject, GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            #region remove DBObject from idx --> inperformant like hell

            foreach (var aIdxShardExceptional in GetAllIdxShards(myDBContext))
            {
                #region get the shard

                if (!aIdxShardExceptional.Item2.Success())
                {
                    return new Exceptional(aIdxShardExceptional.Item2);
                }
                var idxRefVal = aIdxShardExceptional.Item2.Value;

                #endregion

                #region remove

                HashSet<IndexKey> toBeRemovedIdxKeys = new HashSet<IndexKey>();

                foreach (var aKeyValue in idxRefVal.GetIDictionary())
                {
                    if (aKeyValue.Value.Remove(myDBObject.ObjectUUID))
                    {
                        //there has been something removed
                        DecreaseValueCount(1UL);
                    }

                    if (aKeyValue.Value.Count == 0)
                    {
                        toBeRemovedIdxKeys.Add(aKeyValue.Key);
                    }
                }

                foreach (var aToBeDeletedIndexKey in toBeRemovedIdxKeys)
                {
                    //a complete key has been removed
                    idxRefVal.Remove(aToBeDeletedIndexKey);

                    DecreaseKeyCount();
                }

                #endregion
            }

            #endregion

            #region insert new values

            if (myDBObject.HasAtLeastOneAttribute(this.IndexKeyDefinition.IndexKeyAttributeUUIDs, myTypeOfDBObject, myDBContext.SessionSettings))
            {
                //insert
                foreach (var aIndexKey in this.GetIndexkeysFromDBObject(myDBObject, myTypeOfDBObject, myDBContext))
                {
                    //get the actual shard
                    var currentIdxShard = GetIndexReference(myDBContext.DBIndexManager, myDBContext.DBIndexManager.GetIndexShardID(aIndexKey, this.AttributeIdxShards));

                    if (!currentIdxShard.Success())
                    {
                        return new Exceptional(currentIdxShard);
                    }
                    var currentIdxShardValue = currentIdxShard.Value;

                    SetIndexKeyAndValue(currentIdxShardValue, aIndexKey, myDBObject.ObjectUUID, IndexSetStrategy.MERGE);
                }
            }

            #endregion

            return Exceptional.OK;
        }

        #endregion

        #region Insert

        /// <summary>
        /// <seealso cref=" IAtributeIndex"/>
        /// </summary>
        public override Exceptional Insert(DBObjectStream myDBObject, GraphDBType myTypeOfDBobject, DBContext myDBContext)
        {
            return Insert(myDBObject, IndexSetStrategy.MERGE, myTypeOfDBobject, myDBContext);
        }

        /// <summary>
        /// <seealso cref=" IAtributeIndex"/>
        /// </summary>        
        public override Exceptional Insert(DBObjectStream myDBObject, IndexSetStrategy myIndexSetStrategy, GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            foreach (var aIndexKex in GetIndexkeysFromDBObject(myDBObject, myTypeOfDBObject, myDBContext))
            {
                #region get the shard

                //get the actual shard
                var currentIdxShard = GetIndexReference(myDBContext.DBIndexManager, myDBContext.DBIndexManager.GetIndexShardID(aIndexKex, this.AttributeIdxShards));

                if (!currentIdxShard.Success())
                {
                    return new Exceptional(currentIdxShard);
                }
                var currentIdxShardValue = currentIdxShard.Value;

                #endregion

                #region Check for uniqueness - TODO: remove me as soon as we have a unique indexObject implementation

                if (IsUniqueAttributeIndex)
                {
                    if (currentIdxShardValue.ContainsKey(aIndexKex))
                    {
                        return new Exceptional(new Error_UniqueConstrainViolation(myTypeOfDBObject.Name, IndexName));
                    }
                }

                #endregion

                SetIndexKeyAndValue(currentIdxShardValue, aIndexKex, myDBObject.ObjectUUID, myIndexSetStrategy);
            }
            
            return Exceptional.OK;

        }

        internal Exceptional Insert(IndexKey indexKey, HashSet<ObjectUUID> hashSet, int shard, DBIndexManager dBIndexManager, GraphDBType myTypeOfDBObject)
        {
            //get the actual shard
            var currentIdxShard = GetIndexReference(dBIndexManager, shard);

            if (!currentIdxShard.Success())
            {
                return new Exceptional(currentIdxShard);
            }

            #region Check for uniqueness - TODO: remove me as soon as we have a unique indexObject implementation

            if (IsUniqueAttributeIndex)
            {
                if (currentIdxShard.Value.ContainsKey(indexKey))
                {
                    return new Exceptional(new Error_UniqueConstrainViolation(myTypeOfDBObject.Name, IndexName));
                }
            }

            #endregion

            UInt64 previousKeyCount = currentIdxShard.Value.KeyCount();

            HashSet<ObjectUUID> value = null;

            currentIdxShard.Value.TryGetValue(indexKey, out value);

            if (value == null)
            {
                currentIdxShard.Value.Set(indexKey, hashSet, IndexSetStrategy.MERGE);

                IncreaseKeyCount();

                IncreaseValueCount((UInt64)hashSet.Count);
            }
            else
            {
                currentIdxShard.Value.Add(indexKey, hashSet);

                IncreaseValueCount((UInt64)currentIdxShard.Value[indexKey].Count);
            }

            return Exceptional.OK;
        }

        #endregion

        #region Contains
       
        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>        
        public override Boolean Contains(DBObjectStream myDBObject, GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {

            foreach (var aIndexKex in GetIndexkeysFromDBObject(myDBObject, myTypeOfDBObject, myDBContext))
            {
                #region get the shard

                //get the actual shard
                var currentIdxShard = GetIndexReference(myDBContext.DBIndexManager, myDBContext.DBIndexManager.GetIndexShardID(aIndexKex, this.AttributeIdxShards));

                if (!currentIdxShard.Success())
                {
                    throw new GraphDBException(new Error_CouldNotGetIndexReference(currentIdxShard.IErrors, IndexName, IndexEdition, myDBContext.DBIndexManager.GetIndexShardID(aIndexKex, this.AttributeIdxShards)));
                }
                
                var currentIdxShardValue = currentIdxShard.Value;

                #endregion

                if (currentIdxShardValue.Contains(aIndexKex, myDBObject.ObjectUUID))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// <seealso cref=" IAtributeIndex"/>
        /// </summary>        
        public override Boolean Contains(IndexKey myIndexKey, GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            #region get the shard

            //get the actual shard
            var currentIdxShard = GetIndexReference(myDBContext.DBIndexManager, myDBContext.DBIndexManager.GetIndexShardID(myIndexKey, this.AttributeIdxShards));

            if (!currentIdxShard.Success())
            {
                throw new GraphDBException(new Error_CouldNotGetIndexReference(currentIdxShard.IErrors, IndexName, IndexEdition, myDBContext.DBIndexManager.GetIndexShardID(myIndexKey, this.AttributeIdxShards)));
            }

            var currentIdxShardValue = currentIdxShard.Value;

            #endregion

            return currentIdxShardValue.ContainsKey(myIndexKey);
        }

        #endregion

        #region Remove

        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>        
        public override Exceptional Remove(DBObjectStream myDBObject, GraphDBType myTypeOfDBObjects, DBContext dbContext)
        {
            foreach (var aIndexKex in GetIndexkeysFromDBObject(myDBObject, myTypeOfDBObjects, dbContext))
            {
                #region get the shard

                //get the actual shard
                var currentIdxShard = GetIndexReference(dbContext.DBIndexManager, dbContext.DBIndexManager.GetIndexShardID(aIndexKex, this.AttributeIdxShards));

                if (!currentIdxShard.Success())
                {
                    return new Exceptional(currentIdxShard);
                }
                var currentIdxShardValue = currentIdxShard.Value;

                #endregion

                if (currentIdxShardValue.Remove(aIndexKex, myDBObject.ObjectUUID))
                {
                    //the ObjectUUID has been deleted from this idx... so decrease the valueCount
                    DecreaseValueCount(1UL);
                }

                if (currentIdxShardValue[aIndexKex].Count == 0)
                {
                    //so, the last element in this indexKey has just been removed...
                    DecreaseKeyCount();
                }
            }

            return Exceptional.OK;

        }

       
        internal Exceptional Remove(IndexKey indexKey, int shard, DBIndexManager myDBIndexManager)
        {
            //get the actual shard
            var currentIdxShard = GetIndexReference(myDBIndexManager, shard);
            
            if (!currentIdxShard.Success())
            {
                return new Exceptional(currentIdxShard);
            }

            HashSet<ObjectUUID> removedItems = null;

            currentIdxShard.Value.TryGetValue(indexKey, out removedItems);

            if (removedItems != null)
            {
                currentIdxShard.Value.Remove(indexKey);

                DecreaseKeyCount();

                DecreaseValueCount((UInt64)removedItems.Count);
            }

            return Exceptional.OK;
        }

        #endregion

        #region Clear

        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>        
        public override Exceptional ClearAndRemoveFromDisc(DBIndexManager indexManager)
        {
            lock (_lockObject)
            {
                _valueCount = 0;
                _keyCount = 0;
            }

            return indexManager.RemoveDBIndex(FileSystemLocation);
        }

        #endregion

        #region Overrides

        #region Equals Overrides

        public override int GetHashCode()
        {
            return IndexKeyDefinition.GetHashCode() ^ IndexName.GetHashCode() ^ IndexEdition.GetHashCode();
        }

        public override Boolean Equals(Object obj)
        {

            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            if (obj is AttributeIndex)
            {
                AttributeIndex p = (AttributeIndex)obj;
                return Equals(p);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(AttributeIndex p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            if (this.IndexName != p.IndexName)
            {
                return false;
            }

            if (this.IndexEdition != p.IndexEdition)
            {
                return false;
            }

            if (this.IndexKeyDefinition != p.IndexKeyDefinition)
            {
                return false;
            }

            return true;
        }

        public static Boolean operator ==(AttributeIndex a, AttributeIndex b)
        {
            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static Boolean operator !=(AttributeIndex a, AttributeIndex b)
        {
            return !(a == b);
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return IndexName;
        }

        #endregion

        #endregion

        #region GetKeys

        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>        
        public override IEnumerable<IndexKey> GetKeys(GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            foreach (var aIdxShardExceptional in GetAllIdxShards(myDBContext))
            {
                #region get the shard

                if (!aIdxShardExceptional.Item2.Success())
                {
                    throw new GraphDBException(new Error_CouldNotGetIndexReference(aIdxShardExceptional.Item2.IErrors, IndexName, IndexEdition, aIdxShardExceptional.Item1));
                }

                var idxRefVal = aIdxShardExceptional.Item2.Value;

                #endregion

                foreach (var aKey in idxRefVal.Keys())
                {
                    yield return aKey;
                }
            }

            yield break;
        }

        #endregion

        #region GetValues

        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>
        public override IEnumerable<ObjectUUID> GetValues(IndexKey myIndeyKey, GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            #region get the shard

            //get the actual shard
            var currentIdxShard = GetIndexReference(dbContext.DBIndexManager, dbContext.DBIndexManager.GetIndexShardID(myIndeyKey, this.AttributeIdxShards));

            if (!currentIdxShard.Success())
            {
                throw new GraphDBException(new Error_CouldNotGetIndexReference(currentIdxShard.IErrors, IndexName, IndexEdition, dbContext.DBIndexManager.GetIndexShardID(myIndeyKey, this.AttributeIdxShards)));
            }
            var currentIdxShardValue = currentIdxShard.Value;

            #endregion

            return currentIdxShardValue[myIndeyKey];
        }

        #endregion

        #region GetAllValues

        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>
        public override IEnumerable<IEnumerable<ObjectUUID>> GetAllValues(GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            foreach (var aIdxShardExceptional in GetAllIdxShards(dbContext))
            {
                #region get the shard

                if (!aIdxShardExceptional.Item2.Success())
                {
                    throw new GraphDBException(new Error_CouldNotGetIndexReference(aIdxShardExceptional.Item2.IErrors, IndexName, IndexEdition, aIdxShardExceptional.Item1));
                }

                var idxRefVal = aIdxShardExceptional.Item2.Value;

                #endregion

                foreach (var aValue in idxRefVal.Values())
                {
                    yield return aValue;
                }
            }

            yield break;
        }

        #endregion

        #region GetKeyValues

        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>
        public override IEnumerable<KeyValuePair<IndexKey, HashSet<ObjectUUID>>> GetKeyValues(GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            foreach (var aIdxShardExceptional in GetAllIdxShards(dbContext))
            {
                #region get the shard

                if (!aIdxShardExceptional.Item2.Success())
                {
                    throw new GraphDBException(new Error_CouldNotGetIndexReference(aIdxShardExceptional.Item2.IErrors, IndexName, IndexEdition, aIdxShardExceptional.Item1));
                }

                var idxRefVal = aIdxShardExceptional.Item2.Value;

                #endregion

                foreach (var aKV in idxRefVal)
                {
                    yield return aKV;
                }
            }

            yield break;

        }

        #endregion

        #region GetValueCount

        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>
        public override UInt64 GetValueCount()
        {
            return _valueCount;
        }

        #endregion

        #region GetKeyCount

        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>
        public override UInt64 GetKeyCount()
        {
            return _keyCount;
        }

        #endregion

        #region InRange

        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>
        public override IEnumerable<ObjectUUID> InRange(IndexKey fromIndexKey, IndexKey toIndexKey, bool myOrEqualFromKey, bool myOrEqualToKey, GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            //TODO: PERFORMANCE BUG!!!!!!!

            foreach (var aIdxShardExceptional in GetAllIdxShards(myDBContext))
            {
                #region get the shard

                if (!aIdxShardExceptional.Item2.Success())
                {
                    throw new GraphDBException(new Error_CouldNotGetIndexReference(aIdxShardExceptional.Item2.IErrors, IndexName, IndexEdition, aIdxShardExceptional.Item1));
                }

                var idxRefVal = aIdxShardExceptional.Item2.Value;

                #endregion

                foreach (var aUUID in idxRefVal.InRange(fromIndexKey, toIndexKey, myOrEqualFromKey, myOrEqualToKey))
                {
                    yield return aUUID;
                }
            }

            yield break;

        }

        #endregion

        #endregion

        #region private helper

        #region IndexReference

        /// <summary>
        /// A reference to this index after it was loaded into the memory
        /// or connected by a proxy class
        /// </summary>
        /// <param name="indexManager">The database index manager</param>
        /// <param name="idxShard">The shard that should be loaded</param>
        /// <returns>A versioned idx object</returns>
        public Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>> GetIndexReference(DBIndexManager indexManager, int idxShard)
        {
            if (!indexManager.HasIndex(IndexType))
            {
                // the index type does not exist anymore - return null or throw exception
                return new Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>>(new GraphDBError("Index is away!"));
            }

            var emptyIdx = indexManager.GetIndex(IndexType);
            if (!emptyIdx.Success())
            {
                return emptyIdx;
            }

            var indexExceptional = indexManager.LoadOrCreateDBIndex(FileSystemLocation + idxShard.ToString(), emptyIdx.Value, this);

            if (indexExceptional.Failed())
            {
                return new Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>>(indexExceptional);
            }

            return indexExceptional;
        }


        #endregion

        #region GetIndexkeysFromDBObject

        /// <summary>
        /// Creates IndexKeys from a DBObject.
        /// </summary>
        /// <param name="myDBObject">The DBObject reference for the resulting IndexKeys</param>
        /// <param name="myTypeOfDBObject">The Type of the DBObject</param>
        /// <param name="myToken">The SessionInfos</param>
        /// <returns>A HashSet of IndexKeys</returns>
        private HashSet<IndexKey> GetIndexkeysFromDBObject(DBObjectStream myDBObject, GraphDBType myTypeOfDBObject, DBContext dbContext)
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

        private void IncreaseValueCount(UInt64 increase)
        {
            lock (_lockObject)
            {
                _valueCount += increase;
            }
        }

        private void IncreaseKeyCount()
        {
            lock (_lockObject)
            {
                _keyCount++;
            }
        }

        private void DecreaseKeyCount()
        {
            lock (_lockObject)
            {
                _keyCount--;
            }
        }

        private void DecreaseValueCount(UInt64 decrease)
        {
            lock (_lockObject)
            {
                _valueCount -= decrease;
            }
        }

        /// <summary>
        /// Get all index shards
        /// </summary>
        /// <param name="dbContext">The current database context</param>
        /// <returns>An enumerable of shardID/IVersionedIndexObject</returns>
        internal IEnumerable<Tuple<int, Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>>>> GetAllIdxShards(DBContext dbContext)
        {
            foreach (var aIdxShardID in GetAllIdxShardIDs())
            {
                #region load the shard

                yield return new Tuple<int, Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>>>(aIdxShardID, LoadAnIdxShard(aIdxShardID, dbContext));

                #endregion
            }

            yield break;
        }

        /// <summary>
        /// Loads an index shard
        /// </summary>
        /// <param name="aIdxShardID">The index shard id</param>
        /// <param name="dbContext">The current database context</param>
        /// <returns>An IVersionedIndexObject</returns>
        private Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>> LoadAnIdxShard(ushort aIdxShardID, DBContext dbContext)
        {
            var idxRef = GetIndexReference(dbContext.DBIndexManager, aIdxShardID);
            if (!idxRef.Success())
            {
                return new Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>>(idxRef);
            }

            return idxRef;
        }

        /// <summary>
        /// Returns all index shard ids
        /// </summary>
        /// <returns>Number of shards</returns>
        private IEnumerable<UInt16> GetAllIdxShardIDs()
        {
            //stupid
            for (UInt16 i = 0; i < AttributeIdxShards; i++)
            {
                yield return i;
            }

            yield break;
        }

        private void SetIndexKeyAndValue(IVersionedIndexObject<IndexKey, ObjectUUID> currentIdxShardValue, IndexKey aIndexKex, ObjectUUID objectUUID, IndexSetStrategy myIndexSetStrategy)
        {

            UInt64 previousKeyCount = currentIdxShardValue.KeyCount();

            currentIdxShardValue.Set(aIndexKex, objectUUID, myIndexSetStrategy);

            UInt64 afterKeyCount = currentIdxShardValue.KeyCount();

            if (afterKeyCount > previousKeyCount)
            {
                //so there is one more key...
                IncreaseKeyCount();
            }

            IncreaseValueCount(1UL);
        }

        #endregion
    }
}
