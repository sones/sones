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
using sones.GraphDB.Structures.Result;
using sones.GraphDB.ObjectManagement;
using sones.Lib;
using sones.GraphFS.DataStructures;
using System.Text;

#endregion

namespace sones.GraphIO.GEXF
{

    /// <summary>
    /// Transforms a QueryResult and a DBObjectReadout into an application/gexf
    /// representation and vice versa.
    /// </summary>

    public class GEXF_IO : IGraphIO
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


        #region IGraphExport Members

        #region ExportContentType

        public ContentType ExportContentType
        {
            get
            {
                return _ExportContentType;
            }
        }

        #endregion


        #region ExportQueryResult(myQueryResult)

        public Byte[] ExportQueryResult(QueryResult myQueryResult)
        {
            return Encoding.UTF8.GetBytes(
                     GEXF_IO_Extensions.GEXFDocument2String(
                       GEXF_IO_Extensions.BuildGEXFDocument(
                         myQueryResult.ToGEXF()
                       )
                     )
                   );
        }

        #endregion


        #region ExportVertex(myDBObjectReadout)

        public Object ExportVertex(DBObjectReadout myDBObjectReadout)
        {
            return Export(myDBObjectReadout, false);
        }

        #endregion

        #region (private) Export(myDBObjectReadout, myRecursion)

        private Object Export(DBObjectReadout myDBObjectReadout, Boolean myRecursion)
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

                        if (_GroupedDBObjects.GrouppedVertices != null)
                            foreach (var _DBObjectReadout in _GroupedDBObjects.GrouppedVertices)
                                _Grouped.Add(ExportVertex(_DBObjectReadout));

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
                            _ListAttribute.Add(ExportVertex(_DBObjectReadout));

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

        #endregion


        #region IGraphImport Members

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
