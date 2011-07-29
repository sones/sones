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
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.Expression;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphDB.Expression.Tree;
using sones.GraphDB.Request;
using sones.Library.Commons.VertexStore;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.Manager.TypeManagement;
using sones.GraphDB.Request.Insert;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.Manager.QueryPlan;
using sones.Library.LanguageExtensions;

namespace sones.GraphDB.Manager.Vertex
{
    internal abstract class AVertexHandler: IVertexHandler
    {
        #region IVertexTypeHandler member

        #region Get Vertices

        /// <summary>
        /// Gets all vertices correspondig to a vertex type
        /// </summary>
        /// <param name="myVertexType">The interesting vertex type</param>
        /// <param name="myTransaction">The current transaction token</param>
        /// <param name="mySecurity">The current security token</param>
        /// <returns>An enumerable of all vertices</returns>
        public abstract IEnumerable<IVertex> GetVertices(IVertexType myVertexType, TransactionToken myTransaction, SecurityToken mySecurity, Boolean includeSubtypes = true);

        /// <summary>
        /// Gets all vertices depending to a request
        /// </summary>
        /// <param name="myRequest">The request</param>
        /// <param name="TransactionToken">The current transaction token</param>
        /// <param name="SecurityToken">The current security token</param>
        /// <returns>An enumerable of all vertices</returns>
        public abstract IEnumerable<IVertex> GetVertices(RequestGetVertices myRequest, TransactionToken TransactionToken, SecurityToken SecurityToken);

        /// <summary>
        /// Gets all vertices for one vertex type.
        /// </summary>
        /// <param name="myVertexType">The interesting vertex type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>
        /// All vertices of the interesting vertex type.
        /// </returns>
        public abstract IEnumerable<IVertex> GetVertices(String myVertexType, TransactionToken myTransaction, SecurityToken mySecurity, Boolean includeSubtypes = true);

        /// <summary>
        /// Gets all vertices for one vertex type ID.
        /// </summary>
        /// <param name="myVertexType">The interesting vertex type ID.</param>
        /// <param name="myTransactionToken">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        /// <returns>
        /// All vertices of the interesting vertex type.
        /// </returns>
        public abstract IEnumerable<IVertex> GetVertices(long myTypeID, TransactionToken myTransaction, SecurityToken mySecurity, Boolean includeSubtypes = true);

        /// <summary>
        /// Returns the list of vertices that matches the expression.
        /// </summary>
        /// <param name="myExpression">An logical expression tree. Migth be unoptimized.</param>
        /// <param name="myIsLongrunning">Determines whether it is anticipated that the request could take longer.</param>
        /// <param name="myTransactionToken">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        /// <returns>
        /// A possible emtpy list of vertices that matches the expression. The myResult is never <c>NULL</c>.
        /// Any implementation should try to optimize the way the underlying parentVertex store and indices are used to get the myResult.
        /// </returns>
        public abstract IEnumerable<IVertex> GetVertices(IExpression myExpression, Boolean myIsLongrunning, TransactionToken myTransactionToken, SecurityToken mySecurityToken);

        #endregion

        #region GetVertex

