using System;
using System.Collections.Generic;
using System.Linq;
using sones.Library.Commons.VertexStore;
using sones.GraphDB.Manager.Index;
using sones.GraphDB.Manager.TypeManagement;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.Expression;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphDB.ErrorHandling.Expression;
using sones.GraphDB.Manager.QueryPlan;
using sones.GraphDB.Expression.Tree;
using sones.Library.Commons.VertexStore.Definitions;
using sones.GraphDB.Request;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.TypeSystem;
using sones.Plugins.Index.Interfaces;
using sones.Library.LanguageExtensions;
using sones.GraphDB.Expression.Tree.Literals;
using System.Collections;
using sones.GraphDB.Request.Insert;

namespace sones.GraphDB.Manager.Vertex
{

    /// <summary>
    /// This manager is responsible for getting (chosen) vertices from the persistence layer
    /// </summary>
    public sealed class VertexManager : IVertexManager
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
        /// Needed for VertexType interaction
        /// </summary>
        private IManagerOf<IVertexTypeHandler> _vertexTypeManager;

        /// <summary>
        /// Needed for transforming an expression into a query plan
        /// </summary>
        private IQueryPlanManager _queryPlanManager;

        private IDManager _idManager;

        #endregion

        #region c'tor

        public VertexManager(IDManager myIDManager)
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

        public void CanGetVertices(IExpression iExpression, bool myIsLongRunning, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            
        }

        #endregion

        #region GetVertex

        public void CanGetVertex(string myVertexTypeName, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            _vertexTypeManager.CheckManager.GetVertexType(myVertexTypeName, myTransactionToken, mySecurityToken);
        }

        public IVertex GetVertex(string myVertexTypeName, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            return _vertexStore.GetVertex(mySecurityToken, myTransactionToken, myVertexID, _vertexTypeManager.ExecuteManager.GetVertexType(myVertexTypeName, myTransactionToken, mySecurityToken).ID, myEdition);
        }

        public void CanGetVertex(long myVertexTypeID, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken TransactionToken, SecurityToken SecurityToken)
        {
            _vertexTypeManager.CheckManager.GetVertexType(myVertexTypeID, TransactionToken, SecurityToken);
        }

        public IVertex GetVertex(long myVertexTypeID, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken TransactionToken, SecurityToken SecurityToken)
        {
            return _vertexStore.GetVertex(SecurityToken, TransactionToken, myVertexID, myVertexTypeID, myEdition);
        }

        #endregion

        public void CanAddVertex(RequestInsertVertex myInsertDefinition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            IVertexType vertexType = GetVertexType(myInsertDefinition.VertexTypeName, myTransaction, mySecurity);

            if (vertexType.IsAbstract)
                throw new AbstractConstraintViolationException(myInsertDefinition.VertexTypeName);

            ConvertUnknownProperties(myInsertDefinition, vertexType);


            var mandatoryProps = vertexType.GetPropertyDefinitions(true).Where(IsMustSetProperty).ToArray();


            if (myInsertDefinition.StructuredProperties != null)
            {
                CheckAddStructuredProperties(myInsertDefinition, vertexType);
                CheckMandatoryConstraint(myInsertDefinition, mandatoryProps);
            }
            else
            {
                if (mandatoryProps.Length > 0)
                {
                    throw new MandatoryConstraintViolationException(String.Join(",", mandatoryProps.Select(x => x.Name)));
                }
            }

            if (myInsertDefinition.BinaryProperties != null)
                CheckAddBinaryProperties(myInsertDefinition, vertexType);
        }

        private void ConvertUnknownProperties(IPropertyProvider myPropertyProvider, IBaseType myBaseType)
        {
            if (myPropertyProvider.UnknownProperties != null)
            {
                foreach (var unknownProp in myPropertyProvider.UnknownProperties)
                {
                    if (myBaseType.HasProperty(unknownProp.Key))
                    {
                        if (unknownProp.Value is IComparable)
                        {
                            myPropertyProvider.AddStructuredProperty(unknownProp.Key, (IComparable)unknownProp.Value);
                        }
                        else
                        {
                            //TODO: better exception
                            throw new Exception("Type of property does not match.");
                        }
                    }
                    else
                    {
                        myPropertyProvider.AddUnstructuredProperty(unknownProp.Key, unknownProp.Value);
                    }
                }
                myPropertyProvider.ClearUnknown();
            }
        }

