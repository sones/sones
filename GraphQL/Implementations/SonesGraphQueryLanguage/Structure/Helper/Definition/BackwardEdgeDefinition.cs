using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    public sealed class IncomingEdgeDefinition
    {

        public IncomingEdgeDefinition(string myAttributeName, string myTypeName, string myTypeAttributeName)
        {
            AttributeName = myAttributeName;
            TypeName = myTypeName;
            TypeAttributeName = myTypeAttributeName;
        }

        #region Data

        /// <summary>
        /// The destination type of the backwardedge
        /// </summary>
        public String TypeName { get; private set; }

        /// <summary>
        /// the destination attribute on the TypeName
        /// </summary>
        public String TypeAttributeName { get; private set; }

        /// <summary>
        /// The real new name of the attribute
        /// </summary>
        public String AttributeName { get; private set; }

        #endregion

    }
}
