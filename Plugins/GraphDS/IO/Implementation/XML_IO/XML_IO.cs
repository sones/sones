using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using sones.GraphQL.Result;
using sones.Library.Settings;
using sones.Library.VersionedPluginManager;
using SchemaToClassesGenerator;
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

        #region AppendVertices
        
        private void AppendVertices(List<IVertexView> myEdges, List<VertexView> myResultVertices)
        {
            var currentLevel = myEdges.LastOrDefault();
            var currentResult = myResultVertices.LastOrDefault();

            if (myEdges.Count == 0 && myResultVertices.Count == 0)
            {
                return;
            }            
            
            currentResult.VertexID = currentLevel.VertexID;
            currentResult.VertexRevisionID.ID = currentLevel.VertexRevisionID.ID;
            currentResult.VertexRevisionID.Timestamp = currentLevel.VertexRevisionID.Timestamp;
            currentResult.VertexTypeName = currentLevel.VertexTypeName;
            currentResult.EditionName = currentLevel.EditionName;
            currentResult.BinaryPropertys = new BinaryData[currentLevel.GetAllBinaryProperties().Count()];

            for (Int32 cnt = 0; cnt < currentLevel.GetAllBinaryProperties().Count(); cnt++)
            {
                currentResult.BinaryPropertys.ElementAt(cnt).Name = currentLevel.GetAllBinaryProperties().ElementAt(cnt).Item1;
                byte[] buffer = new byte[currentLevel.GetAllBinaryProperties().ElementAt(cnt).Item2.Length];
                currentLevel.GetAllBinaryProperties().ElementAt(cnt).Item2.Read(buffer, 0, buffer.Length);

                currentResult.BinaryPropertys.ElementAt(cnt).Content = buffer;

                Array.Clear(buffer, 0, buffer.Length);
            }

            var edges = currentLevel.GetAllEdges();

            for (Int32 cnt = 0; cnt < edges.Count(); cnt++)
            {
                currentResult.Edges[cnt].Edge.Comment = edges.ElementAt(cnt).Item2.Comment;
                currentResult.Edges[cnt].Edge.CreationDate = edges.ElementAt(cnt).Item2.CreationDate;
                currentResult.Edges[cnt].Edge.EdgeTypeName = edges.ElementAt(cnt).Item2.EdgeTypeName;
                currentResult.Edges[cnt].Edge.ModificationDate = edges.ElementAt(cnt).Item2.ModificationDate;
                currentResult.Edges[cnt].Edge.CountOfProperties = edges.ElementAt(cnt).Item2.GetCountOfProperties();

                var countOfProperties = edges.ElementAt(cnt).Item2.GetCountOfProperties();
                var properties = edges.ElementAt(cnt).Item2.GetAllProperties();
                
                currentResult.Edges[cnt].Edge.Properties = new Properties[countOfProperties];

                for (Int32 propCnt = 0; propCnt < countOfProperties; propCnt++)
                {
                    currentResult.Edges[cnt].Edge.Properties[propCnt].Name = properties.ElementAt(propCnt).Item1;
                    currentResult.Edges[cnt].Edge.Properties[propCnt].Type =
                        properties.ElementAt(propCnt).Item2.GetType().ToString();
                    currentResult.Edges[cnt].Edge.Properties[propCnt].Value =
                        properties.ElementAt(propCnt).Item2.ToString();
                }

                currentResult.Edges[cnt].Name = edges.ElementAt(cnt).Item1;
                currentResult.Edges[cnt].Edge.EdgeTypeName = edges.ElementAt(cnt).Item2.EdgeTypeName;
                    
                myEdges.Add(edges.ElementAt(cnt).Item2.GetSourceVertex());
                myResultVertices.Add(currentResult.Edges[cnt].Edge.SourceVertex);
                    
                AppendVertices(myEdges, myResultVertices);

                myEdges.AddRange(edges.ElementAt(cnt).Item2.GetTargetVertices());
                myResultVertices.AddRange(currentResult.Edges[cnt].Edge.TargetVertices);

                AppendVertices(myEdges, myResultVertices);
            }               
            

            myEdges.Remove(currentLevel);
            myResultVertices.Remove(currentResult);

            AppendVertices(myEdges, myResultVertices);
        }

        #endregion

        #region IOInterface

        public string GenerateOutputResult(QueryResult myQueryResult)
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
            result.Vertices = new VertexView[myQueryResult.Vertices.Count()];

            AppendVertices(myQueryResult.Vertices.Reverse().ToList(), result.Vertices.ToList());

            var stream = new MemoryStream();

            var writer = new System.Xml.Serialization.XmlSerializer(result.GetType());
            writer.Serialize(stream, result);

            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
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
