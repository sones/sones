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
using System.Net.Mime;
using System.IO;
using sones.GraphQL.Result;
using sones.Library.VersionedPluginManager;
using Newtonsoft.Json.Linq;
using System.Text;
using sones.GraphDB.Expression.Tree.Literals;

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
            _Query.Add(new JProperty("Query", myQueryResult.Query));
            // result -------------------------------
            _Query.Add(new JProperty("Result", myQueryResult.TypeOfResult.ToString()));
            // duration -----------------------------
            _Query.Add(new JProperty("Duration", new JArray(myQueryResult.Duration, "ms")));
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
                _Query.Add(new JProperty("Errors", new JArray(
                             new JObject(
                             new JProperty("Code", myQueryResult.Error.GetType().ToString()),
                             new JProperty("Description", HandleQueryExceptions(myQueryResult))
                           ))));
            }
            // results ------------------------------
            JArray _resultsArray = new JArray();
            // fill the results array
            if (myQueryResult.Vertices != null)
            {
                foreach (IVertexView _vertex in myQueryResult.Vertices)
                {
                    _resultsArray.Add(GenerateVertexViewJSON(_vertex));
                }
            }
            // add the results to the query....
            _Query.Add(new JProperty("Results", _resultsArray));

            return _Query.ToString();
        }

        private String HandleQueryExceptions(QueryResult queryresult)
        {
            StringBuilder SB = new StringBuilder();

            SB.Append(queryresult.Error.ToString());
            if (queryresult.Error.InnerException != null)
                SB.Append(" InnerException: "+queryresult.Error.InnerException.Message);

            return SB.ToString();
        }

        #region private toJSON Extensions
        private JArray GenerateVertexViewJSON(IVertexView aVertex)
        {
            JArray _results = new JArray();

            if (aVertex != null)
            {
                // take one IVertexView and traverse through it
                #region Vertex Properties
                JObject _properties = new JObject();

                foreach (var _property in aVertex.GetAllProperties())
                {
                    if (_property.Item2 == null)
                        _properties.Add(new JProperty(_property.Item1, ""));
                    else
                        if (_property.Item2 is Stream)
                        {
                            _properties.Add(new JProperty(_property.Item1, "BinaryProperty"));
                        }
                        else
                        {                            
                            if (_property.Item2 is ICollectionWrapper)
                            {
                                var values = new JArray();

                                foreach (var value in ((ICollectionWrapper)_property.Item2))
                                {
                                    values.Add(new JArray(value.ToString()));
                                }

                                _properties.Add(new JProperty(_property.Item1, values));
                            }
                            else
                            {
                                _properties.Add(new JProperty(_property.Item1, _property.Item2.ToString()));
                            }
                        }
                }
                // add to the results...
                _results.Add(new JObject(new JProperty("Properties", new JObject(_properties))));
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
                        _edges.Add(new JObject(new JProperty(_edge.Item2.GetType().Name, new JObject(new JProperty(_edge.Item1, _newEdge)))));
                    }
                }
                // add to the results...
                _results.Add(new JObject(new JProperty("Edges", _edges)));
                #endregion
            }
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

                Output.Add(new JObject(new JProperty("Properties", new JObject(_newEdge))));
            }
            #endregion

            if (aEdge is IHyperEdgeView)
            {                
                var edgeProperties = new JArray();                  

                foreach (var singleEdge in ((IHyperEdgeView)aEdge).GetAllEdges())
                {
                    foreach (var singleEdgeProp in singleEdge.GetAllProperties())
                    {
                        edgeProperties.Add(new JObject(new JProperty(singleEdgeProp.Item1, singleEdgeProp.Item2.ToString())));
                    }

                    Output.Add(new JObject(new JProperty("SingleEdge", new JObject(new JProperty("Properties", new JArray(edgeProperties))), new JObject(new JProperty("TargetVertex", GenerateVertexViewJSON(singleEdge.GetTargetVertex()))))));
                    
                    edgeProperties.Clear();                    
                }                
                
            }
            else
            {
                Output.Add(new JObject(new JProperty("TargetVertex", GenerateVertexViewJSON(((ISingleEdgeView)aEdge).GetTargetVertex()))));
            }

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
