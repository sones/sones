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
    public sealed class BARCHART_IO : IOInterface
    {

        #region Data

        private readonly ContentType _contentType;

        #endregion

        #region Constructors

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

        public String EscapeForXMLandHTML(String myString)
        {
            myString = myString.Replace("<", "&lt;");
            myString = myString.Replace(">", "&gt;");
            myString = myString.Replace("&", "&amp;");
            return myString;
        }

        public string GenerateOutputResult(QueryResult myQueryResult)
        {
            StringBuilder Output = new StringBuilder();

            Output.Append("var data = d3.range(10).map(Math.random);");

            Output.Append("var w = 430,");
            Output.Append("h = 230,");
            Output.Append("x = d3.scale.linear().domain([0, 1]).range([0, w]),");
            Output.Append("y = d3.scale.ordinal().domain(d3.range(data.length)).rangeBands([0, h], .2);");

            Output.Append("var vis = d3.select(\"#output\")");
            Output.Append(".append(\"svg:svg\")");
            Output.Append(".attr(\"width\", w + 40)");
            Output.Append(".attr(\"height\", h + 20)");
            Output.Append(".append(\"svg:g\")");
            Output.Append(".attr(\"transform\", \"translate(20,0)\");");

            Output.Append("var bars = vis.selectAll(\"g.bar\")");
            Output.Append(".data(data)");
            Output.Append(".enter().append(\"svg:g\")");
            Output.Append(".attr(\"class\", \"bar\")");
            Output.Append(".attr(\"transform\", function(d, i) { return \"translate(0,\" + y(i) + \")\"; });");
        
            Output.Append("bars.append(\"svg:rect\")");
            Output.Append(".attr(\"fill\", \"steelblue\")");
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
            Output.Append(".attr(\"x\", 0)");
            Output.Append(".attr(\"y\", y.rangeBand() / 2)");
            Output.Append(".attr(\"dx\", -6)");
            Output.Append(".attr(\"dy\", \".35em\")");
            Output.Append(".attr(\"text-anchor\", \"end\")");
            Output.Append(".text(function(d, i) { return String.fromCharCode(65 + i); });");

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

            return Output.ToString();
        }

        private String HandleQueryExceptions(QueryResult queryresult)
        {
            StringBuilder SB = new StringBuilder();

            SB.Append(queryresult.Error.ToString());
            if (queryresult.Error.InnerException != null)
                SB.Append(" InnerException: " + queryresult.Error.InnerException.Message);

            return SB.ToString();
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
