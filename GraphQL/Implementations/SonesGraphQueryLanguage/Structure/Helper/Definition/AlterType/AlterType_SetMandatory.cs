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
    /// Set the mandatory flag for an attribute
    /// </summary>
    public sealed class AlterType_SetMandatory : AAlterTypeCommand
    {

        /// <summary>
        /// List of mandatory attributes of the given vertex
        /// </summary>
        public List<String> MandatoryAttributes { get; set; }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.Mandatory; }
        }


        public override IVertexView CreateResult(IVertexType myAlteredVertexType)
        {
            throw new NotImplementedException();
        }
    }
}
