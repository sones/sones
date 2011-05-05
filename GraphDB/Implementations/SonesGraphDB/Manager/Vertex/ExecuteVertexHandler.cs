using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.VertexStore.Definitions;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Expression.Tree;
using sones.GraphDB.Request;
using sones.GraphDB.Manager.TypeManagement;
using sones.Library.Commons.VertexStore;
using sones.GraphDB.Manager.Index;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.Manager.QueryPlan;
using sones.GraphDB.Request.Insert;
using sones.GraphDB.Expression;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.Expression.Tree.Literals;
using sones.Plugins.Index.Interfaces;
using sones.Library.Commons.VertexStore.Definitions.Update;

namespace sones.GraphDB.Manager.Vertex
{
    internal class ExecuteVertexHandler : AVertexHandler, IVertexHandler
    {
        #region data

        /// <summary>
        /// Needed for getting vertices from the persistence layer
        /// </summary>
        private IVertexStore _vertexStore;

        /// <summary>
        /// Needed for index interaction
        /// </summary>
        private IIndexManager _indexManager;

        /// <summary>
        /// Needed for transforming an expression into a query plan
        /// </summary>
        private IQueryPlanManager _queryPlanManager;

        private IDManager _idManager;

        #endregion

        #region c'tor

        public ExecuteVertexHandler(IDManager myIDManager)
        {
            _idManager = myIDManager;
        }

        #endregion

        #region IVertexManager Members

        #region GetVertices

        public IEnumerable<IVertex> GetVertices(IExpression myExpression, bool myIsLongrunning, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            var queryPlan = _queryPlanManager.CreateQueryPlan(myExpression, myIsLongrunning, myTransactionToken, mySecurityToken);

            return queryPlan.Execute();
        }

        public IEnumerable<IVertex> GetVertices(String myVertexType, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var vertextype = _vertexTypeManager.ExecuteManager.GetVertexType(myVertexType, myTransaction, mySecurity);
            return _vertexStore.GetVerticesByTypeID(mySecurity, myTransaction, vertextype.ID);
        }

        public IEnumerable<IVertex> GetVertices(long myTypeID, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return _vertexStore.GetVerticesByTypeID(mySecurity, myTransaction, myTypeID);
        }

        public IEnumerable<IVertex> GetVertices(RequestGetVertices _request, TransactionToken TransactionToken, SecurityToken SecurityToken)
        {
            IEnumerable<IVertex> result;
            #region case 1 - Expression

            if (_request.Expression != null)
            {
                result = GetVertices(_request.Expression, _request.IsLongrunning, TransactionToken, SecurityToken);
            }

            #endregion

            #region case 2 - No Expression

            else if (_request.VertexTypeName != null)
            {
                //2.1 typeName as string
                if (_request.VertexIDs != null)
                {
                    //2.1.1 vertex ids
                    List<IVertex> fetchedVertices = new List<IVertex>();

                    foreach (var item in _request.VertexIDs)
                    {
                        fetchedVertices.Add(GetVertex(_request.VertexTypeName, item, null, null, TransactionToken, SecurityToken));
                    }

                    result = fetchedVertices;
                }
                else
                {
                    //2.1.2 no vertex ids ... take all
                    result = GetVertices(_request.VertexTypeName, TransactionToken, SecurityToken);
                }
            }
            else
            {
                //2.2 type as id
                if (_request.VertexIDs != null)
                {
                    //2.2.1 vertex ids
                    List<IVertex> fetchedVertices = new List<IVertex>();

                    foreach (var item in _request.VertexIDs)
                    {
                        fetchedVertices.Add(GetVertex(_request.VertexTypeID, item, null, null, TransactionToken, SecurityToken));
                    }

                    result = fetchedVertices;
                }
                else
                {
                    //2.2.2 no vertex ids ... take all
                    result = GetVertices(_request.VertexTypeID, TransactionToken, SecurityToken);
                }
            }

            #endregion

            return result;
        }

        #endregion

        #region GetVertex

        public IVertex GetVertex(string myVertexTypeName, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            return _vertexStore.GetVertex(mySecurityToken, myTransactionToken, myVertexID, _vertexTypeManager.ExecuteManager.GetVertexType(myVertexTypeName, myTransactionToken, mySecurityToken).ID, (aEdition) => myEdition == aEdition, (aVertexRevisionID) => myTimespan.IsWithinTimeStamp(aVertexRevisionID));
        }