        /// <summary>
        /// Execution of the request
        /// </summary>
        /// <param name="myVertexTypeID">The vertex type id of the requested vertex</param>
        /// <param name="myVertexID">The id of the requested vertex</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimespan">The timespan that should be processed</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation</param>
        /// <returns>The requested vertex</returns>
        public abstract IVertex GetVertex(long myVertexTypeID, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Execution of the request
        /// </summary>
        /// <param name="myVertexTypeName">The vertex type name of the requested vertex</param>
        /// <param name="myVertexID">The id of the requested vertex</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimespan">The timespan that should be processed</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation</param>
        /// <returns>The requested vertex</returns>
        public abstract IVertex GetVertex(string myVertexTypeName, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken myTransaction, SecurityToken mySecurity);

        public abstract IVertex GetSingleVertex(IExpression myExpression, TransactionToken myTransaction, SecurityToken mySecurity);

        #endregion

        /// <summary>
        /// Adds a vertex to the FS.
        /// </summary>
        /// <param name="myInsertDefinition">The insert request.</param>
        /// <param name="TransactionToken">A transaction token for this operation.</param>
        /// <param name="SecurityToken">A security token for this operation.</param>
        /// <returns>The added vertex.</returns>
        public abstract IVertex AddVertex(RequestInsertVertex myInsertDefinition, TransactionToken myTransaction, SecurityToken mySecurity);


        /// <summary>
        /// Updates a set of vertices and returns them.
        /// </summary>
        /// <param name="myUpdate">The request that represents the update.</param>
        /// <param name="TransactionToken">A transaction token for this operation.</param>
        /// <param name="SecurityToken">A security token for this operation.</param>
        /// <returns>The updated vertivess.</returns>
        public abstract IEnumerable<IVertex> UpdateVertices(RequestUpdate myUpdate, TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Gets the vertex store this vertex manager is acting on.
        /// </summary>
        public abstract IVertexStore VertexStore { get; }

        /// <summary>
        /// Deletes a set of vertices
        /// </summary>
        /// <param name="myDeleteRequest">The request that represents the delete operation</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        public abstract void Delete(RequestDelete myDeleteRequest, SecurityToken mySecurityToken, TransactionToken myTransactionToken);

        #endregion

        protected IManagerOf<IVertexTypeHandler> _vertexTypeManager;
        protected IQueryPlanManager _queryPlanManager;


        protected IVertexType GetVertexType(String myVertexTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
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

        protected void CheckMandatoryConstraint(IPropertyProvider myPropertyProvider, IBaseType myType)
        {
            var mandatories = myType.GetPropertyDefinitions(true).Where(IsMustSetProperty).ToArray();

            foreach (var mand in mandatories)
            {
                if ( myPropertyProvider == null || myPropertyProvider.StructuredProperties == null || !myPropertyProvider.StructuredProperties.Any(x => mand.Name.Equals(x.Key)))
                {
                    throw new MandatoryConstraintViolationException(mand.Name);
                }
            }
        }

        protected static void ConvertUnknownProperties(IPropertyProvider myPropertyProvider, IBaseType myBaseType)
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

        protected static void CheckAddStructuredProperties(IDictionary<String, IComparable> myProperties, IVertexType vertexType)
        {
            foreach (var prop in myProperties)
            {
                var propertyDef = vertexType.GetPropertyDefinition(prop.Key);
                if (propertyDef == null)
                    throw new AttributeDoesNotExistException(prop.Key, vertexType.Name);

                if (propertyDef.Multiplicity == PropertyMultiplicity.Single)
                {
                    CheckPropertyType(vertexType.Name, prop.Value, propertyDef);
                }
                else
                {
                    IEnumerable<IComparable> items = prop.Value as IEnumerable<IComparable>;
                    if (items == null)
                    {
                        throw new PropertyHasWrongTypeException(vertexType.Name, prop.Key, propertyDef.Multiplicity, propertyDef.BaseType.Name);
                    }

                    foreach (var item in items)
                    {
                        CheckPropertyType(vertexType.Name, item, propertyDef);
                    }
                }
            }
        }

        protected static void CheckPropertyType(String myVertexTypeName, IComparable myValue, IPropertyDefinition propertyDef)
        {
            //Assign safty should be suffice.
            if (!propertyDef.BaseType.IsAssignableFrom(myValue.GetType()))
                throw new PropertyHasWrongTypeException(myVertexTypeName, propertyDef.Name, propertyDef.BaseType.Name, myValue.GetType().Name);
        }



        private static bool IsMustSetProperty(IPropertyDefinition myPropertyDefinition)
        {
            return IsMandatoryProperty(myPropertyDefinition) && !HasDefaultValue(myPropertyDefinition) && myPropertyDefinition.RelatedType.ID != (long)BaseTypes.Vertex;
        }

        private static bool IsMandatoryProperty(IPropertyDefinition myPropertyDefinition)
        {
            return myPropertyDefinition.IsMandatory;
        }

        private static bool HasDefaultValue(IPropertyDefinition myPropertyDefinition)
        {
            return myPropertyDefinition.DefaultValue != null;
        }



        #region IManager Members

        public virtual void Initialize(IMetaManager myMetaManager)
        {
            _vertexTypeManager = myMetaManager.VertexTypeManager;
            _queryPlanManager = myMetaManager.QueryPlanManager;
        }

        public virtual void Load(TransactionToken myTransaction, SecurityToken mySecurity)
        {
        }

        #endregion
    }
}
