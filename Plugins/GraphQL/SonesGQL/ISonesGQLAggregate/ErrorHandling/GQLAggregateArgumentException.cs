using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.SonesGQL.Aggregates.ErrorHandling
{
    public sealed class GQLAggregateArgumentException : ASonesQLAggregateException
    {
        /// <summary>
        /// Creates a new GQL Aggregate argument exception
        /// </summary>
        /// <param name="myInfo">The info concerning the exception</param>
        public GQLAggregateArgumentException(string myInfo)
        {
            _msg = myInfo;
        }
    }
}
