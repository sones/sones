using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    public enum CollectionType
    {
        Set,
        List,
        SetOfUUIDs
    }

    public sealed class CollectionDefinition
    {

        #region Properties

        public CollectionType CollectionType { get; private set; }
        public TupleDefinition TupleDefinition { get; private set; }

        #endregion

        #region Ctor

        public CollectionDefinition(CollectionType myCollectionType, TupleDefinition myTupleDefinition)
        {
            CollectionType = myCollectionType;
            TupleDefinition = myTupleDefinition;
        }

        #endregion

    }
}
