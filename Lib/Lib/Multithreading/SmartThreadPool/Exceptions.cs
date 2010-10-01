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

using System;
#if !(WindowsCE)
using System.Runtime.Serialization;
#endif

namespace sones.Lib.Threading
{
    #region Exceptions

    /// <summary>
    /// Represents an exception in case IWorkItemResult.GetResult has been canceled
    /// </summary>
    public sealed partial class WorkItemCancelException : ApplicationException
    {
        public WorkItemCancelException()
        {
        }

        public WorkItemCancelException(String message)
            : base(message)
        {
        }

        public WorkItemCancelException(String message, Exception e)
            : base(message, e)
        {
        }
    }

    /// <summary>
    /// Represents an exception in case IWorkItemResult.GetResult has been timed out
    /// </summary>
    public sealed partial class WorkItemTimeoutException : ApplicationException
    {
        public WorkItemTimeoutException()
        {
        }

        public WorkItemTimeoutException(String message)
            : base(message)
        {
        }

        public WorkItemTimeoutException(String message, Exception e)
            : base(message, e)
        {
        }
    }

    /// <summary>
    /// Represents an exception in case IWorkItemResult.GetResult has been timed out
    /// </summary>
    public sealed partial class WorkItemResultException : ApplicationException
    {
        public WorkItemResultException()
        {
        }

        public WorkItemResultException(String message)
            : base(message)
        {
        }

        public WorkItemResultException(String message, Exception e)
            : base(message, e)
        {
        }
    }


#if !(WindowsCE)
    /// <summary>
    /// Represents an exception in case IWorkItemResult.GetResult has been canceled
    /// </summary>
    
    public sealed partial class WorkItemCancelException
    {
        public WorkItemCancelException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }

    /// <summary>
    /// Represents an exception in case IWorkItemResult.GetResult has been timed out
    /// </summary>
    
    public sealed partial class WorkItemTimeoutException
    {
        public WorkItemTimeoutException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }

    /// <summary>
    /// Represents an exception in case IWorkItemResult.GetResult has been timed out
    /// </summary>
    
    public sealed partial class WorkItemResultException
    {
        public WorkItemResultException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }

#endif

    #endregion
}
