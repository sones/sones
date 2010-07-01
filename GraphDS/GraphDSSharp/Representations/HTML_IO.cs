/* 
 * HTML_IO
 * Achim 'ahzf' Friedland, 2009-2010
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Net.Mime;
using System.Collections.Generic;

using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Result;

using sones.Lib;

#endregion

namespace sones.GraphDS.API.CSharp
{

    /// <summary>
    /// Transforms a QueryResult and a DBObjectReadout into a text/html representation
    /// </summary>

    public class HTML_IO : IDBExport
    {

        #region Data

        private readonly ContentType _ExportContentType;

        #endregion

        #region Constructor

        public HTML_IO()
        {
            _ExportContentType = new ContentType("text/html") { CharSet = "UTF-8" };
        }

        #endregion


        #region IDBExport Members

        #region ExportContentType

        public ContentType ExportContentType
        {
            get
            {
                return _ExportContentType;
            }
        }

        #endregion


        #region Export(myQueryResult)

        public Object Export(QueryResult myQueryResult)
        {
            return Export(myQueryResult, new StringBuilder());
        }

        #endregion

        #region Export(myQueryResult, myStringBuilder)

        public Object Export(QueryResult myQueryResult, StringBuilder myStringBuilder)
        {

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
            foreach (var _SelectionListElementResult in myQueryResult.Results)
                if (_SelectionListElementResult.Objects != null)
                    foreach (var _DBObject in _SelectionListElementResult.Objects)
                        myStringBuilder.Append("<tr><td>" + (String) Export(_DBObject, myStringBuilder) + "</td></tr>");
            myStringBuilder.AppendLine("</table></td></tr>");

            myStringBuilder.AppendLine("</table>");

            return myStringBuilder;

        }

        #endregion


        #region Export(myDBObjectReadout)

        public Object Export(DBObjectReadout myDBObjectReadout)
        {
            return Export(myDBObjectReadout, false);
        }

        #endregion

        #region Export(myDBObjectReadout, myRecursion = false)

        public Object Export(DBObjectReadout myDBObjectReadout, Boolean myRecursion)
        {
            return Export(myDBObjectReadout, new StringBuilder(), myRecursion);
        }

        #endregion

        #region Export(myDBObjectReadout, myStringBuilder, myRecursion = false)

        public object Export(DBObjectReadout myDBObjectReadout, StringBuilder myStringBuilder, Boolean myRecursion = false)
        {

            var    _TypeObject  = myDBObjectReadout["TYPE"] as GraphDBType;
            String _Type        = (_TypeObject != null) ? _TypeObject.Name : "";
            var    _UUID        = myDBObjectReadout["UUID"];

            IEnumerable<DBObjectReadout> _DBObjects             = null;
            IEnumerable<Object>          _AttributeValueList    = null;

            myStringBuilder.AppendLine("<table class=\"gql_table\" border=\"1\">");

            #region DBWeightedObjectReadout

            var _WeightedDBObject1 = myDBObjectReadout as DBWeightedObjectReadout;

            if (_WeightedDBObject1 != null)
            {
                myStringBuilder.Append("<tr><td style=\"width:250px\">edgelabel</td><td style=\"width:400px\">");
                myStringBuilder.Append("<table border=\"1\"><tr><td>weight</td><td>" + _WeightedDBObject1.Weight.Type.ToString().EscapeForXMLandHTML() + "</td><td>" + _WeightedDBObject1.Weight.ToString().EscapeForXMLandHTML() + "</td></tr></table>");
                myStringBuilder.Append("</td></tr>");
            }

            #endregion

            foreach (var _Attribute in myDBObjectReadout.Attributes)
            {

                switch (_Attribute.Key)
                {

                    case "TYPE":
                    case "Type":
                        var    _ThisTypeObject  = _Attribute.Value as GraphDBType;
                        String _ThisType        = (_ThisTypeObject != null) ? _ThisTypeObject.Name : "";
                        myStringBuilder.Append("<tr><td style=\"width:250px\"><a href=\"/objects/\">TYPE</a></td><td style=\"width:400px\">").Append(_ThisType.EscapeForXMLandHTML()).Append("</td></tr>");
                        break;

                    case "UUID":
                        myStringBuilder.Append("<tr><td style=\"width:250px\"><a href=\"/objects/").Append(_Type).Append("/").Append(_UUID.ToString().EscapeForXMLandHTML()).Append("/\">UUID</a></td><td style=\"width:400px\">").Append(_Attribute.Value.ToString().EscapeForXMLandHTML()).Append("</td></tr>");
                        break;

                    case "EDITION":
                        myStringBuilder.Append("<tr><td style=\"width:250px\"><a href=\"/objects/").Append(_Type).Append("\">EDITION</a></td><td style=\"width:400px\">").Append(_Attribute.Value.ToString().EscapeForXMLandHTML()).Append("</td></tr>");
                        break;

                    case "EDITIONS":
                        myStringBuilder.Append("<tr><td style=\"width:250px\"><a href=\"/objects/").Append(_Type).Append("\">EDITIONS</a></td><td style=\"width:400px\">").Append(_Attribute.Value.ToString().EscapeForXMLandHTML()).Append("</td></tr>");
                        break;

                    case "REVISION":
                        myStringBuilder.Append("<tr><td style=\"width:250px\"><a href=\"/objects/").Append(_Type).Append("\">REVISION</a></td><td style=\"width:400px\">").Append(_Attribute.Value.ToString().EscapeForXMLandHTML()).Append("</td></tr>");
                        break;

                    case "REVISIONS":
                        myStringBuilder.Append("<tr><td style=\"width:250px\"><a href=\"/objects/").Append(_Type).Append("\">REVISIONS</a></td><td style=\"width:400px\">").Append(_Attribute.Value.ToString().EscapeForXMLandHTML()).Append("</td></tr>");
                        break;


                    default:

                        if (_Attribute.Value != null)
                        {

                            myStringBuilder.Append("<tr><td style=\"width:250px\">").Append(_Attribute.Key.ToString().EscapeForXMLandHTML()).Append("</td><td style=\"width:400px\">");

                            #region IEnumerable<DBObjectReadout>

                            _DBObjects = _Attribute.Value as IEnumerable<DBObjectReadout>;

                            if (_DBObjects != null && _DBObjects.Count() > 0)
                            {

                                var _EdgeInfo = (_Attribute.Value as Edge);
                                var _EdgeType = (_EdgeInfo != null) ? _EdgeInfo.EdgeTypeName : "";                                

                                // An edgelabel for all edges together...
                                //_ListAttribute.Add(new XElement("hyperedgelabel"));

                                foreach (var _DBObjectReadout in _DBObjects)
                                    Export(_DBObjectReadout, myStringBuilder);

                                myStringBuilder.Append("</td></tr>");

                                continue;

                            }

                            #endregion

                            #region Attribute value and attribute value lists

                            _AttributeValueList = _Attribute.Value as IEnumerable<Object>;

                            if (_AttributeValueList != null)
                            {

                                myStringBuilder.Append("<table border=\"1\">");

                                foreach (var _Value in _AttributeValueList)
                                    myStringBuilder.Append("<tr><td>").Append(_Value.ToString().EscapeForXMLandHTML()).Append("</td></tr>");

                                myStringBuilder.Append("</table></td></tr>");

                                continue;

                            }

                            #endregion

                            myStringBuilder.Append(_Attribute.Value.ToString().EscapeForXMLandHTML()).Append("</td></tr>");

                        }

                        break;

                }

            }

            myStringBuilder.AppendLine("</table>");

            return "";

        }

        #endregion


        #region ExportString(myQueryResult)

        public String ExportString(QueryResult myQueryResult)
        {
            
            var _StringBuilder = new StringBuilder();
            
            Export(myQueryResult, _StringBuilder);

            return _StringBuilder.ToString();

        }

        #endregion

        #endregion

    }

}
