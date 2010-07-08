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


using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.ErrorHandling;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.Exceptions;

using sones.GraphDB.ObjectManagement;
using sones.GraphFS.Objects;
using sones.GraphDB.Errors;
using sones.GraphDB.Structures;
using sones.GraphDB.Warnings;
using sones.Lib;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.GraphDB.QueryLanguage.ExpressionGraph;
using sones.Lib.Session;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.Managers;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Drop
{
    public class DeleteNode : AStatement
    {

        #region Data

        private ObjectManipulationManager _ObjectManipulationManager;

        private BinaryExpressionNode _WhereExpression;

        private Dictionary<String, List<TypeAttribute>> _DBTypeAttributeToDelete;
        private Dictionary<String, GraphDBType> _ReferenceTypeLookup;
        private Dictionary<GraphDBType, List<String>> _TypeWithUndefAttrs;

        #endregion

        #region Properties - Statement information

        public override String StatementName { get { return "Delete"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        
        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {
            var dbContext = myCompilerContext.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;

            #region Data

            var _TypeList = new HashSet<ATypeNode>();
            _DBTypeAttributeToDelete = new Dictionary<string, List<TypeAttribute>>();
            _ReferenceTypeLookup = new Dictionary<string, GraphDBType>();
            _TypeWithUndefAttrs = new Dictionary<GraphDBType, List<String>>();
            String undefAttrName = String.Empty;

            #endregion

            #region TypeList

            foreach (ParseTreeNode _ParseTreeNode in myParseTreeNode.ChildNodes[1].ChildNodes)
            {

                ATypeNode ATypeNode;

                if (_ParseTreeNode.AstNode is ATypeNode)
                    ATypeNode = (ATypeNode)_ParseTreeNode.AstNode;
                
                else if (_ParseTreeNode.AstNode is IDNode)
                    ATypeNode = new ATypeNode(((IDNode)_ParseTreeNode.AstNode).LastAttribute.GetRelatedType(dbContext.DBTypeManager));
                
                else 
                    continue; // we found just a reference

                if (!_TypeList.Contains(ATypeNode))
                {
                    _TypeList.Add(ATypeNode);
                }
                else
                {
                    throw new GraphDBException(new Error_DuplicateReferenceOccurence(ATypeNode.DBTypeStream));
                }
            }

            #endregion

            if (myParseTreeNode.ChildNodes[3].HasChildNodes())
            {   
                IDNode tempIDNode;
                foreach (var _ParseTreeNode in myParseTreeNode.ChildNodes[3].ChildNodes[0].ChildNodes)
                {
                    if (_ParseTreeNode.AstNode is IDNode)
                    {
                        tempIDNode = (IDNode)_ParseTreeNode.AstNode;

                        if ((tempIDNode.Level > 0) && (tempIDNode.Depth > 1))
                        {
                            throw new GraphDBException(new Error_RemoveTypeAttribute(tempIDNode.LastType, tempIDNode.LastAttribute));
                        }

                        if (tempIDNode.IsValidated)
                        {

                            if (!_DBTypeAttributeToDelete.ContainsKey(tempIDNode.Reference.Item1))
                                _DBTypeAttributeToDelete.Add(tempIDNode.Reference.Item1, new List<TypeAttribute>());

                            if (tempIDNode.LastAttribute != null && !tempIDNode.IsAsteriskSet)
                                _DBTypeAttributeToDelete[tempIDNode.Reference.Item1].Add(tempIDNode.LastAttribute);

                            if (!_ReferenceTypeLookup.ContainsKey(tempIDNode.Reference.Item1))
                                _ReferenceTypeLookup.Add(tempIDNode.Reference.Item1, tempIDNode.Reference.Item2);
                        }
                        else
                        {
                            #region undefined Attributes

                            if (tempIDNode.Reference != null)
                            {
                                var firstUndefinedNode = tempIDNode.GetInvalidIDNodeParts().First();

                                #region GetType

                                var type = _TypeList.Where(item => item.Reference == tempIDNode.Reference.Item1).FirstOrDefault().DBTypeStream;

                                #endregion

                                if (!(firstUndefinedNode.AstNode is EdgeTraversalNode))
                                {
                                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                                }

                                undefAttrName = ((EdgeTraversalNode)firstUndefinedNode.AstNode).AttributeName;

                                if (!_TypeWithUndefAttrs.ContainsKey(type))
                                    _TypeWithUndefAttrs.Add(type, new List<String>() { undefAttrName });
                                else
                                    _TypeWithUndefAttrs[type].Add(undefAttrName);

                            }
                            else
                            {
                                undefAttrName = _ParseTreeNode.ChildNodes[0].Token.ValueString;

                                if (_TypeWithUndefAttrs.Count == 0)
                                {
                                    foreach (var types in _TypeList)
                                        _TypeWithUndefAttrs.Add(types.DBTypeStream, new List<String>() { undefAttrName });
                                }
                                else
                                {
                                    foreach (var types in _TypeWithUndefAttrs)
                                        types.Value.Add(undefAttrName);
                                }
                            }

                            #endregion
                        }
                    }                   
                }                
            }
            else
            {
                foreach (ATypeNode _ATypeNode in _TypeList)
                {   
                    _DBTypeAttributeToDelete.Add(_ATypeNode.Reference, new List<TypeAttribute>());

                    if (!_ReferenceTypeLookup.ContainsKey(_ATypeNode.Reference))
                        _ReferenceTypeLookup.Add(_ATypeNode.Reference, _ATypeNode.DBTypeStream);

                }
            }

            _ObjectManipulationManager = new ObjectManipulationManager(dbContext.SessionSettings, null, dbContext, this);

            #region whereClauseOpt

            if (myParseTreeNode.ChildNodes[4].HasChildNodes())
            {
                WhereExpressionNode tempWhereNode = (WhereExpressionNode)myParseTreeNode.ChildNodes[4].AstNode;
                _WhereExpression = tempWhereNode.BinExprNode;

                Exceptional validateResult = _WhereExpression.Validate(dbContext, _TypeList.Select(tnode => tnode.DBTypeStream).ToArray());
                if (!validateResult.Success)
                {
                    throw new GraphDBException(validateResult.Errors);
                }

            }

            #endregion
        
        }

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="dbContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession graphDBSession, DBContext dbContext)
        {

            using (var transaction = graphDBSession.BeginTransaction())
            {

                var dbInnerContext = transaction.GetDBContext();

                var result = _ObjectManipulationManager.Delete(_WhereExpression, dbContext, _TypeWithUndefAttrs, _DBTypeAttributeToDelete, _ReferenceTypeLookup);

                #region Commit transaction and add all Warnings and Errors

                result.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return result;

            }

        }

    }
}
