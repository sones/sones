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


/* <id Name="sones GraphDB – ToUpperFunc" />
 * <copyright file="ToUpperFunc.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This function represents the well known ToUpper function.<summary>
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
    public class ToUpperFunc : ABaseFunction
    {
        public override string FunctionName
        {
            get { return "TOUPPER"; }
        }

        #region GetDescribeOutput()

        public override String GetDescribeOutput()
        {
            return "Returns a copy of this attribute value converted to uppercase.";
        }

        #endregion


        public ToUpperFunc()
        {
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

            throw new NotImplementedException();
        }

        public override Exceptional<FuncParameter> ExecFunc(DBContext dbContext, params FuncParameter[] myParams)
        {
            if (CallingObject is ADBBaseObject)
                return new Exceptional<FuncParameter>(new FuncParameter(new DBString((CallingObject as ADBBaseObject).Value.ToString().ToUpper())));
            else
                return new Exceptional<FuncParameter>(new Errors.Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }
    }
}
