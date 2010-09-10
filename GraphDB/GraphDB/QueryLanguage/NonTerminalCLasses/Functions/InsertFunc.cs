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
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.Lib.ErrorHandling;
using sones.GraphDB.ObjectManagement;

using sones.GraphDB.TypeManagement;
using sones.GraphFS.Session;
using sones.GraphDB.QueryLanguage.Result;
using sones.Lib.Session;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Functions
{
    public class InsertFunc : ABaseFunction
    {
        public override string FunctionName
        {
            get { return "INSERT"; }
        }

        #region GetDescribeOutput()

        public override String GetDescribeOutput()
        {
            return "This function inserts one or more strings at the given position.";
        }

        #endregion

        public InsertFunc()
        {
            Parameters.Add(new ParameterValue("Position", new DBInt32()));
            Parameters.Add(new ParameterValue("StringPart", new DBString(), true));
        }

        public override bool ValidateWorkingBase(TypeAttribute workingBase, DBTypeManager typeManager)
        {
            if (workingBase != null)
            {
                if (workingBase.GetDBType(typeManager).IsUserDefined)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public override Exceptional<FuncParameter> ExecFunc(DBContext dbContext, params FuncParameter[] myParams)
        {
            var result = new Exceptional<FuncParameter>();

            if (!(CallingObject is ADBBaseObject))
            {
                return new Exceptional<FuncParameter>(new Errors.Error_InvalidFunctionParameter("CallingObject", "ADBBaseObject", CallingObject.GetType()));
            }

            var pos = (myParams[0].Value as DBInt32).GetValue();

            StringBuilder resString = new StringBuilder((CallingObject as ADBBaseObject).Value.ToString().Substring(0, pos));
            foreach (FuncParameter fp in myParams.Skip(1))
            {
                resString.Append((fp.Value as DBString).GetValue());
            }
            resString.Append((CallingObject as ADBBaseObject).Value.ToString().Substring(pos));

            result = new Exceptional<FuncParameter>(new FuncParameter(new DBString(resString.ToString())));

            return result;
        }
    }
}
