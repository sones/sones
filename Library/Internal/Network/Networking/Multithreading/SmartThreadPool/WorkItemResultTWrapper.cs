/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Threading;

namespace sones.Library.Threading
{
    #region WorkItemResultTWrapper class

    internal class WorkItemResultTWrapper<TResult> : IWorkItemResult<TResult>, IInternalWaitableResult
    {
        private readonly IWorkItemResult _workItemResult;

        public WorkItemResultTWrapper(IWorkItemResult workItemResult)
        {
            _workItemResult = workItemResult;
        }

        #region IWorkItemResult<TResult> Members

        public TResult GetResult()
        {
            return (TResult)_workItemResult.GetResult();
        }

        public TResult GetResult(int millisecondsTimeout, bool exitContext)
        {
            return (TResult)_workItemResult.GetResult(millisecondsTimeout, exitContext);
        }

        public TResult GetResult(TimeSpan timeout, bool exitContext)
        {
            return (TResult)_workItemResult.GetResult(timeout, exitContext);
        }

        public TResult GetResult(int millisecondsTimeout, bool exitContext, WaitHandle cancelWaitHandle)
        {
            return (TResult)_workItemResult.GetResult(millisecondsTimeout, exitContext, cancelWaitHandle);
        }

        public TResult GetResult(TimeSpan timeout, bool exitContext, WaitHandle cancelWaitHandle)
        {
            return (TResult)_workItemResult.GetResult(timeout, exitContext, cancelWaitHandle);
        }

        public TResult GetResult(out Exception e)
        {
            return (TResult)_workItemResult.GetResult(out e);
        }

        public TResult GetResult(int millisecondsTimeout, bool exitContext, out Exception e)
        {
            return (TResult)_workItemResult.GetResult(millisecondsTimeout, exitContext, out e);
        }

        public TResult GetResult(TimeSpan timeout, bool exitContext, out Exception e)
        {
            return (TResult)_workItemResult.GetResult(timeout, exitContext, out e);
        }

        public TResult GetResult(int millisecondsTimeout, bool exitContext, WaitHandle cancelWaitHandle, out Exception e)
        {
            return (TResult)_workItemResult.GetResult(millisecondsTimeout, exitContext, cancelWaitHandle, out e);
        }

        public TResult GetResult(TimeSpan timeout, bool exitContext, WaitHandle cancelWaitHandle, out Exception e)
        {
            return (TResult)_workItemResult.GetResult(timeout, exitContext, cancelWaitHandle, out e);
        }

        public bool IsCompleted
        {
            get { return _workItemResult.IsCompleted; }
        }

        public bool IsCanceled
        {
            get { return _workItemResult.IsCanceled; }
        }

        public object State
        {
            get { return _workItemResult.State; }
        }

        public bool Cancel()
        {
            return _workItemResult.Cancel();
        }

        public bool Cancel(bool abortExecution)
        {
            return _workItemResult.Cancel(abortExecution);
        }

        public WorkItemPriority WorkItemPriority
        {
            get { return _workItemResult.WorkItemPriority; }
        }

        public TResult Result
        {
            get { return (TResult)_workItemResult.Result; }
        }

        public object Exception
        {
            get { return (TResult)_workItemResult.Exception; }
        }

        #region IInternalWorkItemResult Members

        public IWorkItemResult GetWorkItemResult()
        {
            return _workItemResult.GetWorkItemResult();
        }

        public IWorkItemResult<TRes> GetWorkItemResultT<TRes>()
        {
            return (IWorkItemResult<TRes>)this;
        }

        #endregion

        #endregion
    }

    #endregion

}
