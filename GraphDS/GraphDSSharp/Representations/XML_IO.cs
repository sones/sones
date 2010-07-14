/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/

/* 
 * XML_IO
 * Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Net.Mime;
using System.Xml.Linq;
using System.Collections.Generic;

using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Result;

using sones.Lib;
using sones.GraphDB.ObjectManagement;

#endregion

namespace sones.GraphDS.API.CSharp
{

    /// <summary>
    /// Transforms a QueryResult and a DBObjectReadout into an application/xml
    /// representation and vice versa.
    /// </summary>

    public class XML_IO : IDBExport, IDBImport
    {

        #region Data

        private readonly ContentType _ExportContentType;
        private readonly ContentType _ImportContentType;

        #endregion

        #region Constructor

        public XML_IO()
        {
            _ExportContentType = new ContentType("application/xml") { CharSet = "UTF-8" };
            _ImportContentType = new ContentType("application/xml");
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
            var _Query = new XElement("queryresult", new XAttribute("version", "1.0"));


            // query --------------------------------
            _Query.Add(new XElement("query", myQueryResult.Query));

            // result -------------------------------
            _Query.Add(new XElement("result", myQueryResult.ResultType));

            // duration -----------------------------
            _Query.Add(new XElement("duration", new XAttribute("resolution", "ms"), myQueryResult.Duration));

            // warnings -----------------------------
            _Query.Add(new XElement("warnings",
                from _Warning in myQueryResult.Warnings
                select
                    new XElement("warning",
                    new XAttribute("code", _Warning.GetType().Name),
                    _Warning.ToString())
                ));

            // errors -------------------------------
            _Query.Add(new XElement("errors",
                from _Error in myQueryResult.Errors
                select
                    new XElement("error",
                    new XAttribute("code", _Error.GetType().Name),
                    _Error.ToString())
                ));

            // results ------------------------------
            _Query.Add(new XElement("results",
                from _SelectionListElementResult in myQueryResult.Results
                where _SelectionListElementResult.Objects != null
                select
                    from _DBObject in _SelectionListElementResult.Objects
                    select
                        Export(_DBObject)
            ));

            return _Query;

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
            return XML_IO_Extensions.BuildXMLDocument(Export(myQueryResult) as XElement).XDocument2String();
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
