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
using sones.Library.LanguageExtensions;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.ErrorHandling;
using ISonesGQLFunction.Structure;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using sones.Plugins.SonesGQL.Aggregates.ErrorHandling;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph.Helper;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.Expression;
using sones.GraphDB.ErrorHandling;
using sones.Library.Commons.VertexStore.Definitions;

namespace sones.GraphQL.GQL.Structure.Helper.Operator
{
    public abstract class ABinaryCompareOperator : ABinaryBaseOperator
    {

        /// <summary>
        /// This method joins two data tuples.
        /// </summary>
        /// <param name="leftData">Left data tuple.</param>
        /// <param name="rightData">Right data tuple.</param>
        /// <returns>A data tuple.</returns>
        private static DataContainer JoinData(DataContainer leftData, DataContainer rightData)
        {
            return new DataContainer(new Tuple<IDChainDefinition, IDChainDefinition>(leftData.IDChainDefinitions.Item1, rightData.IDChainDefinitions.Item1), new Tuple<AExpressionDefinition, AExpressionDefinition>(leftData.Operands.Item1, rightData.Operands.Item1), new Tuple<AExpressionDefinition, AExpressionDefinition>(leftData.Extraordinaries.Item1, rightData.Extraordinaries.Item1));
        }

        /// <summary>
        /// Extracts data for a binary expression
        /// </summary>
        /// <param name="myComplexValue">The complex part of the binary expression.</param>
        /// <param name="mySimpleValue">The simple/atomic part of the expression.</param>
        /// <param name="errors">The list of errors.</param>
        /// <param name="typeOfBinExpr">The kind of the binary expression</param>
        /// <returns>A data tuple.</returns>
        private static DataContainer ExtractData(AExpressionDefinition myComplexValue, AExpressionDefinition mySimpleValue, ref TypesOfBinaryExpression typeOfBinExpr, GQLPluginManager myPluginManager, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken, Boolean aggregateAllowed)
        {
            #region data

            //the complex IDNode (sth. like U.Age or Count(U.Friends))
            IDChainDefinition complexIDNode = null;

            //the value that is on the opposite of the complex IDNode
            AExpressionDefinition simpleValue = null;

            //a complex IDNode may result in a complexValue (i.e. Count(U.Friends) --> 3)
            AExpressionDefinition complexValue = null;

            //reference to former myComplexValue
            AExpressionDefinition extraordinaryValue = null;

            #endregion

            #region extraction

            if (myComplexValue is IDChainDefinition)
            {
                #region IDNode

                #region Data

                complexIDNode = (IDChainDefinition)myComplexValue;
                complexIDNode.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, false);
                                if (complexIDNode.Any(id => id is ChainPartFuncDefinition))
                {
                    if (complexIDNode.Edges == null || complexIDNode.Edges.Count == 0)
                    {
                        #region parameterless function

                        var fcn = (complexIDNode.First(id => id is ChainPartFuncDefinition) as ChainPartFuncDefinition);

                        // somes functions (aggregates) like SUM are not valid for where expressions, though they are not resolved
                        if (fcn.Function == null)
                        {
                            throw new FunctionDoesNotExistException(fcn.FuncName);
                        }

                        FuncParameter pResult = fcn.Function.ExecFunc(null, null, null, myGraphDB, mySecurityToken, myTransactionToken);

                        //simpleValue = new AtomValue(fcn.Function.TypeOfResult, ((FuncParameter)pResult.Value).Value); //the new simple value extraced from the function
                        simpleValue = new ValueDefinition(((FuncParameter)pResult).Value);
                        typeOfBinExpr = TypesOfBinaryExpression.Unknown; //we do not know if we are left or right associated
                        complexIDNode = null; //we resolved it... so it's null

                        #endregion
                    }
                    else
                    {
                        //extraordinaryValue = (complexIDNode.First(id => id is ChainPartFuncDefinition) as ChainPartFuncDefinition);
                        extraordinaryValue = complexIDNode;

                        if (mySimpleValue is ValueDefinition)
                        {
                            simpleValue = mySimpleValue;
                        }
                    }
                }
                else
                {
                    if (mySimpleValue is ValueDefinition)
                    {
                        try
                        {
                            if (complexIDNode.IsUndefinedAttribute)
                            {
                                throw new VertexAttributeIsNotDefinedException(complexIDNode.UndefinedAttribute);
                            }

                            simpleValue = GetCorrectValueDefinition(complexIDNode.LastAttribute, complexIDNode.LastType, ((ValueDefinition)mySimpleValue));
                        }
                        catch (FormatException)
                        {
                            throw new DataTypeDoesNotMatchException(((IPropertyDefinition)complexIDNode.LastAttribute).BaseType.Name, ((ValueDefinition)mySimpleValue).Value.GetType().Name);
                        }
                    }
                    else
                    {
                        if (mySimpleValue is TupleDefinition)
                        {
                            ((TupleDefinition)mySimpleValue).ConvertToAttributeType(myPluginManager, complexIDNode.LastAttribute, myGraphDB, mySecurityToken, myTransactionToken);

                            simpleValue = mySimpleValue;
                        }
                    }
                }

                #endregion


                #endregion
            }
            else if (myComplexValue is TupleDefinition)
            {
                #region TupleSetNode

                complexValue = ((TupleDefinition)myComplexValue);
                simpleValue = mySimpleValue;
                typeOfBinExpr = TypesOfBinaryExpression.Atom;

                #endregion
            }
            else if (myComplexValue is AggregateDefinition)
            {
                #region AggregateNode

                if (aggregateAllowed)
                {
                    if (((AggregateDefinition)myComplexValue).ChainPartAggregateDefinition.Parameters.Count != 1)
                    {
                        throw new GQLAggregateArgumentException("An aggregate must have exactly one expression.");
                    }

                    if (!(((AggregateDefinition)myComplexValue).ChainPartAggregateDefinition.Parameters[0] is IDChainDefinition))
                    {
                        throw new GQLAggregateArgumentException("An aggregate must have exactly one IDNode.");
                    }

                    #region Data

                    complexIDNode = (((AggregateDefinition)myComplexValue).ChainPartAggregateDefinition.Parameters[0] as IDChainDefinition);

                    if (complexIDNode == null)
                    {
                        throw new InvalidIDNodeException("Only single IDNodes are currently allowed in aggregates!");
                    }

                    #endregion

                    #region values

                    simpleValue = mySimpleValue;
                    extraordinaryValue = myComplexValue;

                    #endregion
                }
                else
                {
                    throw new AggregateNotAllowedException(((AggregateDefinition)myComplexValue).ChainPartAggregateDefinition.Aggregate.PluginShortName);
                }
                #endregion
            }
            else
            {
                throw new NotImplementedQLException("");
            }

