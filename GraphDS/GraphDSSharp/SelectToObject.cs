/*
 * SelectToObjectGraph
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using sones.GraphDB.Structures;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphDB.NewAPI;

using sones.GraphFS.DataStructures;

using sones.GraphIO;
using sones.GraphIO.XML;
using sones.GraphDB.Result;

#endregion

namespace sones.GraphDS.API.CSharp
{

    /// <summary>
    /// Use the result of the GraphDB to create a ObjectGraph
    /// </summary>
    public class SelectToObjectGraph
    {

        #region Data

        private readonly QueryResult _QueryResult;
        private readonly XDocument _XMLQueryResult;
        private readonly String UUIDName = SpecialTypeAttribute_UUID.AttributeName;
        private readonly XElement _QueryResultElement;
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
            : this(XML_IO_Extensions.BuildXMLDocument(myQueryResult.ToXML()).XMLDocument2String())
        {
            _QueryResult = myQueryResult;
        }

        #endregion

        #region SelectToObjectGraph(myXMLQueryResult)

        public SelectToObjectGraph(String myXMLQueryResult)
        {

            #region Init

            _Warnings = new List<String>();
            _Errors = new List<String>();
            _VisitedVerticesCache = new Dictionary<Type, Dictionary<String, Object>>();

            #endregion

            #region Set QueryResult data

            _XMLQueryResult = XDocument.Parse(myXMLQueryResult, LoadOptions.None);
            _QueryResultElement = _XMLQueryResult.Element("sones").Element("graphdb").Element("queryresult");

            Query = _QueryResultElement.Element("query").Value;
            QueryResult = (ResultType)Enum.Parse(typeof(ResultType), _QueryResultElement.Element("result").Value);
            Duration = UInt64.Parse(_QueryResultElement.Element("duration").Value);

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

            //var selection = (from sel in _QueryResult.Results where sel.TypeName == theType.Name select sel).FirstOrDefault();

            foreach (var _Vertex in _QueryResult)
            {

                if (!_VisitedVerticesCache.ContainsKey(typeof(T)))
                    _VisitedVerticesCache.Add(typeof(T), new Dictionary<String, Object>());

                T newDBO;

                #region Create new instance or use an existing one

                if (_VisitedVerticesCache[typeof(T)].ContainsKey(_Vertex.UUID.ToString()))
                {
                    newDBO = _VisitedVerticesCache[typeof(T)][_Vertex.UUID.ToString()] as T;
                }

                else
                {
                    newDBO = new T();
                    _VisitedVerticesCache[typeof(T)].Add(_Vertex.UUID.ToString(), newDBO);
                }

                #endregion

                foreach (var attr in _Vertex)
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

            var _Type = typeof(T);
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

            var _Type = typeof(T);
            var _ReturnList = new List<T>();
            T _NewVertex = default(T);

            #region Find the SelectionList to type T

            foreach (var _Element in _QueryResultElement.Elements("results").Elements("DBObject"))
            {

                _NewVertex = GetObjectFromCache<T>(GetUUIDFromXML(_Element));

                try
                {
                    _NewVertex = (T)Activator.CreateInstance(typeof(T));
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

            var _Type = typeof(T);
            var _ReturnList = new List<T>();
            T _NewVertex = default(T);

            #region Find the SelectionList to type T

            foreach (var _Element in _QueryResultElement.Element("results").Elements("DBObject"))
            {

                // Perhaps first collect all attributes and then create the object using all attributes as parameter

                var _AttributesList = new List<Object>();

                foreach (var _Property in _Element.Elements("attribute"))
                    _AttributesList.Add(_Property.Attribute("name").Value);

                _AttributesList.Clear();
                _AttributesList.Add(new ObjectUUID());

                var ar = _AttributesList.ToArray();

                _NewVertex = (T) Activator.CreateInstance(typeof(T), ar);

                //var _NewVertex2 = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
                //// _NewVertex = (T)_NewVertex2;
                //var _Members1 = typeof(T).GetMembers();
                //var _Members2 = new MemberInfo[1] { _Members1[6] };

                //_NewVertex = (T)System.Runtime.Serialization.FormatterServices.PopulateObjectMembers(_NewVertex2, _Members2, new Object[1] { new ObjectUUID() });
                //try
                //{
                //    _NewVertex = (T)Activator.CreateInstance(typeof(T));
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

            foreach (var _Element in _QueryResultElement.Element("results").Elements("DBObject"))
            {

                try
                {
                    _NewVertex = (T) Activator.CreateInstance(typeof(T));
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
                _T = (T)_NewVertex;
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
            var refObject = GetObjectFromCache<Object>(GetUUIDFromXML(myXElement));

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

            if (myAttributeValue is List<Vertex>)
            {

                /// Check whether it is really a List
                var refType = curProp.PropertyType;
                //var refObjectRator = Activator.CreateInstance(refType) as IList;

                #region Get property value to check whether it was already set

                var propVal = curProp.GetValue(myDBObject, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null) as IList;
                if (propVal == null)
                    propVal = Activator.CreateInstance(refType) as IList;

                #endregion

                foreach (var _Vertex in (myAttributeValue as List<Vertex>))
                {
                    /// Check whether it is really a List with one generic arg, what is about Weighted etc?
                    var listType = refType.GetGenericArguments()[0];

                    #region Create new instance or use an existing one

                    if (!_VisitedVerticesCache.ContainsKey(listType))
                        _VisitedVerticesCache.Add(listType, new Dictionary<String, Object>());

                    DBObject refObject;
                    if (_VisitedVerticesCache[listType].ContainsKey(_Vertex.UUID.ToString()))
                    {
                        refObject = _VisitedVerticesCache[listType][_Vertex.UUID.ToString()] as DBObject;
                    }
                    else
                    {
                        refObject = Activator.CreateInstance(listType) as DBObject;
                        _VisitedVerticesCache[listType].Add(_Vertex.UUID.ToString(), refObject);
                    }

                    #endregion

                    foreach (var attr in _Vertex)
                        ApplyAttributeToObject(listType, refObject, attr.Key, attr.Value);

                    propVal.Add(refObject);

                }

                curProp.SetValue(myDBObject, propVal, null);

            }

            else if (myAttributeValue is Vertex)
            {

                var refType = curProp.PropertyType;
                var refObject = Activator.CreateInstance(refType) as DBObject;

                foreach (var attr in (myAttributeValue as Vertex))
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


///* SelectToObject
// * (c) Stefan Licht, 2010
// * 
// * Lead programmer:
// *      Stefan Licht
// * 
// */

