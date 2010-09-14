/*
 * Exceptional/Exceptional<TValue>
 * (c) Achim Friedland, 2009 - 2010
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

        #region OK

        private static readonly Exceptional _OK = new Exceptional();

        /// <summary>
        /// A static and empty Exceptional for successful return values.
        /// </summary>
        public static Exceptional OK
        {
            get
            {
                return _OK;
            }
        }

        #endregion

        #region IWarnings

        protected Stack<IWarning> _IWarnings;

        /// <summary>
        /// A list of exceptions that might have been thrown while determining the actual value of T.
        /// It is a list, as within an expression tree there might occure more than one exception.
        /// </summary>
        public IEnumerable<IWarning> IWarnings
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
        public IEnumerable<IError> IErrors
        {
            get
            {
                return _IErrors;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region Exceptional()

        public Exceptional()
        {
            _IWarnings  = new Stack<IWarning>();
            _IErrors    = new Stack<IError>();
        }

        #endregion

        #region Exceptional(myIWarning)

        /// <summary>
        /// Init using a single warning.
        /// </summary>
        public Exceptional(IWarning myIWarning)
            : this()
        {
            PushIWarning(myIWarning);
        }

        #endregion

        #region Exceptional(myIWarnings)

        /// <summary>
        /// Init using a list of warnings.
        /// </summary>
        public Exceptional(IEnumerable<IWarning> myIWarnings)
            : this()
        {
            PushIWarnings(myIWarnings);
        }

        #endregion

        #region Exceptional(myIError)

        /// <summary>
        /// Init using a single error.
        /// </summary>
        public Exceptional(IError myIError)
            : this()
        {
            PushIError(myIError);
        }

        #endregion

        #region Exceptional(myIErrors)

        /// <summary>
        /// Init using a list of errors.
        /// </summary>
        public Exceptional(IEnumerable<IError> myIErrors)
            : this()
        {
            PushIErrors(myIErrors);
        }

        #endregion

        #region Exceptional(myIExceptional)

        /// <summary>
        /// Init using a single exceptional.
        /// </summary>
        public Exceptional(IExceptional myIExceptional)
            : this()
        {
            PushIExceptional(myIExceptional);
        }

        #endregion

        #endregion


        #region Push(IWarning(s)/IError(s)/IExceptional)

        #region PushIWarning(myIWarning)

        /// <summary>
        /// Adds a single warning.
        /// </summary>
        public Exceptional PushIWarning(IWarning myIWarning)
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
        public Exceptional PushIWarnings(IEnumerable<IWarning> myIWarnings)
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
        public Exceptional PushIError(IError myIError)
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
        public Exceptional PushIErrors(IEnumerable<IError> myIErrors)
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
        public Exceptional PushIExceptional(IExceptional myIExceptional)
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

            if (_IErrors.Any())
                return "[Failed] " + GetIErrorsAsString();

            return String.Format("{0} error(s), {1} warning(s) occured!", _IErrors.Count(), _IWarnings.Count());

        }

        #endregion

        #endregion


        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion

    }


    /// <summary>
    /// Exceptional&lt;TValue/gt; holds a value of type TValue and errors that might have
    /// been errored while determining the actual value of TValue.
    /// For more information on this idea please watch the following MSDN Channel 9 video:
    /// http://channel9.msdn.com/shows/Going+Deep/E2E-Erik-Meijer-and-Burton-Smith-Concurrency-Parallelism-and-Programming/
    /// </summary>
    /// <typeparam name="TValue">the type of the encapsulated value</typeparam>
    public class Exceptional<TValue> : Exceptional, IExceptional<TValue>
    {

        #region Properties

        #region Value

        private TValue _Value;

        /// <summary>
        /// The encapsulated value.
        /// </summary>
        public TValue Value
        {
            get
            {
                return _Value;
            }

            set
            {
                _Value = value;
            }

        }        

        #endregion

        #endregion

        #region Constructor(s)

        #region Exceptional()

        public Exceptional()
            : base()
        {
            _Value      = default(TValue);
        }

        #endregion

        #region Exceptional(myValue)

        /// <summary>
        /// Adds the given value
        /// </summary>
        public Exceptional(TValue myValue)
            : this()
        {
            _Value = myValue;
        }

        #endregion

        #region Exceptional(myIWarning)

        /// <summary>
        /// Init using a single warning.
        /// </summary>
        public Exceptional(IWarning myIWarning)
            : base(myIWarning)
        {
        }

        #endregion

        #region Exceptional(myIWarnings)

        /// <summary>
        /// Init using a list of warnings.
        /// </summary>
        public Exceptional(IEnumerable<IWarning> myIWarnings)
            : base(myIWarnings)
        {
        }

        #endregion

        #region Exceptional(myValue, myIWarning)

        /// <summary>
        /// Adds the given value and warning.
        /// </summary>
        public Exceptional(TValue myValue, IWarning myIWarning)
            : this(myIWarning)
        {
            _Value = myValue;
        }

        #endregion

        #region Exceptional(myValue, myIWarnings)

        /// <summary>
        /// Adds the given value and list of warnings.
        /// </summary>
        public Exceptional(TValue myValue, IEnumerable<IWarning> myIWarnings)
            : this(myIWarnings)
        {
            _Value = myValue;
        }

        #endregion

        #region Exceptional(myIError)

        /// <summary>
        /// Init using a single error.
        /// </summary>
        public Exceptional(IError myIError)
            : base(myIError)
        {
        }

        #endregion

        #region Exceptional(myIErrors)

        /// <summary>
        /// Init using a list of errors.
        /// </summary>
        public Exceptional(IEnumerable<IError> myIErrors)
            : base(myIErrors)
        {
        }

        #endregion

        #region Exceptional(myIExceptional)

        /// <summary>
        /// Init using a single exceptional.
        /// </summary>
        public Exceptional(IExceptional myIExceptional)
            : base(myIExceptional)
        {
        }

        #endregion

        #region Exceptional(myValue, myExceptional)

        /// <summary>
        /// Init using the given value and a single exceptional.
        /// </summary>
        public Exceptional(TValue myValue, IExceptional myIExceptional)
            : this(myIExceptional)
        {
            _Value = myValue;
        }

        #endregion

        #endregion


        #region PushT(IWarning(s)/IError(s)/IExceptional)

        #region PushIWarningT(myIWarning)

        /// <summary>
        /// Adds a single warning.
        /// </summary>
        public Exceptional<TValue> PushIWarningT(IWarning myIWarning)
        {
            lock (this)
            {
                base.PushIWarning(myIWarning);
                return this;
            }
        }

        #endregion

        #region PushIWarningsT(myIWarnings)

        /// <summary>
        /// Adds a list of warnings.
        /// </summary>
        public Exceptional<TValue> PushIWarningsT(IEnumerable<IWarning> myIWarnings)
        {
            lock (this)
            {
                base.PushIWarnings(myIWarnings);
                return this;
            }
        }

        #endregion

        #region PushIErrorT(myIError)

        /// <summary>
        /// Adds a single error.
        /// </summary>
        public Exceptional<TValue> PushIErrorT(IError myIError)
        {
            lock (this)
            {
                base.PushIError(myIError);
                return this;
            }
        }

        #endregion

        #region PushIErrorsT(myIErrors)

        /// <summary>
        /// Adds a list of errors.
        /// </summary>
        public Exceptional<TValue> PushIErrorsT(IEnumerable<IError> myIErrors)
        {
            lock (this)
            {
                base.PushIErrors(myIErrors);
                return this;
            }
        }

        #endregion

        #region PushIExceptionalT(myExceptional)

        /// <summary>
        /// Adds the given exceptional.
        /// </summary>
        public Exceptional<TValue> PushIExceptionalT(IExceptional myIExceptional)
        {
            lock (this)
            {
                base.PushIExceptional(myIExceptional);
                return this;
            }
        }

        #endregion

        #endregion


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

            if (_IErrors.Any())
                return "[Failed] " + GetIErrorsAsString();

            if (_Value != null)
                return "[Success] " + _Value.ToString();

            return "[Success] <null>";

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

