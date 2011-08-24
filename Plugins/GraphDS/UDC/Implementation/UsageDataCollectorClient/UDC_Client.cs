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
using System.Text;
using sones.GraphDS.UDC.Helper;
using sones.Library.VersionedPluginManager;
using System.Threading;

namespace sones.GraphDS.UDC
{
    /// <summary>
    /// 
    /// </summary>
    public class UDC_Client : IUsageDataCollector
    {
        private UDC_Client_Thread UDCClientThreadInstance = null;
        private Thread UDCClientThread = null;

        #region Constructors
        public UDC_Client()
        {}

        public UDC_Client(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            Int32 WaitUpFront = 1 * 60 * 1000;  // 1 minute
            Int32 UpdateInterval = 30 * 60 * 1000; // 30 minutes

            try
            {
                if (myParameters != null)
                {
                    if (myParameters.ContainsKey("UDCWaitUpfrontTime"))
                    {
                        WaitUpFront = (Int32)myParameters["UDCWaitUpfrontTime"];
                    }
                    if (myParameters.ContainsKey("UDCUpdateInterval"))
                    {
                        UpdateInterval = (Int32)myParameters["UDCUpdateInterval"];
                    }
                }
            }
            catch (Exception)    // eat all exceptions...
            { }

            // start up the thread...
            UDCClientThreadInstance = new UDC_Client_Thread(WaitUpFront, UpdateInterval);
            UDCClientThread = new Thread(new ThreadStart(UDCClientThreadInstance.Run));
            UDCClientThread.Start();
        }
        #endregion

        #region IPluginable
        public string PluginName
        {
            get { return "sones.GraphDS.UsageDataCollectorClient"; }
        }

        public string PluginShortName
        {
            get { return "sones.GraphDS.UsageDataCollectorClient"; }
        }

        /*public String PluginDescription
        {
            get { return ""; }
        }*/

        public Library.VersionedPluginManager.PluginParameters<Type> SetableParameters
        {
            get
            {
                return new PluginParameters<Type> 
                { 
                    { "UDCWaitUpfrontTime", typeof(Int32) },
                    { "UDCUpdateInterval", typeof(Int32) },
                };
            }
        }

        public Library.VersionedPluginManager.IPluginable InitializePlugin(string UniqueString, Dictionary<string, object> myParameters = null)
        {
            //Console.WriteLine("UDC");
            var result = new UDC_Client(UniqueString, myParameters);
            return (IPluginable)result;

        }
        #endregion

        public void Dispose()
        {            
        }

        public void Shutdown()
        {
            if (UDCClientThreadInstance != null)
            {
                UDCClientThreadInstance.Shutdown();
            }
        }
    }
}