//#region Usings

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Xml.Linq;

//
//using sones.GraphDB.Structures;
//using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
//using sones.GraphDS.API.CSharp.Reflection;
//using sones.GraphFS.DataStructures;

//#endregion

//namespace sones.GraphDS.API.CSharp
//{

//    /// <summary>
//    /// Use the result of the GraphDB to create a ObjectGraph
//    /// </summary>
//    public class SelectToObjectGraph
//    {

//        #region Data

//        private QueryResult _QueryResult;
//        private String _XmlResult;
//        private String UUIDName = SpecialTypeAttribute_UUID.AttributeName;
//        private XElement _QResultElement;
//        private Dictionary<Type, Dictionary<String, DBObject>> visitedDBOs = new Dictionary<Type, Dictionary<string, DBObject>>();

//        #endregion

//        #region Properties

//        private List<String> _Errors;
//        public List<String> Errors
//        {
//            get { return _Errors; }
//        }

//        public Boolean Failed
//        {
//            get
//            {
//                return (_Errors != null && _Errors.Count > 0);
//            }
//        }

//        #endregion

//        #region Ctors

//        public SelectToObjectGraph(QueryResult myQueryResult)
//            : this(myQueryResult.ToXML().ToString())
//        { }

//        public SelectToObjectGraph(String myXmlResult)
//        {

