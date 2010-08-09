/*
 * DescribeFuncDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.GraphDB.Functions;
using sones.GraphDB.TypeManagement;

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

        public override Exceptional<List<SelectionResultSet>> GetResult(DBContext myDBContext)
        {

            var result = new List<SelectionResultSet>();

            if (!String.IsNullOrEmpty(_FuncName))
            {

                #region Specific func

                var func = myDBContext.DBPluginManager.GetFunction(_FuncName);
                if (func != null)
                {
                    result.Add(new SelectionResultSet(GenerateOutput(func, _FuncName, myDBContext.DBTypeManager)));
                }
                else
                {
                    return new Exceptional<List<SelectionResultSet>>(new Error_EdgeTypeDoesNotExist(_FuncName));
                }

                #endregion

            }
            else
            {

                #region All edge

                foreach (var func in myDBContext.DBPluginManager.GetAllFunctions())
                {
                    result.Add(new SelectionResultSet(GenerateOutput(func.Value, func.Key, myDBContext.DBTypeManager)));
                }

                #endregion

            }

            return new Exceptional<List<SelectionResultSet>>(result);

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
        private IEnumerable<DBObjectReadout> GenerateOutput(ABaseFunction myFunc, string myFuncName, DBTypeManager myTypeManager)
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
            Func.Add("Parameters", new Edge(new DBObjectReadout(parameters), ""));

            #endregion

            return new List<DBObjectReadout>() { new DBObjectReadout(Func) };

        }

        #endregion

    }
}
