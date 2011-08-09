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
using sones.GraphQL.Result;
using sones.Library.Settings;
using sones.Library.VersionedPluginManager;
using System.Xml;
using System.Reflection;
using System.Text;

namespace sones.Plugins.GraphDS.IO
{

    /// <summary>
    /// This class realize an barchart output based on D3 framework.
    /// </summary>
    public sealed class BARCHART_IO : IOInterface
    {

        #region Data

        /// <summary>
        /// The io content type.
        /// </summary>
        private readonly ContentType _contentType;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for a barchart io instance.
        /// </summary>
        public BARCHART_IO()
        {
            _contentType = new ContentType("application/x-sones-barchart") { CharSet = "UTF-8" };
        }

        #endregion

        #region IPluginable

        public string PluginName
        {
            get { return "sones.barchart_io"; }
        }

        public PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }

        public IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            var result = new BARCHART_IO();

            return (IPluginable)result;
        }

        public void Dispose()
        { }

        #endregion

        #region IOInterface

        #region Generate Output from Query Result

        public string GenerateOutputResult(QueryResult myQueryResult)
        {
            StringBuilder Output = new StringBuilder();
            Dictionary<String, object> barchart;

            if (myQueryResult.Error != null)
            {
                Output.Append(ConvertString2WebShellOut(HandleQueryExceptions(myQueryResult)));
                return Output.ToString();
            }

            barchart = GenerateBarChart(myQueryResult);

            if (barchart.Count == 0)
            {
                Output.Append(ConvertString2WebShellOut("Error: No Properties with name x and y found!"));
                return Output.ToString();
            }
            else
            {
                Output.Append("var data = new Array();");
                Output.Append("var names = new Array();");

                foreach (KeyValuePair<string, object> bar in barchart)
                {
                    Output.Append("data.push(" + bar.Value + ");");
                    Output.Append("names.push(\'" + bar.Key.ToString() + "\');");
                }

                Output.Append("var w = 430,");
                Output.Append("h = data.length * 23,");
                Output.Append("x = d3.scale.linear().domain([0, d3.max(data)]).range([0, w]),");
                Output.Append("y = d3.scale.ordinal().domain(d3.range(data.length)).rangeBands([0, h], .2);");

                Output.Append("var vis = d3.select(\"#output\")");
                Output.Append(".append(\"svg:svg\")");
				Output.Append(".attr(\"class\",\"twodbarchart\")");
				
                Output.Append(".attr(\"width\", w + 200)");
                Output.Append(".attr(\"height\", h + 20)");
                Output.Append(".append(\"svg:g\")");
                Output.Append(".attr(\"transform\", \"translate(20,0)\");");

                Output.Append("var bars = vis.selectAll(\"g.bar\")");
                Output.Append(".data(data)");
                Output.Append(".enter().append(\"svg:g\")");
                Output.Append(".attr(\"class\", \"bar\")");
                Output.Append(".attr(\"transform\", function(d, i) { return \"translate(0,\" + y(i) + \")\"; });");

                Output.Append("bars.append(\"svg:rect\")");
                Output.Append(".attr(\"width\", x)");
                Output.Append(".attr(\"height\", y.rangeBand());");

                Output.Append("bars.append(\"svg:text\")");
                Output.Append(".attr(\"x\", x)");
                Output.Append(".attr(\"y\", y.rangeBand() / 2)");
                Output.Append(".attr(\"dx\", -6)");
                Output.Append(".attr(\"dy\", \".35em\")");
                Output.Append(".attr(\"fill\", \"white\")");
                Output.Append(".attr(\"text-anchor\", \"end\")");
                Output.Append(".text(x.tickFormat(100));");

                Output.Append("bars.append(\"svg:text\")");
                Output.Append(".attr(\"x\", x)");
                Output.Append(".attr(\"y\", y.rangeBand() / 2)");
                Output.Append(".attr(\"dx\", 2)");
                Output.Append(".attr(\"dy\", \".35em\")");
                Output.Append(".attr(\"text-anchor\", \"start\")");
                Output.Append(".text(function(d, i) { return names[i]; });");

                Output.Append("var rules = vis.selectAll(\"g.rule\")");
                Output.Append(".data(x.ticks(10))");
                Output.Append(".enter().append(\"svg:g\")");
                Output.Append(".attr(\"class\", \"rule\")");
                Output.Append(".attr(\"transform\", function(d) { return \"translate(\" + x(d) + \",0)\"; });");
                Output.Append("rules.append(\"svg:line\")");
                Output.Append(".attr(\"y1\", h)");
                Output.Append(".attr(\"y2\", h + 6)");
                Output.Append(".attr(\"stroke\", \"black\");");

                Output.Append("rules.append(\"svg:line\")");
                Output.Append(".attr(\"y1\", 0)");
                Output.Append(".attr(\"y2\", h)");
                Output.Append(".attr(\"stroke\", \"white\")");
                Output.Append(".attr(\"stroke-opacity\", .3);");

                Output.Append("rules.append(\"svg:text\")");
                Output.Append(".attr(\"y\", h + 9)");
                Output.Append(".attr(\"dy\", \".71em\")");
                Output.Append(".attr(\"text-anchor\", \"middle\")");
                Output.Append(".text(x.tickFormat(10));");

                Output.Append("vis.append(\"svg:line\")");
                Output.Append(".attr(\"y1\", 0)");
                Output.Append(".attr(\"y2\", h)");
                Output.Append(".attr(\"stroke\", \"black\");");
            }

            return Output.ToString();
        }

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
                SB.Append(" InnerException: " + queryresult.Error.InnerException.Message);

            return SB.ToString();
        }

        /// <summary>
        /// Add necessary JS commands to a string to enable output in webshell
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>string containing JS commands and embedded input string</returns>
        private String ConvertString2WebShellOut(String input)
        {
            StringBuilder SB = new StringBuilder();

            SB.Append("goosh.gui.out(\'");
            SB.Append(input.Replace("\n", "<br>").Replace("\'", "\\\'").Replace("\"", "\\\""));
            SB.Append("\');");

            return SB.ToString();
        }

        /// <summary>
        /// generate bar chart data dictionary out of query result by searching recursively
        /// </summary>
        /// <param name="myQueryResult">query result</param>
        /// <returns>dictionary containing bar chart data</returns>
        private Dictionary<String, object> GenerateBarChart(QueryResult myQueryResult)
        {
            Dictionary<String, object> barchart = new Dictionary<string, object>();

            foreach (var aVertex in myQueryResult)
            {
                AnalyzeProperties(aVertex.GetAllProperties(), ref barchart);
                AnalyzeEdges(aVertex.GetAllEdges(), ref barchart);
            }

            return barchart;
        }

        /// <summary>
        /// scan IEnumerable of properties for such with name x and y that can be used for bar chart
        /// </summary>
        /// <param name="properties">IEnumerable of properties to scan</param>
        /// <param name="barchart">reference to dictionary where found data should be added</param>
        private void AnalyzeProperties(IEnumerable<Tuple<String, Object>> properties, ref Dictionary<String, object> barchart)
        {
            String x = null;
            object y = 0;
            bool hasx = false;
            bool hasy = false;

            foreach (var property in properties)
            {
                if ((property.Item1 != null) && (property.Item2 != null))
                {
                    if ((property.Item1 is String) && (property.Item1.ToString().ToUpper() == "X"))
                    {
                        hasx = true;
                        x = property.Item2.ToString();
                    }

                    if ((property.Item2 is object) && (property.Item1.ToString().ToUpper() == "Y"))
                    {
                        hasy = true;
                        y = property.Item2;
                    }
                }
            }

            if (hasx && hasy)
            {
                barchart.Add(x, y);
            }
        }

        /// <summary>
        /// scan IEnumerable of edges recursively for bar chart data
        /// </summary>
        /// <param name="edges">IEnumerable of edges to be scanned</param>
        /// <param name="barchart">reference to dictionary where found data should be added</param>
        private void AnalyzeEdges(IEnumerable<Tuple<String, IEdgeView>> edges, ref Dictionary<String, object> barchart)
        {
            foreach (Tuple<String, IEdgeView> edge in edges)
            {
                AnalyzeTargetVertices(edge.Item2.GetTargetVertices(), ref barchart);
            }
        }

        /// <summary>
        /// scan IEnumerable of vertices recursively for bar chart data
        /// </summary>
        /// <param name="edges">IEnumerable of vertices to be scanned</param>
        /// <param name="barchart">reference to dictionary where found data should be added</param>
        private void AnalyzeTargetVertices(IEnumerable<IVertexView> targetvertices, ref Dictionary<String, object> barchart)
        {
            foreach (var aVertex in targetvertices)
            {
                AnalyzeProperties(aVertex.GetAllProperties(), ref barchart);
                AnalyzeEdges(aVertex.GetAllEdges(), ref barchart);
            }
        }

        #region Generate a QueryResult from HTML - not really needed right now
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

        #endregion

    }
}