//            _XmlResult = myXmlResult;

//            var xml = XDocument.Parse(_XmlResult, LoadOptions.None);

//            _QResultElement = xml.Element("sones").Element("GraphDB").Element("queryresult");

//            #region Query

//            var query = _QResultElement.Element("query");

//            #region ResultType

//            var _ResultType = (ResultType)Enum.Parse(typeof(ResultType), _QResultElement.Element("result").Value);

//            #endregion

//            #region Query

//            var _Query = query.Value;

//            #endregion

//            #region Errors

//            var errors = _QResultElement.Element("errors").Elements("error");
//            if (errors != null)
//            {
//                _Errors = errors.Aggregate<XElement, List<String>>(new List<String>(), (result, elem) =>
//                {
//                    result.Add(String.Format("[{0}]: {1}", elem.Attribute("error code").Value, elem.Value));
//                    return result;
//                });
//            }

//            #endregion

//            #endregion

//        }

//        #endregion

//        #region GetAsGraph

//        #region GetAsGraph<T>

//        /// <summary>
//        /// Return the select result as a Graph representation
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <returns></returns>
//        public IEnumerable<T> ToVertexType<T>()
//            where T : DBObject, new()
//        {

//            if (Failed)
//            {
//                throw new Exception("The query failed! Please check the errors!");
//            }

//            if (_QueryResult != null)
//                return FromQueryResult<T>();
//            else if (_XmlResult != null)
//                return FromXML<T>();
//            else
//                throw new NotImplementedException();
//        }

//        private IEnumerable<T> FromXML<T>()
//            where T : DBObject, new()
//        {

//            var xml = XDocument.Parse(_XmlResult, LoadOptions.None);

//            #region ResultSet

//            #region Go through each DBO

//            var theType = typeof(T);
//            var retVal = new List<T>();

//            #region Find the SelectionList to type T

//            foreach (var elem in _QResultElement.Elements("results").Elements("DBObject"))
//            {
//                if (!elem.Elements("attribute").Any(attr => attr.Attribute("name").Value == UUIDName))
//                    throw new Exception("no UUID found!");

//                var DBObjectUUID = GetUUIDFromXML(elem);
//                T newDBO = getObject(DBObjectUUID, typeof(T)) as T;

//                foreach (var attr in elem.Elements("attribute"))
//                {
//                    ApplyAttributeToObjectFromXML(theType, newDBO, attr.Attribute("name").Value, attr);
//                }

//                foreach (var attr in elem.Elements("edge"))
//                {
//                    ApplyReferenceAttributeToObjectFromXML(theType, newDBO, attr.Attribute("name").Value, attr);
//                }
//                retVal.Add(newDBO);
//            }

//            #endregion

//            #endregion

//            #endregion

//            return retVal;
//        }

//        #endregion

//        #region GetAsGraph(Type)

//        /// <summary>
//        /// Return the select result as a Graph representation
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <returns></returns>
//        public IEnumerable<DBObject> GetAsGraph(Type myType)
//        {

//            if (Failed)
//            {
//                throw new Exception("The query failed! Please check the errors!");
//            }

//            if (_XmlResult != null)
//                return FromXML(myType);
//            else
//                throw new NotImplementedException();
//        }

//        private IEnumerable<DBObject> FromXML(Type myType)
//        {
//            if (!(typeof(DBObject).IsAssignableFrom(myType)))
//                throw new Exception("Invalid type " + myType.Name);


//            var xml = XDocument.Parse(_XmlResult, LoadOptions.None);

//            #region ResultSet

//            var retVal = new List<DBObject>();

//            #region Go through each DBO

//            var theType = myType;

//            #region Find the SelectionList to type T

