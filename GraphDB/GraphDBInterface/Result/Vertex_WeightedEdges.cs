/* <id name="GraphDB – WeightedAttributeObject" />
 * <copyright file="WeightedAttributeObject.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>Carries information of DBObject WeightedList Attribute.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDB.Result
{

    /// <summary>
    /// Carries information of DBObjects but without their whole functionality.
    /// </summary>
    public class Vertex_WeightedEdges : Vertex
    {

        public Object Weight   { get; private set; }
        public String TypeName { get; private set; }

        public Vertex_WeightedEdges(IDictionary<String, Object> myAttributes, object myWeight, String myTypeName)
            : base (myAttributes)
        {
            Weight   = myWeight;
            TypeName = myTypeName;
        }

    }

}
