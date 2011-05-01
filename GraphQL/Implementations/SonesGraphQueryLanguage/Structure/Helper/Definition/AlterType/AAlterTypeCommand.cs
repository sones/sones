using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.Result;

namespace sones.GraphQL.GQL.Structure.Helper.Definition.AlterType
{
    /// <summary>
    /// This is the abstract class for all alter type commands
    /// </summary>
    public abstract class AAlterTypeCommand
    {
        /// <summary>
        /// The type of the alter type command
        /// </summary>
        public abstract TypesOfAlterCmd AlterType { get; }

        /// <summary>
        /// Creates the result of altering a vertex type
        /// </summary>
        /// <param name="myAlteredVertexType">The vertex type that has been altered</param>
        /// <returns>A vertex view</returns>
        public abstract IVertexView CreateResult(IVertexType myAlteredVertexType);
    }
}
