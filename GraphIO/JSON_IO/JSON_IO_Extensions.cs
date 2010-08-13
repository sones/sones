/* 
 * JSON_IO_Extensions
 * Achim 'ahzf' Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;

using Newtonsoft.Json.Linq;

using sones.GraphDB.Structures.Result;
using System.Collections.Generic;
using sones.GraphDB.ObjectManagement;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphIO.JSON
{

    /// <summary>
    /// Extension methods to transform a QueryResult and a DBObjectReadout into an
    /// application/json representation an vice versa.
    /// </summary>

    public static class JSON_IO_Extensions
    {

        #region ToJSON(this myINode)

        public static JObject ToJSON(this INode myINode)
        {

            return
                new JObject("INode",
                    new JProperty("Version",                    myINode.StructureVersion),
                    new JProperty("ObjectUUID",                 myINode.ObjectUUID),

                    new JProperty("CreationTime",               myINode.CreationTime),
                    new JProperty("LastAccessTime",             myINode.LastAccessTime),
                    new JProperty("LastModificationTime",       myINode.LastModificationTime),
                    new JProperty("DeletionTime",               myINode.DeletionTime),
                    new JProperty("ReferenceCount",             myINode.ReferenceCount),
                    new JProperty("ObjectSize",                 myINode.ObjectSize),
                    new JProperty("IntegrityCheckAlgorithm",    myINode.IntegrityCheckAlgorithm),
                    new JProperty("EncryptionAlgorithm",        myINode.EncryptionAlgorithm),

                    new JProperty("ObjectLocatorPosition", new JArray(
                        new JProperty("Length", myINode.ObjectLocatorLength),
                        new JProperty("NumberOfCopies", myINode.ObjectLocatorCopies),

                        from _ExtendedPosition in myINode.ObjectLocatorPositions
                        select new JProperty("ExtendedPosition", new JArray(
                            new JProperty("StorageID", _ExtendedPosition.StorageUUID),
                            new JProperty("Position", _ExtendedPosition.Position)
                        ))
                    ))

                );

        }

        #endregion


        #region ToJSON(this myQueryResult)

        public static JObject ToJSON(this QueryResult myQueryResult)
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
                    select _DBObjectReadout.ToJSON()
                )));

            return _Query;

        }

        #endregion

        #region ToJSON(this myDBObjectReadout)

        public static JObject ToJSON(this DBObjectReadout myDBObjectReadout)
        {
            return myDBObjectReadout.ToJSON(false);
        }

        #endregion

        #region (private) ToJSON(this myDBObjectReadout, myRecursion = false)

        private static JObject ToJSON(this DBObjectReadout myDBObjectReadout, Boolean myRecursion = false)
        {

            var _DBObject = new JObject();

            DBObjectReadoutGroup         _GroupedDBObjects   = null;
            DBWeightedObjectReadout      _WeightedDBObject   = null;
            IEnumerable<DBObjectReadout> _DBObjects          = null;
            IEnumerable<Object>          _AttributeValueList = null;
            IGetName                     _IGetName           = null;

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

                        if (_GroupedDBObjects.GrouppedVertices != null)
                            foreach (var _DBObjectReadout in _GroupedDBObjects.GrouppedVertices)
                                _Grouped.Add(_DBObjectReadout.ToJSON());

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
                                    select _DBObjectReadout.ToJSON(true)
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

                        var _JArray = new JArray();

                        foreach (var item in _AttributeValueList)
                        {

                            // item.ToString() may not always return the information we need!
                            _IGetName = item as IGetName;

                            if (_IGetName != null)
                                _JArray.Add(_IGetName.Name);
                            else
                                _JArray.Add(item.ToString());

                        }

                        _Attributes.Add(new JProperty(_Attribute.Key, _JArray));

                        continue;

                    }

                    #endregion

                    #region Attribute Value

                    // _Attribute.Value.ToString() may not always return the information we need!
                    _IGetName = _Attribute.Value as IGetName;

                    if (_IGetName != null)
                        _Attributes.Add(new JProperty(_Attribute.Key, _IGetName.Name));
                    else
                        _Attributes.Add(new JProperty(_Attribute.Key, _Attribute.Value.ToString()));

                    #endregion

                }

            }

            return _DBObject;

        }

        #endregion

    }

}
