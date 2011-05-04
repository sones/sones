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
    /// Drop attributes from a vertex
    /// </summary>
    public sealed class AlterType_DropAttributes : AAlterTypeCommand
    {
        public List<String> ListOfAttributes = new List<String>();

        public AlterType_DropAttributes(List<String> listOfAttributes)
        {
            ListOfAttributes = listOfAttributes;
        }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.DropAttribute; }
        }

        public override IVertexView CreateResult(IVertexType myAlteredVertexType)
        {
            throw new NotImplementedException();
        }
    }
}
