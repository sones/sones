/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


/* <id name="sones GraphDB – AttributeUpdateOrAssign" />
 * <copyright file="AttributeUpdateOrAssign.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.Enums;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{

    public class AttributeUpdateOrAssign
    {

        #region Data

        Object _Value = null;
        TypesOfUpdate _TypeOfUpdate;

        #endregion

        public AttributeUpdateOrAssign(TypesOfUpdate TypeOfUpdate, Object Value)
        {
            _TypeOfUpdate = TypeOfUpdate;
            _Value = Value;
        }

        public Object Value { get { return _Value; } }

        public TypesOfUpdate TypeOfUpdate { get { return _TypeOfUpdate; } }

        public Boolean IsUndefinedAttribute { get; set; }

        public override string ToString()
        {
            return String.Concat("[", _TypeOfUpdate.ToString(), "] ", _Value.ToString(), "{", _Value.GetType().Name, "}");
        }

    }
}
