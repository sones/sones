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


/* <id Name=”sones GraphDB – db object cache” />
 * <copyright file=”DBObjectCache.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>The DBObject cache is the interface to the DBObjects stored in PandoraFS. 
 * It is used to store all DBObjects and BackwardEdges that are used within a query.
 * So, DBObjects are only catched once during a query.<summary>
 */

#region Usings

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphFS.DataStructures;
using sones.Lib.ErrorHandling;
using System.Threading;

#endregion

namespace sones.GraphDB.ObjectManagement
{

    /// <summary>
    /// The DBObject cache is the interface to the DBObjects stored in PandoraFS. 
    /// It is used to store all DBObjects and BackwardEdges that are used within a query.
    /// So, DBObjects are only catched once during a query.
    /// </summary>
    public class DBObjectCache
    {

        #region Properties

        /// <summary>
        /// used for loading DBObjects and BackwardEdges from PandoraFS
        /// </summary>
        DBTypeManager _typeManager;

        /// <summary>
        /// Maximum number of elements
        /// </summary>
        long _maxElements;

        /// <summary>
        /// Current Number of elements
        /// </summary>
        long _currentElements;

        /// <summary>
        /// DBObjectStreams
        /// </summary>
        Dictionary<TypeUUID, ConcurrentDictionary<ObjectUUID, WeakReference>> _cachedDBObjectStreams;

        /// <summary>
        /// BackwardEdges
        /// </summary>
        Dictionary<TypeUUID, ConcurrentDictionary<ObjectUUID, WeakReference>> _cachedBackwardEdges;

        DBObjectManager _DBObjectManager;

        #endregion

        #region Constructor

        public DBObjectCache(DBTypeManager myTypeManager, DBObjectManager objectManager, long myMaxElements)
        {
            _typeManager = myTypeManager;
            _DBObjectManager = objectManager;
            _cachedDBObjectStreams = new Dictionary<TypeUUID, ConcurrentDictionary<ObjectUUID, WeakReference>>();
            _cachedBackwardEdges = new Dictionary<TypeUUID, ConcurrentDictionary<ObjectUUID, WeakReference>>();
            _maxElements = myMaxElements;
            _currentElements = 0;
        }

        #endregion

        #region Public methods

        #region DBObjectStream

        /// <summary>
        /// Loads a DBObject from internal cache structure or PandoraFS (if it is not present in cache)
        /// </summary>
        /// <param name="myType">The type of the DBObject as TypeUUID.</param>
        /// <param name="myObjectUUID">The UUID of the DBObject.</param>
        /// <returns>A DBObject.</returns>
        public Exceptional<DBObjectStream> LoadDBObjectStream(TypeUUID myType, ObjectUUID myObjectUUID)
        {
            return LoadDBObjectStream(_typeManager.GetTypeByUUID(myType), myObjectUUID);
        }

        /// <summary>
        /// Loads a DBObject from internal cache structure or PandoraFS (if it is not present in cache)
        /// </summary>
        /// <param name="myType">The type of the DBObject as DBTypeStream.</param>
        /// <param name="myObjectUUID">The UUID of the DBObject.</param>
        /// <returns>A DBObject.</returns>
        public Exceptional<DBObjectStream> LoadDBObjectStream(GraphDBType myType, ObjectUUID myObjectUUID)
        {
            ConcurrentDictionary<ObjectUUID, WeakReference> items = null;
            lock (_cachedDBObjectStreams)
            {
                if (!_cachedDBObjectStreams.ContainsKey(myType.UUID))
                {
                    _cachedDBObjectStreams.Add(myType.UUID, new ConcurrentDictionary<ObjectUUID, WeakReference>());
                }
                items = _cachedDBObjectStreams[myType.UUID];
            }

            try
            {
                if (_currentElements > _maxElements)
                {
                    if (items.ContainsKey(myObjectUUID))
                    {
                        return GetDBObjectStreamViaWeakReference(myObjectUUID, myType, items[myObjectUUID]);
                    }
                    else
                    {
                        //just load from fs
                        return LoadDBObjectInternal(myType, myObjectUUID);
                    }
                }
                else
                {
                    #region items can be added

                    var aWeakReference = items.GetOrAdd(myObjectUUID, (aObjectUUID) =>
                        {
                            //DBObject must be loaded from PandoraFS
                            var tempResult = LoadDBObjectInternal(myType, aObjectUUID);

                            if (tempResult.Failed)
                            {
                                throw new GraphDBException(tempResult.Errors);
                            }

                            Interlocked.Increment(ref _currentElements);

                            return new WeakReference(tempResult);
                        });

                    return GetDBObjectStreamViaWeakReference(myObjectUUID, myType, aWeakReference);

                    #endregion
                }
            }
            catch (GraphDBException ex)
            {
                return new Exceptional<DBObjectStream>(ex.GraphDBErrors);
            }
        }

