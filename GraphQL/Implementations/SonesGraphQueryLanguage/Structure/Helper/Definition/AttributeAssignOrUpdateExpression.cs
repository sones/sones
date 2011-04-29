using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Nodes.Misc;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    /// <summary>
    /// A class that contains the BinaryExpressionDefinition of an attribute with his values
    /// </summary>
    public sealed class AttributeAssignOrUpdateExpression : AAttributeAssignOrUpdate
    {

        #region Properties

        /// <summary>
        /// The BinaryExpressionDefinition of an attribute
        /// </summary>
        public BinaryExpressionDefinition BinaryExpressionDefinition { get; private set; }

        #endregion

        #region Ctor

        public AttributeAssignOrUpdateExpression(IDChainDefinition myIDChainDefinition, BinaryExpressionDefinition binaryExpressionDefinition)
            : base(myIDChainDefinition)
        {
            BinaryExpressionDefinition = binaryExpressionDefinition;
        }

        #endregion
    }
}
