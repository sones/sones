﻿using System;
using System.Collections.Generic;
using sones.GraphDB;
using sones.GraphQL.GQL.ErrorHandling;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.ErrorHandling;
using sones.Library.VersionedPluginManager;
using sones.Plugins.SonesGQL.Functions;

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

        public override QueryResult GetResult(
                                                GQLPluginManager myPluginManager, 
                                                IGraphDB myGraphDB, 
                                                SecurityToken mySecurityToken, 
                                                TransactionToken myTransactionToken)
        {
            var resultingVertices = new List<IVertexView>();
            ASonesException error = null;

            if (!String.IsNullOrEmpty(_FuncName))
            {

                #region Specific function

                //aggregate is user defined
                if (_FuncName.Contains("."))
                {
                    try
                    {
                        var aggregate = myPluginManager.GetAndInitializePlugin<IGQLFunction>(_FuncName);

                        if (aggregate != null)
                        {
                            resultingVertices = new List<IVertexView>() { GenerateOutput(aggregate, _FuncName) };
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
                }
                //maybe user forgot prefix "sones."
                else
                {
                    try
                    {
                        var aggregate = myPluginManager.GetAndInitializePlugin<IGQLFunction>("SONES." + _FuncName);

                        if (aggregate != null)
                        {
                            resultingVertices = new List<IVertexView>() { GenerateOutput(aggregate, "SONES." + _FuncName) };
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
                        var func = myPluginManager.GetAndInitializePlugin<IGQLFunction>(funcName);

                        if (func != null)
                        {
                            resultingVertices.Add(GenerateOutput(func, funcName));
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

            if(error != null)
                return new QueryResult("", "GQL", 0L, ResultType.Failed, resultingVertices, error);
            else
                return new QueryResult("", "GQL", 0L, ResultType.Successful, resultingVertices);
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
            var temp = new Dictionary<String, object>();
            var temp2 = new Dictionary<String, object>();
            var edges = new Dictionary<String, IEdgeView>();

            _Function.Add("Function", myFunc.FunctionName);
            _Function.Add("Type", myFuncName);

            int count = 1;
            foreach (var parameter in ((IPluginable)myFunc).SetableParameters)
            {
                temp.Add("Parameter " + count.ToString() + " Key: ", parameter.Key);

                count++;
            }

            count = 1;
            foreach (var parameter in myFunc.GetParameters())
            {
                temp2.Add("Parameter " + count.ToString() + " Name: ", parameter.Name);

                count++;
            }

            edges.Add("SetableParameters", new EdgeView(temp, null));
            edges.Add("Parameters", new EdgeView(temp2, null));

            return new VertexView(_Function, edges);
        }

        #endregion
    }
}