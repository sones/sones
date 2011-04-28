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
    public sealed class CreateTypesNode : AStatement, IAstNodeInit
    {
        #region Data

        private List<GraphDBTypeDefinition> _TypeDefinitions = new List<GraphDBTypeDefinition>();
        private String _query;

        #endregion

        #region constructor

        public CreateTypesNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode myParseTreeNode)
        {
            //createTypesStmt.Rule =      S_CREATE + S_VERTEX + S_TYPES + bulkTypeList
            //                        |   S_CREATE + S_ABSTRACT + S_VERTEX + S_TYPE + bulkType
            //                        |   S_CREATE + S_VERTEX + S_TYPE + bulkType;

            if (myParseTreeNode.ChildNodes[1].Token.KeyTerm == ((SonesGQLGrammar)context.Language.Grammar).S_ABSTRACT)
            {
                #region Abstract & Single VertexType

                BulkTypeNode aTempNode = (BulkTypeNode)myParseTreeNode.ChildNodes[4].AstNode;

                Boolean isAbstract = true;

                if (HasChildNodes(myParseTreeNode.ChildNodes[1]))
                {
                    isAbstract = true;
                }

                _TypeDefinitions.Add(new GraphDBTypeDefinition(aTempNode.TypeName, aTempNode.Extends, isAbstract, aTempNode.Attributes, aTempNode.BackwardEdges, aTempNode.Indices, aTempNode.Comment));

                #endregion
            }
            else
            {
                if (myParseTreeNode.ChildNodes[2].Token.KeyTerm == ((SonesGQLGrammar)context.Language.Grammar).S_TYPES)
                {
                    #region multiple VertexTypes

                    foreach (var _ParseTreeNode in myParseTreeNode.ChildNodes[3].ChildNodes)
                    {
                        if (_ParseTreeNode.AstNode != null)
                        {
                            BulkTypeListMemberNode aTempNode = (BulkTypeListMemberNode)_ParseTreeNode.AstNode;
                            _TypeDefinitions.Add(new GraphDBTypeDefinition(aTempNode.TypeName, aTempNode.Extends, aTempNode.IsAbstract, aTempNode.Attributes, aTempNode.BackwardEdges, aTempNode.Indices, aTempNode.Comment));
                        }
                    }

                    #endregion
                }
                else
                {
                    #region single vertex type

                    BulkTypeNode aTempNode = (BulkTypeNode)myParseTreeNode.ChildNodes[3].AstNode;

                    _TypeDefinitions.Add(new GraphDBTypeDefinition(aTempNode.TypeName, aTempNode.Extends, false, aTempNode.Attributes, aTempNode.BackwardEdges, aTempNode.Indices, aTempNode.Comment));

                    #endregion
                }
            }
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "CreateTypes"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            _query = myQuery;

            QueryResult result;

            try
            {
                result = myGraphDB.CreateVertexType<QueryResult>(
                        mySecurityToken,
                        myTransactionToken,
                        new RequestCreateVertexTypes(GenerateVertexTypePredefinitions()),
                        CreateQueryResult);
            }
            catch (ASonesException e)
            {
                result = new QueryResult(_query, SonesGQLConstants.GQL, 0, ResultType.Failed, null, e);
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
        private QueryResult CreateQueryResult(IRequestStatistics myStats, IEnumerable<IVertexType> myCreatedVertexTypes)
        {
            return new QueryResult(_query, SonesGQLConstants.GQL, Convert.ToUInt64(myStats.ExecutionTime.Milliseconds), ResultType.Successful, CreateVertexViews(myCreatedVertexTypes));
        }

        /// <summary>
        /// Creates vertex views corresponding to the created vertex types
        /// </summary>
        /// <param name="myCreatedVertexTypes">The vertex types that have been created</param>
        /// <returns>An enumerable of vertex views</returns>
        private IEnumerable<IVertexView> CreateVertexViews(IEnumerable<IVertexType> myCreatedVertexTypes)
        {
            List<IVertexView> result = new List<IVertexView>();

            foreach (var aCreatedVertes in myCreatedVertexTypes)
            {
                result.Add(GenerateAVertexView(aCreatedVertes));
            }

            return result;
        }

        /// <summary>
        /// Generates a single vertex view corresponding to a created vertex type
        /// </summary>
        /// <param name="aCreatedVertes">The vertex type that has been created</param>
        /// <returns>The resulting vertex view</returns>
        private IVertexView GenerateAVertexView(IVertexType aCreatedVertes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates the vertex type predefinitions that are necessary to create the vertex types
        /// </summary>
        /// <returns>An enumerable of those pre definitions</returns>
        private IEnumerable<VertexTypePredefinition> GenerateVertexTypePredefinitions()
        {
            List<VertexTypePredefinition> result = new List<VertexTypePredefinition>();

            foreach (var aDefinition in _TypeDefinitions)
            {
                result.Add(GenerateAVertexTypePredefinition(aDefinition));
            }

            return result;
        }

        /// <summary>
        /// Generates a single vertex type predefinition
        /// </summary>
        /// <param name="aDefinition">The definition that has been created by the gql</param>
        /// <returns>The corresponding vertex type predefinition</returns>
        private VertexTypePredefinition GenerateAVertexTypePredefinition(GraphDBTypeDefinition aDefinition)
        {
            var result = new VertexTypePredefinition(aDefinition.Name);

            #region extends

            if (aDefinition.ParentType != null && aDefinition.ParentType.Length > 0)
            {
                result.SetSuperVertexTypeName(aDefinition.ParentType);
            }

            #endregion

            #region abstract

            if (aDefinition.IsAbstract)
            {
                result.MarkAsAbstract();
            }

            #endregion

            #region comment

            if (aDefinition.Comment != null && aDefinition.Comment.Length > 0)
            {
                result.SetComment(aDefinition.Comment);
            }

            #endregion

            #region attributes

            if (aDefinition.Attributes != null)
            {
                foreach (var aAttribute in aDefinition.Attributes)
                {
                }
            }

            #endregion

            #region incoming edges

            if (aDefinition.BackwardEdgeNodes != null)
            {
                foreach (var aIncomingEdge in aDefinition.BackwardEdgeNodes)
                {

                }
            }

            #endregion

            #region indices

            if (aDefinition.Indices != null)
            {
                foreach (var aIndex in aDefinition.Indices)
                {

                }
            }

            #endregion

            return result;
        }

        #endregion

    }
}