//            foreach (var elem in _QResultElement.Elements("results").Elements("DBObject"))
//            {
//                if (!elem.Elements("attribute").Any(attr => attr.Attribute("name").Value == UUIDName))
//                    throw new Exception("no UUID found!");

//                var DBObjectUUID = GetUUIDFromXML(elem);
//                var newDBO = getObject(DBObjectUUID, myType);

//                foreach (var attr in elem.Elements("attribute"))
//                {
//                    ApplyAttributeToObjectFromXML(theType, newDBO, attr.Attribute("name").Value, attr);
//                }

//                foreach (var attr in elem.Elements("edge"))
//                {
//                    ApplyReferenceAttributeToObjectFromXML(theType, newDBO, attr.Attribute("name").Value, attr);
//                }
//                retVal.Add(newDBO);
//            }

//            #endregion

//            #endregion

//            #endregion

//            return retVal;

//        }

//        #endregion

//        #endregion

//        #region Some helpers

//        /// <summary>
//        /// Gets the UUID from xml
//        /// </summary>
//        /// <param name="elem"></param>
//        /// <returns></returns>
//        private String GetUUIDFromXML(XElement elem)
//        {
//            return elem.Elements("attribute").Where(attr => attr.Attributes("name").First().Value == UUIDName).First().Value;
//        }

//        /// <summary>
//        /// Set the property with name <paramref name="myAttributeName"/> and value of <paramref name="myAttributeValue"/>
//        /// </summary>
//        /// <param name="myDBObjectType">The type of the object</param>
//        /// <param name="myDBObject">The object instance itself</param>
//        /// <param name="myAttributeName">The name of the property</param>
//        /// <param name="myAttributeValue">The XML element containing the value</param>
//        private void ApplyAttributeToObjectFromXML(Type myDBObjectType, DBObject myDBObject, String myAttributeName, XElement myAttributeValue)
//        {
//            if (myAttributeValue == null)
//                return;

//            Object value = myAttributeValue.Value;

//            #region UUID check/hack

//            if (myAttributeName == UUIDName)
//            {
//                /// the myDBObject already has a valid UUID
//                /// Just a sanity check - at some time we can remove these 2 lines
//                if (ObjectUUID.FromString((String)value) != myDBObject.UUID)
//                    throw new Exception("Did not get the correct object!");

//                return;
//                //myAttributeName = "UUID";
//                //value = ObjectUUID.FromString((String)value);
//            }

//            #endregion

//            var curProp = myDBObjectType.GetProperty(myAttributeName);
//            if (curProp == null)
//            {
//                System.Diagnostics.Debug.WriteLine(String.Format("Could not find property \"{0}\"", myAttributeName));
//                Console.WriteLine(String.Format("Could not find property \"{0}\"", myAttributeName));
//                //throw new Exception(String.Format("Could not find property \"{0}\"", myAttributeName));
//                return;
//            }

//            var elemType = myAttributeValue.Attributes("type").First().Value;

//            #region An ordinary value, will set the property

//            curProp.SetValue(myDBObject, ConvertValue(elemType, value), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null);

//            #endregion
//        }

//        /// <summary>
//        /// Set the reference property with name <paramref name="myAttributeName"/> and value of <paramref name="myAttributeValue"/>
//        /// </summary>
//        /// <param name="myDBObjectType">The type of the object</param>
//        /// <param name="myDBObject">The object instance itself</param>
//        /// <param name="myAttributeName">The name of the property</param>
//        /// <param name="myAttributeValue">The XML element containing the value</param>
//        private void ApplyReferenceAttributeToObjectFromXML(Type myDBObjectType, DBObject myDBObject, String myAttributeName, XElement myAttributeValue)
//        {
//            if (myAttributeValue == null)
//                return;

//            Object value = myAttributeValue.Value;

//            var curProp = myDBObjectType.GetProperty(myAttributeName);
//            if (curProp == null)
//                throw new Exception(String.Format("Could not find property \"{0}\"", myAttributeName));

//            var elemType = myAttributeValue.Attributes("type").First().Value;

