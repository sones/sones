using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    /// <summary>
    /// This class holds all characteristics of an specific attribute of a DB type.
    /// </summary>
    public sealed class TypeCharacteristics
    {

        /// <summary>
        /// This attribute is an incoming edge
        /// </summary>
        public Boolean IsIncomingEdge { get; set; }

        /// <summary>
        /// If set to true, all object of the RelatedGraphType with this attribute a unique for this RelatedGraphType.
        /// If more attribute are set to UNIQUE, all together are unique.
        /// </summary>
        public Boolean IsUnique { get; set; }

        /// <summary>
        /// The Mandatory flag
        /// </summary>
        public Boolean IsMandatory { get; set; }

        /// <summary>
        /// Create standard Unique typeCharacteristic (no Queue, no Weighted)
        /// </summary>
        public TypeCharacteristics()
        {
            IsIncomingEdge = false;
            IsUnique = false;
            IsMandatory = false;
        }
    }
}
