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
using sones.XML_IO.Result;


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

      /*  #region AppendVertices
        
        private static void AppendVertices(List<VertexViewEdges> myVertices, List<VertexView> myResultVertices)
        {
            Boolean appendNewResult = true;
            
            if (myVertices.Count == 0)
            {
                return;
            }

            var currentLevel = myVertices.LastOrDefault().Vertex;

            var currentResult = myResultVertices.LastOrDefault();

            if (currentResult.Edges != null)
            {
                if (currentResult.Edges.Count() > 0)
                {
                    var lastEdge = currentResult.Edges.Last(item => item != null);
                    
                    if (lastEdge == null)
                    {
                        currentResult.Edges[currentResult.Edges.Length - 1] = new EdgeTuple();
                    }
                    else
                    {
                        for (Int32 i = 0; i < lastEdge.Edge.TargetVertices.Length; i++)
                        {
                            if (lastEdge.Edge.TargetVertices[i] == null)
                            {
                                currentResult = lastEdge.Edge.TargetVertices[i] = new VertexView();
                                appendNewResult = false;
                                break;
                            }
                        }
                    }
                }
            }

            if (currentResult.Properties == null)
            {
                var vertexProperties = currentLevel.GetAllProperties();
                currentResult.Properties = new Properties[vertexProperties.Count()];

                for (Int32 cnt = 0; cnt < vertexProperties.Count(); cnt++)
                {
                    currentResult.Properties[cnt] = new Properties();
                    currentResult.Properties[cnt].ID = vertexProperties.ElementAt(cnt).Item1;
                    currentResult.Properties[cnt].Type = vertexProperties.ElementAt(cnt).Item2.GetType().ToString();
                    currentResult.Properties[cnt].Value = vertexProperties.ElementAt(cnt).Item2.ToString();
                }

            }

            if (currentResult.BinaryPropertys == null)
            {
                var binaryProps = currentLevel.GetAllBinaryProperties();

                currentResult.BinaryPropertys = new BinaryData[binaryProps.Count()];

                for (Int32 cnt = 0; cnt < binaryProps.Count(); cnt++)
                {
                    currentResult.BinaryPropertys[cnt] = new BinaryData();
                    currentResult.BinaryPropertys[cnt].ID = binaryProps.ElementAt(cnt).Item1;
                    var buffer = new byte[binaryProps.ElementAt(cnt).Item2.Length];
                    binaryProps.ElementAt(cnt).Item2.Read(buffer, 0, buffer.Length);

                    currentResult.BinaryPropertys[cnt].Content = buffer;

                    Array.Clear(buffer, 0, buffer.Length);
                }
            }

            var edges = currentLevel.GetAllEdges();

            if (currentResult.Edges == null)
            {
                currentResult.Edges = new EdgeTuple[edges.Count()];
            }

            for (Int32 cnt = myVertices.LastOrDefault().EdgeIndex; cnt < edges.Count(); cnt++)
            {
                currentResult.Edges[cnt] = new EdgeTuple();
                currentResult.Edges[cnt].Edge = new EdgeView();

                currentResult.Edges[cnt].Edge.CountOfProperties = edges.ElementAt(cnt).Item2.GetCountOfProperties();

                var countOfProperties = edges.ElementAt(cnt).Item2.GetCountOfProperties();
                var properties = edges.ElementAt(cnt).Item2.GetAllProperties();
                
                currentResult.Edges[cnt].Edge.Properties = new Properties[countOfProperties];

                for (Int32 propCnt = 0; propCnt < countOfProperties; propCnt++)
                {
                    currentResult.Edges[cnt].Edge.Properties[propCnt] = new Properties();

                    currentResult.Edges[cnt].Edge.Properties[propCnt].ID = properties.ElementAt(propCnt).Item1;
                    currentResult.Edges[cnt].Edge.Properties[propCnt].Type =
                        properties.ElementAt(propCnt).Item2.GetType().ToString();
                    currentResult.Edges[cnt].Edge.Properties[propCnt].Value =
                        properties.ElementAt(propCnt).Item2.ToString();
                }

                if(myVertices.Count > 0)
                {myVertices[myVertices.Count - 1].EdgeIndex++;}
                currentResult.Edges[cnt].Name = edges.ElementAt(cnt).Item1;
                currentResult.Edges[cnt].Edge.SourceVertex = new VertexView();

                currentResult.Edges[cnt].Edge.SourceVertex.BinaryPropertys = currentResult.BinaryPropertys;
                currentResult.Edges[cnt].Edge.SourceVertex.Edges = currentResult.Edges;
                currentResult.Edges[cnt].Edge.SourceVertex.Properties = currentResult.Properties;

                var targetVertices = edges.ElementAt(cnt).Item2.GetTargetVertices();

                currentResult.Edges[cnt].Edge.TargetVertices = new VertexView[targetVertices.Count()];

                for (Int32 i = 0; i < targetVertices.Count(); i++)
                {
                    myVertices.Add(new VertexViewEdges(targetVertices.ElementAt(i), 0));
                }

                AppendVertices(myVertices, myResultVertices);
            }

            myVertices.Remove(myVertices.LastOrDefault());

            if (myVertices.Count > 0 && appendNewResult)
            {
                myResultVertices.Add(new VertexView());
            }

            AppendVertices(myVertices, myResultVertices);
        }

        #endregion*/

        #region IOInterface

        public string GenerateOutputResult(QueryResult myQueryResult)
        {
            var result = new Result();

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

       /* public string GenerateOutputResult(QueryResult myQueryResult)
        {
            var result = new Result();

            result.Query = new Query() { Language = myQueryResult.NameOfQuerylanguage, Value = myQueryResult.Query };
            result.Number = myQueryResult.NumberOfAffectedVertices;

            if (myQueryResult.Error != null)
            {
                result.Error = myQueryResult.Error.Message;
            }

            result.Error = "";
            result.Duration = myQueryResult.Duration;

            var resultVertices = new List<VertexViewEdges>();
            var results = new List<VertexView>(){new VertexView()};

            foreach(var item in myQueryResult.Vertices)
            {                
                resultVertices.Add(new VertexViewEdges(item, 0));
            }
            
            AppendVertices(resultVertices, results);

            result.Vertices = results.ToArray();

            var stream = new MemoryStream();

            var writer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            writer.Serialize(stream, result);

            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }*/

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
            List<VertexView> vertices;

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

            return new QueryResult(query, language, duration);
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
                Int32 cnt = 0;

                var propElement = property.FirstChild;

                while (propElement != null)
                {
                    switch (propElement.ChildNodes[cnt].Name)
                    {
                            case "ID" :
                                key = propElement.Value;
                            break;

                            case "Type":
                                type = propElement.Value;
                            break;

                            case "Value":
                                switch (type)
                                {
                                    case "String" :
                                        value = propElement.Value;
                                        break;
                                    case "Int32" :
                                        value = System.Convert.ToInt32(propElement.Value);
                                        break;
                                    case "Int64" :
                                        value = System.Convert.ToInt64(propElement.Value);
                                        break;
                                    case "UInt32":
                                        value = System.Convert.ToUInt32(propElement.Value);
                                        break;
                                    case "UInt64":
                                        value = System.Convert.ToUInt64(propElement.Value);
                                        break;
                                }
                            break;
                    }

                    propElement = propElement.NextSibling;
                    cnt++;
                }

                result.Add(key, value);
                cnt =0;
                property = property.NextSibling;
            }

            return result;
        }

        private Dictionary<String, Stream> ParseBinaryVertex(XmlNode myVertexNode)
        {
            var result = new Dictionary<String, Stream>();
            
            //while()

            return result;
        }

        private List<VertexView> ParseVertices(XmlNode myVerticeList)
        {
            var result = new List<VertexView>();

            var element = myVerticeList.FirstChild;
            Dictionary<String, Object> propList = null;
            Dictionary<String, Stream> binaryProperties = null;
            Int32 cnt = 0;

            while (element != null)
            {
                if (element.HasChildNodes)
                {
                    switch (element.ChildNodes[cnt].Name)
                    {
                        case "Properties":
                            propList = ParseVertexProperties(element);
                            break;

                        case "BinaryProperties":
                            binaryProperties = ParseBinaryVertex(element);
                            break;
                    }
                }

                result.Add(new VertexView(propList, null));
                cnt++;
                element = element.NextSibling;
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
            return null;
        }

        public IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            throw new NotImplementedException();
        }

        #endregion
        
    }
}
