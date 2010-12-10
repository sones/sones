/*
 * AlterType_DropMandatory
 * (c) Stefan Licht, 2010
 */

#region Usings

using sones.GraphDB.Structures.Enums;
using sones.GraphDB.TypeManagement;

using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Managers.AlterType
{

    /// <summary>
    /// Removes the mandatory flag of an attribute
    /// </summary>
    public class AlterType_DropMandatory : AAlterTypeCommand
    {
        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.DropMandatory; }
        }

        /// <summary>
        /// Removes of an mandatory flag
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return myGraphDBType.DropMandatoryAttributes(myDBContext.DBTypeManager);
        }

    }

}
