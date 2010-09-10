/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

/* 
 * GEXF_IO_Extensions
 * Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

using sones.Lib;

using System.IO;
using System.Xml;
using System.Text;
using sones.GraphFS.DataStructures;
using sones.GraphDB.ObjectManagement;
using sones.GraphDBInterface.Result;
using sones.GraphDBInterface.ObjectManagement;

#endregion

namespace sones.GraphIO.GEXF
{

    /// <summary>
    /// Extension methods to transform a QueryResult and a DBObjectReadout into an
    /// application/gexf representation an vice versa.
    /// </summary>

    public static class GEXF_IO_Extensions
    {

        #region ToGEXF(this myQueryResult)

        public static XElement ToGEXF(this QueryResult myQueryResult)
        {

            // root element...
            var _graph = new XElement("graph", new XAttribute("defaultedgetype", "directed"));


            // query --------------------------------
            var _nodes = new XElement("nodes");
            var _nodeattributes = new XElement("attributes", new XAttribute("class", "node"));
            var _edges = new XElement("edges");
            var _edgeattributes = new XElement("attributes", new XAttribute("class", "edge"));

            //// result -------------------------------
            //_Query.Add(new XElement("result", myQueryResult.ResultType));

            //// duration -----------------------------
            //_Query.Add(new XElement("duration", new XAttribute("resolution", "ms"), myQueryResult.Duration));

            //// warnings -----------------------------
            //_Query.Add(new XElement("warnings",
            //    from _Warning in myQueryResult.Warnings
            //    select
            //        new XElement("warning",
            //        new XAttribute("code", _Warning.GetType().Name),
            //        _Warning.ToString())
            //    ));

            //// errors -------------------------------
            //_Query.Add(new XElement("errors",
            //    from _Error in myQueryResult.Errors
            //    select
            //        new XElement("error",
            //        new XAttribute("code", _Error.GetType().Name),
            //        _Error.ToString())
            //    ));

            //// results ------------------------------
            //_Query.Add(new XElement("results",
            //    from _SelectionListElementResult in myQueryResult.Results
            //    where _SelectionListElementResult.Objects != null
            //    select
            //        from _DBObject in _SelectionListElementResult.Objects
            //        select
            //            Export(_DBObject)
            //));

            // results ------------------------------

            var _allNodes = new HashSet<ObjectUUID>();

            //foreach (var _SelectionListElementResult in myQueryResult.Results)
            //{
            //    if (_SelectionListElementResult.Objects != null)
            //    {
            //        foreach (var _DBObject in _SelectionListElementResult.Objects)
            //        {
            //            var _UUID = (_DBObject["UUID"] != null) ? _DBObject["UUID"] : "n/a";
            //            _allNodes.Add((ObjectUUID)_UUID);
            //            _nodes.Add(new XElement("node",  new XAttribute("id", _UUID), new XAttribute("label", "node:" + _UUID)));
            //        }
            //    }
            //}

            if (myQueryResult.Results.Objects != null)
            {
                foreach (var _DBObject in myQueryResult.Results.Objects)
                {

                    var _UUID = (_DBObject["UUID"] != null) ? _DBObject["UUID"] : "n/a";
                    var _TYPE = (_DBObject["TYPE"] != null) ? _DBObject["TYPE"] : "n/a";
                    var _Name = (_DBObject["Name"] != null) ? _DBObject["Name"] : "n/a";

                    if (_UUID is ObjectUUID)
                    {
                        if (!_allNodes.Contains((ObjectUUID)_UUID))
                        {
                            _allNodes.Add((ObjectUUID)_UUID);
                            _nodes.Add(new XElement("node", new XAttribute("id", _UUID), new XAttribute("type", _TYPE), new XAttribute("label", _TYPE + "." + _Name)));
                        }
                    }
                    else
                    {
                        if (!_allNodes.Contains(new ObjectUUID((String)_UUID)))
                        {
                            _allNodes.Add(new ObjectUUID((String)_UUID));
                            _nodes.Add(new XElement("node", new XAttribute("id", _UUID), new XAttribute("type", _TYPE), new XAttribute("label", _TYPE + "." + _Name)));
                        }
                    }

                    foreach (var _Attribute in _DBObject.Attributes)
                    {
                        if (_Attribute.Value != null)
                        {
                            #region IEnumerable<DBObjectReadout>

                            var _DBObjects = _Attribute.Value as IEnumerable<DBObjectReadout>;

                            if (_DBObjects != null && _DBObjects.Count() > 0)
                            {

                                var _EdgeInfo = (_Attribute.Value as Edge);
                                var _EdgeType = (_EdgeInfo != null) ? _EdgeInfo.EdgeTypeName : "";

                                //var _ListAttribute = new XElement("edge",
                                //    new XAttribute("name", _Attribute.Key.EscapeForXMLandHTML()),
                                //    new XAttribute("type", _EdgeType));

                                // An edgelabel for all edges together...
                                //_ListAttribute.Add(new XElement("hyperedgelabel"));

                                foreach (var _DBObjectReadout in _DBObjects)
                                {

                                    var _OtherUUID = _DBObjectReadout.Attributes["UUID"];

                                    Object _OtherTYPE = "";
                                    if (_DBObjectReadout.Attributes.ContainsKey("TYPE"))
                                    {
                                        if (_DBObjectReadout.Attributes["TYPE"] is IGetName)
                                            _OtherTYPE = ((IGetName)_DBObjectReadout.Attributes["TYPE"]).Name;
                                        else
                                            _OtherTYPE = _DBObjectReadout.Attributes["TYPE"];
                                    }

                                    Object _OtherName = "";
                                    if (_DBObjectReadout.Attributes.ContainsKey("Name"))
                                        _OtherName = _DBObjectReadout.Attributes["Name"];

                                    if (!_allNodes.Contains((ObjectUUID)_OtherUUID))
                                    {
                                        _allNodes.Add((ObjectUUID)_OtherUUID);
                                        _nodes.Add(new XElement("node", new XAttribute("id", _OtherUUID), new XAttribute("type", _OtherTYPE), new XAttribute("label", _OtherTYPE + "." + _OtherName)));
                                    }

                                    //if (_allNodes.Contains(_UUID) && _allNodes.Contains(_OtherUUID))
                                    _edges.Add(new XElement("edge", new XAttribute("id", _UUID.ToString() + "->" + _OtherUUID.ToString()),
                                                                    new XAttribute("source", _UUID),
                                                                    new XAttribute("target", _OtherUUID)

                                        ));

                                }

                                //_DBObject.Add(_ListAttribute);
                                continue;

                            }

                            #endregion
                        }
                    }

                }
            }

            _graph.Add(_nodeattributes);
            _graph.Add(_edgeattributes);
            _graph.Add(_nodes);
            _graph.Add(_edges);

            return _graph;

        }

        #endregion

        #region ToGEXF(this myDBObjectReadout)

        private static XElement ToGEXF(this DBObjectReadout myDBObjectReadout)
        {
            return new GEXF_IO().ExportVertex(myDBObjectReadout) as XElement;
        }

        #endregion


        #region BuildGEXFDocument(params myXElements)

        public static XDocument BuildGEXFDocument(params XElement[] myXElements)
        {

            //<gexf xmlns="http://www.gexf.net/1.1draft"
            //      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            //      xsi:schemaLocation="http://www.gexf.net/1.1draft http://www.gexf.net/1.1draft/gexf.xsd"
            //      version="1.1">
            //    <meta lastmodifieddate="2009-03-20">
            //        <creator>Gexf.net</creator>
            //        <description>A hello world! file</description>
            //    </meta>
            //    <graph mode="static" defaultedgetype="directed">
            //     ...
            //    </graph>
            //</gexf>

            var _GEXFDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));

            var _GEXF         = new XElement("gexf",
                                        //              new XAttribute("xmlns",              "http://www.gexf.net/1.1draft"),
                                        //              new XAttribute(XNamespace.Xmlns + "xsi",          "http://www.w3.org/2001/XMLSchema-instance"),
                                        //              new XAttribute(XNamespace.Xmlns + "xsi" + "schemaLocation", "http://www.gexf.net/1.1draft http://www.gexf.net/1.1draft/gexf.xsd"),
                                                      new XAttribute("version", "1.1"));
            
            var _Meta         = new XElement("meta",  new XAttribute("lastmodifieddate", DateTime.Now),
                                                      new XElement("creator",              "sones GraphDS"),
                                                      new XElement("description",          "sones GEXF output plug-in"));
            
            var _Graph        = new XElement("graph", new XAttribute("mode", "static"), // really important?
                                                      new XAttribute("defaultedgetype", "directed"));

            foreach (var _XElement in myXElements)
                _Graph.Add(_XElement);

            _GEXF.Add(_Meta);
            _GEXF.Add(_Graph);
            _GEXFDocument.Add(_GEXF);

            return _GEXFDocument;

        }

        #endregion

        #region GEXFDocument2String(this myXDocument)

        public static String GEXFDocument2String(this XDocument myXDocument)
        {

         //   var _StringWriter = new StringWriter();

         //   var _XmlWriterSettings = new XmlWriterSettings()
         //   {
         //       Encoding         = Encoding.UTF8,
         //   //    ConformanceLevel = ConformanceLevel.Document,
         //   //    Indent           = true
         //   };

         //   var writer = XmlWriter.Create(_StringWriter, _XmlWriterSettings);
         //   myXDocument.Save(writer);

         //   return _StringWriter.GetStringBuilder().ToString();

            var _String = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" + Environment.NewLine;
            _String += myXDocument.ToString();

            return _String;

        }

        #endregion

    }

}
