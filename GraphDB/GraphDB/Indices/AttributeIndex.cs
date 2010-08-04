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


/* <id name="GraphDB – AttributeIndex" />
 * <copyright file="AttributeIndex.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Achim Friedland</developer>
 * <developer>Henning Rauch</developer>
 * <summary>This datastructure contains information concerning a single attribute index</summary>
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

#endregion

namespace sones.GraphDB.Indices
{
    /// <summary>
    /// This datastructure contains information concerning a single attribute index
    /// </summary>
    public class AttributeIndex : AAttributeIndex
    {
        #region properties

        #region IndexReference

        private IVersionedIndexObject<IndexKey, ObjectUUID> _indexReference = null;
        
        /// <summary>
        /// A reference to this index after it was loaded into the memory
        /// or connected by a proxy class
        /// </summary>
        private Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>> GetIndexReference(DBIndexManager indexManager)
        {

            if (_indexReference == null)
            {
                if (!indexManager.HasIndex(IndexType))
                {
                    // the index type does not exist anymore - return null or throw exception
                    return new Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>>(new GraphDBError("Index is away!"));
                }

                var emptyIdx = indexManager.GetIndex(IndexType);
                if (!emptyIdx.Success)
                {
                    return emptyIdx;
                }

                var indexExceptional = indexManager.LoadOrCreateDBIndex(FileSystemLocation, emptyIdx.Value);

                if (indexExceptional.Failed)
                {
                    return new Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>>(indexExceptional);
                }

                _indexReference = indexExceptional.Value;
            }

            return new Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>>(_indexReference);
        }

        #endregion

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
        public AttributeIndex(String myIndexName, String myIndexEdition, List<AttributeUUID> myAttributes, GraphDBType correspondingType, String myIndexType = null)
            :this(myIndexName, new IndexKeyDefinition(myAttributes), correspondingType, myIndexType, myIndexEdition)
        { }

        public AttributeIndex(string indexName, IndexKeyDefinition idxKey, GraphDBType correspondingType, string indexType = null, string indexEdition = DBConstants.DEFAULTINDEX)
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
                if (typeAttr != null && (typeAttr.EdgeType is AListBaseEdgeType || typeAttr.EdgeType is ASetBaseEdgeType))
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
                                
                                foreach (var aBaseObject in ((AListBaseEdgeType)myDBObject.GetAttribute(aIndexAttributeUUID, myTypeOfDBObject, dbContext)).GetAll())
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

            #region Get index reference

            var idxRef = GetIndexReference(dbContext.DBIndexManager);
            if (!idxRef.Success)
            {
                return new Exceptional(idxRef);
            }
            var idxRefVal = idxRef.Value;

            #endregion

            if (idxRefVal != null)
            {

                #region remove

                HashSet<IndexKey> toBeRemovedIdxKeys = new HashSet<IndexKey>();

                foreach (var aKeyValue in idxRefVal.GetIDictionary())
                {
                    aKeyValue.Value.Remove(myDBObject.ObjectUUID);
                    if (aKeyValue.Value.Count == 0)
                    {
                        toBeRemovedIdxKeys.Add(aKeyValue.Key);
                    }
                }

                foreach (var aToBeDeletedIndexKey in toBeRemovedIdxKeys)
                {
                    idxRefVal.Remove(aToBeDeletedIndexKey);
                }

                #endregion

                #region insert

                if (myDBObject.HasAtLeastOneAttribute(this.IndexKeyDefinition.IndexKeyAttributeUUIDs, myTypeOfDBObject, dbContext.SessionSettings))
                {
                    //insert
                    foreach (var aIndexKey in this.GetIndexkeysFromDBObject(myDBObject, myTypeOfDBObject, dbContext))
                    {
                        idxRefVal.Set(aIndexKey, myDBObject.ObjectUUID, IndexSetStrategy.MERGE);
                    }
                }

                #endregion

            }
            else
            {
                return new Exceptional(new Error_InvalidIndexReference(IndexName, IndexEdition));
            }

            return Exceptional.OK;

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

            #region Get index reference

            var idxRef = GetIndexReference(dbContext.DBIndexManager);
            if (!idxRef.Success)
            {
                return new Exceptional(idxRef);
            }
            var idxRefVal = idxRef.Value;

            #endregion

            if (idxRefVal != null)
            {
                foreach (var aIndexKex in GetIndexkeysFromDBObject(myDBObject, myTypeOfDBObject, dbContext))
                {

                    #region Check for uniqueness - TODO: remove me as soon as we have a unique indexObject implementation

                    if (IsUniqueAttributeIndex)
                    {
                        if (idxRefVal.ContainsKey(aIndexKex))
                        {
                            return new Exceptional(new Error_UniqueConstrainViolation(myTypeOfDBObject.Name, IndexName));
                        }
                    }

                    #endregion


                    idxRefVal.Set(aIndexKex, myDBObject.ObjectUUID, myIndexSetStrategy);
                }
            }
            else
            {
                return new Exceptional(new Error_InvalidIndexReference(IndexName, IndexEdition));
            }

            return Exceptional.OK;

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

            #region Get index reference

            var idxRef = GetIndexReference(dbContext.DBIndexManager);
            if (idxRef.Failed)
            {
                throw new GraphDBException(new Error_CouldNotGetIndexReference(idxRef.Errors, IndexName, IndexEdition));
            }
            var idxRefVal = idxRef.Value;

            #endregion

            if (idxRefVal != null)
            {
                foreach (var aIndexKex in GetIndexkeysFromDBObject(myDBObject, myTypeOfDBObject, dbContext))
                {
                    if (idxRefVal.Contains(aIndexKex, myDBObject.ObjectUUID))
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                throw new GraphDBException(new Error_InvalidIndexReference(IndexName, IndexEdition));
            }

        }

        public override Boolean Contains(IndexKey myIndeyKey, GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            #region Get index reference

            var idxRef = GetIndexReference(dbContext.DBIndexManager);
            if (idxRef.Failed)
            {
                throw new GraphDBException(new Error_CouldNotGetIndexReference(idxRef.Errors, IndexName, IndexEdition));
            }
            var idxRefVal = idxRef.Value;

            #endregion

            if (idxRefVal != null)
            {
                return idxRefVal.ContainsKey(myIndeyKey);
            }
            else
            {
                throw new GraphDBException(new Error_InvalidIndexReference(IndexName, IndexEdition));
            }
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

            #region Get index reference

            var idxRef = GetIndexReference(dbContext.DBIndexManager);
            if (!idxRef.Success)
            {
                return new Exceptional(idxRef);
            }
            var idxRefVal = idxRef.Value;

            #endregion
            if (idxRefVal != null)
            {
                foreach (var aIndexKex in GetIndexkeysFromDBObject(myDBObject, myTypeOfDBObjects, dbContext))
                {
                    idxRefVal.Remove(aIndexKex, myDBObject.ObjectUUID);
                }
            }
            else
            {
                return new Exceptional(new Error_InvalidIndexReference(IndexName, IndexEdition));
            }

            return Exceptional.OK;

        }

        #endregion

        #region Clear

        public override Exceptional Clear(DBIndexManager indexManager)
        {
            _indexReference = null;
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

        public override IEnumerable<IndexKey> GetKeys(GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            #region Get index reference

            var idxRef = GetIndexReference(dbContext.DBIndexManager);
            if (idxRef.Failed)
            {
                throw new GraphDBException(new Error_CouldNotGetIndexReference(idxRef.Errors, IndexName, IndexEdition));
            }
            var idxRefVal = idxRef.Value;

            #endregion

            if (idxRefVal != null)
            {
                return idxRefVal.Keys();
            }
            else
            {
                throw new GraphDBException(new Error_InvalidIndexReference(IndexName, IndexEdition));
            }
        }

        #endregion

        #region GetValues

        public override IEnumerable<ObjectUUID> GetValues(IndexKey myIndeyKey, GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            #region Get index reference

            var idxRef = GetIndexReference(dbContext.DBIndexManager);
            if (idxRef.Failed)
            {
                throw new GraphDBException(new Error_CouldNotGetIndexReference(idxRef.Errors, IndexName, IndexEdition));
            }
            var idxRefVal = idxRef.Value;

            #endregion

            if (idxRefVal != null)
            {
                return idxRef.Value[myIndeyKey];
            }
            else
            {
                throw new GraphDBException(new Error_InvalidIndexReference(IndexName, IndexEdition));
            }
        }

        #endregion

        #region GetAllValues

        public override IEnumerable<IEnumerable<ObjectUUID>> GetAllValues(GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            #region Get index reference

            var idxRef = GetIndexReference(dbContext.DBIndexManager);
            if (idxRef.Failed)
            {
                throw new GraphDBException(new Error_CouldNotGetIndexReference(idxRef.Errors, IndexName, IndexEdition));
            }
            var idxRefVal = idxRef.Value;

            #endregion

            if (idxRefVal != null)
            {
                foreach (var aValue in idxRefVal.Values())
                {
                    yield return aValue;
                }

                yield break;
            }
            else
            {
                throw new GraphDBException(new Error_InvalidIndexReference(IndexName, IndexEdition));
            }
        }

        #endregion

        #region GetKeyValues

        public override IEnumerable<KeyValuePair<IndexKey, HashSet<ObjectUUID>>> GetKeyValues(GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            #region Get index reference

            var idxRef = GetIndexReference(dbContext.DBIndexManager);
            if (idxRef.Failed)
            {
                throw new GraphDBException(new Error_CouldNotGetIndexReference(idxRef.Errors, IndexName, IndexEdition));
            }
            var idxRefVal = idxRef.Value;

            #endregion

            if (idxRefVal != null)
            {
                foreach (var aKV in idxRef.Value)
                {
                    yield return aKV;
                }

                yield break;
            }
            else
            {
                throw new GraphDBException(new Error_InvalidIndexReference(IndexName, IndexEdition));
            }
        }

        #endregion

        #region GetValueCount

        public override UInt64 GetValueCount(GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            #region Get index reference

            var idxRef = GetIndexReference(dbContext.DBIndexManager);
            if (idxRef.Failed)
            {
                throw new GraphDBException(new Error_CouldNotGetIndexReference(idxRef.Errors, IndexName, IndexEdition));
            }
            var idxRefVal = idxRef.Value;

            #endregion

            if (idxRefVal != null)
            {
                return idxRefVal.ValueCount();
            }
            else
            {
                throw new GraphDBException(new Error_InvalidIndexReference(IndexName, IndexEdition));
            }
        }

        #endregion

        #region GetKeyCount

        public override UInt64 GetKeyCount(GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            #region Get index reference

            var idxRef = GetIndexReference(dbContext.DBIndexManager);
            if (idxRef.Failed)
            {
                throw new GraphDBException(new Error_CouldNotGetIndexReference(idxRef.Errors, IndexName, IndexEdition));
            }
            var idxRefVal = idxRef.Value;

            #endregion

            if (idxRefVal != null)
            {
                return idxRefVal.KeyCount();
            }
            else
            {
                throw new GraphDBException(new Error_InvalidIndexReference(IndexName, IndexEdition));
            }
        }

        #endregion

        #region InRange

        public override IEnumerable<ObjectUUID> InRange(IndexKey fromIndexKey, IndexKey toIndexKey, bool myOrEqualFromKey, bool myOrEqualToKey, GraphDBType myTypeOfDBObject, DBContext dbContext)
        {
            #region Get index reference

            var idxRef = GetIndexReference(dbContext.DBIndexManager);
            if (idxRef.Failed)
            {
                throw new GraphDBException(new Error_CouldNotGetIndexReference(idxRef.Errors, IndexName, IndexEdition));
            }
            var idxRefVal = idxRef.Value;

            #endregion

            if (idxRefVal != null)
            {
                return idxRefVal.InRange(fromIndexKey, toIndexKey, myOrEqualFromKey, myOrEqualToKey);
            }
            else
            {
                throw new GraphDBException(new Error_InvalidIndexReference(IndexName, IndexEdition));
            }
        }

        #endregion

        #endregion
    }
}
