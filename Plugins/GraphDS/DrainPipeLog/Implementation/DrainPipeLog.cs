using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDS;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Request;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.VersionedPluginManager;
using sones.Plugins.GraphDS.DrainPipeLog.Storage;
using System.Threading;
using sones.GraphDB.Request.GetVertexType;
using sones.GraphDB.Request.GetEdgeType;
using sones.GraphDB.Request.GetIndex;
using sones.GraphDB.Request.Delete;
using sones.GraphDB.Request.Update;
using sones.GraphDB.Request.DropType;

namespace sones.Plugins.GraphDS.DrainPipeLog
{
    /// <summary>
    /// this is a GraphDS plugin which can be used to create a GraphDS bypass if you like. This
    /// plugin will be notified of each and every GQL and API query and can react uppon this
    /// </summary>
    public class DrainPipeLog : IDrainPipe
    {
        private AppendLog _AppendLog = null;
        // this holds the information if this plugin handles requests asyncronously or syncronously
        // this will have a large impact on performance and/or reliability
        private Boolean AsynchronousMode = false;
        // when writing in asynchronous mode everything will be written in a separate thread
        private Thread Async_WriteThread = null;
        // the max number of bytes to hold in the buffer, defaults to 10 MByte
        private Int32 MaximumAsyncBufferSize = 1024*1024*10;    // 10 MB
        private WriteThread WriteThreadInstance = null;        

        public DrainPipeLog()
        {

        }

        public DrainPipeLog(Dictionary<string, object> myParameters = null)
        {
            #region handle parameters
            String AppendLogPathAndName = "";
            Boolean CreateNew = false;
            Boolean FlushOnWrite = true;

            #region AsynchronousMode
            if (myParameters.ContainsKey("AsynchronousMode"))
            {
                AsynchronousMode = (Boolean)myParameters["AsynchronousMode"];
            }
            else
            {
                AsynchronousMode = false;
            }
            #endregion

            #region MaximumAsyncBufferSize
            if (myParameters.ContainsKey("MaximumAsyncBufferSize"))
            {
                MaximumAsyncBufferSize = (Int32)myParameters["MaximumAsyncBufferSize"];
            }
            else
            {
                MaximumAsyncBufferSize = 1024 * 1024 * 10;
            }
            #endregion

            #region AppendLogPathAndName
            if (myParameters.ContainsKey("AppendLogPathAndName"))
            {
                AppendLogPathAndName = (String)myParameters["AppendLogPathAndName"];
            }
            else
            {
                AppendLogPathAndName = "sones.drainpipelog";
            }
            #endregion

            #region CreateNew
            if (myParameters.ContainsKey("CreateNew"))
            {
                CreateNew = (Boolean)myParameters["CreateNew"];
            }
            #endregion

            #region FlushOnWrite
            if (myParameters.ContainsKey("FlushOnWrite"))
            {
                FlushOnWrite = (Boolean)myParameters["FlushOnWrite"];
            }
            #endregion

            #endregion

            _AppendLog = new AppendLog(AppendLogPathAndName, CreateNew, FlushOnWrite);
            WriteThreadInstance = new WriteThread(_AppendLog);

            #region Handle Asynchronous Mode
            if (AsynchronousMode)
            {
                Async_WriteThread = new Thread(new ThreadStart(WriteThreadInstance.Run));
                Async_WriteThread.Start();
            }
            #endregion
        }
        
        #region IPluginable
        public string PluginName
        {
            get { return "sones.drainpipelog"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get
            {
                return new Dictionary<string, Type> 
                { 
                    { "AsynchronousMode", typeof(Boolean) },
                    { "MaximumAsyncBufferSize", typeof(Int32) },
                    { "AppendLogPathAndName", typeof(String) },
                    { "CreateNew", typeof(Boolean) },
                    { "FlushOnWrite", typeof(Boolean) },                    
                };
            }
        }

        public IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            object result = typeof(DrainPipeLog).
                GetConstructor(new Type[] { typeof(Dictionary<string, object>) }).Invoke(new object[] { myParameters });
            return (IPluginable)result;
        }
        #endregion
        
        #region IGraphDS
        /// <summary>
        /// Shutdown of this plugin / GraphDS interface handling
        /// </summary>
        /// <param name="mySecurityToken"></param>
        public void Shutdown(sones.Library.Commons.Security.SecurityToken mySecurityToken)
        {
            WriteThreadInstance.Shutdown();

            if (AsynchronousMode)
            { 
                while (!WriteThreadInstance.ShutdownComplete)
                    Thread.Sleep(1);
            }
            // flush and close up
            if (_AppendLog != null)
                _AppendLog.Shutdown();

        }

