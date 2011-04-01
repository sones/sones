using System;
using System.Collections.Generic;
using sones.GraphDB.Request.Helper.Expression;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The definition of what kind of vertices should be requested from the graphdb
    /// </summary>
    public sealed class GetVerticesDefinition
    {
        #region Data

        /// <summary>
        /// The expression which should be evaluated
        /// </summary>
        public readonly IExpression Expression;

        /// <summary>
        /// Determines whether it is anticipated that the request could take longer
        /// </summary>
        public readonly Boolean IsLongrunning;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new get vertices definition
        /// </summary>
        /// <param name="myExpression">The expression which should be evaluated</param>
        /// <param name="myIsLongrunning">Determines whether it is anticipated that the request could take longer</param>
        public GetVerticesDefinition(IExpression myExpression, Boolean myIsLongrunning = false)
        {
            Expression = myExpression;
            IsLongrunning = myIsLongrunning;
        }

        #endregion
    }
}