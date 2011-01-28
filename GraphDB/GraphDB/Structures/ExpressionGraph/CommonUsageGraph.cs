/* <id name="GraphDB – Graph" />
 * <copyright file="CommonUsageGraph.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This class implements a common usage expression graph.</summary>
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
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.Warnings;
using sones.GraphFS.DataStructures;
using sones.Lib;
using sones.Lib.ErrorHandling;
using System.Threading.Tasks;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures.Enums;


#endregion

namespace sones.GraphDB.Structures.ExpressionGraph
{

    /// <summary>
    /// This class implements the expression graph.
    /// </summary>
    public class CommonUsageGraph : AExpressionGraph
    {
        #region Properties

        /// <summary>
        /// The TypeManager of the Database
        /// </summary>
        private DBContext _DBContext;

        /// <summary>
        /// The cache of the current query
        /// </summary>
        private DBObjectCache _DBObjectCache;

        /// <summary>
        /// The levels of the expression graph
        /// </summary>
        private Dictionary<int, IExpressionLevel> _Levels;

        ///// <summary>
        ///// Es wird ein BackwardEdge level aufgelöst (U.Friends.Friends.Friends.Name = >löst nur ...[BE]Friends.Name auf - aber nicht bis U
        ///// Es wird validiert! Aber nicht im graphen gespeichert.
        ///// </summary>
        //private int _defaultBackwardResolution = 0;

        private List<IWarning> _warnings;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">The TypeManager of the database.</param>
        public CommonUsageGraph(DBContext dbContext)
            : this()
        {
            _DBContext = dbContext;
            _DBObjectCache = _DBContext.DBObjectCache;
            _Levels = new Dictionary<int, IExpressionLevel>();
        }

        private CommonUsageGraph()
        {
            _performanceStatement.Add(GraphPerformanceCriteria.Multithreading, 2);
            _performanceStatement.Add(GraphPerformanceCriteria.Space, 5);
            _performanceStatement.Add(GraphPerformanceCriteria.Time, 5);
            _performanceStatement.Add(GraphPerformanceCriteria.LevelResolution, 10);
            _performanceStatement.Add(GraphPerformanceCriteria.FastInsertion, 10);
            _performanceStatement.Add(GraphPerformanceCriteria.FastDataExtraction, 10);

            _warnings = new List<IWarning>();
        }

        #endregion

        #region Public methods

        public override bool ContainsRelevantLevelForType(GraphDBType myType)
        {
            foreach (var aLevel in _Levels)
            {
                foreach (var aLevelContent in aLevel.Value.ExpressionLevels)
                {
                    if (aLevelContent.Key.Edges[0].TypeUUID == myType.UUID)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override List<IWarning> GetWarnings()
        {
            lock (_warnings)
            {
                return _warnings;
            }
        }

        public override Dictionary<GraphPerformanceCriteria, short> GetPerformanceStatement()
        {
            return _performanceStatement;
        }

        public override IExpressionLevel GetLevel(int myLevel)
        {
            lock (_Levels)
            {
                if (_Levels.ContainsKey(myLevel))
                    return _Levels[myLevel];
            }
            return null;
        }

        public override Dictionary<int, IExpressionLevel> Levels
        {
            get
            {
                lock (_Levels)
                {
                    return _Levels;
                }
            }
        }

        public override Boolean IsGraphRelevant(LevelKey myLevelKey, DBObjectStream mySourceDBObject)
        {
            lock (_Levels)
            {
                if (!this.ContainsLevelKey(myLevelKey))
                {
                    return false;
                }

                if (myLevelKey.Level == 0)
                {
                    return this._Levels[myLevelKey.Level].ExpressionLevels[myLevelKey].Nodes.ContainsKey(mySourceDBObject.ObjectUUID);
                }
                else
                {
                    if (mySourceDBObject != null)
                    {
                        var predecessorLevelKey = myLevelKey.GetPredecessorLevel(_DBContext.DBTypeManager);

                        if (!this.ContainsLevelKey(predecessorLevelKey))
                        {
                            var interestingEdge = new ExpressionEdge(mySourceDBObject.ObjectUUID, null, predecessorLevelKey.LastEdge);
                            //take the backwardEdges 
                            return this._Levels[myLevelKey.Level].ExpressionLevels[myLevelKey].Nodes.Exists(item => item.Value.BackwardEdges[predecessorLevelKey.LastEdge].Contains(interestingEdge));
                        }
                        else
                        {
                            if (this._Levels[predecessorLevelKey.Level].ExpressionLevels[predecessorLevelKey].Nodes[mySourceDBObject.ObjectUUID].ForwardEdges.ContainsKey(myLevelKey.LastEdge))
                            {
                                if (this._Levels[predecessorLevelKey.Level].ExpressionLevels[predecessorLevelKey].Nodes[mySourceDBObject.ObjectUUID].ForwardEdges[myLevelKey.LastEdge].Count > 0)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        throw new GraphDBException(new Error_ExpressionGraphInternal(new System.Diagnostics.StackTrace(true), "No DBObjectStream givon for graph relevance test in a higher level."));
                    }
                }
            }

        }

        public override IEnumerable<Exceptional<DBObjectStream>> Select(LevelKey myLevelKey, DBObjectStream mySourceDBObject = null, Boolean doLevelGeneration = true)
        {
            lock (_Levels)
            {
                if (doLevelGeneration)
                {
                    if (!this.ContainsLevelKey(myLevelKey))
                    {
                        //the graph does not contain the LevelKey, so create it
                        #region create LevelKey

                        GenerateLevel(myLevelKey);

                        #endregion
                    }
                }

                if (myLevelKey.Level == 0)
                {
                    if (this.ContainsLevelKey(myLevelKey))
                    {
                        //return all Objects of this levelKey
                        foreach (var aNode in this._Levels[myLevelKey.Level].ExpressionLevels[myLevelKey].Nodes)
                        {
                            yield return new Exceptional<DBObjectStream>(aNode.Value.GetDBObjectStream(_DBObjectCache, myLevelKey.LastEdge.TypeUUID));
                        }
                    }
                    else
                    {
                        yield break;
                    }
                }
                else
                {
                    if (mySourceDBObject != null)
                    {
                        var predecessorLevelKey = myLevelKey.GetPredecessorLevel(_DBContext.DBTypeManager);

                        if (!this.ContainsLevelKey(predecessorLevelKey))
                        {
                            //take the backwardEdges 
                            foreach (var aNode in this._Levels[myLevelKey.Level].ExpressionLevels[myLevelKey].Nodes.Where(item => item.Value.BackwardEdges[predecessorLevelKey.LastEdge].Exists(aBackWardEdge => aBackWardEdge.Destination == mySourceDBObject.ObjectUUID)))
                            {
                                yield return new Exceptional<DBObjectStream>(aNode.Value.GetDBObjectStream(_DBObjectCache, myLevelKey.LastEdge.TypeUUID));
                            }
                        }
                        else
                        {
                            //take the forwardEdges

                            TypeAttribute currentAttribute = _DBContext.DBTypeManager.GetTypeByUUID(myLevelKey.LastEdge.TypeUUID).GetTypeAttributeByUUID(myLevelKey.LastEdge.AttrUUID);
                            GraphDBType myType = GetTypeOfAttribute(currentAttribute.GetRelatedType(_DBContext.DBTypeManager), currentAttribute);


                            foreach (var aDBO in _DBObjectCache.LoadListOfDBObjectStreams(myType, this._Levels[predecessorLevelKey.Level].ExpressionLevels[predecessorLevelKey].Nodes[mySourceDBObject.ObjectUUID].ForwardEdges[myLevelKey.LastEdge].Select(item => item.Destination)))
                            {
                                if (aDBO.Failed())
                                {
                                    AddWarning(new Warning_CouldNotLoadDBObject(aDBO.IErrors, new System.Diagnostics.StackTrace(true)));
                                }
                                else
                                {
                                    yield return aDBO;
                                }
                            }
                        }
                    }
                    else
                    {
                        //there is no sourceObject given, so return the complete level
                        foreach (var aNode in this._Levels[myLevelKey.Level].ExpressionLevels[myLevelKey].Nodes)
                        {
                            yield return new Exceptional<DBObjectStream>(aNode.Value.GetDBObjectStream(_DBObjectCache, myLevelKey.LastEdge.TypeUUID));
                        }
                    }
                }

            }
            //all done 
            yield break;

        }

        public override IEnumerable<ObjectUUID> SelectUUIDs(LevelKey myLevelKey, DBObjectStream mySourceDBObject = null, Boolean doLevelGeneration = true)
        {
            lock (_Levels)
            {
                if (doLevelGeneration)
                {
                    if (!this.ContainsLevelKey(myLevelKey))
                    {
                        //the graph does not contain the LevelKey, so create it
                        #region create LevelKey

                        GenerateLevel(myLevelKey);

                        #endregion
                    }
                }

                if (myLevelKey.Level == 0)
                {
                    if (this.ContainsLevelKey(myLevelKey))
                    {
                        //return all Objects of this levelKey
                        foreach (var aNode in this._Levels[myLevelKey.Level].ExpressionLevels[myLevelKey].Nodes)
                        {
                            yield return aNode.Value.GetObjectUUID();
                        }
                    }
                    else
                    {
                        yield break;
                    }
                }
                else
                {
                    if (mySourceDBObject != null)
                    {
                        var predecessorLevelKey = myLevelKey.GetPredecessorLevel(_DBContext.DBTypeManager);

                        if (!this.ContainsLevelKey(predecessorLevelKey))
                        {
                            //take the backwardEdges 
                            foreach (var aNode in this._Levels[myLevelKey.Level].ExpressionLevels[myLevelKey].Nodes.Where(item => item.Value.BackwardEdges[predecessorLevelKey.LastEdge].Exists(aBackWardEdge => aBackWardEdge.Destination == mySourceDBObject.ObjectUUID)))
                            {
                                yield return aNode.Value.GetObjectUUID();
                            }
                        }
                        else
                        {
                            //take the forwardEdges
                            if (this._Levels[predecessorLevelKey.Level].ExpressionLevels[predecessorLevelKey].Nodes[mySourceDBObject.ObjectUUID].ForwardEdges.ContainsKey(myLevelKey.LastEdge))
                            {
                                foreach (var aUUID in this._Levels[predecessorLevelKey.Level].ExpressionLevels[predecessorLevelKey].Nodes[mySourceDBObject.ObjectUUID].ForwardEdges[myLevelKey.LastEdge].Select(item => item.Destination))
                                {
                                    yield return aUUID;
                                }
                            }
                            //else
                            //{
                            //    AddNode(mySourceDBObject, myLevelKey);
                            //    //var attrVal = mySourceDBObject.GetAttribute(myLevelKey.LastEdge.AttrUUID);
                            //}
                        }
                    }
                    else
                    {
                        //there is no sourceObject given, so return the complete level
                        foreach (var aNode in this._Levels[myLevelKey.Level].ExpressionLevels[myLevelKey].Nodes)
                        {
                            yield return aNode.Value.GetObjectUUID();
                        }
                    }
                }

            }
            //all done 
            yield break;

        }

        public override void BuildDifferenceWith(IExpressionGraph anotherGraph)
        {
            throw new NotImplementedException();
        }

        public override void AddNodesWithComplexRelation(Exceptional<DBObjectStream> leftDBObject, LevelKey leftLevelKey, Exceptional<DBObjectStream> rightDBObject, LevelKey rightLevelKey, DBObjectCache dbObjectCache, int backwardResolutiondepth)
        {
            lock (_Levels)
            {
                if ((AddNodeIfValid(leftDBObject.Value, leftLevelKey, 0, leftDBObject.Value.ObjectUUID, backwardResolutiondepth)) && (AddNodeIfValid(rightDBObject.Value, rightLevelKey, 0, rightDBObject.Value.ObjectUUID, backwardResolutiondepth)))
                {
                    //both nodes have been inserted correctly
                    //--> create a connection between both
                    _Levels[leftLevelKey.Level].ExpressionLevels[leftLevelKey].Nodes[leftDBObject.Value.ObjectUUID].AddComplexConnection(rightLevelKey, rightDBObject.Value.ObjectUUID);
                    _Levels[rightLevelKey.Level].ExpressionLevels[rightLevelKey].Nodes[rightDBObject.Value.ObjectUUID].AddComplexConnection(leftLevelKey, leftDBObject.Value.ObjectUUID);
                }
                else
                {
                    #region remove both nodes

                    if (ContainsLevelKey(leftLevelKey))
                    {
                        _Levels[leftLevelKey.Level].RemoveNode(leftLevelKey, leftDBObject.Value.ObjectUUID);
                    }
                    if (ContainsLevelKey(rightLevelKey))
                    {
                        _Levels[rightLevelKey.Level].RemoveNode(rightLevelKey, rightDBObject.Value.ObjectUUID);
                    }

                    #endregion
                }
            }
        }

        public override void AddNodes(IEnumerable<IExpressionNode> iEnumerable, LevelKey myPath)
        {
            foreach (var aNode in iEnumerable)
            {
                AddNodeToLevel(aNode, myPath);
            }
        }

        public override void AddNode(DBObjectStream myDBObjectStream, LevelKey myLevelKey, int backwardResolution)
        {
            AddNodeIfValid(myDBObjectStream, myLevelKey, 0, myDBObjectStream.ObjectUUID, backwardResolution);
        }

        public override void AddNode(DBObjectStream myDBObjectStream, LevelKey myLevelKey)
        {
            AddNode(myDBObjectStream, myLevelKey, 0);
        }

        public override Boolean AddEmptyLevel(LevelKey myLevelKey)
        {
            lock (_Levels)
            {
                if (!_Levels.ContainsKey(myLevelKey.Level))
                {
                    _Levels.Add(myLevelKey.Level, new ExpressionLevel());
                    _Levels[myLevelKey.Level].AddEmptyLevelKey(myLevelKey);

                    return true;
                }
                if (!_Levels[myLevelKey.Level].HasLevelKey(myLevelKey))
                {
                    _Levels[myLevelKey.Level].AddEmptyLevelKey(myLevelKey);

                    return true;
                }
            }
            return false;
        }

        public override IExpressionGraph GetNewInstance(DBContext dbContext)
        {
            return new ExpressionGraph.CommonUsageGraph(dbContext);
        }

        public override void GatherEdgeWeight(LevelKey StartLevel, LevelKey EndLevel)
        {
            throw new NotImplementedException();
        }

        public override void UnionWith(IExpressionGraph anotherGraph)
        {
            #region Downfill both graphs

            #region Find min level of both graphs

            var thisMinLevel = this.Levels.Min(item => item.Key);
            var thisMaxLevel = this.Levels.Max(item => item.Key);
            var thisMinLevelKeys = GetMinLevelKeys(this, thisMinLevel);

            var anotherMinLevel = anotherGraph.Levels.Min(item => item.Key);
            var anotherMaxLevel = anotherGraph.Levels.Max(item => item.Key);
            var anotherMinLevelKeys = GetMinLevelKeys(anotherGraph, anotherMinLevel);

            #endregion

            var runMT = DBConstants.RunMT;

            #region Downfill

            //TODO: only fill down to a certain levelKey)
            if (runMT)
            {
                Parallel.Invoke(
                    () =>
                    {
                        DownFillGraph(this, thisMinLevelKeys, thisMinLevel);
                    },
                    () =>
                    {
                        DownFillGraph(anotherGraph, anotherMinLevelKeys, anotherMinLevel);
                    });
            }
            else
            {
                DownFillGraph(this, thisMinLevelKeys, thisMinLevel);
                DownFillGraph(anotherGraph, anotherMinLevelKeys, anotherMinLevel);

            }

            #endregion

            #endregion

            #region build level differences

            var diffForThis = GetLevelKeyDifference(this, anotherGraph);
            var diffForAnotherGraph = GetLevelKeyDifference(anotherGraph, this);

            #endregion

            #region upfill
            
            //upfill levelKey differences of each graph & update Nodes in both graphs
            if (runMT)
            {
                Parallel.Invoke(
                    () =>
                    {
                        UpgradeGraphStructure(this, diffForThis);
                        UpfillGraph(this, diffForThis);
                    },
                    () =>
                    {
                        UpgradeGraphStructure(anotherGraph, diffForAnotherGraph);
                        UpfillGraph(anotherGraph, diffForAnotherGraph);
                    });
            }
            else
            {
                UpgradeGraphStructure(this, diffForThis);
                UpfillGraph(this, diffForThis);

                UpgradeGraphStructure(anotherGraph, diffForAnotherGraph);
                UpfillGraph(anotherGraph, diffForAnotherGraph);
            }

            #endregion

            #region merge another into this graph

            foreach (var aLevel in anotherGraph.Levels)
            {
                foreach (var aLevelPayLoad in aLevel.Value.ExpressionLevels)
                {
                    foreach (var aNode in aLevelPayLoad.Value.Nodes)
                    {
                        MergeNodeIntoGraph(this, aLevelPayLoad.Key, aNode.Value, anotherGraph);
                    }
                }
            }

            #endregion
        }

        public override void IntersectWith(IExpressionGraph anotherGraph)
        {
            #region Data

            HashSet<LevelKey> thisMinLevelKeys;
            int thisMinLevel;
            int thisMaxLevel;

            HashSet<LevelKey> anotherMinLevelKeys;
            int anotherMinLevel;
            int anotherMaxLevel;

            #endregion

            #region special case
            //there are two graphs with only one identical level and only one identical levelKey
            // sth. like (u.Friends.Friends.Friends and u.Friends.Friends.Friends)
            if ((this.Levels.Count == 1) && (anotherGraph.Levels.Count == 1) && (this.Levels.ContainsKey(0)) && (anotherGraph.Levels.ContainsKey(0)) && (this.Levels[0].ExpressionLevels.Count == 1) && (anotherGraph.Levels[0].ExpressionLevels.Count == 1) && (this.Levels[0].ExpressionLevels.First().Key == anotherGraph.Levels[0].ExpressionLevels.First().Key))
            {
                LevelKey interestingLevelKey = this.Levels[0].ExpressionLevels.First().Key;
                List<ObjectUUID> toBeDeletedNodes = new List<ObjectUUID>();

                foreach (var aNode in this.Levels[0].ExpressionLevels.First().Value.Nodes)
                {
                    if (!anotherGraph.Levels[0].ExpressionLevels.First().Value.Nodes.ContainsKey(aNode.Key))
                    {
                        toBeDeletedNodes.Add(aNode.Key);
                    }
                }

                foreach (var aUUID in toBeDeletedNodes)
                {
                    this.Levels[0].RemoveNode(interestingLevelKey, aUUID);
                }

            }

            #endregion

            else
            {
                #region Find min level of both graphs

                thisMinLevel = this.Levels.Min(item => item.Key);
                thisMaxLevel = this.Levels.Max(item => item.Key);
                thisMinLevelKeys = GetMinLevelKeys(this, thisMinLevel);

                anotherMinLevel = anotherGraph.Levels.Min(item => item.Key);
                anotherMaxLevel = anotherGraph.Levels.Max(item => item.Key);
                anotherMinLevelKeys = GetMinLevelKeys(anotherGraph, anotherMinLevel);

                #endregion

                var runMT = DBConstants.RunMT;

                #region Downfill both graphs

                //TODO: only fill down to a certain levelKey)
                if (runMT)
                {
                    Parallel.Invoke(
                        () =>
                        {
                            DownFillGraph(this, thisMinLevelKeys, thisMinLevel);
                        },
                        () =>
                        {
                            DownFillGraph(anotherGraph, anotherMinLevelKeys, anotherMinLevel);
                        });
                }
                else
                {
                    DownFillGraph(this, thisMinLevelKeys, thisMinLevel);
                    DownFillGraph(anotherGraph, anotherMinLevelKeys, anotherMinLevel);

                }

                #endregion

                #region intersection

                #region --> *MinLevel

                CleanGraphDown(this, thisMinLevel, anotherGraph, thisMinLevelKeys);
                CleanGraphDown(anotherGraph, anotherMinLevel, this, anotherMinLevelKeys);

                #endregion

                #region *MinLevel -->
                //integrate levels that are greater than thisMinLevel from another graph into this

                CleanGraphUp(this, anotherGraph, thisMinLevel, anotherMaxLevel);
                CleanGraphUp(anotherGraph, this, anotherMinLevel, thisMaxLevel);

                #endregion

                #region Merge both graphs

                MergeGraphs(this, thisMaxLevel, anotherGraph, anotherMaxLevel);

                #endregion

                #endregion
            }
        }

        #endregion

        #region private methods

        /// <summary>
        /// This method adds a Node to a Level.
        /// </summary>
        /// <param name="myNode">The Node that is going to be added</param>
        /// <param name="myPath">The place where the node is going to be added.</param>
        private void AddNodeToLevel(IExpressionNode myNode, LevelKey myPath)
        {
            lock (_Levels)
            {
                AddEmptyLevel(myPath);

                _Levels[myPath.Level].AddNode(myPath, myNode);
            }
        }

        /// <summary>
        /// This method adds a DBOBjectStream to a Level if it is valid for a LevelKey.
        /// </summary>
        /// <param name="aDBObject">The Object that is going to be added</param>
        /// <param name="myLevelKey">The LevelKey which is needed for validation.</param>
        /// <param name="currentBackwardResolution">The current backward resolution (initially 0)</param>
        /// <param name="source">The ObjectUUID of the </param>
        /// <returns>True if it was valid or false otherwise.</returns>
        private bool AddNodeIfValid(DBObjectStream aDBObject, LevelKey myLevelKey, int currentBackwardResolution, ObjectUUID source, int backwardResolutiondepth)
        {
            #region data

            Exceptional<BackwardEdgeStream> beStream            = null;
            TypeAttribute                   tempTypeAttribute   = null;
            
            IEnumerable<ObjectUUID>         referenceUUIDs      = null;
            GraphDBType                     referenceType       = null;

            #endregion

            if ((myLevelKey.Level - currentBackwardResolution) > 0)
            {
                #region level > 0

                int desiredBackwardEdgeLevel = myLevelKey.Level - currentBackwardResolution - 1;

                tempTypeAttribute = _DBContext.DBTypeManager.GetTypeByUUID(myLevelKey.Edges[desiredBackwardEdgeLevel].TypeUUID).GetTypeAttributeByUUID(myLevelKey.Edges[desiredBackwardEdgeLevel].AttrUUID);

                #region get reference UUIDs

                if (tempTypeAttribute.IsBackwardEdge)
                {
                    #region backward edge handling

                    referenceType = _DBContext.DBTypeManager.GetTypeByUUID(tempTypeAttribute.BackwardEdgeDefinition.TypeUUID).GetTypeAttributeByUUID(tempTypeAttribute.BackwardEdgeDefinition.AttrUUID).GetDBType(_DBContext.DBTypeManager);

                    if (aDBObject.HasAttribute(tempTypeAttribute.BackwardEdgeDefinition.AttrUUID, _DBContext.DBTypeManager.GetTypeByUUID(tempTypeAttribute.BackwardEdgeDefinition.TypeUUID)))
                    {
                        referenceUUIDs = GetUUIDsForAttribute(aDBObject, tempTypeAttribute.BackwardEdgeDefinition.GetTypeAndAttributeInformation(_DBContext.DBTypeManager).Item2, _DBContext.DBTypeManager.GetTypeByUUID(aDBObject.TypeUUID));
                    }

                    #endregion
                }
                else
                {
                    #region forward edge handling

                    beStream = _DBObjectCache.LoadDBBackwardEdgeStream(tempTypeAttribute.GetDBType(_DBContext.DBTypeManager), aDBObject.ObjectUUID);

                    if (beStream.Failed())
                    {
                        throw new GraphDBException(new Error_CouldNotLoadBackwardEdge(aDBObject, tempTypeAttribute, beStream.IErrors));
                    }

                    var tempEdgeKey = GetBackwardEdgeKey(myLevelKey, desiredBackwardEdgeLevel, _DBContext);

                    if (!beStream.Value.ContainsBackwardEdge(tempEdgeKey))
                    {
                        return false;
                    }

                    referenceUUIDs = beStream.Value.GetBackwardEdgeUUIDs(tempEdgeKey);
                    referenceType = tempTypeAttribute.GetRelatedType(_DBContext.DBTypeManager);

                    #endregion
                }

                #endregion

                if (referenceUUIDs != null)
                {
                    #region references

                    Exceptional<DBObjectStream> tempDbo = null;
                    Dictionary<ObjectUUID, ADBBaseObject> validUUIDs = new Dictionary<ObjectUUID, ADBBaseObject>();
                    SettingInvalidReferenceHandling invalidReferenceSetting = null;

                    #region process references recursivly

                    foreach (var aReferenceUUID in referenceUUIDs)
                    {
                        tempDbo = _DBObjectCache.LoadDBObjectStream(referenceType, aReferenceUUID);

                        if (!tempDbo.Success())
                        {
                            #region error

                            if (invalidReferenceSetting == null)
                            {
                                invalidReferenceSetting = (SettingInvalidReferenceHandling)_DBContext.DBSettingsManager.GetSetting(SettingInvalidReferenceHandling.UUID, _DBContext, TypesSettingScope.ATTRIBUTE, referenceType, tempTypeAttribute).Value;
                            }

                            switch (invalidReferenceSetting.Behaviour)
                            {
                                case BehaviourOnInvalidReference.ignore:
                                    #region ignore

                                    //insert if the next lower level is 0

                                    if ((myLevelKey.Level - currentBackwardResolution) == 0)
                                    {
                                        if (currentBackwardResolution <= backwardResolutiondepth)
                                        {
                                            #region fill graph

                                            LevelKey newLevelKey = new LevelKey(myLevelKey.Edges.First().TypeUUID, _DBContext.DBTypeManager);

                                            if (currentBackwardResolution > 0)
                                            {
                                                //we have to add forwardEdges and (if not already there) add nodes
                                                AddEmptyLevel(newLevelKey);
                                                Levels[0].AddNode(newLevelKey, new ExpressionNode(aReferenceUUID, null));
                                                Levels[0].AddForwardEdgeToNode(newLevelKey, aReferenceUUID, new EdgeKey(myLevelKey.Edges[0].TypeUUID, myLevelKey.Edges[0].AttrUUID), aDBObject.ObjectUUID, null);

                                            }
                                            else
                                            {
                                                //we are in the lowest level and are the first time in this method... so there's no need for adding forward-edges to nodes
                                                AddEmptyLevel(newLevelKey);
                                                Levels[0].AddNode(newLevelKey, new ExpressionNode(aReferenceUUID, null));
                                            }

                                            #endregion
                                        }

                                        //add to valid uuids be
                                        validUUIDs.Add(tempDbo.Value.ObjectUUID, null);
                                    }

                                    #endregion
                                    break;
                                case BehaviourOnInvalidReference.log:

                                    AddWarning(new Warning_EdgeToNonExistingNode(aDBObject, tempTypeAttribute.GetDBType(_DBContext.DBTypeManager), tempTypeAttribute, tempDbo.IErrors));

                                    break;

                                default:

                                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace()));
                            }

                            #endregion
                        }
                        else
                        {
                            if (AddNodeIfValid(tempDbo.Value, myLevelKey, currentBackwardResolution + 1, aDBObject.ObjectUUID, backwardResolutiondepth))
                            {
                                validUUIDs.Add(tempDbo.Value.ObjectUUID, null);
                            }
                        }
                    }

                    #endregion

                    if (validUUIDs.Count > 0)
                    {
                        //some valid uuids
                        if (currentBackwardResolution <= backwardResolutiondepth)
                        {
                            #region fill graph

                            FillGraph(aDBObject, myLevelKey, currentBackwardResolution, source, myLevelKey.Edges[desiredBackwardEdgeLevel], validUUIDs);

                            #endregion
                        }

                        return true;
                    }
                    else
                    {
                        //not valid
                        return false;
                    }

                    #endregion
                }
                else
                {
                    #region no references

                    return false;

                    #endregion
                }
                #endregion
            }
            else
            {
                #region Level 0

                if (currentBackwardResolution <= backwardResolutiondepth)
                {
                    #region fill graph

                    LevelKey newLevelKey = new LevelKey(myLevelKey.Edges.First().TypeUUID, _DBContext.DBTypeManager);

                    if (currentBackwardResolution > 0)
                    {
                        //we have to add forwardEdges and (if not already there) add nodes
                        AddEmptyLevel(newLevelKey);
                        Levels[0].AddNode(newLevelKey, new ExpressionNode(aDBObject, null));
                        Levels[0].AddForwardEdgeToNode(newLevelKey, aDBObject.ObjectUUID, new EdgeKey(myLevelKey.Edges[0].TypeUUID, myLevelKey.Edges[0].AttrUUID), source, null);

                    }
                    else
                    {
                        //we are in the lowest level and are the first time in this method... so there's no need for adding forward-edges to nodes
                        AddEmptyLevel(newLevelKey);
                        Levels[0].AddNode(newLevelKey, new ExpressionNode(aDBObject, null));
                    }

                    #endregion
                }

                return true;

                #endregion
            }

        }

        private void FillGraph(DBObjectStream aDBObject, LevelKey myPath, int currentBackwardResolution, ObjectUUID source, EdgeKey tempEdgeKey, Dictionary<ObjectUUID, ADBBaseObject> validUUIDs)
        {
            lock (_Levels)
            {

                if (currentBackwardResolution == 0)
                {
                    #region Top level

                    //there is no need for forward edges, because we are in the maximum level
                    AddEmptyLevel(myPath);

                    if (!_Levels[myPath.Level].ExpressionLevels[myPath].Nodes.ContainsKey(aDBObject.ObjectUUID))
                    {
                        _Levels[myPath.Level].ExpressionLevels[myPath].Nodes.Add(aDBObject.ObjectUUID, new ExpressionNode(aDBObject, null));
                    }

                    _Levels[myPath.Level].AddBackwardEdgesToNode(myPath, aDBObject.ObjectUUID, GetCorrectBackwardEdge(myPath), validUUIDs);

                    #endregion
                }
                else
                {
                    #region Top level - 1

                    int desiredLevel = myPath.Level - currentBackwardResolution;

                    LevelKey desiredLevelKey = new LevelKey(myPath.Edges.Take(desiredLevel), _DBContext.DBTypeManager);
                    AddEmptyLevel(desiredLevelKey);

                    _Levels[desiredLevel].AddForwardEdgeToNode(desiredLevelKey, aDBObject.ObjectUUID, new EdgeKey(myPath.Edges[desiredLevel].TypeUUID, myPath.Edges[desiredLevel].AttrUUID), source, null);

                    #endregion
                }
            }
        }

        private EdgeKey GetCorrectBackwardEdge(LevelKey myPath)
        {
            switch (myPath.Level)
            {
                case 1:

                    return new EdgeKey(myPath.Edges[0].TypeUUID, null);

                case 0:

                    throw new GraphDBException(new Error_ExpressionGraphInternal(new System.Diagnostics.StackTrace(true), "It is not possible to get a BackwardEdge from a level 0 LevelKey"));

                default:

                    return myPath.Edges[myPath.Level - 2];
            }
        }

        private void MergeGraphs(IExpressionGraph destinationGraph, int maxDestLevel, IExpressionGraph sourceGraph, int maxSourceLevel)
        {
            #region merge up to the limit of the sourceGraph

            foreach (var aLevel in sourceGraph.Levels.OrderBy(item => item.Key))
            {
                foreach (var aLevelPayload in aLevel.Value.ExpressionLevels)
                {
                    //create empty level
                    destinationGraph.AddEmptyLevel(aLevelPayload.Key);

                    foreach (var aNode in aLevelPayload.Value.Nodes)
                    {
                        MergeNodeIntoGraph(destinationGraph, aLevelPayload.Key, aNode.Value, sourceGraph);
                    }
                }
            }

            #endregion
        }

        private void MergeNodeIntoGraph(IExpressionGraph destinationGraph, LevelKey levelKey, IExpressionNode aNode, IExpressionGraph sourceGraph)
        {
            if (destinationGraph.Levels[levelKey.Level].ExpressionLevels[levelKey].Nodes.ContainsKey(aNode.GetObjectUUID()))
            {
                if (levelKey.Level != 0)
                {
                    //check if the node has backward edes
                    if ((aNode.BackwardEdges.Count != 0) || (destinationGraph.Levels[levelKey.Level].ExpressionLevels[levelKey].Nodes[aNode.GetObjectUUID()].BackwardEdges.Count != 0))
                    {
                        //check if the node has backward edes
                        destinationGraph.Levels[levelKey.Level].AddNode(levelKey, aNode);
                    }
                }
                else
                {
                    //nothing has to be checked
                    destinationGraph.Levels[levelKey.Level].AddNode(levelKey, aNode);
                }
            }
            else
            {
                //the node does not exist
                if (levelKey.Level != 0)
                {
                    if (aNode.BackwardEdges.Exists(item => item.Value.Count != 0))
                    {
                        //check if the node has backward edes
                        destinationGraph.Levels[levelKey.Level].AddNode(levelKey, aNode);
                    }
                    else
                    {
                        //remove the backward edge from the upper level
                        RemoveReferenceFromUpperLevel(aNode, levelKey, sourceGraph);
                    }
                }
                else
                {
                    //nothing has to be checked
                    destinationGraph.Levels[levelKey.Level].AddNode(levelKey, aNode);
                }
            }
        }

        private void CleanGraphUp(IExpressionGraph toBeCleanedGraph, IExpressionGraph referenceGraph, int lowerBound, int upperBound)
        {
            foreach (var aLevel in toBeCleanedGraph.Levels.Where(item => item.Key > lowerBound && item.Key <= upperBound).OrderBy(item => item.Key))
            {
                CleanLevel(toBeCleanedGraph, referenceGraph, aLevel, null);
            }
        }

        private void CleanGraphDown(IExpressionGraph toBeCleanedGraph, int toBeCleanedLevel, IExpressionGraph referenceGraph, HashSet<LevelKey> integratedByAnOtherGraph)
        {
            foreach (var aLevel in toBeCleanedGraph.Levels.Where(item => item.Key >= 0 && item.Key <= toBeCleanedLevel).OrderBy(item => item.Key))
            {
                CleanLevel(toBeCleanedGraph, referenceGraph, aLevel, integratedByAnOtherGraph);
            }
        }

        private void CleanLevel(IExpressionGraph toBeCleanedGraph, IExpressionGraph referenceGraph, KeyValuePair<int, IExpressionLevel> aLevel, HashSet<LevelKey> integratedByAnOtherGraph)
        {
            foreach (var aLevelKeyPayload in aLevel.Value.ExpressionLevels)
            {
                List<ObjectUUID> toBeDeletedNodes = new List<ObjectUUID>();

                //check if the level exists in the other graph
                if (referenceGraph.Levels.ContainsKey(aLevel.Key) && referenceGraph.Levels[aLevel.Key].ExpressionLevels.ContainsKey(aLevelKeyPayload.Key))
                {
                    foreach (var aNode in aLevelKeyPayload.Value.Nodes)
                    {
                        if (!referenceGraph.Levels[aLevel.Key].ExpressionLevels[aLevelKeyPayload.Key].Nodes.ContainsKey(aNode.Key))
                        {
                            //the other graph does not contain the current node from this
                            RemoveNodeReferncesFromGraph(aNode.Value, aLevelKeyPayload.Key, toBeCleanedGraph, integratedByAnOtherGraph);
                            toBeDeletedNodes.Add(aNode.Key);
                        }
                    }
                }

                foreach (var aNode in toBeDeletedNodes)
                {
                    #region remove from current level

                    toBeCleanedGraph.Levels[aLevelKeyPayload.Key.Level].RemoveNode(aLevelKeyPayload.Key, aNode);

                    #endregion
                }
            }
        }

        private void RemoveNodeReferncesFromGraph(IExpressionNode myExpressionNode, LevelKey mylevelKey, IExpressionGraph myGraph, HashSet<LevelKey> integratedByAnOtherGraph)
        {
            #region remove complex connection

            foreach (var aComplexConnection in myExpressionNode.ComplexConnection)
            {
                var expressionLevelEntry = myGraph.Levels[aComplexConnection.Key.Level].ExpressionLevels[aComplexConnection.Key];

                foreach (var aReference in aComplexConnection.Value)
                {
                    if (expressionLevelEntry.Nodes.ContainsKey(aReference))
                    {
                        expressionLevelEntry.Nodes[aReference].RemoveComplexConnection(mylevelKey, myExpressionNode.GetObjectUUID());
                        RemoveNodeReferncesFromGraph(expressionLevelEntry.Nodes[aReference], aComplexConnection.Key, myGraph, integratedByAnOtherGraph);
                    }

                    if (myGraph.Levels.ContainsKey(aComplexConnection.Key.Level - 1))
                    {
                        foreach (var aBackwardEdgeSet in expressionLevelEntry.Nodes[aReference].BackwardEdges)
                        {
                            //go to every object the backwardEdge points to and remove the forward reference

                            var backwardLevelKey = GetBackwardLevelKey(aComplexConnection.Key, aBackwardEdgeSet.Key, _DBContext.DBTypeManager);

                            if (myGraph.ContainsLevelKey(backwardLevelKey))
                            {
                                foreach (var aBackwardEdge in aBackwardEdgeSet.Value)
                                {
                                    if (myGraph.Levels[backwardLevelKey.Level].ExpressionLevels[backwardLevelKey].Nodes.ContainsKey(aBackwardEdge.Destination))
                                    {
                                        myGraph.Levels[backwardLevelKey.Level].RemoveNode(backwardLevelKey, aBackwardEdge.Destination);
                                    }
                                }
                            }
                        }
                    }



                    myGraph.Levels[aComplexConnection.Key.Level].RemoveNode(aComplexConnection.Key, aReference);
                }
            }

            #endregion


            #region remove reference from lower level

            if (mylevelKey.Level != 0)
            {
                if (myGraph.Levels.ContainsKey(mylevelKey.Level - 1))
                {
                    foreach (var aBackwardEdgeSet in myExpressionNode.BackwardEdges)
                    {
                        //go to every object the backwardEdge points to and remove the forward reference

                        var backwardLevelKey = GetBackwardLevelKey(mylevelKey, aBackwardEdgeSet.Key, _DBContext.DBTypeManager);

                        if (myGraph.ContainsLevelKey(backwardLevelKey))
                        {
                            foreach (var aBackwardEdge in aBackwardEdgeSet.Value)
                            {
                                if (myGraph.Levels[backwardLevelKey.Level].ExpressionLevels[backwardLevelKey].Nodes.ContainsKey(aBackwardEdge.Destination))
                                {
                                    myGraph.Levels[backwardLevelKey.Level].ExpressionLevels[backwardLevelKey].Nodes[aBackwardEdge.Destination].RemoveForwardEdge(mylevelKey.LastEdge, myExpressionNode.GetObjectUUID());
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            RemoveReferenceFromUpperLevel(myExpressionNode, mylevelKey, myGraph);
        }

        private void RemoveReferenceFromUpperLevel(IExpressionNode myExpressionNode, LevelKey mylevelKey, IExpressionGraph myGraph)
        {
            #region remove reference from upper level

            if (myGraph.Levels.ContainsKey(mylevelKey.Level + 1))
            {
                foreach (var aForwardEdgeSet in myExpressionNode.ForwardEdges)
                {
                    var forwardLevelKey = GetForwardLevelKey(mylevelKey, aForwardEdgeSet.Key, _DBContext.DBTypeManager);

                    if (myGraph.ContainsLevelKey(forwardLevelKey))
                    {
                        //go to every object the forwardEdge points to and remove the backward reference
                        foreach (var aForwardEdge in aForwardEdgeSet.Value)
                        {
                            if (myGraph.Levels[forwardLevelKey.Level].ExpressionLevels[forwardLevelKey].Nodes.ContainsKey(aForwardEdge.Destination))
                            {
                                myGraph.Levels[forwardLevelKey.Level].ExpressionLevels[forwardLevelKey].Nodes[aForwardEdge.Destination].RemoveBackwardEdge(mylevelKey.LastEdge, myExpressionNode.GetObjectUUID());

                                if (!myGraph.Levels[forwardLevelKey.Level].ExpressionLevels[forwardLevelKey].Nodes[aForwardEdge.Destination].BackwardEdges.Exists(item => item.Value.Count > 0))
                                {
                                    //the current object has no backward Edges... delete it

                                    //remove upper references
                                    RemoveReferenceFromUpperLevel(myGraph.Levels[forwardLevelKey.Level].ExpressionLevels[forwardLevelKey].Nodes[aForwardEdge.Destination], forwardLevelKey, myGraph);

                                    //remove the node itself
                                    myGraph.Levels[forwardLevelKey.Level].ExpressionLevels[forwardLevelKey].Nodes.Remove(aForwardEdge.Destination);
                                }
                            }
                        }
                    }
                }
            }

            #endregion
        }

        private LevelKey GetForwardLevelKey(LevelKey mylevelKey, EdgeKey edgeKey, DBTypeManager myTypeManager)
        {
            if (mylevelKey.Level == 0)
            {
                return new LevelKey(new List<EdgeKey> { edgeKey }, myTypeManager);
            }
            else
            {
                List<EdgeKey> newEdges = new List<EdgeKey>();

                newEdges.AddRange(mylevelKey.Edges);
                newEdges.Add(edgeKey);

                return new LevelKey(newEdges, myTypeManager);
            }
        }

        private LevelKey GetBackwardLevelKey(LevelKey mylevelKey, EdgeKey edgeKey, DBTypeManager myTypeManager)
        {
            if (mylevelKey.Level == 1)
            {
                return new LevelKey(new List<EdgeKey> { edgeKey }, myTypeManager);
            }
            else
            {
                List<EdgeKey> newEdges = new List<EdgeKey>();

                newEdges.AddRange(mylevelKey.Edges.Take(mylevelKey.Level - 2));
                newEdges.Add(edgeKey);

                return new LevelKey(newEdges, myTypeManager);
            }

            throw new NotImplementedException();
        }

        private void DownFillGraph(IExpressionGraph aGraph, HashSet<LevelKey> myLevelKeys, int myMinLevel)
        {
            lock (aGraph)
            {
                foreach (var aLevel in myLevelKeys)
                {
                    DownfillLevelKey(aGraph, aLevel);
                }

                //find levelkeys in upper levels that are not compatible to myLevelKeys --> DownFill them too

                var upperLevels = (from aLevel in aGraph.Levels where aLevel.Key > myMinLevel select aLevel.Key).OrderBy(item => item);
                foreach (var aUpperLevel in upperLevels)
                {

                    var upperLevelKeys = aGraph.Levels[aUpperLevel].ExpressionLevels.Where(longerItem => myLevelKeys.Exists(shorterItem => IsValidLevelKeyNeighbourship(shorterItem, longerItem.Key)) == false).Select(item => item.Key);

                    if (upperLevelKeys.Count() > 0)
                    {
                        DownFillGraph(aGraph, new HashSet<LevelKey>(upperLevelKeys), aUpperLevel);
                        break;
                    }
                }
            }
        }

        private void DownfillLevelKey(IExpressionGraph aGraph, LevelKey aLevel)
        {
            lock (aGraph)
            {
                DownFillStructureOfGraph(aGraph, aLevel);

                #region get levelKeys that match

                var lowerLevelKeys = ExtractLowerLevelKeys(aLevel.Level - 1, aLevel, aGraph);

                #endregion

                if (lowerLevelKeys != null)
                {
                    foreach (var aNode in aGraph.Levels[aLevel.Level].ExpressionLevels[aLevel].Nodes)
                    {
                        #region update levels that are lower (e.g. 1(current)-->0)

                        if (lowerLevelKeys != null)
                        {
                            UpdateLowerLevels(aNode.Value, aLevel, lowerLevelKeys, aGraph);
                        }

                        #endregion

                    }
                }
            }
        }

        private HashSet<LevelKey> GetMinLevelKeys(IExpressionGraph myGraph, int minLevel)
        {
            return new HashSet<LevelKey>(myGraph.Levels[minLevel].ExpressionLevels.Keys);
        }

        private void DownFillStructureOfGraph(IExpressionGraph anotherGraph, LevelKey levelKey)
        {
            lock (anotherGraph)
            {
                if (levelKey.Level > 0)
                {
                    var nextLowerLevel = levelKey.Level - 1;
                    var nextLowerLevelKey = levelKey.GetPredecessorLevel(_DBContext.DBTypeManager);

                    if (anotherGraph.Levels.ContainsKey(nextLowerLevel))
                    {
                        if (!anotherGraph.Levels[nextLowerLevel].ExpressionLevels.ContainsKey(nextLowerLevelKey))
                        {
                            anotherGraph.Levels[nextLowerLevel].AddEmptyLevelKey(nextLowerLevelKey);

                            if (nextLowerLevel > 0)
                            {
                                DownFillStructureOfGraph(anotherGraph, nextLowerLevelKey.GetPredecessorLevel(_DBContext.DBTypeManager));
                            }
                        }
                    }
                    else
                    {
                        anotherGraph.Levels.Add(nextLowerLevel, new ExpressionLevel());
                        anotherGraph.Levels[nextLowerLevel].AddEmptyLevelKey(nextLowerLevelKey);

                        if (nextLowerLevel > 0)
                        {
                            DownFillStructureOfGraph(anotherGraph, nextLowerLevelKey);
                        }
                    }
                }
            }
        }

        private int GetPreviousLevel(int p, Dictionary<int, IExpressionLevel> myLevels)
        {
            var lowerLevels = from aLevel in myLevels where aLevel.Key < p select aLevel;

            if (lowerLevels.Count() > 0)
            {
                return lowerLevels.Max(item => item.Key);
            }
            else
            {
                return -1;
            }
        }

        private void UpdateLowerLevels(IExpressionNode myNode, LevelKey myCurrentLevelKey, IEnumerable<LevelKey> myLowerLevelKeys, IExpressionGraph myGraph)
        {
            if (myCurrentLevelKey.Level > 0)
            {
                lock (myGraph)
                {
                    //iterate the next lower LevelKeys
                    foreach (var aLowerLevelKey in myLowerLevelKeys)
                    {
                        #region data

                        //get next lower attribute (might be more than one step away)
                        int levelDistance = myCurrentLevelKey.Level - aLowerLevelKey.Level;
                        //GraphDBType currentType = null;
                        EdgeKey myCurrentBackwardEdgekey = null;
                        TypeAttribute currentAttribute = null;
                        SettingInvalidReferenceHandling invalidReferenceSetting = null;

                        #endregion

                        if (levelDistance >= 1)
                        {
                            if (myCurrentLevelKey.Level > 1)
                            {
                                myCurrentBackwardEdgekey = myCurrentLevelKey.Edges[myCurrentLevelKey.Level - 1];
                            }
                            else
                            {
                                myCurrentBackwardEdgekey = myCurrentLevelKey.Edges[0];
                            }
                        }
                        else
                        {
                            throw new GraphDBException(new Error_ExpressionGraphInternal(new System.Diagnostics.StackTrace(true), "Distances below 1 are not valid."));
                        }

                        IEnumerable<ObjectUUID> referencedUUIDs = null;
                        GraphDBType referencedType = null;
                        currentAttribute = _DBContext.DBTypeManager.GetTypeByUUID(myCurrentBackwardEdgekey.TypeUUID).GetTypeAttributeByUUID(myCurrentBackwardEdgekey.AttrUUID);

                        if (currentAttribute.IsBackwardEdge)
                        {
                            var backwardEdgeTypeInfo = currentAttribute.BackwardEdgeDefinition.GetTypeAndAttributeInformation(_DBContext.DBTypeManager);

                            var dbObjectStream = myNode.GetDBObjectStream(_DBObjectCache, backwardEdgeTypeInfo.Item1.UUID);

                            referencedType = backwardEdgeTypeInfo.Item2.GetDBType(_DBContext.DBTypeManager);

                            if (dbObjectStream.HasAttribute(backwardEdgeTypeInfo.Item2.UUID, backwardEdgeTypeInfo.Item1))
                            {
                                referencedUUIDs = GetUUIDsForAttribute(dbObjectStream, backwardEdgeTypeInfo.Item2, backwardEdgeTypeInfo.Item1);
                            }
                        }
                        else
                        {
                            referencedType = currentAttribute.GetRelatedType(_DBContext.DBTypeManager);

                            if (myNode.BackwardEdges.ContainsKey(aLowerLevelKey.LastEdge))
                            {
                                //take the edges that are already available

                                referencedUUIDs = myNode.BackwardEdges[aLowerLevelKey.LastEdge].Select(item => item.Destination);
                            }
                            else
                            {
                                //load the backward edge stream
                                var currentBackwardEdgeStream = _DBObjectCache.LoadDBBackwardEdgeStream(currentAttribute.GetDBType(_DBContext.DBTypeManager), myNode.GetObjectUUID());

                                if (currentBackwardEdgeStream.Failed())
                                {
                                    throw new GraphDBException(new Error_CouldNotLoadBackwardEdge(myNode.GetDBObjectStream(_DBObjectCache, currentAttribute.GetRelatedType(_DBContext.DBTypeManager).UUID), currentAttribute, currentBackwardEdgeStream.IErrors));
                                }

                                if (currentBackwardEdgeStream.Value.ContainsBackwardEdge(myCurrentBackwardEdgekey))
                                {
                                    referencedUUIDs = currentBackwardEdgeStream.Value.GetBackwardEdgeUUIDs(myCurrentBackwardEdgekey);
                                }

                            }
                        }

                        if (referencedUUIDs != null)
                        {
                            var lowerLevelKeys = ExtractLowerLevelKeys(GetPreviousLevel(aLowerLevelKey.Level, myGraph.Levels), aLowerLevelKey, myGraph);

                            EdgeKey edgeKeyForBackwardEdge = null;
                            //get edgeKey for backwardEdge
                            if (myCurrentLevelKey.Level == 1)
                            {
                                edgeKeyForBackwardEdge = aLowerLevelKey.LastEdge;
                            }
                            else
                            {
                                edgeKeyForBackwardEdge = aLowerLevelKey.Edges[aLowerLevelKey.Level - 1];
                            }

                            Exceptional<DBObjectStream> referencedDBObject = null;

                            foreach (var aReferenceObjectUUID in referencedUUIDs)
                            {
                                referencedDBObject = _DBObjectCache.LoadDBObjectStream(referencedType, aReferenceObjectUUID);

                                if (!referencedDBObject.Success())
                                {
                                    #region error

                                    if (invalidReferenceSetting == null)
                                    {
                                        invalidReferenceSetting = (SettingInvalidReferenceHandling)_DBContext.DBSettingsManager.GetSetting(SettingInvalidReferenceHandling.UUID, _DBContext, Enums.TypesSettingScope.ATTRIBUTE, referencedType, currentAttribute).Value;
                                    }

                                    switch (invalidReferenceSetting.Behaviour)
                                    {
                                        case BehaviourOnInvalidReference.ignore:
                                            #region ignore
                                            
                                            //set lower levelKeys to null because it is not possible to go any lower
                                            AddNodeRecursiveBackward(aReferenceObjectUUID, myNode.GetObjectUUID(), myCurrentLevelKey, aLowerLevelKey, null, myGraph);
                                            myNode.AddBackwardEdge(edgeKeyForBackwardEdge, aReferenceObjectUUID, null);

                                            #endregion
                                            break;
                                        case BehaviourOnInvalidReference.log:

                                            AddWarning(new Warning_EdgeToNonExistingNode(myNode.GetDBObjectStream(_DBObjectCache, referencedType.UUID), currentAttribute.GetDBType(_DBContext.DBTypeManager), currentAttribute, referencedDBObject.IErrors));

                                            break;

                                        default:

                                            throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace()));
                                    }

                                    #endregion

                                }
                                else
                                {
                                    AddNodeRecursiveBackward(aReferenceObjectUUID, myNode.GetObjectUUID(), myCurrentLevelKey, aLowerLevelKey, lowerLevelKeys, myGraph);
                                    myNode.AddBackwardEdge(edgeKeyForBackwardEdge, aReferenceObjectUUID, null);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddWarning(IWarning aWarning)
        {
            lock (_warnings)
            {
                _warnings.Add(aWarning);
            }
        }

        private void AddNodeRecursiveBackward(ObjectUUID myNewObjectUUID, ObjectUUID mySourceUUID, LevelKey mySourceLevelKey, LevelKey myNewNodeLevelKey, List<LevelKey> lowerLevelKeys, IExpressionGraph myGraph)
        {
            lock (myGraph)
            {

                #region add node
                //add node and the node's backwardEdge


                //in this point we are shure that the level reall exists

                if (!myGraph.Levels[myNewNodeLevelKey.Level].ExpressionLevels[myNewNodeLevelKey].Nodes.ContainsKey(myNewObjectUUID))
                {
                    myGraph.Levels[myNewNodeLevelKey.Level].ExpressionLevels[myNewNodeLevelKey].Nodes.Add(myNewObjectUUID, new ExpressionNode(myNewObjectUUID, null));
                }

                myGraph.Levels[myNewNodeLevelKey.Level].AddForwardEdgeToNode(myNewNodeLevelKey, myNewObjectUUID, mySourceLevelKey.LastEdge, mySourceUUID, null);



                #endregion

                #region recursion

                if (lowerLevelKeys != null)
                {
                    UpdateLowerLevels(myGraph.Levels[myNewNodeLevelKey.Level].ExpressionLevels[myNewNodeLevelKey].Nodes[myNewObjectUUID], myNewNodeLevelKey, lowerLevelKeys, myGraph);
                }

                #endregion
            }
        }

        private IEnumerable<Exceptional<DBObjectStream>> GetReferenceObjects(DBObjectStream myStartingDBObject, TypeAttribute interestingAttributeEdge, GraphDBType myStartingDBObjectType)
        {
            if (interestingAttributeEdge.GetDBType(_DBContext.DBTypeManager).IsUserDefined || interestingAttributeEdge.IsBackwardEdge)
            {
                switch (interestingAttributeEdge.KindOfType)
                {
                    case KindsOfType.SingleReference:

                        yield return _DBObjectCache.LoadDBObjectStream(interestingAttributeEdge.GetDBType(_DBContext.DBTypeManager), ((ASingleReferenceEdgeType)myStartingDBObject.GetAttribute(interestingAttributeEdge.UUID)).GetUUID());

                        break;

                    case KindsOfType.SetOfReferences:

                        if (interestingAttributeEdge.IsBackwardEdge)
                        {
                            //get backwardEdge
                            var beStream = _DBObjectCache.LoadDBBackwardEdgeStream(myStartingDBObjectType, myStartingDBObject.ObjectUUID);

                            if (beStream.Failed())
                            {
                                throw new GraphDBException(new Error_CouldNotLoadBackwardEdge(myStartingDBObject, interestingAttributeEdge, beStream.IErrors));
                            }

                            if (beStream.Value.ContainsBackwardEdge(interestingAttributeEdge.BackwardEdgeDefinition))
                            {
                                foreach (var aBackwardEdgeObject in _DBObjectCache.LoadListOfDBObjectStreams(interestingAttributeEdge.BackwardEdgeDefinition.TypeUUID, beStream.Value.GetBackwardEdgeUUIDs(interestingAttributeEdge.BackwardEdgeDefinition)))
                                {
                                    yield return aBackwardEdgeObject;
                                }
                            }
                        }
                        else
                        {

                            foreach (var aDBO in ((ASetOfReferencesEdgeType)myStartingDBObject.GetAttribute(interestingAttributeEdge.UUID)).GetAllEdgeDestinations(_DBContext.DBObjectCache))
                            {
                                yield return aDBO;
                            } 

                            //foreach (var aDBOStream in _DBObjectCache.LoadListOfDBObjectStreams(interestingAttributeEdge.GetDBType(_DBContext.DBTypeManager), ((ASetReferenceEdgeType)myStartingDBObject.GetAttribute(interestingAttributeEdge.UUID)).GetAllReferenceIDs()))
                            //{
                            //    yield return aDBOStream;
                            //}
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

        private bool IsValidLevelKeyNeighbourship(LevelKey myShorterLevelKey, LevelKey myLongerLevelKey)
        {
            if (myShorterLevelKey.Level <= myLongerLevelKey.Level)
            {
                if (myShorterLevelKey.Level == 0)
                {
                    if (myLongerLevelKey.Level == 0)
                    {
                        if (myLongerLevelKey == myShorterLevelKey)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (myLongerLevelKey.Edges[0].TypeUUID == myShorterLevelKey.Edges[0].TypeUUID)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    int counter = 0;
                    foreach (var aEntry in myLongerLevelKey.Edges.Take(myShorterLevelKey.Level))
                    {
                        if (aEntry != myShorterLevelKey.Edges[counter])
                        {
                            return false;
                        }

                        counter++;
                    }

                    return true;

                }
            }
            else
            {
                return false;
            }
        }

        private List<LevelKey> ExtractLowerLevelKeys(int lowerLevel, LevelKey currentLevelKey, IExpressionGraph aGraph)
        {
            if ((lowerLevel < currentLevelKey.Level) && (lowerLevel >= 0))
            {
                var lowerLevelKeys = (from aLowerExpressionLevel in aGraph.Levels[lowerLevel].ExpressionLevels
                                      where IsValidLevelKeyNeighbourship(aLowerExpressionLevel.Key, currentLevelKey)
                                      select aLowerExpressionLevel.Key);

                if (lowerLevelKeys.Count() > 0)
                {
                    return lowerLevelKeys.ToList();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private void UpfillGraph(IExpressionGraph anotherGraph, Dictionary<int, HashSet<LevelKey>> diffForAnotherGraph)
        {
            lock (anotherGraph)
            {
                if ((diffForAnotherGraph != null) && (diffForAnotherGraph.Count != 0))
                {
                    foreach (var aLevel in diffForAnotherGraph.OrderBy(item => item.Key))
                    {
                        foreach (var aLevelKey in aLevel.Value)
                        {
                            if (aLevelKey.Level > 0)
                            {
                                //find the LevelKey that is below aLevelKey
                                var lowerLevel = GetNextLowerLevel(anotherGraph, aLevelKey);

                                ExtendGraphUp(lowerLevel, aLevelKey, anotherGraph);
                            }
                        }
                    }
                }
            }
        }

        private void ExtendGraphUp(LevelKey startLevelKey, LevelKey endLevelKey, IExpressionGraph aGraph)
        {
            if (startLevelKey != endLevelKey)
            {
                var nextHigherLevelKeys = (from aHigherExpressionLevel in aGraph.Levels[startLevelKey.Level + 1].ExpressionLevels
                                           where IsValidLevelKeyNeighbourship(startLevelKey, aHigherExpressionLevel.Key)
                                           select aHigherExpressionLevel.Key);
                var nextHigherLevelKey = (from aLowerExpressionLevel in nextHigherLevelKeys where IsValidLevelKeyNeighbourship(aLowerExpressionLevel, endLevelKey) select aLowerExpressionLevel).FirstOrDefault();


                DBObjectStream currentDBObject = null;
                SettingInvalidReferenceHandling invalidReferenceSetting = null;
                EdgeKey myCurrentForwardEdgekey = nextHigherLevelKey.Edges[startLevelKey.Level];
                GraphDBType currentType = _DBContext.DBTypeManager.GetTypeByUUID(myCurrentForwardEdgekey.TypeUUID);
                TypeAttribute interestingAttribute = currentType.GetTypeAttributeByUUID(myCurrentForwardEdgekey.AttrUUID);

                //find out whats the real type of the referenced objects
                var typeOfReferencedObjects = GetTypeOfAttribute(currentType, interestingAttribute);

                //Extend graph
                foreach (var aNode in aGraph.Levels[startLevelKey.Level].ExpressionLevels[startLevelKey].Nodes)
                {
                    currentDBObject = aNode.Value.GetDBObjectStream(_DBObjectCache, currentType.UUID);

                    if (currentDBObject != null)
                    {
                        //there is no need to extend the graph if there is no DBOBJECTStream available

                        if (currentDBObject.HasAttribute(interestingAttribute.UUID, currentType) || interestingAttribute.IsBackwardEdge)
                        {
                            #region process referenced objects

                            foreach (ObjectUUID aReferenceUUID in GetUUIDsForAttribute(currentDBObject, interestingAttribute, currentType))
                            {
                                var aReferenceStream = _DBObjectCache.LoadDBObjectStream(typeOfReferencedObjects, aReferenceUUID);

                                if (aReferenceStream.Failed())
                                {
                                    if (invalidReferenceSetting == null)
                                    {
                                        invalidReferenceSetting = (SettingInvalidReferenceHandling)_DBContext.DBSettingsManager.GetSetting(SettingInvalidReferenceHandling.UUID, _DBContext, Enums.TypesSettingScope.ATTRIBUTE, typeOfReferencedObjects, interestingAttribute).Value;
                                    }

                                    switch (invalidReferenceSetting.Behaviour)
                                    {
                                        case BehaviourOnInvalidReference.ignore:
                                            #region ignore
                                            //add the nodes as ObjectUUIDs and don't go any deeper

                                            //add backwardEdge to node (and itself)
                                            aGraph.Levels[nextHigherLevelKey.Level].AddNodeAndBackwardEdge(nextHigherLevelKey, aReferenceUUID, startLevelKey.LastEdge, currentDBObject.ObjectUUID, null, null);

                                            //no recursion
                                            aNode.Value.AddForwardEdge(myCurrentForwardEdgekey, aReferenceUUID, null);

                                            #endregion
                                            break;
                                        case BehaviourOnInvalidReference.log:

                                            AddWarning(new Warning_EdgeToNonExistingNode(currentDBObject, interestingAttribute.GetDBType(_DBContext.DBTypeManager), interestingAttribute, aReferenceStream.IErrors));

                                            break;

                                        default:

                                            throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace()));
                                    }
                                }
                                else
                                {
                                    //add backwardEdge to node (and itself)
                                    aGraph.Levels[nextHigherLevelKey.Level].AddNodeAndBackwardEdge(nextHigherLevelKey, aReferenceStream.Value, startLevelKey.LastEdge, currentDBObject.ObjectUUID, null, null);

                                    //recursion
                                    ExtendGraphUp(nextHigherLevelKey, endLevelKey, aGraph);
                                    aNode.Value.AddForwardEdge(myCurrentForwardEdgekey, aReferenceStream.Value.ObjectUUID, null);
                                }
                            }

                            #endregion
                        }
                    }
                }
            }
        }

        private IEnumerable<ObjectUUID> GetUUIDsForAttribute(DBObjectStream currentDBObject, TypeAttribute interestingAttribute, GraphDBType myCurrentDBObjectType)
        {
            switch (interestingAttribute.KindOfType)
            {
                case KindsOfType.SingleReference:
                case KindsOfType.SetOfReferences:

                    if (interestingAttribute.IsBackwardEdge)
                    {
                        //get backwardEdge
                        var beStream = _DBObjectCache.LoadDBBackwardEdgeStream(myCurrentDBObjectType, currentDBObject.ObjectUUID);

                        if (beStream.Failed())
                        {
                            throw new GraphDBException(new Error_CouldNotLoadBackwardEdge(currentDBObject, interestingAttribute, beStream.IErrors));
                        }

                        if (beStream.Value.ContainsBackwardEdge(interestingAttribute.BackwardEdgeDefinition))
                        {
                            foreach (var aBackwardEdgeUUID in beStream.Value.GetBackwardEdgeUUIDs(interestingAttribute.BackwardEdgeDefinition))
                            {
                                yield return aBackwardEdgeUUID;
                            }
                        }
                    }
                    else
                    {
                        foreach (var aObjectUUID in ((IReferenceEdge)currentDBObject.GetAttribute(interestingAttribute.UUID)).GetAllReferenceIDs())
                        {
                            yield return aObjectUUID;
                        }
                    }

                    break;

                case KindsOfType.SetOfNoneReferences:
                case KindsOfType.ListOfNoneReferences:
                case KindsOfType.SingleNoneReference:
                default:
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace()));
            }
        }

        private GraphDBType GetTypeOfAttribute(GraphDBType currentType, TypeAttribute interestingAttribute)
        {

            switch (interestingAttribute.KindOfType)
            {
                case KindsOfType.SingleReference:

                    return interestingAttribute.GetDBType(_DBContext.DBTypeManager);

                case KindsOfType.SetOfReferences:

                    if (interestingAttribute.IsBackwardEdge)
                    {
                        return _DBContext.DBTypeManager.GetTypeByUUID(interestingAttribute.BackwardEdgeDefinition.TypeUUID);
                    }
                    else
                    {
                        return interestingAttribute.GetDBType(_DBContext.DBTypeManager);
                    }

                case KindsOfType.SetOfNoneReferences:
                case KindsOfType.ListOfNoneReferences:
                case KindsOfType.SingleNoneReference:

                    return interestingAttribute.GetDBType(_DBContext.DBTypeManager);

                default:
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace()));
            }
        }

        private LevelKey GetNextLowerLevel(IExpressionGraph anotherGraph, LevelKey aLevelKey)
        {
            return (from aLowerExpressionLevel in anotherGraph.Levels[aLevelKey.Level - 1].ExpressionLevels
                    where IsValidLevelKeyNeighbourship(aLowerExpressionLevel.Key, aLevelKey)
                    select aLowerExpressionLevel.Key).OrderByDescending(item => item.Level).Select(item => item).FirstOrDefault();
        }

        private void UpgradeGraphStructure(IExpressionGraph aGraph, Dictionary<int, HashSet<LevelKey>> difference)
        {
            foreach (var aDiffLevel in difference)
            {
                foreach (var aLevelKey in aDiffLevel.Value)
                {
                    aGraph.AddEmptyLevel(aLevelKey);
                }
            }
        }

        private Dictionary<int, HashSet<LevelKey>> GetLevelKeyDifference(IExpressionGraph fromGraph, IExpressionGraph toGraph)
        {
            #region data

            Dictionary<int, HashSet<LevelKey>> result = new Dictionary<int, HashSet<LevelKey>>();

            #endregion

            foreach (var aLevel in toGraph.Levels)
            {
                foreach (var aLevelPayLoad in aLevel.Value.ExpressionLevels)
                {
                    if (!fromGraph.ContainsLevelKey(aLevelPayLoad.Key))
                    {
                        if (result.ContainsKey(aLevel.Key))
                        {
                            result[aLevel.Key].Add(aLevelPayLoad.Key);
                        }
                        else
                        {
                            result.Add(aLevel.Key, new HashSet<LevelKey>() { aLevelPayLoad.Key });
                        }
                    }
                }
            }

            return result;
        }

        private delegate void CalcDownFillGraph(IExpressionGraph aGraph, HashSet<LevelKey> aLevelKeys, int minLevel);
        private delegate void CalcUpFillGraph(IExpressionGraph aGraph, Dictionary<int, HashSet<LevelKey>> diffForAnotherGraph);

        private void UpFillGraphWrapper(IExpressionGraph aGraph, Dictionary<int, HashSet<LevelKey>> diffForAnotherGraph)
        {
            UpgradeGraphStructure(aGraph, diffForAnotherGraph);
            UpfillGraph(aGraph, diffForAnotherGraph);
        }

        private void GenerateLevel(LevelKey myLevelKey)
        {
            lock (_Levels)
            {
                #region check the environment

                var lowerLevel = GetLowerLevelKeys(this, myLevelKey);
                var upperLevelSet = GetUpperLevelKeys(this, myLevelKey);

                #endregion

                if (lowerLevel.Count > 0)
                {
                    if (upperLevelSet.Count > 0)
                    {
                        //there are limits from both sides
                        throw new GraphDBException(new Error_ExpressionGraphInternal(new System.Diagnostics.StackTrace(true), "Its currently not implemented to select a LevelKey from a ExpressionGraph that is surrounded by lower and upper LevelKeys."));

                    }
                    else
                    {
                        //we have to move from the lower to the upper

                        this.AddEmptyLevel(myLevelKey);
                        DownFillStructureOfGraph(this, myLevelKey);

                        foreach (var aLevel in lowerLevel)
                        {
                            ExtendGraphUp(aLevel, myLevelKey, this);
                        }
                    }
                }
                else
                {
                    if (upperLevelSet.Count > 0)
                    {
                        foreach (var aLevel in upperLevelSet)
                        {
                            DownfillLevelKey(this, aLevel);
                        }
                    }
                    else
                    {
                        if (myLevelKey.Level > 0)
                        {
                            foreach (var aLevelKey in GenerateAllLowerLevelKeys(myLevelKey))
                            {
                                GenerateLevel(aLevelKey);
                            }

                            GenerateLevel(myLevelKey);
                        }
                        else
                        {
                            this.AddEmptyLevel(myLevelKey);
                            DownFillStructureOfGraph(this, myLevelKey);

                            #region add first level


                            LevelKey lowestLevelKey = new LevelKey(myLevelKey.Edges[0].TypeUUID, _DBContext.DBTypeManager);

                            GraphDBType lowestType = _DBContext.DBTypeManager.GetTypeByUUID(lowestLevelKey.LastEdge.TypeUUID);
                            var idx = _DBContext.DBTypeManager.GetTypeByUUID(lowestLevelKey.LastEdge.TypeUUID).GetUUIDIndex(_DBContext);
                            var indexRelatedType = _DBContext.DBTypeManager.GetTypeByUUID(idx.IndexRelatedTypeUUID);

                            foreach (var ids in idx.GetAllValues(indexRelatedType, _DBContext))
                            {
                                foreach (var aDBO in _DBObjectCache.LoadListOfDBObjectStreams(lowestType, ids))
                                {
                                    if (aDBO.Failed())
                                    {
                                        AddWarning(new Warning_CouldNotLoadDBObject(aDBO.IErrors, new System.Diagnostics.StackTrace(true)));
                                    }
                                    else
                                    {
                                        this.AddNode(aDBO.Value, lowestLevelKey, 0);
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                }
            }
        }

        private IEnumerable<LevelKey> GenerateAllLowerLevelKeys(LevelKey myLevelKey)
        {
            LevelKey myLevelKeyPred;

            if (myLevelKey.Level > 0)
            {
                do
                {
                    myLevelKeyPred = myLevelKey.GetPredecessorLevel(_DBContext.DBTypeManager);

                    yield return myLevelKeyPred;

                } while (myLevelKeyPred.Level != 0);
            }

            yield break;
        }

        private HashSet<LevelKey> GetUpperLevelKeys(IExpressionGraph aGraph, LevelKey myLevelKey)
        {

            HashSet<LevelKey> result = new HashSet<LevelKey>();

            lock (aGraph)
            {
                foreach (var aLevel in aGraph.Levels.Where(aaLevel => aaLevel.Key > myLevelKey.Level).OrderBy(item => item.Key))
                {
                    foreach (var aLevelPayLoad in aLevel.Value.ExpressionLevels)
                    {
                        if (IsValidLevelKeyNeighbourship(myLevelKey, aLevelPayLoad.Key))
                        {
                            result.Add(aLevelPayLoad.Key);
                        }
                    }

                    if (result.Count > 0)
                    {
                        break;
                    }
                }
            }
            return result;
        }

        private HashSet<LevelKey> GetLowerLevelKeys(IExpressionGraph aGraph, LevelKey myLevelKey)
        {
            HashSet<LevelKey> result = new HashSet<LevelKey>();

            lock (aGraph)
            {
                foreach (var aLevel in aGraph.Levels.Where(aaLevel => aaLevel.Key < myLevelKey.Level).OrderByDescending(item => item.Key))
                {
                    foreach (var aLevelPayLoad in aLevel.Value.ExpressionLevels)
                    {
                        if (IsValidLevelKeyNeighbourship(aLevelPayLoad.Key, myLevelKey))
                        {
                            result.Add(aLevelPayLoad.Key);
                        }
                    }

                    if (result.Count > 0)
                    {
                        break;
                    }
                }
            }
            return result;
        }

        #endregion
    }

}
