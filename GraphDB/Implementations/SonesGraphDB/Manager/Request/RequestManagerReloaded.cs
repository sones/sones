using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using sones.GraphDB.Request;
using sones.Library.ErrorHandling;
using sones.Library.Settings;
using sones.Library.VersionedPluginManager;
using System.Diagnostics;

namespace sones.GraphDB.Manager
{
    /// <summary>
    /// A manager that handles requests in a pipeline
    /// </summary>
    public sealed class RequestManagerReloaded : IRequestManager
    {
        #region data

        /// <summary>
        /// The meta manager that contains all relevant manager
        /// </summary>
        private IMetaManager _metaManager;

        /// <summary>
        /// The cancellation token source
        /// </summary>
        private CancellationTokenSource _cts;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request manager 
        /// BEWARE!!! This constructor is necessary for plugin-functionality.
        /// DO NOT USE THIS ONE IF YOU DIRECTLY INITIALIZE THIS COMPONENT
        /// </summary>
        public RequestManagerReloaded()
        {

        }

        /// <summary>
        /// Creates a new request manager
        /// </summary>
        /// <param name="myMetaManager">The meta mananger of the graphdb</param>
        public RequestManagerReloaded(IMetaManager myMetaManager)
        {
            _metaManager = myMetaManager;
        }

        #endregion


        #region IRequestManager Members

        public APipelinableRequest SynchronExecution(APipelinableRequest myToBeExecutedRequest)
        {
            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                myToBeExecutedRequest.Validate(_metaManager);

                myToBeExecutedRequest.Execute(_metaManager);

                sw.Stop();
            }
            catch (Exception e)
            {
                HandleErroneousRequest(ref myToBeExecutedRequest, e);
            }

            //set the stats
            myToBeExecutedRequest.Statistics = new RequestStatistics(sw.Elapsed);

            return myToBeExecutedRequest;
        }

        #endregion


        #region IPluginable Members

        public String PluginName
        {
            get { return "sones.requestmanager"; }
        }

        public Dictionary<String, Type> SetableParameters
        {
            get
            {
                return new Dictionary<string, Type>();
            }
        }

        public IPluginable InitializePlugin(Dictionary<String, Object> myParameters)
        {
            var result =  new RequestManagerReloaded();

            return result;
        }

        #endregion

        #region private helper

        #region HandleErroneousRequest

        /// <summary>
        /// Handles exceptions that occured while processing a request
        /// </summary>
        /// <param name="pipelineRequest">The request that has been processed</param>
        /// <param name="e">The exception that has been thrown</param>
        private void HandleErroneousRequest(ref APipelinableRequest pipelineRequest, Exception e)
        {
            var aSonesException = e as ASonesException ?? new UnknownException(e);

            //add the exception to the request
            pipelineRequest.Exception = aSonesException;
        }

        #endregion

        #endregion
    }
}