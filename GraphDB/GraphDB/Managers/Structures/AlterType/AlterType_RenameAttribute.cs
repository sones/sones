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

    /// <summary>
    /// Change the name of an vertex attribute
    /// </summary>
    public class AlterType_RenameAttribute : AAlterTypeCommand
    {

        /// <summary>
        /// The old name of the attribute
        /// </summary>
        public String OldName { get; set; }
        /// <summary>
        /// The new name of the attribute
        /// </summary>
        public String NewName { get; set; }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.RenameAttribute; }
        }

        /// <summary>
        /// Executes the renaming of the attribute of a given type.
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return myDBContext.DBTypeManager.RenameAttributeOfType(myGraphDBType, OldName, NewName);
        }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>        
        public override IEnumerable<Vertex> CreateVertex(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return CreateRenameResult("RENAME ATTRIBUTE", OldName, NewName, myGraphDBType);
        }

    }

}
