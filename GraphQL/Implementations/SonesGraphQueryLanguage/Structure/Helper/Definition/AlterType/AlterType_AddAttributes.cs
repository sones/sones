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
    /// Adds attributes to a vertex
    /// </summary>
    public sealed class AlterType_AddAttributes : AAlterTypeCommand
    {

        public readonly List<AttributeDefinition> ListOfAttributes;
        public readonly List<BackwardEdgeDefinition> BackwardEdgeInformation;

        public AlterType_AddAttributes(List<AttributeDefinition> listOfAttributes, List<BackwardEdgeDefinition> backwardEdgeInformation)
        {
            ListOfAttributes = listOfAttributes;
            BackwardEdgeInformation = backwardEdgeInformation;
        }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.Add; }
        }

        public override IVertexView CreateResult(IVertexType myAlteredVertexType)
        {
            throw new NotImplementedException();
        }
    }
}
