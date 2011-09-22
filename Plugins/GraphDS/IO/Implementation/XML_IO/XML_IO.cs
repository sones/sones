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
using System.Net.Mime;
using System.IO;
using System.Xml.Schema;
using System.Xml;
using sones.GraphQL.Result;
using sones.Library.VersionedPluginManager;
using SchemaToClassesGenerator;
using sones.Plugins.GraphDS.IO.XML_IO.ErrorHandling;
using System.Text;
using System.Text.RegularExpressions;
using VertexView = sones.GraphQL.Result.VertexView;
using sones.Library.CollectionWrapper;


namespace sones.Plugins.GraphDS.IO.XML_IO
{
    /// <summary>
    /// This class realize an XML output.
    /// </summary>
    public sealed class XML_IO : IOInterface
    {

        #region Data

        /// <summary>
        /// The io content type.
        /// </summary>
        private readonly ContentType _contentType;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for a xml io instance.
        /// </summary>
        public XML_IO()
        {
            _contentType = new ContentType("application/xml") { CharSet = "UTF-8" };
        }

        #endregion

        #region IOInterface

        public string GenerateOutputResult(QueryResult myQueryResult, Dictionary<String, String> myParams)
        {
            var result = new SchemaToClassesGenerator.Result();

            result.Version = IOInterfaceCompatibility.MaxVersion.ToString();

            result.Query = new Query() { Duration = myQueryResult.Duration, ResultType = Enum.GetName(typeof(ResultType), myQueryResult.TypeOfResult), Language = myQueryResult.NameOfQuerylanguage, Value = myQueryResult.Query, VerticesCount = myQueryResult.Vertices.LongCount(), Error = myQueryResult.Error == null ? null : HandleQueryExceptions(myQueryResult) };

            List<SchemaToClassesGenerator.VertexView> vertices = new List<SchemaToClassesGenerator.VertexView>();

            foreach (var aVertex in myQueryResult)
            {
                vertices.Add(GenerateVertexView(aVertex));
            }

            result.VertexViews = vertices.ToArray();
            
            var stream = new MemoryStream();

            var writer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            writer.Serialize(stream, result);

            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }

        public String ListAvailParams()
        {
            throw new NotImplementedException();
        }

        #region private output result helpers
        
        /// <summary>
        /// Handles list properties.
        /// </summary>
        /// <param name="myItemProperty">The property of the item(edge, hyperedge, vertex).</param>
        /// <param name="myPropertyToFill">The schema property which is to fill.</param>
        private void HandleListProperties(ICollectionWrapper myItemProperty, ref Property myPropertyToFill)
        {
            Type propertyElementType = typeof(Object);
            
            foreach (var value in myItemProperty)
            {
                myPropertyToFill.Value += "[" + value.ToString() + "],";
                propertyElementType = value.GetType();
            }

            var index = -1;

            if (myPropertyToFill.Value != null)
            {
                index = myPropertyToFill.Value.LastIndexOf(',');
            }

            if (index > -1)
            {
                myPropertyToFill.Value = myPropertyToFill.Value.Remove(index, 1);
            }

            myPropertyToFill.Type = myItemProperty.GetType().Name + "(" + propertyElementType.Name + ")";            
        }

