using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.TypeSystem;
using sones.Library.PropertyHyperGraph;
using sones.GraphQL.GQL.ErrorHandling;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph.Helper;
using sones.GraphDB.Request;

namespace sones.GraphQL.GQL.Structure.Helper.ExpressionGraph
{
    /// <summary>
    /// This class implements the expression graph.
    /// </summary>
    public sealed class CommonUsageGraph : AExpressionGraph
    {
        #region Properties

        /// <summary>
        /// The graph db instance
        /// </summary>
        private readonly IGraphDB _iGraphDB;

        /// <summary>
        /// The current security token
        /// </summary>
        private readonly SecurityToken _securityToken;

        /// <summary>
        /// The current transaction token
        /// </summary>
        private readonly TransactionToken _transactionToken;

        /// <summary>
        /// The levels of the expression graph
        /// </summary>
        private Dictionary<int, IExpressionLevel> _Levels;

        ///// <summary>
        ///// Es wird ein BackwardEdge level aufgelöst (U.Friends.Friends.Friends.Name = >löst nur ...[BE]Friends.Name auf - aber nicht bis U
        ///// Es wird validiert! Aber nicht im graphen gespeichert.
        ///// </summary>
        //private int _defaultBackwardResolution = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public CommonUsageGraph(IGraphDB myIGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
            : this()
        {
            _iGraphDB = myIGraphDB;
            _securityToken = mySecurityToken;
            _transactionToken = myTransactionToken;
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
        }

        #endregion

        #region Public methods

        public override bool ContainsRelevantLevelForType(IVertexType myType)
        {
            foreach (var aLevel in _Levels)
            {
                foreach (var aLevelContent in aLevel.Value.ExpressionLevels)
                {
                    if (aLevelContent.Key.Edges[0].VertexTypeID == myType.ID)
                    {
                        return true;
                    }
                }
            }

            return false;
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

        public override Boolean IsGraphRelevant(LevelKey myLevelKey, IVertex mySourceDBObject)
        {
            lock (_Levels)
            {
                if (!this.ContainsLevelKey(myLevelKey))
                {
                    return false;
                }

                if (myLevelKey.Level == 0)
                {
                    return this._Levels[myLevelKey.Level].ExpressionLevels[myLevelKey].Nodes.ContainsKey(mySourceDBObject.VertexID);
                }
                else
                {
                    if (mySourceDBObject != null)
                    {
                        var predecessorLevelKey = myLevelKey.GetPredecessorLevel(_iGraphDB, _securityToken, _transactionToken);

                        if (!this.ContainsLevelKey(predecessorLevelKey))
                        {
                            var interestingEdge = new ExpressionEdge(mySourceDBObject.VertexID, null, predecessorLevelKey.LastEdge);
                            //take the backwardEdges 
                            return this._Levels[myLevelKey.Level].ExpressionLevels[myLevelKey].Nodes.Where(item => item.Value.BackwardEdges[predecessorLevelKey.LastEdge].Contains(interestingEdge)).Count() > 0;
                        }
                        else
                        {
                            if (this._Levels[predecessorLevelKey.Level].ExpressionLevels[predecessorLevelKey].Nodes[mySourceDBObject.VertexID].ForwardEdges.ContainsKey(myLevelKey.LastEdge))
                            {
                                if (this._Levels[predecessorLevelKey.Level].ExpressionLevels[predecessorLevelKey].Nodes[mySourceDBObject.VertexID].ForwardEdges[myLevelKey.LastEdge].Count > 0)
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
                        throw new ExpressionGraphInternalException("No IVertex givon for graph relevance test in a higher level.");
                    }
                }
            }

        }

        public override IEnumerable<IVertex> Select(LevelKey myLevelKey, IVertex mySourceDBObject = null, Boolean doLevelGeneration = true)
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
                            yield return aNode.Value.GetIVertex(_iGraphDB, myLevelKey.LastEdge.VertexTypeID, _securityToken, _transactionToken);
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
                        var predecessorLevelKey = myLevelKey.GetPredecessorLevel(_iGraphDB, _securityToken, _transactionToken);

                        if (!this.ContainsLevelKey(predecessorLevelKey))
                        {
                            //take the backwardEdges 
                            foreach (var aNode in this._Levels[myLevelKey.Level].ExpressionLevels[myLevelKey].Nodes.Where(item => item.Value.BackwardEdges[predecessorLevelKey.LastEdge].Where(aBackWardEdge => aBackWardEdge.Destination == mySourceDBObject.VertexID).Count() > 0))
                            {
                                yield return aNode.Value.GetIVertex(_iGraphDB, myLevelKey.LastEdge.VertexTypeID, _securityToken, _transactionToken);
                            }
                        }
                        else
                        {
                            //take the forwardEdges

                            IVertexType aType = _iGraphDB.GetVertexType<IVertexType>(
                                _securityToken,
                                _transactionToken,
                                new GraphDB.Request.RequestGetVertexType(myLevelKey.LastEdge.VertexTypeID),
                                (stats, vertexType) => vertexType);

                            IAttributeDefinition aAttributeDefinition = aType.GetAttributeDefinition(myLevelKey.LastEdge.AttributeID);

                            var interestingType = GetTypeOfAttribute(aType, aAttributeDefinition);

                            var interestingVertexIDs = this._Levels[predecessorLevelKey.Level]
                                .ExpressionLevels[predecessorLevelKey]
                                .Nodes[mySourceDBObject.VertexID]
                                .ForwardEdges[myLevelKey.LastEdge].Select(item => item.Destination);

                            var interestingVertices = _iGraphDB.GetVertices<IEnumerable<IVertex>>(
                                _securityToken,
                                _transactionToken,
                                new RequestGetVertices(interestingType.ID, interestingVertexIDs),
                                (stats, vertices) => vertices);

                            foreach (var aVertex in interestingVertices)
                            {
                                yield return aVertex;
                            }
                        }
                    }
                    else
                    {
                        //there is no sourceObject given, so return the complete level
                        foreach (var aNode in this._Levels[myLevelKey.Level].ExpressionLevels[myLevelKey].Nodes)
                        {
                            yield return aNode.Value.GetIVertex(_iGraphDB, myLevelKey.LastEdge.VertexTypeID, _securityToken, _transactionToken);
                        }
                    }
                }

            }
            //all done 
            yield break;

        }