        /// <summary>
        /// Loads an Enumaration of DBObjects (if possible from internal cache structure).
        /// </summary>
        /// <param name="myTypeUUID">The Type of the DBObjects as TypeUUID.</param>
        /// <param name="myListOfObjectUUID">The list of ObjectsUUIDs.</param>
        /// <returns>An Enumeratiuon of DBObjects.</returns>
        public IEnumerable<Exceptional<DBObjectStream>> LoadListOfDBObjectStreams(TypeUUID myTypeUUID, IEnumerable<ObjectUUID> myListOfObjectUUID)
        {
            return LoadListOfDBObjectStreams(_typeManager.GetTypeByUUID(myTypeUUID), myListOfObjectUUID);
        }

        /// <summary>
        /// Loads an Enumaration of DBObjects (if possible from internal cache structure).
        /// </summary>
        /// <param name="myType">The Type of the DBObjects as DBTypeStream.</param>
        /// <param name="myListOfObjectUUID">The list of ObjectsUUIDs.</param>
        /// <returns>An Enumeratiuon of DBObjects.</returns>
        public IEnumerable<Exceptional<DBObjectStream>> LoadListOfDBObjectStreams(GraphDBType myType, IEnumerable<ObjectUUID> myListOfObjectUUID)
        {
            var myEnumerator = myListOfObjectUUID.GetEnumerator();

            while (myEnumerator.MoveNext())
            {
                yield return LoadDBObjectStream(myType, myEnumerator.Current);
            }

            yield break;
        }

        public IEnumerable<Exceptional<DBObjectStream>> SelectDBObjectsForLevelKey(LevelKey myLevelKey, DBContext dbContext)
        {
            GraphDBType typeOfDBObjects = null;

            if (myLevelKey.Level == 0)
            {
                typeOfDBObjects = _typeManager.GetTypeByUUID(myLevelKey.Edges[0].TypeUUID);

                if (!typeOfDBObjects.IsAbstract)
                {
                    var idx = typeOfDBObjects.GetUUIDIndex(dbContext.DBTypeManager);
                    var currentIndexType = dbContext.DBTypeManager.GetTypeByUUID(idx.IndexRelatedTypeUUID);

                    foreach (var aDBO in LoadListOfDBObjectStreams(typeOfDBObjects, idx.GetAllUUIDs(currentIndexType, dbContext)))
                    {
                        yield return aDBO;
                    }
                }
                else
                {
                    foreach (var aType in _typeManager.GetAllSubtypes(typeOfDBObjects, false))
                    {
                        var idx = aType.GetUUIDIndex(dbContext.DBTypeManager);
                        var currentIndexType = dbContext.DBTypeManager.GetTypeByUUID(idx.IndexRelatedTypeUUID);

                        foreach (var aDBO in LoadListOfDBObjectStreams(aType, idx.GetAllUUIDs(currentIndexType, dbContext)))
                        {
                            yield return aDBO;
                        }
                    }
                }
            }
            else
            {

                #region data

                TypeAttribute lastAttributeOfLevelKey = null;

                #endregion

                #region find the correct attribute

                lastAttributeOfLevelKey = _typeManager.GetTypeByUUID(myLevelKey.LastEdge.TypeUUID).GetTypeAttributeByUUID(myLevelKey.LastEdge.AttrUUID);

                if (lastAttributeOfLevelKey == null)
                {
                    throw new GraphDBException(new Error_InvalidAttribute(String.Format("The attribute with UUID \"{0}\" is not valid for type with UUID \"{1}\".", myLevelKey.LastEdge.AttrUUID, myLevelKey.LastEdge.TypeUUID)));
                }

                #endregion

                #region find out which type we need

                if (lastAttributeOfLevelKey.GetDBType(dbContext.DBTypeManager).IsUserDefined)
                {
                    typeOfDBObjects = lastAttributeOfLevelKey.GetDBType(dbContext.DBTypeManager);
                }
                else
                {
                    if (lastAttributeOfLevelKey.IsBackwardEdge)
                    {
                        typeOfDBObjects = _typeManager.GetTypeByUUID(lastAttributeOfLevelKey.BackwardEdgeDefinition.TypeUUID);
                    }
                    else
                    {
                        typeOfDBObjects = _typeManager.GetTypeByUUID(myLevelKey.LastEdge.TypeUUID);
                    }
                }

                #endregion

                #region yield dbos

                var idx = typeOfDBObjects.GetUUIDIndex(dbContext.DBTypeManager);
                var currentIndexType = dbContext.DBTypeManager.GetTypeByUUID(idx.IndexRelatedTypeUUID);

                foreach (var aDBO in LoadListOfDBObjectStreams(typeOfDBObjects, idx.GetAllUUIDs(currentIndexType, dbContext)))
                {
                    if (IsValidDBObjectForLevelKey(aDBO, myLevelKey, typeOfDBObjects))
                    {
                        yield return aDBO;
                    }
                }
            }

            yield break;

                #endregion
        }

