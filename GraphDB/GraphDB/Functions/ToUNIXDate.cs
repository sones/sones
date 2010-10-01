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


#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.Errors;
using sones.Lib.DataStructures.Timestamp;

using System.Diagnostics;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.Functions
{

    /// <summary>
    /// Convert the datetime value to the unix datetime format.
    /// </summary>
    public class ToUNIXDate : ABaseFunction
    {

        #region constructors 

        public ToUNIXDate()
        { }

        #endregion

        #region ABaseFunction

        public override string FunctionName
        {
            get { return "ToUNIXDate"; }
        }

        public override string GetDescribeOutput()
        {
            return "Convert the datetime value to the unix datetime format.";
        }

        public override bool ValidateWorkingBase(IObject workingBase, DBTypeManager typeManager)
        {
            if (workingBase is DBUInt64 || workingBase is DBDateTime)
            {
                return true;
            }
            else if (workingBase is DBTypeAttribute)
            {
                if ((workingBase as DBTypeAttribute).GetValue().GetDBType(typeManager).UUID == DBUInt64.UUID
                 || (workingBase as DBTypeAttribute).GetValue().GetDBType(typeManager).UUID == DBDateTime.UUID)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override IObject GetReturnType(IObject myWorkingBase, DBTypeManager myTypeManager)
        {
            return new DBInt64();
        }

        public override Exceptional<FuncParameter> ExecFunc(DBContext dbContext, params FuncParameter[] myParams)
        {
            Exceptional<FuncParameter> result = new Exceptional<FuncParameter>();

            if (CallingObject is DBUInt64)
            {
                try
                {
                    var dtValue = new DateTime(System.Convert.ToInt64(((DBUInt64)CallingObject).Value));
                    result.Value = new FuncParameter(new DBInt64(dtValue.ToUnixTimeStamp()));
                }
                catch (Exception e)
                {
                    return new Exceptional<FuncParameter>(new Error_UnknownDBError(e.Message));
                }

                return result;
            }
            else if (CallingObject is DBDateTime)
            {
                result.Value = new FuncParameter(new DBInt64(((DateTime)((DBDateTime)CallingObject).Value).ToUnixTimeStamp()));
            }
            else
            {
                Debug.Assert(false); // this should never happen due to the ValidateWorkingBase method
                return new Exceptional<FuncParameter>(new Error_FunctionParameterTypeMismatch(typeof(DBDateTime), CallingObject.GetType()));
            }

            return result;
        }

        #endregion
    }
}
