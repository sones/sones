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
using sones.Lib.ErrorHandling;

namespace sones.GraphDB.Errors
{
    public class Error_CouldNotGetIndexReference : GraphDBIndexError
    {
        public IEnumerable<IError>  Errors { get; private set; }
        public String               IndexName { get; private set; }
        public String               IndexEdition { get; private set; }
        public int                  ShardID { get; private set; }

        public Error_CouldNotGetIndexReference(IEnumerable<IError> myErrors, String myIndexName, String myIndexEdition, int shardID = -1)
        {
            Errors          = myErrors;
            IndexName       = myIndexName;
            IndexEdition    = myIndexEdition;
            ShardID         = shardID;
        }

        public override string ToString()
        {
            return String.Format("The index reference of {0} with edition {1} could not be loaded!", IndexName, IndexEdition);
        }
    }
}
