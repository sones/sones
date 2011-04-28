using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.Result;
using sones.Plugins.SonesGQL.Functions;
using sones.GraphQL.GQL.ErrorHandling;
using sones.Library.ErrorHandling;
using System.Diagnostics;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    public sealed class DescribeFuncDefinition : ADescribeDefinition
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

        public override QueryResult GetResult(ParsingContext myContext, 
                                                GQLPluginManager myPluginManager, 
                                                IGraphDB myGraphDB, 
                                                SecurityToken mySecurityToken, 
                                                TransactionToken myTransactionToken)
        {
            var sw = new Stopwatch();

            sw.Reset();
            sw.Start();

            var resultingVertices = new List<IVertexView>();
            ASonesException error = null;

            if (!String.IsNullOrEmpty(_FuncName))
            {

                #region Specific aggregate

                try
                {
                    var func = myPluginManager.GetAndInitializePlugin<IGQLFunction>(_FuncName);

                    if (func != null)
                    {
                        resultingVertices = new List<IVertexView>() { GenerateOutput(func, _FuncName) };
                    }
                    else
                    {
                        error = new AggregateOrFunctionDoesNotExistException(typeof(IGQLFunction), _FuncName, "");
                    }
                }
                catch (ASonesException e)
                {
                    error = new AggregateOrFunctionDoesNotExistException(typeof(IGQLFunction), _FuncName, "", e);
                }

                #endregion

            }

            else
            {

                #region All aggregates

                myPluginManager.GetPluginsForType<IGQLFunction>();
                foreach (var funcName in myPluginManager.GetPluginsForType<IGQLFunction>())
                {
                    try
                    {
                        var aggregate = myPluginManager.GetAndInitializePlugin<IGQLFunction>(funcName);

                        if (aggregate != null)
                        {
                            resultingVertices.Add(GenerateOutput(aggregate, funcName));
                        }
                        else
                        {
                            error = new AggregateOrFunctionDoesNotExistException(typeof(IGQLFunction), funcName, "");
                        }
                    }
                    catch (ASonesException e)
                    {
                        error = new AggregateOrFunctionDoesNotExistException(typeof(IGQLFunction), funcName, "", e);
                    }
                }

                #endregion

            }

            sw.Stop();

            return new QueryResult("", "GQL", (ulong)sw.ElapsedMilliseconds, ResultType.Successful, resultingVertices, error);
        }

        #region Output

        /// <summary>
        /// generates an output for a function 
        /// </summary>
        /// <param name="myFunc">the function</param>
        /// <param name="myFuncName">function name</param>
        /// <param name="myTypeManager">type manager</param>
        /// <returns>a list of readouts which contains the information</returns>
        private IVertexView GenerateOutput(IGQLFunction myFunc, String myFuncName)
        {
            var _Function = new Dictionary<String, Object>();

            _Function.Add("Aggregate", myFunc.PluginName);
            _Function.Add("Type", myFuncName);

            foreach (var parameter in myFunc.SetableParameters)
            {
                _Function.Add("SetableParameter Key ", parameter.Key);
                _Function.Add("SetableParameter Value ", parameter.Value);
            }

            foreach (var parameter in myFunc.GetParameters())
            {
                _Function.Add("Parameter Name", parameter.Name);
                _Function.Add("Parameter Value ", parameter.Value);
            }

            return new VertexView(_Function, new Dictionary<String, IEdgeView>());

        }

        #endregion
    }
}
