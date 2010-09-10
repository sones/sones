/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

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
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Errors;
using sones.GraphFS.Exceptions;
using sones.GraphFS.Objects;
using sones.GraphFS.Session;
using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures.Indices;

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


        public Exceptional<Boolean> CheckUniqueConstraint(GraphDBType myPandoraType, IEnumerable<GraphDBType> myParentTypes, Dictionary<AttributeUUID, AObject> toBeCheckedForUniqueConstraint)
        {
            var UniqueAttributes = myPandoraType.GetAllUniqueAttributes(true, _DBContext.DBTypeManager);

            if (!UniqueAttributes.IsNullOrEmpty())
            {
                var dbObjectAttributes = (from aAttribute in toBeCheckedForUniqueConstraint where UniqueAttributes.Contains(aAttribute.Key) select aAttribute);

                if (dbObjectAttributes.Count() != 0)
                {
                    AttributeIndex AttrIndex = null;

                    foreach (var PType in myParentTypes)
                    {
                        AttrIndex = PType.FindUniqueIndex();

                        if (AttrIndex != null)
                        {
                            var toBeCheckedIdxKey = GenerateIndexKeyForUniqueConstraint(toBeCheckedForUniqueConstraint, AttrIndex.IndexKeyDefinition, myPandoraType);

                            var idxRef = AttrIndex.GetIndexReference(this);
                            if (!idxRef.Success)
                            {
                                return new Exceptional<bool>(idxRef);
                            }

                            if (idxRef.Value.ContainsKey(toBeCheckedIdxKey))
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
            var allDBOLocations = _IGraphFSSession.GetFilteredDirectoryListing(objectLocation, null, null, null, new List<String>(new String[] { DBConstants.DBOBJECTSTREAM }), null, null, null, null, null, null);

            if (allDBOLocations.Failed && allDBOLocations.Errors.First().GetType() != typeof(GraphFSError_ObjectLocatorNotFound))
                return new Exceptional<ResultType>(allDBOLocations);

            try
            {

                var index = myDBTypeStream.GetAttributeIndex(myIndexName, myIndexEdition);

                index.Clear(this);

                if (allDBOLocations.Value != null)
                {
                    foreach (var loc in allDBOLocations.Value)
                    {
                        var dbo = _DBContext.DBObjectManager.LoadDBObject(new ObjectLocation(myDBTypeStream.ObjectLocation, DBConstants.DBObjectsLocation, loc));

                        if (dbo.Failed)
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
                            if (dbo.Value.HasAtLeastOneAttribute(index.IndexKeyDefinition.IndexKeyAttributeUUIDs, myDBTypeStream, null))
                            {
                                var insertResult = index.Insert(dbo.Value, myIndexSetStrategy, myDBTypeStream, _DBContext);
                                if (!insertResult.Success)
                                {
                                    return new Exceptional<ResultType>(insertResult);
                                }
                            }
                        }
                    }
                }
            }
            catch (GraphDBException pe)
            {
                var _Exceptional = new Exceptional<ResultType>();
                foreach (var _ex in pe.GraphDBErrors)
                    _Exceptional.Push(_ex);
                return _Exceptional;
            }
            catch (PandoraFSException_IndexKeyAlreadyExist)
            {
                //NLOG: temporarily commented
                ////_Logger.ErrorException("PandoraFSException_IndexKeyAlreadyExist", ikae);
                return new Exceptional<ResultType>(new Error_UniqueConstrainViolation(myDBTypeStream.Name, myIndexName));
            }
            catch
            {
                return new Exceptional<ResultType>(new Error_IndexDoesNotExist(myIndexName, myIndexEdition));
            }

            return new Exceptional<ResultType>(ResultType.Successful);

        }

        public Exceptional<Boolean> RebuildIndices(Dictionary<TypeUUID, GraphDBType> myUserDefinedTypes)
        {

            #region Remove old indices

            foreach (var type in myUserDefinedTypes)
            {
                foreach (var attrIdx in type.Value.GetAllAttributeIndices(true))
                {
                    attrIdx.Clear(this);
                }
            }
            
            #endregion

            foreach (var userDefinedType in myUserDefinedTypes.Values)
            {
                var _ObjectLocation = new ObjectLocation(userDefinedType.ObjectLocation, DBConstants.DBObjectsLocation);

                using (var _DBObjectsLocationsExceptional = _IGraphFSSession.GetFilteredDirectoryListing(_ObjectLocation, null, null, null, new List<String>(new String[] { DBConstants.DBOBJECTSTREAM }), null, null, null, null, null, null))
                {

                    if (_DBObjectsLocationsExceptional.Success && _DBObjectsLocationsExceptional.Value != null)
                    {

                        foreach (var loc in _DBObjectsLocationsExceptional.Value)
                        {

                            #region If we have only a GUID index - we do not need to load the DB Object

                            var GuidUUID = _DBContext.DBTypeManager.GetGUIDTypeAttribute().UUID;

                            var guidIndexKey = new IndexKeyDefinition(new List<AttributeUUID>() { GuidUUID });

                            if (userDefinedType.AttributeIndices.Count == 1 && userDefinedType.AttributeIndices.ContainsKey(guidIndexKey))
                            {
                                foreach (var edition in userDefinedType.AttributeIndices[guidIndexKey])
                                {

                                    //var curUUID = new ObjectUUID(ByteArrayHelper.FromHexString(loc));
                                    var curUUID = new ObjectUUID(loc);

                                    var indexKey = new IndexKey(GuidUUID, new DBReference(curUUID), edition.Value.IndexKeyDefinition);

                                    var idxRef = edition.Value.GetIndexReference(this);
                                    if (!idxRef.Success)
                                    {
                                        return new Exceptional<bool>(idxRef);
                                    }

                                    idxRef.Value.Add(indexKey, curUUID);
                                }
                            }

                            #endregion

                            else
                            {

                                var dbo = _DBContext.DBObjectManager.LoadDBObject(new ObjectLocation(userDefinedType.ObjectLocation, DBConstants.DBObjectsLocation, loc));

                                if (dbo.Failed)
                                {
                                    return new Exceptional<bool>(dbo);
                                }

                                foreach (var index in userDefinedType.AttributeIndices)
                                {
                                    foreach (var edition in index.Value.Values)
                                    {
                                        edition.Insert(dbo.Value, userDefinedType, _DBContext);
                                    }
                                }
                            }
                        }
                    }
                    else
                        return new Exceptional<bool>(new Error_IndexRebuildError(userDefinedType, _ObjectLocation));
                }
            }

            return new Exceptional<bool>(true);
        }

        public Exceptional RemoveGuidIndexEntriesOfParentTypes(GraphDBType dbType, DBIndexManager dbIndexManager)
        {
            List<GraphDBType> parentTypes = new List<GraphDBType> (_DBContext.DBTypeManager.GetAllParentTypes(dbType, false, false));

            var idxRef = dbType.GetUUIDIndex(_DBContext.DBTypeManager).GetIndexReference(dbIndexManager);
            if (!idxRef.Success)
            {
                return new Exceptional(idxRef);
            }

            foreach (var uuid in idxRef.Value)
            {
                foreach (var type in parentTypes)
                {

                    var typeIdxRef = type.GetUUIDIndex(_DBContext.DBTypeManager).GetIndexReference(dbIndexManager);
                    if (!typeIdxRef.Success)
                    {
                        return new Exceptional(typeIdxRef);
                    }
                    typeIdxRef.Value.Remove(uuid.Key);

                }
            }

            return Exceptional.OK;
        }

        #endregion

        #region private Helper methods

        private IndexKey GenerateIndexKeyForUniqueConstraint(Dictionary<AttributeUUID, AObject> toBeCheckedForUniqueConstraint, IndexKeyDefinition myIndexKeyDefinition, GraphDBType myType)
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

        #region ReCreateDBIndex()

        internal Exceptional RemoveDBIndex(ObjectLocation myObjectLocation)
        {

            return  _IGraphFSSession.RemoveObject(myObjectLocation, DBConstants.DBINDEXSTREAM);

        }

        #endregion

        #region LoadOrCreateDBIndex()

        internal Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>> LoadOrCreateDBIndex(ObjectLocation myObjectLocation, IVersionedIndexObject<IndexKey, ObjectUUID> myIIndexObject)
        {

            if (!(myIIndexObject is AFSObject))
            {
                return new Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>>(new Error_IndexIsNotPersistent(myIIndexObject));
            }

            var result = _IGraphFSSession.GetOrCreateObject<AFSObject>(myObjectLocation, DBConstants.DBINDEXSTREAM, () => myIIndexObject as AFSObject);
            if (!result.Success)
            {
                return new Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>>(result);
            }
            else
            {
                if (result.Value.isNew)
                {
                    // Uncomment as soon as index is serializeable
                    result.AddErrorsAndWarnings(_IGraphFSSession.StoreFSObject(result.Value, false));
                    //result.AddErrorsAndWarnings(result.Value.Save());
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
    }
}
