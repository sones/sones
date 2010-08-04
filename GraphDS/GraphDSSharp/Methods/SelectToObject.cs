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

/* SelectToObject
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphDS.API.CSharp.Reflection;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphDS.API.CSharp
{

    /// <summary>
    /// Use the result of the GraphDB to create a ObjectGraph
    /// </summary>
    public class SelectToObjectGraph
    {

        #region Data

        private readonly QueryResult    _QueryResult;
        private readonly XDocument      _XMLQueryResult;
        private readonly String         UUIDName        = SpecialTypeAttribute_UUID.AttributeName;
        private readonly XElement       _QueryResultElement;
        private readonly Dictionary<Type, Dictionary<String, Object>> _VisitedVerticesCache;

        #endregion

        #region Properties

        #region Query

        public String Query { get; private set; }

        #endregion

        #region QueryResult

        public ResultType QueryResult { get; private set; }

        #endregion

        #region Duration

        public UInt64 Duration { get; private set; }

        #endregion


        #region Warnings

        private readonly List<String> _Warnings;

        public IEnumerable<String> Warnings
        {
            get
            {
                return _Warnings;
            }
        }

        #endregion

        #region Errors

        private readonly List<String> _Errors;

        public IEnumerable<String> Errors
        {
            get
            {
                return _Errors;
            }
        }

        #endregion


        #region Failed

        public Boolean Failed
        {
            get
            {
                return (_Errors != null && _Errors.Count > 0);
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region SelectToObjectGraph(myQueryResult)

        public SelectToObjectGraph(QueryResult myQueryResult)
            : this(new XML_IO().ExportString(myQueryResult))
        {
            _QueryResult        = myQueryResult;
        }

        #endregion

        #region SelectToObjectGraph(myXMLQueryResult)

        public SelectToObjectGraph(String myXMLQueryResult)
        {

            #region Init

            _Warnings               = new List<String>();
            _Errors                 = new List<String>();
            _VisitedVerticesCache   = new Dictionary<Type, Dictionary<String, Object>>();

            #endregion

            #region Set QueryResult data

            _XMLQueryResult     = XDocument.Parse(myXMLQueryResult, LoadOptions.None);
            _QueryResultElement = _XMLQueryResult.Element("sones").Element("GraphDB").Element("queryresult");

            Query               = _QueryResultElement.Element("query").Value;
            QueryResult         = (ResultType) Enum.Parse(typeof(ResultType), _QueryResultElement.Element("result").Value);
            Duration            = UInt64.Parse(_QueryResultElement.Element("duration").Value);

            // Warnings
            var warnings = _QueryResultElement.Element("warnings").Elements("warning");
            if (warnings != null)
                foreach (var _warning in warnings)
                    _Warnings.Add(String.Format("[{0}]: {1}", _warning.Attribute("error code").Value, _warning.Value));

            // Errors
            var errors = _QueryResultElement.Element("errors").Elements("error");
            if (errors != null)
                foreach (var _error in errors)
                    _Errors.Add(String.Format("[{0}]: {1}", _error.Attribute("error code").Value, _error.Value));

            #endregion

        }

        #endregion

        #endregion



        #region GetAsGraph<T>()

        /// <summary>
        /// Return the select result as a Graph representation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAsGraph<T>()
            where T : DBObject, new()
        {

            if (Failed)
                throw new Exception("The query failed! Please check the errors!");

            if (_QueryResult != null)
                return FromQueryResult<T>();
            
            else if (_XMLQueryResult != null)
                return FromXML<T>();
            
            else
                throw new NotImplementedException();

        }

        #endregion

        #region FromQueryResult<T>()

        private IEnumerable<T> FromQueryResult<T>()
            where T : DBObject, new()
        {

            var theType = typeof(T);
            var retVal = new List<T>();

            #region Find the SelectionList to type T

            var selection = (from sel in _QueryResult.Results where sel.Type.Name == theType.Name select sel).FirstOrDefault();

            foreach (var elem in selection.Objects)
            {

                if (!_VisitedVerticesCache.ContainsKey(typeof(T)))
                    _VisitedVerticesCache.Add(typeof(T), new Dictionary<String, Object>());

                T newDBO;

                #region Create new instance or use an existing one

                if (_VisitedVerticesCache[typeof(T)].ContainsKey((String)elem.Attributes["UUID"]))
                {
                    newDBO = _VisitedVerticesCache[typeof(T)][(String)elem.Attributes["UUID"]] as T;
                }

                else
                {
                    newDBO = new T();
                    _VisitedVerticesCache[typeof(T)].Add((String)elem.Attributes["UUID"], newDBO);
                }

                #endregion

                foreach (var attr in elem.Attributes)
                    ApplyAttributeToObject(theType, newDBO, attr.Key, attr.Value);

                retVal.Add(newDBO);

            }

            #endregion

            //_QueryResult.SelectionList[0].Type
            return retVal;

        }

        #endregion

        #region FromXML<T>()

        private IEnumerable<T> FromXML<T>()
            where T : DBObject, new()
        {

            var _Type       = typeof(T);
            var _ReturnList = new List<T>();

            #region Find the SelectionList to type T

            foreach (var _Element in _QueryResultElement.Elements("results").Elements("DBObject"))
            {

                if (!_Element.Elements("attribute").Any(attr => attr.Attribute("name").Value == UUIDName))
                    throw new Exception("no UUID found!");

                T newDBO = GetObjectFromCache<T>(GetUUIDFromXML(_Element));

                foreach (var _Property in _Element.Elements("attribute"))
                    ApplyPropertyToObjectFromXML<T>(newDBO, _Property.Attribute("name").Value, _Property);

                foreach (var _Edge in _Element.Elements("edge"))
                    ApplyEdgeToObjectFromXML<T>(newDBO, _Edge.Attribute("name").Value, _Edge);

                _ReturnList.Add(newDBO);

            }

            #endregion

            return _ReturnList;

        }

        #endregion

        #region ToVertexType<T>()

        public IEnumerable<T> ToVertexType<T>()
        {

            var _Type       = typeof(T);
            var _ReturnList = new List<T>();
            T   _NewVertex  = default(T);

            #region Find the SelectionList to type T

            foreach (var _Element in _QueryResultElement.Elements("results").Elements("DBObject"))
            {

                _NewVertex = GetObjectFromCache<T>(GetUUIDFromXML(_Element));

                try
                {
                    _NewVertex = (T) Activator.CreateInstance(typeof(T));
                }
                catch (Exception e)
                {
                    _Errors.Add(e.Message);
                }

                foreach (var _Property in _Element.Elements("attribute"))
                    ApplyPropertyToObjectFromXML<T>(_NewVertex, _Property.Attribute("name").Value, _Property);

                foreach (var _Edge in _Element.Elements("edge"))
                    ApplyEdgeToObjectFromXML<T>(_NewVertex, _Edge.Attribute("name").Value, _Edge);

                _ReturnList.Add(_NewVertex);

            }

            #endregion

            return _ReturnList;

        }

        #endregion

        #region ToAnonymousType<T>()

        public IEnumerable<T> ToAnonymousType<T>()
        {

            var _Type       = typeof(T);
            var _ReturnList = new List<T>();
            T   _NewVertex  = default(T);

            #region Find the SelectionList to type T

            foreach (var _Element in _QueryResultElement.Elements("results").Elements("DBObject"))
            {

                // Perhaps first collect all attributes and then create the object using all attributes as parameter

                var l = new List<Object>();
                l.Add(new ObjectUUID());
                var ar = l.ToArray();

                _NewVertex = (T) Activator.CreateInstance(typeof(T), ar);

               // var _NewVertex2 = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
               //// _NewVertex = (T)_NewVertex2;
               // var _Members1 = typeof(T).GetMembers();
               // var _Members2 = new MemberInfo[1] { _Members1[6] };

               // _NewVertex = (T) System.Runtime.Serialization.FormatterServices.PopulateObjectMembers(_NewVertex2, _Members2, new Object[1] { new ObjectUUID()});
                //try
                //{
                //    _NewVertex = (T) Activator.CreateInstance(typeof(T));
                //}
                //catch (Exception e)
                //{
                //    _Errors.Add(e.Message);
                //}

                //foreach (var _Property in _Element.Elements("attribute"))
                //    ApplyPropertyToObjectFromXML<T>(_NewVertex, _Property.Attribute("name").Value, _Property);

                //foreach (var _Edge in _Element.Elements("edge"))
                //    ApplyEdgeToObjectFromXML<T>(_NewVertex, _Edge.Attribute("name").Value, _Edge);


                _ReturnList.Add(_NewVertex);

            }

            #endregion

            return _ReturnList;

        }

        #endregion

        #region ToDotNetObject<T>(myAttributeName)

        public IEnumerable<T> ToDotNetObject<T>(String myAttributeName)
        {

            var _Type = typeof(T);
            var _ReturnList = new List<T>();
            T _NewVertex = default(T);

            #region Find the SelectionList to type T

            foreach (var _Element in _QueryResultElement.Elements("results").Elements("DBObject"))
            {

                try
                {
                    _NewVertex = (T)Activator.CreateInstance(typeof(T));
                }
                catch (Exception e)
                {
                    _Errors.Add(e.Message);
                }

                _ReturnList.Add(_NewVertex);

            }

            #endregion

            return _ReturnList;

        }

        #endregion


        //#region GetAsGraph(Type)

        ///// <summary>
        ///// Return the select result as a Graph representation
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //public IEnumerable<DBObject> GetAsGraph(Type myType)
        //{

        //    if (Failed)
        //        throw new Exception("The query failed! Please check the errors!");

        //    if (_XMLQueryResult != null)
        //        return FromXML(myType);

        //    else
        //        throw new NotImplementedException();

        //}

        //#endregion

        //#region FromXML(myType)

        //private IEnumerable<DBObject> FromXML(Type myType)
        //{

        //    if (!(typeof(DBObject).IsAssignableFrom(myType)))
        //        throw new Exception("Invalid type " + myType.Name);

        //    var retVal = new List<DBObject>();
        //    var theType = myType;

        //    #region Find the SelectionList to type T

        //    foreach (var elem in _QueryResultElement.Elements("results").Elements("DBObject"))
        //    {

        //        if (!elem.Elements("attribute").Any(attr => attr.Attribute("name").Value == UUIDName))
        //            throw new Exception("no UUID found!");

        //        var DBObjectUUID = GetUUIDFromXML(elem);
        //        var newDBO = GetObjectFromCache(DBObjectUUID);

        //        foreach (var _Property in elem.Elements("attribute"))
        //            ApplyPropertyToObjectFromXML(theType, newDBO, _Property.Attribute("name").Value, _Property);

        //        foreach (var _Edge in elem.Elements("edge"))
        //            ApplyEdgeToObjectFromXML(theType, newDBO, _Edge.Attribute("name").Value, _Edge);

        //        retVal.Add(newDBO);

        //    }

        //    #endregion

        //    return retVal;

        //}

        //#endregion



        #region Helpers

        #region GetUUIDFromXML(myXElement)

        private String GetUUIDFromXML(XElement myXElement)
        {

            if (!myXElement.Elements("attribute").Any(attr => attr.Attribute("name").Value == UUIDName))
                throw new Exception("no UUID found!");

            return myXElement.Elements("attribute").Where(attr => attr.Attributes("name").First().Value == UUIDName).First().Value;

        }

        #endregion

        #region ApplyPropertyToObjectFromXML<T>(myDBObject, myAttributeName, myAttributeValue)

        /// <summary>
        /// Set the property with name <paramref name="myAttributeName"/> and value of <paramref name="myAttributeValue"/>
        /// </summary>
        /// <param name="myDBObjectType">The type of the object</param>
        /// <param name="myDBObject">The object instance itself</param>
        /// <param name="myAttributeName">The name of the property</param>
        /// <param name="myAttributeValue">The XML element containing the value</param>
        private void ApplyPropertyToObjectFromXML<T>(Object myDBObject, String myAttributeName, XElement myAttributeValue)
        {

            if (myAttributeValue == null)
                return;

            Object value = myAttributeValue.Value;

            var curProp = typeof(T).GetProperty(myAttributeName);
            if (curProp == null)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Could not find property \"{0}\"", myAttributeName));
                Console.WriteLine(String.Format("Could not find property \"{0}\"", myAttributeName));
                //throw new Exception(String.Format("Could not find property \"{0}\"", myAttributeName));
                return;
            }

            var elemType = myAttributeValue.Attributes("type").First().Value;

            // An ordinary value, will set the property
            curProp.SetValue(myDBObject, ConvertValue(elemType, value), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null);

        }

        #endregion

        #region ApplyEdgeToObjectFromXML<T>(myDBObject, myAttributeName, myAttributeValue)

        /// <summary>
        /// Set the reference property with name <paramref name="myAttributeName"/> and value of <paramref name="myAttributeValue"/>
        /// </summary>
        /// <param name="myDBObjectType">The type of the object</param>
        /// <param name="myDBObject">The object instance itself</param>
        /// <param name="myAttributeName">The name of the property</param>
        /// <param name="myAttributeValue">The XML element containing the value</param>
        private void ApplyEdgeToObjectFromXML<T>(Object myDBObject, String myAttributeName, XElement myAttributeValue)
        {

            if (myAttributeValue == null)
                return;

            Object value = myAttributeValue.Value;

            var curProp = typeof(T).GetProperty(myAttributeName);
            if (curProp == null)
                throw new Exception(String.Format("Could not find property \"{0}\"", myAttributeName));

            var elemType = myAttributeValue.Attributes("type").First().Value;

            var edges = myAttributeValue.Elements("DBObject");

            #region references

            var refType = curProp.PropertyType;

            //TODO: Check whether it is really a List, Hashset or whatever
            if (refType.GetInterface("IList") != null)
            {

                #region Get property value to check whether it was already set, if not

                var propVal = curProp.GetValue(myDBObject, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null) as IList;
                if (propVal == null)
                {
                    propVal = Activator.CreateInstance(refType) as IList;
                }

                #endregion

                foreach (var val in edges)
                {

                    /// Check whether it is really a List with one generic arg, what is about Weighted etc?
//                    var refObject = ParseXMLObject(refType.GetGenericArguments()[0], val);
                    var refObject = ParseXMLObject(val);

                    propVal.Add(refObject);

                }

                curProp.SetValue(myDBObject, propVal, null);
            }

            else
            {
//                var refObject = ParseXMLObject(refType, edges.First());
                var refObject = ParseXMLObject(edges.First());
                curProp.SetValue(myDBObject, refObject, null);
            }


            #endregion

        }

        #endregion

        #region ConvertValue(myType, myValue)

        /// <summary>
        /// Converts the value of type <paramref name="myType"/> to the corresponding csharp type
        /// </summary>
        /// <param name="myType"></param>
        /// <param name="myValue"></param>
        /// <returns>The corresponding csharp type</returns>
        private Object ConvertValue(String myType, Object myValue)
        {

            switch (myType)
            {

                case "Int64":
                    return Convert.ToInt64(myValue);
                case "UInt64":
                    return Convert.ToUInt64(myValue);
                case "String":
                    return Convert.ToString(myValue);
                case "Double":
                    return Convert.ToDouble(myValue);
                case "DateTime":
                    return Convert.ToDateTime(myValue);
                case "GraphDBType":
                    return Convert.ToString(myValue);
                case "ObjectUUID":
                    return new ObjectUUID(Convert.ToString(myValue));
                
                default:
                    throw new NotImplementedException("ConvertValue of type " + myType + " value: " + myValue.ToString());

            }

        }

        #endregion

        #region GetObjectFromCache(myUUID, myType)

        /// <summary>
        /// Get a object identified by the <paramref name="myUUID"/>. Either create a new one or use an existing one.
        /// </summary>
        /// <param name="myUUID"></param>
        /// <param name="myType"></param>
        /// <returns></returns>
        private T GetObjectFromCache<T>(String myUUID)
        {

            var _Type = typeof(T);

            if (!_VisitedVerticesCache.ContainsKey(_Type))
                _VisitedVerticesCache.Add(_Type, new Dictionary<String, Object>());

            Object _NewVertex;

            if (_VisitedVerticesCache[_Type].ContainsKey(myUUID))
                _NewVertex = _VisitedVerticesCache[_Type][myUUID];

            else
            {

                try
                {
                    _NewVertex = Activator.CreateInstance(typeof(T));
                }
                catch (Exception e)
                {
//                    _Errors.Add(e.Message);
                    throw new GraphDSSharpException(e.Message);
                }

                var curProp = _NewVertex.GetType().GetProperty("UUID");
                if (curProp != null)
                    curProp.SetValue(_NewVertex, ObjectUUID.FromString(myUUID), new Object[0]);

                _VisitedVerticesCache[_Type].Add(myUUID, _NewVertex);

            }

            T _T = default(T);

            try
            {
                _T = (T) _NewVertex;
            }
            catch (Exception)
            { }

            return _T;

        }

        #endregion

        #region ParseXMLObject<T>(myType, myXElement)

        /// <summary>
        /// Parses the xml tag for all attributes and apply them to a new object
        /// </summary>
        /// <param name="myType"></param>
        /// <param name="myXElement"></param>
        /// <returns></returns>
        private Object ParseXMLObject(XElement myXElement)
        {

            // Create new instance or use an existing one
            var  refObject = GetObjectFromCache<Object>(GetUUIDFromXML(myXElement));

            foreach (var attr in myXElement.Elements("attribute"))
                ApplyPropertyToObjectFromXML<Object>(refObject, attr.Attributes("name").First().Value, attr);

            foreach (var attr in myXElement.Elements("edge"))
                ApplyEdgeToObjectFromXML<Object>(refObject, attr.Attribute("name").Value, attr);

            return refObject;

        }

        #endregion

        #region ParseXMLObject<T>(myType, myXElement)

        /// <summary>
        /// Parses the xml tag for all attributes and apply them to a new object
        /// </summary>
        /// <param name="myType"></param>
        /// <param name="myXElement"></param>
        /// <returns></returns>
        private T ParseXMLObject<T>(XElement myXElement)
        {

            // Create new instance or use an existing one
            var refObject = GetObjectFromCache<T>(GetUUIDFromXML(myXElement));

            foreach (var attr in myXElement.Elements("attribute"))
                ApplyPropertyToObjectFromXML<Object>(refObject, attr.Attributes("name").First().Value, attr);

            foreach (var attr in myXElement.Elements("edge"))
                ApplyEdgeToObjectFromXML<Object>(refObject, attr.Attribute("name").Value, attr);

            return refObject;

        }

        #endregion

        #region ApplyAttributeToObject(Type myDBObjectType, DBObject myDBObject, String myAttributeName, Object myAttributeValue)

        private void ApplyAttributeToObject(Type myDBObjectType, DBObject myDBObject, String myAttributeName, Object myAttributeValue)
        {

            if (myAttributeValue == null)
                return;

            var curProp = myDBObjectType.GetProperty(myAttributeName);

            if (myAttributeValue is List<DBObjectReadout>)
            {

                /// Check whether it is really a List
                var refType = curProp.PropertyType;
                //var refObjectRator = Activator.CreateInstance(refType) as IList;

                #region Get property value to check whether it was already set

                var propVal = curProp.GetValue(myDBObject, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null) as IList;
                if (propVal == null)
                    propVal = Activator.CreateInstance(refType) as IList;

                #endregion

                foreach (var val in (myAttributeValue as List<DBObjectReadout>))
                {
                    /// Check whether it is really a List with one generic arg, what is about Weighted etc?
                    var listType = refType.GetGenericArguments()[0];

                    #region Create new instance or use an existing one

                    if (!_VisitedVerticesCache.ContainsKey(listType))
                        _VisitedVerticesCache.Add(listType, new Dictionary<String, Object>());

                    DBObject refObject;
                    if (_VisitedVerticesCache[listType].ContainsKey((String)val.Attributes["UUID"]))
                    {
                        refObject = _VisitedVerticesCache[listType][(String)val.Attributes["UUID"]] as DBObject;
                    }
                    else
                    {
                        refObject = Activator.CreateInstance(listType) as DBObject;
                        _VisitedVerticesCache[listType].Add((String)val.Attributes["UUID"], refObject);
                    }

                    #endregion

                    foreach (var attr in (val as DBObjectReadout).Attributes)
                        ApplyAttributeToObject(listType, refObject, attr.Key, attr.Value);

                    propVal.Add(refObject);
                
                }

                curProp.SetValue(myDBObject, propVal, null);

            }

            else if (myAttributeValue is DBObjectReadout)
            {

                var refType = curProp.PropertyType;
                var refObject = Activator.CreateInstance(refType) as DBObject;

                foreach (var attr in (myAttributeValue as DBObjectReadout).Attributes)
                    ApplyAttributeToObject(refType, refObject, attr.Key, attr.Value);
                
                curProp.SetValue(myDBObject, refObject, null);

            }

            else
                curProp.SetValue(myDBObject, myAttributeValue, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null);

        }
        
        #endregion

        #endregion

    }
}
