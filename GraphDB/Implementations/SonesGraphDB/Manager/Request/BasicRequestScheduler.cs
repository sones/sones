using System;
using System.Collections.Generic;
using sones.GraphDB.Request;
using sones.Library.Settings;
using sones.Library.VersionedPluginManager;

namespace sones.GraphDB.Manager
{
    /// <summary>
    /// A really simple request scheduler, should be really fast
    /// </summary>
    public sealed class BasicRequestScheduler : IRequestScheduler
    {
        #region constructor

        /// <summary>
        /// Creates a new BasicRequestScheduler 
        /// BEWARE!!! This constructor is necessary for plugin-functionality.
        /// </summary>
        public BasicRequestScheduler()
        {

        }

        #endregion

        #region IRequestScheduler Members

        public bool ExecuteRequestInParallel(IRequest myRequest)
        {
            return true;

            //Example:
            //return myRequest.AccessMode != GraphDBAccessMode.TypeChange;
        }

        #endregion

        #region IPluginable Members

        public String PluginName
        {
            get { return "BasicRequestScheduler"; }
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
            return new BasicRequestScheduler();
        }

        #endregion
    }
}