/*
 * AlterType_RenameAttribute
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Structures.Enums;
using sones.GraphDB.TypeManagement;

using sones.Lib.ErrorHandling;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDB.Managers.AlterType
{

    public class AlterType_RenameAttribute : AAlterTypeCommand
    {

        public String OldName { get; set; }
        public String NewName { get; set; }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.RenameAttribute; }
        }

        /// <summary>
        /// Executes the renaming of the attribute of a given type.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="graphDBType"></param>
        /// <returns></returns>
        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return myDBContext.DBTypeManager.RenameAttributeOfType(myGraphDBType, OldName, NewName);
        }

        public override IEnumerable<Vertex> CreateVertex(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return CreateRenameResult("RENAME ATTRIBUTE", OldName, NewName, myGraphDBType);
        }

    }

}
