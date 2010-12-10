/*
 * DescribeAggregateDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.Aggregates;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Result;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDB.Managers.Structures.Describe
{
    /// <summary>
    /// Describes aggregates
    /// </summary>
    public class DescribeAggregateDefinition : ADescribeDefinition
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

        #region ADescribeDefinition

        /// <summary>
        /// <seealso cref=" ADescribeDefinition"/>
        /// </summary>
        public override Exceptional<IEnumerable<Vertex>> GetResult(DBContext myDBContext)
        {

            if (!String.IsNullOrEmpty(_AggregateName))
            {

                #region Specific aggregate

                var aggregate = myDBContext.DBPluginManager.GetAggregate(_AggregateName);
                if (aggregate != null)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new List<Vertex>(){GenerateOutput(aggregate, _AggregateName)});
                }
                else
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_AggregateOrFunctionDoesNotExist(_AggregateName));
                }

                #endregion

            }

            else
            {

                #region All aggregates

                var resultingVertices = new List<Vertex>();

                foreach (var aggregate in myDBContext.DBPluginManager.GetAllAggregates())
                {
                    resultingVertices.Add(GenerateOutput(aggregate.Value, aggregate.Key));
                }

                return new Exceptional<IEnumerable<Vertex>>(resultingVertices);

                #endregion

            }

        }

        #endregion

        #region GenerateOutput

        /// <summary>
        /// generate an output for an aggregate
        /// </summary>
        /// <param name="myAggregate">the aggregate</param>
        /// <param name="myAggrName">aggregate name</param>
        /// <returns>list of readouts with the information</returns>
        private Vertex GenerateOutput(ABaseAggregate myAggregate, String myAggrName)
        {

            var _Aggregate = new Dictionary<String, Object>();

            _Aggregate.Add("Aggregate", myAggrName);
            _Aggregate.Add("Type",      myAggrName);

            return new Vertex(_Aggregate);

        }
        
        #endregion

    }
}
