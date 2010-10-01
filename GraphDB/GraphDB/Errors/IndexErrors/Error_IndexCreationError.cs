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
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_IndexCreationError : GraphDBIndexError
    {

        public String IndexName { get; private set; }
        public String IndexEdition { get; private set; }
        public String Info { get; private set; }
        public Exception Exception { get; private set; }
        public GraphDBType Type { get; private set; }

        public Error_IndexCreationError(String myIndexName, String myIndexEdition, String myInfo)
        {
            IndexName = myIndexName;
            IndexEdition = myIndexEdition;
            Info = myInfo;
            Type = null;
            Exception = null;
        }

        public Error_IndexCreationError(GraphDBType myType, Exception myException)
        {
            Type = myType;
            Exception = myException;
        }

        public override string ToString()
        {
            if (Type != null)
            {
                if (Exception != null && Exception.Message != null)
                {
                    return String.Format("Could not create index on type \"{0}\". Description:" + Environment.NewLine + "{1}", Type.Name, Exception.Message);
                }
                else
                {
                    return String.Format("Could not create index on type \"{0}\".", Type.Name);
                }
            }
            else
            {
                return String.Format("Could not create index \"{0}\" (Edition: \"{1}\"). Error:" + Environment.NewLine + "{2}", IndexName, IndexEdition, Info);
            }
        }
    }
}
