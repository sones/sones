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
 * JSON_IO
 * Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Net.Mime;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Result;

#endregion

namespace sones.GraphDS.API.CSharp
{

    /// <summary>
    /// Transforms a QueryResult and a DBObjectReadout into an application/json
    /// representation and vice versa.
    /// </summary>

    public class JSON_IO : IDBExport, IDBImport
    {

        #region Data

        private readonly ContentType _ExportContentType;
        private readonly ContentType _ImportContentType;

        #endregion

        #region Constructor

        public JSON_IO()
        {
            _ExportContentType = new ContentType("application/json") { CharSet = "UTF-8" };
            _ImportContentType = new ContentType("application/json");
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
            var _Query = new JObject();


            // query --------------------------------
            _Query.Add(new JProperty("query", myQueryResult.Query));

            // result -------------------------------
            _Query.Add(new JProperty("result", myQueryResult.ResultType.ToString()));

            // duration -----------------------------
            _Query.Add(new JProperty("duration", new JArray(myQueryResult.Duration, "ms")));

            // warnings -----------------------------
            _Query.Add(new JProperty("warnings", new JArray(
                from _Warning in myQueryResult.Warnings
                select new JObject(
                         new JProperty("code", _Warning.GetType().ToString()),
                         new JProperty("description", _Warning.ToString())
                       ))));

            // errors -------------------------------
            _Query.Add(new JProperty("errors", new JArray(
                from _Error in myQueryResult.Errors
                select new JObject(
                         new JProperty("code", _Error.GetType().ToString()),
                         new JProperty("description", _Error.ToString())
                       ))));

            // results ------------------------------
            _Query.Add(new JProperty("results", new JArray(
                from _SelectionListElementResult in myQueryResult.Results
                where _SelectionListElementResult.Objects != null
                select
                    from _DBObjectReadout in _SelectionListElementResult.Objects
                    select Export(_DBObjectReadout)
                )));

            return _Query;

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

            var _DBObject = new JObject();

            DBObjectReadoutGroup _GroupedDBObjects = null;
            DBWeightedObjectReadout _WeightedDBObject = null;
            IEnumerable<DBObjectReadout> _DBObjects = null;
            IEnumerable<Object> _AttributeValueList = null;

            #region DBWeightedObjectReadout

            var _WeightedDBObject1 = myDBObjectReadout as DBWeightedObjectReadout;
            if (_WeightedDBObject1 != null)
            {
                _DBObject.Add(new JProperty("edgelabel", new JObject(new JProperty("weight", _WeightedDBObject1.Weight.ToString()))));
            }

            #endregion

            #region Attributes

            JObject _Attributes;

            if (myRecursion)
            {
                _Attributes = new JObject();
                _DBObject.Add(new JProperty("attributes", _Attributes));
            }
            else
                _Attributes = _DBObject;

            #endregion

            foreach (var _Attribute in myDBObjectReadout.Attributes)
            {

                if (_Attribute.Value != null)
                {

                    #region DBObjectReadoutGroup

                    _GroupedDBObjects = _Attribute.Value as DBObjectReadoutGroup;

                    if (_GroupedDBObjects != null)
                    {

                        var _Grouped = new JArray("grouped");

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
                        _DBObject.Add(new JProperty("strange-edgelabel", new JObject(new JProperty("weight", _WeightedDBObject1.Weight.ToString()))));
                    }

                    #endregion

                    #region IEnumerable<DBObjectReadout>

                    _DBObjects = _Attribute.Value as IEnumerable<DBObjectReadout>;

                    if (_DBObjects != null && _DBObjects.Count() > 0)
                    {

                        _Attributes.Add(
                            new JProperty(_Attribute.Key, new JObject(

                                // An edgelabel for all edges together...
                                new JProperty("hyperedgelabel", new JObject()),

                                new JProperty("DBObjects", new JArray(
                                    from _DBObjectReadout in _DBObjects
                                    select Export(_DBObjectReadout, true)
                                ))

                            )
                        ));

                        continue;

                    }

                    #endregion

                    #region IEnumerable<Object>

                    _AttributeValueList = _Attribute.Value as IEnumerable<Object>;

                    if (_AttributeValueList != null)
                    {

                        JArray array = new JArray();

                        foreach (var item in _AttributeValueList)
                        {
                            if (!(item is GraphDBType))
                                array.Add(item);
                        }

                        if (_AttributeValueList.Count() > 0)
                        {
                            if (_AttributeValueList.First() is GraphDBType)
                            {
                                _Attributes.Add(new JProperty(_Attribute.Key, ((GraphDBType)_AttributeValueList.First()).Name + " []"));
                            }
                            else
                                _Attributes.Add(new JProperty(_Attribute.Key, array));
                        }
                        else
                            _Attributes.Add(new JProperty(_Attribute.Key, new JArray()));

                        continue;

                    }

                    #endregion

                    #region Attribute Value

                    if (_Attribute.Value is GraphDBType)
                        _Attributes.Add(new JProperty(_Attribute.Key, ((GraphDBType)_Attribute.Value).Name));
                    else if(_Attribute.Value is TypeAttribute)
                        _Attributes.Add(new JProperty(_Attribute.Key, ((TypeAttribute)_Attribute.Value).Name));
                    else
                        _Attributes.Add(new JProperty(_Attribute.Key, _Attribute.Value.ToString()));

                    #endregion

                }

            }

            return _DBObject;

        }

        #endregion

        #region ExportString(myQueryResult)

        public String ExportString(QueryResult myQueryResult)
        {
            return (Export(myQueryResult) as JObject).ToString(Formatting.Indented);
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