        public override IEnumerable<Int64> SelectVertexIDs(LevelKey myLevelKey, IVertex mySourceDBObject = null, Boolean doLevelGeneration = true)
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
                            yield return aNode.Value.GetVertexID();
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
                        var predecessorLevelKey = myLevelKey.GetPredecessorLevel(_iGraphDB, _securityToken, _transactionToken);

                        if (!this.ContainsLevelKey(predecessorLevelKey))
                        {
                            //take the backwardEdges 
                            foreach (var aNode in this._Levels[myLevelKey.Level].ExpressionLevels[myLevelKey].Nodes.Where(item => item.Value.BackwardEdges[predecessorLevelKey.LastEdge].Where(aBackWardEdge => aBackWardEdge.Destination == mySourceDBObject.VertexID).Count() > 0))
                            {
                                yield return aNode.Value.GetVertexID();
                            }
                        }
                        else
                        {
                            //take the forwardEdges
                            if (this._Levels[predecessorLevelKey.Level].ExpressionLevels[predecessorLevelKey].Nodes[mySourceDBObject.VertexID].ForwardEdges.ContainsKey(myLevelKey.LastEdge))
                            {
                                foreach (var aUUID in this._Levels[predecessorLevelKey.Level].ExpressionLevels[predecessorLevelKey].Nodes[mySourceDBObject.VertexID].ForwardEdges[myLevelKey.LastEdge].Select(item => item.Destination))
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
                            yield return aNode.Value.GetVertexID();
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

        public override void AddNodesWithComplexRelation(IVertex leftDBObject, LevelKey leftLevelKey, IVertex rightDBObject, LevelKey rightLevelKey, int backwardResolutiondepth)
        {
            lock (_Levels)
            {
                if ((AddNodeIfValid(leftDBObject, leftLevelKey, 0, leftDBObject.VertexID, backwardResolutiondepth)) && (AddNodeIfValid(rightDBObject, rightLevelKey, 0, rightDBObject.VertexID, backwardResolutiondepth)))
                {
                    //both nodes have been inserted correctly
                    //--> create a connection between both
                    _Levels[leftLevelKey.Level].ExpressionLevels[leftLevelKey].Nodes[leftDBObject.VertexID].AddComplexConnection(rightLevelKey, rightDBObject.VertexID);
                    _Levels[rightLevelKey.Level].ExpressionLevels[rightLevelKey].Nodes[rightDBObject.VertexID].AddComplexConnection(leftLevelKey, leftDBObject.VertexID);
                }
                else
                {
                    #region remove both nodes

                    if (ContainsLevelKey(leftLevelKey))
                    {
                        _Levels[leftLevelKey.Level].RemoveNode(leftLevelKey, leftDBObject.VertexID);
                    }
                    if (ContainsLevelKey(rightLevelKey))
                    {
                        _Levels[rightLevelKey.Level].RemoveNode(rightLevelKey, rightDBObject.VertexID);
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

        public override void AddNode(IVertex myIVertex, LevelKey myLevelKey, int backwardResolution)
        {
            AddNodeIfValid(myIVertex, myLevelKey, 0, myIVertex.VertexID, backwardResolution);
        }

        public override void AddNode(IVertex myIVertex, LevelKey myLevelKey)
        {
            AddNode(myIVertex, myLevelKey, 0);
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

        public override IExpressionGraph GetNewInstance(IGraphDB myGraphDB, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            return new ExpressionGraph.CommonUsageGraph(myGraphDB, mySecurityToken, myTransactionToken);
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

            
            DownFillGraph(this, thisMinLevelKeys, thisMinLevel);
            DownFillGraph(anotherGraph, anotherMinLevelKeys, anotherMinLevel);

            #endregion

            #endregion

            #region build level differences

            var diffForThis = GetLevelKeyDifference(this, anotherGraph);
            var diffForAnotherGraph = GetLevelKeyDifference(anotherGraph, this);

            #endregion

            #region upfill

            //upfill levelKey differences of each graph & update Nodes in both graphs
            
            UpgradeGraphStructure(this, diffForThis);
            UpfillGraph(this, diffForThis);

            UpfillGraph(anotherGraph, diffForAnotherGraph);
            UpgradeGraphStructure(anotherGraph, diffForAnotherGraph);
            

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
                List<Int64> toBeDeletedNodes = new List<Int64>();

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

                #region Downfill both graphs

                DownFillGraph(this, thisMinLevelKeys, thisMinLevel);
                DownFillGraph(anotherGraph, anotherMinLevelKeys, anotherMinLevel);

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
        /// This method adds a IVertex to a Level if it is valid for a LevelKey.
        /// </summary>
        /// <param name="aDBObject">The Object that is going to be added</param>
        /// <param name="myLevelKey">The LevelKey which is needed for validation.</param>
        /// <param name="currentBackwardResolution">The current backward resolution (initially 0)</param>
        /// <param name="source">The Int64 of the </param>
        /// <returns>True if it was valid or false otherwise.</returns>
        private bool AddNodeIfValid(IVertex aDBObject, LevelKey myLevelKey, int currentBackwardResolution, Int64 source, int backwardResolutiondepth)
        {
            #region data

            IAttributeDefinition tempTypeAttribute = null;

            IEnumerable<IVertex> referenceUUIDs = null;

            #endregion

            if ((myLevelKey.Level - currentBackwardResolution) > 0)
            {
                #region level > 0

                int desiredBackwardEdgeLevel = myLevelKey.Level - currentBackwardResolution - 1;

                var tempVertexType = _iGraphDB.GetVertexType(
                    _securityToken,
                    _transactionToken,
                    new RequestGetVertexType(myLevelKey.Edges[desiredBackwardEdgeLevel].VertexTypeID),
                    (stats, vertexType) => vertexType);
                
                tempTypeAttribute = tempVertexType.GetAttributeDefinition(myLevelKey.Edges[desiredBackwardEdgeLevel].AttributeID);

                #region get reference UUIDs

                if (tempTypeAttribute.Kind == AttributeType.IncomingEdge)
                {
                    #region backward edge handling

                    var incomingEdgeAttribute = tempTypeAttribute as IIncomingEdgeDefinition;
                    
                    if (aDBObject.HasOutgoingEdge(incomingEdgeAttribute.RelatedEdgeDefinition.AttributeID))
                    {
                        referenceUUIDs = aDBObject.GetOutgoingEdge(incomingEdgeAttribute.RelatedEdgeDefinition.AttributeID).GetTargetVertices();
                        //GetUUIDsForAttribute(aDBObject, incomingEdgeAttribute.RelatedEdgeDefinition, tempTypeAttribute.BackwardEdgeDefinition.GetTypeAndAttributeInformation(_DBContext.DBTypeManager).Item2, _DBContext.DBTypeManager.GetTypeByUUID(aDBObject.TypeUUID));
                    }

                    #endregion
                }
                else
                {
                    #region forward edge handling

                    var tempEdgeKey = GetBackwardEdgeKey(myLevelKey, desiredBackwardEdgeLevel, _iGraphDB, _securityToken, _transactionToken);

                    if (!aDBObject.HasIncomingVertices(tempEdgeKey.VertexTypeID, tempEdgeKey.AttributeID))
                    {
                        return false;
                    }

                    referenceUUIDs = aDBObject.GetIncomingVertices(tempEdgeKey.VertexTypeID, tempEdgeKey.AttributeID);

                    #endregion
                }

                #endregion

                if (referenceUUIDs != null)
                {
                    #region references

                    Dictionary<Int64, IComparable> validUUIDs = new Dictionary<Int64, IComparable>();

                    #region process references recursivly

                    foreach (var aReferenceUUID in referenceUUIDs)
                    {
                        if (AddNodeIfValid(aReferenceUUID, myLevelKey, currentBackwardResolution + 1, aDBObject.VertexID, backwardResolutiondepth))
                        {
                            validUUIDs.Add(aReferenceUUID.VertexID, null);
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

                    LevelKey newLevelKey = new LevelKey(myLevelKey.Edges.First().VertexTypeID, _iGraphDB, _securityToken, _transactionToken);

                    if (currentBackwardResolution > 0)
                    {
                        //we have to add forwardEdges and (if not already there) add nodes
                        AddEmptyLevel(newLevelKey);
                        Levels[0].AddNode(newLevelKey, new ExpressionNode(aDBObject, null));
                        Levels[0].AddForwardEdgeToNode(newLevelKey, aDBObject.VertexID, new EdgeKey(myLevelKey.Edges[0].VertexTypeID, myLevelKey.Edges[0].AttributeID), source, null);

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

        private void FillGraph(IVertex aDBObject, LevelKey myPath, int currentBackwardResolution, Int64 source, EdgeKey tempEdgeKey, Dictionary<Int64, IComparable> validUUIDs)
        {
            lock (_Levels)
            {

                if (currentBackwardResolution == 0)
                {
                    #region Top level

                    //there is no need for forward edges, because we are in the maximum level
                    AddEmptyLevel(myPath);

                    if (!_Levels[myPath.Level].ExpressionLevels[myPath].Nodes.ContainsKey(aDBObject.VertexID))
                    {
                        _Levels[myPath.Level].ExpressionLevels[myPath].Nodes.Add(aDBObject.VertexID, new ExpressionNode(aDBObject, null));
                    }

                    _Levels[myPath.Level].AddBackwardEdgesToNode(myPath, aDBObject.VertexID, GetCorrectBackwardEdge(myPath), validUUIDs);

                    #endregion
                }
                else
                {
                    #region Top level - 1

                    int desiredLevel = myPath.Level - currentBackwardResolution;

                    LevelKey desiredLevelKey = new LevelKey(myPath.Edges.Take(desiredLevel), _iGraphDB, _securityToken, _transactionToken);
                    AddEmptyLevel(desiredLevelKey);

                    _Levels[desiredLevel].AddForwardEdgeToNode(desiredLevelKey, aDBObject.VertexID, new EdgeKey(myPath.Edges[desiredLevel].VertexTypeID, myPath.Edges[desiredLevel].AttributeID), source, null);

                    #endregion
                }
            }
        }

        private EdgeKey GetCorrectBackwardEdge(LevelKey myPath)
        {
            switch (myPath.Level)
            {
                case 1:

                    return new EdgeKey(myPath.Edges[0].VertexTypeID);

                case 0:

                    throw new ExpressionGraphInternalException("It is not possible to get a BackwardEdge from a level 0 LevelKey");

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
            if (destinationGraph.Levels[levelKey.Level].ExpressionLevels[levelKey].Nodes.ContainsKey(aNode.GetVertexID()))
            {
                if (levelKey.Level != 0)
                {
                    //check if the node has backward edes
                    if ((aNode.BackwardEdges.Count != 0) || (destinationGraph.Levels[levelKey.Level].ExpressionLevels[levelKey].Nodes[aNode.GetVertexID()].BackwardEdges.Count != 0))
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
                    if (aNode.BackwardEdges.Where(item => item.Value.Count != 0).Count() > 0)
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
                List<Int64> toBeDeletedNodes = new List<Int64>();

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
                        expressionLevelEntry.Nodes[aReference].RemoveComplexConnection(mylevelKey, myExpressionNode.GetVertexID());
                        RemoveNodeReferncesFromGraph(expressionLevelEntry.Nodes[aReference], aComplexConnection.Key, myGraph, integratedByAnOtherGraph);
                    }

                    if (myGraph.Levels.ContainsKey(aComplexConnection.Key.Level - 1))
                    {
                        foreach (var aBackwardEdgeSet in expressionLevelEntry.Nodes[aReference].BackwardEdges)
                        {
                            //go to every object the backwardEdge points to and remove the forward reference

                            var backwardLevelKey = GetBackwardLevelKey(aComplexConnection.Key, aBackwardEdgeSet.Key);

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

                        var backwardLevelKey = GetBackwardLevelKey(mylevelKey, aBackwardEdgeSet.Key);

                        if (myGraph.ContainsLevelKey(backwardLevelKey))
                        {
                            foreach (var aBackwardEdge in aBackwardEdgeSet.Value)
                            {
                                if (myGraph.Levels[backwardLevelKey.Level].ExpressionLevels[backwardLevelKey].Nodes.ContainsKey(aBackwardEdge.Destination))
                                {
                                    myGraph.Levels[backwardLevelKey.Level].ExpressionLevels[backwardLevelKey].Nodes[aBackwardEdge.Destination].RemoveForwardEdge(mylevelKey.LastEdge, myExpressionNode.GetVertexID());
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
                    var forwardLevelKey = GetForwardLevelKey(mylevelKey, aForwardEdgeSet.Key);

                    if (myGraph.ContainsLevelKey(forwardLevelKey))
                    {
                        //go to every object the forwardEdge points to and remove the backward reference
                        foreach (var aForwardEdge in aForwardEdgeSet.Value)
                        {
                            if (myGraph.Levels[forwardLevelKey.Level].ExpressionLevels[forwardLevelKey].Nodes.ContainsKey(aForwardEdge.Destination))
                            {
                                myGraph.Levels[forwardLevelKey.Level].ExpressionLevels[forwardLevelKey].Nodes[aForwardEdge.Destination].RemoveBackwardEdge(mylevelKey.LastEdge, myExpressionNode.GetVertexID());

                                if (!(myGraph.Levels[forwardLevelKey.Level].ExpressionLevels[forwardLevelKey].Nodes[aForwardEdge.Destination].BackwardEdges
                                    .Where(item => item.Value.Count > 0).Count() > 0))
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

        private LevelKey GetForwardLevelKey(LevelKey mylevelKey, EdgeKey edgeKey)
        {
            if (mylevelKey.Level == 0)
            {
                return new LevelKey(new List<EdgeKey> { edgeKey }, _iGraphDB, _securityToken, _transactionToken);
            }
            else
            {
                List<EdgeKey> newEdges = new List<EdgeKey>();

                newEdges.AddRange(mylevelKey.Edges);
                newEdges.Add(edgeKey);

                return new LevelKey(newEdges, _iGraphDB, _securityToken, _transactionToken);
            }
        }

        private LevelKey GetBackwardLevelKey(LevelKey mylevelKey, EdgeKey edgeKey)
        {
            if (mylevelKey.Level == 1)
            {
                return new LevelKey(new List<EdgeKey> { edgeKey }, _iGraphDB, _securityToken, _transactionToken);
            }
            else
            {
                List<EdgeKey> newEdges = new List<EdgeKey>();

                newEdges.AddRange(mylevelKey.Edges.Take(mylevelKey.Level - 2));
                newEdges.Add(edgeKey);

                return new LevelKey(newEdges, _iGraphDB, _securityToken, _transactionToken);
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
                    List<LevelKey> upperLevelKeys = new List<LevelKey>();

                    foreach (var aExpressionLevel in aGraph.Levels[aUpperLevel].ExpressionLevels)
                    {
                        foreach (var aLevelKey in myLevelKeys)
                        {
                            if (!IsValidLevelKeyNeighbourship(aLevelKey, aExpressionLevel.Key))
	                        {
                                upperLevelKeys.Add(aExpressionLevel.Key);
	                        } 
                        }
                    }


                    if (upperLevelKeys.Count > 0)
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
                    var nextLowerLevelKey = levelKey.GetPredecessorLevel(_iGraphDB, _securityToken, _transactionToken);

                    if (anotherGraph.Levels.ContainsKey(nextLowerLevel))
                    {
                        if (!anotherGraph.Levels[nextLowerLevel].ExpressionLevels.ContainsKey(nextLowerLevelKey))
                        {
                            anotherGraph.Levels[nextLowerLevel].AddEmptyLevelKey(nextLowerLevelKey);

                            if (nextLowerLevel > 0)
                            {
                                DownFillStructureOfGraph(anotherGraph, nextLowerLevelKey.GetPredecessorLevel(_iGraphDB, _securityToken, _transactionToken));
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
                        IAttributeDefinition currentAttribute = null;

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
                            throw new ExpressionGraphInternalException("Distances below 1 are not valid.");
                        }

                        IEnumerable<IVertex> referencedUUIDs = null;

                        IVertexType tempVertexType = _iGraphDB.GetVertexType<IVertexType>(
                            _securityToken,
                            _transactionToken,
                            new RequestGetVertexType(myCurrentBackwardEdgekey.VertexTypeID),
                            (stats, vertexType) => vertexType);

                        currentAttribute = tempVertexType.GetAttributeDefinition(myCurrentBackwardEdgekey.AttributeID);
                        
                        if (currentAttribute.Kind == AttributeType.IncomingEdge)
                        {
                            var incomingAttribite = (IIncomingEdgeDefinition)currentAttribute;

                            var IVertex = myNode.GetIVertex(_iGraphDB, incomingAttribite.RelatedEdgeDefinition.SourceVertexType.ID, _securityToken, _transactionToken);

                            if (IVertex.HasOutgoingEdge(incomingAttribite.RelatedEdgeDefinition.AttributeID))
                            {
                                referencedUUIDs = IVertex.GetOutgoingEdge(incomingAttribite.RelatedEdgeDefinition.AttributeID).GetTargetVertices();
                            }
                        }
                        else
                        {
                            var outgoingAttribute = (IOutgoingEdgeDefinition)currentAttribute;

                            if (myNode.BackwardEdges.ContainsKey(aLowerLevelKey.LastEdge))
                            {
                                //take the edges that are already available

                                referencedUUIDs = _iGraphDB.GetVertices<IEnumerable<IVertex>>(
                                    _securityToken,
                                    _transactionToken,
                                    new RequestGetVertices(outgoingAttribute.SourceVertexType.ID, myNode.BackwardEdges[aLowerLevelKey.LastEdge].Select(item => item.Destination)),
                                    (stats, vertices) => vertices);
                            }
                            else
                            {
                                var aVertex = myNode.GetIVertex(_iGraphDB, outgoingAttribute.SourceVertexType.ID, _securityToken, _transactionToken);

                                if (aVertex.HasIncomingVertices(myCurrentBackwardEdgekey.VertexTypeID, myCurrentBackwardEdgekey.AttributeID))
                                {
                                    referencedUUIDs = aVertex.GetIncomingVertices(myCurrentBackwardEdgekey.VertexTypeID, myCurrentBackwardEdgekey.AttributeID);
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

                            foreach (var aVertex in referencedUUIDs)
                            {
                                AddNodeRecursiveBackward(aVertex, myNode.GetVertexID(), myCurrentLevelKey, aLowerLevelKey, lowerLevelKeys, myGraph);
                                myNode.AddBackwardEdge(edgeKeyForBackwardEdge, aVertex.VertexID, null);
                            }
                        }
                    }
                }
            }
        }

        private void AddNodeRecursiveBackward(IVertex myNewVertex, Int64 mySourceUUID, LevelKey mySourceLevelKey, LevelKey myNewNodeLevelKey, List<LevelKey> lowerLevelKeys, IExpressionGraph myGraph)
        {
            lock (myGraph)
            {

                #region add node
                //add node and the node's backwardEdge


                //in this point we are shure that the level reall exists

                if (!myGraph.Levels[myNewNodeLevelKey.Level].ExpressionLevels[myNewNodeLevelKey].Nodes.ContainsKey(myNewVertex.VertexID))
                {
                    myGraph.Levels[myNewNodeLevelKey.Level].ExpressionLevels[myNewNodeLevelKey].Nodes.Add(myNewVertex.VertexID, new ExpressionNode(myNewVertex, null));
                }

                myGraph.Levels[myNewNodeLevelKey.Level].AddForwardEdgeToNode(myNewNodeLevelKey, myNewVertex.VertexID, mySourceLevelKey.LastEdge, mySourceUUID, null);

                #endregion

                #region recursion

                if (lowerLevelKeys != null)
                {
                    UpdateLowerLevels(myGraph.Levels[myNewNodeLevelKey.Level].ExpressionLevels[myNewNodeLevelKey].Nodes[myNewVertex.VertexID], myNewNodeLevelKey, lowerLevelKeys, myGraph);
                }

                #endregion
            }
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
                        if (myLongerLevelKey.Edges[0].VertexTypeID == myShorterLevelKey.Edges[0].VertexTypeID)
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


                IVertex currentDBObject = null;
                EdgeKey myCurrentForwardEdgekey = nextHigherLevelKey.Edges[startLevelKey.Level];

                IVertexType currentType = _iGraphDB.GetVertexType<IVertexType>(
                    _securityToken,
                    _transactionToken,
                    new RequestGetVertexType(myCurrentForwardEdgekey.VertexTypeID),
                    (stats, vertexType) => vertexType);

                IAttributeDefinition interestingAttribute = currentType.GetAttributeDefinition(myCurrentForwardEdgekey.AttributeID);

                //find out whats the real type of the referenced objects
                var typeOfReferencedObjects = GetTypeOfAttribute(currentType, interestingAttribute);

                //Extend graph
                foreach (var aNode in aGraph.Levels[startLevelKey.Level].ExpressionLevels[startLevelKey].Nodes)
                {
                    currentDBObject = aNode.Value.GetIVertex(_iGraphDB, currentType.ID, _securityToken, _transactionToken);

                    if (currentDBObject != null)
                    {
                        //there is no need to extend the graph if there is no IVertex available
                        switch (interestingAttribute.Kind)
                        {
                            case AttributeType.IncomingEdge:

                                var incomingAttribute = (IIncomingEdgeDefinition)interestingAttribute;

                                if (currentDBObject.HasIncomingVertices(incomingAttribute.RelatedEdgeDefinition.SourceVertexType.ID, incomingAttribute.RelatedEdgeDefinition.AttributeID))
                                {
                                    foreach (var aIncomingVertex in currentDBObject.GetIncomingVertices(incomingAttribute.RelatedEdgeDefinition.SourceVertexType.ID, incomingAttribute.RelatedEdgeDefinition.AttributeID))
                                    {
                                        //add backwardEdge to node (and itself)
                                        aGraph.Levels[nextHigherLevelKey.Level].AddNodeAndBackwardEdge(nextHigherLevelKey, aIncomingVertex, startLevelKey.LastEdge, currentDBObject.VertexID, null, null);

                                        //recursion
                                        ExtendGraphUp(nextHigherLevelKey, endLevelKey, aGraph);
                                        aNode.Value.AddForwardEdge(myCurrentForwardEdgekey, aIncomingVertex.VertexID, null);
                                    }
                                }

                                break;
                            case AttributeType.OutgoingEdge:

                                var outgoingEdgeAttribute = (IOutgoingEdgeDefinition)interestingAttribute;

                                if (currentDBObject.HasOutgoingEdge(outgoingEdgeAttribute.AttributeID))
                                {
                                    foreach (var aOutgoingVertex in currentDBObject.GetOutgoingEdge(outgoingEdgeAttribute.AttributeID).GetTargetVertices())
                                    {
                                        //add backwardEdge to node (and itself)
                                        aGraph.Levels[nextHigherLevelKey.Level].AddNodeAndBackwardEdge(nextHigherLevelKey, aOutgoingVertex, startLevelKey.LastEdge, currentDBObject.VertexID, null, null);

                                        //recursion
                                        ExtendGraphUp(nextHigherLevelKey, endLevelKey, aGraph);
                                        aNode.Value.AddForwardEdge(myCurrentForwardEdgekey, aOutgoingVertex.VertexID, null);
                                    }
                                }

                                break;
                            case AttributeType.Property:                         
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private IVertexType GetTypeOfAttribute(IVertexType currentType, IAttributeDefinition interestingAttribute)
        {
            switch (interestingAttribute.Kind)
            {
                case AttributeType.IncomingEdge:

                    #region incoming edge

                    IIncomingEdgeDefinition incomingEdgeDefinition = interestingAttribute as IIncomingEdgeDefinition;

                    return incomingEdgeDefinition.RelatedEdgeDefinition.SourceVertexType;

                    #endregion

                case AttributeType.OutgoingEdge:

                #region outgoing edge

                    IOutgoingEdgeDefinition outgoingEdgeDefinition = interestingAttribute as IOutgoingEdgeDefinition;

                    return outgoingEdgeDefinition.SourceVertexType;

                #endregion

                case AttributeType.Property:
                default:
                    throw new ExpressionGraphInternalException("wicked thing happend");
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
                        throw new ExpressionGraphInternalException("Its currently not implemented to select a LevelKey from a ExpressionGraph that is surrounded by lower and upper LevelKeys.");

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

                            LevelKey lowestLevelKey = new LevelKey(myLevelKey.Edges[0].VertexTypeID, _iGraphDB, _securityToken, _transactionToken);

                            var allVerticesOfAType = _iGraphDB.GetVertices<IEnumerable<IVertex>>(
                                _securityToken,
                                _transactionToken,
                                new RequestGetVertices(lowestLevelKey.LastEdge.VertexTypeID),
                                (stats, vertices) => vertices);

                            foreach (var aVertex in allVerticesOfAType)
                            {
                                this.AddNode(aVertex, lowestLevelKey, 0);
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
                    myLevelKeyPred = myLevelKey.GetPredecessorLevel(_iGraphDB, _securityToken, _transactionToken);

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
