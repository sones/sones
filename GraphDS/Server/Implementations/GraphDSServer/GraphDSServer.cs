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
using System.IdentityModel.Tokens;
using System.Net;
using System.Collections.Generic;
using System.ServiceModel;
using System.IdentityModel.Selectors;
using sones.GraphDB;
using sones.GraphDB.Request;
using sones.GraphDSServer.ErrorHandling;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDS.PluginManager.GraphDSPluginManager;
using sones.GraphQL;
using sones.GraphDB.Request.GetVertexType;
using sones.GraphDB.Request.GetEdgeType;
using sones.GraphDB.Request.GetIndex;
using sones.Plugins.GraphDS;
using sones.Library.VersionedPluginManager;
using sones.GraphDS.PluginManager;
using sones.Library.Network.HttpServer;
using sones.Library.Network.HttpServer.Security;
using System.Net.Sockets;
using sones.Plugins.GraphDS.Services;
using sones.GraphDS;

namespace sones.GraphDSServer
{
    public sealed class GraphDS_Server : IGraphDSServer
    {
        #region Data

        /// <summary>
        /// The internal iGraphDB instance
        /// </summary>
        private readonly IGraphDB                       _iGraphDB;

        /// <summary>
        /// The service guid.
        /// </summary>
        private readonly Guid                           _ID;

        /// <summary>
        /// The graph ds plugin manager.
        /// </summary>
        private readonly GraphDSPluginManager           _pluginManager;

        /// <summary>
        /// The plugins with there settings.
        /// </summary>
        private readonly GraphDSPlugins                 _plugins;

        /// <summary>
        /// The list of supported graph ql type instances
        /// </summary>
        private readonly Dictionary<String, IGraphQL>   _QueryLanguages;            // dictionary because we can only have one query language instance per name
        private readonly Dictionary<String, IService> _IServices;                   // dictionary because only one name per service
        private readonly List<KeyValuePair<String,IDrainPipe>> _DrainPipes;         // you could have multiple drainpipes with different parameters sporting the same name

        #endregion

        #region Constructor

        /// <summary>
        /// The GraphDS server constructor.
        /// </summary>
        /// <param name="myGraphDB">The graph db instance.</param>
        /// <param name="myIPAddress">the IP adress this GraphDS Server should bind itself to</param>
        /// <param name="myPort">the port this GraphDS Server should listen on</param>
        /// <param name="PluginDefinitions">the plugins that shall be loaded and their according parameters</param>
        public GraphDS_Server(IGraphDB myGraphDB, GraphDSPlugins Plugins = null)
        {        
            _iGraphDB = myGraphDB;
            _pluginManager = new GraphDSPluginManager();
            _ID = new Guid();
            _QueryLanguages = new Dictionary<string, IGraphQL>();
            _DrainPipes = new List<KeyValuePair<String, IDrainPipe>>();
            _IServices = new Dictionary<String, IService>();
            _plugins = Plugins;

            #region Load Configured Plugins
            
            if (_plugins == null)
            {
                #region set the defaults
                // which are: 
                //  GQL with GraphDB Parameter
                #region GQL
                List<PluginDefinition> QueryLanguages = new List<PluginDefinition>();
                Dictionary<string, object> GQL_Parameters = new Dictionary<string, object>();
                GQL_Parameters.Add("GraphDB", myGraphDB);

                QueryLanguages.Add(new PluginDefinition("sones.gql", GQL_Parameters));
                #endregion

                _plugins = new GraphDSPlugins(QueryLanguages,null);
                #endregion
            }

            // now at least the default plugins or a user setup is stored in the _plugins structure
            // iterate through and instantiate if found
            #region IGraphQL Plugins
            if (_plugins.IGraphQLPlugins != null)
            {
                // we got QL
                foreach (PluginDefinition _pd in _plugins.IGraphQLPlugins)
                {
                    // load!
                    IGraphQL loaded = LoadIGraphQLPlugins(_pd);
                    
                    // add!
                    if (loaded != null)
                    {
                        _QueryLanguages.Add(_pd.NameOfPlugin, loaded);
                    }
                    //                    else
                    //                        System.Diagnostics.Debug.WriteLine("Could not load plugin " + _pd.NameOfPlugin);
                }
            }
            #endregion

           
            #region IDrainPipe Plugins
            if (_plugins.IDrainPipePlugins != null)
            {
                // we got IDrainPipePlugins
                foreach (PluginDefinition _pd in _plugins.IDrainPipePlugins)
                {
                    // load!
                    IDrainPipe loaded = LoadIDrainPipes(_pd);
                    // add!
                    if (loaded != null)
                    {
                        _DrainPipes.Add(new KeyValuePair<string,IDrainPipe>(_pd.NameOfPlugin, loaded));
                    }
                    //                    else
                    //                        System.Diagnostics.Debug.WriteLine("Could not load plugin " + _pd.NameOfPlugin);

                }
            }
            #endregion

            #endregion
        }

        

