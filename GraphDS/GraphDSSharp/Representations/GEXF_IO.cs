/* 
 * GEXF_IO
 * Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Linq;
using System.Net.Mime;
using System.Xml.Linq;
using System.Collections.Generic;

using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.ObjectManagement;
using sones.Lib;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphDS.API.CSharp
{

    /// <summary>
    /// Transforms a QueryResult and a DBObjectReadout into an application/gexf
    /// representation and vice versa.
    /// </summary>

    public class GEXF_IO : IDBExport, IDBImport
    {

        #region Data

        private readonly ContentType _ExportContentType;
        private readonly ContentType _ImportContentType;

        #endregion

        #region Constructor

        public GEXF_IO()
        {
            _ExportContentType = new ContentType("application/gexf") { CharSet = "UTF-8" };
            _ImportContentType = new ContentType("application/gexf");
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

            // root element...
            var _graph = new XElement("graph", new XAttribute("defaultedgetype", "directed"));


            // query --------------------------------
            var _nodes          = new XElement("nodes");
            var _nodeattributes = new XElement("attributes", new XAttribute("class", "node"));
            var _edges          = new XElement("edges");
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

            foreach (var _SelectionListElementResult in myQueryResult.Results)
            {
                if (_SelectionListElementResult.Objects != null)
                {
                    foreach (var _DBObject in _SelectionListElementResult.Objects)
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
            }

            _graph.Add(_nodeattributes);
            _graph.Add(_edgeattributes);
            _graph.Add(_nodes);
            _graph.Add(_edges);

            return _graph;

        }

        #endregion


        #region Export(myDBObjectReadout)

        public Object Export(DBObjectReadout myDBObjectReadout)
        {
            return Export(myDBObjectReadout, false);
        }

        #endregion

        #region Export(myDBObjectReadout, myRecursion)

        public Object Export(DBObjectReadout myDBObjectReadout, Boolean myRecursion)
        {

            //<?xml version="1.0" encoding="UTF-8"?>
            //<gexf xmlns="http://www.gexf.net/1.1draft"
            //      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            //      xsi:schemaLocation="http://www.gexf.net/1.1draft http://www.gexf.net/1.1draft/gexf.xsd"
            //      version="1.1">
            //    <meta lastmodifieddate="2009-03-20">
            //        <creator>Gephi.org</creator>
            //        <description>A Web network</description>
            //    </meta>
            //    <graph defaultedgetype="directed">
            //        <attributes class="node">
            //            <attribute id="0" title="url" type="string"/>
            //            <attribute id="1" title="indegree" type="float"/>
            //            <attribute id="2" title="frog" type="boolean">
            //                <default>true</default>
            //            </attribute>
            //        </attributes>
            //        <nodes>
            //            <node id="0" label="Gephi">
            //                <attvalues>
            //                    <attvalue for="0" value="http://gephi.org"/>
            //                    <attvalue for="1" value="1"/>
            //                </attvalues>
            //            </node>
            //            <node id="1" label="Webatlas">
            //                <attvalues>
            //                    <attvalue for="0" value="http://webatlas.fr"/>
            //                    <attvalue for="1" value="2"/>
            //                </attvalues>
            //            </node>
            //            <node id="2" label="RTGI">
            //                <attvalues>
            //                    <attvalue for="0" value="http://rtgi.fr"/>
            //                    <attvalue for="1" value="1"/>
            //                </attvalues>
            //            </node>
            //            <node id="3" label="BarabasiLab">
            //                <attvalues>
            //                    <attvalue for="0" value="http://barabasilab.com"/>
            //                    <attvalue for="1" value="1"/>
            //                    <attvalue for="2" value="false"/>
            //                </attvalues>
            //            </node>
            //        </nodes>
            //        <edges>
            //            <edge id="0" source="0" target="1"/>
            //            <edge id="1" source="0" target="2"/>
            //            <edge id="2" source="1" target="0"/>
            //            <edge id="3" source="2" target="1"/>
            //            <edge id="4" source="0" target="3"/>
            //        </edges>
            //    </graph>
            //</gexf>




            //<?xml version="1.0"?>
            //<gexf version="1.1" xmlns="http://www.gexf.net/1.1draft">
  
            //  <graph defaultedgetype="directed" mode="static">
    
            //    <attributes class="node">
            //      <attribute id="0" title="url"      type="string"></attribute>
            //      <attribute id="1" title="indegree" type="string"></attribute>
            //      <attribute id="2" title="frog"     type="string"></attribute>
            //    </attributes>
	
            //    <attributes class="edge"></attributes>
	
            //    <nodes>
	  
            //      <node id="3" label="BarabasiLab">
            //        <attvalues>
            //          <attvalue for="0" value="http://barabasilab.com"></attvalue>
            //          <attvalue for="1" value="1"></attvalue>
            //          <attvalue for="2" value="false"></attvalue>
            //        </attvalues>
            //      </node>
	  
            //      <node id="2" label="RTGI">
            //        <attvalues>
            //          <attvalue for="0" value="http://rtgi.fr"></attvalue>
            //          <attvalue for="1" value="1"></attvalue>
            //        </attvalues>
            //      </node>
	  
            //      <node id="1" label="Webatlas">
            //        <attvalues>
            //          <attvalue for="0" value="http://webatlas.fr"></attvalue>
            //          <attvalue for="1" value="2"></attvalue>
            //        </attvalues>
            //      </node>
	  
            //      <node id="0" label="Gephi">
            //        <attvalues>
            //          <attvalue for="0" value="http://gephi.org"></attvalue>
            //          <attvalue for="1" value="1"></attvalue>
            //        </attvalues>
            //      </node>
	  
            //    </nodes>
	
            //    <edges>
            //      <edge id="3" label="" source="2" target="1" weight="1.0"></edge>
            //      <edge id="2" label="" source="1" target="0" weight="1.0"></edge>
            //      <edge id="0" label="" source="0" target="1" weight="1.0"></edge>
            //      <edge id="1" label="" source="0" target="2" weight="1.0"></edge>
            //      <edge id="4" label="" source="0" target="3" weight="1.0"></edge>
            //    </edges>
	
            //  </graph>
  
            //</gexf>



            Type _AttributeType         = null;
            var  _AttributeTypeString   = "";
            var  _DBObject              = new XElement("DBObject");

            DBObjectReadoutGroup         _GroupedDBObjects      = null;
            DBWeightedObjectReadout      _WeightedDBObject      = null;
            IEnumerable<DBObjectReadout> _DBObjects             = null;
            IEnumerable<Object>          _AttributeValueList    = null;
            IGetName                     _IGetName              = null;

            #region DBWeightedObjectReadout

            var _WeightedDBObject1 = myDBObjectReadout as DBWeightedObjectReadout;

            if (_WeightedDBObject1 != null)
            {
                _DBObject.Add(new XElement("edgelabel", new XElement("attribute", new XAttribute("name", "weight"), new XAttribute("type", _WeightedDBObject1.Weight.Type.ToString()), _WeightedDBObject1.Weight)));
            }

            #endregion

            foreach (var _Attribute in myDBObjectReadout.Attributes)
            {

                if (_Attribute.Value != null)
                {

                    #region DBObjectReadoutGroup

                    _GroupedDBObjects = _Attribute.Value as DBObjectReadoutGroup;

                    if (_GroupedDBObjects != null)
                    {

                        var _Grouped = new XElement("grouped");

                        if (_GroupedDBObjects.CorrespondingDBObjects != null)
                            foreach (var _DBObjectReadout in _GroupedDBObjects.CorrespondingDBObjects)
                                _Grouped.Add(Export(_DBObjectReadout));

                        _DBObject.Add(_Grouped);

                        continue;

                    }

                    #endregion

                    #region DBWeightedObjectReadout

                    _WeightedDBObject = _Attribute.Value as DBWeightedObjectReadout;

                    if (_WeightedDBObject != null)
                    {
                        _DBObject.Add(new XElement("edgelabel", new XElement("attribute", new XAttribute("name", "weight"), new XAttribute("type", _WeightedDBObject1.Weight.Type.ToString()), _WeightedDBObject1.Weight)));
                        continue;
                    }

                    #endregion

                    #region IEnumerable<DBObjectReadout>

                    _DBObjects = _Attribute.Value as IEnumerable<DBObjectReadout>;

                    if (_DBObjects != null && _DBObjects.Count() > 0)
                    {

                        var _EdgeInfo = (_Attribute.Value as Edge);
                        var _EdgeType = (_EdgeInfo != null) ? _EdgeInfo.EdgeTypeName : "";

                        var _ListAttribute = new XElement("edge",
                            new XAttribute("name", _Attribute.Key.EscapeForXMLandHTML()),
                            new XAttribute("type", _EdgeType));

                        // An edgelabel for all edges together...
                        _ListAttribute.Add(new XElement("hyperedgelabel"));

                        foreach (var _DBObjectReadout in _DBObjects)
                            _ListAttribute.Add(Export(_DBObjectReadout));

                        _DBObject.Add(_ListAttribute);
                        continue;

                    }

                    #endregion

                    #region Attribute Type (may be generic!)

                    _AttributeType = _Attribute.Value.GetType();

                    if (_AttributeType.IsGenericType)
                    {
                        _AttributeTypeString = _AttributeType.Name;
                        _AttributeTypeString = _AttributeTypeString.Substring(0, _AttributeTypeString.IndexOf('`')).ToUpper();
                        _AttributeTypeString += "&lt;";
                        _AttributeTypeString += _AttributeType.GetGenericArguments()[0].Name;
                        _AttributeTypeString += "&gt;";
                    }

                    else
                        _AttributeTypeString = _AttributeType.Name;

                    #endregion

                    #region Add result to _DBObject

                    var _AttributeTag = new XElement("attribute",
                        new XAttribute("name", _Attribute.Key.EscapeForXMLandHTML()),
                        new XAttribute("type", _AttributeTypeString)
                    );

                    _DBObject.Add(_AttributeTag);

                    #endregion

                    #region Attribute value and attribute value lists

                    _AttributeValueList = _Attribute.Value as IEnumerable<Object>;

                    if (_AttributeValueList != null)
                    {
                        foreach (var _Value in _AttributeValueList)
                        {

                            // _Value.ToString() may not always return the information we need!
                            _IGetName = _Value as IGetName;
                            if (_IGetName != null)
                                _AttributeTag.Add(new XElement("item", _IGetName.Name));
                            else
                                _AttributeTag.Add(new XElement("item", _Value.ToString().EscapeForXMLandHTML()));

                        }
                    }

                    else
                    {

                        // _Attribute.Value.ToString() may not always return the information we need!
                        _IGetName = _Attribute.Value as IGetName;

                        if (_IGetName != null)
                            _AttributeTag.Value = _IGetName.Name;
                        else
                            _AttributeTag.Value = _Attribute.Value.ToString().EscapeForXMLandHTML();

                    }

                    #endregion

                }

            }

            return _DBObject;

        }

        #endregion

        #region ExportString(myQueryResult)

        public String ExportString(QueryResult myQueryResult)
        {
            return GEXF_IO_Extensions.BuildGEXFDocument(Export(myQueryResult) as XElement).GEXFDocument2String();
        }

        #endregion

        #endregion


        #region IDBImport Members

        #region ImportContentType

        public ContentType ImportContentType
        {
            get
            {
                return _ImportContentType;
            }
        }

        #endregion

        #region ParseQueryResult(myInput)

        public QueryResult ParseQueryResult(String myInput)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ParseDBObject(myInput)

        public DBObjectReadout ParseDBObject(String myInput)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

    }

}