        /// <summary>
        /// Generates a xml vertex view class.
        /// </summary>
        /// <param name="aVertex">The vertex view of the query result.</param>
        /// <returns>An generated xml class.</returns>
        private SchemaToClassesGenerator.VertexView GenerateVertexView(IVertexView aVertex)
        {
            var resultVertex = new SchemaToClassesGenerator.VertexView();

            if (aVertex != null)
            {
                #region properties

                List<Property> properties = new List<Property>();

                foreach (var aProperty in aVertex.GetAllProperties())
                {
                    var property = new Property();

                    property.ID = aProperty.Item1;

                    if (aProperty.Item2 != null)
                    {
                        if (aProperty.Item2 is ICollectionWrapper)
                        {
                            HandleListProperties((ICollectionWrapper)aProperty.Item2, ref property);
                        }
                        else
                        {
                            property.Value = aProperty.Item2.ToString();
                            property.Type = aProperty.Item2.GetType().Name;
                        }
                        
                    }
                    else
                    {
                        property.Value = String.Empty;
                        property.Type = "null";
                    }

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
                    aProperty.Item2.Read(content, 0, content.Length);

                    binProp.Content = content;

                    binProperties.Add(binProp);
                }

                resultVertex.BinaryProperties = binProperties.ToArray();

                #endregion

                #region edges

                List<SchemaToClassesGenerator.EdgeView> edges = new List<SchemaToClassesGenerator.EdgeView>();

                foreach (var aEdge in aVertex.GetAllEdges())
                {
                    if (aEdge.Item2 is IHyperEdgeView)
                    {
                        List<Tuple<SchemaToClassesGenerator.VertexView, IEnumerable<Tuple<String, Object>>>> innerVertices = new List<Tuple<SchemaToClassesGenerator.VertexView, IEnumerable<Tuple<String, Object>>>>();

                        #region single edges

                        foreach (var SingleEdges in ((sones.GraphQL.Result.HyperEdgeView)aEdge.Item2).GetAllEdges())
                        {
                            innerVertices.Add(new Tuple<SchemaToClassesGenerator.VertexView, IEnumerable<Tuple<String, Object>>>(GenerateVertexView(SingleEdges.GetTargetVertex()), SingleEdges.GetAllProperties()));
                        }

                        #endregion

                        var hyperEdge = new SchemaToClassesGenerator.HyperEdgeView();

                        hyperEdge.Name = aEdge.Item1;

                        #region set hyperedge properties

                        var edgeProperties = aEdge.Item2.GetAllProperties().ToArray();
                        hyperEdge.Properties = new Property[edgeProperties.Count()];

                        for (Int32 i = 0; i < edgeProperties.Count(); i++)
                        {
                            hyperEdge.Properties[i] = new Property();
                            hyperEdge.Properties[i].ID = edgeProperties[i].Item1;

                            if (edgeProperties[i].Item2 is ICollectionWrapper)
                            {
                                HandleListProperties((ICollectionWrapper)edgeProperties[i].Item2, ref hyperEdge.Properties[i]);
                            }
                            else
                            {
                                hyperEdge.Properties[i].Type = edgeProperties[i].Item2.GetType().Name;
                                hyperEdge.Properties[i].Value = edgeProperties[i].Item2.ToString();
                            }
                        }

                        #endregion

                        hyperEdge.SingleEdge = new SchemaToClassesGenerator.SingleEdgeView[innerVertices.Count];

                        for (Int32 i = 0; i < innerVertices.Count; i++)
                        {
                            hyperEdge.SingleEdge[i] = new SchemaToClassesGenerator.SingleEdgeView();
                            var SingleEdgesProperties = innerVertices[i].Item2.ToArray();
                            
                            hyperEdge.SingleEdge[i].Properties = new Property[SingleEdgesProperties.Count()];

                            #region single edge properties

                            for (Int32 j = 0; j < SingleEdgesProperties.Count(); j++)
                            {
                                hyperEdge.SingleEdge[i].Properties[j] = new Property();
                                hyperEdge.SingleEdge[i].Properties[j].ID = SingleEdgesProperties[j].Item1;

                                if (SingleEdgesProperties[j].Item2 is ICollectionWrapper)
                                {
                                    HandleListProperties((ICollectionWrapper)SingleEdgesProperties[j].Item2, ref hyperEdge.SingleEdge[i].Properties[j]);
                                }
                                else
                                {
                                    hyperEdge.SingleEdge[i].Properties[j].Type = SingleEdgesProperties[j].Item2.GetType().Name;
                                    hyperEdge.SingleEdge[i].Properties[j].Value = SingleEdgesProperties[j].Item2.ToString();
                                }
                            }

                            #endregion

                            #region target vertex

                            hyperEdge.SingleEdge[i].TargetVertex = new SchemaToClassesGenerator.VertexView();
                            
                            if (innerVertices[i].Item1.Properties != null)
                            {
                                hyperEdge.SingleEdge[i].TargetVertex.Properties = innerVertices[i].Item1.Properties.ToArray();
                            }

                            if (innerVertices[i].Item1.BinaryProperties != null)
                            {
                                hyperEdge.SingleEdge[i].TargetVertex.BinaryProperties = innerVertices[i].Item1.BinaryProperties.ToArray();
                            }

                            if (innerVertices[i].Item1.Edges != null)
                            {
                                hyperEdge.SingleEdge[i].TargetVertex.Edges = innerVertices[i].Item1.Edges.ToArray();
                            }

                            #endregion
                        }

                        edges.Add(hyperEdge);

                    }
                    else
                    {
                        var SingleEdges = new SchemaToClassesGenerator.SingleEdgeView();

                        SingleEdges.Name = aEdge.Item1;

                        var edgeProperties = aEdge.Item2.GetAllProperties().ToArray();

                        #region properties

                        SingleEdges.Properties = new Property[edgeProperties.Count()];

                        for (Int32 i = 0; i < edgeProperties.Count(); i++)
                        {
                            SingleEdges.Properties[i] = new Property();
                            SingleEdges.Properties[i].ID = edgeProperties[i].Item1;

                            if (edgeProperties[i].Item2 is ICollectionWrapper)
                            {
                                HandleListProperties((ICollectionWrapper)edgeProperties[i].Item2, ref SingleEdges.Properties[i]);
                            }
                            else
                            {
                                SingleEdges.Properties[i].Type = edgeProperties[i].Item2.GetType().Name;
                                SingleEdges.Properties[i].Value = edgeProperties[i].Item2.ToString();
                            }
                        }

                        #endregion

                        #region target vertex

                        SingleEdges.TargetVertex = new SchemaToClassesGenerator.VertexView();
                        
                        var edgeTargetVertex = ((sones.GraphQL.Result.SingleEdgeView)aEdge.Item2).GetTargetVertex();

                        var targetVertex = GenerateVertexView(edgeTargetVertex);

                        if (edgeTargetVertex != null)
                        {
                            SingleEdges.TargetVertex.Properties = targetVertex.Properties.ToArray();
                            SingleEdges.TargetVertex.BinaryProperties = targetVertex.BinaryProperties.ToArray();
                            SingleEdges.TargetVertex.Edges = targetVertex.Edges.ToArray();
                        }

                        #endregion

                        edges.Add(SingleEdges);

                    }

                #endregion

                }

                resultVertex.Edges = edges.ToArray();

                #endregion
            }
            return resultVertex;
        }

