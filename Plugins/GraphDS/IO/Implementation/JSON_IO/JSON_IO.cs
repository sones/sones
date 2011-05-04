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
using Newtonsoft.Json.Linq;

namespace sones.Plugins.GraphDS.IO.JSON_IO
{
    public sealed class JSON_IO : IOInterface
    {
        #region Data

        private readonly ContentType _contentType;

        #endregion

        #region Constructors

        public JSON_IO()
        {
            _contentType = new ContentType("application/json") { CharSet = "UTF-8" };
        }

        #endregion

        #region IPluginable

        public string PluginName
        {
            get { return "sones.json_io"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string, Type>(); }
        }

        /* ASK: whats this?
        public IPluginable InitializePlugin(Dictionary<string, object> myParameters, GraphApplicationSettings myApplicationSetting)
        {
            return InitializePlugin();
        }
        */
        public IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            var result = new JSON_IO();

            return (IPluginable)result;
        }

        #endregion

        #region IOInterface

        #region Generate Output from Query Result
        public string GenerateOutputResult(QueryResult myQueryResult)
        {
            // root element...
            var _Query = new JObject();
            // query --------------------------------
            _Query.Add(new JProperty("query", myQueryResult.Query));
            // result -------------------------------
            _Query.Add(new JProperty("result", myQueryResult.TypeOfResult.ToString()));
            // duration -----------------------------
            _Query.Add(new JProperty("duration", new JArray(myQueryResult.Duration, "ms")));
            // warnings -----------------------------
            // currently not in 2.0
            //_Query.Add(new JProperty("warnings", new JArray(
            //    from _Warning in myQueryResult.Warnings
            //    select new JObject(
            //             new JProperty("code", _Warning.GetType().ToString()),
            //             new JProperty("description", _Warning.ToString())
            //           ))));
            // errors -------------------------------
            if (myQueryResult.Error != null)
            { 
                _Query.Add(new JProperty("errors", new JArray(
                             new JObject(
                             new JProperty("code", myQueryResult.Error.GetType().ToString()),
                             new JProperty("description", myQueryResult.Error.ToString())
                           ))));
            }
            // results ------------------------------
            JArray _resultsArray = new JArray();
            // fill the results array
            if (myQueryResult.Vertices != null)
            {
                foreach (IVertexView _vertex in myQueryResult.Vertices)
                {
                    _resultsArray.Add(GenererateVertexViewJSON(_vertex));
                }
            }
            // add the results to the query....
            _Query.Add(new JProperty("results", _resultsArray));

            return _Query.ToString();
        }

        #region private toJSON Extensions
        private JArray GenererateVertexViewJSON(IVertexView aVertex)
        {
            JArray _results = new JArray();

            // take one IVertexView and traverse through it
            #region Vertex Properties
            JObject _properties = new JObject();
            foreach (var _property in aVertex.GetAllProperties())
            {
                if (_property.Item2 == null)
                    _properties.Add(new JProperty(_property.Item1, ""));
                else
                    if (_property.Item2 is Stream)
                        _properties.Add(new JProperty(_property.Item1, "BinaryProperty"));
                    else
                        _properties.Add(new JProperty(_property.Item1, _property.Item2.ToString()));
            }
            // add to the results...
            _results.Add(new JObject(new JProperty("properties", new JObject(_properties))));
            #endregion

            #region Edges
            JArray _edges = new JArray();
            foreach (var _edge in aVertex.GetAllEdges())
            {
                if (_edge.Item2 == null)
                {
                    _edges.Add(new JObject(new JProperty(_edge.Item1, "")));             
                }
                else
                {
                    JArray _newEdge = GenerateEdgeViewJSON(_edge.Item2);
                    _edges.Add(new JObject(new JProperty(_edge.Item1, _newEdge)));
                }
            }
            // add to the results...
            _results.Add(new JObject(new JProperty("edges", _edges)));
            #endregion
            return _results;
        }

        private JArray GenerateEdgeViewJSON(IEdgeView aEdge)
        {
            JArray Output = new JArray();
            #region Edge Properties
            foreach (var _property in aEdge.GetAllProperties())
            {
                JProperty _newEdge = null;
                if (_property.Item2 == null)
                    _newEdge = new JProperty(_property.Item1, "");
                else
                    if (_property.Item2 is Stream)
                    {
                        _newEdge = new JProperty(_property.Item1, "BinaryProperty");
                    }
                    else
                        _newEdge = new JProperty(_property.Item1, _property.Item2.ToString());

                Output.Add(new JObject(_newEdge));
            }
            #endregion

            #region Target Vertices
            JArray _resultsArray = new JArray();
            // fill the results array
            foreach (IVertexView _vertex in aEdge.GetTargetVertices())
            {
                _resultsArray.Add(GenererateVertexViewJSON(_vertex));
            }
            #endregion
            Output.Add(new JObject(new JProperty("targetvertices", _resultsArray)));

            return Output;
        }

        #endregion

        #endregion

        #region Generate a QueryResult from JSON - not really needed right now
        public QueryResult GenerateQueryResult(string myResult)
        {
            throw new NotImplementedException();
        }

        public ContentType ContentType
        {
            get { return _contentType; }
        }
        #endregion

        #endregion

    }
}