        private void CheckMandatoryConstraint(RequestInsertVertex myInsertDefinition, IEnumerable<IPropertyDefinition> myMandatoryProperties)
        {
            foreach (var mand in myMandatoryProperties)
            {
                if (!myInsertDefinition.StructuredProperties.Any(x => mand.Name.Equals(x)))
                {
                    throw new MandatoryConstraintViolationException(mand.Name);
                }
            }
        }


        private static bool IsMustSetProperty(IPropertyDefinition myPropertyDefinition)
        {
            return IsMandatoryProperty(myPropertyDefinition) && !HasDefaultValue(myPropertyDefinition);
        }

        private static bool IsMandatoryProperty(IPropertyDefinition myPropertyDefinition)
        {
            return myPropertyDefinition.IsMandatory;
        }

        private static bool HasDefaultValue(IPropertyDefinition myPropertyDefinition)
        {
            return myPropertyDefinition.DefaultValue != null;
        }

        private static void CheckAddBinaryProperties(RequestInsertVertex myInsertDefinition, IVertexType vertexType)
        {
            foreach (var prop in myInsertDefinition.BinaryProperties)
            {
                var propertyDef = vertexType.GetBinaryPropertyDefinition(prop.Key);
                if (propertyDef == null)
                    throw new AttributeDoesNotExistException(prop.Key, myInsertDefinition.VertexTypeName);
            }
        }

        private static void CheckAddStructuredProperties(RequestInsertVertex myInsertDefinition, IVertexType vertexType)
        {
            foreach (var prop in myInsertDefinition.StructuredProperties)
            {
                var propertyDef = vertexType.GetPropertyDefinition(prop.Key);
                if (propertyDef == null)
                    throw new AttributeDoesNotExistException(prop.Key, myInsertDefinition.VertexTypeName);

                if (propertyDef.Multiplicity == PropertyMultiplicity.Single)
                {
                    CheckPropertyType(myInsertDefinition, prop.Key, prop.Value, propertyDef);
                }
                else
                {
                    IEnumerable<IComparable> items = prop.Value as IEnumerable<IComparable>;
                    if (items == null)
                    {
                        throw new PropertyHasWrongTypeException(myInsertDefinition.VertexTypeName, prop.Key, propertyDef.Multiplicity, propertyDef.BaseType.Name);
                    }

                    foreach (var item in items)
                    {
                        CheckPropertyType(myInsertDefinition, prop.Key, item, propertyDef);
                    }
                }
            }
        }

        private static void CheckPropertyType(RequestInsertVertex myInsertDefinition, String myAttributeName, IComparable myValue, IPropertyDefinition propertyDef)
        {
            //Assign safty should be suffice.
            if (!propertyDef.BaseType.IsAssignableFrom(myValue.GetType()))
                throw new PropertyHasWrongTypeException(myInsertDefinition.VertexTypeName, myAttributeName, propertyDef.BaseType.Name, myValue.GetType().Name);
        }


        private IVertexType GetVertexType(String myVertexTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            try
            {
                //check if the vertex type exists.
                return _vertexTypeManager.ExecuteManager.GetVertexType(myVertexTypeName, myTransaction, mySecurity);
            }
            catch (KeyNotFoundException)
            {
                throw new VertexTypeDoesNotExistException(myVertexTypeName);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new EmptyVertexTypeNameException();
            }
        }

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

            var result = _vertexStore.AddVertex(mySecurity, myTransaction, RequestInsertVertexToVertexAddDefinition(myInsertDefinition, vertexType, myTransaction, mySecurity));


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
                    (index as IMultipleValueIndex<IComparable, Int64>).Add(key, new HashSet<Int64>{ result.VertexID });
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
                            : myInsertDefinition.BinaryProperties.Select(x => new StreamAddDefinition(myVertexType.GetAttributeDefinition(x.Key).AttributeID, x.Value));

            var structured = ConvertStructuredProperties(myInsertDefinition, myVertexType);
            
