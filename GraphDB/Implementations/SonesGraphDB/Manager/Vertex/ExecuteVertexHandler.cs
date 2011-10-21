/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.Expression;
using sones.GraphDB.Expression.Tree;
using sones.GraphDB.Extensions;
using sones.GraphDB.Manager.Index;
using sones.GraphDB.Manager.TypeManagement;
using sones.GraphDB.Request;
using sones.GraphDB.Request.Insert;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.TypeSystem;
using sones.Library.CollectionWrapper;
using sones.Library.Commons.Security;
using sones.Library.Commons.VertexStore;
using sones.Library.Commons.VertexStore.Definitions;
using sones.Library.Commons.VertexStore.Definitions.Update;
using sones.Library.LanguageExtensions;
using sones.Library.PropertyHyperGraph;
using sones.Plugins.Index;
using sones.Plugins.Index.Helper;
using sones.Plugins.Index.ErrorHandling;
using System.Threading.Tasks;
using sones.GraphFS.ErrorHandling;

namespace sones.GraphDB.Manager.Vertex
{
    internal class ExecuteVertexHandler : AVertexHandler
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

        public override IEnumerable<IVertex> GetVertices(IExpression myExpression, bool myIsLongrunning, Int64 myTransactionToken, SecurityToken mySecurityToken)
        {
            var queryPlan = _queryPlanManager.CreateQueryPlan(myExpression, myIsLongrunning, myTransactionToken, mySecurityToken);

            return queryPlan.Execute();
        }

        public override IEnumerable<IVertex> GetVertices(IVertexType myVertexType, Int64 myTransaction, SecurityToken mySecurity, Boolean includeSubtypes)
        {
            if (includeSubtypes)
            {
                return myVertexType.GetDescendantVertexTypesAndSelf().SelectMany(_ => _vertexStore.GetVerticesByTypeID(mySecurity, myTransaction, _.ID));
            }
            else
            {
                return _vertexStore.GetVerticesByTypeID(mySecurity, myTransaction, myVertexType.ID);
            }

        }

        public override IEnumerable<IVertex> GetVertices(String myVertexTypeName, Int64 myTransaction, SecurityToken mySecurity, Boolean includeSubtypes)
        {
            return GetVertices(_vertexTypeManager.ExecuteManager.GetType(myVertexTypeName, myTransaction, mySecurity), myTransaction, mySecurity, includeSubtypes);
        }

        public override IEnumerable<IVertex> GetVertices(long myTypeID, Int64 myTransaction, SecurityToken mySecurity, Boolean includeSubtypes)
        {
            return GetVertices(_vertexTypeManager.ExecuteManager.GetType(myTypeID, myTransaction, mySecurity), myTransaction, mySecurity, includeSubtypes);
        }

        public override IEnumerable<IVertex> GetVertices(RequestGetVertices _request, Int64 Int64, SecurityToken SecurityToken)
        {
            IEnumerable<IVertex> result;
            #region case 1 - Expression

            if (_request.Expression != null)
            {
                result = GetVertices(_request.Expression, _request.IsLongrunning, Int64, SecurityToken);
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
                        fetchedVertices.Add(GetVertex(_request.VertexTypeName, item, null, null, Int64, SecurityToken));
                    }

                    result = fetchedVertices;
                }
                else
                {
                    //2.1.2 no vertex ids ... take all
                    result = GetVertices(_request.VertexTypeName, Int64, SecurityToken, true);
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
                        fetchedVertices.Add(GetVertex(_request.VertexTypeID, item, null, null, Int64, SecurityToken));
                    }

