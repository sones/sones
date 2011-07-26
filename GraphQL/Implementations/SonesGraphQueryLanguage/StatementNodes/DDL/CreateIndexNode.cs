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
using Irony.Ast;
using Irony.Parsing;
using sones.GraphDB;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.Result;
using sones.GraphQL.Structure.Nodes.DDL;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.ErrorHandling;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;

namespace sones.GraphQL.StatementNodes.DDL
{
    public sealed class CreateIndexNode : AStatement, IAstNodeInit
    {
        #region Data

        String _IndexName = null;
        String _IndexEdition = null;
        String _DBType = null;
        List<IndexAttributeDefinition> _AttributeList = null;
        String _IndexType;
        Dictionary<String, String> _options = null;

        public String Query;

        #endregion

        #region Constructors

        public CreateIndexNode()
        { }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            var grammar = (SonesGQLGrammar)context.Language.Grammar;

            var childNum = 0;
            foreach (var child in parseNode.ChildNodes)
            {
                if (child.AstNode != null)
                {
                    if (child.AstNode is IndexNameOptNode)
                    {
                        _IndexName = (child.AstNode as IndexNameOptNode).IndexName;
                    }
                    else if (child.AstNode is EditionOptNode)
                    {
                        _IndexEdition = (child.AstNode as EditionOptNode).IndexEdition;
                    }
                    else if (child.AstNode is ATypeNode)
                    {
                        _DBType = (child.AstNode as ATypeNode).ReferenceAndType.Reference;

                        if (parseNode.ChildNodes[childNum - 1].Token.KeyTerm == grammar.S_ON)
                        {
                            //ParsingResult.PushIWarning(new Warnings.Warning_ObsoleteGQL("CREATE INDEX ... ON", "CREATE INDEX ... ON VERTEX"));
                        }
                    }
                    else if (child.AstNode is IndexAttributeListNode)
                    {
                        //ParsingResult.PushIExceptional((child.AstNode as IndexAttributeListNode).ParsingResult);

                        _AttributeList = (child.AstNode as IndexAttributeListNode).IndexAttributes;
                    }
                    else if (child.AstNode is IndexTypeOptNode)
                    {
                        _IndexType = (child.AstNode as IndexTypeOptNode).IndexType;
                    }
                    else if (child.AstNode is OptionsNode)
                    {
                        _options = (child.AstNode as OptionsNode).Options;
                    }

                }
                childNum++;
            }
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "CreateIndex"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            Query = myQuery;

            var indexDef = new IndexPredefinition(_IndexName);
            indexDef.SetIndexType(_IndexType);
            indexDef.SetVertexType(_DBType);
            indexDef.SetEdition(_IndexEdition);
            
            //to be indices attributes
            foreach (var aIndexedProperty in _AttributeList)
            {
                indexDef.AddProperty(aIndexedProperty.IndexAttribute.ContentString);
            }

            //options for the index
            if (_options != null)
            {
                foreach (var aKV in _options)
                {
                    indexDef.AddOption(aKV.Key, aKV.Value);
                }
            }

            return myGraphDB.CreateIndex<QueryResult>(mySecurityToken, myTransactionToken, new RequestCreateIndex(indexDef), GenerateResult);
        }

        #endregion

        private QueryResult GenerateResult(IRequestStatistics myStats, IIndexDefinition myIndexDefinition)
        {
            return new QueryResult(Query, 
                                    "sones.gql", 
                                    Convert.ToUInt64(myStats.ExecutionTime.TotalMilliseconds), 
                                    ResultType.Successful, 
                                    new List<IVertexView> { new VertexView(new Dictionary<String, object> { {"CreatedIndex", myIndexDefinition} } , 
                                                                            new Dictionary<String, IEdgeView>()) });
        }

    }

    
}
