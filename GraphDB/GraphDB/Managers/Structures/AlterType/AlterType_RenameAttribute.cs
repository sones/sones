/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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

    public class AlterType_RenameAttribute : AAlterTypeCommand
    {

        public String OldName { get; set; }
        public String NewName { get; set; }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.RenameAttribute; }
        }

        /// <summary>
        /// Executes the renaming of the attribute of a given type.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="graphDBType"></param>
        /// <returns></returns>
        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return myDBContext.DBTypeManager.RenameAttributeOfType(myGraphDBType, OldName, NewName);
        }

        public override IEnumerable<Vertex> CreateVertex(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return CreateRenameResult("RENAME ATTRIBUTE", OldName, NewName, myGraphDBType);
        }

    }

}
