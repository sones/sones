using System;
using System.Collections.Generic;
using sones.GraphDB;
using sones.GraphQL.GQL.ErrorHandling;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.ErrorHandling;
using sones.Library.VersionedPluginManager;
using sones.Plugins.SonesGQL.Aggregates;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    /// <summary>
    /// Describes aggregates
    /// </summary>
    public sealed class DescribeAggregateDefinition : ADescribeDefinition
    {
        #region Data

        /// <summary>
        /// The aggregate name
        /// </summary>
        private String _AggregateName;

        #endregion

        #region Ctor

        public DescribeAggregateDefinition(String myAggregateName = null)
        {
            _AggregateName = myAggregateName;
        }

        #endregion

        /// <summary>
        /// <seealso cref=" ADescribeDefinition"/>
        /// </summary>
        public override QueryResult GetResult(
                                                GQLPluginManager myPluginManager,
                                                IGraphDB myGraphDB,
                                                SecurityToken mySecurityToken,
                                                TransactionToken myTransactionToken)
        {
            var resultingVertices = new List<IVertexView>();
            ASonesException error = null;

            //aggregate name is not empty
            if (!String.IsNullOrEmpty(_AggregateName))
            {

                #region Specific aggregate

                //aggregate is user defined
                if (_AggregateName.Contains("."))
                {
                    try
                    {
                        //get plugin
                        var aggregate = myPluginManager.GetAndInitializePlugin<IGQLAggregate>(_AggregateName);

                        if (aggregate != null)
                        {
                            resultingVertices = new List<IVertexView>() { GenerateOutput(aggregate, _AggregateName) };
                        }
                        else
                        {
                            error = new AggregateOrFunctionDoesNotExistException(typeof(IGQLAggregate), _AggregateName, "");
                        }
                    }
                    catch (ASonesException e)
                    {
                        error = new AggregateOrFunctionDoesNotExistException(typeof(IGQLAggregate), _AggregateName, "", e);
                    }
                }
                //try get aggregate
                else
                {
                    try
                    {
                        //get plugin
                        var aggregate = myPluginManager.GetAndInitializePlugin<IGQLAggregate>(_AggregateName);

                        if (aggregate != null)
                        {
                            resultingVertices = new List<IVertexView>() { GenerateOutput(aggregate, _AggregateName) };
                        }
                        else
                        {
                            error = new AggregateOrFunctionDoesNotExistException(typeof(IGQLAggregate), _AggregateName, "");
                        }
                    }
                    catch (ASonesException e)
                    {
                        //maybe user forgot prefix 'sones.'
                        try
                        {
                            //get plugin
                            var aggregate = myPluginManager.GetAndInitializePlugin<IGQLAggregate>("SONES." + _AggregateName);

                            if (aggregate != null)
                            {
                                resultingVertices = new List<IVertexView>() { GenerateOutput(aggregate, "SONES." + _AggregateName) };
                            }
                            else
                            {
                                error = new AggregateOrFunctionDoesNotExistException(typeof(IGQLAggregate), _AggregateName, "");
                            }
                        }
                        catch (ASonesException ee)
                        {
                            error = new AggregateOrFunctionDoesNotExistException(typeof(IGQLAggregate), _AggregateName, "", e);
                        }
                    }
                }

                #endregion

            }

            else
            {

                #region All aggregates

                myPluginManager.GetPluginsForType<IGQLAggregate>();
                foreach (var aggregateName in myPluginManager.GetPluginsForType<IGQLAggregate>())
                {
                    try
                    {
                        var aggregate = myPluginManager.GetAndInitializePlugin<IGQLAggregate>(aggregateName);

                        if (aggregate != null)
                        {
                            resultingVertices.Add(GenerateOutput(aggregate, aggregateName));
                        }
                        else
                        {
                            error = new AggregateOrFunctionDoesNotExistException(typeof(IGQLAggregate), _AggregateName, "");
                        }
                    }
                    catch (ASonesException e)
                    {
                        error = new AggregateOrFunctionDoesNotExistException(typeof(IGQLAggregate), _AggregateName, "", e);
                    }
                }

                #endregion

            }

            //an error occured
            if(error != null)
                return new QueryResult("", "GQL", 0L, ResultType.Failed, resultingVertices, error);
            else
                return new QueryResult("", "GQL", 0L, ResultType.Successful, resultingVertices);
        }

        #region GenerateOutput

        /// <summary>
        /// Generate an output for an aggregate description.
        /// </summary>
        /// <param name="myAggregate">The aggregate.</param>
        /// <param name="myAggrName">The aggregate name.</param>
        /// <returns>List of readouts with the information.</returns>
        private IVertexView GenerateOutput(IGQLAggregate myAggregate, String myAggrName)
        {

            var _Aggregate = new Dictionary<String, Object>();

            var temp = new Dictionary<String, object>();
            var edges = new Dictionary<String, IEdgeView>();

            _Aggregate.Add("Aggregate", myAggregate.AggregateName);
            _Aggregate.Add("Type", myAggrName);
            _Aggregate.Add("Description", myAggregate.GetDescribeOutput());

            int count = 1;
            foreach (var parameter in ((IPluginable)myAggregate).SetableParameters)
            {
                temp.Add("Parameter " + count.ToString() + " Key: ", parameter.Key);

                count++;
            }

            edges.Add("SetableParameters", new SingleEdgeView(temp, null));

            return new VertexView(_Aggregate, edges);

        }

        #endregion
    }
}
