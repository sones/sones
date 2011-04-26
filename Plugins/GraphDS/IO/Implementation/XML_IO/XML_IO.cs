using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.IO;
using System.Xml.Schema;
using sones.GraphQL.Result;
using sones.Library.Settings;
using sones.Library.VersionedPluginManager;
using SchemaToClassesGenerator;
using System.Xml;
using sones.Plugins.GraphDS.IOInterface.XML_IO.Result;


namespace sones.Plugins.GraphDS.IOInterface.XML_IO
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
            var result = new SchemaToClassesGenerator.Result();

            result.Query = new Query() {Language = myQueryResult.NameOfQuerylanguage, Value = myQueryResult.Query};
            result.Number = myQueryResult.NumberOfAffectedVertices;

            if (myQueryResult.Error != null)
            {
                result.Error = myQueryResult.Error.Message;
            }
            else
            {
                result.Error = "";                    
            }

            result.Duration = myQueryResult.Duration;

            List<SchemaVertexView> vertices = new List<SchemaVertexView>();

            foreach (var aVertex in myQueryResult)
            {
                vertices.Add(GenerateVertexView(aVertex));
            }

            result.Vertices = vertices.ToArray();

            var stream = new MemoryStream();

            var writer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            writer.Serialize(stream, result);

            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }

        private SchemaVertexView GenerateVertexView(IVertexView aVertex)
        {
            var resultVertex = new SchemaVertexView();

            #region properties

            List<Property> properties = new List<Property>();

            foreach (var aProperty in aVertex.GetAllProperties())
            {
                var property = new Property();

                property.ID = aProperty.Item1;
                property.Value = aProperty.Item2.ToString();
                property.Type = aProperty.Item2.GetType().Name;

                properties.Add(property);
            }

            resultVertex.Properties = properties.ToArray();

            #endregion

            #region binaries

            List<BinaryData> binProperties = new List<BinaryData>();
            
            foreach (var aProperty in aVertex.GetAllBinaryProperties())
            {
                var binProp = new BinaryData();

                binProp.ID = aProperty.Item1;
                var content = new byte[aProperty.Item2.Length];
                aProperty.Item2.Write(content, 0, content.Length);

                binProp.Content = content;

                Array.Clear(content, 0 , content.Length);

                binProperties.Add(binProp);
            }

            resultVertex.BinaryProperties = binProperties.ToArray();
            
            #endregion

            #region edges

            List<EdgeTuple> edges = new List<EdgeTuple>();

            foreach (var aEdge in aVertex.GetAllEdges())
            {
                EdgeTuple et = new EdgeTuple();
                
                et.Name = aEdge.Item1;

                #region target vertices

                List<SchemaVertexView> innerVertices = new List<SchemaVertexView>();
                
                foreach (var aTargetVertex in aEdge.Item2.GetTargetVertices())
                {
                    innerVertices.Add(GenerateVertexView(aTargetVertex));
                }
                
                et.Edge = new SchemaEdgeView();
                et.Edge.TargetVertices = innerVertices.ToArray();

                #endregion

                #region properties
                
                List<Property> edgeProps = new List<Property>();

                foreach (var aProperty in aEdge.Item2.GetAllProperties())
                {
                    var prop = new Property();

                    prop.ID = aProperty.Item1;
                    prop.Type = aProperty.Item2.GetType().Name;
                    prop.Value = aProperty.Item2.ToString();

                    edgeProps.Add(prop);
                }

                et.Edge.Properties = edgeProps.ToArray();

                #endregion

                edges.Add(et);
                
            }

            resultVertex.Edges = edges.ToArray();

            #endregion

            return resultVertex;
        }

        public QueryResult GenerateQueryResult(string myResult)
        {
            /*var xmlSettings = new XmlReaderSettings();
            xmlSettings.Schemas.Add("http://sones.com/QueryResultSchema.xsd", @"X:\Experimental\IGraphFSReDesign\Plugins\GraphDS\IO\Implementation\XML_IO\QueryResultSchema.xsd");
            xmlSettings.ValidationType = ValidationType.Schema;

            XmlReader xmlReader = XmlReader.Create(myResult, xmlSettings);*/

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(myResult);

            var evHandler = new ValidationEventHandler(ValidationEventHandler);

            //xmlDocument.Validate(evHandler);
            
            var rootNode = xmlDocument.FirstChild.NextSibling;

            String query = String.Empty;
            String language = String.Empty;
            String error = String.Empty;
            UInt64 duration = 0;
            Int64  nrOfVertices = 0;
            List<VertexView> vertices = null;

            var nextNode = rootNode.FirstChild;

            while (nextNode != null)
            {
                if (nextNode.Attributes.Count > 0)
                {
                    query = nextNode.Attributes[0].Value;
                    language = nextNode.Attributes[1].Value;
                }
                else
                {
                    if (nextNode.HasChildNodes)
                    {
                        switch (nextNode.Name)
                        {
                            case "Duration":
                                duration = System.Convert.ToUInt64(nextNode.FirstChild.Value);
                                break;

                            case "Number":
                                nrOfVertices = System.Convert.ToInt64(nextNode.FirstChild.Value);
                                break;

                            case "Error":
                                error = nextNode.FirstChild.Value;
                                break;

                            case "Vertices":
                                vertices = ParseVertices(nextNode);
                                break;
                        }
                    }
                }

                nextNode = nextNode.NextSibling;
            }

            return new QueryResult(query, language, duration, vertices);
        }

        public ContentType ContentType
        {
            get { return _contentType; }
        }

        #endregion

        private void ValidationEventHandler(object sender, ValidationEventArgs eventArgs)
        {
            if (eventArgs.Severity == XmlSeverityType.Error)
            {
                throw new Exception("");
            }
        }

        private Dictionary<String, Object> ParseVertexProperties(XmlNode myVertexNode)
        {
            var result = new Dictionary<String, Object>();

            var property = myVertexNode.FirstChild;

            while (property != null)
            {
                String key = String.Empty;
                Object value = null;
                String type = String.Empty;                

                var propElement = property.FirstChild;

                while (propElement != null)
                {
                    if (propElement.HasChildNodes)
                    {
                        switch (propElement.Name)
                        {
                            case "ID":
                                key = propElement.InnerText;                                    
                                break;

                            case "Type":
                                type = propElement.InnerText;
                                break;

                            case "Value":
                                switch (type)
                                {
                                    case "String":
                                        value = propElement.InnerText;
                                        break;
                                    case "Int32":
                                        value = System.Convert.ToInt32(propElement.InnerText);
                                        break;
                                    case "Int64":
                                        value = System.Convert.ToInt64(propElement.InnerText);
                                        break;
                                    case "UInt32":
                                        value = System.Convert.ToUInt32(propElement.InnerText);
                                        break;
                                    case "UInt64":
                                        value = System.Convert.ToUInt64(propElement.InnerText);
                                        break;
                                }
                                break;
                        }
                    }
                    propElement = propElement.NextSibling;                    
                }

                result.Add(key, value);
                property = property.NextSibling;
            }

            return result;
        }

        private Dictionary<String, Stream> ParseBinaryVertex(XmlNode myVertexNode)
        {
            var result = new Dictionary<String, Stream>();

            var binProp = myVertexNode.FirstChild;

            while (binProp != null)
            {
                if (binProp.HasChildNodes)
                { 
                    var binPropElement = binProp.FirstChild;

                    String name = String.Empty;
                    Stream contentStream = null;
                    
                    while (binPropElement != null)
                    {
                        if (binPropElement.HasChildNodes)
                        {
                            switch (binPropElement.Name)
                            {
                                case "ID":
                                    name = binPropElement.InnerText;
                                    break;

                                case "Content":
                                    contentStream = new MemoryStream();
                                    var buf = System.Text.Encoding.UTF8.GetBytes(binPropElement.InnerText);

                                    contentStream.Write(buf, 0, buf.Length);                                    
                                    break;
                            }                            
                        }                        
                        binPropElement = binPropElement.NextSibling;
                    }
                    result.Add(name, contentStream);
                }
                
                binProp = binProp.NextSibling;
            }

            return result;
        }

        private void ParseEdgeProperties(XmlNode myEdgeProp, Dictionary<String, Object> myEdgeProperties, List<VertexView> myTargetVertices)
        {            
            Int32 cnt = 0;
            var property = myEdgeProp.FirstChild;

            while(property != null)
            {
                if (property.HasChildNodes)
                {
                    switch (property.Name)
                    {
                        case "CountOfProperties":
                            cnt = System.Convert.ToInt32(property.InnerText);
                            break;

                        case "Properties":
                            myEdgeProperties = ParseVertexProperties(property);
                            break;

                        case "TargetVertices" :
                            myTargetVertices.AddRange(ParseVertices(property));
                            break;
                    }
                }
                property = property.NextSibling;
            }            
        }

        private Dictionary<String, IEdgeView> ParseEdges(XmlNode myEdge)
        {
            var result = new Dictionary<String, IEdgeView>();
            var edge = myEdge.FirstChild;

            if (edge.HasChildNodes)
            {
                while (edge != null)
                {
                    if (edge.HasChildNodes)
                    {
                        var edgeItems = edge.FirstChild;

                        var name = String.Empty;
                        IEdgeView edgeView = null;

                        while (edgeItems != null)
                        {
                            switch (edgeItems.Name)
                            { 
                                case "Name" :
                                    name = edgeItems.InnerText;
                                    break;
                                
                                case "Edge":
                                    var edgeProps = new Dictionary<String, Object>();
                                    var targetVertices = new List<VertexView>();

                                    ParseEdgeProperties(edgeItems, edgeProps, targetVertices);

                                    edgeView = new EdgeView(edgeProps, targetVertices);
                                    break;
                            }

                            edgeItems = edgeItems.NextSibling;
                        }
                        
                        result.Add(name, edgeView);
                    }

                    edge = edge.NextSibling;
                }
            }

            return result;
        }

        private List<VertexView> ParseVertices(XmlNode myVerticeList)
        {
            var result = new List<VertexView>();

            var vertex = myVerticeList.FirstChild;
            Dictionary<String, Object> propList = null;
            Dictionary<String, IEdgeView> edges = null;
            Dictionary<String, Stream> binaryProperties = null;            

            if(vertex.HasChildNodes)
            {
                while (vertex != null)
                {
                    var items = vertex.FirstChild;

                    while (items != null)
                    {
                        if (items.HasChildNodes)
                        {
                            switch (items.Name)
                            {
                                case "Properties":
                                    propList = ParseVertexProperties(items);
                                    break;

                                case "BinaryProperties":
                                    binaryProperties = ParseBinaryVertex(items);

                                    if (propList != null)
                                    {
                                        foreach (var item in binaryProperties)
                                        {
                                            if (propList.ContainsKey(item.Key))
                                            {
                                                propList[item.Key] = item.Value;
                                            }
                                            else
                                            {
                                                propList.Add(item.Key, item.Value);
                                            }
                                        }
                                    }

                                    break;

                                case "Edges" :
                                    edges = ParseEdges(items);
                                    break;
                            }
                        }

                        items = items.NextSibling;
                    }

                    result.Add(new VertexView(propList, edges));
                    vertex = vertex.NextSibling;
                }                
            }            

            return result;
        }

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
