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
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.Indices;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement.PandoraTypes;
using System.Collections.Generic;

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
        /// The ObjectLocation of the IndexObject within a pandora file system
        /// </summary>
        public virtual ObjectLocation FileSystemLocation { get; protected set; }
        
        #endregion

        #region IndexName

        /// <summary>
        /// The name of the index
        /// </summary>
        public virtual TypeUUID IndexRelatedTypeUUID { get; protected set; }

        #endregion

        #endregion

        #region IAttributeIndex methods

        public abstract Exceptional Update(DBObjectStream myDBObject, GraphDBType myTypeOfDBObject, DBContext dbContext);

        public abstract Exceptional Insert(DBObjectStream myDBObject, GraphDBType myTypeOfDBobject, DBContext dbContext);

        public abstract Exceptional Insert(DBObjectStream myDBObject, IndexSetStrategy myIndexSetStrategy, GraphDBType myTypeOfDBObject, DBContext dbContext);

        public abstract bool Contains(DBObjectStream myDBObject, GraphDBType myTypeOfDBObject, DBContext dbContext);

        public abstract bool Contains(IndexKey myIndeyKey, GraphDBType myTypeOfDBObject, DBContext dbContext);

        public abstract Exceptional Remove(DBObjectStream myDBObject, GraphDBType myTypeOfDBObjects, DBContext dbContext);

        public abstract Exceptional Clear(DBIndexManager indexManager);

        public abstract IEnumerable<IndexKey> GetKeys(GraphDBType myTypeOfDBObject, DBContext dbContext);

        public abstract IEnumerable<IEnumerable<ObjectUUID>> GetAllValues(GraphDBType myTypeOfDBObject, DBContext dbContext);

        public abstract IEnumerable<ObjectUUID> GetValues(IndexKey myIndeyKey, GraphDBType myTypeOfDBObject, DBContext dbContext);

        public abstract IEnumerable<KeyValuePair<IndexKey, HashSet<ObjectUUID>>> GetKeyValues(GraphDBType myTypeOfDBObject, DBContext dbContext);

        public abstract UInt64 GetValueCount(GraphDBType myTypeOfDBObject, DBContext dbContext);

        public abstract UInt64 GetKeyCount(GraphDBType myTypeOfDBObject, DBContext dbContext);

        public abstract IEnumerable<ObjectUUID> InRange(IndexKey fromIndexKey, IndexKey toIndexKey, bool myOrEqualFromKey, bool myOrEqualToKey, GraphDBType myTypeOfDBObject, DBContext dbContext);

        #endregion
    }
}
