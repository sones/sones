using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.IO;
using System.Xml.Schema;
using System.Xml;
using sones.GraphQL.Result;
using sones.Library.VersionedPluginManager;
using SchemaToClassesGenerator;
using sones.Plugins.GraphDS.IO.XML_IO.ErrorHandling;
using System.Text;


namespace sones.Plugins.GraphDS.IO.XML_IO
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

            result.Version = IOInterfaceCompatibility.MaxVersion.ToString();

            result.Query = new Query() { Duration = myQueryResult.Duration, ResultType = Enum.GetName(typeof(ResultType), myQueryResult.TypeOfResult), Language = myQueryResult.NameOfQuerylanguage, Value = myQueryResult.Query, VerticesCount = myQueryResult.Vertices.LongCount(), Error = myQueryResult.Error == null ? null : HandleQueryExceptions(myQueryResult) };
                     
            List<SchemaVertexView> vertices = new List<SchemaVertexView>();

            foreach (var aVertex in myQueryResult)
            {
                vertices.Add(GenerateVertexView(aVertex));
            }

            result.VertexView = vertices.ToArray();            
            
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

                if (aProperty.Item2 != null)
                {
                    property.Value = aProperty.Item2.ToString();
                    property.Type = aProperty.Item2.GetType().Name;
                }
                else
                {
                    property.Value = String.Empty;
                    property.Type = "null";
                }                

                properties.Add(property);
            }

            resultVertex.Property = properties.ToArray();

            #endregion

            #region binaries

            List<BinaryData> binProperties = new List<BinaryData>();
            
            foreach (var aProperty in aVertex.GetAllBinaryProperties())
            {
                var binProp = new BinaryData();

                binProp.ID = aProperty.Item1;
                var content = new byte[aProperty.Item2.Length];
                aProperty.Item2.Read(content, 0, content.Length);
                
                binProp.Content = content;               

                binProperties.Add(binProp);
            }

            resultVertex.BinaryProperty = binProperties.ToArray();
            
            #endregion

            #region edges

            List<SchemaHyperEdgeView> edges = new List<SchemaHyperEdgeView>();
                        
            foreach (var aEdge in aVertex.GetAllEdges())
            {
                
                if (aEdge.Item2 is IHyperEdgeView)
                {

                    List<Tuple<SchemaVertexView, IEnumerable<Tuple<String, Object>>>> innerVertices = new List<Tuple<SchemaVertexView, IEnumerable<Tuple<String, Object>>>>();

                    #region single edges

                    foreach (var singleEdge in ((HyperEdgeView)aEdge.Item2).GetAllEdges())
                    {
                        innerVertices.Add(new Tuple<SchemaVertexView, IEnumerable<Tuple<String, Object>>>(GenerateVertexView(singleEdge.GetTargetVertex()), singleEdge.GetAllProperties()));
                    }

                    #endregion

                    var hyperEdge = new SchemaHyperEdgeView();

                    hyperEdge.Name = aEdge.Item1;

                    #region set hyperedge properties

                    var edgeProperties = aEdge.Item2.GetAllProperties().ToArray();
                    hyperEdge.CountOfProperties = edgeProperties.Count();
                    hyperEdge.Property = new Property[edgeProperties.Count()];

                    for (Int32 i = 0; i < edgeProperties.Count(); i++)
                    {
                        hyperEdge.Property[i] = new Property();
                        hyperEdge.Property[i].ID = edgeProperties[i].Item1;
                        hyperEdge.Property[i].Type = edgeProperties[i].Item2.GetType().Name;
                        hyperEdge.Property[i].Value = edgeProperties[i].Item2.ToString();
                    }

                    #endregion

                    hyperEdge.SingleEdge = new SchemaSingleEdgeView[innerVertices.Count];

                    for (Int32 i = 0; i < innerVertices.Count; i++)
                    {
                        hyperEdge.SingleEdge[i] = new SchemaSingleEdgeView();
                        var singleEdgeProperties = innerVertices[i].Item2.ToArray();

                        hyperEdge.SingleEdge[i].CountOfProperties = singleEdgeProperties.Count();
                        hyperEdge.SingleEdge[i].EdgeProperty = new Property[hyperEdge.SingleEdge[i].CountOfProperties];

                        #region single edge properties

                        for (Int32 j = 0; j < singleEdgeProperties.Count(); j++)
                        {
                            hyperEdge.SingleEdge[i].EdgeProperty[j] = new Property();
                            hyperEdge.SingleEdge[i].EdgeProperty[j].ID = singleEdgeProperties[j].Item1;
                            hyperEdge.SingleEdge[i].EdgeProperty[j].Type = singleEdgeProperties[j].Item2.GetType().Name;
                            hyperEdge.SingleEdge[i].EdgeProperty[j].Value = singleEdgeProperties[j].Item2.ToString();
                        }

                        #endregion

                        #region target vertex

                        hyperEdge.SingleEdge[i].TargetVertex = new SchemaVertexView();
                        hyperEdge.SingleEdge[i].TargetVertex.Property = innerVertices[i].Item1.Property.ToArray();
                        hyperEdge.SingleEdge[i].TargetVertex.BinaryProperty = innerVertices[i].Item1.BinaryProperty.ToArray();
                        hyperEdge.SingleEdge[i].TargetVertex.Edge = innerVertices[i].Item1.Edge.ToArray();

                        #endregion
                    }

                    edges.Add(hyperEdge);

                }
                else
                {
                    var singleEdge = new SchemaHyperEdgeView();
                    
                    singleEdge.Name = aEdge.Item1;

                    var edgeProperties = aEdge.Item2.GetAllProperties().ToArray();
                    singleEdge.CountOfProperties = edgeProperties.Count();

                    #region properties

                    singleEdge.Property = new Property[edgeProperties.Count()];

                    for (Int32 i = 0; i < edgeProperties.Count(); i++)
                    {
                        singleEdge.Property[i] = new Property();
                        singleEdge.Property[i].ID = edgeProperties[i].Item1;
                        singleEdge.Property[i].Type = edgeProperties[i].Item2.GetType().Name;
                        singleEdge.Property[i].Value = edgeProperties[i].Item2.ToString();
                    }

                    #endregion

                    #region target vertex

                    singleEdge.SingleEdge = new SchemaSingleEdgeView[1];

                    singleEdge.SingleEdge[0] = new SchemaSingleEdgeView();
                    singleEdge.SingleEdge[0].TargetVertex = new SchemaVertexView();

                    var targetVertex = GenerateVertexView(((SingleEdgeView)aEdge.Item2).GetTargetVertex());

                    singleEdge.SingleEdge[0].TargetVertex.Property = targetVertex.Property.ToArray();
                    singleEdge.SingleEdge[0].TargetVertex.BinaryProperty = targetVertex.BinaryProperty.ToArray();
                    singleEdge.SingleEdge[0].TargetVertex.Edge = targetVertex.Edge.ToArray();

                    #endregion

                    edges.Add(singleEdge);

                }

                #endregion                
                
            }

            resultVertex.Edge = edges.ToArray();

            #endregion

            return resultVertex;
        }

        public QueryResult GenerateQueryResult(string myResult)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(myResult);
            
            var evHandler = new ValidationEventHandler(ValidationEventHandler);
            xmlDocument.Schemas.Add(XmlSchema.Read(typeof(XML_IO).Assembly.GetManifestResourceStream("sones.Plugins.GraphDS.IOInterface.XML_IO.QueryResultSchema.xsd"), evHandler));                      

            xmlDocument.Validate(evHandler);
           
            var rootNode = xmlDocument.FirstChild.NextSibling;

            String version = String.Empty;

            if (rootNode.Attributes.Count > 0)
            {
                for (int i = 0; i < rootNode.Attributes.Count; i++)
                {
                    if (rootNode.Attributes[i].Name == "Version")
                    {
                        version = rootNode.Attributes[i].Value;
                        break;
                    }
                }

                if (version != IOInterfaceCompatibility.MaxVersion.ToString())
                {
                    throw new XmlVersionException(String.Format("The xml version is not compatible with the version {0}.", IOInterfaceCompatibility.MaxVersion.ToString()));
                }
            }

            String query = String.Empty;
            String language = String.Empty;
            String error = String.Empty;
            ResultType result = ResultType.Failed;
            UInt64 duration = 0;
            Int64  nrOfVertices = 0;
            List<VertexView> vertices = new List<VertexView>();

            var nextNode = rootNode.FirstChild;

            while (nextNode != null)
            {
                for(Int32 i = 0; i< nextNode.Attributes.Count; i++)
                {
                    switch(nextNode.Attributes[i].Name)
                    {
                        case "Value":
                            query = nextNode.Attributes[i].Value;
                            break;
                        case "Language":
                            language = nextNode.Attributes[i].Value;
                            break;
                        case "Duration":
                            duration = System.Convert.ToUInt64(nextNode.Attributes[i].Value);
                            break;
                        case "VerticesCount":
                            nrOfVertices = System.Convert.ToInt64(nextNode.Attributes[i].Value);
                            break;
                        case "Error":
                            error = nextNode.Attributes[i].Value;
                            break;
                        case "ResultType":
                            ResultType resType = ResultType.Failed;

                            if (Enum.TryParse(nextNode.Attributes[i].Value, true, out resType))
                            {
                                result = resType;
                            }
                            break;

                    }
                }

                if (nextNode.Name == "VertexView")
                {
                    vertices.Add(ParseVertex(nextNode));
                }

                nextNode = nextNode.NextSibling;
            }

            return new QueryResult(query, language, duration, ResultType.Successful, vertices, error != String.Empty ? new QueryException(error) : null);
        }

        public ContentType ContentType
        {
            get { return _contentType; }
        }

        #region private helpers

        private String HandleQueryExceptions(QueryResult queryresult)
        {
            StringBuilder SB = new StringBuilder();

            SB.Append(queryresult.Error.ToString());
            if (queryresult.Error.InnerException != null)
                SB.Append(" InnerException: " + queryresult.Error.InnerException.Message);

            return SB.ToString();
        }

        private void ValidationEventHandler(object sender, ValidationEventArgs eventArgs)
        {
            if (eventArgs.Severity == XmlSeverityType.Error)
            {
                throw new XmlValidationException(String.Format("Could not validate xml reason: {0}", eventArgs.Message));
            }
        }

        private Tuple<String, Object> ParseProperties(XmlNode myVertexNode)
        {
            var property = myVertexNode.FirstChild;
                        
            String key = String.Empty;
            Object value = null;
            String type = String.Empty;

            while (property != null)
            {
                if (property.HasChildNodes)
                {
                    switch (property.Name)
                    {
                        case "ID":
                            key = property.InnerText;
                            break;

                        case "Type":
                            type = property.InnerText;
                            break;

                        case "Value":
                            switch (type)
                            {
                                case "String":
                                    value = property.InnerText;
                                    break;
                                case "Int32":
                                    value = System.Convert.ToInt32(property.InnerText);
                                    break;
                                case "Int64":
                                    value = System.Convert.ToInt64(property.InnerText);
                                    break;
                                case "UInt32":
                                    value = System.Convert.ToUInt32(property.InnerText);
                                    break;
                                case "UInt64":
                                    value = System.Convert.ToUInt64(property.InnerText);
                                    break;
                            }
                            break;
                    }
                }

                property = property.NextSibling;
            }

            return new Tuple<string, object>(key, value);
        }

        private Tuple<String, Stream> ParseBinaryVertex(XmlNode myVertexNode)
        {
            var binProp = myVertexNode.FirstChild;
            String name = String.Empty;
            Stream contentStream = null;

            while (binProp != null)
            {
                if (binProp.HasChildNodes)
                {                    
                    switch (binProp.Name)
                    {
                        case "ID":
                            name = binProp.InnerText;
                            break;

                        case "Content":
                            contentStream = new MemoryStream();
                            string str = binProp.InnerText;

                            //convert string (hex) to MemoryStream 
                            for (int i = 0; i < str.Length; i += 2)
                            {
                                string sub = str.Substring(i, 2);
                                byte b = byte.Parse(sub, System.Globalization.NumberStyles.HexNumber);
                                contentStream.WriteByte(b);
                            }
                            contentStream.Position = 0;
                            break;

                    }                    
                }
                
                binProp = binProp.NextSibling;
            }

            return new Tuple<string,Stream>(name, contentStream);
        }

        private ISingleEdgeView ParseSingleEdge(XmlNode mySingleEdges)
        {            
            var edgeProperties = new Dictionary<String, Object>();
            IVertexView target = null;            

            if (mySingleEdges.HasChildNodes)
            {
                var edgeItems = mySingleEdges.FirstChild;

                while (edgeItems != null)
                {
                    switch (edgeItems.Name)
                    { 
                        case "EdgeProperty":
                            var edgeProp = ParseProperties(edgeItems);
                            edgeProperties.Add(edgeProp.Item1, edgeProp.Item2);
                            break;
                            
                        case "TargetVertex":
                            target = ParseVertex(edgeItems);
                            break;
                    }

                    edgeItems = edgeItems.NextSibling;
                }
            }
                
            return new SingleEdgeView(new Dictionary<String, Object>(edgeProperties), target);            
        }

        private void ParseEdgeProperties(XmlNode myEdgeProp, ref Dictionary<String, Object> myEdgeProperties)
        { 
            while(myEdgeProp != null)
            {
                if (myEdgeProp.HasChildNodes)
                {
                    switch (myEdgeProp.Name)
                    {
                        case "Property":
                            var props = ParseProperties(myEdgeProp);
                            myEdgeProperties.Add(props.Item1, props.Item2);
                            break;                        
                    }
                }

                myEdgeProp = myEdgeProp.NextSibling;
            }            
        }

        private Tuple<String, IEdgeView> ParseEdge(XmlNode myEdge)
        {                      
            IEdgeView edgeView = null;
            var edgeProps = new Dictionary<String, Object>();
            var singleEdges = new List<ISingleEdgeView>();
            var edge = myEdge.FirstChild;
            var name = String.Empty;     


            if (edge.HasChildNodes)
            {
                while (edge != null)
                {
                    if (edge.HasChildNodes)
                    {                        
                        switch (edge.Name)
                        { 
                            case "Name" :
                                name = edge.InnerText;
                                break;
                            
                            case "Property":
                                ParseEdgeProperties(edge, ref edgeProps);
                                break;

                            case "SingleEdge" :
                                singleEdges.Add(ParseSingleEdge(edge));
                                break;
                        }
                        
                    }

                    edge = edge.NextSibling;
                }
            }

            if (singleEdges.Count > 1)
            {
                edgeView = new HyperEdgeView(edgeProps, singleEdges);
            }
            else
            {
                edgeView = singleEdges.First();
            }

            return new Tuple<string, IEdgeView>(name, edgeView);
        }

        private VertexView ParseVertex(XmlNode myVertex)
        {                        
            Dictionary<String, Object> propList = new Dictionary<string,object>();
            Dictionary<String, IEdgeView> edges = new Dictionary<string, IEdgeView>();
            Dictionary<String, Stream> binaryProperties = new Dictionary<string,Stream>();            

            if(myVertex.HasChildNodes)
            {               
               var items = myVertex.FirstChild;

                while (items != null)
                {
                    if (items.HasChildNodes)
                    {
                        switch (items.Name)
                        {
                            case "Property":
                                var prop = ParseProperties(items);
                                propList.Add(prop.Item1, prop.Item2);
                                break;

                            case "BinaryProperty":
                                var binProp = ParseBinaryVertex(items);
                                binaryProperties.Add(binProp.Item1, binProp.Item2);
                                    
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

                                break;

                            case "Edge" :
                                var edge = ParseEdge(items);
                                edges.Add(edge.Item1, edge.Item2);
                                break;
                        }
                    }

                    items = items.NextSibling;
                }               
            }            

            return new VertexView(propList, edges);
        }

        #endregion

        #region IPluginable

        public string PluginName
        {
            get { return "sones.xml_io"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string, Type>(); }
        }

        public IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {

            var result = new XML_IO();

            return (IPluginable)result;
        }

        #endregion
        
    }
}
