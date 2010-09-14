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
