using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.GraphDS.Services;
using sones.Library.VersionedPluginManager;

namespace sones.GraphDS.Services.RESTService
{
    public class RESTService : IService, IPluginable
    {
        #region Data

        private IGraphDS GraphDS;

        #endregion

        #region C'tors

        public RESTService()
        {
        }

        public RESTService(IGraphDS myGraphDS)
        {
            GraphDS = myGraphDS;
        }


        #endregion
       

               
        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public AServiceStatus GetCurrentStatus()
        {
            throw new NotImplementedException();
        }

        public string PluginName
        {
            get { return "sones.RESTService"; }
        }

        public PluginParameters<Type> SetableParameters
        {
            get { return null; }
        }

        public IPluginable InitializePlugin(string UniqueString, Dictionary<string, object> myParameters = null)
        {
            return new RESTService((IGraphDS)myParameters["GraphDS"]);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