        #endregion

        #region Plugin Loading Helpers
        /// <summary>
        /// Load the IDrainPipes plugins
        /// </summary>
        private IDrainPipe LoadIDrainPipes(PluginDefinition myIDrainPipesPlugin)
        {
            return _pluginManager.GetAndInitializePlugin<IDrainPipe>(myIDrainPipesPlugin.NameOfPlugin, myParameter: myIDrainPipesPlugin.PluginParameter);
        }

        /// <summary>
        /// Load the IGraphQL plugins
        /// </summary>
        private IGraphQL LoadIGraphQLPlugins(PluginDefinition myIGraphQLPlugin)
        {
            return _pluginManager.GetAndInitializePlugin<IGraphQL>(myIGraphQLPlugin.NameOfPlugin, myParameter: myIGraphQLPlugin.PluginParameter);
        }

        #endregion

        #region IGraphDSServer Member

        #region Start GraphDS Service

        public void StartService(String myServiceName, IDictionary<string, object> myParameter)
        {
            IService Service = null;

            if (!_IServices.TryGetValue(myServiceName, out Service))
            {
                try
                {
                    IGraphDS GraphDS = this as IGraphDS;
                    Dictionary<string, object> Parameter = new Dictionary<string, object>();
                    Parameter.Add("GraphDS", GraphDS);
                    Service = _pluginManager.GetAndInitializePlugin<IService>(myServiceName, Parameter);
                }
                catch (Exception Ex)
                {
                    throw new ServiceException("An error occured when trying to initialize " + myServiceName + "!" + Environment.NewLine + "See inner exception for details.", Ex);
                }
                _IServices.Add(Service.PluginName, Service);
            }
            try
            {
                Service.Start(myParameter);
            }
            catch (Exception Ex)
            {
                throw new ServiceException("An error occured when trying to start " + myServiceName + "!" + Environment.NewLine + "See inner exception for details.", Ex);
            }

        }

        #endregion

        #region Stop GraphDS Service

        public void StopService(String myServiceName)
        {
            IService Service = null;
            if (_IServices.TryGetValue(myServiceName, out Service))
            {
                try
                {
                    Service.Stop();
                }
                catch (Exception Ex)
                {
                    throw new ServiceException("An error occured when trying to stop " + myServiceName + "!", Ex);
                }

            }
            else
            {
                throw new ServiceException("The service " + myServiceName + "is unrecognized! Maybe the service was never started.");
            }
        }

        #endregion

        #region Status

        public AServiceStatus GetServiceStatus(String myServiceName)
        {
            IService Service = null;
            if (_IServices.TryGetValue(myServiceName, out Service))
            {
                try
                {
                    return Service.GetCurrentStatus();
                }
                catch (Exception Ex)
                {
                    throw new ServiceException("An error occured when trying to get the current status of " + myServiceName + "!", Ex);
                }

            }
            else
            {
                throw new ServiceException("The service " + myServiceName + "is unrecognized! Maybe the service was never started.");
            }
        }

        #endregion
        
        #endregion
         
        #region IGraphDS Members

        public void Shutdown(sones.Library.Commons.Security.SecurityToken mySecurityToken)
        {
            _iGraphDB.Shutdown(mySecurityToken);
        }

