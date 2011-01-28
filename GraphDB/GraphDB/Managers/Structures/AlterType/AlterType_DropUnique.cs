/*
 * AlterType_DropUnique
 * (c) Stefan Licht, 2010
 */

#region Usings

using sones.GraphDB.Managers.AlterType;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.TypeManagement;

using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Managers.AlterType
{

    /// <summary>
    /// Drops the unique flag of an attribute
    /// </summary>
    public class AlterType_DropUnique : AAlterTypeCommand
    {

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.DropUnqiue; }
        }

        /// <summary>
        /// Drops the unique flag of an attribute
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return myGraphDBType.DropUniqueAttributes(myDBContext);
        }

    }

}
