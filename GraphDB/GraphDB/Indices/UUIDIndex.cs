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
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.Lib.DataStructures;
using sones.Lib.DataStructures.Indices;
using sones.Lib.ErrorHandling;
using System.Linq;
using sones.GraphFS.InternalObjects;
using sones.Lib;

#endregion

namespace sones.GraphDB.Indices
{
    /// <summary>
    /// This index is a wrapper for the GraphFS directory object
    /// </summary>
    public class UUIDIndex : AAttributeIndex
    {
        #region Constructor

        /// <summary>
        /// Creates a new AttributeIndex object
        /// </summary>
        /// <param name="myIndexName">The user-defined name of this index</param>
        /// <param name="myIndexEdition">You may have different versions of an attribute index, e.g. HashMap, BTree to speed up different database operations</param>
        /// <param name="myAttributes">The list of attributes that is needed for the creation of a IndexKeyDefinition</param>
        /// <param name="myIndexType">The IndexType e.g. HashMap, BTree of this AttributeIndex</param>
        /// <param name="correspondingType">The corresponding type of this index, used to get the file system location</param>
        public UUIDIndex(String myIndexName, String myIndexEdition, List<AttributeUUID> myAttributes, GraphDBType correspondingType, String myIndexType = null)
            :this(myIndexName, new IndexKeyDefinition(myAttributes), correspondingType, myIndexType, myIndexEdition)
        { }

        public UUIDIndex(string indexName, IndexKeyDefinition idxKey, GraphDBType correspondingType, string indexType = null, string indexEdition = DBConstants.DEFAULTINDEX)
        {
            IndexName          = indexName;
            IndexEdition       = indexEdition;
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

            if (String.IsNullOrEmpty(indexType))
            {
                IndexType = "UUIDIndex";
            }
            else
            {
                IndexType = indexType;
            }

            #region Workaround for current IndexOperation of InOperator - just follow the IsListOfBaseObjectsIndex property

            IsListOfBaseObjectsIndex = false;

            #endregion

            FileSystemLocation = (correspondingType.ObjectLocation + "Indices") + (IndexName + "#" + IndexEdition);
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
            return new Exceptional(new Error_InvalidIndexOperation(IndexName, "Update"));
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
            return new Exceptional(new Error_InvalidIndexOperation(IndexName, "Insert"));
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
        public override Boolean Contains(DBObjectStream myDBObject, GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            throw new NotImplementedException();
        }

        public override Boolean Contains(IndexKey myIndeyKey, GraphDBType myTypeOfDBObject, DBContext dbContext)
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
                    return GetDirectoryObject(myTypeOfDBObject, dbContext).ObjectExists(uuid.ToString());
                }
            }
            else
            {
                throw new GraphDBException(new Error_InvalidIndexOperation(IndexName));
            }
        }

        public Boolean Contains(ObjectUUID myUUID, GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            return GetDirectoryObject(myTypeOfDBObject, dbContext).ObjectExists(myUUID.ToString());
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
            return new Exceptional(new Error_InvalidIndexOperation(IndexName, "Remove"));            
        }

        #endregion

        #region Clear

        public override Exceptional Clear(DBIndexManager indexManager)
        {
            return new Exceptional(new Error_InvalidIndexOperation(IndexName, "Clear"));
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
            if (Contains(myIndeyKey, myTypeOfDBObject, dbContext))
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

        public override UInt64 GetValueCount(GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            //thats the characteristc of the UUIDIDx... count(keys) == count(values)
            return GetKeyCount(myTypeOfDBObject, dbContext);
        }

        #endregion

        #region GetKeyCount

        public override UInt64 GetKeyCount(GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            return GetDirectoryObject(myTypeOfDBObject, dbContext).GetDirectoryListing(null, null, null, new List<String>(new String[] { DBConstants.DBOBJECTSTREAM }), null).ULongCount();
        }

        #endregion

        #region GetAllUUIDs

        public IEnumerable<ObjectUUID> GetAllUUIDs(GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            return GetDirectoryObject(myTypeOfDBObject, dbContext).GetDirectoryListing(null, null, null, new List<String>(new String[] { DBConstants.DBOBJECTSTREAM }), null).Select(item => new ObjectUUID(item));
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

                    if (Contains(fromIndexKey, myTypeOfDBObject, dbContext))
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

        private DirectoryObject GetDirectoryObject(GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            var directoryException = dbContext.DBTypeManager.GetObjectsDirectory(myTypeOfDBObject);

            if (directoryException.Failed)
            {
                throw new GraphDBException(new Error_CouldNotGetIndexReference(directoryException.Errors, IndexName, IndexEdition));
            }

            return directoryException.Value;
        }

        #endregion
    }
}
