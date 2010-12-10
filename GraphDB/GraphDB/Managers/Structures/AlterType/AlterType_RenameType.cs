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

    /// <summary>
    /// Change the name of a vertex
    /// </summary>
    public class AlterType_RenameType : AAlterTypeCommand
    {

        /// <summary>
        /// The old name of the vertex
        /// </summary>
        public String OldName { get; set; }
        /// <summary>
        /// The new name of the vertex
        /// </summary>
        public String NewName { get; set; }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.RenameType; }
        }

        /// <summary>
        /// Execute the renaming of a given type.
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            OldName = myGraphDBType.Name;
            return myDBContext.DBTypeManager.RenameType(myGraphDBType, NewName);
        }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override IEnumerable<Vertex> CreateVertex(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return CreateRenameResult("RENAME TYPE", OldName, myGraphDBType.Name, myGraphDBType);
        }

    }

}
