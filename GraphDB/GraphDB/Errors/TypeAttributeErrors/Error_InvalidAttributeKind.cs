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
    public class Error_InvalidAttributeKind : GraphDBAttributeError
    {
        public KindsOfType[] ExpectedKindsOfType { get; private set; }
        public KindsOfType CurrentKindsOfType { get; private set; }

        public Error_InvalidAttributeKind()
        {
            ExpectedKindsOfType = new KindsOfType[0];
        }

        public Error_InvalidAttributeKind(KindsOfType myCurrentKindsOfType, params KindsOfType[] myExpectedKindsOfType)
            : this()
        {
            ExpectedKindsOfType = myExpectedKindsOfType;
            CurrentKindsOfType = myCurrentKindsOfType;
        }

        public override string ToString()
        {
            return String.Format("The given kind \"{0}\" does not match the expected \"{0}\"", CurrentKindsOfType,
                ExpectedKindsOfType.Aggregate<KindsOfType, StringBuilder>(new StringBuilder(), (result, elem) => { result.AppendFormat("{0},", elem); return result; }));
        }
    }
}