                    result = fetchedVertices;
                }
                else
                {
                    //2.2.2 no vertex ids ... take all
                    result = GetVertices(_request.VertexTypeID, Int64, SecurityToken, true);
                }
            }

            #endregion

            return result;
        }

        #endregion

        #region GetVertex

        public override IVertex GetVertex(string myVertexTypeName, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, Int64 myTransactionToken, SecurityToken mySecurityToken)
        {
            return _vertexStore.GetVertex(mySecurityToken, myTransactionToken, myVertexID, _vertexTypeManager.ExecuteManager.GetType(myVertexTypeName, myTransactionToken, mySecurityToken).ID, (aEdition) => myEdition == aEdition, (aVertexRevisionID) => myTimespan.IsWithinTimeStamp(aVertexRevisionID));
        }

        public override IVertex GetVertex(long myVertexTypeID, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, Int64 Int64, SecurityToken SecurityToken)
        {
            return _vertexStore.GetVertex(SecurityToken, Int64, myVertexID, myVertexTypeID, (aEdition) => myEdition == aEdition, (aVertexRevisionID) => myTimespan.IsWithinTimeStamp(aVertexRevisionID));
        }

        public override IVertex GetSingleVertex(IExpression myExpression, Int64 myTransactionToken, SecurityToken mySecurityToken)
        {
            return GetVertices(myExpression, false, myTransactionToken, mySecurityToken).FirstOrDefault();
        }

        #endregion


        public override IVertex AddVertex(RequestInsertVertex myInsertDefinition, Int64 myTransaction, SecurityToken mySecurity)
        {
            IVertexType vertexType = GetType(myInsertDefinition.VertexTypeName, myTransaction, mySecurity);

            //we check unique constraints here 
            foreach (var unique in vertexType.GetUniqueDefinitions(true))
            {
                var key = CreateIndexEntry(unique.CorrespondingIndex.IndexedProperties, myInsertDefinition.StructuredProperties);

                var definingVertexType = unique.DefiningVertexType;

                foreach (var vtype in definingVertexType.GetDescendantVertexTypesAndSelf())
                {
                    var indices = _indexManager.GetIndices(vtype, unique.CorrespondingIndex.IndexedProperties, mySecurity, myTransaction);

                    foreach (var index in indices)
                    {
                        if (index.ContainsKey(key))
                            throw new IndexUniqueConstrainViolationException(myInsertDefinition.VertexTypeName, unique.CorrespondingIndex.Name);
                    }
                }
            }

            var addDefinition = RequestInsertVertexToVertexAddDefinition(myInsertDefinition, vertexType, myTransaction, mySecurity);

            var result = (addDefinition.Item1.HasValue)
                            ? _vertexStore.AddVertex(mySecurity, myTransaction, addDefinition.Item2, addDefinition.Item1.Value)
                            : _vertexStore.AddVertex(mySecurity, myTransaction, addDefinition.Item2);

            // add vertex to the indices of the corresponding vertextype
            foreach (var indexDef in vertexType.GetIndexDefinitions(false))
            {
                var index = _indexManager.GetIndex(indexDef.Name, mySecurity, myTransaction);
                index.Add(result);
            }

            return result;
        }



        private Tuple<long?, VertexAddDefinition> RequestInsertVertexToVertexAddDefinition(RequestInsertVertex myInsertDefinition,
                                                                                            IVertexType myVertexType,
                                                                                            Int64 myTransaction,
                                                                                            SecurityToken mySecurity)
        {
            long vertexID = (myInsertDefinition.VertexUUID.HasValue)
                ? myInsertDefinition.VertexUUID.Value
                : _idManager.GetVertexTypeUniqeID(myVertexType.ID).GetNextID();

            var source = new VertexInformation(myVertexType.ID, vertexID);
            long creationdate = DateTime.UtcNow.ToBinary();
            long modificationDate = creationdate;
            String comment = myInsertDefinition.Comment;
            String edition = myInsertDefinition.Edition;
            long? revision = null;

            IEnumerable<SingleEdgeAddDefinition> singleEdges;
            IEnumerable<HyperEdgeAddDefinition> hyperEdges;

            CreateEdgeAddDefinitions(myInsertDefinition.OutgoingEdges,
                                        myVertexType,
                                        myTransaction,
                                        mySecurity,
                                        source,
                                        creationdate,
                                        out singleEdges,
                                        out hyperEdges);

            var binaries = (myInsertDefinition.BinaryProperties == null)
                            ? null
                            : myInsertDefinition.BinaryProperties.Select(x => new StreamAddDefinition(myVertexType.GetAttributeDefinition(x.Key).ID, x.Value));

            var structured = ConvertStructuredProperties(myInsertDefinition, myVertexType);

            ExtractVertexProperties(ref edition,
                                    ref revision,
                                    ref comment,
                                    ref vertexID,
                                    ref creationdate,
                                    ref modificationDate,
                                    structured);

            //set id to maximum to allow user set UUIDs
            _idManager.GetVertexTypeUniqeID(myVertexType.ID).SetToMaxID(vertexID);

            return Tuple.Create(revision, new VertexAddDefinition(vertexID,
                                                                    myVertexType.ID,
                                                                    edition,
                                                                    hyperEdges,
                                                                    singleEdges,
                                                                    null,
                                                                    binaries,
                                                                    comment,
                                                                    creationdate,
                                                                    modificationDate,
                                                                    structured,
                                                                    myInsertDefinition.UnstructuredProperties));
        }

        private static void ExtractVertexProperties(ref String edition,
                                                    ref long? revision,
                                                    ref String comment,
                                                    ref long vertexID, ref 
                                                    long creationdate,
                                                    ref long modificationDate,
                                                    IDictionary<long, IComparable> structured)
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
                            creationdate = (long)structure.Value;
                            toDelete = structure.Key;
                            break;

                        case AttributeDefinitions.VertexDotEdition:
                            edition = structure.Value as String;
                            toDelete = structure.Key;
                            break;

                        case AttributeDefinitions.VertexDotModificationDate:
                            modificationDate = (long)structure.Value;
                            toDelete = structure.Key;
                            break;

                        case AttributeDefinitions.VertexDotRevision:
                            revision = (long)structure.Value;
                            toDelete = structure.Key;
                            break;

                        case AttributeDefinitions.VertexDotVertexID:
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
            Int64 myTransaction,
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
                            var edge = CreateSingleEdgeAddDefinition(myTransaction, 
                                                                        mySecurity, 
                                                                        date, 
                                                                        attrDef.ID, 
                                                                        edgeDef, 
                                                                        attrDef.EdgeType, 
                                                                        source,
                                                                        attrDef);

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
                            var edge = CreateMultiEdgeAddDefinition(myTransaction, 
                                                                    mySecurity, 
                                                                    source, 
                                                                    date, 
                                                                    edgeDef, 
                                                                    attrDef);

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
            Int64 myTransaction,
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

            return new HyperEdgeAddDefinition(attrDef.ID,
                                                attrDef.EdgeType.ID,
                                                source,
                                                contained,
                                                edgeDef.Comment,
                                                date,
                                                date,
                                                ConvertStructuredProperties(edgeDef, attrDef.InnerEdgeType), 
                                                edgeDef.UnstructuredProperties);
        }

        /// <summary>
        /// Creates SingleEdgeAddDefintions to create single edges.
        /// </summary>
        /// <param name="myTransaction">TransactionID</param>
        /// <param name="mySecurity">SecurityToken</param>
        /// <param name="myDate">Actual DateTime in long.</param>
        /// <param name="vertexIDs"></param>
        /// <param name="edgeDef">The EdgePredefnintion.</param>
        /// <param name="attrDef">The attribute defintion of the outgoing egde.</param>
        /// <param name="mySource">The source of the edge.</param>
        /// <returns></returns>
        private IEnumerable<SingleEdgeAddDefinition> CreateContainedEdges(
            Int64 myTransaction,
            SecurityToken mySecurity,
            long myDate,
            IEnumerable<VertexInformation> vertexIDs,
            EdgePredefinition edgeDef,
            IOutgoingEdgeDefinition attrDef,
            VertexInformation mySource)
        {
            if ((vertexIDs == null || vertexIDs.Count() == 0) && 
                (edgeDef.ContainedEdges == null || edgeDef.ContainedEdges.Count() == 0))
                return null;

            List<SingleEdgeAddDefinition> result = new List<SingleEdgeAddDefinition>();

            if (vertexIDs != null)
            {
                foreach (var vertex in vertexIDs)
                {
                    var toAdd = CreateSingleEdgeAddDefinition(myTransaction, 
                                                                mySecurity, 
                                                                myDate, 
                                                                Int64.MinValue,
                                                                edgeDef, 
                                                                attrDef.InnerEdgeType, 
                                                                mySource, 
                                                                attrDef);

                    if (toAdd.HasValue)
                        result.Add(toAdd.Value);
                }
            }

            if (edgeDef.ContainedEdges != null)
            {
                foreach (var edge in edgeDef.ContainedEdges)
                {
                    if (edge.ContainedEdges != null)
                        //TODO a better exception here
                        throw new Exception("An edge within a multi edge cannot have contained edges.");

                    var toAdd = CreateSingleEdgeAddDefinition(myTransaction, 
                                                                mySecurity, 
                                                                myDate, 
                                                                Int64.MinValue, 
                                                                edge, 
                                                                attrDef.InnerEdgeType, 
                                                                mySource, 
                                                                attrDef);

                    if (toAdd.HasValue)
                        result.Add(toAdd.Value);
                }
            }
            return result;
        }

        /// <summary>
        /// Adds edge default properties like CreationDate, ModifactionDate sao. to the predefinition if not already existing
        /// </summary>
        /// <param name="myPredef">The to be chaged Predefinition</param>
        /// <param name="myAttrDef">The outgoing edge definition</param>
        /// <param name="myDate">actual date long</param>
        private void AddDefaultPropertiesToEdgePredefinition(ref EdgePredefinition myPredef, 
                                                                IOutgoingEdgeDefinition myAttrDef, 
                                                                long myDate)
        {
            foreach (var item in new Dictionary<String, IComparable>
                                        { { "CreationDate", myDate },
                                          { "ModificationDate", myDate },
                                          { "EdgeTypeName", (myAttrDef.InnerEdgeType == null) 
                                                            ? myAttrDef.EdgeType.Name 
                                                            : myAttrDef.InnerEdgeType.Name },
                                          { "EdgeTypeID", (myAttrDef.InnerEdgeType == null) 
                                                            ? myAttrDef.EdgeType.ID 
                                                            : myAttrDef.InnerEdgeType.ID } })
            {
                if (myPredef.StructuredProperties == null || 
                    !myPredef.StructuredProperties.ContainsKey(item.Key))
                    myPredef.AddStructuredProperty(item.Key, item.Value );
            }
        }

        private SingleEdgeAddDefinition? CreateSingleEdgeAddDefinition(
            Int64 myTransaction,
            SecurityToken mySecurity,
            long date,
            long myAttributeID,
            EdgePredefinition edgeDef,
            IEdgeType myEdgeType,
            VertexInformation source,
            IOutgoingEdgeDefinition attrDef)
        {
            AddDefaultPropertiesToEdgePredefinition(ref edgeDef, attrDef, date);

            var vertexIDs = GetResultingVertexIDs(myTransaction,
                                                    mySecurity,
                                                    edgeDef,
                                                    attrDef.TargetVertexType);

            if (vertexIDs == null)
                return null;

            CheckTargetVertices(attrDef.TargetVertexType, vertexIDs);

            //adds the basic attributes like CreationDate, ModificationDate ... to the structured properties
            AddDefaultValues(ref edgeDef, myEdgeType);

            return new SingleEdgeAddDefinition(myAttributeID,
                                                myEdgeType.ID,
                                                source,
                                                vertexIDs.First(),
                                                edgeDef.Comment,
                                                date,
                                                date,
                                                ConvertStructuredProperties(edgeDef, myEdgeType),
                                                edgeDef.UnstructuredProperties);
        }

        private void AddDefaultValues(ref EdgePredefinition edgeDef, IEdgeType myEdgeType)
        {
            var mandatoryProps = myEdgeType.GetPropertyDefinitions(true).Where(_ => _.IsMandatory);

            foreach (var propertyDefinition in mandatoryProps)
            {
                if (edgeDef.StructuredProperties == null || 
                    !edgeDef.StructuredProperties.ContainsKey(propertyDefinition.Name))
                    edgeDef.AddStructuredProperty(propertyDefinition.Name, propertyDefinition.DefaultValue);
            }
        }

        private static void CheckTargetVertices(IVertexType myTargetVertexType, IEnumerable<VertexInformation> vertexIDs)
        {
            var distinctTypeIDS = new HashSet<Int64>(vertexIDs.Select(x => x.VertexTypeID));
            var allowedTypeIDs = new HashSet<Int64>(myTargetVertexType.GetDescendantVertexTypesAndSelf().Select(x => x.ID));
            distinctTypeIDS.ExceptWith(allowedTypeIDs);
            if (distinctTypeIDS.Count > 0)
                throw new Exception("A target vertex has a type, that is not assignable to the target vertex type of the edge.");
        }

        private IEnumerable<VertexInformation> GetResultingVertexIDs(Int64 myTransaction, SecurityToken mySecurity, EdgePredefinition myEdgeDef, IVertexType myTargetType = null)
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
                        var vertexType = _vertexTypeManager.ExecuteManager.GetType(kvP.Key, myTransaction, mySecurity);
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


        public override IVertexStore VertexStore
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

        public override void Delete(RequestDelete myDeleteRequest, SecurityToken mySecurityToken, Int64 myTransactionToken)
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
                    var vertexType = _vertexTypeManager.ExecuteManager.GetType(aVertexTypeGroup.Key, myTransactionToken, mySecurityToken);

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
                        myDeleteRequest.AddDeletedAttribute(attribute.ID);

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
                            foreach (var aIndexDefinition in aToBeDeletedProperty.Value.InIndices)
                            {
                                RemoveFromIndices(aVertex, 
                                    _indexManager.GetIndices(vertexType, aIndexDefinition.IndexedProperties, mySecurityToken, myTransactionToken));
                            }
                            toBeDeletedStructuredPropertiesUpdate.Add(aToBeDeletedProperty.Key);
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

                        _vertexStore.UpdateVertex(mySecurityToken, myTransactionToken, aVertex.VertexID, aVertex.VertexTypeID, update, true, aVertex.EditionName, aVertex.VertexRevisionID, false);
                    }
                }
            }
            else
            {
                //remove the nodes
                foreach (var aVertexTypeGroup in toBeProcessedVertices.GroupBy(_ => _.VertexTypeID))
                {
                    var vertexType = _vertexTypeManager.ExecuteManager.GetType(aVertexTypeGroup.Key, myTransactionToken, mySecurityToken);

                    foreach (var aVertex in aVertexTypeGroup.ToList())
                    {
                        myDeleteRequest.AddDeletedVertex(aVertex.VertexID);
                        RemoveVertex(aVertex, vertexType, mySecurityToken, myTransactionToken);
                    }
                }
            }
        }

        #endregion

        private void RemoveVertex(IVertex aVertex, IVertexType myVertexType, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            RemoveVertexFromIndices(aVertex, myVertexType, mySecurityToken, myTransactionToken);

            _vertexStore.RemoveVertex(mySecurityToken, myTransactionToken, aVertex.VertexID, aVertex.VertexTypeID);
        }

        private void RemoveVertexFromIndices(IVertex aVertex, IVertexType myVertexType, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            foreach (var aStructuredProperty in myVertexType.GetPropertyDefinitions(true))
            {
                if (aVertex.HasProperty(aStructuredProperty.ID))
                {
                    foreach (var aIndexDefinition in aStructuredProperty.InIndices)
                    {
                        RemoveFromIndices(aVertex,
                            _indexManager.GetIndices(myVertexType, aIndexDefinition.IndexedProperties, mySecurityToken, myTransactionToken));
                    }
                }
            }
        }

        public override IEnumerable<IVertex> UpdateVertices(RequestUpdate myUpdate,
                                                            Int64 myTransaction,
                                                            SecurityToken mySecurity)
        {
            var toBeUpdated = GetVertices(myUpdate.GetVerticesRequest, myTransaction, mySecurity);

            var groupedByTypeID = toBeUpdated.GroupBy(_ => _.VertexTypeID);

            if (groupedByTypeID.CountIsGreater(0))
            {
                Dictionary<IVertex, Tuple<long?, String, VertexUpdateDefinition>> updates =
                    new Dictionary<IVertex, Tuple<long?, String, VertexUpdateDefinition>>();

                foreach (var group in groupedByTypeID)
                {
                    var vertexType = _vertexTypeManager
                                        .ExecuteManager
                                        .GetType(group.Key, myTransaction, mySecurity);

                    //we copy each property in property provider, 
                    //because an unknown property can be a structured at one type but unstructured at the other.
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
                                if (!uniquemaker.CountIsGreater(0) && group.CountIsGreater(0))
                                    throw new IndexUniqueConstrainViolationException(vertexType.Name, String.Join(", ", uniquemaker));
                            }
                        }

                        var toBeUpdatedUniques = vertexType
                                                    .GetUniqueDefinitions(true)
                                                    .Where(_ => _.UniquePropertyDefinitions.Any(__ => toBeUpdatedStructuredNames.Contains(__.Name)));
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

                //force execution
                return ExecuteUpdates(groupedByTypeID, updates, myTransaction, mySecurity);
            }

            return Enumerable.Empty<IVertex>();
        }

        private IEnumerable<IVertex> ExecuteUpdates(IEnumerable<IGrouping<long, IVertex>> groups,
                                                    Dictionary<IVertex, Tuple<long?, string, VertexUpdateDefinition>> updates,
                                                    Int64 myTransaction,
                                                    SecurityToken mySecurity)
        {
            List<IVertex> result = new List<IVertex>();

            foreach (var group in groups)
            {
                var vertexType = _vertexTypeManager
                                    .ExecuteManager
                                    .GetType(group.Key, myTransaction, mySecurity);

                var indexedProps = vertexType
                                    .GetIndexDefinitions(false)
                                    .Select(_ => _.IndexedProperties);

                var indices = indexedProps
                                .ToDictionary(_ => _, _ => _indexManager
                                                            .GetIndices(vertexType,
                                                                        _,
                                                                        mySecurity,
                                                                        myTransaction));

                var updateVertex = updates[group.First()];

                var myChangedProperties =
                    (updateVertex.Item3.UpdatedStructuredProperties != null &&
                    updateVertex.Item3.UpdatedStructuredProperties.Updated != null)
                    ? updateVertex
                        .Item3
                        .UpdatedStructuredProperties
                        .Updated
                        .Select(_ => vertexType.GetPropertyDefinition(_.Key).Name)
                    : Enumerable.Empty<String>();

                var neededPropNames = indexedProps
                                        .SelectMany(_ => _)
                                        .Select(_ => _.Name)
                                        .Distinct()
                                        .Intersect(myChangedProperties);

                foreach (var vertex in group)
                {
                    var update = updates[vertex];

                    if (neededPropNames.CountIsGreater(0))
                    {
                        RemoveFromIndices(vertex, indices);
                    }

                    var updatedVertex =
                        (update.Item1 != null && update.Item1.HasValue)
                        ? _vertexStore.UpdateVertex(mySecurity,
                                                    myTransaction,
                                                    vertex.VertexID,
                                                    group.Key,
                                                    update.Item3,
                                                    true,
                                                    update.Item2,
                                                    update.Item1.Value)
                        : _vertexStore.UpdateVertex(mySecurity,
                                                    myTransaction,
                                                    vertex.VertexID,
                                                    group.Key,
                                                    update.Item3,
                                                    true,
                                                    update.Item2,
                                                    0L,
                                                    true);

                    result.Add(updatedVertex);

                    if (neededPropNames.CountIsGreater(0))
                    {
                        AddToIndices(updatedVertex, indices);
                    }

                }
            }
            return result;
        }

        /// <summary>
        /// Adds a given vertex to the defined indices.
        /// The indices are mapped to the properties.
        /// </summary>
        /// <param name="myVertex">Vertex which shall be indexed</param>
        /// <param name="myIndices">Indices which are mapped to a list of indexed properties.</param>
        private void AddToIndices(IVertex myVertex,
            IDictionary<IList<IPropertyDefinition>, IEnumerable<ISonesIndex>> myIndices)
        {
            foreach (var indexGroup in myIndices)
            {
                AddToIndices(myVertex, indexGroup.Value);
            }
        }

        /// <summary>
        /// Adds a given vertex to a collection of indices.
        /// 
        /// Adding is done in parallel.
        /// </summary>
        /// <param name="myVertex">Vertex which shall be indexed</param>
        /// <param name="myIndices">A collection of indices</param>
        private void AddToIndices(IVertex myVertex, IEnumerable<ISonesIndex> myIndices)
        {
            Parallel.ForEach<ISonesIndex>(myIndices, idx => idx.Add(myVertex));
        }

        /// <summary>
        /// Removes a given vertex from all given indices.
        /// The indices are mapped to the properties.
        /// </summary>
        /// <param name="myVertex">Vertex which shall be removed</param>
        /// <param name="myIndexGroups">Indices which are mapped to a list of indexed properties.</param>
        private void RemoveFromIndices(IVertex myVertex,
            IEnumerable<KeyValuePair<IList<IPropertyDefinition>, IEnumerable<ISonesIndex>>> myIndexGroups)
        {
            foreach (var indexGroup in myIndexGroups)
            {
                RemoveFromIndices(myVertex, indexGroup.Value);
            }
        }

        /// <summary>
        /// Removes a given vertex from all indices in a given collection.
        /// 
        /// Removing is done in parallel.
        /// </summary>
        /// <param name="myVertex">Vertex which shall be removed</param>
        /// <param name="myIndices">A collection of indices</param>
        private void RemoveFromIndices(IVertex myVertex,
            IEnumerable<ISonesIndex> myIndices)
        {
            Parallel.ForEach<ISonesIndex>(myIndices, idx => idx.Remove(myVertex));
        }

        private Tuple<long?, String, VertexUpdateDefinition> CreateVertexUpdateDefinition(
                                                                IVertex myVertex,
                                                                IVertexType myVertexType,
                                                                RequestUpdate myUpdate,
                                                                IPropertyProvider myPropertyCopy,
                                                                Int64 myTransaction,
                                                                SecurityToken mySecurity)
        {

            #region get removes

            List<long> toBeDeletedSingle = null;
            List<long> toBeDeletedHyper = null;
            List<long> toBeDeletedStructured = null;
            List<String> toBeDeletedUnstructured = null;
            List<long> toBeDeletedBinaries = null;

            if (myUpdate.RemovedAttributes != null)
            {
                #region remove each defined attribute

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

                #endregion
            }

            if (myUpdate.RemovedUnstructuredProperties != null)
            {
                #region remove each unstructured property

                foreach (var name in myUpdate.RemovedUnstructuredProperties)
                {
                    if ((myVertexType.HasAttribute(name)) && (myVertexType.GetAttributeDefinition(name).Kind == AttributeType.Property))
                    {
                        toBeDeletedUnstructured = toBeDeletedUnstructured ?? new List<String>();
                        toBeDeletedUnstructured.Add(name);
                    }
                }

                #endregion
            }

            #endregion

            #region get update definitions

            IDictionary<Int64, HyperEdgeUpdateDefinition> toBeUpdatedHyper = null;
            IDictionary<Int64, SingleEdgeUpdateDefinition> toBeUpdatedSingle = null;
            IDictionary<Int64, IComparable> toBeUpdatedStructured = null;
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
                                    : (ICollectionWrapper)propDef.GetValue(myVertex);

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
                                toBeUpdatedStructured = toBeUpdatedStructured ?? new Dictionary<long, IComparable>();
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
                                : (ICollectionWrapper)propDef.GetValue(myVertex);

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
                                toBeUpdatedStructured = toBeUpdatedStructured ?? new Dictionary<long, IComparable>();
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

            if (myUpdate.AddedElementsToCollectionEdges != null ||
                myUpdate.RemovedElementsFromCollectionEdges != null ||
                myUpdate.UpdateOutgoingEdges != null ||
                myUpdate.UpdateOutgoingEdgesProperties != null)
            {
                VertexInformation source = new VertexInformation(myVertex.VertexTypeID, myVertex.VertexID);

                #region update outgoing edges

                if (myUpdate.UpdateOutgoingEdges != null)
                {
                    foreach (var edge in myUpdate.UpdateOutgoingEdges)
                    {
                        var edgeDef = myVertexType.GetOutgoingEdgeDefinition(edge.EdgeName);

                        switch (edgeDef.Multiplicity)
                        {
                            case EdgeMultiplicity.SingleEdge:
                                {
                                    #region SingleEdge
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

                                        toBeUpdatedSingle.Add(edgeDef.ID,
                                                                new SingleEdgeUpdateDefinition(source,
                                                                                                targets.First(),
                                                                                                edgeDef.EdgeType.ID,
                                                                                                edge.Comment,
                                                                                                structured,
                                                                                                unstructured));
                                    }
                                    #endregion
                                }
                                break;

                            case EdgeMultiplicity.MultiEdge:
                                {
                                    #region MultiEdge
                                    // Why deleting the edge instances ???
                                    // they will never be inserted inside the update !!!
                                    // After delete the update will be needless because the edges are deleted !!!

                                    //List<SingleEdgeDeleteDefinition> internSingleDelete = null;
                                    //if (myVertex.HasOutgoingEdge(edgeDef.ID))
                                    //{
                                    //    internSingleDelete = new List<SingleEdgeDeleteDefinition>();
                                    //    foreach (var edgeInstance in myVertex.GetOutgoingHyperEdge(edgeDef.ID).GetTargetVertices())
                                    //    {
                                    //        internSingleDelete.Add(
                                    //            new SingleEdgeDeleteDefinition(source,
                                    //                                            new VertexInformation(edgeInstance.VertexTypeID,
                                    //                                                                    edgeInstance.VertexID)));
                                    //    }
                                    //}

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

                                                    internSingleUpdate.Add(
                                                        new SingleEdgeUpdateDefinition(source,
                                                                                        target,
                                                                                        edgeDef.InnerEdgeType.ID,
                                                                                        innerEdge.Comment,
                                                                                        structured,
                                                                                        unstructured));
                                                }
                                            }

                                        }
                                    }
                                    ConvertUnknownProperties(edge, edgeDef.EdgeType);
                                    var outerStructured = CreateStructuredUpdate(edge.StructuredProperties, edgeDef.EdgeType);
                                    var outerUnstructured = CreateUnstructuredUpdate(edge.UnstructuredProperties);

                                    toBeUpdatedHyper = toBeUpdatedHyper ?? new Dictionary<long, HyperEdgeUpdateDefinition>();

                                    toBeUpdatedHyper.Add(edgeDef.ID,
                                                            new HyperEdgeUpdateDefinition(edgeDef.EdgeType.ID,
                                                                                            edge.Comment,
                                                                                            outerStructured,
                                                                                            outerUnstructured,
                                                                                            null,//internSingleDelete,
                                                                                            internSingleUpdate));
                                    #endregion
                                }
                                break;

                            case EdgeMultiplicity.HyperEdge:
                                break;

                            default:
                                throw new Exception("The enumeration EdgeMultiplicity was changed, but not this switch statement.");
                        }
                    }
                }

                #endregion

                #region update outgoing edges properties

                if (myUpdate.UpdateOutgoingEdgesProperties != null)
                {
                    foreach (var edge in myUpdate.UpdateOutgoingEdgesProperties)
                    {
                        var edgeDef = myVertexType
                                        .GetOutgoingEdgeDefinitions(true)
                                        .Where(_ => _.ID.Equals(edge.EdgeTypeID) ||
                                                _.InnerEdgeType.ID.Equals(edge.EdgeTypeID)).FirstOrDefault();

                        switch (edgeDef.Multiplicity)
                        {
                            case EdgeMultiplicity.SingleEdge:
                                {
                                    #region SingleEdge
                                    //var targets = GetResultingVertexIDs(myTransaction, mySecurity, edge, edgeDef.TargetVertexType);

                                    //if (targets == null || !targets.CountIsGreater(0))
                                    //{
                                    //    toBeDeletedSingle = toBeDeletedSingle ?? new List<long>();
                                    //    toBeDeletedSingle.Add(edgeDef.ID);
                                    //}
                                    //else if (targets.CountIsGreater(1))
                                    //{
                                    //    throw new Exception("Single edge can not have more than one target.");
                                    //}
                                    //else
                                    //{
                                    //    ConvertUnknownProperties(edge, edgeDef.EdgeType);
                                    //    var structured = CreateStructuredUpdate(edge.StructuredProperties, edgeDef.EdgeType);
                                    //    var unstructured = CreateUnstructuredUpdate(edge.UnstructuredProperties);

                                    //    toBeUpdatedSingle = toBeUpdatedSingle ?? new Dictionary<long, SingleEdgeUpdateDefinition>();

                                    //    toBeUpdatedSingle.Add(edgeDef.ID,
                                    //                            new SingleEdgeUpdateDefinition(source,
                                    //                                                            targets.First(),
                                    //                                                            edgeDef.EdgeType.ID,
                                    //                                                            edge.Comment,
                                    //                                                            structured,
                                    //                                                            unstructured));
                                    //}
                                    #endregion
                                }
                                break;

                            case EdgeMultiplicity.MultiEdge:
                                {
                                    #region MultiEdge
                                    List<SingleEdgeUpdateDefinition> internSingleUpdate = null;

                                    var targets =
                                        myVertex
                                        .GetOutgoingEdge(edgeDef.ID)
                                        .GetTargetVertices()
                                        .Select(_ => new VertexInformation(_.VertexTypeID,
                                                                            _.VertexID));

                                    if (targets != null && targets.CountIsGreater(0))
                                    {
                                        var structured = edge.UpdatedStructuredProperties;
                                        var unstructured = edge.UpdatedUnstructuredProperties;

                                        foreach (var target in targets)
                                        {
                                            internSingleUpdate = internSingleUpdate ?? new List<SingleEdgeUpdateDefinition>();

                                            internSingleUpdate.Add(
                                                new SingleEdgeUpdateDefinition(source,
                                                                                target,
                                                                                edgeDef.InnerEdgeType.ID,
                                                                                edge.CommentUpdate,
                                                                                structured,
                                                                                unstructured));
                                        }
                                    }

                                    toBeUpdatedHyper = toBeUpdatedHyper ??
                                                        new Dictionary<long, HyperEdgeUpdateDefinition>();

                                    if (toBeUpdatedHyper.ContainsKey(edgeDef.ID))
                                    {
                                        var temp = toBeUpdatedHyper[edgeDef.ID];
                                        toBeUpdatedHyper.Remove(edgeDef.ID);
                                        toBeUpdatedHyper.Add(edgeDef.ID,
                                                            MergeToBeAddedHyperEdgeUpdates(temp,
                                                                                            internSingleUpdate));
                                    }
                                    else
                                        toBeUpdatedHyper.Add(edgeDef.ID,
                                                                new HyperEdgeUpdateDefinition(edgeDef.EdgeType.ID,
                                                                                                null,
                                                                                                null,
                                                                                                null,
                                                                                                null,
                                                                                                internSingleUpdate));

                                    #endregion
                                }
                                break;

                            case EdgeMultiplicity.HyperEdge:
                                break;

                            default:
                                throw new Exception("The enumeration EdgeMultiplicity was changed, but not this switch statement.");
                        }
                    }
                }

                #endregion

                #region update AddedElementsToCollectionEdges

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

                        CheckIfToBeAddedElementAlreadyExist(myVertex, 
                                                            edgeDef, 
                                                            hyperEdge.Value,
                                                            myTransaction,
                                                            mySecurity);

                        CreateSingleEdgeUpdateDefinitions(source,
                                                            myTransaction,
                                                            mySecurity,
                                                            hyperEdge.Value,
                                                            edgeDef,
                                                            out structuredUpdate,
                                                            out unstructuredUpdate,
                                                            out singleUpdate);

                        toBeUpdatedHyper = toBeUpdatedHyper ?? new Dictionary<long, HyperEdgeUpdateDefinition>();

                        toBeUpdatedHyper.Add(edgeTypeID, new HyperEdgeUpdateDefinition(edgeTypeID, null, structuredUpdate, unstructuredUpdate, null, singleUpdate));
                    }
                }

                #endregion

                #region update RemovedElementsFromCollectionEdges

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

                #endregion
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

            return Tuple.Create(revision,
                                edition,
                                new VertexUpdateDefinition(comment,
                                                            updateStructured,
                                                            updateUnstructured,
                                                            updateBinaries,
                                                            updateSingle,
                                                            updateHyper));
        }

        /// <summary>
        /// This method checks if a element inside the to be updated edge already exist,
        /// if it exist an exception is thrown.
        /// </summary>
        /// <param name="myVertex">the to be updated vertex.</param>
        /// <param name="myEdgeDef">The edge definition.</param>
        /// <param name="myEdgePredef">The update edge predefinition.</param>
        /// <param name="myTransaction">TransactionID</param>
        /// <param name="mySecurityToken">SecurityToken</param>
        private void CheckIfToBeAddedElementAlreadyExist(IVertex myVertex,
                                                            IOutgoingEdgeDefinition myEdgeDef,
                                                            EdgePredefinition myEdgePredef,
                                                            Int64 myTransaction,
                                                            SecurityToken mySecurityToken)
        {
            switch (myEdgeDef.Multiplicity)
            {
                case EdgeMultiplicity.HyperEdge:
                    break;
                case EdgeMultiplicity.MultiEdge:
                    var newTargets = GetResultingVertexIDs(myTransaction, mySecurityToken, myEdgePredef, myEdgeDef.TargetVertexType);
                    var existTargets = myVertex.GetOutgoingHyperEdge(myEdgeDef.ID) == null
                                        ? new List<IVertex>()
                                        : myVertex.GetOutgoingHyperEdge(myEdgeDef.ID).GetTargetVertices();

                    if (newTargets == null)
                    {
                        if (myEdgePredef.ContainedEdges != null)
                        {
                            foreach (var innerEdge in myEdgePredef.ContainedEdges)
                            {
                                newTargets = GetResultingVertexIDs(myTransaction, mySecurityToken, innerEdge, myEdgeDef.TargetVertexType);
                                
                                foreach (var target in newTargets)
                                    if (existTargets.Any(item => item.VertexID.Equals(target.VertexID) && 
                                            item.VertexTypeID.Equals(target.VertexTypeID)))
                                        throw new VertexAlreadyExistException(target.VertexTypeID, target.VertexID);
                            }
                        }
                    }
                    else
                        foreach (var target in newTargets)
                            if (existTargets.Any(item => item.VertexID.Equals(target.VertexID) &&
                                        item.VertexTypeID.Equals(target.VertexTypeID)))
                                throw new VertexAlreadyExistException(target.VertexTypeID, target.VertexID);

                    break;
                default: throw new Exception("The EdgeMultiplicity enumeration was changed, but not this switch statement.");
            }
        }

        private IEnumerable<SingleEdgeDeleteDefinition> CreateSingleEdgeDeleteDefinitions(VertexInformation mySource, Int64 myTransaction, SecurityToken mySecurity, EdgePredefinition myEdge, IOutgoingEdgeDefinition edgeDef)
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
                default: throw new Exception("The EdgeMultiplicity enumeration was changed, but not this switch statement.");
            }

            return result;
        }

        private void CreateSingleEdgeUpdateDefinitions(VertexInformation mySource, Int64 myTransaction, SecurityToken mySecurity, EdgePredefinition myEdge, IOutgoingEdgeDefinition edgeDef, out StructuredPropertiesUpdate outStructuredUpdate, out UnstructuredPropertiesUpdate outUnstructuredUpdate, out IEnumerable<SingleEdgeUpdateDefinition> outSingleUpdate)
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

                    outStructuredUpdate = CreateStructuredUpdate(myEdge.StructuredProperties, edgeDef.InnerEdgeType);
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

            return new StructuredPropertiesUpdate(myStructured.ToDictionary(_ => myVertexType.GetAttributeDefinition(_.Key).ID, _ => _.Value));
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

        private HyperEdgeUpdateDefinition MergeToBeAddedHyperEdgeUpdates(
                HyperEdgeUpdateDefinition myExisting,
                IEnumerable<SingleEdgeUpdateDefinition> myToBeAdded)
        {
            IEnumerable<SingleEdgeDeleteDefinition> del = null;
            IEnumerable<SingleEdgeUpdateDefinition> update = null;
            StructuredPropertiesUpdate structuredUp = null;
            UnstructuredPropertiesUpdate unstructuredUp = null;

            if (myExisting.ToBeUpdatedSingleEdges != null && myToBeAdded != null)
            {
                update = myExisting.ToBeUpdatedSingleEdges;
                (update as List<SingleEdgeUpdateDefinition>).AddRange(myToBeAdded);
            }

            return new HyperEdgeUpdateDefinition(myExisting.EdgeTypeID,
                                                    null,
                                                    structuredUp,
                                                    unstructuredUp,
                                                    del,
                                                    update);
        }

            #endregion
    }
}