            return new VertexAddDefinition(vertexID, myVertexType.ID, myInsertDefinition.Edition, hyperEdges, singleEdges, binaries, myInsertDefinition.Comment, date, date, structured, myInsertDefinition.UnstructuredProperties);
        }

        private static Dictionary<long, IComparable> ConvertStructuredProperties(IPropertyProvider myInsertDefinition, IBaseType myType)
        {
            return  (myInsertDefinition.StructuredProperties == null)
                             ? null
                             : myInsertDefinition.StructuredProperties.ToDictionary(x => myType.GetAttributeDefinition(x.Key).AttributeID, x => x.Value);
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

                var vertexIDs = GetResultingVertexIDs(myTransaction, mySecurity, edgeDef, attrDef);
                CheckTargetVertices(attrDef, vertexIDs);
                switch (attrDef.Multiplicity)
                {
                    case EdgeMultiplicity.SingleEdge:
                        {
                            if (vertexIDs.Count > 1)
                                //TODO: better exception here
                                throw new Exception("More than one target vertices for a single edge is not allowed.");

                            if (vertexIDs.Count == 0)
                                //TODO: better exception here
                                throw new Exception("A single edge needs at least one target.");


                            var edge = CreateSingleEdgeAddDefinition(date, edgeDef, attrDef, source, vertexIDs.First());
                            singleEdges.Add(attrDef.Name, edge);
                        }
                        break;
                    case EdgeMultiplicity.HyperEdge:
                        {
                            var edge = CreateHyperEdgeAddDefinition(source, date, edgeDef, attrDef, vertexIDs);
                            hyperEdges.Add(attrDef.Name, edge);
                        }
                        break;
                    default:
                        throw new UnknownDBException("The EdgeMultiplicy enumeration was updated, but not this switch statement.");
                }
            }

            outSingleEdges = singleEdges.Select(x => x.Value);
            outHyperEdges = hyperEdges.Select(x => x.Value);
        }

        private static HyperEdgeAddDefinition CreateHyperEdgeAddDefinition(VertexInformation source, long date, EdgePredefinition edgeDef, IOutgoingEdgeDefinition attrDef, ISet<VertexInformation> vertexIDs)
        {
            return new HyperEdgeAddDefinition(attrDef.AttributeID, attrDef.EdgeType.ID, source, vertexIDs.Select(x => CreateSingleEdgeAddDefinition(date, edgeDef, attrDef, source, x)), edgeDef.Comment, date, date, ConvertStructuredProperties(edgeDef,attrDef.EdgeType), edgeDef.UnstructuredProperties);
        }

        private static SingleEdgeAddDefinition CreateSingleEdgeAddDefinition(long date, EdgePredefinition edgeDef, IOutgoingEdgeDefinition attrDef, VertexInformation source, VertexInformation target)
        {
            return new SingleEdgeAddDefinition(attrDef.AttributeID, attrDef.EdgeType.ID, source, target, edgeDef.Comment, date, date, ConvertStructuredProperties(edgeDef, attrDef.EdgeType), edgeDef.UnstructuredProperties);
        }

        private static void CheckTargetVertices(IOutgoingEdgeDefinition attrDef, IEnumerable<VertexInformation> vertexIDs)
        {
            var distinctTypeIDS = new HashSet<Int64>(vertexIDs.Select(x => x.VertexTypeID));
            var allowedTypeIDs = new HashSet<Int64>(attrDef.TargetVertexType.GetChildVertexTypes(true, true).Select(x => x.ID));
            distinctTypeIDS.ExceptWith(allowedTypeIDs);
            if (distinctTypeIDS.Count > 0)
                throw new Exception("A target vertex has a type, that is not assignable to the target vertex type of the edge.");
        }

        private ISet<VertexInformation> GetResultingVertexIDs(TransactionToken myTransaction, SecurityToken mySecurity, EdgePredefinition myEdgeDef, IOutgoingEdgeDefinition myAttributeDef)
        {
            HashSet<VertexInformation> result = new HashSet<VertexInformation>();
            if (myEdgeDef.VertexIDs != null)
            {
                result.UnionWith(myEdgeDef.VertexIDs.Select(x=>new VertexInformation(myAttributeDef.TargetVertexType.ID, x)));
            }
            if (myEdgeDef.Expressions != null)
            {
                foreach (var expr in myEdgeDef.Expressions)
                {
                    var vertices = GetVertices(expr, false, myTransaction, mySecurity);
                    result.UnionWith(vertices.Select(x => new VertexInformation(x.VertexTypeID, x.VertexID)));
                }
            }

            return result;
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

        public IVertex GetSingleVertex(IExpression myExpression, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            return GetVertices(myExpression, false, myTransactionToken, mySecurityToken).FirstOrDefault();
        }

        public IVertexStore VertexStore
        {
            get { return _vertexStore; }
        }

        #endregion

        #region IManager Members

        void IManager.Initialize(IMetaManager myMetaManager)
        {
            _indexManager      = myMetaManager.IndexManager;
            _vertexTypeManager = myMetaManager.VertexTypeManager;
            _vertexStore       = myMetaManager.VertexStore;
            _queryPlanManager  = myMetaManager.QueryPlanManager;

        }

        void IManager.Load(TransactionToken myTransaction, SecurityToken mySecurity) {}

        #endregion
    }
}
