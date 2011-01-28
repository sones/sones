/* <id name="GraphDB – UUIDIndex" />
 * <copyright file="UUIDIndex.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This index is a wrapper for the GraphFS directory object</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.DataStructures;
using sones.GraphFS.InternalObjects;
using sones.Lib;
using sones.Lib.DataStructures.Indices;
using sones.Lib.ErrorHandling;
using System.Threading.Tasks;
using System.Threading;
using sones.GraphFS.Settings;
using sones.GraphFS.Objects;

#endregion

namespace sones.GraphDB.Indices
{
    /// <summary>
    /// This index is a wrapper for the GraphFS directory object
    /// </summary>
    public class UUIDIndex : AAttributeIndex
    {
        public const String INDEX_TYPE = "UUID";
        public override string IndexType
        {
            get { return INDEX_TYPE; }
        }

        #region Properties

        Object _lockObject = new object();
        UInt64 _KeyCount = 0UL;

        public override Boolean IsUUIDIndex { get { return true; } }

        #endregion

        #region Constructor

        public UUIDIndex() { }

        /// <summary>
        /// Creates a new AttributeIndex object
        /// </summary>
        /// <param name="myIndexName">The user-defined name of this index</param>
        /// <param name="myIndexEdition">You may have different versions of an attribute index, e.g. HashMap, BTree to speed up different database operations</param>
        /// <param name="myAttributes">The list of attributes that is needed for the creation of a IndexKeyDefinition</param>
        /// <param name="myIndexType">The IndexType e.g. HashMap, BTree of this AttributeIndex</param>
        /// <param name="correspondingType">The corresponding type of this index, used to get the file system location</param>
        /// <param name="myFileSystemLocation">The location oif the index. If null it will be generated based on the <paramref name="correspondingType"/>.</param>
        public UUIDIndex(DBContext myDBContext, String myIndexName, String myIndexEdition, List<AttributeUUID> myAttributes, GraphDBType correspondingType, ulong myDirectoryDelta)
            : this(myDBContext, myIndexName, new IndexKeyDefinition(myAttributes), correspondingType, myIndexEdition)
        { }

        public UUIDIndex(DBContext myDBContext, string indexName, IndexKeyDefinition idxKey, GraphDBType correspondingType, String indexEdition = DBConstants.DEFAULTINDEX)
        {
            Initialize(myDBContext, indexEdition, idxKey, correspondingType, indexEdition);
        }

        #endregion

        #region public methods

        #region Update

        /// <summary>
        /// This method updates the idx corresponding to an DBObject
        /// </summary>
        /// <param name="myDBObject">The DBObject that should be updated</param>
        /// <param name="myTypeOfDBObject">The type of the DBObject</param>
        /// <param name="myToken">The SessionInfos</param>
        public override Exceptional Update(DBObjectStream myDBObject, GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            //don't do anything, because the number of DBObjects does not change

            return new Exceptional();
        }

        #endregion

        #region Insert

        /// <summary>
        /// This method inserts the given DBObject into the index
        /// </summary>
        /// <param name="myDBObject">The DBObject that should be inserted</param>
        /// <param name="myTypeOfDBobject">The type of the DBObject</param>
        /// <param name="myToken">The SessionInfos</param>
        public override Exceptional Insert(DBObjectStream myDBObject, GraphDBType myTypeOfDBobject, DBContext dbContext)
        {
            return Insert(myDBObject, IndexSetStrategy.MERGE, myTypeOfDBobject, dbContext);
        }

        /// <summary>
        /// This method inserts the given DBObject into the index
        /// </summary>
        /// <param name="myDBObject">The DBObject that should be inserted</param>
        /// <param name="myIndexSetStrategy">The index merge strategy</param>
        /// <param name="myTypeOfDBObject">The type of the DBObject</param>
        /// <param name="myToken">The SessionInfos</param>
        public override Exceptional Insert(DBObjectStream myDBObject, IndexSetStrategy myIndexSetStrategy, GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            lock (_lockObject)
            {
                _KeyCount++;
                return this.Save();
            }
        }

        #endregion

        #region Contains
       
        /// <summary>
        /// This method checks if the current attribute index contains a DBObject
        /// </summary>
        /// <param name="myDBObject">The DBObject that should be checked</param>
        /// <param name="myTypeOfDBObject">The Type of the DBObject</param>
        /// <param name="myToken">The SessionInfos</param>
        /// <returns>A Trinary</returns>
        public override Exceptional<Boolean> Contains(DBObjectStream myDBObject, GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            return new Exceptional<bool>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        public override Exceptional<Boolean> Contains(IndexKey myIndeyKey, GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            if (myIndeyKey.IndexKeyValues.Count == 1)
            {
                var uuid = myIndeyKey.IndexKeyValues[0].Value as ObjectUUID;

                if(uuid == null)
                {
                    throw new GraphDBException(new Error_InvalidIndexOperation(IndexName));
                }
                else
                {
                    return Contains_private(GetDirectoryObject(myTypeOfDBObject, dbContext), uuid.ToString());
                }
            }
            else
            {
                return new Exceptional<bool>(new Error_InvalidIndexOperation(IndexName));
            }
        }

        private Exceptional<Boolean> Contains_private(IDirectoryObject myIDirectoryObject, string myUUIDAsString)
        {
            return new Exceptional<Boolean>((bool)myIDirectoryObject.ObjectExists(myUUIDAsString) && myIDirectoryObject.GetDirectoryEntry(myUUIDAsString).ObjectStreamsList.Contains(DBConstants.DBOBJECTSTREAM));
        }

        public Exceptional<Boolean> Contains(ObjectUUID myUUID, GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            return Contains_private(GetDirectoryObject(myTypeOfDBObject, dbContext), myUUID.ToString());
        }

        #endregion

        #region Remove

        /// <summary>
        /// This method removes a given DBObject from the index
        /// </summary>
        /// <param name="myDBObject">The DBObject that should be removed</param>
        /// <param name="myTypeOfDBObjects">The type of the DBObject</param>
        /// <param name="myToken">The SessionInfos</param>
        public override Exceptional Remove(DBObjectStream myDBObject, GraphDBType myTypeOfDBObjects, DBContext dbContext)
        {
            lock (_lockObject)
            {
                _KeyCount--;
                this.Save();
            }
            return Exceptional.OK;
        }

        #endregion

        #region Clear

        public override Exceptional ClearAndRemoveFromDisc(DBContext myDBContext)
        {
            return Exceptional.OK;
        }

        public override Exceptional Clear(DBContext myDBContext, GraphDBType myDBTypeStream)
        {
            lock (_lockObject)
            {
                _KeyCount = 0;
                this.Save();
            }
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
            if (obj is UUIDIndex)
            {
                UUIDIndex p = (UUIDIndex)obj;
                return Equals(p);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(UUIDIndex p)
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

        public static Boolean operator ==(UUIDIndex a, UUIDIndex b)
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

        public static Boolean operator !=(UUIDIndex a, UUIDIndex b)
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

        public override IEnumerable<IndexKey> GetKeys(GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            foreach (var aUUID in GetAllUUIDs(myTypeOfDBObject, dbContext))
            {
                yield return new IndexKey(IndexKeyDefinition.IndexKeyAttributeUUIDs[0], new DBReference(aUUID), IndexKeyDefinition);
            }

            yield break;
        }

        #endregion

        #region GetValues

        public override IEnumerable<ObjectUUID> GetValues(IndexKey myIndeyKey, GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            var result = Contains(myIndeyKey, myTypeOfDBObject, dbContext);
            if (result.Value)
            {
                //this has to work, because the same thing has been tested during contains
                yield return (ObjectUUID)myIndeyKey.IndexKeyValues[0].Value;
            }

            yield break;
        }

        #endregion

        #region GetAllValues

        public override IEnumerable<IEnumerable<ObjectUUID>> GetAllValues(GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            foreach (var aUUID in GetAllUUIDs(myTypeOfDBObject, dbContext))
            {
                IEnumerable<ObjectUUID> payload = new HashSet<ObjectUUID>() {aUUID};

                yield return payload;
            }

            yield break;
        }

        #endregion

        #region GetKeyValues

        public override IEnumerable<KeyValuePair<IndexKey, HashSet<ObjectUUID>>> GetKeyValues(GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            foreach (var aUUID in GetAllUUIDs(myTypeOfDBObject, dbContext))
            {
                HashSet<ObjectUUID> value = new HashSet<ObjectUUID>() { aUUID };

                yield return new KeyValuePair<IndexKey, HashSet<ObjectUUID>>(new IndexKey(IndexKeyDefinition.IndexKeyAttributeUUIDs[0], new DBReference(aUUID), IndexKeyDefinition), value);
            }

            yield break;
        }

        #endregion

        #region GetValueCount

        public override UInt64 GetValueCount(DBContext myDBContext, GraphDBType myTypeOfDBObject)
        {
            lock (_lockObject)
            {
                return _KeyCount;
            }
        }

        #endregion

        #region GetKeyCount

        public override UInt64 GetKeyCount(DBContext myDBContext, GraphDBType myTypeOfDBObject)
        {
            lock (_lockObject)
            {
                return _KeyCount;
            }
        }

        #endregion

        #region GetAllUUIDs

        public IEnumerable<ObjectUUID> GetAllUUIDs(GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            return GetDirectoryObject(myTypeOfDBObject, dbContext).
                GetDirectoryListing(null, null, null, new List<string>() {DBConstants.DBOBJECTSTREAM}, null).
                        Select(aUUIDString => new ObjectUUID(aUUIDString));
        }

        #endregion

        #region InRange

        public override IEnumerable<ObjectUUID> InRange(IndexKey fromIndexKey, IndexKey toIndexKey, bool myOrEqualFromKey, bool myOrEqualToKey, GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            var fromUUID = (ObjectUUID)fromIndexKey.IndexKeyValues[0].Value;
            var toUUID = (ObjectUUID)toIndexKey.IndexKeyValues[0].Value;

            if (fromUUID <= toUUID)
            {
                #region <=

                if (myOrEqualFromKey && myOrEqualToKey)
                {
                    foreach (var aUUID in GetAllUUIDs(myTypeOfDBObject, dbContext).Where(item =>
                        {
                            return (item >= fromUUID) && (item <= toUUID);
                        }))
                    {
                        yield return aUUID;
                    }
                }
                else
                {
                    if (!myOrEqualFromKey && myOrEqualToKey)
                    {
                        foreach (var aUUID in GetAllUUIDs(myTypeOfDBObject, dbContext).Where(item =>
                        {
                            return (item > fromUUID) && (item <= toUUID);
                        }))
                        {
                            yield return aUUID;
                        }
                    }
                    else
                    {
                        if (myOrEqualFromKey && !myOrEqualToKey)
                        {
                            foreach (var aUUID in GetAllUUIDs(myTypeOfDBObject, dbContext).Where(item =>
                            {
                                return (item >= fromUUID) && (item < toUUID);
                            }))
                            {
                                yield return aUUID;
                            }
                        }
                        else
                        {
                            foreach (var aUUID in GetAllUUIDs(myTypeOfDBObject, dbContext).Where(item =>
                            {
                                return (item > fromUUID) && (item < toUUID);
                            }))
                            {
                                yield return aUUID;
                            }
                        }
                    }
                }

                #endregion
            }
            else
            {
                if (fromUUID == toUUID)
                {
                    #region ==
                    var result = Contains(fromIndexKey, myTypeOfDBObject, dbContext);
                    if (result.Value)
                    {
                        yield return (ObjectUUID)fromIndexKey.IndexKeyValues[0].Value;
                    }

                    #endregion
                }
                else
                {
                    #region >

                    #region part 1

                    if (myOrEqualToKey)
                    {
                        foreach (var aUUID in GetAllUUIDs(myTypeOfDBObject, dbContext).Where(item =>
                        {
                            return (item >= fromUUID);
                        }))
                        {
                            yield return aUUID;
                        }
                    }
                    else
                    {
                        foreach (var aUUID in GetAllUUIDs(myTypeOfDBObject, dbContext).Where(item =>
                        {
                            return (item > fromUUID);
                        }))
                        {
                            yield return aUUID;
                        }
                    }

                    #endregion

                    #region part 2

                    if (myOrEqualFromKey)
                    {
                        foreach (var aUUID in GetAllUUIDs(myTypeOfDBObject, dbContext).Where(item =>
                        {
                            return (item <= toUUID);
                        }))
                        {
                            yield return aUUID;
                        }
                    }
                    else
                    {
                        foreach (var aUUID in GetAllUUIDs(myTypeOfDBObject, dbContext).Where(item =>
                        {
                            return (item < toUUID);
                        }))
                        {
                            yield return aUUID;
                        }
                    }

                    #endregion

                    #endregion
                }
            }

            yield break;
        }

        #endregion

        #endregion

        #region private helper

        private IDirectoryObject GetDirectoryObject(GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            var directoryException = dbContext.DBTypeManager.GetObjectsDirectory(myTypeOfDBObject);

            if (directoryException.Failed())
            {
                throw new GraphDBException(new Error_CouldNotGetIndexReference(directoryException.IErrors, IndexName, IndexEdition));
            }

            return directoryException.Value;
        }

        #endregion

        public override AAttributeIndex GetNewInstance()
        {
            return new UUIDIndex();
        }

        #region IFastSerializationTypeSurrogate Members

        public override bool SupportsType(Type type)
        {
            if (type == typeof(UUIDIndex)) return true;
            return false;
        }


        public override uint TypeCode { get { return 1001; } }

        #endregion

        #region IFastSerialize Members

        public override void Serialize(ref Lib.NewFastSerializer.SerializationWriter mySerializationWriter)
        {
            mySerializationWriter.WriteUInt64(_KeyCount);
            mySerializationWriter.WriteString(IndexName);
            mySerializationWriter.WriteString(IndexEdition);
            IndexKeyDefinition.Serialize(ref mySerializationWriter);
            IndexRelatedTypeUUID.Serialize(ref mySerializationWriter);
        }

        public override void Deserialize(ref Lib.NewFastSerializer.SerializationReader mySerializationReader)
        {
            _KeyCount = mySerializationReader.ReadUInt64();
            IndexName = mySerializationReader.ReadString();
            IndexEdition = mySerializationReader.ReadString();
            IndexKeyDefinition = new IndexKeyDefinition();
            IndexKeyDefinition.Deserialize(ref mySerializationReader);
            IndexRelatedTypeUUID = new TypeUUID(ref mySerializationReader);
        }

        #endregion

        public override AFSObject Clone()
        {
            return new UUIDIndex();
        }

        public override ulong GetEstimatedSize()
        {
            return EstimatedSizeConstants.AFSObjectOntologyObject + EstimatedSizeConstants.UInt64 + EstimatedSizeConstants.Boolean;
        }


        public override Exceptional Initialize(DBContext myDBContext, string indexName, IndexKeyDefinition idxKey, GraphDBType correspondingType, string indexEdition = DBConstants.DEFAULTINDEX)
        {
        
            IndexName = indexName;
            IndexEdition = indexEdition;
            IndexKeyDefinition = idxKey;
            IndexRelatedTypeUUID = correspondingType.UUID;

            if (indexEdition == null)
            {
                IndexEdition = DBConstants.DEFAULTINDEX;
            }
            else
            {
                IndexEdition = indexEdition;
            }

            #region Workaround for current IndexOperation of InOperator - just follow the IsListOfBaseObjectsIndex property

            IsListOfBaseObjectsIndex = false;

            #endregion

            FileSystemLocation = (correspondingType.ObjectLocation + "Indices") + (IndexName + "#" + IndexEdition);

            _KeyCount = 0;

            return Exceptional.OK;
        }
    }
}
