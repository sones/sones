using System;
using System.Collections.Generic;
using System.Linq;
using Irony.Parsing;
using sones.GraphDB;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.GQL.ErrorHandling;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.Result;
using sones.GraphQL.StatementNodes;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.ErrorHandling;
using sones.Library.Settings;
using sones.Library.VersionedPluginManager;
using sones.Plugins.Index.Interfaces;
using sones.Plugins.SonesGQL.Aggregates;
using sones.Plugins.SonesGQL.Functions;
using sones.Plugins.SonesGQL.DBImport;

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

        public QueryResult Query(SecurityToken mySecurityToken, TransactionToken myTransactionToken,
                                 string myQueryString)
        {
            //tree-like representation of the query-string
            ParseTree aTree;
            //executeable statement
            AStatement statement;   
            QueryResult queryResult = new QueryResult(myQueryString, "sones.gql", 0L, ResultType.Failed);

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
                queryResult = statement.Execute(_IGraphDBInstance, this, _GQLPluginManager,  myQueryString, mySecurityToken, myTransactionToken);
            }
            //TODO: implement
            //catch (GraphDBWarningException we)
            //{
            //    queryResult = new QueryResult(new List<IError>(), new List<IWarning>() { (we as GraphDBWarningException).GraphDBWarning });
            //}
            catch (ASonesException ee)
            {
                queryResult.Error = (ee as AGraphQLException);
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

        public IEnumerable<string> ExportGraphDDL(SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ExportGraphDML(SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IPluginable Members

        public string PluginName
        {
            get { return "sones.gql"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get
            {
                return new Dictionary<string, Type> 
                { 
                    { "GraphDB", typeof(IGraphDB) }
                };
            }
        }

        public IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            IGraphDB dbInstance = null;

            if (myParameters != null)
            {
                if (myParameters.ContainsKey("GraphDB"))
                {
                    dbInstance = (IGraphDB)myParameters["GraphDB"];
                }
            }

            object result = typeof(SonesQueryLanguage).
                GetConstructor(new Type[] { typeof(IGraphDB) }).Invoke(new object[] { dbInstance });

            return (IPluginable)result;
        }

        #endregion

        #region private helper

        private void SetExtendableMember(SonesGQLGrammar myGQLGrammar)
        {
            List<IGQLAggregate> aggregates = new List<IGQLAggregate>();
            foreach (var plugin in _GQLPluginManager.GetPluginsForType<IGQLAggregate>())
            {
                aggregates.Add(_GQLPluginManager.GetAndInitializePlugin<IGQLAggregate>(plugin));
            }

            if (aggregates.Count == 0)
            {
                throw new GQLGrammarSetExtandableMemberException(typeof(IGQLAggregate), "There is no plugin found to set in GQL grammar.");
            }
            myGQLGrammar.SetAggregates(aggregates);

            List<IGQLFunction> functions = new List<IGQLFunction>();
            foreach (var plugin in _GQLPluginManager.GetPluginsForType<IGQLFunction>())
            {
                functions.Add(_GQLPluginManager.GetAndInitializePlugin<IGQLFunction>(plugin) as IGQLFunction);
            }

            if (functions.Count == 0)
            {
                throw new GQLGrammarSetExtandableMemberException(typeof(IGQLFunction), "There is no plugin found to set in GQL grammar.");
            }
            myGQLGrammar.SetFunctions(functions);

            bool foundAtLeastOneValidIndex = false;

            List<IIndex<IComparable, Int64>> indices = new List<IIndex<IComparable, Int64>>();
            foreach(var plugin in _GQLPluginManager.GetPluginsForType<ISingleValueIndex<IComparable, Int64>>())
            {
                indices.Add((IIndex<IComparable, Int64>)_GQLPluginManager.GetAndInitializePlugin<ISingleValueIndex<IComparable, Int64>>(plugin));

                foundAtLeastOneValidIndex = true;
            }

            foreach(var plugin in _GQLPluginManager.GetPluginsForType<IMultipleValueIndex<IComparable, Int64>>())
            {
                indices.Add((IIndex<IComparable, Int64>)_GQLPluginManager.GetAndInitializePlugin<IMultipleValueIndex<IComparable, Int64>>(plugin));

                foundAtLeastOneValidIndex = true;
            }

            foreach (var plugin in _GQLPluginManager.GetPluginsForType<IVersionedIndex<IComparable, Int64, Int64>>())
            {
                indices.Add((IIndex<IComparable, Int64>)_GQLPluginManager.GetAndInitializePlugin<IVersionedIndex<IComparable, Int64, Int64>>(plugin));
            }

            if (!foundAtLeastOneValidIndex)
            {
                throw new GQLGrammarSetExtandableMemberException(typeof(IIndex<IComparable, Int64>), "There is no valid index plugin found to set in GQL grammar. Expected at least SingleValueIndex or MultiValueIndex");
            }
            myGQLGrammar.SetIndices(indices);

            List<IGraphDBImport> importer = new List<IGraphDBImport>();
            foreach (var plugin in _GQLPluginManager.GetPluginsForType<IGraphDBImport>())
            {
                importer.Add(_GQLPluginManager.GetAndInitializePlugin<IGraphDBImport>(plugin));
            }

            if (importer.Count == 0)
            {
                throw new GQLGrammarSetExtandableMemberException(typeof(IGraphDBImport), "There is no plugin found to set in GQL grammar.");
            }
            myGQLGrammar.SetGraphDBImporter(importer);
        }

        #endregion
    }
}