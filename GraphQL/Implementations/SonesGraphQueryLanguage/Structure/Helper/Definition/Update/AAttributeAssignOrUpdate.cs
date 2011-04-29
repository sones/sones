using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Nodes.Misc;

namespace sones.GraphQL.GQL.Structure.Helper.Definition.Update
{
    /// <summary>
    /// Abstract class to assign or update attributes
    /// </summary>
    public abstract class AAttributeAssignOrUpdate : AAttributeAssignOrUpdateOrRemove
    {

        #region Ctors

        public AAttributeAssignOrUpdate() { }

        public AAttributeAssignOrUpdate(IDChainDefinition myIDChainDefinition)
        {
            AttributeIDChain = myIDChainDefinition;
        }

        #endregion

    }
}