            #endregion

            return new DataContainer(new Tuple<IDChainDefinition, IDChainDefinition>(complexIDNode, null), new Tuple<AExpressionDefinition, AExpressionDefinition>(simpleValue, complexValue), new Tuple<AExpressionDefinition, AExpressionDefinition>(extraordinaryValue, null));
        }

        private static ValueDefinition GetCorrectValueDefinition(IAttributeDefinition typeAttribute, IVertexType graphDBType, ValueDefinition myValueDefinition)
        {

            if (typeAttribute.Kind == AttributeType.IncomingEdge)
            {
                return GetCorrectValueDefinition(((IIncomingEdgeDefinition)typeAttribute).RelatedEdgeDefinition, graphDBType, myValueDefinition);
            }
            else
            {
                if (typeAttribute.Kind == AttributeType.Property)
                {
                    return myValueDefinition;
                }
                else
                {
                    throw new InvalidVertexAttributeValueException(typeAttribute.Name, myValueDefinition.Value);
                }
            }

        }

        /// <summary>
        /// Finds matching result corresponding to a binary expression.
        /// </summary>
        /// <param name="myLeftValueObject">The left value of a binary expression.</param>
        /// <param name="myRightValueObject">The right value of a binary expression.</param>
        /// <returns></returns>
        public static IExpressionGraph TypeOperation( 
            AExpressionDefinition myLeftValueObject, AExpressionDefinition myRightValueObject,
            GQLPluginManager myPluginManager,
            IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken,
            TypesOfBinaryExpression typeOfBinExpr, IExpressionGraph resultGr, TypesOfOperators mytypesOfOpertators, BinaryOperator myOperator, Boolean aggregateAllowed = true)
        {
            #region Data

            //DataContainer for all data that is used by a binary expression/comparer
            DataContainer data;

            #endregion

            #region extract data

            //data extraction with an eye on the type of the binary expression

            switch (typeOfBinExpr)
            {
                case TypesOfBinaryExpression.Atom:

                    //sth like 3 = 4
                    #region Get Atom data

                    //no further data has to be generated

                    //data = new DataContainer(null, new Tuple<Object, Object>(myLeftValueObject, myRightValueObject), null);
                    data = new DataContainer();

                    #endregion

                    break;
                case TypesOfBinaryExpression.LeftComplex:

                    //sth like U.Age = 21
                    #region Get LeftComplex data

                    data = ExtractData(myLeftValueObject, myRightValueObject, ref typeOfBinExpr, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, aggregateAllowed);

                    #endregion

                    break;
                case TypesOfBinaryExpression.RightComplex:

                    //sth like 21 = U.Age
                    #region Get RightComplex data

                    data = ExtractData(myRightValueObject, myLeftValueObject, ref typeOfBinExpr, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, aggregateAllowed);

                    #endregion

                    break;
                case TypesOfBinaryExpression.Complex:

                    //sth like U.Age = F.Alter
                    #region Get Complex data

                    var leftData = ExtractData(myLeftValueObject, myRightValueObject, ref typeOfBinExpr, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, aggregateAllowed);

                    var rightData = ExtractData(myRightValueObject, myLeftValueObject, ref typeOfBinExpr, myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, aggregateAllowed);

                    if (typeOfBinExpr == TypesOfBinaryExpression.Unknown)
                    {
                        typeOfBinExpr = SetTypeOfBinaryExpression(leftData, rightData);

                        switch (typeOfBinExpr)
                        {
                            case TypesOfBinaryExpression.Atom:

                                data = new DataContainer(new Tuple<IDChainDefinition, IDChainDefinition>(null, null), new Tuple<AExpressionDefinition, AExpressionDefinition>(leftData.Operands.Item1, leftData.Operands.Item1), new Tuple<AExpressionDefinition, AExpressionDefinition>(null, null));

                                break;
                            case TypesOfBinaryExpression.LeftComplex:

                                data = new DataContainer(new Tuple<IDChainDefinition, IDChainDefinition>(leftData.IDChainDefinitions.Item1, null), new Tuple<AExpressionDefinition, AExpressionDefinition>(rightData.Operands.Item1, null), new Tuple<AExpressionDefinition, AExpressionDefinition>(null, null));

                                break;
                            case TypesOfBinaryExpression.RightComplex:

                                data = new DataContainer(new Tuple<IDChainDefinition, IDChainDefinition>(rightData.IDChainDefinitions.Item1, null), new Tuple<AExpressionDefinition, AExpressionDefinition>(leftData.Operands.Item1, null), new Tuple<AExpressionDefinition, AExpressionDefinition>(null, null));

                                break;
                            case TypesOfBinaryExpression.Complex:
                            case TypesOfBinaryExpression.Unknown:
                            default:

                                throw new NotImplementedQLException("");
                        }

                    }
                    else
                    {
                        data = JoinData(leftData, rightData);
                    }

                    #endregion

                    break;
                default:

                    throw new ArgumentException();
            }

            #endregion

            #region match data

            switch (typeOfBinExpr)
            {
                case TypesOfBinaryExpression.Atom:

                    #region Atom

                    //do nothing 3 = 3 (or 2 != 3) doesnt bother U

                    #endregion

                    break;

                case TypesOfBinaryExpression.LeftComplex:

                    #region LeftComplex

                    MatchData(data, resultGr, myGraphDB, mySecurityToken, myTransactionToken, mytypesOfOpertators, myOperator);

                    #endregion

                    break;

                case TypesOfBinaryExpression.RightComplex:

                    #region RightComplex

                    MatchData(data, resultGr, myGraphDB, mySecurityToken, myTransactionToken, mytypesOfOpertators, myOperator);

                    #endregion

                    break;

                case TypesOfBinaryExpression.Complex:

                    #region Complex

                    throw new NotImplementedQLException("");

                    #endregion
            }

            #endregion


            return resultGr;
        }

