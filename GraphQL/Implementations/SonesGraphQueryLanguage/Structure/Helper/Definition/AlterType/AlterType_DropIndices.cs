using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.Result;
using sones.GraphQL.Structure.Helper.Enums;

namespace sones.GraphQL.GQL.Structure.Helper.Definition.AlterType
{
    /// <summary>
    /// Removal of attribute indices
    /// </summary>
    public sealed class AlterType_DropIndices : AAlterTypeCommand
    {
        /// <summary>
        /// The list of the indices
        /// </summary>
        public readonly Dictionary<String, String> IdxDropList;

        public AlterType_DropIndices(Dictionary<String, String> myIndices)
        {
            IdxDropList = myIndices;
        }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.DropIndex; }
        }

        public override IVertexView CreateResult(IVertexType myAlteredVertexType)
        {
            throw new NotImplementedException();
        }
    }
}
