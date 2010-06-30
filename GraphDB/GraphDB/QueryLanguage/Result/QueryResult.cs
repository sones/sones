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

/* <id name="sones GraphDB – outer Query Result" />
 * <copyright file="QueryResult.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Martin Junghanns</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Structures;
using sones.GraphDB.Warnings;
using sones.Lib;
using sones.Lib.CLI;
using sones.Lib.ErrorHandling;
using sones.Lib.XML;


#endregion

namespace sones.GraphDB.QueryLanguage.Result
{

    /// <summary>
    /// This class hold all the data that comes out of the database after a query is run
    /// </summary>
    public class QueryResult : DynamicObject, IEnumerable<DBObjectReadout>, IDisposable
    {


        #region Data

        //HashSet<String>            _StructuredData   = new HashSet<String>();
        //Dictionary<String, Object> _UnstructuredData = new Dictionary<String, Object>();

        #endregion

        #region Properties

        #region ResultType

        private ResultType _ResultType;

        public ResultType ResultType
        {
            get
            {
                return _ResultType;
            }
        }

        #endregion

        #region Errors

        private List<IError> _Errors = null;

        public IEnumerable<IError> Errors
        {
            get
            {
                return _Errors;
            }
        }

        #endregion

        #region Warnings

        private List<IWarning> _Warnings = null;

        public IEnumerable<IWarning> Warnings
        {
            get
            {
                return _Warnings;
            }
        }

        #endregion

        #region SelectionListResult

        private List<SelectionResultSet> _Results = null;

        public IEnumerable<SelectionResultSet> Results
        {
            get
            {
                return _Results;
            }
        }

        #endregion

        #region Query

        public String Query { get; set; }

        #endregion

        #region Duration

        public UInt64 Duration { get; set; }

        #endregion

        #region NumberOfAffectedDBObjects

        public UInt64 TotalNumberOfAffectedDBObjects
        {
            get
            {

                var _NumberOfAffectedDBObjects = 0UL;

                if (Results != null)
                    foreach (var _SelectionListElementResult in Results)
                        _NumberOfAffectedDBObjects += _SelectionListElementResult.NumberOfAffectedObjects;

                return _NumberOfAffectedDBObjects;

            }
        }

        #endregion

        #endregion

        #region Constructors

        public QueryResult(Exceptional exceptional)
            : this(exceptional.Errors, exceptional.Warnings)
        {
        }

        public QueryResult(IError error)
            : this(myErrors: new List<IError>() { error })
        { }

        public QueryResult(IWarning warning)
            : this(myErrors: new List<IError>(), myWarnings: new List<IWarning>() { warning })
        { }

        public QueryResult(IEnumerable<IError> myErrors = null, IEnumerable<IWarning> myWarnings = null)
        {               
            _ResultType = Structures.ResultType.Successful;
            _Results = new List<SelectionResultSet>();

            if (myErrors != null)
            {
                _Errors = myErrors.ToList();
            }
            else
                _Errors = new List<Lib.ErrorHandling.IError>();

            if (myWarnings != null)
            {
                _Warnings = myWarnings.ToList();
            }
            else
                _Warnings = new List<Lib.ErrorHandling.IWarning>();

            SetResultType();

        }
        
        public QueryResult(SelectionResultSet mySelectionListElementResult, IEnumerable<IWarning> myWarnings = null)
            : this(new List<SelectionResultSet>(){ mySelectionListElementResult }, myWarnings)
        { }

        public QueryResult(List<SelectionResultSet> mySelectionListElementResult, IEnumerable<IWarning> myWarnings = null)
            : this()
        {

            if (myWarnings != null)
                _Warnings = myWarnings.ToList();

            SetResultType();

            if (mySelectionListElementResult != null)
            {
                _Results = mySelectionListElementResult;
            }
        
        }
        
        private void SetResultType()
        {
            if (!_Errors.IsNullOrEmpty())
                _ResultType = ResultType.Failed;
            else if (!_Warnings.IsNullOrEmpty())
                _ResultType = ResultType.PartialSuccessful;
            else
                _ResultType = ResultType.Successful;
        }

        #endregion


        #region this[myAttribute]

        /// <summary>
        /// This will return the given attribute of the first DBObject of the first
        /// selectionelement or it will return null.
        /// </summary>
        /// <param name="myAttribute"></param>
        /// <returns></returns>
        public Object this[String myAttribute]
        {
            get
            {

                if (_Results != null)
                    if (_Results.Count > 0)
                        if (_Results[0] != null)
                            if (_Results[0].Objects != null)
                            {
                                if (_Results[0].Objects.Count() == 1)
                                {
                                    if (_Results[0].Objects.First().Attributes.ContainsKey(myAttribute))
                                        return _Results[0].Objects.First().Attributes[myAttribute];
                                    else
                                        return null;
                                }

                                if (_Results[0].Objects.Count() > 1)
                                {

                                    var _ReturnValue = new List<Object>();

                                    foreach (var _DBObjectReadout in _Results[0].Objects)
                                        if (_DBObjectReadout.Attributes != null)
                                            if (_DBObjectReadout.Attributes.ContainsKey(myAttribute))
                                                _ReturnValue.Add(_DBObjectReadout.Attributes[myAttribute]);

                                    return _ReturnValue;

                                }

                            }

                return null;

            }
        }

        #endregion

        #region this[mySelectionListElement]

        /// <summary>
        /// This will return a SelectionResultSet
        /// or it will return null.
        /// </summary>
        /// <param name="myAttribute"></param>
        /// <returns></returns>
        public SelectionResultSet this[Int32 mySelectionListElement]
        {
            get
            {

                if (_Results.Count > 0)
                    return _Results[mySelectionListElement];

                return null;

            }
        }

        #endregion


        #region GetFirstResult<T>(myAttribute)

        /// <summary>
        /// This will return a list of resulsts for the given attribute of the first
        /// selectionelement or it will return null.
        /// </summary>
        /// <param name="myAttribute"></param>
        /// <returns></returns>
        public T GetFirstResult<T>(String myAttribute)
        {
            return GetFirstResult<T>(0, myAttribute);
        }

        #endregion

        #region GetFirstResult<T>(mySelectionElement, myAttribute)

        /// <summary>
        /// This will return a list of resulsts for the given attribute of the given
        /// selectionelement or it will return null.
        /// </summary>
        /// <param name="myAttribute"></param>
        /// <returns></returns>
        public T GetFirstResult<T>(Int32 mySelectionElement, String myAttribute)
        {

            if (_Results != null)
                if (_Results.Count > mySelectionElement)
                    if (_Results[mySelectionElement] != null)
                        if (_Results[mySelectionElement].Objects != null)
                            if (_Results[mySelectionElement].Objects.Count() > 0)
                                if (_Results[mySelectionElement].Objects.FirstOrDefault() != null)
                                    if (_Results[mySelectionElement].Objects.First().Attributes.ContainsKey(myAttribute))
                                    {

                                        T tmp;

                                        try
                                        {
                                            tmp = (T)_Results[0].Objects.First().Attributes[myAttribute];
                                        }
                                        catch
                                        {
                                            return default(T);
                                        }

                                        return tmp;

                                    }


            return default(T);

        }

        #endregion

        #region GetResults<T>(myAttribute)

        /// <summary>
        /// This will return the given attribute of the first DBObject of the first
        /// selectionelement or it will return null.
        /// </summary>
        /// <param name="myAttribute"></param>
        /// <returns></returns>
        public List<T> GetResults<T>(String myAttribute)
        {
            return GetResults<T>(0, myAttribute);
        }

        #endregion

        #region GetResults<T>(mySelectionElement, myAttribute)

        /// <summary>
        /// This will return the given attribute of the first DBObject of the given
        /// selectionelement or it will return null.
        /// </summary>
        /// <param name="myAttribute"></param>
        /// <returns></returns>
        public List<T> GetResults<T>(Int32 mySelectionElement, String myAttribute)
        {

            if (_Results != null)
                if (_Results.Count > mySelectionElement)
                    if (_Results[mySelectionElement] != null)
                        if (_Results[mySelectionElement].Objects != null)
                            if (_Results[mySelectionElement].Objects.Count() > 0)
                            {

                                List<T> _ReturnValue = new List<T>();

                                foreach (DBObjectReadout _DBObjectReadout in _Results[mySelectionElement].Objects)
                                    if (_DBObjectReadout.Attributes != null)
                                        if (_DBObjectReadout.Attributes.ContainsKey(myAttribute))
                                        {

                                            T tmp;

                                            try
                                            {
                                                tmp = (T)_DBObjectReadout.Attributes[myAttribute];
                                                _ReturnValue.Add(tmp);
                                            }
                                            catch (InvalidCastException ice)
                                            {
                                                throw ice;
                                            }
                                            catch
                                            {
                                                // Ignore exception and go on adding the next element!
                                                //return default(List<T>);
                                            }

                                        }

                                return _ReturnValue;

                            }


            return default(List<T>);

        }

        #endregion

        #region GetFirstError<T>

        /// <summary>
        /// Returns the first error of the given type T
        /// </summary>
        /// <typeparam name="T">The type of the GraphDBError</typeparam>
        /// <returns>Any instance of error</returns>
        public T GetFirstError<T>()
            where T : GraphDBError
        {
            if (_Errors != null && _Errors.Count > 0)
            {
                //return Errors.GetFirstError<T>();
                return ((from err in Errors where err.GetType() == typeof(T) select err).FirstOrDefault() as T);
            }
            return null;
        }

        #endregion

        #region GetFirstWarning<T>

        /// <summary>
        /// Returns the first error of the given type T
        /// </summary>
        /// <typeparam name="T">The type of the GraphDBError</typeparam>
        /// <returns>Any instance of error</returns>
        public T GetFirstWarning<T>()
            where T : GraphDBWarning
        {
            if (_Warnings != null && _Warnings.Count > 0)
            {
                return ((from warning in Warnings where warning.GetType() == typeof(T) select warning).FirstOrDefault() as T);
            }
            return null;
        }

        #endregion

        #region GetWarningsAsString()

        public String GetWarningsAsString()
        {

            if (_Warnings == null || _Warnings.Count == 0)
                return String.Empty;

            return _Warnings.Aggregate("", (result, element) =>
            {
                result += String.Format("[{0}]\r\n{1}\r\n\r\n", element.GetType().Name, element.ToString());
                return result;
            });

        }

        #endregion

        #region GetErrorsAsString()

        public String GetErrorsAsString()
        {

            if (_Errors == null || _Errors.Count == 0)
                return GetWarningsAsString();

            return _Errors.Aggregate("", (result, element) =>
            {
                result += String.Format("[{0}]\r\n{1}\r\n\r\n", element.GetType().Name, element.ToString());
                return result;
            })

            + GetWarningsAsString();

        }

        #endregion


        #region ToSimpleResult

        public SimpleResult ToSimpleResult()
        {
            return this.ToSimpleResult(0);
        }

        #endregion

        #region ToSimpleResult(mySelectionListNumber)

        public SimpleResult ToSimpleResult(int mySelectionListNumber)
        {

            var sResult = new SimpleResult();

            if (this.ResultType != ResultType.Successful)
            {

                sResult.iResultType = SimpleResult.Failed;

                var errors = this.Errors.ToList();
                sResult.Errors = new List<String>();
                
                foreach (var error in errors)
                    sResult.Errors.Add(error.ToString());

                return sResult;

            }

            if (this.Results == null || _Results.Count == 0) { return sResult; }

            SelectionResultSet table = _Results[mySelectionListNumber];
            var lData = table.Objects;
            int iNr = 0;
            int iCount = 0;
            IDictionary<String, Object> dict = null;
            Object[] oLineData = null;

            sResult.Header = new List<KeyValuePair<String, Object>>();

            foreach (var _DBObjectReadout in lData)
            {

                dict = _DBObjectReadout.Attributes;
                if (iNr == 0)
                {
                    foreach (KeyValuePair<String, Object> attribute in dict)
                    {
                        sResult.Header.Add(new KeyValuePair<String, Object>(attribute.Key, attribute.Value));

                        //if (attribute.Value is String)        type = ""; 
                        //else if (attribute.Value is int)      type = 0; 
                        //else if (attribute.Value is double)   type = 0D; 
                        //else if (attribute.Value is DateTime) type = DateTime.Now; 
                        //else Console.WriteLine(attribute.Value.ToString());

                        //sResult.Header.Add(new KeyValuePair<String, Object>(attribute.Key, type));

                        iNr++;
                    }
                    sResult.Data = new List<object[]>();
                }
                if (iNr == 0) break;  // no attributes ???

                oLineData = new object[iNr];
                iCount = 0;
                
                foreach (var data in dict)
                {
                    if (iCount == iNr) break; // reached array end.....should not happen
                    oLineData[iCount] = data.Value;
                    iCount++;
                }
                
                sResult.Data.Add(oLineData);

            }

            sResult.iResultType = SimpleResult.Successful;
            // end values to be filled
            
            return sResult;

        }
        #endregion


        #region AddWarning(myIWarning)

        public void AddWarning(IWarning myIWarning)
        {

            _Warnings.Add(myIWarning);

            if (_Warnings.Count > 0)
                _ResultType = GraphDB.Structures.ResultType.PartialSuccessful;

        }

        #endregion

        #region AddWarnings(myIWarnings)

        public void AddWarnings(IEnumerable<IWarning> myIWarnings)
        {

            _Warnings.AddRange(myIWarnings);

            if (_Warnings.Count > 0)
                _ResultType = GraphDB.Structures.ResultType.PartialSuccessful;

        }

        #endregion

        #region AddErrors(myErrors)

        public void AddErrors(IEnumerable<IError> myErrors)
        {
            _Errors.AddRange(myErrors);
            _ResultType = Structures.ResultType.Failed;
        }

        #endregion

        #region AddErrorsAndWarnings(myExceptional)

        internal void AddErrorsAndWarnings(Exceptional myExceptional)
        {

            if (!myExceptional.Errors.IsNullOrEmpty())
            {
                AddErrors(myExceptional.Errors);
            }

            if (!myExceptional.Warnings.IsNullOrEmpty())
            {
                AddWarnings(myExceptional.Warnings);
            }

        }

        #endregion

        #region AddResult(SelectionResultSet selResultSet)

        public void AddResult(SelectionResultSet mySelectionResultSet)
        {
            _Results.Add(mySelectionResultSet);
        }

        #endregion


        #region Members of DynamicObject

        #region TryGetMember(myBinder, out myResult)

        public override Boolean TryGetMember(GetMemberBinder myBinder, out Object myResult)
        {

            //return _UnstructuredData.TryGetValue(myBinder.Name, out myResult);

            myResult = GetResults<Object>(0, myBinder.Name);

            return true;

        }

        #endregion

        #region TrySetMember(myBinder, myObject)

        public override Boolean TrySetMember(SetMemberBinder myBinder, Object myObject)
        {

            //if (_StructuredData.Contains(myBinder.Name))
            //    throw new ArgumentException("Invalid operation!");

            //if (_UnstructuredData.ContainsKey(myBinder.Name))
            //    _UnstructuredData[myBinder.Name] = myObject;

            //else
            //    _UnstructuredData.Add(myBinder.Name, myObject);

            return true;

        }

        #endregion

        #endregion

        #region IEnumerable<DBObjectReadout> Members

        public IEnumerator<DBObjectReadout> GetEnumerator()
        {

            if (_Results.Count > 0)
                return _Results[0].Objects.GetEnumerator();

            else
                return new List<DBObjectReadout>().GetEnumerator();

        }

        public IEnumerator<DBObjectReadout> GetEnumerator(Int32 mySelectionElementID)
        {
            return _Results[mySelectionElementID].Objects.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Results[0].Objects.GetEnumerator();
        }

        #endregion


        #region ToString()

        public override String ToString()
        {
            return _ResultType.ToString() + ", " + TotalNumberOfAffectedDBObjects.ToString() + " DBObject(s) affected";
        }

        #endregion

        #region ToErrorString()

        public String ToErrorString()
        {

            var _StringBuilder = new StringBuilder();

            foreach (var _ErrorString in _Errors)
                _StringBuilder.AppendLine(_ErrorString.Message);

            return _StringBuilder.ToString();

        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        #endregion

    }

}
