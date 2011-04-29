using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;
using sones.GraphQL.GQL.Structure.Nodes.Misc;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    /// <summary>
    /// Assign or update reference values for attributes
    /// </summary>
    public sealed class AttributeAssignOrUpdateSetRef : AAttributeAssignOrUpdate
    {

        #region Properties

        /// <summary>
        /// The reference definition
        /// </summary>
        public SetRefDefinition SetRefDefinition { get; private set; }

        #endregion

        #region Ctor

        public AttributeAssignOrUpdateSetRef(IDChainDefinition myIDChainDefinition, SetRefDefinition mySetRefDefinition)
            : base(myIDChainDefinition)
        {
            SetRefDefinition = mySetRefDefinition;
        }

        #endregion

        #region override ToString

        public override string ToString()
        {
            return "SetRefNode";
        }

        #endregion

    }
}
