using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphQL.Result;

namespace sones.GraphQL.GQL.Structure.Helper.Definition.AlterType
{
    /// <summary>
    /// Change the name of a vertex
    /// </summary>
    public sealed class AlterType_RenameType : AAlterTypeCommand
    {

        /// <summary>
        /// The old name of the vertex
        /// </summary>
        public String OldName { get; set; }
        /// <summary>
        /// The new name of the vertex
        /// </summary>
        public String NewName { get; set; }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.RenameType; }
        }

        public override IVertexView CreateResult(GraphDB.TypeSystem.IVertexType myAlteredVertexType)
        {
            throw new NotImplementedException();
        }
    }
}
