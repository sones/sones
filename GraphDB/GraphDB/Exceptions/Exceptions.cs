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

/* <id name="PandoraDB – Exceptions" />
 * <copyright file="PandoraExceptions.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Daniel Kirstenpfad</developer>
 * <summary>This class carries the definitions of exceptions.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.Lib;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Exceptions
{

    #region Generic Standard PandoraDB Exception

    public class GraphDBException : ApplicationException
    {
        public IEnumerable<IError> GraphDBErrors { get; set; }

        public GraphDBException(IError myGraphDBError)
            : base(myGraphDBError.ToString())
        {
            GraphDBErrors = new List<IError>() { myGraphDBError };
        }

        public GraphDBException(IError myGraphDBError, Exception innerException)
            : base(myGraphDBError.ToString(), innerException)
        {
            GraphDBErrors = new List<IError>() { myGraphDBError };
        }

        public GraphDBException(IEnumerable<IError> myGraphDBErrors)
            : base(myGraphDBErrors.ToAggregatedString().ToString())
        {
            GraphDBErrors = myGraphDBErrors;
        }

    }

    #endregion

}
