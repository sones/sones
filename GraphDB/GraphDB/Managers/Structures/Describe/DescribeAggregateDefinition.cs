/*
 * DescribeAggregateDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.Aggregates;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Managers.Structures.Describe
{
    public class DescribeAggregateDefinition : ADescribeDefinition
    {

        #region Data

        private String _AggregateName;

        #endregion

        #region Ctor

        public DescribeAggregateDefinition(String myAggregateName = null)
        {
            _AggregateName = myAggregateName;
        }

        #endregion

        #region ADescribeDefinition

        public override Exceptional<List<SelectionResultSet>> GetResult(DBContext myDBContext)
        {

            var result = new List<SelectionResultSet>();

            if (!String.IsNullOrEmpty(_AggregateName))
            {

                #region Specific aggregate

                var aggregate = myDBContext.DBPluginManager.GetAggregate(_AggregateName);
                if (aggregate != null)
                {
                    result.Add(new SelectionResultSet(GenerateOutput(aggregate, _AggregateName)));
                }
                else
                {
                    return new Exceptional<List<SelectionResultSet>>(new Error_AggregateOrFunctionDoesNotExist(_AggregateName));
                }

                #endregion

            }
            else
            {

                #region All aggregates

                foreach (var aggregate in myDBContext.DBPluginManager.GetAllAggregates())
                {
                    result.Add(new SelectionResultSet(GenerateOutput(aggregate.Value, aggregate.Key)));
                }

                #endregion

            }

            return new Exceptional<List<SelectionResultSet>>(result);

        }

        #endregion

        #region GenerateOutput

        /// <summary>
        /// generate an output for an aggregate
        /// </summary>
        /// <param name="myAggregate">the aggregate</param>
        /// <param name="myAggrName">aggregate name</param>
        /// <returns>list of readouts with the information</returns>
        private IEnumerable<DBObjectReadout> GenerateOutput(ABaseAggregate myAggregate, String myAggrName)
        {

            var _Aggregate = new Dictionary<String, Object>();

            _Aggregate.Add("Aggregate", myAggrName);
            _Aggregate.Add("Type", myAggrName);
            _Aggregate.Add("ResultType", myAggregate.TypeOfResult);

            return new List<DBObjectReadout>() { new DBObjectReadout(_Aggregate) };

        }
        
        #endregion

    }
}
