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
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;
using System.Collections.Generic;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.Structure.Nodes.DDL;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.StatementNodes.DDL
{
    /// <summary>
    /// This node is requested in case of an Create Types statement.
    /// </summary>
    public sealed class CreateEdgeTypesNode : AStatement, IAstNodeInit
    {
        #region Data

        private List<ATypePredefinition> _TypePredefinitions 
                                            = new List<ATypePredefinition>();
        private String _query;

        #endregion

        #region constructor

        public CreateEdgeTypesNode()
        { }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, 
                            ParseTreeNode myParseTreeNode)
        {
            //createTypesStmt.Rule = S_CREATE + S_ABSTRACT + S_EDGE + S_TYPE + bulkEdgeType
            //                        | S_CREATE + S_EDGE + S_TYPE + bulkEdgeType
            //                        | S_CREATE + S_EDGE + S_TYPES + bulkEdgeTypeList;

            if (myParseTreeNode.ChildNodes[1].Token.KeyTerm == 
                ((SonesGQLGrammar)context.Language.Grammar).S_ABSTRACT)
            {
                #region Abstract & Single EdgeType

                BulkEdgeTypeNode aTempNode = (BulkEdgeTypeNode)myParseTreeNode.ChildNodes[4].AstNode;

                Boolean isAbstract = true;

                var predef = 
                    new EdgeTypePredefinition(aTempNode.TypeName)
                        .SetSuperTypeName(aTempNode.Extends)
                        .SetComment(aTempNode.Comment);
       
                if(isAbstract)
                    predef.MarkAsAbstract();

                foreach(var attr in aTempNode.Attributes)
                    predef.AddProperty(GenerateProperty(attr));

                _TypePredefinitions.Add(predef);
                
                #endregion
            }
            else
            {
                if (myParseTreeNode.ChildNodes[2].Token.KeyTerm == 
                    ((SonesGQLGrammar)context.Language.Grammar).S_TYPES)
                {
                    #region multiple VertexTypes

                    foreach (var _ParseTreeNode in myParseTreeNode.ChildNodes[3].ChildNodes)
                    {
                        if (_ParseTreeNode.AstNode != null)
                        {
                            BulkEdgeTypeListMemberNode aTempNode = (BulkEdgeTypeListMemberNode)_ParseTreeNode.AstNode;

                            var predef = new EdgeTypePredefinition(aTempNode.TypeName)
                                                .SetSuperTypeName(aTempNode.Extends)
                                                .SetComment(aTempNode.Comment);

                            if (aTempNode.IsAbstract)
                                predef.MarkAsAbstract();

                            foreach (var attr in aTempNode.Attributes)
                                predef.AddProperty(GenerateProperty(attr));

                            _TypePredefinitions.Add(predef);
                        }
                    }

                    #endregion
                }
                else
                {
                    #region single vertex type

                    BulkEdgeTypeNode aTempNode = (BulkEdgeTypeNode)myParseTreeNode.ChildNodes[3].AstNode;

                    var predef =
                        new EdgeTypePredefinition(aTempNode.TypeName)
                            .SetSuperTypeName(aTempNode.Extends)
                            .SetComment(aTempNode.Comment);

                    foreach (var attr in aTempNode.Attributes)
                        predef.AddProperty(GenerateProperty(attr));

                    _TypePredefinitions.Add(predef);

                    #endregion
                }
            }

        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "CreateEdgeTypes"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, 
                                            IGraphQL myGraphQL, 
                                            GQLPluginManager myPluginManager, 
                                            String myQuery, 
                                            SecurityToken mySecurityToken, 
                                            Int64 myTransactionToken)
        {
            _query = myQuery;

            QueryResult result;

            try
            {
                result = myGraphDB.CreateEdgeTypes<QueryResult>(
                        mySecurityToken,
                        myTransactionToken,
                        new RequestCreateEdgeTypes(_TypePredefinitions),
                        CreateQueryResult);
            }
            catch (ASonesException e)
            {
                result = new QueryResult(_query, 
                                            SonesGQLConstants.GQL, 
                                            0, 
                                            ResultType.Failed, 
                                            null, 
                                            e);
            }

            return result;
        }

        #endregion

        #region private helper

        /// <summary>
        /// Generates a query result using some statistics and the created vertices
        /// </summary>
        /// <param name="myStats">The statistics of the request</param>
        /// <param name="myCreatedVertexTypes">The created vertex types</param>
        /// <returns>A QueryResult</returns>
        private QueryResult CreateQueryResult(IRequestStatistics myStats, 
                                                IEnumerable<IEdgeType> myCreatedEdgeTypes)
        {
            return new QueryResult(_query, 
                                    SonesGQLConstants.GQL, 
                                    Convert.ToUInt64(myStats.ExecutionTime.Milliseconds), 
                                    ResultType.Successful, 
                                    CreateVertexViews(myCreatedEdgeTypes));
        }

        /// <summary>
        /// Creates vertex views corresponding to the created vertex types
        /// </summary>
        /// <param name="myCreatedVertexTypes">The vertex types that have been created</param>
        /// <returns>An enumerable of vertex views</returns>
        private IEnumerable<IVertexView> CreateVertexViews(IEnumerable<IEdgeType> myCreatedVertexTypes)
        {
            List<IVertexView> result = new List<IVertexView>();

            foreach (var aCreatedEdge in myCreatedVertexTypes)
            {
                result.Add(GenerateAVertexView(aCreatedEdge));
            }

            return result;
        }

        /// <summary>
        /// Generates a single vertex view corresponding to a created vertex type
        /// </summary>
        /// <param name="aCreatedVertes">The vertex type that has been created</param>
        /// <returns>The resulting vertex view</returns>
        private IVertexView GenerateAVertexView(IEdgeType aCreatedEdge)
        {
            return new VertexView(new Dictionary<string,object>
                                        {
                                            {"EdgeType", aCreatedEdge.Name},
                                            {"EdgeTypeID", aCreatedEdge.ID}
                                        }, null);
        }
        
        /// <summary>
        /// Generates a attribute definition
        /// </summary>
        /// <param name="aAttribute">The attribute that is going to be transfered</param>
        /// <returns>A attribute predefinition</returns>
        private PropertyPredefinition GenerateProperty(KeyValuePair<AttributeDefinition, string> aAttribute)
        {
            PropertyPredefinition result = new PropertyPredefinition(aAttribute.Key.AttributeName, 
                                                                        aAttribute.Value);
            
            switch (aAttribute.Key.AttributeType.Type)
            {
                case SonesGQLGrammar.TERMINAL_SET:
                    result.SetMultiplicityToSet();
                    break;

                case SonesGQLGrammar.TERMINAL_LIST:
                    result.SetMultiplicityToList();
                    break;
            }

            if (aAttribute.Key.DefaultValue != null)
                result.SetDefaultValue(aAttribute.Key.DefaultValue.ToString());

            if (aAttribute.Key.AttributeType.TypeCharacteristics.IsMandatory)
                result.SetAsMandatory();

            return result;
        }

        #endregion

    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
