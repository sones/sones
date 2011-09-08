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
using Irony.Parsing;
using sones.GraphDB;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.GQL.ErrorHandling;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.Result;
using sones.GraphQL.StatementNodes;
using sones.Library.Commons.Security;
using sones.Library.DataStructures;
using sones.Library.ErrorHandling;
using sones.Library.Settings;
using sones.Library.VersionedPluginManager;
using sones.Plugins.Index;
using sones.Plugins.Index.Versioned;
using sones.Plugins.SonesGQL.Aggregates;
using sones.Plugins.SonesGQL.DBExport;
using sones.Plugins.SonesGQL.DBImport;
using sones.Plugins.SonesGQL.Functions;
using sones.Plugins.SonesGQL.Statements;

namespace sones.GraphQL
{
    /// <summary>
    /// The sones query language
    /// </summary>
    public sealed class SonesQueryLanguage : IGraphQL
    {
        #region Data

        /// <summary>
        /// The IGraphDB instance for accessing the graph database
        /// </summary>
        private IGraphDB _IGraphDBInstance;

        /// <summary>
        /// The settings of the application
        /// </summary>
        private readonly GraphApplicationSettings _settings;

        /// <summary>
        /// A manager to dynamically load versioned plugins
        /// </summary>
        private readonly GQLPluginManager _GQLPluginManager;

        /// <summary>
        /// A SonesGQLGrammar instance to access to the grammar
        /// </summary>
        private readonly SonesGQLGrammar _GQLGrammar;

        /// <summary>
        /// The parser that results the grammar
        /// </summary>
        private readonly Parser _parser;

        #endregion

        #region Constructor

        /// <summary>
        /// The empty constructor, needed to load the query language as plugin.
        /// </summary>
        public SonesQueryLanguage()
        {
        }

        /// <summary>
        /// Creates a new sones GQL instance
        /// </summary>
        /// <param name="myIGraphDBInstace">The graph database instance on which the gql statements are executed</param>
        public SonesQueryLanguage(IGraphDB myIGraphDBInstace)
        {
            _IGraphDBInstance = myIGraphDBInstace;
            _settings = new GraphApplicationSettings(SonesGQLConstants.ApplicationSettingsLocation);

            #region plugin manager

            _GQLPluginManager = new GQLPluginManager();

            #endregion

            #region create gql grammar and set extendable members

            _GQLGrammar = new SonesGQLGrammar(myIGraphDBInstace);

            SetExtendableMember(_GQLGrammar);

            #endregion

            //build parser
            _parser = new Parser(_GQLGrammar);

            //check language
            if (_parser.Language.ErrorLevel != GrammarErrorLevel.Warning && _parser.Language.ErrorLevel != GrammarErrorLevel.NoError)
            {
                throw new IronyInitializeGrammarException(_parser.Language.Errors, "");
            }
        }

        #endregion

        #region IGraphQL Members

        public QueryResult Query(SecurityToken mySecurityToken, 
                                    Int64 myTransactionToken,
                                    string myQueryString)
        {
            //tree-like representation of the query-string
            ParseTree aTree;
            //executeable statement
            AStatement statement;   
            QueryResult queryResult = new QueryResult(myQueryString, 
                                                        SonesGQLConstants.GQL, 
                                                        0L, 
                                                        ResultType.Failed);

            #region Input exceptions - null or empty query

            if (myQueryString == null)
            {
                #region create error object
                
                queryResult.Error = new GqlSyntaxException("Error! Query was null!");

                return queryResult;

                #endregion
            }

            if (myQueryString.Length.Equals(0))
            {
                queryResult.Error = new GqlSyntaxException("Error! Query was empty!");

                return queryResult;
            }

            #endregion

            #region Parse query

            lock (_parser)
            {
                aTree = this._parser.Parse(myQueryString);
            }

            #endregion

            #region error handling

            if (aTree == null)
            {
                queryResult.Error = new GqlSyntaxException("Error! Query could not be parsed!");

                return queryResult;
            }

            if (aTree.HasErrors())
            {
                var error = aTree.ParserMessages.First();

                queryResult.Error = new GQLParsingException(error, myQueryString);

                return queryResult;
            }

            #endregion

            #region Execution

            //get the statement from the tree
            statement = (AStatement)aTree.Root.AstNode;

            try
            {
                queryResult = statement.Execute(_IGraphDBInstance, 
                                                this, 
                                                _GQLPluginManager,  
                                                myQueryString, 
                                                mySecurityToken, 
                                                myTransactionToken);
            }
            catch (ASonesException ee)
            {
                queryResult.Error = ee;
                return queryResult;
            }
            catch (Exception e)
            {
                queryResult.Error = new UnknownException(e);
                return queryResult;
            }

            #endregion

            return queryResult;
        }

