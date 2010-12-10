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
    /// <summary>
    /// Describes a function
    /// </summary>
    public class DescribeFuncDefinition : ADescribeDefinition
    {

        #region Data

        /// <summary>
        /// The function name
        /// </summary>
        private String _FuncName;

        #endregion

        #region Ctor

        public DescribeFuncDefinition(string myFuncName = null)
        {
            _FuncName = myFuncName;
        }

        #endregion

        #region ADescribeDefinition

        /// <summary>
        /// <seealso cref=" ADescribeDefinition"/>
        /// </summary>
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
