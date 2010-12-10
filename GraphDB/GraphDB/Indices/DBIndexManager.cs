/* <id name="GraphdbDB – DBIndexManager" />
 * <copyright file="DBIndexManager.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Result;
using sones.GraphDB.TypeManagement;

using sones.GraphFS.DataStructures;
using sones.GraphFS.Errors;
using sones.GraphFS.Exceptions;
using sones.GraphFS.Objects;
using sones.GraphFS.Session;
using sones.Lib;
using sones.Lib.DataStructures.Indices;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.NewAPI;
using System.Diagnostics;
using sones.GraphDB.ObjectManagement;

#endregion

namespace sones.GraphDB.Indices
{
   
    public class DBIndexManager
    {

        #region data

        private IGraphFSSession _IGraphFSSession;
        private DBContext _DBContext;

        #endregion

        #region Constructor

        public DBIndexManager(IGraphFSSession myIGraphFSSession, DBContext dbContext)
        {
            _IGraphFSSession = myIGraphFSSession;
            _DBContext = dbContext;
        }

        #endregion

        #region public methods

        public string GetUniqueIndexName(List<AttributeUUID> UniqueIDs, GraphDBType aType)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(DBConstants.IndexKeyPrefix + DBConstants.IndexKeySeperator + DBConstants.UNIQUEATTRIBUTESINDEX);

            foreach (var AttrID in UniqueIDs)
            {
                sb.Append("_" + aType.GetTypeAttributeByUUID(AttrID).Name);
            }

            return sb.ToString();
        } 


        public Exceptional<Boolean> CheckUniqueConstraint(GraphDBType myGraphType, IEnumerable<GraphDBType> myParentTypes, Dictionary<AttributeUUID, IObject> toBeCheckedForUniqueConstraint)
        {
            var UniqueAttributes = myGraphType.GetAllUniqueAttributes(true, _DBContext.DBTypeManager);

            if (!UniqueAttributes.IsNullOrEmpty())
            {
                var dbObjectAttributes = (from aAttribute in toBeCheckedForUniqueConstraint where UniqueAttributes.Contains(aAttribute.Key) select aAttribute);

                if (dbObjectAttributes.Count() != 0)
                {
                    AAttributeIndex AttrIndex = null;

                    foreach (var PType in myParentTypes)
                    {
                        AttrIndex = PType.FindUniqueIndex();

                        if (AttrIndex != null)
                        {
                            var toBeCheckedIdxKey = GenerateIndexKeyForUniqueConstraint(toBeCheckedForUniqueConstraint, AttrIndex.IndexKeyDefinition, myGraphType);

                            if (AttrIndex.Contains(toBeCheckedIdxKey, PType, _DBContext))
                            {
                                return new Exceptional<Boolean>(new Error_UniqueConstrainViolation(PType.Name, AttrIndex.IndexName));
                            }
                        }
                    }
                }
            }

            return new Exceptional<Boolean>(true);
        }

        public Exceptional<ResultType> RebuildIndex(String myIndexName, String myIndexEdition, GraphDBType myDBTypeStream, IndexSetStrategy myIndexSetStrategy)
        {

            var objectLocation = new ObjectLocation(myDBTypeStream.ObjectLocation, DBConstants.DBObjectsLocation);
            IEnumerable<String> allDBOLocations = null;

            try
            {
                allDBOLocations = _DBContext.DBObjectManager.GetAllStreamsRecursive(objectLocation, DBConstants.DBOBJECTSTREAM);
            }
            catch (Exception e)
            {
                return new Exceptional<ResultType>(new Error_RebuildIndexFailed(myIndexName, myIndexEdition, e.Message));
                
            }

            try
            {

                var index = myDBTypeStream.GetAttributeIndex(myIndexName, myIndexEdition);

                if (index.Failed())
                {
                    return new Exceptional<ResultType>(index);
                }

                index.Value.ClearAndRemoveFromDisc(this);
                
                    foreach (var loc in allDBOLocations)
                    {
                        var dbo = _DBContext.DBObjectManager.LoadDBObject(new ObjectLocation(myDBTypeStream.ObjectLocation, DBConstants.DBObjectsLocation, loc));

                        if (dbo.Failed())
                        {
                            return new Exceptional<ResultType>(dbo);
                        }

                        if (!dbo.Value.ObjectLocation.Contains(loc))
                        {
                            //NLOG: temporarily commented
                            ////_Logger.Error("Could not found the correct DBObject for Location " + loc + " the ObjectUUID is now " + dbo.Value.ObjectUUID.ToString());
                        }
                        else
                        {
                            if (dbo.Value.HasAtLeastOneAttribute(index.Value.IndexKeyDefinition.IndexKeyAttributeUUIDs, myDBTypeStream, null))
                            {
                                var insertResult = index.Value.Insert(dbo.Value, myIndexSetStrategy, myDBTypeStream, _DBContext);
                                if (!insertResult.Success())
                                {
                                    return new Exceptional<ResultType>(insertResult);
                                }
                            }
                        }
                    
                }
            }
            catch (GraphDBException pe)
            {
                var _Exceptional = new Exceptional<ResultType>();
                foreach (var _ex in pe.GraphDBErrors)
                    _Exceptional.PushIError(_ex);
                return _Exceptional;
            }
            catch (GraphFSException_IndexKeyAlreadyExist)
            {
                //NLOG: temporarily commented
                ////_Logger.ErrorException("GraphFSException_IndexKeyAlreadyExist", ikae);
                return new Exceptional<ResultType>(new Error_UniqueConstrainViolation(myDBTypeStream.Name, myIndexName));
            }
            catch
            {
                return new Exceptional<ResultType>(new Error_IndexDoesNotExist(myIndexName, myIndexEdition));
            }

            return new Exceptional<ResultType>(ResultType.Successful);

        }

        public Exceptional<Boolean> RebuildIndices(GraphDBType aUserDefinedType)
        {
            return RebuildIndices(new List<GraphDBType>() { aUserDefinedType });
        }

        public Exceptional<Boolean> RebuildIndices(IEnumerable<GraphDBType> myUserDefinedTypes)
        {

            #region Remove old attribute indices

            foreach (var _UserDefinedType in myUserDefinedTypes)
            {
                foreach (var _AttributeIndex in _UserDefinedType.GetAllAttributeIndices(includeUUIDIndices: false))
                {
                    // Clears the index and removes it from the file system!
                    _AttributeIndex.ClearAndRemoveFromDisc(this);
                }
            }
            
            #endregion


            foreach (var _UserDefinedType in myUserDefinedTypes)
            {

                var _DBObjectsLocation = new ObjectLocation(_UserDefinedType.ObjectLocation, DBConstants.DBObjectsLocation);
                IEnumerable<String> allDBOLocations = null;

                // Get all DBObjects from DirectoryListing
                try
                {
                    //this is the fasted way to get all UUIDs of a certain vertex type
                    allDBOLocations = _IGraphFSSession.GetDirectoryListing(_DBObjectsLocation).Value;
                }
                catch (Exception e)
                {
                    return new Exceptional<Boolean>(new Error_RebuildIndexFailed(DBConstants.UUIDIdxName, DBConstants.DEFAULTINDEX, e.Message));

                }

                var UUIDAttributeUUID = _DBContext.DBTypeManager.GetUUIDTypeAttribute().UUID;

                var UUIDIdxIndexKey = new IndexKeyDefinition(new List<AttributeUUID>() { UUIDAttributeUUID });

                foreach (var _DBObjectLocation in allDBOLocations)
                {
                    //the "allDBOLocations" are elements of a FS-directory-listing... so there might be default directory entries
                    if (!_DBObjectLocation.StartsWith(DBConstants.DefaultDirectoryEntryPrefix))
                    {
                        var _DBObjectExceptional = _DBContext.DBObjectManager.LoadDBObject(new ObjectLocation(_DBObjectsLocation, _DBObjectLocation));
                        if (_DBObjectExceptional.Failed())
                        {
                            return new Exceptional<Boolean>(_DBObjectExceptional);
                        }

                        //rebuild everything but the UUIDidx
                        foreach (var _KeyValuePair in _UserDefinedType.AttributeIndices.Where(aIDX => aIDX.Key != UUIDIdxIndexKey))
                        {
                            foreach (var _AttributeIndex in _KeyValuePair.Value.Values)
                            {
                                _AttributeIndex.Insert(_DBObjectExceptional.Value, _UserDefinedType, _DBContext);
                            }
                        }
                    }
                }
            }

            return new Exceptional<bool>(true);

        }

        #endregion

        #region private Helper methods

        private IndexKey GenerateIndexKeyForUniqueConstraint(Dictionary<AttributeUUID, IObject> toBeCheckedForUniqueConstraint, IndexKeyDefinition myIndexKeyDefinition, GraphDBType myType)
        {
            var payload = new Dictionary<AttributeUUID, ADBBaseObject>();
            TypeAttribute currentAttribute;

            foreach (var aUnique in myIndexKeyDefinition.IndexKeyAttributeUUIDs)
            {
                currentAttribute = myType.GetTypeAttributeByUUID(aUnique);

                if (!currentAttribute.GetDBType(_DBContext.DBTypeManager).IsUserDefined)
                {
                    #region base attribute

                    if (toBeCheckedForUniqueConstraint.ContainsKey(aUnique))
                    {
                        switch (currentAttribute.KindOfType)
                        {
                            #region List/Set

                            case KindsOfType.ListOfNoneReferences:
                            case KindsOfType.SetOfNoneReferences:

                                throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), "Unique idx on list of base attributes is not implemented."));

                            #endregion

                            #region single/special

                            case KindsOfType.SpecialAttribute:
                            case KindsOfType.SingleNoneReference:
                            case KindsOfType.SingleReference:

                                payload.Add(aUnique, (ADBBaseObject)toBeCheckedForUniqueConstraint[aUnique]);

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
                        //create default adbbaseobject

                        var defaultADBBAseObject = GraphDBTypeMapper.GetADBBaseObjectFromUUID(currentAttribute.DBTypeUUID);
                        defaultADBBAseObject.SetValue(DBObjectInitializeType.Default);

                        payload.Add(aUnique, defaultADBBAseObject);
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

            return new IndexKey(payload, myIndexKeyDefinition);
        }

        #endregion

        #region RemoveDBIndex()

        internal Exceptional RemoveDBIndex(ObjectLocation myObjectLocation)
        {
            return _IGraphFSSession.RemoveDirectoryObject(myObjectLocation, true);
        }
        #endregion

        #region LoadOrCreateDBIndex()

        internal Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>> LoadOrCreateDBIndex(ObjectLocation myShardObjectLocation, IVersionedIndexObject<IndexKey, ObjectUUID> myIIndexObject, AAttributeIndex correspondingIDX)
        {

            if (!(myIIndexObject is AFSObject))
            {
                return new Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>>(new Error_IndexIsNotPersistent(myIIndexObject));
            }

            var result = _IGraphFSSession.GetOrCreateFSObject<AFSObject>(myShardObjectLocation, DBConstants.DBINDEXSTREAM, () => myIIndexObject as AFSObject);
            if (!result.Success())
            {
                return new Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>>(result);
            }

            else
            {

                if (result.Value.isNew)
                {
                    //TODO: might be a performance bug
                    if (!_IGraphFSSession.ObjectExists(correspondingIDX.FileSystemLocation).Value)
                    {
                        result.PushIExceptional(_IGraphFSSession.CreateDirectoryObject(correspondingIDX.FileSystemLocation));
                    }

                    // Uncomment as soon as index is serializeable
                    result.PushIExceptional(_IGraphFSSession.StoreFSObject(result.Value, false));
                    //result.AddErrorsAndWarnings(result.Value.Save());
                    //ToDo: Fehler beim Speichern werden im Weiterm ignoriert statt darauf reagiert!
                }

                return new Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>>(result.Value as IVersionedIndexObject<IndexKey, ObjectUUID>);

            }

        }

        #endregion

        #region HasIndex(indexName)

        /// <summary>
        /// Returns true, if there was a index with the specified name <paramref name="indexName"/>
        /// </summary>
        /// <param name="indexName">The index name.</param>
        /// <returns>True if the index exist.</returns>
        public Boolean HasIndex(String indexName)
        {

            return _DBContext.DBPluginManager.HasIndex(indexName);

        }

        #endregion

        #region GetIndex(indexName)

        /// <summary>
        /// Returns the index with the specified name <paramref name="indexTypeName"/>
        /// </summary>
        /// <param name="indexTypeName">The index name.</param>
        /// <returns>A new instance of the index object.</returns>
        public Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>> GetIndex(String indexTypeName)
        {

            return _DBContext.DBPluginManager.GetIndex(indexTypeName);

        }

        #endregion

        #region Create Index

        public Exceptional<IEnumerable<Vertex>> CreateIndex(DBContext myDBContext, string myDBType, string myIndexName, string myIndexEdition, string myIndexType, List<IndexAttributeDefinition> myAttributeList)
        {

            IEnumerable<Vertex> resultOutput = null;

            var dbObjectType = myDBContext.DBTypeManager.GetTypeByName(myDBType);

            if (dbObjectType == null)
            {
                return new Exceptional<IEnumerable<Vertex>>(new Error_TypeDoesNotExist(myDBType));
            }

            #region Get IndexAttributes

            var indexAttributes = new List<AttributeUUID>();

            foreach (var createIndexAttributeNode in myAttributeList)
            {

                var validateResult = createIndexAttributeNode.IndexAttribute.Validate(myDBContext, false, dbObjectType);

                if (validateResult.Failed())
                {
                    return new Exceptional<IEnumerable<Vertex>>(validateResult);
                }
                var attrName = createIndexAttributeNode.IndexAttribute.LastAttribute.Name;

                var validAttrExcept = myDBContext.DBTypeManager.AreValidAttributes(dbObjectType, attrName);

                if (validAttrExcept.Failed())
                    throw new GraphDBException(validAttrExcept.IErrors);

                if (!validAttrExcept.Value)
                    throw new GraphDBException(new Error_AttributeIsNotDefined(dbObjectType.Name, attrName));

                indexAttributes.Add(createIndexAttributeNode.IndexAttribute.LastAttribute.UUID);

            }

            #endregion


            if (String.IsNullOrEmpty(myIndexName))
            {
                myIndexName = myAttributeList.Aggregate(new StringBuilder(DBConstants.IndexKeyPrefix),
                                                        (result, elem) => {
                                                            result.Append(String.Concat(DBConstants.IndexKeySeperator, elem.IndexAttribute.LastAttribute.Name));
                                                            return result;
                                                        }
                                                       ).ToString();
            }


            #region checking for reference attributes

            TypeAttribute aIdxAttribute;

            foreach (var aAttributeUUID in indexAttributes)
            {
                aIdxAttribute = dbObjectType.GetTypeAttributeByUUID(aAttributeUUID);

                if (aIdxAttribute.GetDBType(myDBContext.DBTypeManager).IsUserDefined)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_NotImplemented(new StackTrace(true), String.Format("Currently it is not implemented to create an index on reference attributes like {0}", aIdxAttribute.Name)));
                }
            }

            #endregion

            foreach (var item in myDBContext.DBTypeManager.GetAllSubtypes(dbObjectType))
            {

                #region Create the index

                var createdIDx = item.CreateAttributeIndex(myDBContext, myIndexName, indexAttributes, myIndexEdition, myIndexType);

                if (createdIDx.Failed())
                {
                    return new Exceptional<IEnumerable<Vertex>>(createdIDx);
                }
                else
                {
                    #region prepare readouts

                    var readOut = GenerateCreateIndexResult(myDBType, myAttributeList, createdIDx.Value);

                    resultOutput = new List<Vertex>(){ readOut };

                    #endregion
                }

                #endregion

                #region (Re)build index

                var rebuildResult = myDBContext.DBIndexManager.RebuildIndex(createdIDx.Value.IndexName, createdIDx.Value.IndexEdition, item, IndexSetStrategy.MERGE);

                if (rebuildResult.Failed())
                {
                    return new Exceptional<IEnumerable<Vertex>>(rebuildResult);
                }


                #endregion

                #region Flush type

                var flushResult = myDBContext.DBTypeManager.FlushType(item);

                if (flushResult.Failed())
                {
                    return new Exceptional<IEnumerable<Vertex>>(flushResult);
                }

                #endregion

            }

            return new Exceptional<IEnumerable<Vertex>>(resultOutput);

        }

        private Vertex GenerateCreateIndexResult(String myDBType, List<IndexAttributeDefinition> myAttributeList, AAttributeIndex myAttributeIndex)
        {

            var payload = new Dictionary<String, Object>();

            payload.Add("NAME", myAttributeIndex.IndexName);
            payload.Add("EDITION", myAttributeIndex.IndexEdition);
            payload.Add("INDEXTYPE", myAttributeIndex.IndexType);
            payload.Add("ONTYPE", myDBType);

            var attributes = new List<Vertex>();

            foreach (var _Attribute in myAttributeList)
            {
                var payloadAttributes = new Dictionary<String, Object>();
                payloadAttributes.Add("ATTRIBUTE", _Attribute.IndexAttribute);
                attributes.Add(new Vertex(payloadAttributes));
            }

            payload.Add("ATTRIBUTES", new Edge(null, attributes) { EdgeTypeName = "ATTRIBUTE" });

            return new Vertex(payload);

        }

        #endregion

        #region ReOrganize index

        public Exceptional ReOrganizeIndexShards(AttributeIndex _AttributeIndex, GraphDBType relatedType, UInt16 oldShardValue, UInt16 newShardValue)
        {
            foreach (var aIndexShard in _AttributeIndex.GetAllIdxShards(_DBContext))
            {
                if (aIndexShard.Item2.Failed())
                {
                    return new Exceptional(aIndexShard.Item2);
                }

                Dictionary<int, KeyValuePair<IndexKey, HashSet<ObjectUUID>>> toBeMovedItems = new Dictionary<int, KeyValuePair<IndexKey, HashSet<ObjectUUID>>>();

                foreach (var aIndexShardPayload in aIndexShard.Item2.Value.ToList())
                {
                    var newIndexShardID = GetIndexShardID(aIndexShardPayload.Key, newShardValue);

                    if (newIndexShardID != aIndexShard.Item1)
                    {
                        toBeMovedItems.Add(newIndexShardID, aIndexShardPayload);

                        var removeException = _AttributeIndex.Remove(aIndexShardPayload.Key, aIndexShard.Item1, this);

                        if(removeException.Failed())
                        {
                            return removeException;
                        }
                    }
                }

                #region insert into other shards

                foreach (var item in toBeMovedItems)
                {
                    var insertException = _AttributeIndex.Insert(item.Value.Key, item.Value.Value, item.Key, this, relatedType);

                    if (insertException.Failed())
                    {
                        return insertException;
                    }

                }

                #endregion

            }

            return Exceptional.OK;
        }

        #endregion

        /// <summary>
        /// Get the shard id corresponding to an indexKey
        /// </summary>
        /// <param name="aIndexKey">An IndexKey</param>
        /// <returns>The shard id</returns>
        public int GetIndexShardID(IndexKey aIndexKey, UInt16 shardCount)
        {
            //stupid... might be solved by constant hashing or sth like that
            return Math.Abs(aIndexKey.GetHashCode()) % shardCount;
        }

    }
}
