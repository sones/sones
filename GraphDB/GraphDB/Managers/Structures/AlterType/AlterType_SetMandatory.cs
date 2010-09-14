/*
 * AlterType_SetMandatory
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

    public class AlterType_SetMandatory : AAlterTypeCommand
    {

        public List<String> MandatoryAttributes { get; set; }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.Mandatory; }
        }

        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return myGraphDBType.ChangeMandatoryAttributes(MandatoryAttributes, myDBContext.DBTypeManager);
        }

    }

}