        public IEnumerable<string> ExportGraphDDL(DumpFormats myDumpFormat, 
                                                    IEnumerable<IVertexType> myVertexTypesToDump,
                                                    IEnumerable<IEdgeType> myEdgeTypesToDump)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ExportGraphDML(DumpFormats myDumpFormat, 
                                                    IEnumerable<IVertexType> myVertexTypesToDump,
                                                    SecurityToken mySecurityToken, 
                                                    Int64 myTransactionToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IPluginable Members

        public string PluginName
        {
            get { return "sones.gql"; }
        }

        public string PluginShortName
        {
            get { return "gql"; }
        }

        public PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type> { { "GraphDB", typeof(IGraphDB) } }; }
        }

        public IPluginable InitializePlugin(String myUniqueString, 
                                            Dictionary<string, object> myParameters = null)
        {
            IGraphDB dbInstance = null;

            if (myParameters != null)
            {
                if (myParameters.ContainsKey("GraphDB"))
                {
                    dbInstance = (IGraphDB)myParameters["GraphDB"];
                }
            }

            var  result = new SonesQueryLanguage(dbInstance);

            return (IPluginable)result;
        }

        public void Dispose()
        { }

        #endregion

        #region private helper

        private void SetExtendableMember(SonesGQLGrammar myGQLGrammar)
        {
            #region aggregate

            List<IGQLAggregate> aggregates = new List<IGQLAggregate>();
            foreach (var plugin in _GQLPluginManager.GetPluginNameForType<IGQLAggregate>())
            {
                aggregates.Add(_GQLPluginManager.GetAndInitializePlugin<IGQLAggregate>(plugin));
            }

            if (aggregates.Count == 0)
            {
                throw new GQLGrammarSetExtandableMemberException(typeof(IGQLAggregate), 
                            "There is no plugin found to set in GQL grammar.");
            }
            myGQLGrammar.SetAggregates(aggregates);

            #endregion

            #region functions

            List<IGQLFunction> functions = new List<IGQLFunction>();
            foreach (var plugin in _GQLPluginManager.GetPluginNameForType<IGQLFunction>())
            {
                functions.Add(_GQLPluginManager.GetAndInitializePlugin<IGQLFunction>(plugin) as IGQLFunction);
            }

            if (functions.Count == 0)
            {
                throw new GQLGrammarSetExtandableMemberException(typeof(IGQLFunction), 
                            "There is no plugin found to set in GQL grammar.");
            }
            myGQLGrammar.SetFunctions(functions);

            #endregion

            #region indces

            List<String> indices = new List<string>();

            indices.AddRange(_GQLPluginManager.GetPluginNameForType<ISonesVersionedIndex>());
            indices.AddRange(_GQLPluginManager.GetPluginNameForType<ISonesIndex>());

            if (indices.Count < 1)
            {
                throw new GQLGrammarSetExtandableMemberException(typeof(ISonesIndex),
                            @"There is no valid index plugin found to set in GQL grammar.");
                           
            }

            myGQLGrammar.SetIndices(indices);

            #endregion

            #region import

            List<IGraphDBImport> importer = new List<IGraphDBImport>();
            foreach (var plugin in _GQLPluginManager.GetPluginNameForType<IGraphDBImport>())
            {
                importer.Add(_GQLPluginManager.GetAndInitializePlugin<IGraphDBImport>(plugin));
            }

            if (importer.Count == 0)
            {
                throw new GQLGrammarSetExtandableMemberException(typeof(IGraphDBImport), 
                            "There is no plugin found to set in GQL grammar.");
            }
            myGQLGrammar.SetGraphDBImporter(importer);

            #endregion

            #region export

            List<IGraphDBExport> exporter = new List<IGraphDBExport>();
            foreach (var plugin in _GQLPluginManager.GetPluginNameForType<IGraphDBExport>())
            {
                exporter.Add(_GQLPluginManager.GetAndInitializePlugin<IGraphDBExport>(plugin));
            }

            if (exporter.Count == 0)
            {
                throw new GQLGrammarSetExtandableMemberException(typeof(IGraphDBExport), 
                            "There is no plugin found to set in GQL grammar.");
            }
            myGQLGrammar.SetGraphDBExporter(exporter);

            #endregion

            #region additional statements

            List<IGQLStatementPlugin> statements = new List<IGQLStatementPlugin>();
            foreach (var plugin in _GQLPluginManager.GetPluginNameForType<IGQLStatementPlugin>())
            {
                statements.Add(_GQLPluginManager.GetAndInitializePlugin<IGQLStatementPlugin>(plugin,
                    new Dictionary<string, object>
                    {
                        {"sonesGQL", myGQLGrammar},
                        {"graphDB", _IGraphDBInstance}
                    }));
            }

            myGQLGrammar.SetStatements(statements);

            #endregion
        }

        #endregion
    }
}