        public IVertex GetVertex(long myVertexTypeID, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken TransactionToken, SecurityToken SecurityToken)
        {
            return _vertexStore.GetVertex(SecurityToken, TransactionToken, myVertexID, myVertexTypeID, (aEdition) => myEdition == aEdition, (aVertexRevisionID) => myTimespan.IsWithinTimeStamp(aVertexRevisionID));
        }

        public IVertex GetSingleVertex(IExpression myExpression, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            return GetVertices(myExpression, false, myTransactionToken, mySecurityToken).FirstOrDefault();
        }

        #endregion


        public IVertex AddVertex(RequestInsertVertex myInsertDefinition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            IVertexType vertexType = GetVertexType(myInsertDefinition.VertexTypeName, myTransaction, mySecurity);

            //we check unique constraints here 
            foreach (var unique in vertexType.GetUniqueDefinitions(true))
            {
                var key = CreateIndexEntry(unique.CorrespondingIndex.IndexedProperties, myInsertDefinition.StructuredProperties);

                var definingVertexType = unique.DefiningVertexType;

                foreach (var vtype in definingVertexType.GetChildVertexTypes(true, true))
                {
                    var index = _indexManager.GetIndex(vtype, unique.CorrespondingIndex.IndexedProperties, mySecurity, myTransaction);

                    if (index.ContainsKey(key))
                        throw new IndexUniqueConstrainViolationException(myInsertDefinition.VertexTypeName, unique.CorrespondingIndex.Name);
                }
            }

            var addDefinition = RequestInsertVertexToVertexAddDefinition(myInsertDefinition, vertexType, myTransaction, mySecurity);
            var result = _vertexStore.AddVertex(mySecurity, myTransaction, addDefinition);


            foreach (var indexDef in vertexType.GetIndexDefinitions(false))
            {
                var key = CreateIndexEntry(indexDef.IndexedProperties, myInsertDefinition.StructuredProperties);
                var index = _indexManager.GetIndex(vertexType, indexDef.IndexedProperties, mySecurity, myTransaction);

                if (index is ISingleValueIndex<IComparable, Int64>)
                {
                    (index as ISingleValueIndex<IComparable, Int64>).Add(key, result.VertexID);
                }
                else if (index is IMultipleValueIndex<IComparable, Int64>)
                {
                    //Perf: We do not need to add a set of values. Initializing a HashSet is to expensive for this operation. 
                    //TODO: Refactor IIndex structure
                    (index as IMultipleValueIndex<IComparable, Int64>).Add(key, new HashSet<Int64> { result.VertexID });
                }
                else
                {
                    throw new NotImplementedException("Indices other than single or multiple value indices are not supported yet.");
                }
            }

            return result;
        }



        private VertexAddDefinition RequestInsertVertexToVertexAddDefinition(RequestInsertVertex myInsertDefinition, IVertexType myVertexType, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            long vertexID;
            if (myInsertDefinition.VertexUUID.HasValue)
            {
                _idManager[myVertexType.ID].SetToMaxID(myInsertDefinition.VertexUUID.Value);
                vertexID = myInsertDefinition.VertexUUID.Value;
            }
            else
            {
                vertexID = _idManager[myVertexType.ID].GetNextID();
            }

            var source = new VertexInformation(myVertexType.ID, vertexID);
            long date = DateTime.UtcNow.ToBinary();

            IEnumerable<SingleEdgeAddDefinition> singleEdges;
            IEnumerable<HyperEdgeAddDefinition> hyperEdges;

            CreateEdgeAddDefinitions(myInsertDefinition.OutgoingEdges, myVertexType, myTransaction, mySecurity, source, date, out singleEdges, out hyperEdges);


            var binaries = (myInsertDefinition.BinaryProperties == null)
                            ? null
                            : myInsertDefinition.BinaryProperties.Select(x => new StreamAddDefinition(myVertexType.GetAttributeDefinition(x.Key).ID, x.Value));

            var structured = ConvertStructuredProperties(myInsertDefinition, myVertexType);

            return new VertexAddDefinition(vertexID, myVertexType.ID, myInsertDefinition.Edition, hyperEdges, singleEdges, binaries, myInsertDefinition.Comment, date, date, structured, myInsertDefinition.UnstructuredProperties);
        }

