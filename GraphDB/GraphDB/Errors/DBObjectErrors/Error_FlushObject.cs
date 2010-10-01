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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphFS.DataStructures;

namespace sones.GraphDB.Errors
{
    public class Error_FlushObject : GraphDBObjectError
    {
        public ObjectLocation ObjectName { get; private set; }
        public Exception Exception { get; private set; }

        public Error_FlushObject(ObjectLocation myObjectName, Exception myException)
        {
            ObjectName = myObjectName;
            Exception = myException;
        }

        public override string ToString()
        {
            if (Exception != null)
                return String.Format("The object \"{0}\" could not be flushed!{1}Message:{2}{3}StackTrace:{4}", ObjectName, "\n\n", Exception.Message, "\n\n", Exception.StackTrace);
            else
                return String.Format("The object \"{0}\" could not be flushed!");
        }
    }
}