        private static TypesOfBinaryExpression SetTypeOfBinaryExpression(DataContainer leftData, DataContainer rightData)
        {
            TypesOfBinaryExpression result;

            if (leftData.IDChainDefinitions.Item1 != null && rightData.IDChainDefinitions.Item1 != null)
            {
                result = TypesOfBinaryExpression.Complex;
            }
            else
            {
                if (leftData.IDChainDefinitions.Item1 == null && rightData.IDChainDefinitions.Item1 == null)
                {
                    result = TypesOfBinaryExpression.Atom;
                }
                else
                {
                    if (leftData.IDChainDefinitions.Item1 != null)
                    {
                        result = TypesOfBinaryExpression.LeftComplex;
                    }
                    else
                    {
                        result = TypesOfBinaryExpression.RightComplex;
                    }
                }
            }

            return result;
        }

        private static void MatchData(DataContainer data, IExpressionGraph resultGraph, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken, TypesOfOperators myTypeOfOperator, BinaryOperator myOperator)
        {
            #region data

            LevelKey myLevelKey = CreateLevelKey(data.IDChainDefinitions.Item1, myGraphDB, mySecurityToken, myTransactionToken);

            #endregion

            var vertices = myGraphDB.GetVertices<List<IVertex>>(
                mySecurityToken,
                myTransactionToken,
                new GraphDB.Request.RequestGetVertices(
                    new BinaryExpression(
                        new PropertyExpression(data.IDChainDefinitions.Item1.LastType.Name, data.IDChainDefinitions.Item1.LastAttribute.Name),
                        myOperator,
                        GenerateLiteral(data.Operands.Item1, ((IPropertyDefinition)data.IDChainDefinitions.Item1.LastAttribute).BaseType))),
                        (stats, vertexEnumerable) => vertexEnumerable.ToList());

            foreach (var aVertex in vertices)
            {
                IntegrateInGraph(aVertex, resultGraph, myLevelKey, myTypeOfOperator);
            }

            if (resultGraph.ContainsLevelKey(myLevelKey))
            {
                #region clean lower levels

                if (myTypeOfOperator == TypesOfOperators.AffectsLowerLevels)
                {
                    CleanLowerLevel(myLevelKey, resultGraph, myGraphDB, mySecurityToken, myTransactionToken);
                }

                #endregion

            }
            else
            {
                resultGraph.AddEmptyLevel(myLevelKey);
            }
        }

