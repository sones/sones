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


#endregion

namespace sones.GraphDB.Indices
{
    /// <summary>
    /// A abstract class for AttributeIndex and UUIDIdx
    /// </summary>
    public abstract class AAttributeIndex : IAttributeIndex
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
        public virtual IndexKeyDefinition IndexKeyDefinition { get; protected set; }

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

        #region IsUniqueIndex

        /// <summary>
        /// Determines whether this index is an unique index
        /// </summary>
        public virtual Boolean IsUniqueIndex { get; protected set; }

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
        public virtual String IndexType { get; protected set; }
        
        #endregion

        #region FileSystemLocation

        /// <summary>
        /// The ObjectLocation of the IndexObject within a Graph file system
        /// </summary>
        public virtual ObjectLocation FileSystemLocation { get; protected set; }
        
        #endregion

        #region IndexName

        /// <summary>
        /// The name of the index
        /// </summary>
        public virtual TypeUUID IndexRelatedTypeUUID { get; protected set; }

        #endregion

        #region Shards

        /// <summary>
        /// The count of attribute idnex shards
        /// </summary>
        public virtual UInt16 AttributeIdxShards { get; set; }

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
        public abstract Exceptional ClearAndRemoveFromDisc(DBIndexManager myIndexManager);

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
        public abstract UInt64 GetValueCount();

        /// <summary>
        /// Return the number of keys for this index
        /// </summary>
        /// <returns>The number of keys</returns>
        public abstract UInt64 GetKeyCount();

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

        #endregion
    }
}
