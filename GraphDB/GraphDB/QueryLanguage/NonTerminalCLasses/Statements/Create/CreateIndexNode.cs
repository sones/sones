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


/* <id name="sones GraphDB – create type astnode" />
 * <copyright file="CreateIndexNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Achim Friedland</developer>
 * <summary>This node is requested in case of an create type statement.</summary>
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using sones.GraphDB.Exceptions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.QueryLanguage.Operators;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Scripting.Ast;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.ObjectManagement;

using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphFS.Objects;
using sones.GraphDB.Errors;
using sones.GraphDB.Structures;
using sones.GraphFS.Session;
using sones.Lib.Session;
using sones.Lib.DataStructures.Indices;
using sones.GraphDB.Indices;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements
{

    /// <summary>
    /// This node is requested in case of an create type statement.
    /// </summary>
    class CreateIndexNode : AStatement
    {

        #region Data

        String _IndexName = null;
        String _IndexEdition = null;
        //String _IndexType = null;
        GraphDBType _DBObjectType = null;
        CreateIndexAttributeListNode _AttributeList = null;

        List<AttributeUUID> _IndexAttributes = null;
        String _IndexType;


        #endregion

        #region Properties - Statement information

        public override String StatementName { get { return "CreateIndex"; } }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        #endregion

        #region Constructors

        public CreateIndexNode()
        {
            
        }

        #endregion

        #region public AStatement methods

        /// <summary>
        /// Executes the statement
        /// </summary>
        /// <param name="graphDBSession">The DBSession to start new transactions</param>
        /// <param name="transactionContext">The current dbContext inside an readonly transaction. For any changes, you need to start a new transaction using <paramref name="graphDBSession"/></param>
        /// <returns>The result of the query</returns>
        public override QueryResult Execute(IGraphDBSession graphDBSession, DBContext dbContext)
        {
            #region checking for reference attributes

            TypeAttribute aIdxAttribute;
            foreach (var aAttributeUUID in _IndexAttributes)
            {
                aIdxAttribute = _DBObjectType.GetTypeAttributeByUUID(aAttributeUUID);

                if (aIdxAttribute.GetDBType(dbContext.DBTypeManager).IsUserDefined)
                {
                    return new QueryResult(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), String.Format("Currently it is not implemented to create an index on reference attributes like {0}", aIdxAttribute.Name) ));
                }
            }

            #endregion

            using (var transaction = graphDBSession.BeginTransaction())
            {
                var transactionContext = transaction.GetDBContext();
                SelectionResultSet resultOutput = null;

                foreach (var item in transactionContext.DBTypeManager.GetAllSubtypes(_DBObjectType))
                {
                    var createdIDx = item.CreateAttributeIndex(_IndexName, _IndexAttributes, _IndexEdition, _IndexType);
                    if (!createdIDx.Success)
                    {
                        return new QueryResult(createdIDx);
                    }
                    else
                    {

                        #region prepare readouts

                        var readOut = GenerateCreateIndexResult(createdIDx.Value);

                        resultOutput = new SelectionResultSet(null, new List<DBObjectReadout> { readOut });

                        #endregion

                    }

                    var rebuildResult = transactionContext.DBIndexManager.RebuildIndex(createdIDx.Value.IndexName, createdIDx.Value.IndexEdition, item, IndexSetStrategy.MERGE);
                    if (!rebuildResult.Success)
                    {
                        return new QueryResult(rebuildResult);
                    }

                    var flushResult = transactionContext.DBTypeManager.FlushType(item);
                    if (flushResult.Failed)
                    {
                        return new QueryResult(flushResult);
                    }

                }

                #region Commit transaction and add all Warnings and Errors

                var result = new QueryResult(transaction.Commit());

                #endregion

                if (result.ResultType == ResultType.Successful)
                {
                    result.AddResult(resultOutput);
                }
                
                return result;
            }

        }

        private DBObjectReadout GenerateCreateIndexResult(AttributeIndex attributeIndex)
        {
            Dictionary<String, Object> payload = new Dictionary<string, object>();

            payload.Add("NAME", attributeIndex.IndexName);
            payload.Add("EDITION", attributeIndex.IndexEdition);
            payload.Add("INDEXTYPE", attributeIndex.IndexType);
            payload.Add("ONTYPE", _DBObjectType.Name);

            List<DBObjectReadout> attributes = new List<DBObjectReadout>();

            foreach (var aAttribute in _AttributeList.IndexAttributes)
            {
                Dictionary<String, Object> payloadAttributes = new Dictionary<string, object>();

                payloadAttributes.Add("ATTRIBUTE", aAttribute.IndexAttribute);

                attributes.Add(new DBObjectReadout(payloadAttributes));
            }

            payload.Add("ATTRIBUTES", new Edge(attributes));


            //payload.Add("NAME", attributeIndex.IndexName)

            return new DBObjectReadout(payload);
        }

        /// <summary>
        /// Gets the content of an UpdateStatement.
        /// </summary>
        /// <param name="context">CompilerContext of Irony.</param>
        /// <param name="parseNode">The current ParseNode.</param>
        /// <param name="typeManager">The TypeManager of the PandoraDB.</param>
        public override void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;

            #region Get IndexName

            _IndexName = (parseNode.ChildNodes[2].AstNode as IndexNameOptNode).IndexName;

            #endregion

            #region Get IndexEdition

            _IndexEdition = (parseNode.ChildNodes[3].AstNode as EditionOptNode).IndexEdition;

            #endregion

            #region Get DBObjectType
            
            _DBObjectType = ((ATypeNode)parseNode.ChildNodes[5].AstNode).DBTypeStream;
            
            #endregion

            #region Get AttributeList and DirOptions

            if (parseNode.ChildNodes[6].HasChildNodes())
            {

                _AttributeList = (CreateIndexAttributeListNode) parseNode.ChildNodes[6].AstNode;
                _IndexAttributes = new List<AttributeUUID>();

                foreach (CreateIndexAttributeNode _CreateIndexAttributeNode in _AttributeList.IndexAttributes)
                {
                    var validAttrExcept = typeManager.AreValidAttributes(_DBObjectType, _CreateIndexAttributeNode.IndexAttribute);

                    if(validAttrExcept.Failed)
                        throw new GraphDBException(validAttrExcept.Errors);
                    
                    if (!validAttrExcept.Value)
                        throw new GraphDBException(new Error_AttributeDoesNotExists(_DBObjectType.Name, _CreateIndexAttributeNode.IndexAttribute));

                    _IndexAttributes.Add(_DBObjectType.GetTypeAttributeByName(_CreateIndexAttributeNode.IndexAttribute).UUID);

                }

            }

            #endregion

            #region Get IndexType

            _IndexType = (parseNode.ChildNodes[7].AstNode as IndexTypeOptNode).IndexType;

            #endregion

        }

        #endregion

    
    }

}
