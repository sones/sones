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

    public class AlterType_RenameBackwardedge : AAlterTypeCommand
    {

        public String OldName { get; set; }
        public String NewName { get; set; }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.RenameBackwardedge; }
        }

        /// <summary>
        /// Execute the renaming of the backwardedge of a given type.
        /// </summary>
        /// <param name="myDBContext"></param>
        /// <param name="myGraphDBType"></param>
        /// <returns></returns>
        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {

            TypeAttribute Attribute = myGraphDBType.GetTypeAttributeByName(OldName);

            if (Attribute == null)
            {
                return new Exceptional(new Error_AttributeIsNotDefined(OldName));
            }

            return myGraphDBType.RenameBackwardedge(Attribute, NewName, myDBContext.DBTypeManager);

        }

        public override IEnumerable<Vertex> CreateVertex(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return CreateRenameResult("RENAME BACKWARDEDGE", OldName, NewName, myGraphDBType);
        }

    }

}
