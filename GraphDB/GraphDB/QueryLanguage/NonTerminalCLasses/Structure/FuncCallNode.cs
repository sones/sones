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
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.QueryLanguage.Operators;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{

    public class FuncCallNode : AStructureNode
    {

        public ChainPartFuncDefinition FuncDefinition { get; private set; }

        protected String _FuncName = null;
        private String _SourceParsedString = String.Empty;

        #region Data

        //private List<Object> _Parameters = null;
        //private ABaseFunction _Function = null;
        //private List<IDChainDefinition> _ContainingIDNodes;
        //private List<EdgeKey> _Edges;

        ///// <summary>
        ///// The calling TypeAttribute. In case of User.Friends it is the attribute 'Friends'
        ///// </summary>
        //public TypeAttribute CallingAttribute
        //{
        //    get { return _CallingAttribute; }
        //    set { _CallingAttribute = value; }
        //}
        //private TypeAttribute _CallingAttribute;

        ///// <summary>
        ///// The Calling object. In case of User.Friends it is the edge 'Friends'
        ///// </summary>
        //public Object CallingObject
        //{
        //    get { return _CallingObject; }
        //    set { _CallingObject = value; }
        //}
        //private Object _CallingObject;

        ///// <summary>
        ///// The Calling db Objectstream which contains the attribute. In case of User.Friends it is the user DBObject
        ///// </summary>
        //public DBObjectStream CallingDBObjectStream
        //{
        //    get { return _CallingDBObjectStream; }
        //    set { _CallingDBObjectStream = value; }
        //}
        //private DBObjectStream _CallingDBObjectStream;

        //#region Getter / Setter

        //public String FuncName { get { return _FuncName; } }
        //public ABaseFunction Function { get { return _Function; } }

        ///// <summary>
        ///// Contains all expressions: Either an IDNode or BinExpression or an atomvalue of ADBBaseObject
        ///// For aggregates, it contains only one IDNode
        ///// </summary>
        //public List<Object> Expressions { get { return Parameters; } }

        //public override string ToString()
        //{
        //    return _SourceParsedString;
        //}


        //public String SourceParsedString
        //{
        //    get
        //    {
        //        return _SourceParsedString;
        //    }
        //}

        //public String Alias
        //{
        //    get
        //    {
        //        if (_Alias != null && _Alias != String.Empty)
        //            return _Alias;
        //        else
        //            return SourceParsedString;
        //    }
        //    set
        //    {
        //        _Alias = value;
        //    }
        //}

        //public List<IDChainDefinition> ContainingIDNodes
        //{
        //    get { return _ContainingIDNodes; }
        //}

        //public List<EdgeKey> Edges
        //{
        //    get { return _Edges; }
        //}

        //#endregion

        #endregion

        #region constructor

        public FuncCallNode()
        {
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            System.Diagnostics.Debug.Assert(parseNode.Span.Start.Line == 0);

            _SourceParsedString = context.CurrentParseTree.SourceText.Substring(parseNode.Span.Start.Position, parseNode.Span.Length);

            _FuncName = parseNode.ChildNodes[0].Token.ValueString.ToUpper();

            FuncDefinition = new ChainPartFuncDefinition(); // new
            FuncDefinition.SourceParsedString = _SourceParsedString; // new
            FuncDefinition.FuncName = _FuncName; // new

            #region Retrieve the parameters

            if (parseNode.ChildNodes.Count > 1)// && parseNode.ChildNodes[1].HasChildNodes())
            {

                if (parseNode.ChildNodes[1].Token != null)
                {
                    if (parseNode.ChildNodes[1].Token.Terminal.DisplayName == "*")
                    {
                        #region asterisk

                        if (GetTypeReferenceDefinitions(context).Count != 1)
                        {
                            throw new GraphDBException(new Error_FunctionParameterInvalidReference(("It is not allowed to execute a function with an asterisk as parameter and more than one type.")));
                        }

                        var listOfReferences = GetTypeReferenceDefinitions(context);
                        var idChainDef = new IDChainDefinition(listOfReferences.First().TypeName, listOfReferences);

                        FuncDefinition.Parameters.Add(idChainDef); // new

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
            
            Int32 paramNum = 0;

            foreach (ParseTreeNode node in parseNodes)
            {
                if (node.AstNode is IDNode)
                {
                    FuncDefinition.Parameters.Add((node.AstNode as IDNode).IDChainDefinition); // new
                }
                else if (node.AstNode is BinaryExpressionNode)
                {
                    FuncDefinition.Parameters.Add((node.AstNode as BinaryExpressionNode).BinaryExpressionDefinition); // new
                }
                else
                {
                    FuncDefinition.Parameters.Add(new ValueDefinition(node.Token.Value)); // new
                }
                paramNum++;
            }
        }

    }
}