        public QueryResult Query(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, string myQueryString, string myQueryLanguageName)
        {
            IGraphQL queryLanguage;

            if (_QueryLanguages.TryGetValue(myQueryLanguageName, out queryLanguage))
            {
                // drain every query (before the query)
                foreach (KeyValuePair<String,IDrainPipe> _drainpipe in _DrainPipes)
                {
                    _drainpipe.Value.Query(mySecurityToken, myTransactionToken, myQueryString, myQueryLanguageName);
                }

                QueryResult result = queryLanguage.Query(mySecurityToken, myTransactionToken, myQueryString);

                // drain every query result (after the query)
                foreach (KeyValuePair<String, IDrainPipe> _drainpipe in _DrainPipes)
                {
                    _drainpipe.Value.DrainQueryResult(result);
                }

                return result;
            }
            else
            {
                throw new QueryLanguageNotFoundException(String.Format("The GraphDS server does not support the query language {0}", myQueryLanguageName));
            }
        }

        #endregion

        #region IGraphDB Members

        public TResult CreateVertexTypes<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestCreateVertexTypes myRequestCreateVertexType, Converter.CreateVertexTypesResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.CreateVertexTypes<TResult>(mySecurityToken, myTransactionToken, myRequestCreateVertexType, myOutputconverter);
        }

