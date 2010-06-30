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
using sones.GraphFS.Transactions;

namespace sones.GraphDB.Errors
{
    public class Error_StatementExpectsWriteTransaction : GraphDBTransactionError
    {
        public String Statement { get; private set; }
        public IsolationLevel IsolationLevel { get; private set; }

        public Error_StatementExpectsWriteTransaction(String statement, IsolationLevel isolationLevel)
        {
            Statement = statement;
            IsolationLevel = isolationLevel;
        }

        public override string ToString()
        {
            return String.Format("The statement \"{0}\" expects an write transaction. Current transaction isolation level is \"{1}\"", Statement, IsolationLevel);
        }
    }
}
