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
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
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

        private QueryResult _QueryResult;
        private String _XmlResult;
        private String UUIDName = SpecialTypeAttribute_UUID.AttributeName;
        private XElement _QResultElement;
        private Dictionary<Type, Dictionary<String, DBObject>> visitedDBOs = new Dictionary<Type, Dictionary<string, DBObject>>();

        #endregion

        #region Properties

        private List<String> _Errors;
        public List<String> Errors
        {
            get { return _Errors; }
        }

        public Boolean Failed
        {
            get
            {
                return (_Errors != null && _Errors.Count > 0);
            }
        }

        #endregion

        #region Ctors

        public SelectToObjectGraph(QueryResult myQueryResult)
            : this(myQueryResult.ToXML().ToString())
        { }

        public SelectToObjectGraph(String myXmlResult)
        {

            _XmlResult = myXmlResult;

            var xml = XDocument.Parse(_XmlResult, LoadOptions.None);

            _QResultElement = xml.Element("sones").Element("GraphDB").Element("queryresult");

            #region Query

            var query = _QResultElement.Element("query");

            #region ResultType

            var _ResultType = (ResultType)Enum.Parse(typeof(ResultType), _QResultElement.Element("result").Value);

            #endregion

            #region Query

            var _Query = query.Value;

            #endregion

            #region Errors

            var errors = _QResultElement.Element("errors").Elements("Error");
            if (errors != null)
            {
                _Errors = errors.Aggregate<XElement, List<String>>(new List<String>(), (result, elem) =>
                {
                    result.Add(String.Format("[{0}]: {1}", elem.Attribute("ErrorCode").Value, elem.Attribute("ErrorDescription").Value));
                    return result;
                });
            }

            #endregion

            #endregion

        }
        
        #endregion

        #region GetAsGraph

        #region GetAsGraph<T>

        /// <summary>
        /// Return the select result as a Graph representation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAsGraph<T>()
            where T : DBObject, new()
        {

            if (Failed)
            {
                throw new Exception("The query failed! Please check the errors!");
            }

            if (_QueryResult != null)
                return FromQueryResult<T>();
            else if (_XmlResult != null)
                return FromXML<T>();
            else
                throw new NotImplementedException();
        }

        private IEnumerable<T> FromXML<T>()
            where T : DBObject, new()
        {

            var xml = XDocument.Parse(_XmlResult, LoadOptions.None);

            #region ResultSet

            #region Go through each DBO

            var theType = typeof(T);
            var retVal = new List<T>();

            #region Find the SelectionList to type T

            foreach (var elem in _QResultElement.Elements("results").Elements("DBObject"))
            {
                if (!elem.Elements("attribute").Any(attr => attr.Attribute("name").Value == UUIDName))
                    throw new Exception("no UUID found!");

                var DBObjectUUID = GetUUIDFromXML(elem);
                T newDBO = getObject(DBObjectUUID, typeof(T)) as T;

                foreach (var attr in elem.Elements("attribute"))
                {
                    ApplyAttributeToObjectFromXML(theType, newDBO, attr.Attribute("name").Value, attr);
                }

                foreach (var attr in elem.Elements("edge"))
                {
                    ApplyReferenceAttributeToObjectFromXML(theType, newDBO, attr.Attribute("name").Value, attr);
                }
                retVal.Add(newDBO);
            }

            #endregion

            #endregion

            #endregion

            return retVal;
        }

        #endregion

        #region GetAsGraph(Type)

        /// <summary>
        /// Return the select result as a Graph representation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<DBObject> GetAsGraph(Type myType)
        {

            if (Failed)
            {
                throw new Exception("The query failed! Please check the errors!");
            }

            if (_XmlResult != null)
                return FromXML(myType);
            else
                throw new NotImplementedException();
        }
        
        private IEnumerable<DBObject> FromXML(Type myType)
        {
            if (!(typeof(DBObject).IsAssignableFrom(myType)))
                throw new Exception("Invalid type " + myType.Name);


            var xml = XDocument.Parse(_XmlResult, LoadOptions.None);

            #region ResultSet

            var retVal = new List<DBObject>();

            #region Go through each DBO

            var theType = myType;

            #region Find the SelectionList to type T

            foreach (var elem in _QResultElement.Elements("results").Elements("DBObject"))
            {
                if (!elem.Elements("attribute").Any(attr => attr.Attribute("name").Value == UUIDName))
                    throw new Exception("no UUID found!");

                var DBObjectUUID = GetUUIDFromXML(elem);
                var newDBO = getObject(DBObjectUUID, myType);

                foreach (var attr in elem.Elements("attribute"))
                {
                    ApplyAttributeToObjectFromXML(theType, newDBO, attr.Attribute("name").Value, attr);
                }

                foreach (var attr in elem.Elements("edge"))
                {
                    ApplyReferenceAttributeToObjectFromXML(theType, newDBO, attr.Attribute("name").Value, attr);
                }
                retVal.Add(newDBO);
            }

            #endregion

            #endregion

            #endregion

            return retVal;

        }

        #endregion

        #endregion

        #region Some helpers

        /// <summary>
        /// Gets the UUID from xml
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        private String GetUUIDFromXML(XElement elem)
        {
            return elem.Elements("attribute").Where(attr => attr.Attributes("name").First().Value == UUIDName).First().Value;
        }

        /// <summary>
        /// Set the property with name <paramref name="myAttributeName"/> and value of <paramref name="myAttributeValue"/>
        /// </summary>
        /// <param name="myDBObjectType">The type of the object</param>
        /// <param name="myDBObject">The object instance itself</param>
        /// <param name="myAttributeName">The name of the property</param>
        /// <param name="myAttributeValue">The XML element containing the value</param>
        private void ApplyAttributeToObjectFromXML(Type myDBObjectType, DBObject myDBObject, String myAttributeName, XElement myAttributeValue)
        {
            if (myAttributeValue == null)
                return;

            Object value = myAttributeValue.Value;

            #region UUID check/hack

            if (myAttributeName == UUIDName)
            {
                /// the myDBObject already has a valid UUID
                /// Just a sanity check - at some time we can remove these 2 lines
                if (ObjectUUID.FromString((String)value) != myDBObject.UUID)
                    throw new Exception("Did not get the correct object!");

                return;
                //myAttributeName = "UUID";
                //value = ObjectUUID.FromString((String)value);
            }

            #endregion

            var curProp = myDBObjectType.GetProperty(myAttributeName);
            if (curProp == null)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Could not find property \"{0}\"", myAttributeName));
                Console.WriteLine(String.Format("Could not find property \"{0}\"", myAttributeName));
                //throw new Exception(String.Format("Could not find property \"{0}\"", myAttributeName));
                return;
            }

            var elemType = myAttributeValue.Attributes("type").First().Value;

            #region An ordinary value, will set the property

            curProp.SetValue(myDBObject, ConvertValue(elemType, value), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null);

            #endregion
        }

        /// <summary>
        /// Set the reference property with name <paramref name="myAttributeName"/> and value of <paramref name="myAttributeValue"/>
        /// </summary>
        /// <param name="myDBObjectType">The type of the object</param>
        /// <param name="myDBObject">The object instance itself</param>
        /// <param name="myAttributeName">The name of the property</param>
        /// <param name="myAttributeValue">The XML element containing the value</param>
        private void ApplyReferenceAttributeToObjectFromXML(Type myDBObjectType, DBObject myDBObject, String myAttributeName, XElement myAttributeValue)
        {
            if (myAttributeValue == null)
                return;

            Object value = myAttributeValue.Value;

            var curProp = myDBObjectType.GetProperty(myAttributeName);
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
                    var refObject = parseXmlObject(refType.GetGenericArguments()[0], val);

                    propVal.Add(refObject);
                }

                curProp.SetValue(myDBObject, propVal, null);
            }
            else
            {
                var refObject = parseXmlObject(refType, edges.First());
                curProp.SetValue(myDBObject, refObject, null);
            }


            #endregion

        }

        /// <summary>
        /// Converts the value of type <paramref name="elemType"/> to the corresponding csharp type
        /// </summary>
        /// <param name="elemType"></param>
        /// <param name="value"></param>
        /// <returns>The corresponding csharp type</returns>
        private object ConvertValue(string elemType, object value)
        {
            switch (elemType)
            {
                case "Int64":
                    return Convert.ToInt64(value);
                case "String":
                    return Convert.ToString(value);
                case "Double":
                    return Convert.ToDouble(value);
                case "DateTime":
                    return Convert.ToDateTime(value);
                default:
                    throw new NotImplementedException("ConvertValue of type " + elemType);
            }
        }

        /// <summary>
        /// Get a object identified by the <paramref name="DBObjectUUID"/>. Either create a new one or use an existing one.
        /// </summary>
        /// <param name="DBObjectUUID"></param>
        /// <param name="myType"></param>
        /// <returns></returns>
        private DBObject getObject(String DBObjectUUID, Type myType)
        {
            if (!visitedDBOs.ContainsKey(myType))
            {
                visitedDBOs.Add(myType, new Dictionary<string, DBObject>());
            }

            DBObject refObject;
            if (visitedDBOs[myType].ContainsKey(DBObjectUUID))
            {
                refObject = visitedDBOs[myType][DBObjectUUID] as DBObject;
            }
            else
            {
                refObject = Activator.CreateInstance(myType) as DBObject;
                refObject.UUID = ObjectUUID.FromString(DBObjectUUID);
                visitedDBOs[myType].Add(DBObjectUUID, refObject);
            }

            return refObject;
        }

        /// <summary>
        /// Parses the xml tag for all attributes and apply them to a new object
        /// </summary>
        /// <param name="refType"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private DBObject parseXmlObject(Type refType, XElement val)
        {
            var listType = refType;
            //var refObject = Activator.CreateInstance(listType) as DBObject;

            #region Create new instance or use an existing one

            var DBObjectUUID = GetUUIDFromXML(val);
            DBObject refObject = getObject(DBObjectUUID, listType);

            #endregion

            foreach (var attr in val.Elements("attribute"))
            {
                ApplyAttributeToObjectFromXML(listType, refObject, attr.Attributes("name").First().Value, attr);
            }
            return refObject;
        }

        #endregion

        #region The direct QueryResult with DBObjectReadout - fix me

        private IEnumerable<T> FromQueryResult<T>()
            where T : DBObject, new()
        {

            var theType = typeof(T);
            var retVal = new List<T>();

            #region Find the SelectionList to type T

            var selection = (from sel in _QueryResult.Results where sel.Type.Name == theType.Name select sel).FirstOrDefault();

            foreach (var elem in selection.Objects)
            {
                if (!visitedDBOs.ContainsKey(typeof(T)))
                    visitedDBOs.Add(typeof(T), new Dictionary<string, DBObject>());

                T newDBO;

                #region Create new instance or use an existing one

                if (visitedDBOs[typeof(T)].ContainsKey((String)elem.Attributes["UUID"]))
                {
                    newDBO = visitedDBOs[typeof(T)][(String)elem.Attributes["UUID"]] as T;
                }
                else
                {
                    newDBO = new T();
                    visitedDBOs[typeof(T)].Add((String)elem.Attributes["UUID"], newDBO);
                }

                #endregion

                foreach (var attr in elem.Attributes)
                {
                    ApplyAttributeToObject(theType, newDBO, attr.Key, attr.Value);
                }
                retVal.Add(newDBO);
            }

            #endregion

            //_QueryResult.SelectionList[0].Type
            return retVal;
        }

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

                    if (!visitedDBOs.ContainsKey(listType))
                        visitedDBOs.Add(listType, new Dictionary<string, DBObject>());

                    DBObject refObject;
                    if (visitedDBOs[listType].ContainsKey((String)val.Attributes["UUID"]))
                    {
                        refObject = visitedDBOs[listType][(String)val.Attributes["UUID"]] as DBObject;
                    }
                    else
                    {
                        refObject = Activator.CreateInstance(listType) as DBObject;
                        visitedDBOs[listType].Add((String)val.Attributes["UUID"], refObject);
                    }

                    #endregion

                    foreach (var attr in (val as DBObjectReadout).Attributes)
                    {
                        ApplyAttributeToObject(listType, refObject, attr.Key, attr.Value);
                    }

                    propVal.Add(refObject);
                }

                curProp.SetValue(myDBObject, propVal, null);
            }
            else if (myAttributeValue is DBObjectReadout)
            {
                var refType = curProp.PropertyType;
                var refObject = Activator.CreateInstance(refType) as DBObject;

                foreach (var attr in (myAttributeValue as DBObjectReadout).Attributes)
                {
                    ApplyAttributeToObject(refType, refObject, attr.Key, attr.Value);
                }
                curProp.SetValue(myDBObject, refObject, null);
            }
            else
            {
                curProp.SetValue(myDBObject, myAttributeValue, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null);
            }
        }

        #endregion

    }
}
