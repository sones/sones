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
