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
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using System.Diagnostics;

namespace sones.GraphDB.Errors
{
    public class Error_NotImplemented : GraphDBError
    {
        
        public Exception    Exception   { get; private set; }

        public Error_NotImplemented(StackTrace myStackTrace)
        {
            StackTrace  = myStackTrace;
        }

        public Error_NotImplemented(StackTrace myStackTrace, Exception myException)
        {
            StackTrace  = myStackTrace;
            Exception   = myException;
        }

        public Error_NotImplemented(StackTrace myStackTrace, String myMessage)
        {
            StackTrace  = myStackTrace;
            Message     = myMessage;
        }

        public override String ToString()
        {
            if (Exception != null)
            {
                return String.Format("{0}" + Environment.NewLine + "Stacktrace" + Environment.NewLine + "{1}" + Environment.NewLine + Environment.NewLine + Message, Exception, StackTrace);
            }
            else
            {
                if (Message != null)
                {
                    return String.Format("{0}\nStacktrace:\n{1}", Message, StackTrace);
                }
                else
                {
                    return String.Format("Stacktrace:\n{0}", StackTrace);

                }
            }
        }


    }

}