        /// <summary>
        /// This will receive a query and store it to the log
        /// </summary>
        public sones.GraphQL.Result.QueryResult Query(sones.Library.Commons.Security.SecurityToken mySecurityToken, sones.Library.Commons.Transaction.TransactionToken myTransactionToken, string myQueryString, string myQueryLanguageName)
        {
            byte[] Part1,Part2,Part3,Part4 = null;
            System.IO.MemoryStream stream = new System.IO.MemoryStream();

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter Formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            #region Generate byte represenation of query
            if (mySecurityToken != null)
                Formatter.Serialize(stream, mySecurityToken);
            else
                Formatter.Serialize(stream, new SecurityToken());

            Part1 = stream.ToArray();
            if (myTransactionToken != null)
                Formatter.Serialize(stream, myTransactionToken);
            else
                Formatter.Serialize(stream, new TransactionToken(long.MaxValue));

            Part2 = stream.ToArray();
            if (myQueryString != null)
                Formatter.Serialize(stream, myQueryString);
            else
                Formatter.Serialize(stream, "");
            Part3 = stream.ToArray();
            if (myQueryLanguageName != null)
                Formatter.Serialize(stream, myQueryLanguageName);
            else
                Formatter.Serialize(stream, "");
            Part4 = stream.ToArray();
            #endregion

            byte[] Data = new byte[Part1.Length + Part2.Length + Part3.Length + Part4.Length];
            System.Buffer.BlockCopy(Part1, 0, Data, 0, Part1.Length);
            System.Buffer.BlockCopy(Part2, 0, Data, Part1.Length, Part2.Length);
            System.Buffer.BlockCopy(Part3, 0, Data, Part1.Length + Part2.Length, Part3.Length);
            System.Buffer.BlockCopy(Part4, 0, Data, Part1.Length + Part2.Length + Part3.Length, Part4.Length);

            Write(Data);
            return null;
        }
        #endregion

        #region Write
        private void Write(byte[] Data)
        {
            // allow this only once at a time...
            lock (this)
            {
                if (AsynchronousMode)
                {
                    if (MaximumAsyncBufferSize < Data.Length)
                    {
                        // if the Maximum Async Buffer Size if larger than the size of the data we have to write it synchronously
                        // And to write it syncronously we have to make sure we do not write in the middle of the existing queue
                        while (WriteThreadInstance.BytesInAsyncBuffer > 0)
                        {
                            Thread.Sleep(1);
                        }
                        // the buffer is empty now, write the new element
                        _AppendLog.Write(Data);
                        Data = null;
                    }
                    else
                    {
                        if (WriteThreadInstance.BytesInAsyncBuffer + Data.Length <= MaximumAsyncBufferSize)
                        {
                            // yeah, it will fit into the buffer
                            WriteThreadInstance.Write(Data);
                        }
                        else
                        {
                            // obviously the buffer is filled - wait till there's room
                            while (WriteThreadInstance.BytesInAsyncBuffer + Data.Length <= MaximumAsyncBufferSize)
                            {
                                Thread.Sleep(1);
                            }
                            WriteThreadInstance.Write(Data);
                        }
                    }
                }
                else
                {
                    // Syncronous-Mode writes are easy
                    _AppendLog.Write(Data);
                }
            }
        }
        #endregion

        #region IGraphDB Members

        public TResult CreateVertexTypes<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestCreateVertexTypes myRequestCreateVertexType, Converter.CreateVertexTypesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Clear<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestClear myRequestClear, Converter.ClearResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Delete<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestDelete myRequestDelete, Converter.DeleteResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Insert<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestInsertVertex myRequestInsert, Converter.InsertResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertices<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertices myRequestGetVertices, Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult TraverseVertex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurity, TransactionToken myTransactionToken, RequestTraverseVertex myRequestTraverseVertex, Converter.TraverseVertexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertexType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertexType myRequestGetVertexType, Converter.GetVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetAllVertexTypes<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetAllVertexTypes myRequestGetAllVertexTypes, Converter.GetAllVertexTypesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetEdgeType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetEdgeType myRequestGetEdgeType, Converter.GetEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetAllEdgeTypes<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetAllEdgeTypes myRequestGetAllEdgeTypes, Converter.GetAllEdgeTypesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertex myRequestGetVertex, Converter.GetVertexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Truncate<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestTruncate myRequestTruncate, Converter.TruncateResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DescribeIndex<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestDescribeIndex myRequestGetAllEdgeTypes, Converter.DescribeIndexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Update<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestUpdate myRequestUpdate, Converter.UpdateResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult CreateVertexType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestCreateVertexType myRequestCreateVertexType, Converter.CreateVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult DropType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestDropType myRequestDropType, Converter.DropTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public Guid ID
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ITransactionable Members

        public TransactionToken BeginTransaction(sones.Library.Commons.Security.SecurityToken mySecurityToken, bool myLongrunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable)
        {
            throw new NotImplementedException();
        }

        public void CommitTransaction(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public void RollbackTransaction(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserAuthentication Members

        public sones.Library.Commons.Security.SecurityToken LogOn(IUserCredentials toBeAuthenticatedCredentials)
        {
            throw new NotImplementedException();
        }

        public void LogOff(sones.Library.Commons.Security.SecurityToken toBeLoggedOfToken)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
