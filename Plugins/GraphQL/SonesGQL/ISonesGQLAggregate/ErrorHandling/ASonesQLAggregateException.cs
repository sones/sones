using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.ErrorHandling;
using sones.GraphQL.ErrorHandling;

namespace sones.Plugins.SonesGQL.Aggregates.ErrorHandling
{
    /// <summary>
    /// The abstract class for all sones GQL aggregate exceptions
    /// </summary>
    public abstract class ASonesQLAggregateException : AGraphQLException
    {
    }
}
