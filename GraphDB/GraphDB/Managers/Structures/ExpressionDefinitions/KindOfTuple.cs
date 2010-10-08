using System;
using System.Collections.Generic;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;

namespace sones.GraphDB.Structures.Operators
{

    public enum KindOfTuple
    {
        Inclusive,
        LeftExclusive,
        RightExclusive,
        Exclusive
    }
}