        #endregion

        #region BackwardEdge

        /// <summary>
        /// Loads a DBBackwardEdge from internal cache structure or PandoraFS (if it is not present in cache)
        /// </summary>
        /// <param name="myType">The Type of the DBObjects as DBTypeStream.</param>
        /// <param name="myObjectUUID">The UUID of the corresponding DBObject.</param>
        /// <returns>A BackwardEdge</returns>
        public Exceptional<BackwardEdgeStream> LoadDBBackwardEdgeStream(GraphDBType myType, ObjectUUID myObjectUUID)
        {
            ConcurrentDictionary<ObjectUUID, WeakReference> items = null;
            lock (_cachedBackwardEdges)
            {
                if (!_cachedBackwardEdges.ContainsKey(myType.UUID))
                {
                    _cachedBackwardEdges.Add(myType.UUID, new ConcurrentDictionary<ObjectUUID, WeakReference>());
                }
                items = _cachedBackwardEdges[myType.UUID];
            }

            try
            {
                if (_currentElements > _maxElements)
                {
                    if (items.ContainsKey(myObjectUUID))
                    {
                        return GetBackwardEdgeStreamViaWeakReference(myObjectUUID, myType, items[myObjectUUID]);
                    }
                    else
                    {
                        //just load from fs
                        return LoadDBBackwardEdgeInternal(myType, myObjectUUID);
                    }
                }
                else
                {
                    var aWeakReference = items.GetOrAdd(myObjectUUID, (aObjectUUID) =>
                    {
                        //DBObject must be loaded from PandoraFS
                        var tempResult = LoadDBBackwardEdgeInternal(myType, aObjectUUID);

                        if (tempResult.Failed)
                        {
                            throw new GraphDBException(tempResult.Errors);
                        }

                        Interlocked.Increment(ref _currentElements);

                        return new WeakReference(tempResult);
                    });

                    return GetBackwardEdgeStreamViaWeakReference(myObjectUUID, myType, aWeakReference);
                }
            }
            catch (GraphDBException ex)
            {
                return new Exceptional<BackwardEdgeStream>(ex.GraphDBErrors);
            }
        }

        /// <summary>
        /// Loads a DBBackwardEdge from internal cache structure or PandoraFS (if it is not present in cache)
        /// </summary>
        /// <param name="myType">The Type of the DBObjects as TypeUUID.</param>
        /// <param name="myObjectUUID">The UUID of the corresponding DBObject.</param>
        /// <returns>A BackwardEdge</returns>
        public Exceptional<BackwardEdgeStream> LoadDBBackwardEdgeStream(TypeUUID myType, ObjectUUID myObjectUUID)
        {
            return LoadDBBackwardEdgeStream(_typeManager.GetTypeByUUID(myType), myObjectUUID);
        }

        #endregion

        #endregion

        #region private methods

        #region DBObject

        /// <summary>
        /// Internal method for loading a DBObject from PandoraFS.
        /// </summary>
        /// <param name="myType">The Type of the DBObjects as TypeUUID.</param>
        /// <param name="myObjectUUID">The UUID of the DBObject.</param>
        /// <returns>An DBObject</returns>
        private Exceptional<DBObjectStream> LoadDBObjectInternal(TypeUUID myType, ObjectUUID myObjectUUID)
        {
            return LoadDBObjectInternal(_typeManager.GetTypeByUUID(myType), myObjectUUID);
        }

