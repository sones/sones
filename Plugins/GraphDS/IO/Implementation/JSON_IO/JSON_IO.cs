using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.IO;
using System.Xml.Schema;
using sones.GraphQL.Result;
using sones.Library.Settings;
using sones.Library.VersionedPluginManager;
using System.Xml;
using System.Reflection;


namespace sones.Plugins.GraphDS.IOInterface.JSON_IO
{
    public sealed class XML_IO : IOInterface
    {

        #region Data

        private readonly ContentType _contentType;

        #endregion

        #region Constructors

        public XML_IO()
        {
            _contentType = new ContentType("application/json") { CharSet = "UTF-8" };
        }

        #endregion

        #region IOInterface

        public string GenerateOutputResult(QueryResult myQueryResult)
        {
            throw new NotImplementedException();
        }

        public QueryResult GenerateQueryResult(string myResult)
        {
            throw new NotImplementedException();
        }

        public ContentType ContentType
        {
            get { return _contentType; }
        }

        #endregion


        #region IPluginable

        public string PluginName
        {
            get { return "JSON_IO"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string, Type>(); }
        }

        public IPluginable InitializePlugin(Dictionary<string, object> myParameters, GraphApplicationSettings myApplicationSetting)
        {
            return InitializePlugin();
        }

        public IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            object result = Activator.CreateInstance(typeof(XML_IO));

            return (IPluginable)result;
        }

        #endregion

    }
}
