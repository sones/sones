using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.Structure.Helper.Enums
{
    public enum TypesOfAlterCmd
    {
        Add,
        Drop,
        RenameAttribute,
        RenameType,
        RenameBackwardedge,
        Unqiue,
        DropUnqiue,
        Mandatory,
        DropMandatory,
        ChangeComment
    }
}