        /// <summary>
        /// Internal method for loading a DBObject from PandoraFS.
        /// </summary>
        /// <param name="myType">The Type of the DBObjects as PandoraType.</param>
        /// <param name="myObjectUUID">The UUID of the DBObject.</param>
        /// <returns>An DBObject</returns>
        private Exceptional<DBObjectStream> LoadDBObjectInternal(GraphDBType myType, ObjectUUID myObjectUUID)
        {

            var tempResult = _DBObjectManager.LoadDBObject(myType, myObjectUUID);

            #region Try all subTypes - as long as the Symlink alternativ does not work

            if (tempResult.Failed)
            {

                var exceptional = new Exceptional<DBObjectStream>(tempResult);

                #region Try sub types

                foreach (var type in _typeManager.GetAllSubtypes(myType, false))
                {
                    tempResult = LoadDBObjectInternal(type, myObjectUUID);
                    if (tempResult.Success)
                        break;
                    else
                        exceptional = new Exceptional<DBObjectStream>(tempResult);
                }

                #endregion

                if (tempResult.Failed)
                    return exceptional;
            }

            #endregion

            return tempResult;
        }

        private Exceptional<DBObjectStream> GetDBObjectStreamViaWeakReference(ObjectUUID uuidOfDBObject, GraphDBType typeOfDBObject, WeakReference wrDBObject)
        {
            var aDBO = wrDBObject.Target as Exceptional<DBObjectStream>;
            if (aDBO == null)
            {
                // Object was reclaimed, so get it again
                aDBO = LoadDBObjectInternal(typeOfDBObject, uuidOfDBObject);
            }

            return aDBO;
        }

        #endregion

        #region BackwardEdge

        /// <summary>
        /// Internal method for loading a DBBackwardEdge from PandoraFS. 
        /// </summary>
        /// <param name="myType">The Type of the DBObjects as TypeUUID.</param>
        /// <param name="myObjectUUID">The UUID of the corresponding DBObject.</param>
        /// <returns>A BackwardEdge</returns>
        private Exceptional<BackwardEdgeStream> LoadDBBackwardEdgeInternal(TypeUUID myType, ObjectUUID myObjectUUID)
        {
            return LoadDBBackwardEdgeInternal(_typeManager.GetTypeByUUID(myType), myObjectUUID);
        }

        /// <summary>
        /// Internal method for loading a DBBackwardEdge from PandoraFS. 
        /// </summary>
        /// <param name="myType">The Type of the DBObjects as PandoraType.</param>
        /// <param name="myObjectUUID">The UUID of the corresponding DBObject.</param>
        /// <returns>A BackwardEdge</returns>
        private Exceptional<BackwardEdgeStream> LoadDBBackwardEdgeInternal(GraphDBType myType, ObjectUUID myObjectUUID)
        {
            return _DBObjectManager.LoadBackwardEdge(new ObjectLocation(myType.ObjectLocation, DBConstants.DBObjectsLocation, myObjectUUID.ToString()));
        }

        private Exceptional<BackwardEdgeStream> GetBackwardEdgeStreamViaWeakReference(ObjectUUID myObjectUUID, GraphDBType myType, WeakReference weakReference)
        {
            var aBackwardEdge = weakReference.Target as Exceptional<BackwardEdgeStream>;
            if (aBackwardEdge == null)
            {
                // Object was reclaimed, so get it again
                aBackwardEdge = LoadDBBackwardEdgeInternal(myType, myObjectUUID);
            }

            return aBackwardEdge;
        }

        #endregion

        #region misc

