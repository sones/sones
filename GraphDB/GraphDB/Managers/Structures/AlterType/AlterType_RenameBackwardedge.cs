/*
 * AlterType_RenameBackwardedge
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.NewAPI;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures.Enums;

using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Managers.AlterType
{

    /// <summary>
    /// Change the name of a backwardedge
    /// </summary>
    public class AlterType_RenameBackwardedge : AAlterTypeCommand
    {
        /// <summary>
        /// The old name of the backwardedge
        /// </summary>
        public String OldName { get; set; }
        /// <summary>
        /// The new name of the backwardedge
        /// </summary>
        public String NewName { get; set; }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.RenameBackwardedge; }
        }

        /// <summary>
        /// Execute the renaming of the backwardedge of a given type.
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {

            TypeAttribute Attribute = myGraphDBType.GetTypeAttributeByName(OldName);

            if (Attribute == null)
            {
                return new Exceptional(new Error_AttributeIsNotDefined(OldName));
            }

            return myGraphDBType.RenameBackwardedge(Attribute, NewName, myDBContext.DBTypeManager);

        }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override IEnumerable<Vertex> CreateVertex(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return CreateRenameResult("RENAME BACKWARDEDGE", OldName, NewName, myGraphDBType);
        }

    }

}
