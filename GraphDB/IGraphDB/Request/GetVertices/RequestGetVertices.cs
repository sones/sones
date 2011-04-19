using sones.GraphDB.Expression;
using System;
namespace sones.GraphDB.Request
{
    /// <summary>
    /// The get vertices request
    /// </summary>
    public sealed class RequestGetVertices : IRequest
    {
        #region data

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
        /// Creates a new request that gets vertices from the Graphdb
        /// </summary>
        /// <param name="myExpression">The expression which should be evaluated</param>
        /// <param name="myIsLongrunning">Determines whether it is anticipated that the request could take longer</param>
        public RequestGetVertices(IExpression myExpression, Boolean myIsLongrunning = false)
        {
            Expression = myExpression;
            IsLongrunning = myIsLongrunning;
        }

        #endregion

        #region IRequest Members

        public GraphDBAccessMode AccessMode
        {
            get { return GraphDBAccessMode.ReadOnly; }
        }

        #endregion
    }
}
