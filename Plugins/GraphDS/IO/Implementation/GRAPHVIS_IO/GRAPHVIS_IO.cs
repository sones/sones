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
    public class GRAPHVIS_IO : IOInterface
    {

        #region Data

        /// <summary>
        /// The io content type.
        /// </summary>
        private readonly ContentType _contentType;

        #endregion

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

            Output.AppendLine("var w = 960,");
            Output.AppendLine("h = 500,");
            Output.AppendLine("fill = d3.scale.category20();");
            Output.AppendLine("var vis = d3.select(\"#output\")");
            Output.AppendLine(".append(\"svg:svg\")");
            Output.AppendLine(".attr(\"width\", w)");
            Output.AppendLine(".attr(\"height\", h);");
            //Output.AppendLine("d3.json(\"miserables.json\", function(json) {");
            Output.AppendLine("var json = jQuery.parseJSON('{\"nodes\":[{\"name\":\"Myriel\",\"group\":1},{\"name\":\"Gervais\",\"group\":2}],\"links\":[{\"source\":1,\"target\":0,\"value\":1}]}\');");
            Output.AppendLine("var force = d3.layout.force()");
            Output.AppendLine(".charge(-120)");
            Output.AppendLine(".linkDistance(30)");
            Output.AppendLine(".nodes(json.nodes)");
            Output.AppendLine(".links(json.links)");
            Output.AppendLine(".size([w, h])");
            Output.AppendLine(".start();");

            Output.AppendLine("var link = vis.selectAll(\"line.link\")");
            Output.AppendLine(".data(json.links)");
            Output.AppendLine(".enter().append(\"svg:line\")");
            Output.AppendLine(".attr(\"class\", \"link\")");
            Output.AppendLine(".style(\"stroke-width\", function(d) { return Math.sqrt(d.value); })");
            Output.AppendLine(".attr(\"x1\", function(d) { return d.source.x; })");
            Output.AppendLine(".attr(\"y1\", function(d) { return d.source.y; })");
            Output.AppendLine(".attr(\"x2\", function(d) { return d.target.x; })");
            Output.AppendLine(".attr(\"y2\", function(d) { return d.target.y; });");
            Output.AppendLine("var node = vis.selectAll(\"circle.node\")");
            Output.AppendLine(".data(json.nodes)");
            Output.AppendLine(".enter().append(\"svg:circle\")");
            Output.AppendLine(".attr(\"class\", \"node\")");
            Output.AppendLine(".attr(\"cx\", function(d) { return d.x; })");
            Output.AppendLine(".attr(\"cy\", function(d) { return d.y; })");
            Output.AppendLine(".attr(\"r\", 5)");
            Output.AppendLine(".style(\"fill\", function(d) { return fill(d.group); })");
            Output.AppendLine(".call(force.drag);");
            Output.AppendLine("node.append(\"svg:title\")");
            Output.AppendLine(".text(function(d) { return d.name; });");
            Output.AppendLine("vis.style(\"opacity\", 1e-6)");
            Output.AppendLine(".transition()");
            Output.AppendLine(".duration(1000)");
            Output.AppendLine(".style(\"opacity\", 1);");
            Output.AppendLine("force.on(\"tick\", function() {");
            Output.AppendLine("link.attr(\"x1\", function(d) { return d.source.x; })");
            Output.AppendLine(".attr(\"y1\", function(d) { return d.source.y; })");
            Output.AppendLine(".attr(\"x2\", function(d) { return d.target.x; })");
            Output.AppendLine(".attr(\"y2\", function(d) { return d.target.y; });");
            Output.AppendLine("node.attr(\"cx\", function(d) { return d.x; })");
            Output.AppendLine(".attr(\"cy\", function(d) { return d.y; });");
            Output.AppendLine("});");

            return Output.ToString();
        }

        public String ListAvailParams()
        {
            throw new NotImplementedException();
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
