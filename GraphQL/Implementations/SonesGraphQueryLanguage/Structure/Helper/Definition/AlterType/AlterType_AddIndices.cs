using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Result;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphDB.TypeSystem;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.GQL.Structure.Helper.Definition.AlterType
{
    /// <summary>
    /// Adds indices to a vertex
    /// </summary>
    public sealed class AlterType_AddIndices : AAlterTypeCommand
    {
        public readonly List<IndexDefinition> IdxDefinitionList;

        public AlterType_AddIndices(List<IndexDefinition> listOfIndices)
        {
            IdxDefinitionList = listOfIndices;
        }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override TypesOfAlterCmd AlterType
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Checks whether the index can be changed
        /// </summary>
        /// <param name="idxDef">List of index attribute definitions</param>
        /// <param name="type">The db type that is to be altered</param>
        /// <returns>An exceptional</returns>
        private void CheckIndexTypeReference(List<IndexAttributeDefinition> idxDef, IVertexType type)
        {
            foreach (var idx in idxDef)
            {
                if (idx.IndexType != null && idx.IndexType != type.Name)
                {
                    throw new CouldNotAlterIndexOnTypeException(idx.IndexType);
                }
            }
        }

        public override IVertexView CreateResult(IVertexType myAlteredVertexType)
        {
            throw new NotImplementedException();
        }
    }
}
