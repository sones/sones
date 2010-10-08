using System;
using System.Collections.Generic;
using System.Text;

namespace sones.StorageEngines.Caches
{

    public class QueueManagerException: ApplicationException
    {
        public QueueManagerException(string message) : base(message) 
		{
			// do nothing extra
		}
    }


    public class QueueEntry_NoPositionsForWrittingGiven : ApplicationException
    {
        public QueueEntry_NoPositionsForWrittingGiven(string message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class QueueEntryObjectStreamIsSmallerThanTheDataLengthException : ApplicationException
    {
        public QueueEntryObjectStreamIsSmallerThanTheDataLengthException(string message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class QueueEntryTooLessAllocatedSpaceException : ApplicationException
    {
        public QueueEntryTooLessAllocatedSpaceException(string message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class QueueEntry_SerializedINodeIsTooLargeException : ApplicationException
    {
        public QueueEntry_SerializedINodeIsTooLargeException(string message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class QueueManagerCouldNotWriteQueueException : ApplicationException
    {
        public QueueManagerCouldNotWriteQueueException(string message)
            : base(message)
        {
            // do nothing extra
        }
    }



    public class QueueEntryObjectStreamNotFoundException : ApplicationException
    {
        public QueueEntryObjectStreamNotFoundException(string message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class QueueEntryObjectEditionNotFoundException : ApplicationException
    {
        public QueueEntryObjectEditionNotFoundException(string message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class QueueEntryObjectRevisionNotFoundException : ApplicationException
    {
        public QueueEntryObjectRevisionNotFoundException(string message)
            : base(message)
        {
            // do nothing extra
        }
    }


    public class QueueEntryNoObjectStreamFoundException : ApplicationException
    {
        public QueueEntryNoObjectStreamFoundException(string message)
            : base(message)
        {
            // do nothing extra
        }
    }



}
