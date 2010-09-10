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
 * FastReflectionSerializerExceptions
 * (c) Daniel Kirstenpfad, 2009
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 * 
 * */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace sones.Lib.Serializer
{

    /// <summary>
    /// This is a class for all FastReflectionSerializerExceptions
    /// </summary>

    #region FastReflectionSerializerExceptions Superclass

    public class FastReflectionSerializerExceptions : ApplicationException
    {
        public FastReflectionSerializerExceptions(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

    #region FastReflectionSerializerExceptions

    public class FastReflectionSerializerExceptions_DeSerializeHashNotValid : FastReflectionSerializerExceptions
    {
        public FastReflectionSerializerExceptions_DeSerializeHashNotValid(String message)
            : base(message)
        {
            // do nothing extra
        }
    }

    #endregion

}
