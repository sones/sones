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


#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Functions;
using sones.GraphDB.TypeManagement;
#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure.Helper
{
    /// <summary>
    /// this class generate an output for the describe function(s) commands
    /// </summary>
    public class DescrFuncOutput
    {

        #region constructors
        
        public DescrFuncOutput()
        { }

        #endregion

        #region Output

        /// <summary>
        /// generates an output for a function 
        /// </summary>
        /// <param name="myFunc">the function</param>
        /// <param name="myFuncName">function name</param>
        /// <param name="myTypeManager">type manager</param>
        /// <returns>a list of readouts which contains the information</returns>
        public IEnumerable<DBObjectReadout> GenerateOutput(ABaseFunction myFunc, string myFuncName, DBTypeManager myTypeManager)
        {

            var Func = new Dictionary<String, Object>();

            Func.Add("Name", myFuncName);
            Func.Add("Description", myFunc.GetDescribeOutput());

            #region Add function parameters

            var parameters = new Dictionary<String, Object>();
            foreach (var param in myFunc.GetParameters())
            {
                if (param.VariableNumOfParams)
                {
                    GraphDBType[] myArray = { myTypeManager.GetTypeByUUID(param.DBType.ID) };
                    parameters.Add(param.Name, myArray);
                }
                else
                {
                    parameters.Add(param.Name, myTypeManager.GetTypeByUUID(param.DBType.ID));
                }
            }
            Func.Add("Parameters", new Edge(new DBObjectReadout(parameters)));

            #endregion            

            return new List<DBObjectReadout>() { new DBObjectReadout(Func) };

        }

        #endregion

    }
}
