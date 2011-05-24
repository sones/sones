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
using sones.Library.LanguageExtensions;

namespace sones.GraphDB.Manager.Vertex
{
    sealed class CheckVertexHandler: AVertexHandler, IVertexHandler
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

        public IEnumerable<IVertex> GetVertices(IVertexType myVertexType, TransactionToken myTransaction, SecurityToken mySecurity, Boolean myIncludeSubtypes)
        {
            if (myVertexType == null)
            {
                throw new ArgumentNullException("myVertexType");
            }
            return null;
        }

        public IEnumerable<IVertex> GetVertices(long myTypeID, TransactionToken myTransaction, SecurityToken mySecurity, Boolean myIncludeSubtypes)
        {
            _vertexTypeManager.CheckManager.GetVertexType(myTypeID, myTransaction, mySecurity);
            return null;
        }

        public IEnumerable<IVertex> GetVertices(string myVertexType, TransactionToken myTransaction, SecurityToken mySecurity, Boolean myIncludeSubtypes)
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
                CheckAddStructuredProperties(myInsertDefinition.StructuredProperties, vertexType);
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
                            if (edge.ContainedEdges != null)
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
            if (edge.ContainedEdges != null)
            {
                //TODO better exception here.
                throw new Exception("A single edge can not contain other edges.");
            }

            if (edge.VertexIDsByVertexTypeID == null && edge.VertexIDsByVertexTypeName == null)
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
                    //ASK: What's about binary properties?
                    if (myBaseType.HasProperty(unknownProp.Key))
                    {
                        var propDef = myBaseType.GetPropertyDefinition(unknownProp.Key);

                        try
                        {
                            var converted = unknownProp.Value.ConvertToIComparable(propDef.BaseType);
                            myPropertyProvider.AddStructuredProperty(unknownProp.Key, converted);
                        }
                        catch (InvalidCastException)                 
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


        private static void CheckAddBinaryProperties(RequestInsertVertex myInsertDefinition, IVertexType vertexType)
        {
            foreach (var prop in myInsertDefinition.BinaryProperties)
            {
                var propertyDef = vertexType.GetBinaryPropertyDefinition(prop.Key);
                if (propertyDef == null)
                    throw new AttributeDoesNotExistException(prop.Key, myInsertDefinition.VertexTypeName);
            }
        }

        public IEnumerable<IVertex> UpdateVertices(RequestUpdate myUpdate, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            return null;
        }

        public void Delete(RequestDelete myDeleteRequest, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            GetVertices(myDeleteRequest.ToBeDeletedVertices, myTransactionToken, mySecurityToken);
        }
    }
}
