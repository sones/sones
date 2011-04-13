using System;
using sones.GraphDB.Expression.Tree;

namespace sones.GraphDB.Expression
{
    /// <summary>
    /// This class represents an property expression
    /// </summary>
    public sealed class PropertyExpression : IExpression
    {
        #region Data

        /// <summary>
        /// The name of the vertex type
        /// </summary>
        public readonly String NameOfVertexType;

        /// <summary>
        /// The name of the attribute
        /// </summary>
        public readonly String NameOfProperty;

        /// <summary>
        /// The edition that should be processed
        /// </summary>
        public readonly String Edition;

        /// <summary>
        /// The timespan that should be processed
        /// </summary>
        public readonly TimeSpanDefinition Timespan;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new property expression
        /// </summary>
        /// <param name="myNameOfVertexType">The name of the vertex type</param>
        /// <param name="myNameOfProperty">The name of the attribute</param>
        /// <param name="myEditionName">The edition that should be processed</param>
        /// <param name="myTimeSpanDefinition">The timespan that should be processed</param>
        public PropertyExpression(String myNameOfVertexType, String myNameOfProperty, String myEditionName = null, TimeSpanDefinition myTimeSpanDefinition = null)
        {
            NameOfVertexType = myNameOfVertexType;
            NameOfProperty = myNameOfProperty;
            Edition = myEditionName;
            Timespan = myTimeSpanDefinition;
        }

        #endregion

        #region IExpression Members

        public TypeOfExpression TypeOfExpression
        {
            get { return TypeOfExpression.Property; }
        }

        #endregion
    }
}
