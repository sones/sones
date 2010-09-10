/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* <id name="GraphDB – outer Query Result" />
 * <copyright file="QueryResult.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Martin Junghanns</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using sones.Lib;
using sones.Lib.ErrorHandling;


#endregion

namespace sones.GraphDBInterface.Result
{

    /// <summary>
    /// This class hold all the data that comes out of the database after a query is run
    /// </summary>
    public class QueryResult : DynamicObject, IEnumerable<DBObjectReadout>, IDisposable
    {

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

        #region Success

        public Boolean Success
        {
            get
            {
                return _ResultType == ResultType.Successful;
            }
        }

        #endregion

        #region PartialSuccess

        public Boolean PartialSuccess
        {
            get
            {
                return _ResultType == ResultType.PartialSuccessful;
            }
        }

        #endregion

        #region Failed

        public Boolean Failed
        {
            get
            {
                return _ResultType == ResultType.Failed;
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

        private SelectionResultSet _Results = null;

        public SelectionResultSet Results
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
                        _NumberOfAffectedDBObjects = Results.NumberOfAffectedObjects;

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
            _ResultType = ResultType.Successful;
            _Results = new SelectionResultSet();

            if (myErrors != null)
            {
                _Errors = GetIErrors(myErrors);
            }
            else
                _Errors = new List<IError>();

            if (myWarnings != null)
            {
                _Warnings = GetDBWarnings(myWarnings);
            }
            else
                _Warnings = new List<IWarning>();

            SetResultType();

        }

        public QueryResult(SelectionResultSet mySelectionListElementResult, IEnumerable<IWarning> myWarnings = null)
            : this()
        {

            if (myWarnings != null)
                _Warnings = GetDBWarnings(myWarnings);

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
                    if (_Results.Objects != null)
                    {
                        if (_Results.Objects.Count() == 1)
                        {
                            if (_Results.Objects.First().Attributes.ContainsKey(myAttribute))
                                return _Results.Objects.First().Attributes[myAttribute];
                            else
                                return null;
                        }

                        if (_Results.Objects.Count() > 1)
                        {

                            var _ReturnValue = new List<Object>();

                            foreach (var _DBObjectReadout in _Results.Objects)
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

        #region GetFirstResult<T>(mySelectionElement, myAttribute)

        /// <summary>
        /// This will return a list of resulsts for the given attribute of the given
        /// selectionelement or it will return null.
        /// </summary>
        /// <param name="myAttribute"></param>
        /// <returns></returns>
        public T GetFirstResult<T>(String myAttribute)
        {

            if (_Results != null)
                if (_Results != null)
                    if (_Results.Objects != null)
                        if (_Results.Objects.Count() > 0)
                            if (_Results.Objects.FirstOrDefault() != null)
                                if (_Results.Objects.First().Attributes.ContainsKey(myAttribute))
                                {

                                    T tmp;

                                    try
                                    {
                                        tmp = (T)_Results.Objects.First().Attributes[myAttribute];
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

        #region GetResults<T>(mySelectionElement, myAttribute)

        /// <summary>
        /// This will return the given attribute of the first DBObject of the given
        /// selectionelement or it will return null.
        /// </summary>
        /// <param name="myAttribute"></param>
        /// <returns></returns>
        public List<T> GetResults<T>(String myAttribute)
        {

            if (_Results != null)
                if (_Results != null)
                    if (_Results.Objects != null)
                        if (_Results.Objects.Count() > 0)
                        {

                            List<T> _ReturnValue = new List<T>();

                            foreach (var _DBObjectReadout in _Results.Objects)
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

        #region AddWarning(myIWarning)

        public void AddWarning(IWarning myIWarning)
        {

            _Warnings.Add(myIWarning);

            if (_Warnings.Count > 0)
                _ResultType = ResultType.PartialSuccessful;

        }

        #endregion

        #region AddWarnings(myIWarnings)

        public void AddWarnings(IEnumerable<IWarning> myIWarnings)
        {

            _Warnings.AddRange(GetDBWarnings(myIWarnings));

            if (_Warnings.Count > 0)
                _ResultType = ResultType.PartialSuccessful;

        }

        #endregion

        #region AddErrors(myErrors)

        public void AddErrors(IEnumerable<IError> myErrors)
        {
            _Errors.AddRange(GetIErrors(myErrors));
            _ResultType = ResultType.Failed;
        }

        #endregion

        #region AddErrorsAndWarnings(myExceptional)

        public void AddErrorsAndWarnings(Exceptional myExceptional)
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

        public void SetResult(SelectionResultSet mySelectionResultSet)
        {
            _Results = mySelectionResultSet;
        }

        #endregion


        #region Members of DynamicObject

        #region TryGetMember(myBinder, out myResult)

        public override Boolean TryGetMember(GetMemberBinder myBinder, out Object myResult)
        {

            myResult = GetResults<Object>(myBinder.Name);

            return true;

        }

        #endregion

        #region TrySetMember(myBinder, myObject)

        public override Boolean TrySetMember(SetMemberBinder myBinder, Object myObject)
        {
            return true;

        }

        #endregion

        #endregion


        #region IEnumerable<DBObjectReadout> Members

        public IEnumerator<DBObjectReadout> GetEnumerator()
        {

            return _Results.Objects.GetEnumerator();

        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Results.Objects.GetEnumerator();
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

        #region private helper

        private List<IWarning> GetDBWarnings(IEnumerable<IWarning> myWarnings)
        {

            var result = new List<IWarning>();

            if (myWarnings != null)
            {
                foreach (var aWarning in myWarnings)
                {
                    result.Add(aWarning);
                }
            }

            return result;
        }

        private List<IError> GetIErrors(IEnumerable<IError> myErrors)
        {
            List<IError> result = new List<IError>();

            if (myErrors != null)
            {
                foreach (var aError in myErrors)
                {
                    result.Add((IError)aError);
                }

            }

            return result;
        }

        #endregion

    }

}
