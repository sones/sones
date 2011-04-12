using System;

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

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new property expression
        /// </summary>
        /// <param name="myNameOfVertexType">The name of the vertex type</param>
        /// <param name="myNameOfProperty">The name of the attribute</param>
        public PropertyExpression(String myNameOfVertexType, String myNameOfProperty)
        {
            NameOfVertexType = myNameOfVertexType;
            NameOfProperty = myNameOfProperty;
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
