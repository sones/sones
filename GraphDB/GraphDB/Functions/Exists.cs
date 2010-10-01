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

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Functions
{

    /// <summary>
    /// Return true if an DBObject contains this attribute.
    /// </summary>
    public class Exists : ABaseFunction
    {

        #region constructors

        public Exists()
        { }

        #endregion

        #region ABaseFunction

        public override string FunctionName
        {
            get { return "EXISTS"; }
        }

        public override string GetDescribeOutput()
        {
            return "Return true if an DBObject contains this attribute.";
        }

        public override bool ValidateWorkingBase(IObject workingBase, DBTypeManager typeManager)
        {
            if (workingBase != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override Exceptional<FuncParameter> ExecFunc(DBContext dbContext, params FuncParameter[] myParams)
        {
            if (CallingObject != null)
                return new Exceptional<FuncParameter>(new FuncParameter(new DBBoolean(true)));
            else
                return new Exceptional<FuncParameter>(new FuncParameter(new DBBoolean(false)));
        }

        #endregion

    }

}
