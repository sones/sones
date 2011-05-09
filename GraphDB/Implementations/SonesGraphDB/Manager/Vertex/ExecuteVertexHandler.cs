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
using sones.Library.LanguageExtensions;
using System.IO;
using sones.GraphDB.TypeManagement.Base;
using sones.Plugins.Index.Helper;

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
            var result = (addDefinition.Item1.HasValue)
                            ? _vertexStore.AddVertex(mySecurity, myTransaction, addDefinition.Item2, addDefinition.Item1.Value)
                            : _vertexStore.AddVertex(mySecurity, myTransaction, addDefinition.Item2);



            foreach (var indexDef in vertexType.GetIndexDefinitions(false))
            {
                var key = CreateIndexEntry(indexDef.IndexedProperties, myInsertDefinition.StructuredProperties);
                if (key != null)
                {
                    //do sth if there is a value corresponding to the index definition

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

            }

            return result;
        }



        private Tuple<long?, VertexAddDefinition> RequestInsertVertexToVertexAddDefinition(RequestInsertVertex myInsertDefinition, IVertexType myVertexType, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            long vertexID = (myInsertDefinition.VertexUUID.HasValue)
                ? myInsertDefinition.VertexUUID.Value
                : _idManager[myVertexType.ID].GetNextID();

            var source = new VertexInformation(myVertexType.ID, vertexID);
            long creationdate = DateTime.UtcNow.ToBinary();
            long modificationDate = creationdate;
            String comment = myInsertDefinition.Comment;
            String edition = myInsertDefinition.Edition;
            long? revision = null;

            IEnumerable<SingleEdgeAddDefinition> singleEdges;
            IEnumerable<HyperEdgeAddDefinition> hyperEdges;

            CreateEdgeAddDefinitions(myInsertDefinition.OutgoingEdges, myVertexType, myTransaction, mySecurity, source, creationdate, out singleEdges, out hyperEdges);


            var binaries = (myInsertDefinition.BinaryProperties == null)
                            ? null
                            : myInsertDefinition.BinaryProperties.Select(x => new StreamAddDefinition(myVertexType.GetAttributeDefinition(x.Key).ID, x.Value));

            var structured = ConvertStructuredProperties(myInsertDefinition, myVertexType);

            ExtractVertexProperties(ref edition, ref revision, ref comment, ref vertexID, ref creationdate, ref modificationDate, structured);

            //set id to maximum to allow user set UUIDs
            _idManager[myVertexType.ID].SetToMaxID(vertexID);

            return Tuple.Create(revision, new VertexAddDefinition(vertexID, myVertexType.ID, edition, hyperEdges, singleEdges, binaries, comment, creationdate, modificationDate, structured, myInsertDefinition.UnstructuredProperties));
        }

        private static void ExtractVertexProperties(ref String edition, ref long? revision, ref String comment, ref long vertexID, ref long creationdate, ref long modificationDate, IDictionary<long, IComparable> structured)
        {
            if (structured != null)
            {
                List<long> toDeleteKeys = null;
                foreach (var structure in structured)
                {
                    long? toDelete = null;
                    switch ((AttributeDefinitions)structure.Key)
                    {
                        case AttributeDefinitions.VertexDotComment:
                            comment = structure.Value as String;
                            toDelete = structure.Key;
                            break;
                        case AttributeDefinitions.VertexDotCreationDate:
                            creationdate = ((DateTime)structure.Value).ToBinary();
                            toDelete = structure.Key;
                            break;
                        case AttributeDefinitions.VertexDotEdition:
                            edition = structure.Value as String;
                            toDelete = structure.Key;
                            break;
                        case AttributeDefinitions.VertexDotModificationDate:
                            modificationDate = ((DateTime)structure.Value).ToBinary();
                            toDelete = structure.Key;
                            break;
                        case AttributeDefinitions.VertexDotRevision:
                            revision = (long)structure.Value;
                            toDelete = structure.Key;
                            break;
                        case AttributeDefinitions.VertexDotUUID:
                            vertexID = (long)structure.Value;
                            toDelete = structure.Key;
                            break;

                    }

                    if (toDelete.HasValue)
                    {
                        toDeleteKeys = toDeleteKeys ?? new List<long>();
                        toDeleteKeys.Add(toDelete.Value);
                    }
                }
                if (toDeleteKeys != null)
                    foreach (var key in toDeleteKeys)
                        structured.Remove(key);
            }
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
            if ((vertexIDs == null || vertexIDs.Count() == 0) && (edgeDef.ContainedEdges == null || edgeDef.ContainedEdges.Count() == 0))
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
            if (myEdgeDef.VertexIDsByVertexTypeID != null || myEdgeDef.VertexIDsByVertexTypeName != null)
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
                IComparable toBeIndexedValue;
                if (myProperties != null && myProperties.TryGetValue(myIndexProps[0].Name, out toBeIndexedValue))
                {
                    return toBeIndexedValue;
                }
                return null;
            }
            throw new ArgumentException("A unique definition must contain at least one element.");
        }

        private static IComparable CreateIndexEntry(IList<IPropertyDefinition> myIndexProps, IDictionary<long, IComparable> myProperties)
        {

            if (myIndexProps.Count > 1)
            {
                List<IComparable> values = new List<IComparable>(myIndexProps.Count);
                for (int i = 0; i < myIndexProps.Count; i++)
                {
                    values[i] = myProperties[myIndexProps[i].ID];
                }

                //using ListCollectionWrapper from Expressions, maybe this class should go to Lib
                return new ListCollectionWrapper(values);
            }
            else if (myIndexProps.Count == 1)
            {
                IComparable toBeIndexedValue;
                if (myProperties != null && myProperties.TryGetValue(myIndexProps[0].ID, out toBeIndexedValue))
                {
                    return toBeIndexedValue;
                }
                return null;
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

        #region class PropertyCopy
        /// <summary>
        /// This class copies the
        /// </summary>
        private class PropertyCopy : IPropertyProvider
        {
            #region c'tor

            public PropertyCopy(IPropertyProvider toCopy)
            {
                if (toCopy.StructuredProperties != null)
                    foreach (var prop in toCopy.StructuredProperties)
                    {
                        AddStructuredProperty(prop.Key, prop.Value);
                    }

                if (toCopy.UnstructuredProperties != null)
                    foreach (var prop in toCopy.UnstructuredProperties)
                    {
                        AddUnstructuredProperty(prop.Key, prop.Value);
                    }

                if (toCopy.UnknownProperties != null)
                    foreach (var prop in toCopy.UnknownProperties)
                    {
                        AddUnknownProperty(prop.Key, prop.Value);
                    }
            }

            #endregion

            #region IPropertyProvider Members

            public IDictionary<string, IComparable> StructuredProperties { get; private set; }

            public IDictionary<string, object> UnstructuredProperties { get; private set; }

            public IDictionary<string, object> UnknownProperties { get; private set; }

            public IPropertyProvider AddStructuredProperty(string myPropertyName, IComparable myProperty)
            {
                StructuredProperties = StructuredProperties ?? new Dictionary<string, IComparable>();
                StructuredProperties.Add(myPropertyName, myProperty);

                return this;
            }

            public IPropertyProvider AddUnstructuredProperty(string myPropertyName, object myProperty)
            {
                UnstructuredProperties = UnstructuredProperties ?? new Dictionary<string, object>();
                UnstructuredProperties.Add(myPropertyName, myProperty);

                return this;
            }

            public IPropertyProvider AddUnknownProperty(string myPropertyName, object myProperty)
            {
                UnknownProperties = UnknownProperties ?? new Dictionary<string, object>();
                UnknownProperties.Add(myPropertyName, myProperty);

                return this;
            }

            #endregion

            #region IUnknownProvider Members

            public void ClearUnknown()
            {
                UnknownProperties = null;
            }

            #endregion
        }
        #endregion

        public void Delete(RequestDelete myDeleteRequest, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            var toBeProcessedVertices = GetVertices(myDeleteRequest.ToBeDeletedVertices, myTransactionToken, mySecurityToken);

            if (myDeleteRequest.ToBeDeletedAttributes.IsNotNullOrEmpty())
            {
                List<long> toBeDeletedStructuredPropertiesUpdate = new List<long>();
                List<String> tobeDeletedUnstructuredProperties = new List<String>();
                List<long> toBeDeletedBinaryProperties = new List<long>();
                List<long> toBeDeletedSingleEdges = new List<long>();
                List<long> toBeDeletedHyperEdges = new List<long>();
                Dictionary<Int64, IPropertyDefinition> toBeDeletedProperties = new Dictionary<long, IPropertyDefinition>();
                Dictionary<Int64, IOutgoingEdgeDefinition> toBeDeletedEdges = new Dictionary<long, IOutgoingEdgeDefinition>();
                Dictionary<Int64, IBinaryPropertyDefinition> toBeDeletedBinaries = new Dictionary<long, IBinaryPropertyDefinition>();
                HashSet<String> toBeDeletedUndefinedAttributes = new HashSet<string>();

                //remove the attributes
                foreach (var aVertexTypeGroup in toBeProcessedVertices.GroupBy(_ => _.VertexTypeID))
                {
                    var vertexType = _vertexTypeManager.ExecuteManager.GetVertexType(aVertexTypeGroup.Key, myTransactionToken, mySecurityToken);

                    #region prepare update definition

                    toBeDeletedStructuredPropertiesUpdate.Clear();
                    StructuredPropertiesUpdate structuredProperties = new StructuredPropertiesUpdate(null, toBeDeletedStructuredPropertiesUpdate);

                    tobeDeletedUnstructuredProperties.Clear();
                    UnstructuredPropertiesUpdate unstructuredProperties = new UnstructuredPropertiesUpdate(null, tobeDeletedUnstructuredProperties);

                    toBeDeletedBinaryProperties.Clear();
                    BinaryPropertiesUpdate binaryProperties = new BinaryPropertiesUpdate(null, toBeDeletedBinaryProperties);

                    toBeDeletedSingleEdges.Clear();
                    SingleEdgeUpdate singleEdges = new SingleEdgeUpdate(null, toBeDeletedSingleEdges);

                    toBeDeletedHyperEdges.Clear();
                    HyperEdgeUpdate hyperEdges = new HyperEdgeUpdate(null, toBeDeletedHyperEdges);

                    VertexUpdateDefinition update = new VertexUpdateDefinition(null, structuredProperties, unstructuredProperties, binaryProperties, singleEdges, hyperEdges);

                    #endregion

                    #region sorting attributes

                    toBeDeletedProperties.Clear();
                    toBeDeletedEdges.Clear();
                    toBeDeletedBinaries.Clear();
                    toBeDeletedUndefinedAttributes.Clear();

                    foreach (var aToBeDeleted in myDeleteRequest.ToBeDeletedAttributes)
                    {
                        if (!vertexType.HasAttribute(aToBeDeleted))
                        {
                            toBeDeletedUndefinedAttributes.Add(aToBeDeleted);
                        }

                        var attribute = vertexType.GetAttributeDefinition(aToBeDeleted);

                        switch (attribute.Kind)
                        {
                            case AttributeType.Property:
                                toBeDeletedProperties.Add(attribute.ID, (IPropertyDefinition)attribute);
                                break;

                            case AttributeType.OutgoingEdge:
                                toBeDeletedEdges.Add(attribute.ID, (IOutgoingEdgeDefinition)attribute);
                                break;

                            case AttributeType.BinaryProperty:
                                toBeDeletedBinaries.Add(attribute.ID, (IBinaryPropertyDefinition)attribute);
                                break;
                        }
                    }

                    #endregion

                    foreach (var aVertex in aVertexTypeGroup.ToList())
                    {
                        #region fetch to be deleted attributes

                        #region properties

                        foreach (var aToBeDeletedProperty in toBeDeletedProperties)
                        {
                            if (aVertex.HasProperty(aToBeDeletedProperty.Key))
                            {
                                foreach (var aIndexDefinition in aToBeDeletedProperty.Value.InIndices)
                                {
                                    RemoveVertexPropertyFromIndex(
                                        aVertex,
                                        aIndexDefinition,
                                        _indexManager.GetIndex(vertexType, aIndexDefinition.IndexedProperties, mySecurityToken, myTransactionToken)
                                        , mySecurityToken, myTransactionToken);
                                }

                                toBeDeletedStructuredPropertiesUpdate.Add(aToBeDeletedProperty.Key);
                            }
                        }

                        #endregion

                        #region edges

                        foreach (var aToBeDeltedEdge in toBeDeletedEdges)
                        {
                            if (aVertex.HasOutgoingEdge(aToBeDeltedEdge.Key))
                            {
                                switch (aToBeDeltedEdge.Value.Multiplicity)
                                {
                                    case EdgeMultiplicity.SingleEdge:

                                        toBeDeletedSingleEdges.Add(aToBeDeltedEdge.Key);

                                        break;
                                    case EdgeMultiplicity.MultiEdge:
                                    case EdgeMultiplicity.HyperEdge:

                                        toBeDeletedHyperEdges.Add(aToBeDeltedEdge.Key);

                                        break;
                                }
                            }
                        }

                        #endregion

                        #region binaries

                        foreach (var aBinaryProperty in toBeDeletedBinaryProperties)
                        {
                            //TODO: Add HasBinaryProperty to IVertex
                            if (aVertex.GetAllBinaryProperties((_, __) => _ == aBinaryProperty).Count() > 0)
                            {
                                toBeDeletedBinaryProperties.Add(aBinaryProperty);
                            }
                        }

                        #endregion

                        #region undefined data

                        foreach (var aUnstructuredProperty in toBeDeletedUndefinedAttributes)
                        {
                            if (aVertex.HasUnstructuredProperty(aUnstructuredProperty))
                            {
                                tobeDeletedUnstructuredProperties.Add(aUnstructuredProperty);
                            }
                        }

                        #endregion

                        #endregion

                        _vertexStore.UpdateVertex(mySecurityToken, myTransactionToken, aVertex.VertexID, aVertex.VertexTypeID, update, aVertex.EditionName, aVertex.VertexRevisionID, false);
                    }
                }
            }
            else
            {
                //remove the nodes
                foreach (var aVertexTypeGroup in toBeProcessedVertices.GroupBy(_ => _.VertexTypeID))
                {
                    var vertexType = _vertexTypeManager.ExecuteManager.GetVertexType(aVertexTypeGroup.Key, myTransactionToken, mySecurityToken);

                    foreach (var aVertex in aVertexTypeGroup.ToList())
                    {
                        RemoveVertex(aVertex, vertexType, mySecurityToken, myTransactionToken);
                    }
                }
            }
        }

        private void RemoveVertex(IVertex aVertex, IVertexType myVertexType, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            RemoveVertexFromIndices(aVertex, myVertexType, mySecurityToken, myTransactionToken);

            _vertexStore.RemoveVertex(mySecurityToken, myTransactionToken, aVertex.VertexID, aVertex.VertexTypeID);
        }

        private void RemoveVertexFromIndices(IVertex aVertex, IVertexType myVertexType, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            foreach (var aStructuredProperty in myVertexType.GetPropertyDefinitions(true))
            {
                if (aVertex.HasProperty(aStructuredProperty.ID))
                {
                    foreach (var aIndexDefinition in aStructuredProperty.InIndices)
                    {
                        RemoveVertexPropertyFromIndex(
                            aVertex,
                            aIndexDefinition,
                            _indexManager.GetIndex(myVertexType, aIndexDefinition.IndexedProperties, mySecurityToken, myTransactionToken)
                            , mySecurityToken, myTransactionToken);
                    }
                }
            }
        }

        private void RemoveVertexPropertyFromIndex(IVertex aVertex, IIndexDefinition aIndexDefinition, IIndex<IComparable, long> iIndex, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            var entry = CreateIndexEntry(aIndexDefinition.IndexedProperties, aVertex.GetAllProperties().ToDictionary(key => key.Item1, value => value.Item2));

            if (iIndex is IMultipleValueIndex<IComparable, long>)
            {
                lock (iIndex)
                {
                    if (iIndex.ContainsKey(entry))
                    {
                        var toBeUpdatedIndex = (IMultipleValueIndex<IComparable, long>)iIndex;

                        var payLoad = toBeUpdatedIndex[entry];
                        payLoad.Remove(aVertex.VertexID);

                        toBeUpdatedIndex.Add(entry, payLoad, IndexAddStrategy.REPLACE);
                    }
                }
            }
            else
            {
                iIndex.Remove(entry);
            }
        }

        public IEnumerable<IVertex> UpdateVertices(RequestUpdate myUpdate, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var toBeUpdated = GetVertices(myUpdate.GetVerticesRequest, myTransaction, mySecurity);
            var groupedByTypeID = toBeUpdated.GroupBy(_ => _.VertexTypeID);

            if (groupedByTypeID.CountIsGreater(0))
            {
                Dictionary<IVertex, Tuple<long?, String, VertexUpdateDefinition>> updates = new Dictionary<IVertex, Tuple<long?, String, VertexUpdateDefinition>>();
                foreach (var group in groupedByTypeID)
                {
                    var vertexType = _vertexTypeManager.ExecuteManager.GetVertexType(group.Key, myTransaction, mySecurity);

                    //we copy each property in property provider, because an unknown property can be a structured at one type but unstructured at the other.
                    //this must be refactored:
                    //idea cancel IPropertyProvider. Change ConvertUnknownProperties to fill the dictionaries directly.
                    PropertyCopy copy = new PropertyCopy(myUpdate);
                    ConvertUnknownProperties(copy, vertexType);

                    if (copy.StructuredProperties != null)
                    {

                        var toBeUpdatedStructuredNames = copy.StructuredProperties.Select(_ => _.Key).ToArray();
                        foreach (var uniqueIndex in vertexType.GetUniqueDefinitions(false))
                        {
                            //if the unique index is defined on a property that will be updated
                            if (uniqueIndex.UniquePropertyDefinitions.Any(_ => toBeUpdatedStructuredNames.Contains(_.Name)))
                            {
                                //the list of property names, that can make an update of multiple vertices on unique properties unique again.

                                var uniquemaker = uniqueIndex.UniquePropertyDefinitions.Select(_ => _.Name).Except(toBeUpdatedStructuredNames);
                                if (!uniquemaker.CountIsGreater(0) && group.CountIsGreater(1))
                                    throw new IndexUniqueConstrainViolationException(vertexType.Name, String.Join(", ", uniquemaker));
                            }
                        }

                        var toBeUpdatedUniques = vertexType.GetUniqueDefinitions(true).Where(_ => _.UniquePropertyDefinitions.Any(__ => toBeUpdatedStructuredNames.Contains(__.Name)));

                    }
                    foreach (var vertex in group)
                    {

                        //var toBeUpdatedIndices = (copy.StructuredProperties != null)
                        //                            ? copy.StructuredProperties.ToDictionary(_ => _.Key, _ => vertexType.GetPropertyDefinition(_.Key).InIndices)
                        //                            : null;


                        var update = CreateVertexUpdateDefinition(vertex, vertexType, myUpdate, copy, myTransaction, mySecurity);
                        updates.Add(vertex, update);
                    }
                }

                IEnumerable<String> updatedNames = myUpdate.UpdatedStructuredProperties != null ? myUpdate.UpdatedStructuredProperties.Select(kv => kv.Key) : Enumerable.Empty<String>();

                //force execution
                return ExecuteUpdates(updatedNames, groupedByTypeID, updates, myTransaction, mySecurity);

            }

            return Enumerable.Empty<IVertex>();

        }

        private IEnumerable<IVertex> ExecuteUpdates(IEnumerable<String> myChangedProperties, IEnumerable<IGrouping<long, IVertex>> groups, Dictionary<IVertex, Tuple<long?, string, VertexUpdateDefinition>> updates, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            List<IVertex> result = new List<IVertex>();
            foreach (var group in groups)
            {
                var vertexType = _vertexTypeManager.ExecuteManager.GetVertexType(group.Key, myTransaction, mySecurity);

                var indices = vertexType.GetIndexDefinitions(false).ToDictionary(_ => _indexManager.GetIndex(vertexType, _.IndexedProperties, mySecurity, myTransaction), _=>_.IndexedProperties);
                var neededPropNames = indices.SelectMany(_=>_.Value).Select(_=>_.Name).Distinct().Intersect(myChangedProperties);

                foreach (var vertex in group)
                {
                    var update = updates[vertex];

                    if (neededPropNames.CountIsGreater(0))
                    {
                        var structured = GetStructuredFromVertex(vertex, vertexType, neededPropNames);

                        RemoveFromIndex(vertex.VertexID, structured, vertexType, indices, myTransaction, mySecurity);
                    }
                    
                    var updatedVertex = (update.Item1 != null &&  update.Item1.HasValue)
                        ? _vertexStore.UpdateVertex(mySecurity, myTransaction, vertex.VertexID, group.Key, update.Item3, update.Item2, update.Item1.Value)
                        : _vertexStore.UpdateVertex(mySecurity, myTransaction, vertex.VertexID, group.Key, update.Item3, update.Item2, 0L, true);

                    result.Add(updatedVertex);

                    if (neededPropNames.CountIsGreater(0))
                    {
                        var structured = GetStructuredFromVertex(updatedVertex, vertexType, neededPropNames);
                        AddToIndex(structured, updatedVertex.VertexID, vertexType, indices, myTransaction, mySecurity);
                    }

                }
            }
            return result;
        }

        private void AddToIndex(IDictionary<string, IComparable> structured, long id, IVertexType vertexType, Dictionary<IIndex<IComparable, long>, IList<IPropertyDefinition>> indices, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            foreach (var index in indices)
            {
                var entry = CreateIndexEntry(index.Value, structured);

                if (index.Key is ISingleValueIndex<IComparable, long>)
                {
                    (index.Key as ISingleValueIndex<IComparable, long>).Add(entry, id);
                }
                else if (index.Key is IMultipleValueIndex<IComparable, long>)
                {
                    //Ask: Why do I need to create a hashset for a single value??? *aaarghhh*
                    (index.Key as IMultipleValueIndex<IComparable, long>).Add(entry, new HashSet<long>(new[] { id }));
                }
                else
                    throw new NotImplementedException("Other index types are not known.");
            }
        }

        private void RemoveFromIndex(long myVertexID, IDictionary<string, IComparable> structured, IVertexType vertexType, Dictionary<IIndex<IComparable, long>, IList<IPropertyDefinition>> indices, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            foreach (var index in indices)
            {
                var entry = CreateIndexEntry(index.Value, structured);

                if (entry != null)
                {


                if (index.Key is IMultipleValueIndex<IComparable, long>)
                {
                    lock (index.Key)
                    {
                        if (index.Key.ContainsKey(entry))
                        {
                            var toBeUpdatedIndex = (IMultipleValueIndex<IComparable, long>)index.Key;

                            var payLoad = toBeUpdatedIndex[entry];
                            payLoad.Remove(myVertexID);

                            toBeUpdatedIndex.Add(entry, payLoad, IndexAddStrategy.REPLACE);
                        }
                    }
                }
                else
                {
                    index.Key.Remove(entry);
                }
                }

            }
        }

        private IDictionary<String, IComparable> GetStructuredFromVertex(IVertex vertex, IVertexType vertexType, IEnumerable<string> neededPropNames)
        {
            var result = new Dictionary<String, IComparable>();
            foreach (var propName in neededPropNames)
            {
                var id = vertexType.GetPropertyDefinition(propName).ID;

                if (vertex.HasProperty(id))
                    result.Add(propName, vertex.GetProperty<IComparable>(id));
            }

            return result;
        }

        private IVertex UpdateVertex(IVertex vertex, VertexUpdateDefinition update, String myEdition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return _vertexStore.UpdateVertex(mySecurity, myTransaction, vertex.VertexID, vertex.VertexTypeID, update, myEdition);
        }

        private Tuple<long?, String, VertexUpdateDefinition> CreateVertexUpdateDefinition(IVertex myVertex, IVertexType myVertexType, RequestUpdate myUpdate, IPropertyProvider myPropertyCopy, TransactionToken myTransaction, SecurityToken mySecurity)
        {

            #region get removes

            List<long> toBeDeletedSingle = null;
            List<long> toBeDeletedHyper = null;
            List<long> toBeDeletedStructured = null;
            List<String> toBeDeletedUnstructured = null;
            List<long> toBeDeletedBinaries = null;

            if (myUpdate.RemovedAttributes != null)
            {
                foreach (var name in myUpdate.RemovedAttributes)
                {
                    if (myVertexType.HasAttribute(name))
                    {
                        var attr = myVertexType.GetAttributeDefinition(name);

                        switch (attr.Kind)
                        {
                            case AttributeType.Property:

                                if ((attr as IPropertyDefinition).IsMandatory)
                                    throw new MandatoryConstraintViolationException(attr.Name);

                                toBeDeletedStructured = toBeDeletedStructured ?? new List<long>();
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

            #endregion

            #region get update definitions

            IDictionary<Int64, HyperEdgeUpdateDefinition> toBeUpdatedHyper = null;
            IDictionary<Int64, SingleEdgeUpdateDefinition> toBeUpdatedSingle = null;
            IDictionary<Int64, IComparable> toBeUpdatedStructured = null ;
            IDictionary<String, Object> toBeUpdatedUnstructured = null;
            IDictionary<Int64, StreamAddDefinition> toBeUpdatedBinaries = null;
            long? revision = null;
            string edition = myUpdate.UpdatedEdition;
            string comment = myUpdate.UpdatedComment;

            #region property copy things

            if (myPropertyCopy.StructuredProperties != null)
            {
                toBeUpdatedStructured = new Dictionary<long, IComparable>();
                foreach (var prop in myPropertyCopy.StructuredProperties)
                {
                    var propDef = myVertexType.GetPropertyDefinition(prop.Key);
                    CheckPropertyType(myVertexType.Name, prop.Value, propDef);
                    toBeUpdatedStructured.Add(propDef.ID, prop.Value);
                }
            }

            toBeUpdatedUnstructured = myPropertyCopy.UnstructuredProperties;

            #endregion

            #region binary properties
            if (myUpdate.UpdatedBinaryProperties != null)
            {
                foreach (var prop in myUpdate.UpdatedBinaryProperties)
                {
                    var propDef = myVertexType.GetBinaryPropertyDefinition(prop.Key);

                    toBeUpdatedBinaries = toBeUpdatedBinaries ?? new Dictionary<long, StreamAddDefinition>();
                    toBeUpdatedBinaries.Add(propDef.ID, new StreamAddDefinition(propDef.ID, prop.Value));
                }
            }
            #endregion

            #region collections

            if (myUpdate.AddedElementsToCollectionProperties != null || myUpdate.RemovedElementsFromCollectionProperties != null)
            {
                if (myUpdate.AddedElementsToCollectionProperties != null && myUpdate.RemovedElementsFromCollectionProperties != null)
                {
                    var keys = myUpdate.AddedElementsToCollectionProperties.Keys.Intersect(myUpdate.RemovedElementsFromCollectionProperties.Keys);
                    if (keys.CountIsGreater(0))
                    {
                        //TOTO a better exception here
                        throw new Exception("You can not add and remove items simultaneously on a collection attribute.");
                    }

                    if (myUpdate.AddedElementsToCollectionProperties != null)
                    {
                        foreach (var added in myUpdate.AddedElementsToCollectionProperties)
                        {
                            var propDef = myVertexType.GetPropertyDefinition(added.Key);
                            
                            var hasValue = (propDef == null)
                                ? myVertex.HasUnstructuredProperty(added.Key)
                                : myVertex.HasProperty(propDef.ID);

                            //if it is not ICollectionWrapper something wrong with deserialization
                            var extractedValue = (!hasValue)
                                ? null
                                : (propDef == null)
                                    ? myVertex.GetUnstructuredProperty<ICollectionWrapper>(added.Key)
                                    : myVertex.GetProperty<ICollectionWrapper>(propDef.ID);

                            PropertyMultiplicity mult;
                            if (propDef != null)
                            {
                                //check types only for structured properties
                                foreach (var element in added.Value)
                                {
                                    CheckPropertyType(myVertexType.Name, element, propDef);
                                }
                                mult = propDef.Multiplicity;
                            }
                            else
                                mult = (added.Value is SetCollectionWrapper)
                                    ? PropertyMultiplicity.Set
                                    : PropertyMultiplicity.List;


                            var newValue = CreateNewCollectionWrapper(
                                (hasValue)
                                    ? extractedValue.Union(added.Value)
                                    : added.Value,
                                 mult);

                            if (propDef == null)
                            {
                                toBeUpdatedUnstructured = toBeUpdatedUnstructured ?? new Dictionary<String, object>();
                                toBeUpdatedUnstructured.Add(added.Key, newValue);
                            }
                            else
                            {
                                toBeUpdatedStructured = toBeUpdatedStructured  ?? new Dictionary<long, IComparable>();
                                toBeUpdatedStructured.Add(propDef.ID, newValue);
                            }
                            
                        }
                    }
                    if (myUpdate.RemovedElementsFromCollectionProperties != null)
                    {
                        foreach (var remove in myUpdate.RemovedElementsFromCollectionProperties)
                        {
                            var propDef = myVertexType.GetPropertyDefinition(remove.Key);
                            
                            var hasValue = (propDef == null)
                                ? myVertex.HasUnstructuredProperty(remove.Key)
                                : myVertex.HasProperty(propDef.ID);

                            //no value, nothing to remove
                            if (!hasValue)
                                continue;

                            //if it is not ICollectionWrapper something wrong with deserialization
                            var extractedValue = (propDef == null)
                                ? myVertex.GetUnstructuredProperty<ICollectionWrapper>(remove.Key)
                                : myVertex.GetProperty<ICollectionWrapper>(propDef.ID);

                            PropertyMultiplicity mult = (propDef != null)
                                ? propDef.Multiplicity
                                : (extractedValue is SetCollectionWrapper)
                                    ? PropertyMultiplicity.Set
                                    : PropertyMultiplicity.List;

                            var newValue = CreateNewCollectionWrapper(extractedValue.Except(remove.Value), mult);

                            toBeUpdatedStructured.Add(propDef.ID, newValue);

                            if (propDef == null)
                            {
                                toBeUpdatedUnstructured = toBeUpdatedUnstructured ?? new Dictionary<String, object>();
                                toBeUpdatedUnstructured.Add(remove.Key, newValue);
                            }
                            else
                            {
                                toBeUpdatedStructured = toBeUpdatedStructured  ?? new Dictionary<long, IComparable>();
                                toBeUpdatedStructured.Add(propDef.ID, newValue);
                            }
                            
                        }
                    }
                }
            }

            #endregion

            #region extract vertex properties
            #region will be ignored
            long vertexID = 0L;
            long creationDate = 0L;
            long modificationDate = 0L;
            #endregion

            ExtractVertexProperties(ref edition, ref revision, ref comment, ref vertexID, ref creationDate, ref modificationDate, toBeUpdatedStructured);
            #endregion

            #region edge magic

            if (myUpdate.AddedElementsToCollectionEdges != null || myUpdate.RemovedElementsFromCollectionEdges != null || myUpdate.UpdateOutgoingEdges != null)
            {
                VertexInformation source = new VertexInformation(myVertex.VertexTypeID, myVertex.VertexID);
                if (myUpdate.UpdateOutgoingEdges != null)
                {
                    foreach (var edge in myUpdate.UpdateOutgoingEdges)
                    {

                        var edgeDef = myVertexType.GetOutgoingEdgeDefinition(edge.EdgeName);
                        switch (edgeDef.Multiplicity)
                        {
                            case EdgeMultiplicity.SingleEdge:
                                {
                                    var targets = GetResultingVertexIDs(myTransaction, mySecurity, edge, edgeDef.TargetVertexType);
                                    if (targets == null || !targets.CountIsGreater(0))
                                    {
                                        toBeDeletedSingle = toBeDeletedSingle ?? new List<long>();
                                        toBeDeletedSingle.Add(edgeDef.ID);
                                    }
                                    else if (targets.CountIsGreater(1))
                                    {
                                        throw new Exception("Single edge can not have more than one target.");
                                    }
                                    else
                                    {
                                        ConvertUnknownProperties(edge, edgeDef.EdgeType);
                                        var structured = CreateStructuredUpdate(edge.StructuredProperties, edgeDef.EdgeType);
                                        var unstructured = CreateUnstructuredUpdate(edge.UnstructuredProperties);

                                        toBeUpdatedSingle = toBeUpdatedSingle ?? new Dictionary<long, SingleEdgeUpdateDefinition>();
                                        toBeUpdatedSingle.Add(edgeDef.ID, new SingleEdgeUpdateDefinition(source, targets.First(), edgeDef.EdgeType.ID, edge.Comment, structured, unstructured));
                                    }
                                }
                                break;
                            case EdgeMultiplicity.MultiEdge:
                                {
                                    List<SingleEdgeDeleteDefinition> internSingleDelete = null;
                                    if (myVertex.HasOutgoingEdge(edgeDef.ID))
                                    {
                                        internSingleDelete = new List<SingleEdgeDeleteDefinition>();
                                        foreach (var edgeInstance in myVertex.GetOutgoingHyperEdge(edgeDef.ID).GetTargetVertices())
                                        {
                                            internSingleDelete.Add(new SingleEdgeDeleteDefinition(source, new VertexInformation(edgeInstance.VertexTypeID, edgeInstance.VertexID)));
                                        }
                                    }

                                    List<SingleEdgeUpdateDefinition> internSingleUpdate = null;
                                    var targets = GetResultingVertexIDs(myTransaction, mySecurity, edge, edgeDef.TargetVertexType);

                                    if (targets != null)
                                    {
                                        foreach (var target in targets)
                                        {
                                            internSingleUpdate = internSingleUpdate ?? new List<SingleEdgeUpdateDefinition>();
                                            internSingleUpdate.Add(new SingleEdgeUpdateDefinition(source, target, edgeDef.InnerEdgeType.ID));
                                        }
                                    }
                                    if (edge.ContainedEdges != null)
                                    {
                                        foreach (var innerEdge in edge.ContainedEdges)
                                        {
                                            targets = GetResultingVertexIDs(myTransaction, mySecurity, innerEdge, edgeDef.TargetVertexType);
                                            if (targets != null && targets.CountIsGreater(0))
                                            {
                                                ConvertUnknownProperties(innerEdge, edgeDef.InnerEdgeType);
                                                var structured = CreateStructuredUpdate(innerEdge.StructuredProperties, edgeDef.InnerEdgeType);
                                                var unstructured = CreateUnstructuredUpdate(innerEdge.UnstructuredProperties);

                                                foreach (var target in targets)
                                                {
                                                    internSingleUpdate = internSingleUpdate ?? new List<SingleEdgeUpdateDefinition>();
                                                    internSingleUpdate.Add(new SingleEdgeUpdateDefinition(source, target, edgeDef.InnerEdgeType.ID, innerEdge.Comment, structured, unstructured));

                                                }
                                            }

                                        }
                                    }
                                    ConvertUnknownProperties(edge, edgeDef.EdgeType);
                                    var outerStructured = CreateStructuredUpdate(edge.StructuredProperties, edgeDef.EdgeType);
                                    var outerUnstructured = CreateUnstructuredUpdate(edge.UnstructuredProperties);

                                    toBeUpdatedHyper = toBeUpdatedHyper ?? new Dictionary<long, HyperEdgeUpdateDefinition>();
                                    toBeUpdatedHyper.Add(edgeDef.ID, new HyperEdgeUpdateDefinition(edgeDef.EdgeType.ID, edge.Comment, outerStructured, outerUnstructured, internSingleDelete, internSingleUpdate));
                                }
                                break;
                            case EdgeMultiplicity.HyperEdge:
                                break;
                            default:
                                throw new Exception("The enumeration EdgeMultiplicity was changed, but not this switch statement.");
                        }

                    }
                }
                if (myUpdate.AddedElementsToCollectionEdges != null)
                {
                    foreach (var hyperEdge in myUpdate.AddedElementsToCollectionEdges)
                    {
                        var edgeDef = myVertexType.GetOutgoingEdgeDefinition(hyperEdge.Key);

                        if (edgeDef == null)
                            //TODO a better exception here
                            throw new Exception("edge attribute not defined.");

                        if (edgeDef.Multiplicity == EdgeMultiplicity.SingleEdge)
                            //TODO a better exception here
                            throw new Exception("Add edges is only defined on hyper/multi edges.");

                        var edgeTypeID = edgeDef.ID;
                        StructuredPropertiesUpdate structuredUpdate;
                        UnstructuredPropertiesUpdate unstructuredUpdate;
                        IEnumerable<SingleEdgeUpdateDefinition> singleUpdate;

                        CreateSingleEdgeUpdateDefinitions(source, myTransaction, mySecurity, hyperEdge.Value, edgeDef, out structuredUpdate, out unstructuredUpdate, out singleUpdate);

                        toBeUpdatedHyper = toBeUpdatedHyper ?? new Dictionary<long, HyperEdgeUpdateDefinition>();
                        toBeUpdatedHyper.Add(edgeTypeID, new HyperEdgeUpdateDefinition(edgeTypeID, null, structuredUpdate, unstructuredUpdate, null, singleUpdate));

                    }

                    if (myUpdate.RemovedElementsFromCollectionEdges != null)
                    {
                        foreach (var hyperEdge in myUpdate.RemovedElementsFromCollectionEdges)
                        {
                            var edgeDef = myVertexType.GetOutgoingEdgeDefinition(hyperEdge.Key);

                            if (edgeDef == null)
                                //TODO a better exception here
                                throw new Exception("Edge attribute not defined.");

                            if (edgeDef.Multiplicity == EdgeMultiplicity.SingleEdge)
                                //TODO a better exception here
                                throw new Exception("Removing edges is only defined on hyper/multi edges.");

                            var edgeTypeID = edgeDef.ID;

                            var del = CreateSingleEdgeDeleteDefinitions(source, myTransaction, mySecurity, hyperEdge.Value, edgeDef);
    
                            toBeUpdatedHyper = toBeUpdatedHyper ?? new Dictionary<long, HyperEdgeUpdateDefinition>();
                            toBeUpdatedHyper.Add(edgeTypeID, new HyperEdgeUpdateDefinition(edgeTypeID, null, null, null, del, null));

                        }
                    }
                }
            }

            #endregion

            #region create updates

            var updateSingle = (toBeUpdatedSingle != null || toBeDeletedSingle != null)
                ? new SingleEdgeUpdate(toBeUpdatedSingle, toBeDeletedSingle)
                : null;

            var updateHyper = (toBeUpdatedHyper != null || toBeDeletedHyper != null)
                ? new HyperEdgeUpdate(toBeUpdatedHyper, toBeDeletedHyper)
                : null;

            var updateStructured = (toBeUpdatedStructured != null || toBeDeletedStructured != null)
                ? new StructuredPropertiesUpdate(toBeUpdatedStructured, toBeDeletedStructured)
                : null;

            var updateUnstructured = (toBeUpdatedUnstructured != null || toBeDeletedUnstructured != null)
                ? new UnstructuredPropertiesUpdate(toBeUpdatedUnstructured, toBeDeletedUnstructured)
                : null;

            var updateBinaries = (toBeUpdatedBinaries != null || toBeDeletedBinaries != null)
                ? new BinaryPropertiesUpdate(toBeUpdatedBinaries, toBeDeletedBinaries)
                : null;

            #endregion

            return Tuple.Create(revision, edition, new VertexUpdateDefinition(comment, updateStructured, updateUnstructured, updateBinaries, updateSingle, updateHyper));
        }

        //HACK: this code style = lassitude * lack of time
        private void CreateEdgeUpdateDefinition(
            IVertex myVertex, IVertexType myVertexType, RequestUpdate myUpdate, IPropertyProvider myPropertyCopy, TransactionToken myTransaction, SecurityToken mySecurity,
            out IDictionary<long, SingleEdgeUpdateDefinition> outUpdatedSingle,
            out IDictionary<long, HyperEdgeUpdateDefinition> outUpdatedHyper,
            out IDictionary<long, IComparable> outUpdatedStructured,
            out IDictionary<string, object> outUpdatedUnstructured,
            out IDictionary<long, StreamAddDefinition> outUpdatedBinaries,
            out long? outRevision,
            out String outEdition,
            out String outComment)
        {
            #region predefine

            IDictionary<long, SingleEdgeUpdateDefinition> toBeUpdatedSingle = null;
            IDictionary<long, HyperEdgeUpdateDefinition> toBeUpdatedHyper = null;
            IDictionary<long, IComparable> toBeUpdatedStructured = null;
            IDictionary<string, object> toBeUpdatedUnstructured = null;
            IDictionary<long, StreamAddDefinition> toBeUpdatedBinaries = null;
            long? revision = null;
            String edition = myUpdate.UpdatedEdition;
            String comment = myUpdate.UpdatedComment;

            #endregion

            #endregion

            #region return

            outUpdatedSingle       = toBeUpdatedSingle;
            outUpdatedHyper        = toBeUpdatedHyper;
            outUpdatedStructured   = toBeUpdatedStructured;
            outUpdatedUnstructured = toBeUpdatedUnstructured;
            outUpdatedBinaries     = toBeUpdatedBinaries;
            outRevision = revision;
            outEdition = edition;
            outComment = comment;

            #endregion
        }

        private IEnumerable<SingleEdgeDeleteDefinition> CreateSingleEdgeDeleteDefinitions(VertexInformation mySource, TransactionToken myTransaction,SecurityToken mySecurity,EdgePredefinition myEdge,IOutgoingEdgeDefinition edgeDef)
        {
            List<SingleEdgeDeleteDefinition> result = new List<SingleEdgeDeleteDefinition>();

            switch (edgeDef.Multiplicity)
            {
                case EdgeMultiplicity.HyperEdge:
                    break;
                case EdgeMultiplicity.MultiEdge:
                    var targets = GetResultingVertexIDs(myTransaction, mySecurity, myEdge, edgeDef.TargetVertexType);
                    if (targets != null)
                        foreach (var target in targets)
                        {
                            result.Add(new SingleEdgeDeleteDefinition(mySource, target));
                        }

                    foreach (var innerEdge in myEdge.ContainedEdges)
                    {
                        targets = GetResultingVertexIDs(myTransaction, mySecurity, innerEdge, edgeDef.TargetVertexType);

                        if (targets != null)
                            foreach (var target in targets)
                            {
                                result.Add(new SingleEdgeDeleteDefinition(mySource, target));
                            }
                    }
                    break;
                default : throw new Exception("The EdgeMultiplicity enumeration was changed, but not this switch statement.");
            }

            return result;
        }

        private void CreateSingleEdgeUpdateDefinitions(VertexInformation mySource, TransactionToken myTransaction, SecurityToken mySecurity, EdgePredefinition myEdge, IOutgoingEdgeDefinition edgeDef, out StructuredPropertiesUpdate outStructuredUpdate, out UnstructuredPropertiesUpdate outUnstructuredUpdate, out IEnumerable<SingleEdgeUpdateDefinition> outSingleUpdate)
        {
            #region predefine
            List<SingleEdgeUpdateDefinition> singleUpdate = new List<SingleEdgeUpdateDefinition>();
            #endregion

            outStructuredUpdate = null;
            outUnstructuredUpdate = null;

            switch (edgeDef.Multiplicity)
            {
                case EdgeMultiplicity.HyperEdge:
                    break;
                case EdgeMultiplicity.MultiEdge:
                    var targets = GetResultingVertexIDs(myTransaction, mySecurity, myEdge, edgeDef.TargetVertexType);
                    if (targets != null)
                        foreach (var target in targets)
                        {
                            singleUpdate.Add(new SingleEdgeUpdateDefinition(mySource, target, Int64.MinValue));
                        }

                    if (myEdge.ContainedEdges != null)
                    {

                        foreach (var innerEdge in myEdge.ContainedEdges)
                        {
                            targets = GetResultingVertexIDs(myTransaction, mySecurity, innerEdge, edgeDef.TargetVertexType);
                            ConvertUnknownProperties(innerEdge, edgeDef.InnerEdgeType);

                            var innerStructuredUpdate = CreateStructuredUpdate(innerEdge.StructuredProperties, edgeDef.InnerEdgeType);
                            var innerUnstructuredUpdate = CreateUnstructuredUpdate(innerEdge.UnstructuredProperties);
                            if (targets != null)
                                foreach (var target in targets)
                                {
                                    singleUpdate.Add(new SingleEdgeUpdateDefinition(mySource, target, Int64.MinValue, null, innerStructuredUpdate, innerUnstructuredUpdate));
                                }
                        }
                    }

                    outStructuredUpdate = CreateStructuredUpdate(myEdge.StructuredProperties, edgeDef.EdgeType);
                    outUnstructuredUpdate = CreateUnstructuredUpdate(myEdge.UnstructuredProperties);

                    break;
                default: throw new Exception("The EdgeMultiplicity enumeration was changed, but not this switch statement.");
            }

            #region return

            outSingleUpdate = singleUpdate;

            #endregion
        }

        private UnstructuredPropertiesUpdate CreateUnstructuredUpdate(IDictionary<string, object> myUnstructured)
        {
            if (myUnstructured == null)
                return null;

            return new UnstructuredPropertiesUpdate(myUnstructured);
        }

        private StructuredPropertiesUpdate CreateStructuredUpdate(IDictionary<string, IComparable> myStructured, IBaseType myVertexType)
        {
            if (myStructured == null)
                return null;

            return new StructuredPropertiesUpdate(myStructured.ToDictionary(_=>myVertexType.GetAttributeDefinition(_.Key).ID, _=>_.Value));
        }

        private ICollectionWrapper CreateNewCollectionWrapper(IEnumerable<IComparable> myValues, PropertyMultiplicity myMultiplicity)
        {
            switch (myMultiplicity)
            {
                case PropertyMultiplicity.List:
                    return new ListCollectionWrapper(myValues);
                case PropertyMultiplicity.Set:
                    return new SetCollectionWrapper(myValues);
                case PropertyMultiplicity.Single:
                    throw new Exception("This property is not a collection property");
                default:
                    throw new Exception("The enumeration PropertyMultiplicity was changed, but not this switch statement.");
            }
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


            #region return

            outDeletedSingle = toBeDeletedSingle;
            outDeletedHyper = toBeDeletedHyper;
            outDeletedStructured = toBeDeletedStructured;
            outDeletedUnstructured = toBeDeletedUnstructured;
            outDeletedBinaries = toBeDeletedBinaries;

            #endregion

        }

        #endregion
    }
}