        private static Dictionary<long, IComparable> ConvertStructuredProperties(IPropertyProvider myInsertDefinition, IBaseType myType)
        {
            return (myInsertDefinition.StructuredProperties == null)
                             ? null
                             : myInsertDefinition.StructuredProperties.ToDictionary(x => myType.GetAttributeDefinition(x.Key).ID, x => x.Value);
        }

        private void CreateEdgeAddDefinitions(
            IEnumerable<EdgePredefinition> myOutgoingEdges,
            IVertexType myVertexType,
            TransactionToken myTransaction,
            SecurityToken mySecurity,
            VertexInformation source,
            long date,
            out IEnumerable<SingleEdgeAddDefinition> outSingleEdges,
            out IEnumerable<HyperEdgeAddDefinition> outHyperEdges)
        {
            outSingleEdges = null;
            outHyperEdges = null;
            if (myOutgoingEdges == null)
                return;

            var singleEdges = new Dictionary<String, SingleEdgeAddDefinition>();
            var hyperEdges = new Dictionary<String, HyperEdgeAddDefinition>();
            foreach (var edgeDef in myOutgoingEdges)
            {
                var attrDef = myVertexType.GetOutgoingEdgeDefinition(edgeDef.EdgeName);

                switch (attrDef.Multiplicity)
                {
                    case EdgeMultiplicity.SingleEdge:
                        {
                            var edge = CreateSingleEdgeAddDefinition(myTransaction, mySecurity, date, attrDef.ID, edgeDef, attrDef.EdgeType, source, attrDef.TargetVertexType);
                            if (edge.HasValue)
                                singleEdges.Add(edgeDef.EdgeName, edge.Value);
                        }
                        break;

                    case EdgeMultiplicity.HyperEdge:
                        {
                            break;
                        }
                    case EdgeMultiplicity.MultiEdge:
                        {
                            var edge = CreateMultiEdgeAddDefinition(myTransaction, mySecurity, source, date, edgeDef, attrDef);
                            if (edge.HasValue)
                                hyperEdges.Add(attrDef.Name, edge.Value);
                        }
                        break;
                    default:
                        throw new UnknownDBException("The EdgeMultiplicy enumeration was updated, but not this switch statement.");
                }
            }

            outSingleEdges = singleEdges.Select(x => x.Value);
            outHyperEdges = hyperEdges.Select(x => x.Value);
        }

        private HyperEdgeAddDefinition? CreateMultiEdgeAddDefinition(
            TransactionToken myTransaction,
            SecurityToken mySecurity, 
            VertexInformation source, 
            long date, 
            EdgePredefinition edgeDef, 
            IOutgoingEdgeDefinition attrDef)
        {
            var vertexIDs = GetResultingVertexIDs(myTransaction, mySecurity, edgeDef, attrDef.TargetVertexType);

            var contained = CreateContainedEdges(myTransaction, mySecurity, date, vertexIDs, edgeDef, attrDef, source);
            if (contained == null)
                return null;

            return new HyperEdgeAddDefinition(attrDef.ID, attrDef.EdgeType.ID, source, contained, edgeDef.Comment, date, date, ConvertStructuredProperties(edgeDef, attrDef.EdgeType), edgeDef.UnstructuredProperties);
        }

        private IEnumerable<SingleEdgeAddDefinition> CreateContainedEdges(
            TransactionToken myTransaction, 
            SecurityToken mySecurity, 
            long myDate,
            IEnumerable<VertexInformation> vertexIDs, 
            EdgePredefinition edgeDef,
            IOutgoingEdgeDefinition attrDef,
            VertexInformation mySource)
        {
            if ((vertexIDs == null || vertexIDs.Count() == 0 )&& (edgeDef.ContainedEdges == null || edgeDef.ContainedEdges.Count() == 0))
                return null;

            List<SingleEdgeAddDefinition> result = new List<SingleEdgeAddDefinition>();
            if (vertexIDs != null)
            {
                foreach (var vertex in vertexIDs)
                {
                    //single edges from VertexIDs or expression does not have user properties
                    //TODO they can have default values
                    CheckMandatoryConstraint(null, attrDef.InnerEdgeType);
                    result.Add(new SingleEdgeAddDefinition(Int64.MinValue, attrDef.InnerEdgeType.ID, mySource, vertex, null, myDate, myDate, null, null));
                }
            }

            if (edgeDef.ContainedEdges != null)
            {
                foreach (var edge in edgeDef.ContainedEdges)
                {
                    if (edge.ContainedEdges != null)
                        //TODO a better exception here
                        throw new Exception("An edge within a multi edge cannot have contained edges.");
                    
                    var toAdd = CreateSingleEdgeAddDefinition(myTransaction, mySecurity, myDate, Int64.MinValue, edge, attrDef.InnerEdgeType, mySource, attrDef.TargetVertexType);

                    if (toAdd.HasValue)
                        result.Add(toAdd.Value);
                }
            }
            return result;
        }

