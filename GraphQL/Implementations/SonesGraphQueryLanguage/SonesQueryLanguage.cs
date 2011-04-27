using System;
using System.Collections.Generic;
using sones.GraphDB;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Settings;
using sones.Library.Commons.Transaction;
using sones.Library.VersionedPluginManager;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.Plugins.SonesGQL.Functions;
using sones.Plugins.SonesGQL.Aggregates;
using sones.Plugins.Index.Interfaces;
using Irony.Parsing;
using sones.GraphQL.GQL.ErrorHandling;
using sones.GraphQL.StatementNodes;
using sones.GraphQL.ErrorHandling;
using System.Linq;
using sones.Library.ErrorHandling;

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

            #region create gql grammar and set aggregates, functions, indices, importer

            _GQLGrammar = new SonesGQLGrammar(myIGraphDBInstace);

            SetExtendableMember();

            #endregion

            //build parser
            _parser = new Parser(_GQLGrammar);

            //check language
            if (_parser.Language.ErrorLevel != GrammarErrorLevel.NoError)
            {
                throw new IronyInitializeGrammarException(_parser.Language.Errors, "");
            }
        }

        #endregion

        #region IGraphQL Members

        public QueryResult Query(SecurityToken mySecurityToken, TransactionToken myTransactionToken,
                                 string myQueryString)
        {
            ParseTree aTree;       //tree-like representation of the query-string
            AStatement statement;   //executeable statement
            //TODO: implement
            QueryResult queryResult = new QueryResult(myQueryString, "GQL", 0L, new ResultType());

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
                queryResult = statement.Execute(_IGraphDBInstance, myQueryString, mySecurityToken, myTransactionToken);
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
            get { return "GQL"; }
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

        private void SetExtendableMember()
        {
            List<IGQLAggregate> aggregates = new List<IGQLAggregate>();
            foreach (var plugin in _GQLPluginManager.GetPluginsForType<IGQLAggregate>())
            {
                aggregates.Add(_GQLPluginManager.GetAndInitializePlugin<IGQLAggregate>(plugin));
            }
            _GQLGrammar.SetAggregates(aggregates);

            List<ABaseFunction> functions = new List<ABaseFunction>();
            foreach (var plugin in _GQLPluginManager.GetPluginsForType<IGQLFunction>())
            {
                functions.Add(_GQLPluginManager.GetAndInitializePlugin<IGQLFunction>(plugin) as ABaseFunction);
            }
            _GQLGrammar.SetFunctions(functions);

            List<IIndex<IComparable, Int64>> indices = new List<IIndex<IComparable, Int64>>();
            foreach (var plugin in _GQLPluginManager.GetPluginsForType<IIndex<IComparable, Int64>>())
            {
                indices.Add(_GQLPluginManager.GetAndInitializePlugin<IIndex<IComparable, Int64>>(plugin));
            }
            _GQLGrammar.SetIndices(indices);
        }

        #endregion
    }
}