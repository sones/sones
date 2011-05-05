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
    /// Removes the mandatory flag of an attribute
    /// </summary>
    public sealed class AlterType_DropMandatory : AAlterTypeCommand
    {
        public readonly String DroppedMandatory;

        public AlterType_DropMandatory(String myDroppedMandatory)
        {
            DroppedMandatory = myDroppedMandatory;
        }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.DropMandatory; }
        }

        public override IVertexView CreateResult(IVertexType myAlteredVertexType)
        {
            throw new NotImplementedException();
        }
    }
}
