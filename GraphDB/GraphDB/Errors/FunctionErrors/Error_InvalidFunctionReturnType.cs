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
using sones.Lib;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidFunctionReturnType : GraphDBFunctionError
    {

        public String FunctionName { get; private set; }
        public Type TypeOfFunctionReturn { get; private set; }
        public Type[] ValidTypes { get; private set; }

        public Error_InvalidFunctionReturnType(String myFunctionName, Type myTypeOfFunctionReturn, params Type[] myValidTypes)
        {
            FunctionName = myFunctionName;
            TypeOfFunctionReturn = myTypeOfFunctionReturn;
            ValidTypes = myValidTypes;
        }

        public override string ToString()
        {
            if (ValidTypes.IsNullOrEmpty())
            {
                return String.Format("The return type [{0}] of function [{1}] is not valid.", TypeOfFunctionReturn, FunctionName);
            }
            else
            {
                return String.Format("The return type [{0}] of function [{1}] is not valid. Please choose one of: {2}", TypeOfFunctionReturn, FunctionName, ValidTypes.ToAggregatedString(t => t.Name));
            }
        }
    }
}
