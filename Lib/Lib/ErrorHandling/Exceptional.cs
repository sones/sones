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
 * Exceptional/Exceptional<TValue>
 * Achim Friedland, 2009-2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

#endregion

namespace sones.Lib.ErrorHandling
{

    /// <summary>
    /// The Exceptional class holds a list of errors occured
    /// </summary>
    public class Exceptional : IExceptional, IDisposable
    {

        #region Properties

        #region Succeess

        /// <summary>
        /// Is true, if there were no errors
        /// </summary>
        public Boolean Success
        {
            get
            {
                CheckForErrors = true;  //HACK: Rethink this!
                return !Failed && Warnings.IsNullOrEmpty();
            }
        }

        #endregion

        #region Failed

        /// <summary>
        /// Is true, if there were at least one error
        /// </summary>
        public Boolean Failed
        {
            get
            {
                CheckForErrors = true;  //HACK: Rethink this!
                return !_IErrors.IsNullOrEmpty();
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

        #region Warnings

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

        #region OK

        private static readonly Exceptional _OK = new Exceptional();

        /// <summary>
        /// This is an empty Exceptional for a successful return and must never be used for errors or warnings.
        /// </summary>
        public static Exceptional OK
        {
            get
            {
                return _OK;
            }
        }

        #endregion

        #region CheckForErrors

        public Boolean CheckForErrors { get; protected set; }

        #endregion

        #endregion

        #region Constructors

        #region Exceptional()

        public Exceptional()
        {
            CheckForErrors = false; //HACK: Rethink this!
            _IErrors    = new Stack<IError>();
            _IWarnings  = new Stack<IWarning>();
        }

        #endregion

        #region Exceptional(myIError)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myIError"></param>
        public Exceptional(IError myIError)
            : this()
        {
            _IErrors.Push(myIError);
        }

        public Exceptional(IEnumerable<IError> myIErrors)
            : this()
        {
            foreach (var err in myIErrors)
            {
                _IErrors.Push(err);
            }
        }

        #endregion

        #region Exceptional(myIWarning)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myIError"></param>
        public Exceptional(IWarning myIWarning)
            : this()
        {
            _IWarnings.Push(myIWarning);
        }

        #endregion

        #region Exceptional(myExceptional)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myIErrors"></param>
        public Exceptional(Exceptional myExceptional)
            : this()
        {

            if (myExceptional.Errors != null && myExceptional.Errors.Count() > 0)
                foreach (var _Error in myExceptional.Errors.ToList().Reverse<IError>())
                    _IErrors.Push(_Error);

            if (myExceptional.Warnings != null && myExceptional.Warnings.Count() > 0)
                foreach (var _Warning in myExceptional.Warnings.ToList().Reverse<IWarning>())
                    _IWarnings.Push(_Warning);

        }

        #endregion

        #endregion


        #region Push

        #region Push(myIError)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myException"></param>
        public Exceptional Push(IError myIError)
        {
            lock (this)
            {
                _IErrors.Push(myIError);
                return this;
            }
        }

        #endregion

        #region Push(myWarning)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myException"></param>
        public Exceptional Push(IWarning myIWarning)
        {
            lock (this)
            {
                _IWarnings.Push(myIWarning);
                return this;
            }
        }

        #endregion

        #region Push(exceptional)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myException"></param>
        public Exceptional Push(Exceptional exceptional)
        {
            lock (this)
            {
                foreach (var err in exceptional.Errors)
                {
                    _IErrors.Push(err);
                }
                foreach (var warn in exceptional.Warnings)
                {
                    _IWarnings.Push(warn);
                }
                return this;
            }
        }

        #endregion

        #endregion


        #region ToString()

        public override String ToString()
        {
            return String.Format("{0} errors occured!", _IErrors.Count());
        }

        #endregion

        #region GetErrorsAsString()

        public String GetErrorsAsString()
        {

            if (_IErrors == null || _IErrors.Count() == 0)
                return String.Empty;

            var _StringBuilder = new StringBuilder();

            foreach (var _IError in _IErrors)
                _StringBuilder.AppendLine(String.Format("{0} => {1}", _IError.GetType().Name, _IError.ToString()));

            return _StringBuilder.ToString();

        }

        #endregion

        #region GetWarningsAsString()

        public String GetWarningsAsString()
        {

            if (_IWarnings == null || _IWarnings.Count() == 0)
                return String.Empty;

            var _StringBuilder = new StringBuilder();

            foreach (var _Warning in _IWarnings)
                _StringBuilder.AppendLine(String.Format("{0} => {1}", _Warning.GetType().Name, _Warning.ToString()));

            return _StringBuilder.ToString();

        }

        #endregion


        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion

        #region AddErrorsAndWarnings

        public void AddErrorsAndWarnings(Exceptional exceptional)
        {
            if (_IErrors.IsNullOrEmpty())
            {
                _IErrors = exceptional.Errors as Stack<IError>;
            }
            else
            {
                foreach (var e in exceptional.Errors)
                {
                    _IErrors.Push(e);
                }
            }
            if (_IWarnings.IsNullOrEmpty())
            {
                _IWarnings = exceptional.Warnings as Stack<IWarning>;
            }
            else
            {
                foreach (var w in exceptional.Warnings)
                {
                    _IWarnings.Push(w);
                }
            }
        }

        #endregion

    }


    /// <summary>
    /// Exceptional&lt;TValue/gt; holds a value of type TValue and errors that might have
    /// been errored while determining the actual value of TValue.
    /// For more information on this idea please watch the following MSDN Channel 9 video:
    /// http://channel9.msdn.com/shows/Going+Deep/E2E-Erik-Meijer-and-Burton-Smith-Concurrency-Parallelism-and-Programming/
    /// </summary>
    /// <typeparam name="TValue">the type of the value</typeparam>
    public class Exceptional<TValue> : Exceptional, IExceptional<TValue>
    {

        #region Properties

        #region Value

        private TValue _Value;

        public TValue Value
        {
            get
            {
                #if __WithCheckExceptionals__
                if(CheckForErrors == false)
                    throw new Exception("The result of the exception was not checked.");
                #endif

                return _Value;
            }

            set
            {
                _Value = value;
//                _Exceptions.Clear();  //ahzf: I don't think that this is usefull!
            }

        }        

        #endregion

        #endregion

        #region Constructors

        #region Exceptional()

        public Exceptional()
        {
            _IErrors    = new Stack<IError>();
            _IWarnings  = new Stack<IWarning>();
            _Value      = default(TValue);
        }

        #endregion

        #region Exceptional(myValue)

        /// <summary>
        /// This will internal set Failed=False and Status=True
        /// </summary>
        /// <param name="myValue"></param>
        public Exceptional(TValue myValue)
            : this()
        {
            _Value = myValue;
        }

        #endregion

        #region Exceptional(myIError)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myException"></param>
        public Exceptional(IError myIError)
            : this()
        {
            _IErrors.Push(myIError);
        }

        #endregion

        #region Exceptional(myIErrors)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myException"></param>
        public Exceptional(IEnumerable<IError> myIErrors)
            : this()
        {
            foreach (var error in myIErrors)
            {
                _IErrors.Push(error);
            }
        }

        #endregion

        #region Exceptional(myIWarning)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myIError"></param>
        public Exceptional(IWarning myIWarning)
            : this()
        {
            _IWarnings.Push(myIWarning);
        }

        #endregion

        #region Exceptional(myValue, myIWarning)

        /// <summary>
        /// This will internal set Failed=False and Status=True
        /// </summary>
        /// <param name="myValue"></param>
        /// <param name="myIWarning"></param>
        public Exceptional(TValue myValue, IWarning myIWarning)
            : this(myIWarning)
        {
            _Value = myValue;
        }

        #endregion

        #region Exceptional(myExceptional)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myExceptional"></param>
        public Exceptional(Exceptional myExceptional)
            : this()
        {

            if (myExceptional.Errors != null && myExceptional.Errors.Count() > 0)
                foreach (var _Error in myExceptional.Errors.ToList().Reverse<IError>())
                    _IErrors.Push(_Error);

            if (myExceptional.Warnings != null && myExceptional.Warnings.Count() > 0)
                foreach (var _Warning in myExceptional.Warnings.ToList().Reverse<IWarning>())
                    _IWarnings.Push(_Warning);

        }

        #endregion

        #region Exceptional(myValue, myExceptional)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myValue"></param>
        /// <param name="myExceptional"></param>
        public Exceptional(TValue myValue, Exceptional myExceptional)
            : this(myExceptional)
        {
            _Value = myValue;
        }

        #endregion

        #endregion


        #region PushT

        #region Push(myExceptional)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myException"></param>
        public Exceptional<TValue> Push(Exceptional myExceptional)
        {
            lock (this)
            {
                AddErrorsAndWarnings(myExceptional);
                return this;
            }
        }

        #endregion

        #region PushT(myIError)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myException"></param>
        public Exceptional<TValue> PushT(IError myIError)
        {
            lock (this)
            {
                _IErrors.Push(myIError);
                return this;
            }
        }

        #endregion

        #region PushT(myWarning)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myException"></param>
        public Exceptional<TValue> PushT(IWarning myIWarning)
        {
            lock (this)
            {
                _IWarnings.Push(myIWarning);
                return this;
            }
        }

        #endregion

        #endregion

        //HACK: Disabled because of nasty side effects!
        //#region Implicit conversation to/from TValue

        //public static implicit operator Exceptional<TValue>(TValue myValue)
        //{
        //    return new Exceptional<TValue>(myValue);
        //}

        //public static implicit operator TValue(Exceptional<TValue> myExceptional)
        //{
        //    return myExceptional.Value;
        //}

        //#endregion


        #region Equals(myObject)

        public override Boolean Equals(Object myObject)
        {
            return _Value.Equals(myObject);
        }

        #endregion

        #region GetHashCode()

        public override Int32 GetHashCode()
        {
            return _Value.GetHashCode();
        }

        #endregion

        #region ToString()

        public override String ToString()
        {

            if (Success)
            {

                if (_Value != null)
                    return _Value.ToString();

                return "[Success]";

            }

            return "[Failed] " + GetErrorsAsString();

        }

        #endregion


        #region IDisposable Members

        public override void Dispose()
        {
            _Value = default(TValue);
        }

        #endregion

    }

}

