using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Request.CreateVertexTypes
{
    /// <summary>
    /// A class that represents a property that has to be mandatory
    /// </summary>
    public sealed class MandatoryPredefinition
    {
        public readonly String MandatoryAttribute;
        public readonly Object DefaultValue;

        /// <summary>
        /// Creates a new instance of MandatoryPredefinition.
        /// </summary>
        /// <param name="myProperty">The property that will be mandatory.</param>
        /// <param name="myDefaultValue">The default value for the mandatory property</param>
        public MandatoryPredefinition(String myProperty, Object myDefaultValue = null) 
        {
            MandatoryAttribute = myProperty;
            DefaultValue = myDefaultValue;
        }
    }
}