        private SingleEdgeAddDefinition? CreateSingleEdgeAddDefinition(
            TransactionToken myTransaction,
            SecurityToken mySecurity, 
            long date, 
            long myAttributeID, 
            EdgePredefinition edgeDef, 
            IEdgeType myEdgeType, 
            VertexInformation source, 
            IVertexType myTargetType = null)
        {
            var vertexIDs = GetResultingVertexIDs(myTransaction, mySecurity, edgeDef, myTargetType);
            if (vertexIDs == null)
                return null;

            CheckMandatoryConstraint(edgeDef, myEdgeType);
            CheckTargetVertices(myTargetType, vertexIDs);

            return new SingleEdgeAddDefinition(myAttributeID, myEdgeType.ID, source, vertexIDs.First(), edgeDef.Comment, date, date, ConvertStructuredProperties(edgeDef, myEdgeType), edgeDef.UnstructuredProperties);
        }

        private static void CheckTargetVertices(IVertexType myTargetVertexType, IEnumerable<VertexInformation> vertexIDs)
        {
            var distinctTypeIDS = new HashSet<Int64>(vertexIDs.Select(x => x.VertexTypeID));
            var allowedTypeIDs = new HashSet<Int64>(myTargetVertexType.GetChildVertexTypes(true, true).Select(x => x.ID));
            distinctTypeIDS.ExceptWith(allowedTypeIDs);
            if (distinctTypeIDS.Count > 0)
                throw new Exception("A target vertex has a type, that is not assignable to the target vertex type of the edge.");
        }

        private IEnumerable<VertexInformation> GetResultingVertexIDs(TransactionToken myTransaction, SecurityToken mySecurity, EdgePredefinition myEdgeDef, IVertexType myTargetType = null)
        {
            if (myEdgeDef.VertexIDsByVertexTypeID != null || myEdgeDef.VertexIDsByVertexTypeName != null )
            {
                HashSet<VertexInformation> result = new HashSet<VertexInformation>();
                if (myEdgeDef.VertexIDsByVertexTypeID != null)
                {
                    foreach (var kvP in myEdgeDef.VertexIDsByVertexTypeID)
                    {
                        foreach (var vertex in kvP.Value)
                        {
                            result.Add(new VertexInformation(kvP.Key, vertex));
                        }

                    }
                }
                if (myEdgeDef.VertexIDsByVertexTypeName != null)
                {
                    foreach (var kvP in myEdgeDef.VertexIDsByVertexTypeName)
                    {
                        var vertexType = _vertexTypeManager.ExecuteManager.GetVertexType(kvP.Key, myTransaction, mySecurity);
                        foreach (var vertex in kvP.Value)
                        {
                            result.Add(new VertexInformation(vertexType.ID, vertex));
                        }

                    }
                }
                return result;
            }
            return null;
        }

        private static IComparable CreateIndexEntry(IList<IPropertyDefinition> myIndexProps, IDictionary<string, IComparable> myProperties)
        {

            if (myIndexProps.Count > 1)
            {
                List<IComparable> values = new List<IComparable>(myIndexProps.Count);
                for (int i = 0; i < myIndexProps.Count; i++)
                {
                    values[i] = myProperties[myIndexProps[i].Name];
                }

                //using ListCollectionWrapper from Expressions, maybe this class should go to Lib
                return new ListCollectionWrapper(values);
            }
            else if (myIndexProps.Count == 1)
            {
                return myProperties[myIndexProps[0].Name];
            }
            throw new ArgumentException("A unique definition must contain at least one element.");
        }


