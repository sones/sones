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
using System.Linq;
using System.Collections.Generic;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Managers;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Drop
{
    public class DeleteNode : AStatement
    {

        #region Data

        private BinaryExpressionDefinition _WhereExpression;
        
        private List<IDChainDefinition> _IDChainDefinitions;

        private List<TypeReferenceDefinition> _TypeReferenceDefinitions;
        

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

            _IDChainDefinitions = new List<IDChainDefinition>();

            _TypeReferenceDefinitions = (myParseTreeNode.ChildNodes[1].AstNode as TypeListNode).Types;

            if (myParseTreeNode.ChildNodes[3].HasChildNodes())
            {
                IDNode tempIDNode;
                foreach (var _ParseTreeNode in myParseTreeNode.ChildNodes[3].ChildNodes[0].ChildNodes)
                {
                    if (_ParseTreeNode.AstNode is IDNode)
                    {
                        tempIDNode = (IDNode)_ParseTreeNode.AstNode;
                        _IDChainDefinitions.Add(tempIDNode.IDChainDefinition);
                    }
                }
            }
            else
            {
                foreach (var type in _TypeReferenceDefinitions)
                {
                    var def = new IDChainDefinition();
                    def.AddPart(new ChainPartTypeOrAttributeDefinition(type.Reference));
                    _IDChainDefinitions.Add(def);
                }
            }

            var dbContext = myCompilerContext.IContext as DBContext;

            #region whereClauseOpt

            if (myParseTreeNode.ChildNodes[4].HasChildNodes())
            {
                WhereExpressionNode tempWhereNode = (WhereExpressionNode)myParseTreeNode.ChildNodes[4].AstNode;
                _WhereExpression = tempWhereNode.BinExprNode.BinaryExpressionDefinition;

                //Exceptional validateResult = _WhereExpression.Validate(dbContext, _TypeReferenceDefinitions.Select(type => dbContext.DBTypeManager.GetTypeByName(type.TypeName)).ToArray());
                Exceptional validateResult = _WhereExpression.Validate(dbContext);
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

                var _TypeWithUndefAttrs = new Dictionary<GraphDBType, List<string>>();
                var _DBTypeAttributeToDelete = new Dictionary<GraphDBType, List<TypeAttribute>>();
                
                var _ReferenceTypeLookup = GetTypeReferenceLookup(dbContext, _TypeReferenceDefinitions);
                if (_ReferenceTypeLookup.Failed)
                {
                    return new QueryResult(_ReferenceTypeLookup);
                }

                foreach (var id in _IDChainDefinitions)
                {
                    
                    id.Validate(dbContext, _ReferenceTypeLookup.Value, true);
                    if (id.ValidateResult.Failed)
                    {
                        return new QueryResult(id.ValidateResult);
                    }

                    if ((id.Level > 0) && (id.Depth > 1))
                    {
                        throw new GraphDBException(new Error_RemoveTypeAttribute(id.LastType, id.LastAttribute));
                    }

                    if (id.IsUndefinedAttribute)
                    {

                        if (!_TypeWithUndefAttrs.ContainsKey(id.LastType))
                        {
                            _TypeWithUndefAttrs.Add(id.LastType, new List<String>());
                        }
                        _TypeWithUndefAttrs[id.LastType].Add(id.UndefinedAttribute);

                    }
                    else
                    {
                        if (!_DBTypeAttributeToDelete.ContainsKey(id.LastType))
                        {
                            _DBTypeAttributeToDelete.Add(id.LastType, new List<TypeAttribute>());
                        }
                        if (id.LastAttribute != null) // in case we want to delete the complete DBO we have no attribute definition
                        {
                            _DBTypeAttributeToDelete[id.LastType].Add(id.LastAttribute);
                        }
                    }

                }


                var _ObjectManipulationManager = new ObjectManipulationManager();


                var result = _ObjectManipulationManager.Delete(_WhereExpression, dbContext, _TypeWithUndefAttrs, _DBTypeAttributeToDelete, _ReferenceTypeLookup.Value);

                #region Commit transaction and add all Warnings and Errors

                result.AddErrorsAndWarnings(transaction.Commit());

                #endregion

                return result;

            }

        }

    }
}