//            var edges = myAttributeValue.Elements("DBObject");

//            #region references

//            var refType = curProp.PropertyType;

//            //TODO: Check whether it is really a List, Hashset or whatever
//            if (refType.GetInterface("IList") != null)
//            {

//                #region Get property value to check whether it was already set, if not

//                var propVal = curProp.GetValue(myDBObject, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null) as IList;
//                if (propVal == null)
//                {
//                    propVal = Activator.CreateInstance(refType) as IList;
//                }

//                #endregion

//                foreach (var val in edges)
//                {
//                    /// Check whether it is really a List with one generic arg, what is about Weighted etc?
//                    var refObject = parseXmlObject(refType.GetGenericArguments()[0], val);

//                    propVal.Add(refObject);
//                }

//                curProp.SetValue(myDBObject, propVal, null);
//            }
//            else
//            {
//                var refObject = parseXmlObject(refType, edges.First());
//                curProp.SetValue(myDBObject, refObject, null);
//            }


//            #endregion

//        }

//        /// <summary>
//        /// Converts the value of type <paramref name="elemType"/> to the corresponding csharp type
//        /// </summary>
//        /// <param name="elemType"></param>
//        /// <param name="value"></param>
//        /// <returns>The corresponding csharp type</returns>
//        private object ConvertValue(string elemType, object value)
//        {
//            switch (elemType)
//            {
//                case "Int64":
//                    return Convert.ToInt64(value);
//                case "UInt64":
//                    return Convert.ToUInt64(value);
//                case "String":
//                    return Convert.ToString(value);
//                case "Double":
//                    return Convert.ToDouble(value);
//                case "DateTime":
//                    return Convert.ToDateTime(value);
//                case "GraphDBType":
//                    return Convert.ToString(value);
//                default:
//                    throw new NotImplementedException("ConvertValue of type " + elemType + " value: " + value.ToString());
//            }
//        }

//        /// <summary>
//        /// Get a object identified by the <paramref name="DBObjectUUID"/>. Either create a new one or use an existing one.
//        /// </summary>
//        /// <param name="DBObjectUUID"></param>
//        /// <param name="myType"></param>
//        /// <returns></returns>
//        private DBObject getObject(String DBObjectUUID, Type myType)
//        {
//            if (!visitedDBOs.ContainsKey(myType))
//            {
//                visitedDBOs.Add(myType, new Dictionary<string, DBObject>());
//            }

//            DBObject refObject;
//            if (visitedDBOs[myType].ContainsKey(DBObjectUUID))
//            {
//                refObject = visitedDBOs[myType][DBObjectUUID] as DBObject;
//            }
//            else
//            {
//                refObject = Activator.CreateInstance(myType) as DBObject;
//                refObject.UUID = ObjectUUID.FromString(DBObjectUUID);
//                visitedDBOs[myType].Add(DBObjectUUID, refObject);
//            }

//            return refObject;
//        }

//        /// <summary>
//        /// Parses the xml tag for all attributes and apply them to a new object
//        /// </summary>
//        /// <param name="refType"></param>
//        /// <param name="val"></param>
//        /// <returns></returns>
//        private DBObject parseXmlObject(Type refType, XElement val)
//        {
//            var listType = refType;
//            //var refObject = Activator.CreateInstance(listType) as DBObject;

//            #region Create new instance or use an existing one

//            var DBObjectUUID = GetUUIDFromXML(val);
//            DBObject refObject = getObject(DBObjectUUID, listType);

//            #endregion

//            foreach (var attr in val.Elements("attribute"))
//            {
//                ApplyAttributeToObjectFromXML(listType, refObject, attr.Attributes("name").First().Value, attr);
//            }

//            foreach (var attr in val.Elements("edge"))
//            {
//                ApplyReferenceAttributeToObjectFromXML(listType, refObject, attr.Attribute("name").Value, attr);
//            }

