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

/*
 * DescribeFuncDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.GraphDB.Functions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Result;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDB.Managers.Structures.Describe
{
    public class DescribeFuncDefinition : ADescribeDefinition
    {

        #region Data

        private String _FuncName;

        #endregion

        #region Ctor

        public DescribeFuncDefinition(string myFuncName = null)
        {
            _FuncName = myFuncName;
        }

        #endregion

        #region ADescribeDefinition

        public override Exceptional<IEnumerable<Vertex>> GetResult(DBContext myDBContext)
        {
            if (!String.IsNullOrEmpty(_FuncName))
            {

                #region Specific func

                var func = myDBContext.DBPluginManager.GetFunction(_FuncName);
                if (func != null)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new List<Vertex>(){(GenerateOutput(func, _FuncName, myDBContext.DBTypeManager))});
                }
                else
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_EdgeTypeDoesNotExist(_FuncName));
                }

                #endregion

            }
            else
            {

                #region All edge

                var resultingReadouts = new List<Vertex>();

                foreach (var func in myDBContext.DBPluginManager.GetAllFunctions())
                {
                    resultingReadouts.Add(GenerateOutput(func.Value, func.Key, myDBContext.DBTypeManager));
                }


                return new Exceptional<IEnumerable<Vertex>>(resultingReadouts);
                #endregion

            }
        }

        #endregion

        #region Output

        /// <summary>
        /// generates an output for a function 
        /// </summary>
        /// <param name="myFunc">the function</param>
        /// <param name="myFuncName">function name</param>
        /// <param name="myTypeManager">type manager</param>
        /// <returns>a list of readouts which contains the information</returns>
        private Vertex GenerateOutput(ABaseFunction myFunc, String myFuncName, DBTypeManager myTypeManager)
        {

            var Func = new Dictionary<String, Object>();

            Func.Add("Name",        myFuncName);
            Func.Add("Description", myFunc.GetDescribeOutput());

            #region Add function parameters

            var parameters = new Dictionary<String, Object>();
            foreach (var param in myFunc.GetParameters())
            {
                if (param.VariableNumOfParams)
                {
                    GraphDBType[] myArray = { myTypeManager.GetTypeByName(param.DBType.ObjectName) };
                    parameters.Add(param.Name, myArray);
                }
                else
                {
                    parameters.Add(param.Name, myTypeManager.GetTypeByName(param.DBType.ObjectName));
                }
            }

            Func.Add("Parameters", new Edge(null, new Vertex(parameters)));

            #endregion

            return new Vertex(Func);

        }

        #endregion

    }
}
