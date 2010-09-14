using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace sones.GraphDB.Query.Helpers
{

    public class TimeOut
    {

        private AutoResetEvent _AutoResetEvent = new AutoResetEvent(false);
        //NLOG: temporarily commented
        //private static Logger //_Logger = LogManager.GetCurrentClassLogger();

        public delegate void RunMethodDelegate();

        /// <summary>
        /// Führt die Methode aus, die in einer festgesetzen Zeit erfolgen soll.
        /// </summary>
        /// <param name="runMethod">Methode zum ausführen</param>
        /// <param name="timeout">Zu erwartende Höchstzeit, bevor die Ausführung der Methode abgebrochen wird</param>
        /// <returns>True, wenn die Ausführung der Methode vor dem Timeout zu Ende gegangen ist. False wenn das Timeout überschritten wurde.</returns>
        public bool DoIt(Delegate runMethod, TimeSpan timeout)
        {
            return this.DoIt(runMethod, timeout, null);
        }

        /// <summary>
        /// Führt die Methode aus, die in einer festgesetzten Zeit erfolgen soll und übergibt die für sie bestimmte Parameter.
        /// </summary>
        /// <param name="runMethod">Methode zum ausführen</param>
        /// <param name="parameters">Parametertabelle</param>
        /// <param name="timeout">Zu erwartende Höchstzeit, bevor die Ausführung der Methode abgebrochen wird</param>
        /// <returns>True, wenn die Ausführung der Methode vor dem Timeout zu Ende gegangen ist. False wenn das Timeout überschritten wurde.</returns>
        public bool DoIt(Delegate myDelegate, TimeSpan myTimeout, params object[] myParameters)
        {
            return this.DoItImp(myDelegate, myTimeout, myParameters);
        }

        /// <summary>
        /// Führt die Methode mittels Delegate und übergebenen Parametern, die in der festgesetzen Zeit ausgeführt wurde.
        /// </summary>
        /// <param name="d">Auszuführendes Delegate</param>
        /// <param name="parameters">Zu übergebende Paramter für das Delegate</param>
        /// <param name="timeout">Zu erwartende Höchstzeit, bevor die Ausführung des Delegates abgebrochen wird</param>
        /// <returns>True, wenn die Ausführung des Delegates vor dem Timeout zu Ende gegangen ist. False wenn das Timeout überschritten wurde.</returns>
        private bool DoItImp(Delegate myDelegate, TimeSpan myTimeout, params object[] myParameters)
        {
            var _Worker = new Worker(myDelegate, myParameters, this._AutoResetEvent);
            var _Thread = new Thread(new ThreadStart(_Worker.Run));

            _AutoResetEvent.Reset();
            _Thread.Start();

            if (_AutoResetEvent.WaitOne(myTimeout, false))
            {
                return true;
            }
            else
            {
                _Thread.Abort();
                return false;
            }
        }


        #region Worker Klasse

        private class Worker
        {

            private AutoResetEvent evnt;
            public Delegate method;
            public object[] parameters;

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
                    this.method.DynamicInvoke(parameters);
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
                    }

                    this.evnt.Set();
                }
            }
        }

        #endregion

    }

}
