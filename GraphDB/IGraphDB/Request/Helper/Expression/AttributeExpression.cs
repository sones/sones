using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Request.Helper.Expression
{

    /// <summary>
    /// This class represents an attribute expression
    /// </summary>
    public sealed class AttributeExpression : IExpression
    {
        #region Data

        /// <summary>
        /// The name of the vertex type
        /// </summary>
        public readonly String NameOfVertexType;

        /// <summary>
        /// The name of the attribute
        /// </summary>
        public readonly String NameOfAttribute;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new Attribute expression
        /// </summary>
        /// <param name="myNameOfVertexType">The name of the vertex type</param>
        /// <param name="myNameOfAttribute">The name of the attribute</param>
        public AttributeExpression(String myNameOfVertexType, String myNameOfAttribute)
        {
            NameOfVertexType = myNameOfVertexType;
            NameOfAttribute = myNameOfAttribute;
        }

        #endregion
    }
}
