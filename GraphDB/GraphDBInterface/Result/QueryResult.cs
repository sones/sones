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
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDB.Result
{

    /// <summary>
    /// This class hold all the data that comes out of the database after a query is run
    /// </summary>
    public class QueryResult : IEnumerable<Vertex>, IDisposable
    {

        #region Properties

        #region ResultType

        public ResultType ResultType
        {
            get
            {
                
                if (Success)
                    return ResultType.Successful;
                
                if (PartialSuccess)
                    return ResultType.PartialSuccessful;

                return ResultType.Failed;

            }
        }

        #endregion

        #region Success

        public Boolean Success
        {
            get
            {
                return !_IErrors.Any() && (!_IWarnings.Any());
            }
        }

        #endregion

        #region PartialSuccess

        public Boolean PartialSuccess
        {
            get
            {
                return (!_IErrors.Any() && _IWarnings.Any());
            }
        }

        #endregion

        #region Failed

        public Boolean Failed
        {
            get
            {
                return _IErrors.Any();
            }
        }

        #endregion


        #region IWarnings

        protected Stack<IWarning> _IWarnings;

        /// <summary>
        /// A list of exceptions that might have been thrown while determining the actual value of T.
        /// It is a list, as within an expression tree there might occure more than one exception.
        /// </summary>
        public IEnumerable<IWarning> Warnings
        {
            get
            {
                return _IWarnings;
            }
        }

        #endregion

        #region IErrors

        protected Stack<IError> _IErrors;

        /// <summary>
        /// A list of exceptions that might have been thrown while determining the actual value of T.
        /// It is a list, as within an expression tree there might occure more than one exception.
        /// </summary>
        public IEnumerable<IError> Errors
        {
            get
            {
                return _IErrors;
            }
        }

        #endregion


        #region Vertices

        public IEnumerable<Vertex> Vertices { get; set; }

        #endregion

        #region Query

        public String Query { get; set; }

        #endregion

        #region Duration

        public UInt64 Duration { get; set; }

        #endregion

        #region NumberOfAffectedVertices

        public UInt64 NumberOfAffectedVertices
        {
            get
            {

                var _NumberOfAffectedVertices = 0UL;

                if (Vertices != null)
                        _NumberOfAffectedVertices = (UInt64) Vertices.Count();

                return _NumberOfAffectedVertices;

            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region QueryResult()

        public QueryResult()
        {
            _IErrors    = new Stack<IError>();
            _IWarnings  = new Stack<IWarning>();
            Vertices     = new List<Vertex>();
        }

        #endregion

        #region QueryResult(myIWarning)

        /// <summary>
        /// Init using a single warning.
        /// </summary>
        public QueryResult(IWarning myIWarning)
            : this()
        {
            PushIWarning(myIWarning);
        }

        #endregion

        #region QueryResult(myIWarnings)

        /// <summary>
        /// Init using a list of warnings.
        /// </summary>
        public QueryResult(IEnumerable<IWarning> myIWarnings)
            : this()
        {
            PushIWarnings(myIWarnings);
        }

        #endregion

        #region QueryResult(myIError)

        public QueryResult(IError myIError)
            : this()
        {
            PushIError(myIError);
        }

        #endregion

        #region QueryResult(myIErrors)

        public QueryResult(IEnumerable<IError> myIErrors)
            : this()
        {
            PushIErrors(myIErrors);
        }

        #endregion

        #region QueryResult(myIExceptional)

        public QueryResult(IExceptional myIExceptional)
            : this()
        {
            PushIExceptional(myIExceptional);
        }

        #endregion

        #region QueryResult(myIErrors, myIWarnings)

        public QueryResult(IEnumerable<IError> myIErrors, IEnumerable<IWarning> myIWarnings)
            : this()
        {
            PushIErrors(myIErrors);
            PushIWarnings(myIWarnings);
        }

        #endregion

        #region QueryResult(myVertex, myIWarnings = null)

        public QueryResult(Vertex myVertex, IEnumerable<IWarning> myIWarnings = null)
            : this()
        {

            PushIWarnings(myIWarnings);

            if (myVertex != null)
                Vertices = new List<Vertex>{ myVertex };

        }

        #endregion

        #region QueryResult(myVertices, myIWarnings = null)

        public QueryResult(IEnumerable<Vertex> myVertices, IEnumerable<IWarning> myIWarnings = null)
            : this()
        {

            PushIWarnings(myIWarnings);

            if (myVertices != null)
                Vertices = myVertices;

        }

        #endregion

        #endregion


        #region Push(IWarning(s)/IError(s)/IExceptional)

        #region PushIWarning(myIWarning)

        /// <summary>
        /// Adds a single warning.
        /// </summary>
        public QueryResult PushIWarning(IWarning myIWarning)
        {
            lock (this)
            {
                _IWarnings.Push(myIWarning);
                return this;
            }
        }

        #endregion

        #region PushIWarnings(myIWarnings)

        /// <summary>
        /// Adds a list of warnings.
        /// </summary>
        public QueryResult PushIWarnings(IEnumerable<IWarning> myIWarnings)
        {
            lock (this)
            {

                if (myIWarnings != null && myIWarnings.Any())
                    foreach (var _Warning in myIWarnings.ToList().Reverse<IWarning>())
                        _IWarnings.Push(_Warning);

                return this;

            }
        }

        #endregion

        #region PushIError(myIError)

        /// <summary>
        /// Adds a single error.
        /// </summary>
        public QueryResult PushIError(IError myIError)
        {
            lock (this)
            {
                _IErrors.Push(myIError);
                return this;
            }
        }

        #endregion

        #region PushIErrors(myIErrors)

        /// <summary>
        /// Adds a list of errors.
        /// </summary>
        public QueryResult PushIErrors(IEnumerable<IError> myIErrors)
        {
            lock (this)
            {

                if (myIErrors != null && myIErrors.Any())
                    foreach (var _Error in myIErrors.ToList().Reverse<IError>())
                        _IErrors.Push(_Error);

                return this;

            }
        }

        #endregion

        #region PushIExceptional(myIExceptional)

        /// <summary>
        /// Adds the given exceptional.
        /// </summary>
        public QueryResult PushIExceptional(IExceptional myIExceptional)
        {
            lock (this)
            {

                // Add warnings...
                if (myIExceptional.IWarnings != null && myIExceptional.IWarnings.Any())
                    foreach (var _Warning in myIExceptional.IWarnings.ToList().Reverse<IWarning>())
                        _IWarnings.Push(_Warning);

                // Add errors...
                if (myIExceptional.IErrors != null && myIExceptional.IErrors.Any())
                    foreach (var _Error in myIExceptional.IErrors.ToList().Reverse<IError>())
                        _IErrors.Push(_Error);

                return this;

            }
        }

        #endregion

        #endregion



        #region this[myAttribute]

        /// <summary>
        /// Collects the values of the given AttributeName from all vertices.
        /// </summary>
        /// <param name="myAttribute"></param>
        /// <returns></returns>
        [Obsolete("Do no longer use this!")]
        public Object this[String myAttribute]
        {
            get
            {

                if (Vertices != null)
                {

                    if (Vertices.Count() == 1)
                    {

                        if (Vertices.First().HasAttribute(myAttribute))
                            return Vertices.First().GetProperty(myAttribute);
                        else
                            return null;

                    }

                    if (Vertices.Count() > 1)
                    {

                        var _ReturnValue = new List<Object>();

                        foreach (var _Vertex in Vertices)
                                if (_Vertex.HasAttribute(myAttribute))
                                    _ReturnValue.Add(_Vertex.GetProperty(myAttribute));

                        return _ReturnValue;

                    }

                }

                return null;

            }
        }

        #endregion

        #region GetResults<T>(myAttribute)

        /// <summary>
        /// This will return the given attribute of the first DBObject of the given
        /// selectionelement or it will return null.
        /// </summary>
        /// <param name="myAttribute"></param>
        /// <returns></returns>
        [Obsolete("Use _QueryResult.First()...")]
        public IEnumerable<T> GetResults<T>(String myAttribute)
        {

            if (Vertices != null)
                if (Vertices != null)
                    if (Vertices != null)
                        if (Vertices.Count() > 0)
                        {

                            var _ReturnValue = new List<T>();

                            foreach (var _Vertex in Vertices)
                                if (_Vertex.HasAttribute(myAttribute))
                                {

                                    T tmp;

                                    try
                                    {
                                        tmp = _Vertex.GetProperty<T>(myAttribute);
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

        #region SearchVertex(params myAttributes)

        /// <summary>
        /// Search for a DBObjectReadout with the specified attribute <paramref name="attributeName"/> and <paramref name="attributeValue"/> and return it.
        /// For more than 1 result an exception will be thrown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selection"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>Null if no element was found or the element itself</returns>
        public Vertex SearchVertex(params Tuple<String, Object>[] myAttributes)
        {

            var _Vertices = Vertices.Where(_Vertex =>
                    {
                        return myAttributes.Any(t => _Vertex.HasAttribute(t.Item1) && _Vertex.GetProperty(t.Item1).Equals(t.Item2));
                    }
                );
            
            return _Vertices.FirstOrDefault();

        }

        #endregion


        #region IEnumerable<Vertex> Members

        public IEnumerator<Vertex> GetEnumerator()
        {
            return Vertices.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Vertices.GetEnumerator();
        }

        #endregion


        #region ToString()

        #region GetIWarningsAsString()

        public String GetIWarningsAsString()
        {

            if (_IWarnings == null || !_IWarnings.Any())
                return String.Empty;

            var _StringBuilder = new StringBuilder();

            foreach (var _Warning in _IWarnings)
                _StringBuilder.AppendLine(String.Format("{0} => {1}", _Warning.GetType().Name, _Warning.ToString()));

            return _StringBuilder.ToString();

        }

        #endregion

        #region GetIErrorsAsString()

        public String GetIErrorsAsString()
        {

            if (_IErrors == null || !_IErrors.Any())
                return String.Empty;

            var _StringBuilder = new StringBuilder();

            foreach (var _IError in _IErrors)
                _StringBuilder.AppendLine(String.Format("{0} => {1}", _IError.GetType().Name, _IError.ToString()));

            return _StringBuilder.ToString();

        }

        #endregion

        #region ToString()

        public override String ToString()
        {
            return ResultType.ToString() + ", " + NumberOfAffectedVertices.ToString() + " Vertices affected";
        }

        #endregion

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        #endregion

    }

}
