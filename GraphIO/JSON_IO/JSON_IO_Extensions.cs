/* 
 * JSON_IO_Extensions
 * Achim 'ahzf' Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;

using Newtonsoft.Json.Linq;


using System.Collections.Generic;
using sones.GraphDB.ObjectManagement;
using sones.GraphFS.DataStructures;
using sones.GraphDB.Result;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.NewAPI;

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
            _Query.Add(new JProperty("results", new JArray(GetJObjectsFromResult(myQueryResult.Vertices))));

            return _Query;

        }

        #endregion

        private static IEnumerable<JObject> GetJObjectsFromResult(IEnumerable<Vertex> myResultSet)
        {
            if (myResultSet != null)
            {
                foreach (var aXElement in from aReadout in myResultSet select aReadout.ToJSON())
                {
                    yield return aXElement;
                }
            }

            yield break;
        }

        #region ToJSON(this myVertex)

        public static JObject ToJSON(this Vertex myVertex)
        {
            return myVertex.ToJSON(false);
        }

        #endregion

        #region (private) ToJSON(this myVertex, myRecursion = false)

        private static JObject ToJSON(this Vertex myVertex, Boolean myRecursion = false)
        {

            var _Vertex = new JObject();

            VertexGroup             _GroupedVertices   = null;
            Vertex_WeightedEdges    _WeightedDBObject   = null;
            IEnumerable<Vertex>     _Vertices           = null;
            IEnumerable<Object>     _AttributeValueList = null;
            IGetName                _IGetName           = null;

            #region Vertex_WeightedEdges

            var _WeightedDBObject1 = myVertex as Vertex_WeightedEdges;
            if (_WeightedDBObject1 != null)
            {
                _Vertex.Add(new JProperty("edgelabel", new JObject(new JProperty("weight", _WeightedDBObject1.Weight.ToString()))));
            }

            #endregion

            #region Attributes

            JObject _Attributes;

            if (myRecursion)
            {
                _Attributes = new JObject();
                _Vertex.Add(new JProperty("attributes", _Attributes));
            }
            else
                _Attributes = _Vertex;

            #endregion

            foreach (var _Attribute in myVertex.ObsoleteAttributes)
            {

                if (_Attribute.Value != null)
                {

                    #region VertexGroup

                    _GroupedVertices = _Attribute.Value as VertexGroup;

                    if (_GroupedVertices != null)
                    {

                        var _Grouped = new JArray("grouped");

                        if (_GroupedVertices.GroupedVertices != null)
                            foreach (var __Vertex in _GroupedVertices.GroupedVertices)
                                _Grouped.Add(__Vertex.ToJSON());

                        _Vertex.Add(_Grouped);

                        continue;

                    }

                    #endregion

                    #region Vertex_WeightedEdges

                    _WeightedDBObject = _Attribute.Value as Vertex_WeightedEdges;

                    if (_WeightedDBObject != null)
                    {
                        _Vertex.Add(new JProperty("strange-edgelabel", new JObject(new JProperty("weight", _WeightedDBObject1.Weight.ToString()))));
                    }

                    #endregion

                    #region IEnumerable<Vertex>

                    _Vertices = _Attribute.Value as IEnumerable<Vertex>;

                    if (_Vertices != null && _Vertices.Count() > 0)
                    {

                        _Attributes.Add(
                            new JProperty(_Attribute.Key, new JObject(

                                // An edgelabel for all edges together...
                                new JProperty("hyperedgelabel", new JObject()),

                                new JProperty("Vertices", new JArray(
                                    from __Vertex in _Vertices
                                    select __Vertex.ToJSON(true)
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

            return _Vertex;

        }

        #endregion

    }

}