        private static IExpression GenerateLiteral(AExpressionDefinition aExpressionDefinition, Type myTypeOfLiteral)
        {
            if (aExpressionDefinition is ValueDefinition)
            {
                return new SingleLiteralExpression( ((ValueDefinition)aExpressionDefinition).Value.ConvertToIComparable(myTypeOfLiteral));

            }
            else
            {
                throw new NotImplementedQLException("TODO");
            }
        }

        private static void CleanLowerLevel(LevelKey myLevelKey, IExpressionGraph myGraph, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            if (myLevelKey.Level > 0)
            {
                var previousLevelKey = myLevelKey.GetPredecessorLevel(myGraphDB, mySecurityToken, myTransactionToken);
                HashSet<VertexInformation> toBeDeletedNodes = new HashSet<VertexInformation>();

                foreach (var aLowerDBO in myGraph.Select(previousLevelKey, null, false))
                {
                    if (aLowerDBO.HasOutgoingEdge(myLevelKey.LastEdge.AttributeID))
                    {
                        foreach (var aVertex in aLowerDBO.GetOutgoingEdge(myLevelKey.LastEdge.AttributeID).GetTargetVertices())
                        {
                            //took the vertextype id of the levelkey, because it is possible that the vertextypeid of the vertex is something inheritated
                            VertexInformation node = new VertexInformation(aVertex.VertexTypeID, aVertex.VertexID);

                            if (!myGraph.GetLevel(myLevelKey.Level).ExpressionLevels[myLevelKey].Nodes.ContainsKey(node))
                            {
                                //a reference occurred that is not in the higher level --> found a Zoidberg

                                toBeDeletedNodes.Add(node);
                                break;
                            }
                        }
                    }
                }

                foreach (var aToBeDeletedNode in toBeDeletedNodes)
                {
                    myGraph.GetLevel(previousLevelKey.Level).RemoveNode(previousLevelKey, aToBeDeletedNode);
                }
            }
        }

