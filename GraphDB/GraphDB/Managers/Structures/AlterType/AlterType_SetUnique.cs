/*
 * AlterType_SetUnique
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Structures.Enums;
using sones.GraphDB.TypeManagement;

using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Managers.AlterType
{

    public class AlterType_SetUnique : AAlterTypeCommand
    {

        public List<String> UniqueAttributes { get; set; }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.Unqiue; }
        }

        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return myGraphDBType.ChangeUniqueAttributes(UniqueAttributes, myDBContext);
        }

    }

}