        public IVertexStore VertexStore
        {
            get { return _vertexStore; }
        }

        #endregion

        #region IManager Members

        public override void Initialize(IMetaManager myMetaManager)
        {
            base.Initialize(myMetaManager);

            _indexManager = myMetaManager.IndexManager;
            _vertexStore = myMetaManager.VertexStore;
            _queryPlanManager = myMetaManager.QueryPlanManager;
        }

        #endregion


        #region IVertexHandler Members


        public IEnumerable<IVertex> UpdateVertices(RequestUpdate myUpdate, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var toBeUpdated = GetVertices(myUpdate.GetVerticesRequest, myTransaction, mySecurity);
            var groupedByTypeID = toBeUpdated.GroupBy(_ => _.VertexTypeID);

            foreach (var group in groupedByTypeID)
            {
                var vertexType = _vertexTypeManager.ExecuteManager.GetVertexType(group.Key, myTransaction, mySecurity);
                foreach (var vertex in group)
                {
                    var update = CreateVertexUpdateDefinition(vertex, vertexType, myUpdate);

                    yield return UpdateVertex(vertex, update, myUpdate.UpdatedEdition, myTransaction, mySecurity);
                }
            }

            yield break;
        }

        private IVertex UpdateVertex(IVertex vertex, VertexUpdateDefinition update, String myEdition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return _vertexStore.UpdateVertex(mySecurity, myTransaction, vertex.VertexID, vertex.VertexTypeID, update, myEdition);
        }

        private VertexUpdateDefinition CreateVertexUpdateDefinition(IVertex myVertex, IVertexType myVertexType, RequestUpdate myUpdate)
        {
            
            #region get removes

            IEnumerable<long> toBeDeletedSingle;
            IEnumerable<long> toBeDeletedHyper;
            IEnumerable<long> toBeDeletedStructured;
            IEnumerable<String> toBeDeletedUnstructured;
            IEnumerable<long> toBeDeletedBinaries;
            
            CreateEdgeDeleteDefinition(myVertex, myVertexType, myUpdate.RemovedAttributes, out toBeDeletedSingle, out toBeDeletedHyper, out toBeDeletedStructured, out toBeDeletedUnstructured, out toBeDeletedBinaries);

            #endregion

            #region get update definitions

            IDictionary<Int64, HyperEdgeUpdateDefinition> toBeUpdatedHyper;
            IDictionary<Int64, SingleEdgeUpdateDefinition> toBeUpdatedSingle;
            IDictionary<Int64, IComparable> toBeUpdatedStructured;
            IDictionary<String, Object> toBeUpdatedUnstructured;
            IDictionary<Int64, StreamAddDefinition> toBeUpdatedBinaries;

            CreateEdgeUpdateDefinition(myVertex, myVertexType, myUpdate, out toBeUpdatedSingle, out toBeUpdatedHyper, out toBeUpdatedStructured, out toBeUpdatedUnstructured, out toBeUpdatedBinaries);

            #endregion

            #region create updates

            var single = (toBeUpdatedSingle != null || toBeDeletedSingle != null)
                ? new SingleEdgeUpdate(toBeUpdatedSingle, toBeDeletedSingle)
                : null;

            var hyper = (toBeUpdatedHyper != null || toBeDeletedHyper != null)
                ? new HyperEdgeUpdate(toBeUpdatedHyper, toBeDeletedHyper)
                : null;

            var structured = (toBeUpdatedStructured != null || toBeDeletedStructured != null)
                ? new StructuredPropertiesUpdate(toBeUpdatedStructured, toBeDeletedStructured)
                : null;

            var unstructured = (toBeUpdatedUnstructured != null || toBeDeletedUnstructured != null)
                ? new UnstructuredPropertiesUpdate(toBeUpdatedUnstructured, toBeDeletedUnstructured)
                : null;

            var binaries = (toBeUpdatedBinaries != null || toBeDeletedBinaries != null)
                ? new BinaryPropertiesUpdate(toBeUpdatedBinaries, toBeDeletedBinaries)
                : null;

            #endregion

            return new VertexUpdateDefinition(myUpdate.UpdatedComment, structured, unstructured, binaries, single, hyper);
        }

