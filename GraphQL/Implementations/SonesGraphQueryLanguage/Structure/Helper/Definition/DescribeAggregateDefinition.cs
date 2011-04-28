using System;
using System.Collections.Generic;
using Irony.Parsing;
using sones.Library.PropertyHyperGraph;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.Plugins.SonesGQL.Aggregates;
using sones.GraphQL.GQL.ErrorHandling;
using sones.Library.ErrorHandling;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

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
        public override IEnumerable<IVertexView> GetResult(ParsingContext myContext,
                                                            GQLPluginManager myPluginManager,
                                                            IGraphDB myGraphDB,
                                                            SecurityToken mySecurityToken,
                                                            TransactionToken myTransactionToken)
        {
            if (!String.IsNullOrEmpty(_AggregateName))
            {

                #region Specific aggregate

                try
                {
                    var aggregate = myPluginManager.GetAndInitializePlugin<IGQLAggregate>(_AggregateName);

                    if (aggregate != null)
                    {
                        return new List<IVertexView>() { GenerateOutput(aggregate, _AggregateName) };
                    }
                    else
                    {
                        throw new AggregateOrFunctionDoesNotExistException(typeof(IGQLAggregate), _AggregateName, "");
                    }
                }
                catch (ASonesException e)
                {
                    throw new AggregateOrFunctionDoesNotExistException(typeof(IGQLAggregate), _AggregateName, "", e);
                }

                #endregion

            }

            else
            {

                #region All aggregates

                var resultingVertices = new List<IVertexView>();

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
                            throw new AggregateOrFunctionDoesNotExistException(typeof(IGQLAggregate), _AggregateName, "");
                        }
                    }
                    catch (ASonesException e)
                    {
                        throw new AggregateOrFunctionDoesNotExistException(typeof(IGQLAggregate), _AggregateName, "", e);
                    }
                }

                return resultingVertices;

                #endregion

            }
        }

        #region GenerateOutput

        /// <summary>
        /// generate an output for an aggregate
        /// </summary>
        /// <param name="myAggregate">the aggregate</param>
        /// <param name="myAggrName">aggregate name</param>
        /// <returns>list of readouts with the information</returns>
        private IVertexView GenerateOutput(IGQLAggregate myAggregate, String myAggrName)
        {

            var _Aggregate = new Dictionary<String, Object>();

            _Aggregate.Add("Aggregate", myAggregate.PluginName);
            _Aggregate.Add("Type", myAggrName);

            foreach (var parameter in myAggregate.SetableParameters)
            {
                _Aggregate.Add("SetableParameter Key ", parameter.Key);
                _Aggregate.Add("SetableParameter Value ", parameter.Value);
            }

            return new VertexView(_Aggregate, new Dictionary<String, IEdgeView>());

        }

        #endregion
    }
}
