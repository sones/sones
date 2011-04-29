using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Nodes.Misc;

namespace sones.GraphQL.GQL.Structure.Helper.Definition.Update
{
    /// <summary>
    /// Assign or update base attributes
    /// </summary>
    public class AttributeAssignOrUpdateValue : AAttributeAssignOrUpdate
    {

        #region Properties

        /// <summary>
        /// The value for the attribute
        /// </summary>
        public Object Value { get; private set; }

        #endregion

        #region Ctor

        public AttributeAssignOrUpdateValue(IDChainDefinition myIDChainDefinition, Object myValue)
            : base(myIDChainDefinition)
        {
            this.Value = myValue;
        }

        #endregion

    }
}