        private void CreateEdgeUpdateDefinition(
            IVertex myVertex, IVertexType myVertexType, RequestUpdate myUpdate, 
            out IDictionary<long, SingleEdgeUpdateDefinition> outUpdatedSingle, 
            out IDictionary<long, HyperEdgeUpdateDefinition> outUpdatedHyper, 
            out IDictionary<long, IComparable> outUpdatedStructured, 
            out IDictionary<string, object> outUpdatedUnstructured, 
            out IDictionary<long, StreamAddDefinition> outUpdatedBinaries)
        {
            #region predefine

            Dictionary<long, SingleEdgeUpdateDefinition> toBeUpdatedSingle = null;
            Dictionary<long, HyperEdgeUpdateDefinition> toBeUpdatedHyper = null;
            Dictionary<long, IComparable> toBeUpdatedStructured = null;
            Dictionary<string, object> toBeUpdatedUnstructured = null;
            Dictionary<long, StreamAddDefinition> toBeUpdatedBinaries = null;

            #endregion

            
            
            #region return

            outUpdatedSingle       = toBeUpdatedSingle;
            outUpdatedHyper        = toBeUpdatedHyper;
            outUpdatedStructured   = toBeUpdatedStructured;
            outUpdatedUnstructured = toBeUpdatedUnstructured;
            outUpdatedBinaries     = toBeUpdatedBinaries;

            #endregion
        }

        private void CreateEdgeDeleteDefinition(
            IVertex myVertex, IVertexType myVertexType, IEnumerable<string> myRemoveAttributes, 
            out IEnumerable<long> outDeletedSingle, 
            out IEnumerable<long> outDeletedHyper,
            out IEnumerable<long> outDeletedStructured,
            out IEnumerable<string> outDeletedUnstructured,
            out IEnumerable<long> outDeletedBinaries)
        {
            #region predefine

            List<long> toBeDeletedSingle = null;
            List<long> toBeDeletedHyper = null;
            List<long> toBeDeletedStructured = null;
            List<String> toBeDeletedUnstructured = null;
            List<long> toBeDeletedBinaries = null;

            #endregion

            if (myRemoveAttributes != null)
            {
                foreach (var name in myRemoveAttributes)
                {
                    if (myVertexType.HasAttribute(name))
                    {
                        var attr = myVertexType.GetAttributeDefinition(name);

                        switch (attr.Kind)
                        {
                            case AttributeType.Property:
                                toBeDeletedStructured =  toBeDeletedStructured ?? new List<long>();
                                toBeDeletedStructured.Add(attr.ID);
                                break;
                            case AttributeType.BinaryProperty:
                                toBeDeletedBinaries = toBeDeletedBinaries ?? new List<long>();
                                toBeDeletedBinaries.Add(attr.ID);
                                break;
                            case AttributeType.IncomingEdge:
                                //TODO: a better exception here.
                                throw new Exception("The edges on an incoming edge attribute can not be removed.");
                            case AttributeType.OutgoingEdge:
                                switch ((attr as IOutgoingEdgeDefinition).Multiplicity)
                                {
                                    case EdgeMultiplicity.HyperEdge:
                                    case EdgeMultiplicity.MultiEdge:
                                        toBeDeletedHyper = toBeDeletedHyper ?? new List<long>();
                                        toBeDeletedHyper.Add(attr.ID);
                                        break;
                                    case EdgeMultiplicity.SingleEdge:
                                        toBeDeletedSingle = toBeDeletedSingle ?? new List<long>();
                                        toBeDeletedSingle.Add(attr.ID);
                                        break;
                                    default:
                                        //TODO a better exception here
                                        throw new Exception("The enumeration EdgeMultiplicity was changed, but not this switch statement.");
                                }
                                break;
                            default:
                                //TODO: a better exception here.
                                throw new Exception("The enumeration AttributeType was updated, but not this switch statement.");

                        }
                    }
                    else
                    {
                        toBeDeletedUnstructured = toBeDeletedUnstructured ?? new List<String>();
                        toBeDeletedUnstructured.Add(name);
                    }


                }
            }

            #region return

            outDeletedSingle       = toBeDeletedSingle;
            outDeletedHyper        = toBeDeletedHyper;
            outDeletedStructured   = toBeDeletedStructured;
            outDeletedUnstructured = toBeDeletedUnstructured;
            outDeletedBinaries     = toBeDeletedBinaries;

            #endregion

        }

        #endregion

    }
}