        private static LevelKey CreateLevelKey(IDChainDefinition myIDChainDefinition, IGraphDB myGraphDB, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            if (myIDChainDefinition.Level == 0)
            {
                return new LevelKey(new List<EdgeKey>() { new EdgeKey(myIDChainDefinition.Edges[0].VertexTypeID) }, myGraphDB, mySecurityToken, myTransactionToken);
            }
            else
            {
                if (myIDChainDefinition.Last() is ChainPartFuncDefinition)
                {
                    // the funtion in the last idnode part processes the last attribute

                    if (myIDChainDefinition.Level == 1)
                    {
                        return new LevelKey(new List<EdgeKey>() { new EdgeKey(myIDChainDefinition.Edges[0].VertexTypeID) }, myGraphDB, mySecurityToken, myTransactionToken);
                    }
                    else
                    {
                        return new LevelKey(myIDChainDefinition.Edges.Take(myIDChainDefinition.Level - 1), myGraphDB, mySecurityToken, myTransactionToken);
                    }
                }
                else
                {
                    return new LevelKey(myIDChainDefinition.Edges.Take(myIDChainDefinition.Level), myGraphDB, mySecurityToken, myTransactionToken);
                }
            }
        }

        private static void IntegrateInGraph(IVertex myDBObjectStream, IExpressionGraph myExpressionGraph, LevelKey myLevelKey, TypesOfOperators myTypesOfOperators)
        {
            if (myTypesOfOperators == TypesOfOperators.AffectsLowerLevels)
            {
                myExpressionGraph.AddNode(myDBObjectStream, myLevelKey, 1);
            }
            else
            {
                myExpressionGraph.AddNode(myDBObjectStream, myLevelKey, 0);
            }
        }

        /// <summary>
        /// We need to add an empty level in case, the DBO should not be integerated. Otherwise the select does not know, either the level was never touched or 
        /// not added due to an expression
        /// </summary>
        /// <param name="myExpressionGraph"></param>
        /// <param name="myLevelKey"></param>
        private void ExcludeFromGraph(IExpressionGraph myExpressionGraph, LevelKey myLevelKey)
        {
            myExpressionGraph.AddEmptyLevel(myLevelKey);
        }

        /// <summary>
        /// Data container for everything that is needed by binary expressions
        /// </summary>
        public struct DataContainer
        {
            #region IDNodes

            private Tuple<IDChainDefinition, IDChainDefinition> _IDChainDefinitions;
            public Tuple<IDChainDefinition, IDChainDefinition> IDChainDefinitions
            {
                get
                {
                    return _IDChainDefinitions;
                }
            }

            #endregion

            #region Operands

            private Tuple<AExpressionDefinition, AExpressionDefinition> _operands;

            /// <summary>
            /// Tuple of operands (in most cases ValueDefinition or tupleDefinition)
            /// </summary>
            public Tuple<AExpressionDefinition, AExpressionDefinition> Operands
            {
                get { return _operands; }
            }

            #endregion

            #region Extraordinaries

            private Tuple<AExpressionDefinition, AExpressionDefinition> _extraordinaries;

            /// <summary>
            /// Extraordinaries are things like aggregates or functions
            /// </summary>
            public Tuple<AExpressionDefinition, AExpressionDefinition> Extraordinaries
            {
                get { return _extraordinaries; }
            }

            #endregion

            #region constructor

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="IdNodes">The IDNodes of the DataContainer.</param>
            /// <param name="Operands">The Operands of the DataContainer.</param>
            /// <param name="Extraordinaries">The Extraordinaries of the DataContainer.</param>
            public DataContainer(Tuple<IDChainDefinition, IDChainDefinition> myIDChainDefinition, Tuple<AExpressionDefinition, AExpressionDefinition> Operands, Tuple<AExpressionDefinition, AExpressionDefinition> Extraordinaries)
            {
                _operands = Operands;
                _extraordinaries = Extraordinaries;
                _IDChainDefinitions = myIDChainDefinition;
            }

            #endregion
        }
    }
}
