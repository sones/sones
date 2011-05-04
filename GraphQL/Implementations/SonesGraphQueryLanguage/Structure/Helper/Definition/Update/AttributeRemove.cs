using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.GQL.Structure.Helper.Definition.Update
{
    /// <summary>
    /// Removes some attributes
    /// </summary>
    public sealed class AttributeRemove : AAttributeRemove
    {

        #region Properties

        /// <summary>
        /// The list of attributes to remove
        /// </summary>
        public List<string> ToBeRemovedAttributes { get; private set; }

        #endregion

        #region Ctor

        public AttributeRemove(List<string> _toBeRemovedAttributes)
        {
            // TODO: Complete member initialization
            this.ToBeRemovedAttributes = _toBeRemovedAttributes;
        }

        #endregion
    }
}
