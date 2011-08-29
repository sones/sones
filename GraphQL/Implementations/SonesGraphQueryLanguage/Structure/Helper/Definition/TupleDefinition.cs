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
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Expression;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Select;
using sones.GraphQL.Result;
using sones.GraphQL.GQL.Structure.Nodes.Misc;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.Library.PropertyHyperGraph;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using sones.Library.CollectionWrapper;

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    public sealed class TupleDefinition : AOperationDefinition, IEnumerable<TupleElement>
    {

        #region statics

        /// <summary>
        /// Create a new Tuple of type <paramref name="myTupleElementType"/> and values <paramref name="myValues"/>
        /// </summary>
        /// <param name="myTupleElementType"></param>
        /// <param name="myValues"></param>
        /// <returns></returns>
        public static TupleDefinition Create(params Object[] myValues)
        {

            var elements = new TupleDefinition();
            foreach (var value in myValues)
            {
                elements.AddElement(new TupleElement(new ValueDefinition(value)));
            }
            return elements;

        }

        #endregion

        #region Properties

        public KindOfTuple KindOfTuple { get; private set; }
        public List<TupleElement> TupleElements { get; private set; }

        #endregion

        #region Ctors

        public TupleDefinition(KindOfTuple kindOfTuple = KindOfTuple.Inclusive)
        {
            TupleElements = new List<TupleElement>();
            KindOfTuple = kindOfTuple;
        }

        public TupleDefinition(Object myObject, IVertexType myGraphType, KindOfTuple kindOfTuple = KindOfTuple.Inclusive)
            : this()
        {
            KindOfTuple = kindOfTuple;

            if (myObject is CollectionLiteralExpression)
            {
                foreach (var obj in (ICollectionWrapper)myObject)
                {
                    TupleElements.Add(new TupleElement(new ValueDefinition(obj)));
                }
            }
            else
            {
                throw new NotImplementedException(myObject.GetType().ToString());
            }
        }

        #endregion

        public void AddElement(TupleElement myTupleElement)
        {
            TupleElements.Add(myTupleElement);
        }

        #region IEnumerable<TupleElement> Members

        public IEnumerator<TupleElement> GetEnumerator()
        {
            return TupleElements.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Remove

        public void Remove(TupleElement left)
        {
            TupleElements.Remove(left);
        }

        public void Remove(ValueDefinition myValueDefinition)
        {
            TupleElements.Remove(new TupleElement(myValueDefinition));
        }

        public void Remove(TupleDefinition myTupleDefinition)
        {
            if (myTupleDefinition == this)
            {
                TupleElements = new List<TupleElement>();
            }
            else
            {
                foreach (var td in myTupleDefinition)
                {
                    Remove(td);
                }
            }
        }

        #endregion

        internal void ConvertToAttributeType(GQLPluginManager myPluginManager, IAttributeDefinition typeAttribute, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            var newTuple = new List<TupleElement>();

            for (int i = 0; i < TupleElements.Count; i++)
            {
                var tupleElement = TupleElements[i].Value;

                if (tupleElement is SelectDefinition)
                {

                    #region partial select

                    var selectManager = new SelectManager(myGraphDB, myPluginManager);

                    var selectDefinition = (tupleElement as SelectDefinition);
                    QueryResult qresult = selectManager.ExecuteSelect(mySecurityToken, myTransactionToken, selectDefinition, String.Empty);
                    if (qresult.Error != null)
                    {
                        throw qresult.Error;
                    }

                    IAttributeDefinition curAttr = ((tupleElement as SelectDefinition).SelectedElements.First().Item1 as IDChainDefinition).LastAttribute;

                    foreach (var _Vertex in qresult.Vertices)
                    {
                        if (!(_Vertex.HasProperty(curAttr.Name)))
                            continue;

                        if (curAttr != null)
                        {
                            var val = new ValueDefinition(_Vertex.GetProperty<Object>(curAttr.Name));
                            newTuple.Add(new TupleElement(val));
                        }
                        else
                        {
                            throw new NotImplementedQLException("");
                        }
                    }

                    #endregion

                }
                else if (TupleElements[i].Value is ValueDefinition)
                {
                    newTuple.Add(new TupleElement(new ValueDefinition((TupleElements[i].Value as ValueDefinition).Value)));
                }
                else
                {
                    throw new InvalidTupleException(TupleElements[i].Value.GetType().Name);
                }

            }

            TupleElements = newTuple;
        }

        public TupleDefinition Simplyfy()
        {
            if (TupleElements.Count == 1 && TupleElements[0].Value is TupleDefinition)
            {
                return (TupleElements[0].Value as TupleDefinition).Simplyfy();
            }
            return this;
        }

        /// <summary>
        /// Checks if there is a valid tuple node. Valid tuple nodes in this case look like : (Name = 'Henning', Age = 10)
        /// </summary>
        /// <param name="tupleElementList">List of tuple elements</param>
        /// <param name="myAttributes">myAttributes of the type</param>
        /// <returns>True if valid or otherwise false</returns>
        internal bool IsValidTupleNode(List<TupleElement> tupleElementList, IVertexType myGraphType, GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            foreach (TupleElement aTupleElement in tupleElementList)
            {
                if (aTupleElement.Value is BinaryExpressionDefinition)
                {
                    ((BinaryExpressionDefinition)aTupleElement.Value).Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, myGraphType);
                }
                else
                {
                    return false;
                }
            }

            return true;

        }

        /// <summary>
        /// returns a list of guids which match the tupleNode of the ListOfDBObjects object.
        /// </summary>
        public IEnumerable<Tuple<IVertex, Dictionary<String, object>>> GetCorrespondigDBObjectUUIDAsList(IVertexType myType, IVertexType validationType, GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            #region Evaluate tuple

            //ask guid-index of type
            foreach (TupleElement aTupleElement in TupleElements)
            {
                if (aTupleElement.Value is BinaryExpressionDefinition)
                {
                    #region Binary Expression

                    var aUniqueExpr = (BinaryExpressionDefinition)aTupleElement.Value;

                    aUniqueExpr.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken , validationType);

                    var _graphResult = aUniqueExpr.Calculon(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, new CommonUsageGraph(myGraphDB, mySecurityToken, myTransactionToken));

                    foreach (var aDBO in _graphResult.Select(new LevelKey(validationType.ID, myGraphDB, mySecurityToken, myTransactionToken), null, true))
                    {
                        yield return new Tuple<IVertex, Dictionary<String, object>>(aDBO, aTupleElement.Parameters);
                    }

                    #endregion
                }
                else
                {
                    #region tuple node

                    if (aTupleElement.Value is TupleDefinition)
                    {
                        var aTupleNode = (TupleDefinition)aTupleElement.Value;

                        if (IsValidTupleNode(aTupleNode.TupleElements, myType, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken))
                        {
                            #region get partial results

                            BinaryExpressionDefinition tempNode = null;

                            foreach (TupleElement aElement in aTupleNode)
                            {
                                tempNode = (BinaryExpressionDefinition)aElement.Value;

                                ValidateBinaryExpression(tempNode, validationType, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);

                                var tempGraphResult = tempNode.Calculon(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, new CommonUsageGraph(myGraphDB, mySecurityToken, myTransactionToken));

                                foreach (var aDBO in tempGraphResult.Select(new LevelKey(validationType.ID, myGraphDB, mySecurityToken, myTransactionToken), null, true))
                                {
                                    yield return new Tuple<IVertex, Dictionary<String, object>>(aDBO, aTupleElement.Parameters);
                                }
                            }

                            #endregion
                        }
                    }
                    #endregion
                }
            }

            #endregion

            yield break;

        }


        #region ValidateBinaryExpression

        protected void ValidateBinaryExpression(BinaryExpressionDefinition aUniqueExpr, IVertexType validationType, GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            switch (aUniqueExpr.TypeOfBinaryExpression)
            {

                case TypesOfBinaryExpression.LeftComplex:
                    ValidateBinaryExpressionInternal(aUniqueExpr.Left, validationType, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);

                    break;

                case TypesOfBinaryExpression.RightComplex:
                    ValidateBinaryExpressionInternal(aUniqueExpr.Right, validationType, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);

                    break;

                case TypesOfBinaryExpression.Complex:
                    ValidateBinaryExpressionInternal(aUniqueExpr.Left, validationType, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);
                    ValidateBinaryExpressionInternal(aUniqueExpr.Right, validationType, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken);

                    break;

                case TypesOfBinaryExpression.Atom:
                default:
                    break;
            }
        }

        private void ValidateBinaryExpressionInternal(AExpressionDefinition aUniqueExpr, IVertexType validationType, GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            if (aUniqueExpr is BinaryExpressionDefinition)
            {
                (aUniqueExpr as BinaryExpressionDefinition).Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, validationType);
            }
            else
            {
                var _potIdNode = aUniqueExpr as IDChainDefinition;

                if (_potIdNode != null)
                {
                    //var validationResult = _potIdNode.ValidateMe(validationType, typeManager);
                    _potIdNode.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, false, validationType);
                }
            }
        }

        #endregion

    }
}