//            return refObject;
//        }

//        #endregion

//        #region The direct QueryResult with Vertex - fix me

//        private IEnumerable<T> FromQueryResult<T>()
//            where T : DBObject, new()
//        {

//            var theType = typeof(T);
//            var retVal = new List<T>();

//            #region Find the SelectionList to type T

//            var selection = (from sel in _QueryResult.Results where sel.Type.Name == theType.Name select sel).FirstOrDefault();

//            foreach (var elem in selection.Objects)
//            {
//                if (!visitedDBOs.ContainsKey(typeof(T)))
//                    visitedDBOs.Add(typeof(T), new Dictionary<string, DBObject>());

//                T newDBO;

//                #region Create new instance or use an existing one

//                if (visitedDBOs[typeof(T)].ContainsKey((String)elem.Attributes["UUID"]))
//                {
//                    newDBO = visitedDBOs[typeof(T)][(String)elem.Attributes["UUID"]] as T;
//                }
//                else
//                {
//                    newDBO = new T();
//                    visitedDBOs[typeof(T)].Add((String)elem.Attributes["UUID"], newDBO);
//                }

//                #endregion

//                foreach (var attr in elem.Attributes)
//                {
//                    ApplyAttributeToObject(theType, newDBO, attr.Key, attr.Value);
//                }
//                retVal.Add(newDBO);
//            }

//            #endregion

//            //_QueryResult.SelectionList[0].Type
//            return retVal;
//        }

//        private void ApplyAttributeToObject(Type myDBObjectType, DBObject myDBObject, String myAttributeName, Object myAttributeValue)
//        {

//            if (myAttributeValue == null)
//                return;

//            var curProp = myDBObjectType.GetProperty(myAttributeName);

//            if (myAttributeValue is List<Vertex>)
//            {

//                /// Check whether it is really a List
//                var refType = curProp.PropertyType;
//                //var refObjectRator = Activator.CreateInstance(refType) as IList;

//                #region Get property value to check whether it was already set

//                var propVal = curProp.GetValue(myDBObject, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null) as IList;
//                if (propVal == null)
//                    propVal = Activator.CreateInstance(refType) as IList;

//                #endregion

//                foreach (var val in (myAttributeValue as List<Vertex>))
//                {
//                    /// Check whether it is really a List with one generic arg, what is about Weighted etc?
//                    var listType = refType.GetGenericArguments()[0];

//                    #region Create new instance or use an existing one

//                    if (!visitedDBOs.ContainsKey(listType))
//                        visitedDBOs.Add(listType, new Dictionary<string, DBObject>());

//                    DBObject refObject;
//                    if (visitedDBOs[listType].ContainsKey((String)val.Attributes["UUID"]))
//                    {
//                        refObject = visitedDBOs[listType][(String)val.Attributes["UUID"]] as DBObject;
//                    }
//                    else
//                    {
//                        refObject = Activator.CreateInstance(listType) as DBObject;
//                        visitedDBOs[listType].Add((String)val.Attributes["UUID"], refObject);
//                    }

//                    #endregion

//                    foreach (var attr in (val as Vertex).Attributes)
//                    {
//                        ApplyAttributeToObject(listType, refObject, attr.Key, attr.Value);
//                    }

//                    propVal.Add(refObject);
//                }

//                curProp.SetValue(myDBObject, propVal, null);
//            }
//            else if (myAttributeValue is Vertex)
//            {
//                var refType = curProp.PropertyType;
//                var refObject = Activator.CreateInstance(refType) as DBObject;

//                foreach (var attr in (myAttributeValue as Vertex).Attributes)
//                {
//                    ApplyAttributeToObject(refType, refObject, attr.Key, attr.Value);
//                }
//                curProp.SetValue(myDBObject, refObject, null);
//            }
//            else
//            {
//                curProp.SetValue(myDBObject, myAttributeValue, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null);
//            }
//        }

//        #endregion

//    }
//}
