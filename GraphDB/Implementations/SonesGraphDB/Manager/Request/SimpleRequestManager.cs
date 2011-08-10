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
using System.Threading;
using sones.GraphDB.Request;
using sones.Library.ErrorHandling;
using sones.Library.VersionedPluginManager;
using System.Diagnostics;

namespace sones.GraphDB.Manager
{
    /// <summary>
    /// A manager that handles requests
    /// </summary>
    public sealed class SimpleRequestManager : IRequestManager
    {
        #region data

        /// <summary>
        /// The meta manager that contains all relevant manager
        /// </summary>
        private IMetaManager _metaManager;

        /// <summary>
        /// The cancellation token source
        /// </summary>
        //private CancellationTokenSource _cts;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new request manager 
        /// BEWARE!!! This constructor is necessary for plugin-functionality.
        /// DO NOT USE THIS ONE IF YOU DIRECTLY INITIALIZE THIS COMPONENT
        /// </summary>
        public SimpleRequestManager()
        {

        }

        /// <summary>
        /// Creates a new request manager
        /// </summary>
        /// <param name="myMetaManager">The meta mananger of the graphdb</param>
        public SimpleRequestManager(IMetaManager myMetaManager)
        {
            _metaManager = myMetaManager;
        }

        #endregion


        #region IRequestManager Members

        public APipelinableRequest SynchronExecution(APipelinableRequest myToBeExecutedRequest)
        {
            Stopwatch sw = Stopwatch.StartNew();

            myToBeExecutedRequest.Validate(_metaManager);

            myToBeExecutedRequest.Execute(_metaManager);

            sw.Stop();

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

        public String PluginShortName
        {
            get { return "requman"; }
        }

        public PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }


        public IPluginable InitializePlugin(String myUniqueString, Dictionary<String, Object> myParameters)
        {
            var result =  new SimpleRequestManager();

            return result;
        }

        public void Dispose()
        { }

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