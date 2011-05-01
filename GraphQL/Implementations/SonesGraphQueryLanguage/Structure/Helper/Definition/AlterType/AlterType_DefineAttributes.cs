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
    /// Create defined attributes for undefined attributes
    /// </summary>
    public sealed class AlterType_DefineAttributes : AAlterTypeCommand
    {

        #region data

        private List<AttributeDefinition> _ListOfAttributes;

        #endregion

        public override TypesOfAlterCmd AlterType
        {
            get { throw new NotImplementedException(); }
        }

        public AlterType_DefineAttributes(List<AttributeDefinition> listOfAttributes)
        {
            _ListOfAttributes = listOfAttributes;
        }

        public override IVertexView CreateResult(IVertexType myAlteredVertexType)
        {
            throw new NotImplementedException();
        }
    }
}
