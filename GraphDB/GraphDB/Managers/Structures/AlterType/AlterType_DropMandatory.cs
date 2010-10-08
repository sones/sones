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

    public class AlterType_DropMandatory : AAlterTypeCommand
    {

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.DropMandatory; }
        }

        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return myGraphDBType.DropMandatoryAttributes(myDBContext.DBTypeManager);
        }

    }

}
