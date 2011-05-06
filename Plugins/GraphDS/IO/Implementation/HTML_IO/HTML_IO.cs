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
    public sealed class HTML_IO : IOInterface
    {

        #region Data

        private readonly ContentType _contentType;

        #endregion

        #region Constructors

        public HTML_IO()
        {
            _contentType = new ContentType("text/html") { CharSet = "UTF-8" };
        }

        #endregion

        #region IPluginable

        public string PluginName
        {
            get { return "sones.html_io"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get { return new Dictionary<string, Type>(); }
        }
        /* ASK what's this?
        public IPluginable InitializePlugin(Dictionary<string, object> myParameters, GraphApplicationSettings myApplicationSetting)
        {
            return InitializePlugin();
        }
        */
        public IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            var result = new HTML_IO();

            return (IPluginable)result;
        }

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

            Output.Append("<table class=\"gql_table\" border=\"1\"> <!-- MainTable -->");
            Output.Append("<tr><td style=\"width:250px\">query</td><td style=\"width:400px\">").Append(EscapeForXMLandHTML(myQueryResult.Query)).Append("</td></tr>");
            Output.Append("<tr><td style=\"width:250px\">result</td><td style=\"width:400px\">").Append(EscapeForXMLandHTML(myQueryResult.TypeOfResult.ToString())).Append("</td></tr>");
            Output.Append("<tr><td style=\"width:250px\">duration</td><td style=\"width:400px\">").Append(myQueryResult.Duration).Append(" ms</td></tr>");

            if (myQueryResult.Error != null)
            {
                Output.Append("<tr><td style=\"width:250px\">error</td><td style=\"width:400px\">").Append(EscapeForXMLandHTML(myQueryResult.Error.GetType().ToString()+" - "+HandleQueryExceptions(myQueryResult))).Append("</td></tr>");
            }

            if (myQueryResult.Vertices != null)
            {
                Output.Append("<table class=\"gql_table\" border=\"1\"> <!-- Vertices -->");
                Output.Append("<tr><td style=\"width:250px\">result</td><td style=\"width:400px\">");

                foreach (IVertexView _vertex in myQueryResult.Vertices)
                {
                    Output.Append("<table class=\"gql_table\" border=\"1\"> <!-- Vertices-2 -->");
                    Output.Append(GenerateVertexViewHTML(_vertex));
                    Output.Append("</table> <!-- Vertices-2 -->");
                }
                
                Output.Append("</td></tr>");
                Output.Append("</table> <!-- Vertices -->");
            }

            Output.Append("</table>  <!-- MainTable -->");
            return HTMLBuilder(myQueryResult,Output).ToString();
        }

        private String HandleQueryExceptions(QueryResult queryresult)
        {
            StringBuilder SB = new StringBuilder();

            SB.Append(queryresult.Error.ToString());
            if (queryresult.Error.InnerException != null)
                SB.Append(" InnerException: " + queryresult.Error.InnerException.Message);

            return SB.ToString();
        }

        #region private toHTML
        private String GenerateVertexViewHTML(IVertexView aVertex)
        {
            StringBuilder Output = new StringBuilder();
            // take one IVertexView and traverse through it
            #region Vertex Properties
            if (aVertex.GetCountOfProperties() > 0)
            {
                //Output.Append("<table class=\"gql_table\" border=\"1\"> <!-- VertexProperties -->");
                foreach (var _property in aVertex.GetAllProperties())
                {
                    if (_property.Item2 == null)
                        Output.Append("<tr><td style=\"width:250px\">").Append(EscapeForXMLandHTML(_property.Item1)).Append("</td><td style=\"width:400px\"></td></tr>");
                    else
                        if (_property.Item2 is Stream)
                            Output.Append("<tr><td style=\"width:250px\">").Append(EscapeForXMLandHTML(_property.Item1)).Append("</td><td style=\"width:400px\">BinaryProperty</td></tr>");
                        else
                            Output.Append("<tr><td style=\"width:250px\">").Append(EscapeForXMLandHTML(_property.Item1)).Append("</td><td style=\"width:400px\">").Append(EscapeForXMLandHTML(_property.Item2.ToString())).Append("</td></tr>");
                }
                //Output.Append("</table> <!-- VertexProperties -->");
            }
            #endregion

            #region Edges
            Output.Append("<tr><td><td><table class=\"gql_table\"border=\"1\"> <!-- Edges -->");
            Output.Append("<tr><td style=\"width:250px\">edges</td><td style=\"width:400px\">");

            foreach (var _edge in aVertex.GetAllEdges())
            {
                if (_edge.Item2 == null)
                {
                    Output.Append("<tr><td style=\"width:250px\">").Append(EscapeForXMLandHTML(_edge.Item1)).Append("</td><td style=\"width:400px\"></td></tr>");
                }
                else
                {
                    Output.Append("<tr><td style=\"width:250px\">").Append(EscapeForXMLandHTML(_edge.Item1)).Append("</td><td style=\"width:400px\">").Append(GenerateEdgeViewHTML(_edge.Item2)).Append("</td></tr>");
                }
            }

            Output.Append("</td></td></tr>");
            Output.Append("</table> <!-- Edges -->");
            // add to the results...
            //_results.Add(new JObject(new JProperty("edges", _edges)));
            #endregion

            return Output.ToString();
        }

        private String GenerateEdgeViewHTML(IEdgeView aEdge)
        {
            StringBuilder Output = new StringBuilder();
            Output.Append("<table class=\"gql_table\"border=\"1\"> <!-- EdgeView -->");

            #region Edge Properties
            if (aEdge.GetCountOfProperties() > 0)
            {
                Output.Append("<tr><td style=\"width:250px\">properties</td><td style=\"width:400px\">");
                Output.Append("<table class=\"gql_table\"border=\"1\"><tr><td style=\"width:400px\"> <!-- EdgeViewProperties -->");

                foreach (var _property in aEdge.GetAllProperties())
                {
                    if (_property.Item2 == null)
                        Output.Append(EscapeForXMLandHTML(_property.Item1)).Append("</td><td style=\"width:400px\"></td></tr>");
                    else
                        if (_property.Item2 is Stream)
                        {
                            Output.Append(EscapeForXMLandHTML(_property.Item1)).Append("</td><td style=\"width:400px\">BinaryProperty</td></tr>");
                        }
                        else
                            Output.Append(EscapeForXMLandHTML(_property.Item1)).Append("</td><td style=\"width:400px\">").Append(EscapeForXMLandHTML(_property.Item2.ToString())).Append("</td></tr>");
                }

                Output.Append("</table></td></tr> <!-- EdgeViewProperties -->");
            }
            #endregion


            #region Target Vertices
            Output.Append("<tr><td style=\"width:250px\">targetvertices</td><td style=\"width:400px\">");
            Output.Append("<table class=\"gql_table\"border=\"1\"><tr><td style=\"width:400px\"> <!-- TargetVertices -->");

            foreach (IVertexView _vertex in aEdge.GetTargetVertices())
            {
                Output.Append("<table class=\"gql_table\" border=\"1\"> <!-- Vertices-2 -->");
                Output.Append(GenerateVertexViewHTML(_vertex));
                Output.Append("</table> <!-- Vertices-2 -->");
            }

            Output.Append("</td></table> <!-- TargetVertices -->");

            Output.Append("</table></td></tr> <!-- EdgesView -->");

            return Output.ToString();
            #endregion

        }


        #endregion

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

        #region HTMLBuilder(myGraphDBName, myFunc)

        public StringBuilder HTMLBuilder(QueryResult myQueryResult, StringBuilder Input)
        {
            var _StringBuilder = new StringBuilder();

            _StringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            _StringBuilder.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");
            _StringBuilder.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            _StringBuilder.AppendLine("<head>");
            _StringBuilder.AppendLine("<title>sones GraphDS</title>");
            _StringBuilder.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"/resources/WebShell/WebShell.css\" />");
            _StringBuilder.AppendLine("</head>");
            _StringBuilder.AppendLine("<body>");
            _StringBuilder.AppendLine("<h1>sones GraphDS</h1>");
            _StringBuilder.Append("<h2>").Append(EscapeForXMLandHTML(myQueryResult.Query)).AppendLine("</h2>");
            _StringBuilder.AppendLine("<table>");
            _StringBuilder.AppendLine("<tr>");
            _StringBuilder.AppendLine("<td style=\"width: 100px\">&nbsp;</td>");
            _StringBuilder.AppendLine("<td>");

            // paste in the previous StringBuilder Input
            _StringBuilder.AppendLine(Input.ToString());

            _StringBuilder.AppendLine("</td>");
            _StringBuilder.AppendLine("</tr>");
            _StringBuilder.AppendLine("</table>");
            _StringBuilder.AppendLine("</body>").AppendLine("</html>").AppendLine();

            return _StringBuilder;

        }

        #endregion

        #endregion

        #endregion

    }
}
