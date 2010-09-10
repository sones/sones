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

/* SpecialTimer
 * (c) Achim Friedland, 2010
 * 
 * Lead programmer:
 *      Achim Friedland
 *  
 * */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

#endregion

namespace sones.Lib.Multithreading
{

    /// <summary>
    /// The SpecialTimer will invoke the given method regularly or
    /// after a given timeout value.
    /// </summary>

    public class SpecialTimer : IDisposable
    {

        #region Data

        private EventWaitHandle _EventWaitHandle = new AutoResetEvent(false);
        private Delegate        _Delegate;
        private Thread          _TimeoutThread;
        private Timer           _RegularTimer;
        private volatile Boolean _Sliding = false;
        private volatile Boolean _End = false;
        private object _Lock = new object();
        private object _Wait = new object();

        #endregion


        #region TimeoutAt

        private Int64 _TimeoutAt;

        public Int64 TimeoutAt
        {
            get
            {
                return _TimeoutAt;
            }
        }

        #endregion

        #region TimeoutInvokation

        private TimeSpan _TimeoutInvokation;

        public TimeSpan TimeoutInvokation
        {
            get
            {
                return _TimeoutInvokation;
            }
            set
            {
                _TimeoutInvokation = value;
            }
        }

        #endregion

        #region RegularInvokation

        private TimeSpan _RegularInvokation;

        public TimeSpan RegularInvokation
        {
            get
            {
                return _RegularInvokation;
            }
            set
            {
                _RegularInvokation = value;
            }
        }

        #endregion

        #region Name

        public String Name
        {
            get
            {
                return _TimeoutThread.Name;
            }
            set
            {
                _TimeoutThread.Name = value;
            }
        }

        #endregion

        #region Priority

        public ThreadPriority Priority
        {
            get
            {
                return _TimeoutThread.Priority;
            }
            set
            {
                _TimeoutThread.Priority = value;
            }
        }

        #endregion


        #region SpecialTimer(myDelegate, myTimeoutInvokation, myRegularInvokation)

        public SpecialTimer(Delegate myDelegate, TimeSpan myTimeoutInvokation, TimeSpan myRegularInvokation)
        {

            _TimeoutInvokation  = myTimeoutInvokation;
            _RegularInvokation  = myRegularInvokation;
            _Delegate           = myDelegate;
            _TimeoutAt          = 0;

            _TimeoutThread      = new Thread(SlidingTimeoutThread);
            _TimeoutThread.Start();

            _RegularTimer       = new Timer(new TimerCallback(InvokeMethod), "ByRegularTimer!", _RegularInvokation, _RegularInvokation);

        }

        #endregion

        #region ScheduleTimeout()

        public void ScheduleTimeout()
        {
            //lock (_Lock)
            //{

                _TimeoutAt = DateTime.Now.Ticks;
                _TimeoutAt += _TimeoutInvokation.Ticks;

                _Sliding = true;
                //Console.WriteLine("Timeout scheduled at: " + _TimeoutAt.ToString());
                _EventWaitHandle.Set();

            //}
        }

        #endregion


        #region Dispose()

        public void Dispose()
        {

            //lock (_Lock)
            //{
                _End = true;            // Signal the thread to end!
                _EventWaitHandle.Set();
            //}

            _TimeoutThread.Join();             // Wait for the thread to finish!
            _RegularTimer.Dispose();
            _EventWaitHandle.Close();   // Release OS resources!

        }

        #endregion

        #region InvokeMethod(myState)

        private void InvokeMethod(Object myState)
        {
            lock (_Lock)
            {
                _Delegate.DynamicInvoke(myState);
            }
        }

        #endregion


        #region SlidingTimeoutThread()

        public void SlidingTimeoutThread()
        {

            try
            {

                while (true)
                {

                    lock(_Lock)
                    {

                        if (_Sliding)
                        {

                            if (_TimeoutAt < DateTime.Now.Ticks)
                            {
                                InvokeMethod("SlidingTimeout!");
                                _Sliding = false;
                            }

                            else
                                Thread.Sleep(1);

                        }

                        if (_End)
                            return;

                    }

                    if (!_Sliding)
                        _EventWaitHandle.WaitOne();


                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        #endregion


    }
}
