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

///*
// * FSExceptional
// * (c) Achim Friedland, 2010
// */

//#region Usings

//using System;
//using System.Text;
//using System.Collections.Generic;
//using sones.Lib.DataStructures.Exceptional;
//using sones.GraphFS.DataStructures;

//#endregion

//namespace sones.Pandora.Storage.PandoraFS.Objects
//{

//    public class FSExceptional : IExceptional
//    {

//        #region Properties

//        #region FSResult

//        private PandoraFSResult _FSResult;

//        public virtual PandoraFSResult FSResult
//        {

//            get
//            {
//                return _FSResult;
//            }

//            set
//            {
//                _FSResult = value;
//            }

//        }

//        #endregion

//        #region Success

//        Boolean _Success = true;

//        /// <summary>
//        /// Will return if there are any exceptions
//        /// </summary>
//        public Boolean Success
//        {
//            get
//            {
//                return _Success;
//            }
//        }

//        #endregion

//        #region Failed

//        /// <summary>
//        /// Will return if there are any exceptions
//        /// </summary>
//        public Boolean Failed
//        {
//            get
//            {
//                return !Success;
//            }
//        }

//        #endregion

//        #region Exceptions

//        protected List<Exception> _Exceptions;

//        /// <summary>
//        /// A list of exceptions that might have been thrown while determining the actual value of T.
//        /// It is a list, as within an expression tree there might occure more than one exception.
//        /// </summary>
//        public IEnumerable<Exception> Exceptions
//        {

//            get
//            {
//                return _Exceptions;
//            }

//            set
//            {
//                _Exceptions.AddRange(value);
//            }

//        }

//        #endregion

//        #region Exceptionlist

//        public String Exceptionlist
//        {
//            get
//            {

//                var _Exceptionlist = "";

//                foreach (var _Exception in _Exceptions)
//                    _Exceptionlist = String.Concat(_Exceptionlist, Environment.NewLine, _Exception.ToString());

//                return _Exceptionlist;

//            }
//        }

//        #endregion

//        #endregion


//        #region Constructors

//        #region FSExceptional()

//        public FSExceptional()
//        {
//            _Exceptions = new List<Exception>();
//        }

//        #endregion

//        #region FSExceptional(myException)

//        public FSExceptional(Exception myException)
//            : this()
//        {
//            Add(myException);
//        }

//        #endregion

//        #region FSExceptional(myExceptions)

//        public FSExceptional(IEnumerable<Exception> myExceptions)
//            : this()
//        {
//            Add(myExceptions);
//        }

//        #endregion

//        #endregion


//        #region Add

//        #region Add(myException)

//        /// <summary>
//        /// This will internal set Failed=True and Status=False
//        /// </summary>
//        /// <param name="myException"></param>
//        public void Add(Exception myException)
//        {
//            _Exceptions.Add(myException);
//            _Success = false;
//        }

//        #endregion

//        #region Add(myExceptions)

//        /// <summary>
//        /// This will internal set Failed=True and Status=False
//        /// </summary>
//        /// <param name="myExceptions"></param>
//        public void Add(IEnumerable<Exception> myExceptions)
//        {
//            _Exceptions.AddRange(myExceptions);
//            _Success = false;
//        }

//        #endregion

//        #region Add(myIExceptional)

//        /// <summary>
//        /// This will internal set Failed=True and Status=False
//        /// </summary>
//        /// <param name="myExceptional"></param>
//        public void Add(IExceptional myIExceptional)
//        {
//            Add(myIExceptional.Exceptions);
//        }

//        #endregion

//        #region Add(myIExceptionals)

//        /// <summary>
//        /// This will internal set Failed=True and Status=False
//        /// </summary>
//        /// <param name="myExceptionals"></param>
//        public void Add(IEnumerable<IExceptional> myIExceptionals)
//        {
//            foreach (var _IExceptional in myIExceptionals)
//                Add(_IExceptional.Exceptions);
//        }

//        #endregion

//        #endregion


//        #region ToString()

//        public override String ToString()
//        {
//            return Exceptionlist;
//        }

//        #endregion

//    }


//    public class FSExceptional<TValue> : FSExceptional, IExceptional<TValue>
//    {

//        #region Properties

//        #region Value

//        protected TValue _Value;

//        /// <summary>
//        /// The inner type
//        /// </summary>
//        public TValue Value
//        {

//            get
//            {
//                return _Value;
//            }

//            set
//            {
//                _Value = value;
//                //_Exceptions.Clear();
//            }

//        }

//        #endregion

//        #endregion


//        #region Constructors

//        #region FSExceptional()

//        public FSExceptional()
//        {
//            _Value      = default(TValue);
//            _Exceptions = new List<Exception>();
//        }

//        #endregion

//        #region FSExceptional(myValue)

//        public FSExceptional(TValue myValue)
//            : this()
//        {
//            _Value      = myValue;
//        }

//        #endregion

//        #region FSExceptional(myException)

//        public FSExceptional(Exception myException)
//            : this()
//        {
//            Add(myException);
//        }

//        #endregion

//        #region FSExceptional(myExceptions)

//        public FSExceptional(IEnumerable<Exception> myExceptions)
//            : this()
//        {
//            Add(myExceptions);
//        }

//        #endregion

//        #region FSExceptional(myValue, myException)

//        public FSExceptional(TValue myValue, Exception myException)
//            : this()
//        {
//            _Value = myValue;
//            Add(myException);
//        }

//        #endregion

//        #region FSExceptional(myValue, myExceptions)

//        public FSExceptional(TValue myValue, IEnumerable<Exception> myExceptions)
//            : this()
//        {
//            _Value = myValue;
//            Add(myExceptions);
//        }

//        #endregion

//        #endregion


//        #region Add

//        #region Add(myIExceptional)

//        /// <summary>
//        /// This will internal set Failed=True and Status=False
//        /// </summary>
//        /// <param name="myExceptional"></param>
//        public void Add(IExceptional<TValue> myIExceptional)
//        {
//            Add(myIExceptional.Exceptions);
//        }

//        #endregion

//        #region Add(myIExceptionals)

//        /// <summary>
//        /// This will internal set Failed=True and Status=False
//        /// </summary>
//        /// <param name="myExceptionals"></param>
//        public void Add(IEnumerable<IExceptional<TValue>> myIExceptionals)
//        {
//            foreach (var _IExceptional in myIExceptionals)
//                Add(_IExceptional.Exceptions);
//        }

//        #endregion

//        #endregion


//        #region Implicit conversation to/from TValue

//        public static implicit operator FSExceptional<TValue>(TValue myValue)
//        {
//            return new FSExceptional<TValue>(myValue);
//        }

//        public static implicit operator TValue(FSExceptional<TValue> myExceptional)
//        {
//            return myExceptional.Value;
//        }

//        #endregion

//        #region Equals(myObject)

//        public override Boolean Equals(Object myObject)
//        {
//            return _Value.Equals(myObject);
//        }

//        #endregion

//        #region GetHashCode()

//        public override int GetHashCode()
//        {
//            return _Value.GetHashCode();
//        }

//        #endregion

//        #region ToString()

//        public override String ToString()
//        {
//            return Success ? _Value.ToString() : "FAILED! " + _Value.ToString();
//        }

//        #endregion

//    }

//}
