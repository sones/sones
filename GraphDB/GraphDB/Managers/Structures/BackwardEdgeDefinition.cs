/*
 * BackwardEdgeDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.EdgeTypes;

#endregion

namespace sones.GraphDB.Managers.Structures
{
    public class BackwardEdgeDefinition
    {

        public BackwardEdgeDefinition(string myAttributeName, string myTypeName, string myTypeAttributeName, AEdgeType myEdgeType)
        {
            AttributeName = myAttributeName;
            TypeName = myTypeName;
            TypeAttributeName = myTypeAttributeName;
            EdgeType = myEdgeType;
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

        /// <summary>
        /// The Type of the edge, currently EdgeTypeList or EdgeTypeWeightedList
        /// </summary>
        public AEdgeType EdgeType { get; private set; }

        #endregion
    
    }
}
