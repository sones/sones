/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/

/* <id name="sones GraphDB – FuncCallNode" />
 * <copyright file="FuncCallNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This represents a function call node. It will lookup the function name in a lookuptable and if there is one it will get a class reference of the representing ABaseFunction.</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Functions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.Session;
using sones.Lib;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.Structures.EdgeTypes;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{

    public struct FuncParameter
    {
        private AObject _Value;
        public AObject Value
        {
            get { return _Value; }
        }

        private TypeAttribute _TypeAttribute;
        public TypeAttribute TypeAttribute
        {
            get { return _TypeAttribute; }
        }
        /*
        /// <summary>
        /// After refactoring the type handling in BinaryOperator we can remove this getter
        /// </summary>
        public TypesOfOperatorResult TypeOfOperatorResult
        {
            get
            {
                if (_Value is ADBBaseObject)
                {
                    return (_Value as ADBBaseObject).Type;
                }
                else if (_Value is IReferenceEdge)
                {
                    return TypesOfOperatorResult.SetOfDBObjects;
                }
                else
                {
                    return TypesOfOperatorResult.Unknown;
                }
            }
        }
        */
        public FuncParameter(AObject myValue, TypeAttribute myTypeAttribute)
        {
            _Value = myValue;
            _TypeAttribute = myTypeAttribute;
        }
        
        public FuncParameter(AObject myValue)
            : this(myValue, null)
        { }
    }

    public class FuncCallNode : AStructureNode
    {
        #region Data

        protected String _FuncName = null;
        private String _SourceParsedString = String.Empty;
        private List<Object> _Expressions = null;
        private List<Object> _Parameters = null;
        private ABaseFunction _Function = null;
        private String _Alias;
        private List<IDNode> _ContainingIDNodes;
        private List<EdgeKey> _Edges;

        /// <summary>
        /// The calling TypeAttribute. In case of User.Friends it is the attribute 'Friends'
        /// </summary>
        public TypeAttribute CallingAttribute
        {
            get { return _CallingAttribute; }
            set { _CallingAttribute = value; }
        }
        private TypeAttribute _CallingAttribute;

        /// <summary>
        /// The Calling object. In case of User.Friends it is the edge 'Friends'
        /// </summary>
        public Object CallingObject
        {
            get { return _CallingObject; }
            set { _CallingObject = value; }
        }
        private Object _CallingObject;

        /// <summary>
        /// The Calling db Objectstream which contains the attribute. In case of User.Friends it is the user DBObject
        /// </summary>
        public DBObjectStream CallingDBObjectStream
        {
            get { return _CallingDBObjectStream; }
            set { _CallingDBObjectStream = value; }
        }
        private DBObjectStream _CallingDBObjectStream;

        #endregion

        #region constructor

        public FuncCallNode()
        {
            _Expressions = new List<object>();
            _Parameters = new List<object>();
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;

            if (parseNode.Span.Start.Line != 0)
                throw new NotImplementedException("parseNode.Span.Start.Line != 1");
            _SourceParsedString = context.CurrentParseTree.SourceText.Substring(parseNode.Span.Start.Position, parseNode.Span.Length);

            _FuncName = parseNode.ChildNodes[0].Token.ValueString.ToUpper();
            if (dbContext.DBPluginManager.HasFunction(_FuncName))
                _Function = dbContext.DBPluginManager.GetFunction(_FuncName);
            else if (!dbContext.DBPluginManager.HasAggregate(_FuncName))
                throw new GraphDBException(new Error_AggregateOrFunctionDoesNotExist(_FuncName));

            #region Retrieve the parameters

            _Edges = new List<EdgeKey>();

            if (parseNode.ChildNodes.Count > 1)// && parseNode.ChildNodes[1].HasChildNodes())
            {
                _ContainingIDNodes = new List<IDNode>();

                if (parseNode.ChildNodes[1].Token != null)
                {
                    if (parseNode.ChildNodes[1].Token.Terminal.DisplayName == "*")
                    {
                        #region asterisk

                        if (context.PandoraListOfReferences.Count != 1)
                        {
                            throw new GraphDBException(new Error_FunctionParameterInvalidReference(("It is not allowed to execute a function with an asterisk as parameter and more than one type.")));
                        }

                        var _generatedIDNode = GenerateIDNode(context.PandoraListOfReferences.First().Value as ATypeNode, dbContext.SessionSettings);

                        _Expressions.Add((Object)_generatedIDNode);
                        _Edges.AddRange(_generatedIDNode.Edges);
                        _ContainingIDNodes.Add(_generatedIDNode);

                        #endregion
                    }
                    else
                    {
                        GenerateData(context, parseNode.ChildNodes.Skip(1).ToList());
                    }
                }
                else
                {
                    GenerateData(context, parseNode.ChildNodes.Skip(1).ToList());
                }
            }
            else if (parseNode.ChildNodes.Count > 1)
            {
                GenerateData(context, parseNode.ChildNodes.Skip(1).ToList());
            }

            #endregion

        }

        private void GenerateData(CompilerContext context, List<ParseTreeNode> parseNodes)
        {
            if (!(this is AggregateNode))
            {
                if ((_Function!= null) && ( parseNodes.Count > _Function.GetParameters().Count))
                {
                    if (!_Function.GetParameters().IsNullOrEmpty())
                    {
                        var lastParam = _Function.GetParameters().Last();
                        if (!lastParam.VariableNumOfParams)
                        {
                            throw new GraphDBException(new Error_FunctionParameterCountMismatch(_Function, _Function.GetParameters().Count, parseNodes.Count));
                        }
                    }
                    else
                    {
                        throw new GraphDBException(new Error_FunctionParameterCountMismatch(_Function, _Function.GetParameters().Count, parseNodes.Count));
                    }
                }
            }

            Int32 paramNum = 0;

            foreach (ParseTreeNode node in parseNodes)
            {
                if (node.AstNode is IDNode)
                {

                    _Expressions.Add(node.AstNode);
                    _Edges.AddRange((node.AstNode as IDNode).Edges);
                    _ContainingIDNodes.Add((node.AstNode as IDNode));
                }
                else if (node.AstNode is BinaryExpressionNode)
                {
                    _Expressions.Add(node.AstNode);
                    _Edges.AddRange((node.AstNode as BinaryExpressionNode).ContainingIDNodes.Aggregate(new List<EdgeKey>(), (result, elem) => { result.AddRange(elem.Edges); return result; }));
                    _ContainingIDNodes.AddRange((node.AstNode as BinaryExpressionNode).ContainingIDNodes);
                }
                else
                {
                    _Expressions.Add(_Function.GetParameter(paramNum).DBType.Clone(node.Token.Value));
                }
                paramNum++;
            }
        }

        #region Getter / Setter

        public String FuncName { get { return _FuncName; } }
        public ABaseFunction Function { get { return _Function; } }

        /// <summary>
        /// Contains all expressions: Either an IDNode or BinExpression or an atomvalue of ADBBaseObject
        /// For aggregates, it contains only one IDNode
        /// </summary>
        public List<Object> Expressions { get { return _Expressions; } }

        public override string ToString()
        {
            return _SourceParsedString;
        }


        public String SourceParsedString
        {
            get
            {
                return _SourceParsedString;
            }
        }

        public String Alias
        {
            get
            {
                if (_Alias != null && _Alias != String.Empty)
                    return _Alias;
                else
                    return SourceParsedString;
            }
            set
            {
                _Alias = value;
            }
        }

        public List<IDNode> ContainingIDNodes
        {
            get { return _ContainingIDNodes; }
        }

        public List<EdgeKey> Edges
        {
            get { return _Edges; }
        }

        #endregion

        public Exceptional<FuncParameter> Execute(GraphDBType myTypeOfDBObject, DBObjectStream myDBObject, String myReference, DBContext dbContext, GraphDBType myType, DBObjectCache dbObjectCache, SessionSettings mySessionToken)
        {
            #region parameter exceptions

            #region check number of parameters

            Boolean containsVariableNumOfParams = this.Function.GetParameters().Exists(p => p.VariableNumOfParams);

            if (this.Function.GetParameters().Count != _Expressions.Count && (!containsVariableNumOfParams))
            {
                return new Exceptional<FuncParameter>(new Error_FunctionParameterCountMismatch(this.Function, this.Function.GetParameters().Count, _Expressions.Count));
            }
            else if (containsVariableNumOfParams && _Expressions.Count == 0)
            {
                return new Exceptional<FuncParameter>(new Error_FunctionParameterCountMismatch(this.Function, 1, _Expressions.Count));
            }

            #endregion

            #endregion

            List<FuncParameter> evaluatedParams = new List<FuncParameter>();
            int paramCounter = 0;
            Exceptional<FuncParameter> validationOutput;
            //ParameterValue currentParameter;
            var _warnings = new List<IWarning>();

            for (int i = 0; i < _Expressions.Count; i++)
            {
                ParameterValue currentParameter = Function.GetParameter(paramCounter);

                if (_Expressions[i] is BinaryExpressionNode)
                {
                    #region handle BinExp

                    var calculatedGraphResult = ((BinaryExpressionNode)_Expressions[i]).Calculon(dbContext, new CommonUsageGraph(dbContext));

                    if (calculatedGraphResult.Failed)
                    {
                        return new Exceptional<FuncParameter>(calculatedGraphResult);
                    }

                    var extractedDBOs = calculatedGraphResult.Value.Select(new LevelKey(myTypeOfDBObject), null, true);

                    #region validation

                    validationOutput = ValidateAndAddParameter(currentParameter, extractedDBOs, null);

                    if (validationOutput.Failed)
                    {
                        return new Exceptional<FuncParameter>(validationOutput);
                    }
                    else
                    {
                        evaluatedParams.Add(validationOutput.Value);
                    }

                    #region expressionGraph error handling

                    _warnings.AddRange(calculatedGraphResult.Value.GetWarnings());

                    #endregion

                    #endregion

                    #endregion
                }
                else
                {
                    if (_Expressions[i] is IDNode)
                    {
                        #region handle IDNode

                        IDNode tempIDNode = (IDNode)_Expressions[i];

                        if (currentParameter.DBType is DBTypeAttribute)
                        {
                            //if (myTypeOfDBObject == tempIDNode.LastType)
                            //{
                                #region validation

                                validationOutput = ValidateAndAddParameter(currentParameter, tempIDNode.LastAttribute, null);

                                if (validationOutput.Failed)
                                {
                                    return new Exceptional<FuncParameter>(validationOutput);
                                }
                                else
                                {
                                    evaluatedParams.Add(validationOutput.Value);
                                }

                                #endregion
                            //}
                        }
                        else
                        {
                            if ((tempIDNode.LastAttribute == null) && tempIDNode.IsAsteriskSet)
                            {
                                #region IDNode with asterisk

                                #region validation

                                validationOutput = ValidateAndAddParameter(currentParameter, tempIDNode, null);

                                if (validationOutput.Failed)
                                {
                                    return new Exceptional<FuncParameter>(validationOutput);
                                }
                                else
                                {
                                    evaluatedParams.Add(validationOutput.Value);
                                }

                                #endregion

                                #endregion
                            }
                            else
                            {
                                if (tempIDNode.LastAttribute.IsBackwardEdge)
                                {
                                    #region BackwardEdge

                                    TypeAttribute typeAttr = dbContext.DBTypeManager.GetTypeAttributeByEdge(tempIDNode.LastAttribute.BackwardEdgeDefinition);
                                    var dbos = myDBObject.GetBackwardEdges(tempIDNode.LastAttribute.BackwardEdgeDefinition, dbContext, dbObjectCache, (_Expressions[i] as IDNode).LastAttribute.GetDBType(dbContext.DBTypeManager));

                                    if (dbos.Failed)
                                        return new Exceptional<FuncParameter>(dbos);

                                    if (dbos.Value == null)
                                        return new Exceptional<FuncParameter>(new FuncParameter(null, tempIDNode.LastAttribute));

                                    #region validation

                                    validationOutput = ValidateAndAddParameter(currentParameter, dbos.Value, tempIDNode.LastAttribute);

                                    if (validationOutput.Failed)
                                    {
                                        return new Exceptional<FuncParameter>(validationOutput);
                                    }
                                    else
                                    {
                                        evaluatedParams.Add(validationOutput.Value);
                                    }

                                    #endregion

                                    #endregion
                                }
                                else
                                {
                                    if (myDBObject.HasAttribute(tempIDNode.LastAttribute.UUID, tempIDNode.LastType, mySessionToken))
                                    {
                                        #region DBObject has attribute

                                        #region validation

                                        validationOutput = ValidateAndAddParameter(currentParameter, myDBObject.GetAttribute(tempIDNode.LastAttribute.UUID, tempIDNode.LastType, dbContext), tempIDNode.LastAttribute);

                                        if (validationOutput.Failed)
                                        {
                                            return new Exceptional<FuncParameter>(validationOutput);
                                        }
                                        else
                                        {
                                            evaluatedParams.Add(validationOutput.Value);
                                        }

                                        #endregion

                                        #endregion
                                    }
                                    else
                                    {
                                        return new Exceptional<FuncParameter>(new Error_DBObjectDoesNotHaveAttribute(tempIDNode.LastAttribute.Name));
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region any other

                        #region validation

                        validationOutput = ValidateAndAddParameter(currentParameter, _Expressions[i], null);

                        if (validationOutput.Failed)
                        {
                            return new Exceptional<FuncParameter>(validationOutput);
                        }
                        else
                        {
                            evaluatedParams.Add(validationOutput.Value);
                        }

                        #endregion

                        #endregion

                    }
                }

                #region increase parameter counter

                if (!currentParameter.VariableNumOfParams)
                {
                    paramCounter++;
                }

                #endregion
            
            }

            _Function.CallingAttribute = _CallingAttribute;
            _Function.CallingObject = _CallingObject;
            _Function.CallingDBObjectStream = _CallingDBObjectStream;

            var result = _Function.ExecFunc(dbContext, evaluatedParams.ToArray());
            
            foreach (var _wa in _warnings)
                result.Push(_wa);

            return result;

        }

        private Exceptional<FuncParameter> ValidateAndAddParameter(ParameterValue myParameter, Object myValue, TypeAttribute myTypeAttribute)
        {
            if (!myParameter.DBType.IsValidValue(myValue))
            {
                return new Exceptional<FuncParameter>(new Error_FunctionParameterTypeMismatch(myParameter.DBType.Value.GetType(), myValue.GetType()));
            }
            myParameter.DBType.SetValue(myValue);

            return new Exceptional<FuncParameter>(new FuncParameter(myParameter.DBType, myTypeAttribute));
        }

    }
}
