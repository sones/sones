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

        /// <summary>
        /// Creates a new instance of MandatoryPredefinition.
        /// </summary>
        /// <param name="myProperty">The property that will be mandatory.</param>
        public MandatoryPredefinition(String myProperty) 
        {
            MandatoryAttribute = myProperty;
        }
    }
}
