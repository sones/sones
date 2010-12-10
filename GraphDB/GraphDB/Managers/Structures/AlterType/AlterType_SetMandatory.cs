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

    /// <summary>
    /// Set the mandatory flag for an attribute
    /// </summary>
    public class AlterType_SetMandatory : AAlterTypeCommand
    {

        /// <summary>
        /// List of mandatory attributes of the given vertex
        /// </summary>
        public List<String> MandatoryAttributes { get; set; }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.Mandatory; }
        }

        /// <summary>
        /// Set the mandatory flag
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return myGraphDBType.ChangeMandatoryAttributes(MandatoryAttributes, myDBContext.DBTypeManager);
        }

    }

}
