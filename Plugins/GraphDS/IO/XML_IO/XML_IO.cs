using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mime;
using System.Xml.Linq;
using sones.GraphQL.Result;
using sones.Library.Settings;
using sones.Library.VersionedPluginManager;
using System.IO;


namespace sones.Plugins.GraphDS.IOInterface
{
    public sealed class XML_IO : IOInterface
    {

        #region Data

        private readonly ContentType _contentType;

        #endregion

        #region Constructors

        public XML_IO()
        {
            _contentType = new ContentType("application/xml") { CharSet = "UTF-8" };
        }

        #endregion

        #region IOInterface
        
        public string GenerateOutputResult(QueryResult myQueryResult)
        {
            XDocument document = new XDocument(new XDeclaration("1.0", _contentType.CharSet, "yes"));
            
            /*document.AddFirst("Result");
            var root = document.Element("Result");

            root.Add("Query", new XAttribute("Language", myQueryResult.NameOfQuerylanguage), myQueryResult.Query,
                     new XElement("Error", myQueryResult.Error.Message),
                     new XElement("Number of effected vertices", myQueryResult.NumberOfAffectedVertices));
            

            var vertices = new XElement("Vertices");


            myQueryResult.Vertices.AsParallel().ForAll(
                (item) =>
                    {
                        vertices.Add("Type Name", item.VertexTypeName);
                        vertices.Add("Vertex ID", item.VertexID);
                        vertices.Add("Revision ID", item.VertexRevisionID.ToString());
                        vertices.Add("Edition Name", item.EditionName);
                        vertices.Add("Binary Data",
                                     from binProp in item.GetAllBinaryProperties()
                                     select
                                         new XElement(binProp.Item1, new StreamReader(binProp.Item2).ReadToEnd()));
                    });*/

            return document.ToString();
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
            get { return "XML_IO"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string, Type>(); }
        }

        public IPluginable InitializePlugin(Dictionary<string, object> myParameters, GraphApplicationSettings myApplicationSetting)
        {
            return null;
        }

        public IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            throw new NotImplementedException();
        }

        #endregion
        
    }
}
