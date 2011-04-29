using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;
using sones.GraphQL.GQL.Structure.Nodes.Misc;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    /// <summary>
    /// Assign or update a list attribute
    /// </summary>
    public sealed class AttributeAssignOrUpdateList : AAttributeAssignOrUpdate
    {

        #region Properties

        public CollectionDefinition CollectionDefinition { get; private set; }
        public Boolean Assign { get; private set; }

        #endregion


        #region Ctor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myCollectionDefinition"></param>
        /// <param name="iDChainDefinition"></param>
        /// <param name="myAssign">True to assign, False to update</param>
        public AttributeAssignOrUpdateList(CollectionDefinition myCollectionDefinition, IDChainDefinition iDChainDefinition, Boolean myAssign)
        {
            CollectionDefinition = myCollectionDefinition;
            AttributeIDChain = iDChainDefinition;
            Assign = myAssign;
        }

        #endregion

    }
}
