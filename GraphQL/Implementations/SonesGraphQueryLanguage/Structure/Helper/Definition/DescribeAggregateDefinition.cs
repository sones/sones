/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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
                                                Int64 myTransactionToken)
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
                        catch (ASonesException)
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

                myPluginManager.GetPluginNameForType<IGQLAggregate>();
                foreach (var aggregateName in myPluginManager.GetPluginNameForType<IGQLAggregate>())
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
                return new QueryResult("", SonesGQLConstants.GQL, 0L, ResultType.Failed, resultingVertices, error);
            else
                return new QueryResult("", SonesGQLConstants.GQL, 0L, ResultType.Successful, resultingVertices);
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

            _Aggregate.Add("Aggregate", myAggregate.PluginShortName);
            _Aggregate.Add("Type", myAggrName);
            _Aggregate.Add("Description", myAggregate.PluginDescription);

            int count = 1;
            foreach (var parameter in ((IPluginable)myAggregate).SetableParameters)
            {
                temp.Add("Parameter " + count.ToString() + " Key: ", parameter.Key);

                count++;
            }

            edges.Add("SetableParameters", new SingleEdgeView(null, new VertexView(temp, null)));

            return new VertexView(_Aggregate, edges);

        }

        #endregion
    }
}
