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

    /// <summary>
    /// Set the unique flag for an attribute
    /// </summary>
    public class AlterType_SetUnique : AAlterTypeCommand
    {

        /// <summary>
        /// List of unique attributes of the given vertex
        /// </summary>
        public List<String> UniqueAttributes { get; set; }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.Unqiue; }
        }

        /// <summary>
        /// Set the unique flag
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return myGraphDBType.ChangeUniqueAttributes(UniqueAttributes, myDBContext);
        }

    }

}
