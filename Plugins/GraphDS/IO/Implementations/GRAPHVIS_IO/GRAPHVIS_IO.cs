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
    /// This class realize a visual graph output based on D3 framework.
    /// </summary>
    public class GRAPHVIS_IO : IOInterface
    {

        #region Data

        /// <summary>
        /// The io content type.
        /// </summary>
        private readonly ContentType _contentType;

        #endregion

        #region Parameter Strings

        /// <summary>
        /// static class containing strings used to set plugin options
        /// </summary>
        private static class _OptionStrings
        {

            /// <summary>
            /// static class containing strings used to set ShowEdgeNames option
            /// </summary>
            public static class ShowEdgeNames
            {
                public static string name = "edgenames";
                public static string description = "show edge names";
                public static string show = "show";
                public static string hide = "hide";
            }
        }

        #endregion

        /// <summary>
        /// class describing one link between nodes
        /// </summary>
        private class NodeLink
        {
            public int source;
            public int target;
            public string name;
        }

        #region Constructors

        /// <summary>
        /// Constructor for a graphvis io instance.
        /// </summary>
        public GRAPHVIS_IO()
        {
            _contentType = new ContentType("application/x-sones-graphvis") { CharSet = "UTF-8" };
        }

        #endregion


        #region IPluginable

        public string PluginName
        {
            get { return "sones.graphvis_io"; }
        }

        public string PluginShortName
        {
            get { return "graphvis"; }
        }

        public string PluginDescription
        {
            get { return "This class realize a visual graph output based on D3 framework."; }
        }

        public PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }

        public IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            var result = new GRAPHVIS_IO();

            return (IPluginable)result;
        }

        public void Dispose()
        { }

        #endregion

        #region IOInterface

        public string GenerateOutputResult(QueryResult myQueryResult, Dictionary<String, String> myParams)
        {
            StringBuilder Output = new StringBuilder();
            bool bShowEdgeNames = false;

            if (myParams.ContainsKey(_OptionStrings.ShowEdgeNames.name))
            {
                if (StringComparer.InvariantCultureIgnoreCase.Compare(myParams[_OptionStrings.ShowEdgeNames.name], _OptionStrings.ShowEdgeNames.show) == 0)
                {
                    bShowEdgeNames = true;
                }
                else if (StringComparer.InvariantCultureIgnoreCase.Compare(myParams[_OptionStrings.ShowEdgeNames.name], _OptionStrings.ShowEdgeNames.hide) == 0)
                {
                    bShowEdgeNames = false;
                }
            }

            if (myQueryResult.Error != null)
            {
                Output.Append(ConvertString2WebShellOut(HandleQueryExceptions(myQueryResult)));
                return Output.ToString();
            }

            List<string> Nodes;
            List<NodeLink> Links;
            GenerateVisGraph(myQueryResult, out Nodes, out Links);

            if (Nodes.Count == 0)
            {
                Output.Append(ConvertString2WebShellOut("Graph Visualisation Plugin:"));
                Output.Append(ConvertString2WebShellOut("Error: No vertex with property \"node\" found!"));
                Output.Append(ConvertString2WebShellOut("Please change your query string using \"as node\" for returned vertices"));
                Output.Append(ConvertString2WebShellOut("If you use WebShell and want to use another output format plugin,"));
                Output.Append(ConvertString2WebShellOut("enter e.g. format json for JSON output!"));
                return Output.ToString();
            }
            else
            {
                Output.AppendLine("var w = 960,");
                Output.AppendLine("h = 500;");
                Output.AppendLine("var vis = d3.select(\"#output\")");
                Output.AppendLine(".append(\"svg:svg\")");
                Output.AppendLine(".attr(\"width\", w)");
                Output.AppendLine(".attr(\"height\", h);");

                Output.AppendLine("var nodes = new Array();");
                Output.AppendLine("var links = new Array();");

                foreach (var Node in Nodes)
                {
                    Output.AppendLine("var node = new Object();");
                    Output.AppendLine("node.name=\"" + Node + "\";");
                    Output.AppendLine("nodes.push(node);");
                }

                foreach (var Link in Links)
                {
                    Output.AppendLine("var link = new Object();");
                    Output.AppendLine("link.source=" + Link.source + ";");
                    Output.AppendLine("link.target=" + Link.target + ";");
                    Output.AppendLine("link.name=\"" + Link.name + "\";");
                    Output.AppendLine("links.push(link);");
                }

                Output.AppendLine("var force = self.force = d3.layout.force()");
                Output.AppendLine(".nodes(nodes)");
                Output.AppendLine(".links(links)");
                Output.AppendLine(".gravity(.05)");
                Output.AppendLine(".distance(100)");
                Output.AppendLine(".charge(-100)");
                Output.AppendLine(".size([w, h])");
                Output.AppendLine(".start();");

                Output.AppendLine("var linkgroup = vis.selectAll(\"g.link\")");
                Output.AppendLine(".data(links)");
                Output.AppendLine(".enter().append(\"svg:g\");");

                Output.AppendLine("var link = linkgroup.append(\"svg:line\")");
                Output.AppendLine(".attr(\"class\", \"graphlink\");");

                if (bShowEdgeNames)
                {
                    Output.AppendLine("var linkname = linkgroup.append(\"svg:text\")");
                    Output.AppendLine(".attr(\"class\", \"graphlinktext\")");
                    Output.AppendLine(".text(function(d) { return d.name; });");
                }

                Output.AppendLine("var node = vis.selectAll(\"g.node\")");
                Output.AppendLine(".data(nodes)");
                Output.AppendLine(".enter().append(\"svg:g\")");
                Output.AppendLine(".attr(\"class\", \"node\")");
                Output.AppendLine(".call(force.drag);");

                Output.AppendLine("node.append(\"svg:image\")");
                Output.AppendLine(".attr(\"class\", \"circle\")");
                Output.AppendLine(".attr(\"xlink:href\", \"favicon.ico\")");
                Output.AppendLine(".attr(\"x\", \"-8px\")");
                Output.AppendLine(".attr(\"y\", \"-8px\")");
                Output.AppendLine(".attr(\"width\", \"16px\")");
                Output.AppendLine(".attr(\"height\", \"16px\");");

                Output.AppendLine("node.append(\"svg:text\")");
                Output.AppendLine(".attr(\"class\", \"nodetext\")");
                Output.AppendLine(".attr(\"dx\", 12)");
                Output.AppendLine(".attr(\"dy\", \".35em\")");
                Output.AppendLine(".text(function(d) { return d.name });");

                Output.AppendLine("force.on(\"tick\", function() {");
                Output.AppendLine("link.attr(\"x1\", function(d) { return d.source.x; })");
                Output.AppendLine(".attr(\"y1\", function(d) { return d.source.y; })");
                Output.AppendLine(".attr(\"x2\", function(d) { return d.target.x; })");
                Output.AppendLine(".attr(\"y2\", function(d) { return d.target.y; });");
                if (bShowEdgeNames)
                {
                    Output.AppendLine("linkname.attr(\"x\", function(d) { return (d.source.x + d.target.x) / 2; })");
                    Output.AppendLine(".attr(\"y\", function(d) { return (d.source.y + d.target.y) / 2; });");
                }
                Output.AppendLine("node.attr(\"transform\", function(d) { return \"translate(\" + d.x + \",\" + d.y + \")\"; });");
                Output.AppendLine("});");

                return Output.ToString();
            }
        }

        /// <summary>
        /// generates information needed for visual graph out of query result
        /// <param name="myQueryResult">The query result.</param>
        /// <param name="Nodes">out parameter returning list of nodes</param>
        /// <param name="Links">out parameter returning list of NodeLink classes</param>
        /// </summary>
        private void GenerateVisGraph(QueryResult myQueryResult, out List<string> Nodes, out List<NodeLink> Links)
        {
            GenerateNodeList(myQueryResult, out Nodes);
            GenerateLinkList(myQueryResult, Nodes, out Links);
        }

        /// <summary>
        /// generate node list needed for visual graph out of query result
        /// <param name="myQueryResult">The query result.</param>
        /// <param name="Nodes">out parameter returning list of nodes</param>
        /// </summary>
        private void GenerateNodeList(QueryResult myQueryResult, out List<string> Nodes)
        {
            Nodes = new List<string>();

            GenerateNodeList_AnalyzeVertices(myQueryResult, ref Nodes);
        }

        /// <summary>
        /// recursive function analyzing vertices and adding found nodes to node list
        /// <param name="myQueryResult">The query result.</param>
        /// <param name="Nodes">reference to list of nodes where found nodes are added</param>
        /// </summary>
        private void GenerateNodeList_AnalyzeVertices(IEnumerable<IVertexView> vertices, ref List<string> Nodes)
        {
            foreach (var aVertex in vertices)
            {
                foreach (var property in aVertex.GetAllProperties())
                {
                    if ((property.Item1 != null) && (property.Item2 != null))
                    {
                        if ((property.Item1 is String) && (property.Item1.ToString().ToUpper().IndexOf("NODE")) >= 0)
                        {
                            if (!Nodes.Exists((s) => (s == property.Item2.ToString())))
                            {
                                Nodes.Add(property.Item2.ToString());
                            }
                        }
                    }
                }

                foreach (var edge in aVertex.GetAllEdges())
                {
                    GenerateNodeList_AnalyzeVertices(edge.Item2.GetTargetVertices(), ref Nodes);
                }
            }
        }

        /// <summary>
        /// generate link list needed for visual graph out of query result
        /// <param name="myQueryResult">The query result.</param>
        /// <param name="Links">out parameter returning list of NodeLink classes</param>
        /// </summary>
        private void GenerateLinkList(QueryResult myQueryResult, List<string> Nodes, out List<NodeLink> Links)
        {
            Links = new List<NodeLink>();

            if (Nodes.Count() <= 0) return;

            GenerateLinkList_AnalyzeVertices(myQueryResult, Nodes, ref Links);
        }

        /// <summary>
        /// recursive function analyzing vertices and adding found links to link list
        /// <param name="myQueryResult">The query result.</param>
        /// <param name="Nodes">list of nodes containing all nodes found by GenerateNodeList previously</param>
        /// <param name="Nodes">reference to list of nodes where found nodes are added</param>
        /// </summary>
        private void GenerateLinkList_AnalyzeVertices(IEnumerable<IVertexView> vertices, List<string> Nodes, ref List<NodeLink> Links)
        {
            foreach (var aVertex in vertices)
            {
                string sourcename = null;

                foreach (var property in aVertex.GetAllProperties())
                {
                    if ((property.Item1 is String) && (property.Item1.ToString().ToUpper().IndexOf("NODE") >= 0) && (property.Item2 is String))
                    {
                        sourcename = property.Item2.ToString();
                    }
                }

                foreach (var edge in aVertex.GetAllEdges())
                {
                    foreach (var targetvertex in edge.Item2.GetTargetVertices())
                    {
                        string targetname = null;

                        foreach (var property in targetvertex.GetAllProperties())
                        {
                            if ((property.Item1 is String) && (property.Item1.ToString().ToUpper().IndexOf("NODE") >= 0) && (property.Item2 is String))
                            {
                                targetname = property.Item2.ToString();
                                break;
                            }
                        }

                        if ((sourcename != null) && (targetname != null))
                        {
                            NodeLink newlink = new NodeLink();

                            if (edge.Item1 is String) newlink.name = edge.Item1.ToString();
                            else newlink.name = "";
                            newlink.source = Nodes.IndexOf(sourcename);
                            newlink.target = Nodes.IndexOf(targetname);

                            if ((newlink.source >= 0) && (newlink.target >= 0))
                            {
                                Links.Add(newlink);
                            }
                        }
                    }

                    GenerateLinkList_AnalyzeVertices(edge.Item2.GetTargetVertices(), Nodes, ref Links);
                }
            }
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

        public String ListAvailParams()
        {
            StringBuilder list = new StringBuilder();

            list.AppendLine("Available Parameters to configure GraphVis Output:");
            list.AppendLine(_OptionStrings.ShowEdgeNames.name + "=[" + _OptionStrings.ShowEdgeNames.show + "|" + _OptionStrings.ShowEdgeNames.hide + "] " + _OptionStrings.ShowEdgeNames.description);

            return list.ToString();
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

    }
}
