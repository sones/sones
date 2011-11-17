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
using sones.GraphDB.Request;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB;
using System.Net;
using System.Web;
using System.IO;
using System.Text;
using sones.Plugins.GraphDS.IO.XML_IO;
using System.Reflection;
using System.Threading;





namespace sones.GraphDS.GraphDSRESTClient
{
    /// <summary>
    /// A GraphDS client that communicates via REST
    /// </summary>
    /// 
    public sealed class GraphDS_RESTClient : IGraphDSClient
    {
        #region Data
        private readonly String _Username;

        private readonly String _Password;

        private readonly NetworkCredential _Credentials;

        private String _Host;
        
        private String getHost() 
        {
         return this._Host; 
        }
                        
        private void setHost(String myHost){
            if (!myHost.Contains("http://"))
            {
                _Host = "http://" + myHost;
            }
            else
            {
                _Host = myHost;
            }
        }
        
        private readonly uint   _Port;

        private readonly String _GQLPATTERN = "/gql";

        private readonly String _GQLUri;

        private XML_IO _Parser;

       

        #endregion

        #region Constructors

        public GraphDS_RESTClient(String myHost, String myUsername, String myPassword, uint myPort = 9975U)
        {
            setHost(myHost);
            _Username = myUsername;
            _Password = myPassword;
            _Port = myPort;
            _Credentials = new NetworkCredential(myUsername, myPassword);
            
            _GQLUri = getHost() + ":" + _Port.ToString() + _GQLPATTERN;

            _Parser = new XML_IO();


            //workaround to communicate with graphds server running on mono
            Assembly curSection = Assembly.GetAssembly(typeof(System.Net.Configuration.SettingsSection));
            if (curSection != null)
            {
                Type setting = curSection.GetType("System.Net.Configuration.SettingsSectionInternal");
                if (setting != null)
                {
                    var instance = setting.InvokeMember("Section", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.NonPublic, null, null, new object[] { });

                    if (instance != null)
                    {
                        FieldInfo unsafeParsing = setting.GetField("useUnsafeHeaderParsing", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (unsafeParsing != null)
                        {
                            unsafeParsing.SetValue(instance, true);
                        }
                    }
                }
            }
           
         }
        #endregion

        #region IGraphDS Members

        public void Shutdown(SecurityToken mySecurityToken)
        {
            //TODO
            //throw new NotImplementedException();
        }

        public IQueryResult Query(SecurityToken mySecurityToken, Int64 myTransactionToken, string myQueryString, string myQueryLanguageName)
        {
            IQueryResult result = null;
            String resonseXML = FetchGraphDBOutput(myQueryString);
            result = _Parser.GenerateQueryResult(resonseXML);
            return result;
        }

        

        #endregion

        #region IGraphDB Members / Not Implemented

        public TResult CreateVertexTypes<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestCreateVertexTypes myRequestCreateVertexType, Converter.CreateVertexTypesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Clear<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestClear myRequestClear, Converter.ClearResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Delete<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, Int64 myTransactionToken, RequestDelete myRequestDelete, Converter.DeleteResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Insert<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestInsertVertex myRequestInsert, Converter.InsertResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertices<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestGetVertices myRequestGetVertices, Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult TraverseVertex<TResult>(SecurityToken mySecurity, Int64 myTransactionToken, RequestTraverseVertex myRequestTraverseVertex, Converter.TraverseVertexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertexType<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestGetVertexType myRequestGetVertexType, Converter.GetVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetAllVertexTypes<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestGetAllVertexTypes myRequestGetAllVertexTypes, Converter.GetAllVertexTypesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetEdgeType<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestGetEdgeType myRequestGetEdgeType, Converter.GetEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetAllEdgeTypes<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestGetAllEdgeTypes myRequestGetAllEdgeTypes, Converter.GetAllEdgeTypesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertex<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestGetVertex myRequestGetVertex, Converter.GetVertexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Truncate<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestTruncate myRequestTruncate, Converter.TruncateResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DescribeIndex<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestDescribeIndex myRequestDescribeIndex, Converter.DescribeIndexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DescribeIndices<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestDescribeIndex myRequestDescribeIndex, Converter.DescribeIndicesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult CreateVertexType<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestCreateVertexType myRequestCreateVertexType, Converter.CreateVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Update<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestUpdate myRequestUpdate, Converter.UpdateResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DropVertexType<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestDropVertexType myRequestDropType, Converter.DropVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DropEdgeType<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestDropEdgeType myRequestDropType, Converter.DropEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }
        public TResult DropIndex<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestDropIndex myRequestDropIndex, Converter.DropIndexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult CreateIndex<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestCreateIndex myRequestCreateIndex, Converter.CreateIndexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult RebuildIndices<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestRebuildIndices myRequestRebuildIndices, Converter.RebuildIndicesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult AlterVertexType<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestAlterVertexType myRequestAlterVertexType, Converter.AlterVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult CreateEdgeType<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestCreateEdgeType myRequestCreateEdgeType, Converter.CreateEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult CreateEdgeTypes<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestCreateEdgeTypes myRequestCreateEdgeTypes, Converter.CreateEdgeTypesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult AlterEdgeType<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestAlterEdgeType myRequestAlterEdgeType, Converter.AlterEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public Guid ID
        {
            get { throw new NotImplementedException(); }
        }

        public TResult GetVertexCount<TResult>(SecurityToken mySecurityToken, Int64 myTransactionToken, RequestGetVertexCount myRequestGetVertexCount, Converter.GetVertexCountResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ITransactionable Members / Not Implemented

        public Int64 BeginTransaction(SecurityToken mySecurityToken, bool myLongrunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable)
        {
            throw new NotImplementedException();
        }

        public void CommitTransaction(SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public void RollbackTransaction(SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserAuthentication Members / Not Implemented

        public SecurityToken LogOn(IUserCredentials toBeAuthenticatedCredentials)
        {
            throw new NotImplementedException();
        }

        public void LogOff(SecurityToken toBeLoggedOfToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Utilities

        private String FetchGraphDBOutput(String myQueryString)
        {
            
            try
            {

                HttpWebRequest request = WebRequest.Create(_GQLUri) as HttpWebRequest;
                
                request.Credentials = _Credentials;
                request.Method = "POST";
                request.Accept = "application/xml";
                request.UserAgent = "GraphDSRESTClient";
                request.KeepAlive = false;
                request.Timeout = Timeout.Infinite;
                var myStream = request.GetRequestStream();

                using (var writer = new StreamWriter(myStream,Encoding.UTF8))
                {
                    try
                    {
                        writer.Write(myQueryString);
                        writer.Flush();
                    }
                    finally
                    {
                        myStream.Close();
                    }
                   
                }

                StreamReader reader;
                StringBuilder resonseXML = null;

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (request.HaveResponse == true && response != null)
                {
                    reader = new StreamReader(response.GetResponseStream());
                    resonseXML = new StringBuilder(reader.ReadToEnd());
                }
                response.Close();
                return resonseXML.ToString();

            }
            catch (WebException ex)
            {
                throw ex;
            }

        }

        #endregion
    }
}
