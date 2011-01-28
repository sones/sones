/* <id name="GraphDB – HashTable AttributeIndex" />
 * <copyright file="HashTableIndex.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
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
using sones.Lib;

#endregion

namespace sones.GraphDB.Indices
{
    /// <summary>
    /// This datastructure contains information concerning a single attribute index
    /// </summary>
    public class HashTableIndex : AAttributeIndex
    {
        public const String INDEX_TYPE = "HashTable";
        public override string IndexType
        {
            get { return INDEX_TYPE; }
        }

        #region Properties

        Object _lockObject = new object();

        /// <summary>
        /// The index datastructure
        /// </summary>
        VersionedHashIndexObject<IndexKey, ObjectUUID> _indexDatastructure = null;

        UInt64 _keyCount = 0;
        UInt64 _valueCount = 0;

        private Boolean _IsUUIDIndex;
        public override Boolean IsUUIDIndex
        {
            get
            {
                return _IsUUIDIndex;
            }
        }

        #endregion

        #region Constructor

        public HashTableIndex() { }

        #endregion

        #region public methods

        #region Update
     
        public override Exceptional Update(DBObjectStream myDBObject, GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            VerifyIndexDatastructure(myDBContext, myTypeOfDBObject);

            #region remove

            HashSet<IndexKey> toBeRemovedIdxKeys = new HashSet<IndexKey>();

            foreach (var aKeyValue in _indexDatastructure.GetIDictionary())
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
                _indexDatastructure.Remove(aToBeDeletedIndexKey);

                DecreaseKeyCount();
            }

            #endregion

            #region insert new values

            if (myDBObject.HasAtLeastOneAttribute(this.IndexKeyDefinition.IndexKeyAttributeUUIDs, myTypeOfDBObject, myDBContext.SessionSettings))
            {
                //insert
                var result = GetIndexkeysFromDBObject(myDBObject, myTypeOfDBObject, myDBContext);
                if (result.Failed())
                {
                    return result;
                }
                foreach (var aIndexKey in result.Value)
                {
                    SetIndexKeyAndValue(aIndexKey, myDBObject.ObjectUUID, IndexSetStrategy.MERGE);
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
            VerifyIndexDatastructure(myDBContext, myTypeOfDBObject);

            System.Diagnostics.Debug.Assert(_indexDatastructure != null);

            var result = GetIndexkeysFromDBObject(myDBObject, myTypeOfDBObject, myDBContext);
            if (result.Failed())
            {
                return result;
            }

            foreach (var aIndexKex in result.Value)
            {
                #region Check for uniqueness - TODO: remove me as soon as we have a unique indexObject implementation

                if (IsUniqueAttributeIndex)
                {
                    if (_indexDatastructure.ContainsKey(aIndexKex))
                    {
                        return new Exceptional(new Error_UniqueConstrainViolation(myTypeOfDBObject.Name, IndexName));
                    }
                }

                #endregion

                SetIndexKeyAndValue(aIndexKex, myDBObject.ObjectUUID, myIndexSetStrategy);
            }
            
            return Exceptional.OK;

        }

        private Exceptional Insert(IndexKey indexKey, HashSet<ObjectUUID> hashSet, int shard, DBIndexManager dBIndexManager, GraphDBType myTypeOfDBObject)
        {

            #region Check for uniqueness - TODO: remove me as soon as we have a unique indexObject implementation

            if (IsUniqueAttributeIndex)
            {
                if (_indexDatastructure.ContainsKey(indexKey))
                {
                    return new Exceptional(new Error_UniqueConstrainViolation(myTypeOfDBObject.Name, IndexName));
                }
            }

            #endregion

            UInt64 previousKeyCount = _keyCount;

            HashSet<ObjectUUID> value = null;

            _indexDatastructure.TryGetValue(indexKey, out value);

            if (value == null)
            {
                _indexDatastructure.Set(indexKey, hashSet, IndexSetStrategy.MERGE);

                IncreaseKeyCount();

                IncreaseValueCount((UInt64)hashSet.Count);
            }
            else
            {
                _indexDatastructure.Add(indexKey, hashSet);

                IncreaseValueCount((UInt64)_indexDatastructure[indexKey].Count);
            }

            return Exceptional.OK;
        }

        #endregion

        #region Contains
       
        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>        
        public override Exceptional<Boolean> Contains(DBObjectStream myDBObject, GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            VerifyIndexDatastructure(myDBContext, myTypeOfDBObject);

            var result = GetIndexkeysFromDBObject(myDBObject, myTypeOfDBObject, myDBContext);
            if (result.Failed())
            {
                return result.Convert<Boolean>();
            }

            foreach (var aIndexKex in result.Value)
            {

                if (_indexDatastructure.Contains(aIndexKex, myDBObject.ObjectUUID))
                {
                    return new Exceptional<bool>(true);
                }
            }

            return new Exceptional<bool>(false);
        }

        /// <summary>
        /// <seealso cref=" IAtributeIndex"/>
        /// </summary>        
        public override Exceptional<Boolean> Contains(IndexKey myIndexKey, GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            VerifyIndexDatastructure(myDBContext, myTypeOfDBObject);
            return new Exceptional<bool>((bool) _indexDatastructure.ContainsKey(myIndexKey));
        }

        #endregion

        #region Remove

        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>        
        public override Exceptional Remove(DBObjectStream myDBObject, GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            VerifyIndexDatastructure(myDBContext, myTypeOfDBObject);

            var result = GetIndexkeysFromDBObject(myDBObject, myTypeOfDBObject, myDBContext);
            if (result.Failed())
            {
                return result;
            }

            foreach (var aIndexKex in result.Value)
            {
                if (_indexDatastructure.Remove(aIndexKex, myDBObject.ObjectUUID))
                {
                    //the ObjectUUID has been deleted from this idx... so decrease the valueCount
                    DecreaseValueCount(1UL);
                }

                if (_indexDatastructure[aIndexKex].Count == 0)
                {
                    //so, the last element in this indexKey has just been removed...
                    DecreaseKeyCount();
                }
            }

            return Exceptional.OK;

        }

       
        internal Exceptional Remove(IndexKey indexKey)
        {
            
            HashSet<ObjectUUID> removedItems = null;

            _indexDatastructure.TryGetValue(indexKey, out removedItems);

            if (removedItems != null)
            {
                _indexDatastructure.Remove(indexKey);

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
        public override Exceptional ClearAndRemoveFromDisc(DBContext myDBContext)
        {
            lock (_lockObject)
            {
                _valueCount = 0;
                _keyCount = 0;
            }

            return myDBContext.IGraphFSSession.RemoveFSObject(FileSystemLocation, DBConstants.DBINDEXSTREAM);
        }

        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>        
        public override Exceptional Clear(DBContext myDBContext, GraphDBType myTypeOfDBObject)
        {
            lock (_lockObject)
            {
                VerifyIndexDatastructure(myDBContext, myTypeOfDBObject);
                _valueCount = 0;
                _keyCount = 0;
            }
            _indexDatastructure.Clear();
            return Exceptional.OK;
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
            if (obj is HashTableIndex)
            {
                HashTableIndex p = (HashTableIndex)obj;
                return Equals(p);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(HashTableIndex p)
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

        public static Boolean operator ==(HashTableIndex a, HashTableIndex b)
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

        public static Boolean operator !=(HashTableIndex a, HashTableIndex b)
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
            VerifyIndexDatastructure(myDBContext, myTypeOfDBObject);
            foreach (var aKey in _indexDatastructure.Keys())
            {
                yield return aKey;
            }

            yield break;
        }

        #endregion

        #region GetValues

        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>
        public override IEnumerable<ObjectUUID> GetValues(IndexKey myIndeyKey, GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            VerifyIndexDatastructure(myDBContext, myTypeOfDBObject);

            return _indexDatastructure[myIndeyKey];
        }

        #endregion

        #region GetAllValues

        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>
        public override IEnumerable<IEnumerable<ObjectUUID>> GetAllValues(GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            VerifyIndexDatastructure(myDBContext, myTypeOfDBObject);
            foreach (var aValue in _indexDatastructure.Values())
            {
                yield return aValue;
            }

            yield break;
        }

        #endregion

        #region GetKeyValues

        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>
        public override IEnumerable<KeyValuePair<IndexKey, HashSet<ObjectUUID>>> GetKeyValues(GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            VerifyIndexDatastructure(myDBContext, myTypeOfDBObject);

            foreach (var aKV in _indexDatastructure)
            {
                yield return aKV;
            }

            yield break;

        }

        private void VerifyIndexDatastructure(DBContext myDBContext, GraphDBType myTypeOfDBObject)
        {
            if (_indexDatastructure == null)
            {
                SetIndexReference(myDBContext);
            }
        }

        #endregion

        #region GetValueCount

        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>
        public override UInt64 GetValueCount(DBContext myDBContext, GraphDBType myTypeOfDBObject)
        {
            VerifyIndexDatastructure(myDBContext, myTypeOfDBObject);

            return _indexDatastructure.ValueCount();
        }

        #endregion

        #region GetKeyCount

        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>
        public override UInt64 GetKeyCount(DBContext myDBContext, GraphDBType myTypeOfDBObject)
        {
            VerifyIndexDatastructure(myDBContext, myTypeOfDBObject);

            return _indexDatastructure.KeyCount();
        }

        #endregion

        #region InRange

        /// <summary>
        /// <seealso cref=" IAttributeIndex"/>
        /// </summary>
        public override IEnumerable<ObjectUUID> InRange(IndexKey fromIndexKey, IndexKey toIndexKey, bool myOrEqualFromKey, bool myOrEqualToKey, GraphDBType myTypeOfDBObject, DBContext myDBContext)
        {
            VerifyIndexDatastructure(myDBContext, myTypeOfDBObject);

            foreach (var aUUID in _indexDatastructure.InRange(fromIndexKey, toIndexKey, myOrEqualFromKey, myOrEqualToKey))
            {
                yield return aUUID;
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
        private Exceptional SetIndexReference(DBContext myDBContext)
        {

            var result = myDBContext.IGraphFSSession.GetOrCreateFSObject<AFSObject>(FileSystemLocation, DBConstants.DBINDEXSTREAM, () => new VersionedHashIndexObject<IndexKey, ObjectUUID>());
            if (!result.Success())
            {
                return new Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>>(result);
            }

            else
            {

                if (result.Value.isNew)
                {

                    // Uncomment as soon as index is serializeable
                    result.PushIExceptional(myDBContext.IGraphFSSession.StoreFSObject(result.Value, false));

                    if (result.Failed())
                    {
                        return new Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>>(result);
                    }

                    //result.AddErrorsAndWarnings(result.Value.Save());
                    //ToDo: Fehler beim Speichern werden im Weiterm ignoriert statt darauf reagiert!
                }

                _indexDatastructure = result.Value as VersionedHashIndexObject<IndexKey, ObjectUUID>;

            }

            return Exceptional.OK;
            
        //    var indexExceptional = indexManager.LoadOrCreateVersionedDBIndex(FileSystemLocation, new VersionedHashIndexObject<IndexKey, ObjectUUID>(), this);

        //    if (indexExceptional.Failed())
        //    {
        //        return new Exceptional(indexExceptional);
        //    }

        //    _indexDatastructure = indexExceptional.Value as VersionedHashIndexObject<IndexKey, ObjectUUID>;

        //    return Exceptional.OK;
        //}
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

        private void SetIndexKeyAndValue(IndexKey myIndexKey, ObjectUUID objectUUID, IndexSetStrategy myIndexSetStrategy)
        {

            HashSet<ObjectUUID> value = null;

            var valueCount = 1UL;

            if (!_indexDatastructure.TryGetValue(myIndexKey, out value))
            {
                //so there is one more key...
                IncreaseKeyCount();
                IncreaseValueCount(valueCount);
            }
            else
            {
                if (!value.Contains(objectUUID))
                {
                    IncreaseValueCount(valueCount);
                }
            }

            _indexDatastructure.Set(myIndexKey, objectUUID, myIndexSetStrategy);
            this.Save();
        }

        #endregion


        public override AAttributeIndex GetNewInstance()
        {
            return new HashTableIndex();
        }

        #region IFastSerializationTypeSurrogate Members

        public override bool SupportsType(Type type)
        {
            if (type == typeof(HashTableIndex)) return true;
            return false;
        }

        public override void Serialize(ref Lib.NewFastSerializer.SerializationWriter mySerializationWriter)
        {
            mySerializationWriter.WriteString(FileSystemLocation.ToString());
            mySerializationWriter.WriteString(IndexEdition);
            mySerializationWriter.WriteString(IndexName);
            mySerializationWriter.WriteBoolean(_IsUUIDIndex);
            IndexKeyDefinition.Serialize(ref mySerializationWriter);
            IndexRelatedTypeUUID.Serialize(ref mySerializationWriter);
        }

        public override void Deserialize(ref Lib.NewFastSerializer.SerializationReader mySerializationReader)
        {
            FileSystemLocation  = new ObjectLocation(ObjectLocation.ParseString(mySerializationReader.ReadString()));
            IndexEdition        = mySerializationReader.ReadString();
            IndexName           = mySerializationReader.ReadString();
            _IsUUIDIndex        = mySerializationReader.ReadBoolean();
            IndexKeyDefinition = new IndexKeyDefinition();
            IndexKeyDefinition.Deserialize(ref mySerializationReader);
            IndexRelatedTypeUUID = new TypeUUID(ref mySerializationReader);
        }

        public override uint TypeCode { get { return 1002; } }

        #endregion

        public override AFSObject Clone()
        {
            return new HashTableIndex();
        }

        public override ulong GetEstimatedSize()
        {
            return EstimatedSizeConstants.AFSObjectOntologyObject;
        }

        public override Exceptional Initialize(DBContext myDBContext, string indexName, IndexKeyDefinition idxKey, GraphDBType correspondingType, string indexEdition = DBConstants.DEFAULTINDEX)
        {
            IndexName = indexName;
            IndexEdition = indexEdition;
            IndexKeyDefinition = idxKey;
            IndexRelatedTypeUUID = correspondingType.UUID;
            _keyCount = 0;
            _valueCount = 0;

            if (indexEdition == null)
            {
                IndexEdition = DBConstants.DEFAULTINDEX;
            }
            else
            {
                IndexEdition = indexEdition;
            }

            _IsUUIDIndex = idxKey.IndexKeyAttributeUUIDs.Count == 1 && idxKey.IndexKeyAttributeUUIDs[0].Equals(myDBContext.DBTypeManager.GetUUIDTypeAttribute().UUID);

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

            FileSystemLocation = correspondingType.ObjectLocation + DBConstants.DBIndicesLocation + (IndexName + "#" + IndexEdition);

            return Exceptional.OK;
        }
    }
}