        #endregion

        public QueryResult GenerateQueryResult(string myResult)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(myResult);
            
            var evHandler = new ValidationEventHandler(ValidationEventHandler);
            xmlDocument.Schemas.Add(XmlSchema.Read(typeof(XML_IO).Assembly.GetManifestResourceStream("sones.Plugins.GraphDS.IOInterface.XML_IO.QueryResultSchema.xsd"), evHandler));                      

            // As long as this doesn't work under mono, we can't validate the incoming xml against the given schema
            // http://bugzilla.xamarin.com/show_bug.cgi?id=220
#if !__MonoCS__ 
            xmlDocument.Validate(evHandler);
#endif


            
           
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
            List<sones.GraphQL.Result.VertexView> vertices = new List<sones.GraphQL.Result.VertexView>();

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

                if (nextNode.Name == "VertexViews")
                {
                    var vertexItems = nextNode.FirstChild;

                    while (vertexItems != null)
                    {
                        vertices.Add(ParseVertex(vertexItems));
                        vertexItems = vertexItems.NextSibling;
                    }
                }

                nextNode = nextNode.NextSibling;
            }

            return new QueryResult(query, language, duration, ResultType.Successful, vertices, error != String.Empty ? new QueryException(error) : null);
        }

        public ContentType ContentType
        {
            get { return _contentType; }
        }

        #region private query result helpers

        /// <summary>
        /// Handles query exceptions.
        /// </summary>
        /// <param name="queryresult">The query result.</param>
        /// <returns>The exception string.</returns>
        private String HandleQueryExceptions(QueryResult queryresult)
        {
            StringBuilder SB = new StringBuilder();

            SB.Append(queryresult.Error.ToString());

            if (queryresult.Error.InnerException != null)
            {
                SB.Append(" InnerException: " + queryresult.Error.InnerException.Message);
            }

            return SB.ToString();
        }

        /// <summary>
        /// Handles validation exceptions.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">Validation event arguments.</param>
        private void ValidationEventHandler(object sender, ValidationEventArgs eventArgs)
        {
            if (eventArgs.Severity == XmlSeverityType.Error)
            {
                throw new XmlValidationException(String.Format("Could not validate xml reason: {0}", eventArgs.Message));
            }
        }

        /// <summary>
        /// Maps base types.
        /// </summary>
        /// <param name="myType">The type as string.</param>
        /// <param name="myValue">The value as string.</param>
        /// <returns>An object of the type.</returns>
        private Object TypeMapper(String myType, String myValue)
        {
            switch (myType)
            {
                case "String":
                    return myValue;
                case "Int16":
                    return System.Convert.ToInt16(myValue);
                case "Int32":
                    return System.Convert.ToInt32(myValue);
                case "Int64":
                    return System.Convert.ToInt64(myValue);
                case "UInt16":
                    return System.Convert.ToUInt16(myValue);
                case "UInt32":
                    return System.Convert.ToUInt32(myValue);                    
                case "UInt64":
                    return System.Convert.ToUInt64(myValue);
                case "Boolean":
                    return System.Convert.ToBoolean(myValue);                    
                case "DateTime":
                    return System.Convert.ToDateTime(myValue);                    
                case "Double":
                    return System.Convert.ToDouble(myValue);
                case "Decimal":
                    return System.Convert.ToDecimal(myValue);
                case "Single":
                    return System.Convert.ToSingle(myValue);
                case "SByte":
                    return System.Convert.ToSByte(myValue);
                
                default:
                    return myValue;
            }
        }

        /// <summary>
        /// Parsing properties.
        /// </summary>
        /// <param name="myVertexNode">The vertex node.</param>
        /// <returns>A tuple with the property name and the value.</returns>
        private Tuple<String, Object> ParseProperties(XmlNode myVertexNode)
        {
            var property = myVertexNode.FirstChild;
                        
            String key = String.Empty;
            Object value = null;
            String type = String.Empty;
            Boolean isCollectionSet = false;
            Boolean isCollectionList = false;

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

                            if (property.InnerText.Contains(typeof(ListCollectionWrapper).Name))
                            {
                                type = property.InnerText.Split('(', ')').ElementAt(1);
                                isCollectionList = true;
                            }
                            else if(property.InnerText.Contains(typeof(SetCollectionWrapper).Name))
                            {
                                type = property.InnerText.Split('(', ')').ElementAt(1);
                                isCollectionSet = true;
                            }
                            else
                            {
                                type = property.InnerText;
                            }
                            
                            break;

                        case "Value":

                            if (isCollectionList)
                            {
                                Regex regExp = new Regex(@"(?<=\[)(.*?)(?=\])");

                                var matches = regExp.Matches(property.InnerText);                                
                                value = new ListCollectionWrapper();

                                foreach (var item in matches)
                                {                                    
                                    ((ListCollectionWrapper)value).Add((IComparable)TypeMapper(type, item.ToString()));
                                }

                                isCollectionList = false;
                            }
                            else if(isCollectionSet)
                            {
                                Regex regExp = new Regex(@"\[(/?[^\]]+)\]");

                                var matches = regExp.Matches(property.InnerText);
                                value = new SetCollectionWrapper();

                                foreach (var item in matches)
                                {
                                    ((SetCollectionWrapper)value).Add((IComparable)TypeMapper(type, item.ToString()));
                                }
                                
                                isCollectionSet = false;
                            }
                            else
                            {
                                value = TypeMapper(type, property.InnerText);
                            }
                            break;
                    }
                }
                
                property = property.NextSibling;
            }

            if (value == null)
            {
                if (isCollectionList)
                {
                    value = new ListCollectionWrapper();
                }

                if (isCollectionSet)
                {
                    value = new SetCollectionWrapper();
                }
            }

            return new Tuple<string, object>(key, value);
        }

        /// <summary>
        /// Parsing binary properties.
        /// </summary>
        /// <param name="myVertexNode">The vertex node.</param>
        /// <returns>An tuple with property name and the property stream.</returns>
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

        /// <summary>
        /// Parsing an single edge.
        /// </summary>
        /// <param name="mySingleEdges">The single edge.</param>
        /// <returns>An single edge view.</returns>
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
                        case "Properties":
                            var prop = edgeItems.FirstChild;

                            while (prop != null)
                            {
                                var edgeProp = ParseProperties(prop);
                                edgeProperties.Add(edgeProp.Item1, edgeProp.Item2);
                                prop = prop.NextSibling;
                            }

                            break;
                            
                        case "TargetVertex":
                            target = ParseVertex(edgeItems);
                            break;
                    }

                    edgeItems = edgeItems.NextSibling;
                }
            }
                
            return new sones.GraphQL.Result.SingleEdgeView(new Dictionary<String, Object>(edgeProperties), target);            
        }

        /// <summary>
        /// Parsing edge properties.
        /// </summary>
        /// <param name="myEdgeProp">The edge property.</param>
        /// <param name="myEdgeProperties">An dictionary with the edge properties.</param>
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

        /// <summary>
        /// Parsing an edge.
        /// </summary>
        /// <param name="myEdge">The current XML node.</param>
        /// <returns>An tuple with the edge name and the edge view.</returns>
        private Tuple<String, IEdgeView> ParseEdge(XmlNode myEdge)
        {                      
            IEdgeView edgeView = null;
            var edgeProps = new Dictionary<String, Object>();
            var singleEdges = new List<ISingleEdgeView>();
            var edge = myEdge.FirstChild;
            var name = String.Empty;
            VertexView targetVertex = null; 
            Boolean isHyperEdge = false;

            isHyperEdge = myEdge.Attributes["xsi:type"].Value == "HyperEdgeView";
            

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
                            
                            case "Properties":
                                ParseEdgeProperties(edge.FirstChild, ref edgeProps);
                                break;

                            case "SingleEdge" :
                                singleEdges.Add(ParseSingleEdge(edge));
                                break;

                            case "TargetVertex":
                                targetVertex = ParseVertex(edge);
                                break;
                        }
                        
                    }

                    edge = edge.NextSibling;
                }
            }

            if (isHyperEdge)
            {
                edgeView = new sones.GraphQL.Result.HyperEdgeView(edgeProps, singleEdges);
            }
            else
            {
                edgeView = new sones.GraphQL.Result.SingleEdgeView(edgeProps, targetVertex);
            }

            return new Tuple<string, IEdgeView>(name, edgeView);
        }

        /// <summary>
        /// Parsing a vertex.
        /// </summary>
        /// <param name="myVertex">The current xml node.</param>
        /// <returns>An vertex view.</returns>
        private sones.GraphQL.Result.VertexView ParseVertex(XmlNode myVertex)
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
                            case "Properties":
                                
                                var propItems = items.FirstChild;

                                while (propItems != null)
                                {
                                    var prop = ParseProperties(propItems);
                                    propList.Add(prop.Item1, prop.Item2);
                                    propItems = propItems.NextSibling;
                                }
                                
                                break;

                            case "BinaryProperties":

                                var binPropItems = items.FirstChild;

                                while (binPropItems != null)
                                {
                                    var binProp = ParseBinaryVertex(binPropItems);
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
                                    
                                    binPropItems = binPropItems.NextSibling;
                                }

                                break;

                            case "Edges" :

                                var edgeItem = items.FirstChild;
                                
                                while (edgeItem != null)
                                {
                                    var edge = ParseEdge(edgeItem);
                                    
                                    edges.Add(edge.Item1, edge.Item2);
                                    edgeItem = edgeItem.NextSibling;
                                }
                                break;
                        }
                    }

                    items = items.NextSibling;
                }               
            }            

            return new sones.GraphQL.Result.VertexView(propList, edges.Count == 0 ? null : edges);
        }

        #endregion

        #region IPluginable

        public string PluginName
        {
            get { return "sones.xml_io"; }
        }

        public string PluginShortName
        {
            get { return "xml"; }
        }

        public string PluginDescription
        {
            get { return "This class realize an XML output."; }
        }
        
        public PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }

        public IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {

            var result = new XML_IO();

            return (IPluginable)result;
        }

        public void Dispose()
        { }

        #endregion
        
    }
}
