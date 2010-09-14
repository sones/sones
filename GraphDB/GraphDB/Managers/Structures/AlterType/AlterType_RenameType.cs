/*
 * AlterType_RenameType
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

    public class AlterType_RenameType : AAlterTypeCommand
    {

        public String OldName { get; set; }
        public String NewName { get; set; }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.RenameType; }
        }

        /// <summary>
        /// Execute the renaming of a given type.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="graphDBType"></param>
        /// <returns></returns>
        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            OldName = myGraphDBType.Name;
            return myDBContext.DBTypeManager.RenameType(myGraphDBType, NewName);
        }

        public override IEnumerable<Vertex> CreateVertex(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return CreateRenameResult("RENAME TYPE", OldName, myGraphDBType.Name, myGraphDBType);
        }

    }

}
