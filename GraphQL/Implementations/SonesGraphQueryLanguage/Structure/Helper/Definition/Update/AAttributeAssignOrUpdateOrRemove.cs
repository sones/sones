using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Nodes.Misc;

namespace sones.GraphQL.GQL.Structure.Helper.Definition.Update
{
    /// <summary>
    /// This is the abstract base class for all attribute manipulations
    /// </summary>
    public abstract class AAttributeAssignOrUpdateOrRemove
    {

        #region Properties

        /// <summary>
        /// The attribute chain definition
        /// </summary>
        public IDChainDefinition AttributeIDChain { get; protected set; }

        #endregion

        #region IsUndefinedAttributeAssign

        /// <summary>
        /// Return true if the attribute is undefined
        /// </summary>
        public bool IsUndefinedAttributeAssign
        {
            get
            {
                return AttributeIDChain == null || AttributeIDChain.IsUndefinedAttribute;
            }
        }

        #endregion

    }
}
