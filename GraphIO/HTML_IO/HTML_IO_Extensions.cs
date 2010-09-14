/* 
 * HTML_IO_Extensions
 * Achim 'ahzf' Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;


using sones.Lib;
using System.Collections.Generic;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Result;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphIO.HTML
{

    /// <summary>
    /// Extension methods to transform a QueryResult and a DBObjectReadout into a text/html representation
    /// </summary>

    public static class HTML_IO_Extensions
    {

        #region ToHTML(this myQueryResult)

        public static String ToHTML(this QueryResult myQueryResult)
        {

            var myStringBuilder = new StringBuilder();

            myStringBuilder.AppendLine("<table class=\"gql_table\" border=\"1\">");

            // root element...
            myStringBuilder.AppendLine("<tr><td>queryresult</td><td>version = \"1.0\"</td></tr>");


            // query --------------------------------
            myStringBuilder.AppendLine("<tr><td>query</td><td>" + myQueryResult.Query.EscapeForXMLandHTML() + "</td></tr>");

            // result -------------------------------
            myStringBuilder.AppendLine("<tr><td>result</td><td>" + myQueryResult.ResultType.ToString().EscapeForXMLandHTML() + "</td></tr>");

            // duration -----------------------------
            myStringBuilder.AppendLine("<tr><td>duration</td><td>" + myQueryResult.Duration.ToString().EscapeForXMLandHTML() + " ms</td></tr>");

            // warnings -----------------------------
            myStringBuilder.Append("<tr><td>warnings</td><td><table border=\"1\">");
            foreach (var _Warning in myQueryResult.Warnings)
                myStringBuilder.Append("<tr><td>" + _Warning.GetType().Name + "</td><td> => </td><td>" + _Warning.ToString().EscapeForXMLandHTML() + "</td></tr>");
            myStringBuilder.AppendLine("</table></td></tr>");

            // errors -------------------------------
            myStringBuilder.Append("<tr><td>errors</td><td><table border=\"1\">");
            foreach (var _Error in myQueryResult.Errors)
                myStringBuilder.Append("<tr><td>" + _Error.GetType().Name + "</td><td> => </td><td>" + _Error.ToString().EscapeForXMLandHTML() + "</td></tr>");
            myStringBuilder.AppendLine("</table></td></tr>");

            // results ------------------------------
            myStringBuilder.Append("<tr><td>results</td><td><table>");
            if (myQueryResult.Vertices != null)
                foreach (var _DBObject in myQueryResult)
                {
                    myStringBuilder.Append("<tr><td>");
                    myStringBuilder.Append(_DBObject.ToHTML());
                    myStringBuilder.Append("</td></tr>");
                }
            myStringBuilder.AppendLine("</table></td></tr>");

            myStringBuilder.AppendLine("</table>");

            return myStringBuilder.ToString();

        }

        #endregion


        #region ToHTML(this myVertex)

        public static String ToHTML(this Vertex myVertex)
        {
            return myVertex.ToHTML(false);
        }

        #endregion

        #region (private) ToHTML(this myVertex, myRecursion)

        private static String ToHTML(this Vertex myVertex, Boolean myRecursion)
        {

            var _StringBuilder = new StringBuilder();

            var _TypeObject = myVertex.TYPE;
            //String _Type = (_TypeObject != null) ? _TypeObject.Name : "";
            String _Type = _TypeObject;
            var _UUID = myVertex.UUID;

            IEnumerable<Vertex> _Vertex = null;
            IEnumerable<Object> _AttributeValueList = null;

            _StringBuilder.AppendLine("<table class=\"gql_table\" border=\"1\">");

            #region Vertex_WeightedEdges

            var _WeightedDBObject1 = myVertex as Vertex_WeightedEdges;

            if (_WeightedDBObject1 != null)
            {
                _StringBuilder.Append("<tr><td style=\"width:250px\">edgelabel</td><td style=\"width:400px\">");
                _StringBuilder.Append("<table border=\"1\"><tr><td>weight</td><td>" + _WeightedDBObject1.TypeName.EscapeForXMLandHTML() + "</td><td>" + _WeightedDBObject1.Weight.ToString().EscapeForXMLandHTML() + "</td></tr></table>");
                _StringBuilder.Append("</td></tr>");
            }

            #endregion

            foreach (var _Attribute in myVertex.ObsoleteAttributes)
            {

                switch (_Attribute.Key)
                {

                    case "TYPE":
                    case "Type":
                        var _ThisTypeObject = _Attribute.Value as GraphDBType;
                        String _ThisType = (_ThisTypeObject != null) ? _ThisTypeObject.Name : "";
                        _StringBuilder.Append("<tr><td style=\"width:250px\"><a href=\"/objects/\">TYPE</a></td><td style=\"width:400px\">").Append(_ThisType.EscapeForXMLandHTML()).Append("</td></tr>");
                        break;

                    case "UUID":
                        _StringBuilder.Append("<tr><td style=\"width:250px\"><a href=\"/objects/").Append(_Type).Append("/").Append(_UUID.ToString().EscapeForXMLandHTML()).Append("/\">UUID</a></td><td style=\"width:400px\">").Append(_Attribute.Value.ToString().EscapeForXMLandHTML()).Append("</td></tr>");
                        break;

                    case "EDITION":
                        _StringBuilder.Append("<tr><td style=\"width:250px\"><a href=\"/objects/").Append(_Type).Append("\">EDITION</a></td><td style=\"width:400px\">").Append(_Attribute.Value.ToString().EscapeForXMLandHTML()).Append("</td></tr>");
                        break;

                    case "EDITIONS":
                        _StringBuilder.Append("<tr><td style=\"width:250px\"><a href=\"/objects/").Append(_Type).Append("\">EDITIONS</a></td><td style=\"width:400px\">").Append(_Attribute.Value.ToString().EscapeForXMLandHTML()).Append("</td></tr>");
                        break;

                    case "REVISION":
                        _StringBuilder.Append("<tr><td style=\"width:250px\"><a href=\"/objects/").Append(_Type).Append("\">REVISION</a></td><td style=\"width:400px\">").Append(_Attribute.Value.ToString().EscapeForXMLandHTML()).Append("</td></tr>");
                        break;

                    case "REVISIONS":
                        _StringBuilder.Append("<tr><td style=\"width:250px\"><a href=\"/objects/").Append(_Type).Append("\">REVISIONS</a></td><td style=\"width:400px\">").Append(_Attribute.Value.ToString().EscapeForXMLandHTML()).Append("</td></tr>");
                        break;


                    default:

                        if (_Attribute.Value != null)
                        {

                            _StringBuilder.Append("<tr><td style=\"width:250px\">").Append(_Attribute.Key.ToString().EscapeForXMLandHTML()).Append("</td><td style=\"width:400px\">");

                            #region IEnumerable<Vertex>

                            _Vertex = _Attribute.Value as IEnumerable<Vertex>;

                            if (_Vertex != null && _Vertex.Count() > 0)
                            {

                                var _EdgeInfo = (_Attribute.Value as Edge);
                                var _EdgeType = (_EdgeInfo != null) ? _EdgeInfo.EdgeTypeName : "";

                                // An edgelabel for all edges together...
                                //_ListAttribute.Add(new XElement("hyperedgelabel"));

                                foreach (var _DBObjectReadout in _Vertex)
                                    _DBObjectReadout.ToHTML(true);

                                _StringBuilder.Append("</td></tr>");

                                continue;

                            }

                            #endregion

                            #region Attribute value and attribute value lists

                            _AttributeValueList = _Attribute.Value as IEnumerable<Object>;

                            if (_AttributeValueList != null)
                            {

                                _StringBuilder.Append("<table border=\"1\">");

                                foreach (var _Value in _AttributeValueList)
                                    _StringBuilder.Append("<tr><td>").Append(_Value.ToString().EscapeForXMLandHTML()).Append("</td></tr>");

                                _StringBuilder.Append("</table></td></tr>");

                                continue;

                            }

                            #endregion

                            _StringBuilder.Append(_Attribute.Value.ToString().EscapeForXMLandHTML()).Append("</td></tr>");

                        }

                        break;

                }

            }

            _StringBuilder.AppendLine("</table>");

            return _StringBuilder.ToString();

        }

        #endregion


        #region HTMLBuilder(myGraphDBName, myFunc)

        public static String HTMLBuilder(String myGraphDBName, Action<StringBuilder> myFunc)
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
            _StringBuilder.Append("<h2>").Append(myGraphDBName).AppendLine("</h2>");
            _StringBuilder.AppendLine("<table>");
            _StringBuilder.AppendLine("<tr>");
            _StringBuilder.AppendLine("<td style=\"width: 100px\">&nbsp;</td>");
            _StringBuilder.AppendLine("<td>");
            
            myFunc(_StringBuilder);

            _StringBuilder.AppendLine("</td>");
            _StringBuilder.AppendLine("</tr>");
            _StringBuilder.AppendLine("</table>");
            _StringBuilder.AppendLine("</body>").AppendLine("</html>").AppendLine();

            return _StringBuilder.ToString();

        }

        #endregion

    }

}
