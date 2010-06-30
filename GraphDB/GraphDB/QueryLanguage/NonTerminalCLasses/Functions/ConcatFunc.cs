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
    public class ConcatFunc : ABaseFunction
    {
        public override string FunctionName
        {
            get { return "CONCAT"; }
        }

        #region GetDescribeOutput()

        public override String GetDescribeOutput()
        {
            return "This will concatenate some strings. This function can be used as type independent to concatenate string values or as type dependent to concatenate an attribute output with other strings.";
        }
        
        #endregion

        public ConcatFunc()
        {
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
                return true;
            }
        }

        public override Exceptional<FuncParameter> ExecFunc(DBContext dbContext, params FuncParameter[] myParams)
        {
            var result = new Exceptional<FuncParameter>();

            StringBuilder resString = new StringBuilder();

            if (CallingObject != null)
            {
                if (CallingObject is DBString)
                    resString.Append((CallingObject as DBString).GetValue());
            }

            foreach (FuncParameter fp in myParams)
            {
                resString.Append(fp.Value);
            }


            result = new Exceptional<FuncParameter>(new FuncParameter(new DBString(resString.ToString())));

            return result;
        }
    }
}
