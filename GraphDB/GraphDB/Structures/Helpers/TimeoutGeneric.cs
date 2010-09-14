using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using sones.GraphDB.Exceptions;

namespace sones.GraphDB.Query.Helpers
{

    public class TimeOutResult<T>
    {
        public TimeOutResult(Boolean timeoutExceeded)
        {
            _TimeoutExceeded = timeoutExceeded;
        }

        public TimeOutResult(T methodResult)
        {
            _MethodResult = methodResult;
            _TimeoutExceeded = false;
        }

        private T _MethodResult;
        public T MethodResult
        {
            get { return _MethodResult; }
        }

        private Boolean _TimeoutExceeded;
        public Boolean TimeoutExceeded
        {
            get { return _TimeoutExceeded; }
        }

        public static implicit operator Boolean(TimeOutResult<T> timeOutResult)
        {
            return timeOutResult.TimeoutExceeded;
        }
    }

    public class TimeOutExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
    }

    public class TimeOutGeneric
    {

        //NLOG: temporarily commented
        //private static Logger //_Logger = LogManager.GetCurrentClassLogger();

        #region Data

        private AutoResetEvent _AutoResetEvent = new AutoResetEvent(false);
        private delegate void RunMethodDelegate();

        public event EventHandler<TimeOutExceptionEventArgs> TimeOutException;
        public List<GraphDBException> GraphDBExceptions { get; set; }

        #endregion

        public TimeOutGeneric()
        {
            GraphDBExceptions = new List<GraphDBException>();
        }

        #region DoIt's

        /// <summary>
        /// Führt die Methode aus, die in einer festgesetzen Zeit erfolgen soll.
        /// </summary>
        /// <param name="runMethod">Methode zum ausführen</param>
        /// <param name="timeout">Zu erwartende Höchstzeit, bevor die Ausführung der Methode abgebrochen wird</param>
        /// <returns>True, wenn die Ausführung der Methode vor dem Timeout zu Ende gegangen ist. False wenn das Timeout überschritten wurde.</returns>
        public TimeOutResult<T> DoIt<T>(Delegate runMethod, TimeSpan timeout)
            where T : class
        {
            return this.DoIt<T>(runMethod, timeout, null);
        }

        /// <summary>
        /// Führt die Methode aus, die in einer festgesetzten Zeit erfolgen soll und übergibt die für sie bestimmte Parameter.
        /// </summary>
        /// <param name="runMethod">Methode zum ausführen</param>
        /// <param name="parameters">Parametertabelle</param>
        /// <param name="timeout">Zu erwartende Höchstzeit, bevor die Ausführung der Methode abgebrochen wird</param>
        /// <returns>True, wenn die Ausführung der Methode vor dem Timeout zu Ende gegangen ist. False wenn das Timeout überschritten wurde.</returns>
        public TimeOutResult<T> DoIt<T>(Delegate myDelegate, TimeSpan myTimeout, params object[] myParameters)
            where T : class
        {
            return this.DoItImp<T>(myDelegate, myTimeout, myParameters);
        }

        /// <summary>
        /// Führt die Methode mittels Delegate und übergebenen Parametern, die in der festgesetzen Zeit ausgeführt wurde.
        /// </summary>
        /// <param name="d">Auszuführendes Delegate</param>
        /// <param name="parameters">Zu übergebende Paramter für das Delegate</param>
        /// <param name="timeout">Zu erwartende Höchstzeit, bevor die Ausführung des Delegates abgebrochen wird</param>
        /// <returns>True, wenn die Ausführung des Delegates vor dem Timeout zu Ende gegangen ist. False wenn das Timeout überschritten wurde.</returns>
        private TimeOutResult<T> DoItImp<T>(Delegate myDelegate, TimeSpan myTimeout, params object[] myParameters)
            where T : class
        {
            var _Worker = new Worker(myDelegate, myParameters, this._AutoResetEvent);
            _Worker.TimeOutException += new EventHandler<TimeOutExceptionEventArgs>(_Worker_TimeOutException);
            _Worker.TimeOutException += TimeOutException;

            var _Thread = new Thread(new ThreadStart(_Worker.Run));

            _AutoResetEvent.Reset();
            _Thread.Start();

            if (_AutoResetEvent.WaitOne(myTimeout, false))
            {
                return new TimeOutResult<T>(_Worker.ReturnValue as T);
            }
            else
            {
                _Thread.Abort();
                return new TimeOutResult<T>(true);
            }
        }

        void _Worker_TimeOutException(object sender, TimeOutExceptionEventArgs e)
        {
            if (e.Exception is GraphDBException)
                GraphDBExceptions.Add(e.Exception as GraphDBException);
            else
            {
                if ((e.Exception.InnerException != null) && (e.Exception.InnerException is GraphDBException))
                {
                    GraphDBExceptions.Add(e.Exception.InnerException as GraphDBException);
                }
                else
                {
                    GraphDBExceptions.Add(new GraphDBException(new Errors.Error_UnknownDBError(e.Exception)));
                }
            }
        }

        #endregion

        #region Worker Klasse

        private class Worker
        {

            public event EventHandler<TimeOutExceptionEventArgs> TimeOutException;

            private AutoResetEvent evnt;
            private Delegate method;
            private object[] parameters;
            
            private Object _ReturnValue;
            public Object ReturnValue
            {
                get { return _ReturnValue; }
            }

            public Worker(Delegate method, object[] parameters, AutoResetEvent evnt)
            {
                this.method = method;
                this.parameters = parameters;
                this.evnt = evnt;
            }

            public void Run()
            {
                try
                {
                    _ReturnValue = this.method.DynamicInvoke(parameters);
                    evnt.Set();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("[DatabaseTest][TimeOut][Worker] Unhandled Exception! " + ex.Message + Environment.NewLine + ex.StackTrace);

                    //NLOG: temporarily commented
                    ////_Logger.ErrorException("Worker exception", ex);
                    if (ex.InnerException != null)
                    {
                        //NLOG: temporarily commented
                        ////_Logger.ErrorException("Worker inner exception", ex.InnerException);
                        System.Diagnostics.Debug.WriteLine("\r\nInnerException: " + ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace);

                        if (TimeOutException != null)
                            TimeOutException(this, new TimeOutExceptionEventArgs() { Exception = ex.InnerException });
                    }
                    else
                    {

                        if (TimeOutException != null)
                            TimeOutException(this, new TimeOutExceptionEventArgs() { Exception = ex });

                    }

                    this.evnt.Set();
                }
            }
        }

        #endregion

    }

}