        public TResult Clear<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestClear myRequestClear, Converter.ClearResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.Clear<TResult>(mySecurityToken, myTransactionToken, myRequestClear, myOutputconverter);
        }

        public TResult Delete<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestDelete myRequestDelete, Converter.DeleteResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.Delete<TResult>(mySecurityToken, myTransactionToken, myRequestDelete, myOutputconverter);
        }

        public TResult Insert<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestInsertVertex myRequestInsert, Converter.InsertResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.Insert<TResult>(mySecurityToken, myTransactionToken, myRequestInsert, myOutputconverter);
        }

        public TResult GetVertices<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertices myRequestGetVertices, Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.GetVertices<TResult>(mySecurityToken, myTransactionToken, myRequestGetVertices, myOutputconverter);
        }

        public TResult TraverseVertex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurity, TransactionToken myTransactionToken, RequestTraverseVertex myRequestTraverseVertex, Converter.TraverseVertexResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.TraverseVertex<TResult>(mySecurity, myTransactionToken, myRequestTraverseVertex, myOutputconverter);
        }

        public TResult GetVertexType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertexType myRequestGetVertexType, Converter.GetVertexTypeResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.GetVertexType<TResult>(mySecurityToken, myTransactionToken, myRequestGetVertexType, myOutputconverter);
        }

        public TResult GetAllVertexTypes<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetAllVertexTypes myRequestGetAllVertexTypes, Converter.GetAllVertexTypesResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.GetAllVertexTypes<TResult>(mySecurityToken, myTransactionToken, myRequestGetAllVertexTypes, 
                                                        myOutputconverter);
        }

        public TResult GetEdgeType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetEdgeType myRequestGetEdgeType, Converter.GetEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.GetEdgeType<TResult>(mySecurityToken, myTransactionToken, myRequestGetEdgeType,
                                                  myOutputconverter);
        }

        public TResult GetAllEdgeTypes<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetAllEdgeTypes myRequestGetAllEdgeTypes, Converter.GetAllEdgeTypesResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.GetAllEdgeTypes<TResult>(mySecurityToken, myTransactionToken, myRequestGetAllEdgeTypes,
                                                  myOutputconverter);
        }

        public TResult GetVertex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertex myRequestGetVertex, Converter.GetVertexResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.GetVertex<TResult>(mySecurityToken, myTransactionToken, myRequestGetVertex,
                                                myOutputconverter);
        }

        public TResult Truncate<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestTruncate myRequestTruncate, Converter.TruncateResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.Truncate<TResult>(mySecurityToken, myTransactionToken, myRequestTruncate, 
                                                myOutputconverter);
        }

        public TResult DescribeIndex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestDescribeIndex myRequestDescribeIndex, Converter.DescribeIndexResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.DescribeIndex<TResult>(mySecurityToken, myTransactionToken, myRequestDescribeIndex, 
                                                    myOutputconverter);
        }

        public TResult DescribeIndices<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestDescribeIndex myRequestDescribeIndex, Converter.DescribeIndicesResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.DescribeIndices<TResult>(mySecurityToken, myTransactionToken, myRequestDescribeIndex, myOutputconverter);
        }

        public TResult CreateVertexType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestCreateVertexType myRequestCreateVertexType, Converter.CreateVertexTypeResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.CreateVertexType<TResult>(mySecurityToken, myTransactionToken, myRequestCreateVertexType, myOutputconverter);
        }

        public TResult Update<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestUpdate myRequestUpdate, Converter.UpdateResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.Update<TResult>(mySecurityToken, 
                                                myTransactionToken,     
                                                myRequestUpdate, 
                                                myOutputconverter);
        }

        public TResult DropType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestDropVertexType myRequestDropType, Converter.DropVertexTypeResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.DropType<TResult>(mySecurityToken, 
                                                myTransactionToken, 
                                                myRequestDropType, 
                                                myOutputconverter);
        }

        public TResult DropIndex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestDropIndex myRequestDropIndex, Converter.DropIndexResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.DropIndex<TResult>(mySecurityToken, 
                                                myTransactionToken, 
                                                myRequestDropIndex,     
                                                myOutputconverter);
        }

        public TResult CreateIndex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestCreateIndex myRequestCreateIndex, Converter.CreateIndexResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.CreateIndex<TResult>(mySecurityToken, 
                                                    myTransactionToken, 
                                                    myRequestCreateIndex,     
                                                    myOutputconverter);
        }

        public TResult RebuildIndices<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestRebuildIndices myRequestRebuildIndices, Converter.RebuildIndicesResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.RebuildIndices<TResult>(mySecurityToken,
                                                    myTransactionToken,
                                                    myRequestRebuildIndices,
                                                    myOutputconverter);
        }

        public TResult AlterVertexType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestAlterVertexType myRequestAlterVertexType, Converter.AlterVertexTypeResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.AlterVertexType<TResult>(
                mySecurityToken,
                myTransactionToken,
                myRequestAlterVertexType,
                myOutputconverter);
        }

        public TResult CreateEdgeType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestCreateEdgeType myRequestCreateVertexType, Converter.CreateEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.CreateEdgeType<TResult>(
                mySecurityToken,
                myTransactionToken,
                myRequestCreateVertexType,
                myOutputconverter);
        }

        public TResult AlterEdgeType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestAlterEdgeType myRequestAlterEdgeType, Converter.AlterEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.AlterEdgeType<TResult>(
                mySecurityToken,
                myTransactionToken,
                myRequestAlterEdgeType,
                myOutputconverter);
        }

        public Guid ID
        {
            get { return _ID; }
        }

        public TResult GetVertexCount<TResult>(Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertexCount myRequestGetVertexCount, Converter.GetVertexCountResultConverter<TResult> myOutputconverter)
        {
            return _iGraphDB.GetVertexCount<TResult>(
                mySecurityToken,
                myTransactionToken,
                myRequestGetVertexCount,
                myOutputconverter);
        }

        #endregion

        #region ITransactionable Members

        public TransactionToken BeginTransaction(sones.Library.Commons.Security.SecurityToken mySecurityToken, bool myLongrunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable)
        {
            return _iGraphDB.BeginTransaction(mySecurityToken, myLongrunning, myIsolationLevel);
        }

        public void CommitTransaction(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            _iGraphDB.CommitTransaction(mySecurityToken, myTransactionToken);
        }

        public void RollbackTransaction(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            _iGraphDB.RollbackTransaction(mySecurityToken, myTransactionToken);
        }

        #endregion

        #region IUserAuthentication Members

        public sones.Library.Commons.Security.SecurityToken LogOn(IUserCredentials toBeAuthenticatedCredentials)
        {
            return _iGraphDB.LogOn(toBeAuthenticatedCredentials);
        }

        public void LogOff(sones.Library.Commons.Security.SecurityToken toBeLoggedOfToken)
        {
            _iGraphDB.LogOff(toBeLoggedOfToken);
        }

        #endregion
    }
}
