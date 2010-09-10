/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* <id name="PandoraDB – CreateTypes astnode" />
 * <copyright file="CreateTypesNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of an Create Types statement.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Structures;
using sones.GraphFS.Session;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Warnings;
using sones.Lib;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.Session;
using sones.GraphDB.QueryLanguage.Enums;
using sones.Lib.ErrorHandling;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements
{
    /// <summary>
    /// This node is requested in case of an Create Types statement.
    /// </summary>
    public class CreateTypesNode : AStatement
    {

        #region Data

        //NLOG: temporarily commented
        //private static Logger //_Logger = LogManager.GetCurrentClassLogger();
        private List<GraphDBTypeDefinition> _ListOfTypes = null;

        #endregion

        #region Properties - Statement information

        public override String StatementName { get { return "CreateTypes"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        #region constructor

        public CreateTypesNode()
        {
            
        }

        #endregion

        #region public AStatement methods

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

                var result = transaction.GetDBContext().DBTypeManager.AddBulkTypes(_ListOfTypes);

                if (!result.Success)
                {

                    #region Rollback transaction and add all Warnings and Errors

                    result.AddErrorsAndWarnings(transaction.Rollback());

                    #endregion

                    return new QueryResult(result.Errors);

                }
                else
                {

                    #region Commit transaction and add all Warnings and Errors

                    result.Value.AddErrorsAndWarnings(transaction.Commit());

                    #endregion

                    return result.Value;

                }

            }

        }

        #region private helper methods

        /// <summary>
        /// check for the extends type in create types statement
        /// </summary>
        /// <param name="myChildNode">the interessting extends node</param>
        /// <param name="dbContext">the typemanager</param>
        /// <param name="parseTreeNodeList">the list of types in the statement</param>
        private void CheckExtends(BulkTypeListMemberNode myChildNode, DBContext dbContext, ParseTreeNodeList parseTreeNodeList)
        {
            #region check extend
            if (!String.IsNullOrEmpty(myChildNode.Extends))
            {
                if (myChildNode.Extends != DBConstants.DBObject)
                {
                    GraphDBType extendType = dbContext.DBTypeManager.GetTypeByName(myChildNode.Extends);

                    if (extendType == null)
                    {

                        var typeFound = from type in parseTreeNodeList where ((BulkTypeListMemberNode)type.AstNode).TypeName == myChildNode.Extends select type.AstNode;
                        
                        if(typeFound.IsNullOrEmpty())
                            throw new GraphDBException(new Error_TypeDoesNotExist(myChildNode.Extends));
                    }
                    else
                    {
                        if (!extendType.IsUserDefined)
                            throw new GraphDBException(new Error_InvalidBaseType(myChildNode.Extends));
                    }
                }
            }
            #endregion
        }


        /// <summary>
        /// check for the extends type in create type statement
        /// </summary>
        /// <param name="myChildNode">the interessting extends node</param>
        /// <param name="dbContext">the typemanager</param>
        private void CheckExtends(BulkTypeNode myChildNode, DBContext dbContext)
        {
            #region check extend
            if (!String.IsNullOrEmpty(myChildNode.Extends))
            {
                if (myChildNode.Extends != DBConstants.DBObject)
                {
                    GraphDBType extendType = dbContext.DBTypeManager.GetTypeByName(myChildNode.Extends);

                    if (extendType == null)
                        throw new GraphDBException(new Error_TypeDoesNotExist(myChildNode.Extends));

                    if (!extendType.IsUserDefined)
                        throw new GraphDBException(new Error_InvalidBaseType(myChildNode.Extends));
                }
            }
            #endregion
        }

        /// <summary>
        /// check for set or list
        /// </summary>
        /// <param name="aChildNode">the interessting ParseNode</param>
        /// <returns></returns>
        private Boolean CheckForSet(ParseTreeNode aChildNode)
        {
            if (aChildNode.HasChildNodes())
            {
                if (aChildNode.ChildNodes[0] != null)
                {
                    if (aChildNode.ChildNodes[0].HasChildNodes())
                    {
                        if (aChildNode.ChildNodes[0].ChildNodes[0] != null)
                        {
                            if (aChildNode.ChildNodes[0].ChildNodes[0].HasChildNodes())
                            {
                                if (aChildNode.ChildNodes[0].ChildNodes[0].ChildNodes[0].Token.ValueString.ToUpper() == DBConstants.SET)
                                    return true;
                            }
                            else if (aChildNode.ChildNodes[0].ChildNodes[0].Token.ValueString.ToUpper() == DBConstants.SET)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// check for set constrain attributes in the create type statement
        /// </summary>
        /// <param name="aAttrDefNode">the interessting attribute</param>
        /// <param name="dbContext">the typemanager</param>
        /// <param name="myTypeName">current type name</param>
        private void CheckForSetConstraint(ParseTreeNode aAttrDefNode, string myTypeName, DBContext dbContext)
        {
            GraphDBType dbType = null;
            AttributeDefinitionNode attrDef = null;

            foreach (var item in aAttrDefNode.ChildNodes)
            {
                foreach(var attr in item.ChildNodes)
                {
                    attrDef = (AttributeDefinitionNode)attr.AstNode;

                    if (attrDef.Type != myTypeName)
                    {
                        dbType = dbContext.DBTypeManager.GetTypeByName(attrDef.Type);

                        if (dbType == null)
                            throw new GraphDBException(new Error_TypeDoesNotExist(attrDef.Type));

                        if (attrDef.TypeAttribute.KindOfType == KindsOfType.SetOfReferences)
                        {
                            if (!CheckForSet(attr))
                                throw new GraphDBException(new Error_ListAttributeNotAllowed(myTypeName));
                        }

                    }
                    else if (attrDef.Type == myTypeName && attrDef.TypeAttribute.KindOfType == KindsOfType.SetOfReferences)
                    {
                        if (!CheckForSet(attr))
                            throw new GraphDBException(new Error_ListAttributeNotAllowed(myTypeName));
                    }
                }
            }
        }

        /// <summary>
        /// check for set constrain attributes in the create types statement
        /// </summary>
        /// <param name="aAttrDefNode">the interessting attribute</param>
        /// <param name="myTypeName">current type name</param>
        /// <param name="dbContext">the typemanager</param>
        /// <param name="typeList">type node list</param>
        private void CheckForSetConstraint(ParseTreeNode aAttrDefNode, string myTypeName, DBContext dbContext, ParseTreeNodeList typeList)
        {
            GraphDBType dbType = null;
            AttributeDefinitionNode attrDef = null;
            Boolean isUserDefined = false;
                        
            foreach (var item in aAttrDefNode.ChildNodes)
            {
                foreach (var attr in item.ChildNodes)
                {
                    attrDef = (AttributeDefinitionNode)attr.AstNode;
                    isUserDefined = false;

                    if (attrDef.Type != myTypeName)
                    {
                        dbType = dbContext.DBTypeManager.GetTypeByName(attrDef.Type);

                        if (dbType == null)
                        {
                            var foundType = from type in typeList where ((BulkTypeListMemberNode)type.AstNode).TypeName == attrDef.Type select type.AstNode;

                            if (foundType.IsNullOrEmpty())
                                throw new GraphDBException(new Error_TypeDoesNotExist(attrDef.Type));

                            isUserDefined = true;
                        }
                        else
                            isUserDefined = dbType.IsUserDefined;

                        if (attrDef.TypeAttribute.KindOfType == KindsOfType.SetOfReferences && isUserDefined)
                        {
                            if (!CheckForSet(attr))
                                throw new GraphDBException(new Error_ListAttributeNotAllowed(myTypeName));
                        }

                    }
                    else if (attrDef.Type == myTypeName && attrDef.TypeAttribute.KindOfType == KindsOfType.SetOfReferences)
                    {
                        if (!CheckForSet(attr))
                            throw new GraphDBException(new Error_ListAttributeNotAllowed(myTypeName));
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Gets the content of a CreateTypeStatement.
        /// </summary>
        /// <param name="myCompilerContext">CompilerContext of Irony.</param>
        /// <param name="myParseTreeNode">The current ParseNode.</param>
        /// <param name="typeManager">The TypeManager of the PandoraDB.</param>
        public override void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {

            var _DBContext   = myCompilerContext.IContext as DBContext;
            var _TypeManager = _DBContext.DBTypeManager;

            #region Data

            _ListOfTypes = new List<GraphDBTypeDefinition>();

            #endregion

            try
            {

                if (myParseTreeNode.ChildNodes.Count > 3)
                {

                    BulkTypeNode aTempNode = (BulkTypeNode) myParseTreeNode.ChildNodes[3].AstNode;
                    CheckExtends(aTempNode, _DBContext);
                    CheckForSetConstraint(myParseTreeNode.ChildNodes[3].ChildNodes[2], aTempNode.TypeName, _DBContext);

                    Boolean isAbstract = false;

                    if (myParseTreeNode.ChildNodes[1].HasChildNodes())
                        isAbstract = true;

                    _ListOfTypes.Add(new GraphDBTypeDefinition(aTempNode.TypeName, aTempNode.Extends, isAbstract, aTempNode.Attributes, aTempNode.BackwardEdges, aTempNode.Indices, aTempNode.Comment));
                    
                }

                else
                {
                    foreach (var _ParseTreeNode in myParseTreeNode.ChildNodes[2].ChildNodes)
                    {
                        if (_ParseTreeNode.AstNode != null)
                        {
                            BulkTypeListMemberNode aTempNode = (BulkTypeListMemberNode)_ParseTreeNode.AstNode;
                            CheckExtends(aTempNode, _DBContext, myParseTreeNode.ChildNodes[2].ChildNodes);
                            CheckForSetConstraint(_ParseTreeNode.ChildNodes[1].ChildNodes[2], aTempNode.TypeName, _DBContext, myParseTreeNode.ChildNodes[2].ChildNodes);
                            _ListOfTypes.Add(new GraphDBTypeDefinition(aTempNode.TypeName, aTempNode.Extends, aTempNode.IsAbstract, aTempNode.Attributes, aTempNode.BackwardEdges, aTempNode.Indices, aTempNode.Comment));
                        }
                    }
                }

            }

            catch (GraphDBException e)
            {
                throw new GraphDBException(e.GraphDBErrors);
            }

        }        

        #endregion
    
    }

}
