using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphQL.Result;
using sones.GraphDB.TypeSystem;

namespace sones.GraphQL.GQL.Structure.Helper.Definition.AlterType
{
    /// <summary>
    /// Set the unique flag for an attribute
    /// </summary>
    public sealed class AlterType_SetUnique : AAlterTypeCommand
    {

        /// <summary>
        /// List of unique attributes of the given vertex
        /// </summary>
        public List<String> UniqueAttributes { get; set; }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.Unqiue; }
        }


        public override IVertexView CreateResult(IVertexType myAlteredVertexType)
        {
            throw new NotImplementedException();
        }
    }
}