        private IEnumerable<Exceptional<DBObjectStream>> GetReferenceObjects(DBObjectStream myStartingDBObject, TypeAttribute interestingAttributeEdge, GraphDBType myStartingDBObjectType, DBTypeManager myDBTypeManager)
        {
            if (interestingAttributeEdge.GetDBType(myDBTypeManager).IsUserDefined || interestingAttributeEdge.IsBackwardEdge)
            {
                switch (interestingAttributeEdge.KindOfType)
                {
                    case KindsOfType.SingleReference:

                        yield return LoadDBObjectStream(interestingAttributeEdge.GetDBType(myDBTypeManager), ((ASingleReferenceEdgeType)myStartingDBObject.GetAttribute(interestingAttributeEdge.UUID)).GetUUID());

                        break;

                    case KindsOfType.SetOfReferences:

                        if (interestingAttributeEdge.IsBackwardEdge)
                        {
                            //get backwardEdge
                            var beStream = LoadDBBackwardEdgeStream(myStartingDBObjectType, myStartingDBObject.ObjectUUID);

                            if (beStream.Failed)
                            {
                                throw new GraphDBException(new Error_ExpressionGraphInternal(null, String.Format("Error while trying to get BackwardEdge of the DBObject: \"{0}\"", myStartingDBObject.ToString())));
                            }

                            if (beStream.Value.ContainsBackwardEdge(interestingAttributeEdge.BackwardEdgeDefinition))
                            {
                                foreach (var aBackwardEdgeObject in LoadListOfDBObjectStreams(interestingAttributeEdge.BackwardEdgeDefinition.TypeUUID, beStream.Value.GetBackwardEdgeUUIDs(interestingAttributeEdge.BackwardEdgeDefinition)))
                                {
                                    yield return aBackwardEdgeObject;
                                }
                            }
                        }
                        else
                        {
                            foreach (var aDBOStream in LoadListOfDBObjectStreams(interestingAttributeEdge.GetDBType(myDBTypeManager), ((ASetReferenceEdgeType)myStartingDBObject.GetAttribute(interestingAttributeEdge.UUID)).GetAllReferenceIDs()))
                            {
                                yield return aDBOStream;
                            }
                        }

                        break;

                    case KindsOfType.SetOfNoneReferences:
                    case KindsOfType.ListOfNoneReferences:
                    default:
                        throw new GraphDBException(new Error_ExpressionGraphInternal(new System.Diagnostics.StackTrace(true), String.Format("The attribute \"{0}\" has an invalid KindOfType \"{1}\"!", interestingAttributeEdge.Name, interestingAttributeEdge.KindOfType.ToString())));
                }
            }
            else
            {
                throw new GraphDBException(new Error_ExpressionGraphInternal(new System.Diagnostics.StackTrace(true), String.Format("The attribute \"{0}\" is no reference attribute.", interestingAttributeEdge.Name)));
            }

            yield break;
        }

        private bool IsValidDBObjectForLevelKey(Exceptional<DBObjectStream> aDBO, LevelKey myLevelKey, GraphDBType typeOfDBO)
        {
            if (myLevelKey.Level == 0)
            {
                return true;
            }
            else
            {
                Boolean isValidDBO = false;

                EdgeKey backwardEdgeKey = myLevelKey.LastEdge;
                TypeAttribute currentAttribute = _typeManager.GetTypeByUUID(backwardEdgeKey.TypeUUID).GetTypeAttributeByUUID(backwardEdgeKey.AttrUUID);
                IEnumerable<Exceptional<DBObjectStream>> dbobjects = null;
                GraphDBType typeOfBackwardDBOs = null;

                if (currentAttribute.IsBackwardEdge)
                {
                    backwardEdgeKey = currentAttribute.BackwardEdgeDefinition;

                    currentAttribute = _typeManager.GetTypeByUUID(backwardEdgeKey.TypeUUID).GetTypeAttributeByUUID(backwardEdgeKey.AttrUUID);

                    typeOfBackwardDBOs = currentAttribute.GetDBType(_typeManager);

                    if (aDBO.Value.HasAttribute(backwardEdgeKey.AttrUUID, typeOfDBO))
                    {
                        dbobjects = GetReferenceObjects(aDBO.Value, currentAttribute, typeOfDBO, _typeManager);
                    }
                }
                else
                {
                    BackwardEdgeStream beStreamOfDBO = LoadDBBackwardEdgeStream(typeOfDBO, aDBO.Value.ObjectUUID).Value;

                    typeOfBackwardDBOs = _typeManager.GetTypeByUUID(backwardEdgeKey.TypeUUID);

                    if (beStreamOfDBO.ContainsBackwardEdge(backwardEdgeKey))
                    {
                        dbobjects = LoadListOfDBObjectStreams(typeOfBackwardDBOs, beStreamOfDBO.GetBackwardEdgeUUIDs(backwardEdgeKey));
                    }
                }

                if (dbobjects != null)
                {
                    LevelKey myLevelKeyPred = myLevelKey.GetPredecessorLevel(_typeManager);

                    foreach (var aBackwardDBO in dbobjects)
                    {
                        if (aBackwardDBO.Success)
                        {
                            if (IsValidDBObjectForLevelKey(aBackwardDBO, myLevelKeyPred, typeOfBackwardDBOs))
                            {
                                isValidDBO = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }

                return isValidDBO;
            }
        }

        #endregion

        #endregion
    }

}
