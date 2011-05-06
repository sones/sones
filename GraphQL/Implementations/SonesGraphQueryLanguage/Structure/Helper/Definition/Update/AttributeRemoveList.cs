using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Nodes.Misc;

namespace sones.GraphQL.GQL.Structure.Helper.Definition.Update
{
    /// <summary>
    /// Removes some values from all DBOs of the given IDChain or attributeName
    /// </summary>
    public sealed class AttributeRemoveList : AAttributeRemove
    {

        #region Properties

        /// <summary>
        /// The name of the attribute
        /// </summary>
        public string AttributeName { get; private set; }
        public Object TupleDefinition { get; private set; }

        #endregion

        #region Ctor

        public AttributeRemoveList(IDChainDefinition myIDChainDefinition, string myAttributeName, Object myTupleDefinition)
        {
            // TODO: Complete member initialization
            this.AttributeIDChain = myIDChainDefinition;
            this.AttributeName = myAttributeName;
            this.TupleDefinition = myTupleDefinition;
        }

        #endregion
    }
}
