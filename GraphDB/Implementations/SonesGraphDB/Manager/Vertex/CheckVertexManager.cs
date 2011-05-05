using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Expression;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.Expression.Tree;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.Library.Commons.VertexStore;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.Request.Insert;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.ErrorHandling.Expression;

namespace sones.GraphDB.Manager.Vertex
{
    class CheckVertexManager: AVertexHandler, IVertexHandler
    {
        #region IVertexHandler Members

        public IEnumerable<IVertex> GetVertices(RequestGetVertices _request, TransactionToken TransactionToken, SecurityToken SecurityToken)
        {
            #region case 1 - Expression

            if (_request.Expression != null)
            {
                if (!_queryPlanManager.IsValidExpression(_request.Expression))
                {
                    throw new InvalidExpressionException(_request.Expression);
                }
            }
            
            #endregion

            #region case 2 - No Expression

            else if (_request.VertexTypeName != null)
            {
                //2.1 typeName as string
                _vertexTypeManager.CheckManager.GetVertexType(_request.VertexTypeName, TransactionToken, SecurityToken);
            }
            else
            {
                //2.2 type as id
                _vertexTypeManager.CheckManager.GetVertexType(_request.VertexTypeID, TransactionToken, SecurityToken);
            }

            #endregion

            return null;
        }

        public IEnumerable<IVertex> GetVertices(IExpression myExpression, bool myIsLongrunning, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            return null;
        }

        public IEnumerable<IVertex> GetVertices(long myTypeID, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            _vertexTypeManager.CheckManager.GetVertexType(myTypeID, myTransaction, mySecurity);
            return null;
        }

        public IEnumerable<IVertex> GetVertices(string myVertexType, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            _vertexTypeManager.CheckManager.GetVertexType(myVertexType, myTransaction, mySecurity);
            return null;
        }

        public IVertex GetVertex(string myVertexTypeName, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return null;
        }

        public IVertex GetVertex(long myVertexTypeID, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return null;
        }

        public IVertex GetSingleVertex(IExpression myExpression, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return null;
        }

        public IVertex AddVertex(RequestInsertVertex myInsertDefinition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            IVertexType vertexType = GetVertexType(myInsertDefinition.VertexTypeName, myTransaction, mySecurity);

            if (vertexType.IsAbstract)
                throw new AbstractConstraintViolationException(myInsertDefinition.VertexTypeName);

            ConvertUnknownProperties(myInsertDefinition, vertexType);

            if (myInsertDefinition.OutgoingEdges != null)
                CheckOutgoingEdges(myInsertDefinition.OutgoingEdges, vertexType);


            if (myInsertDefinition.StructuredProperties != null)
            {
                CheckAddStructuredProperties(myInsertDefinition, vertexType);
            }

            CheckMandatoryConstraint(myInsertDefinition, vertexType);

            if (myInsertDefinition.BinaryProperties != null)
                CheckAddBinaryProperties(myInsertDefinition, vertexType);

            return null;
        }

        private void CheckOutgoingEdges(IEnumerable<EdgePredefinition> myEdges, IVertexType myVertexType)
        {
            foreach (var edge in myEdges)
            {
                var edgeDef = myVertexType.GetOutgoingEdgeDefinition(edge.EdgeName);
                switch (edgeDef.Multiplicity)
                {
                    case EdgeMultiplicity.SingleEdge:
                        {
                            CheckSingleEdge(edge, edgeDef.EdgeType);
                            break;
                        }
                    case EdgeMultiplicity.MultiEdge:
                        {
                            if (edge.ContainedEdgeCount > 0)
                            {
                                foreach (var innerEdge in edge.ContainedEdges)
                                {
                                    CheckSingleEdge(innerEdge, edgeDef.InnerEdgeType);
                                }
                            } 
                            break;
                        }
                    case EdgeMultiplicity.HyperEdge:
                        {
                            break;
                        }
                }

            }
        }

        private void CheckSingleEdge(EdgePredefinition edge, IBaseType myTargetType)
        {
            if (edge.ContainedEdgeCount > 0)
            {
                //TODO better exception here.
                throw new Exception("A single edge can not contain other edges.");
            }

            if (edge.ContainedEdgeCount > 1)
                //TODO: better exception here
                throw new Exception("More than one target vertices for a single edge is not allowed.");

            if (edge.VertexIDCount == 0 && edge.Expressions == null)
                //TODO: better exception here
                throw new Exception("A single edge needs at least one target.");

            ConvertUnknownProperties(edge, myTargetType);
        }

        public IVertexStore VertexStore
        {
            get { return null; }
        }

        #endregion

        private static void ConvertUnknownProperties(IPropertyProvider myPropertyProvider, IBaseType myBaseType)
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

        private static void CheckAddBinaryProperties(RequestInsertVertex myInsertDefinition, IVertexType vertexType)
        {
            foreach (var prop in myInsertDefinition.BinaryProperties)
            {
                var propertyDef = vertexType.GetBinaryPropertyDefinition(prop.Key);
                if (propertyDef == null)
                    throw new AttributeDoesNotExistException(prop.Key, myInsertDefinition.VertexTypeName);
            }
        }




        #region IVertexHandler Members


        public IEnumerable<IVertex> UpdateVertex(RequestUpdate myUpdate, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return null;
        }

        #endregion
    }
}
