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
 * IGraphDBImport
 * Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Net.Mime;

using sones.GraphDB.Result;
using sones.Lib.ErrorHandling;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphIO
{

    /// <summary>
    /// An interface to import data into the graph database
    /// </summary>

    public interface IGraphDBImport
    {

        ContentType         ImportContentType           { get; }

        QueryResult         ParseQueryResult            (String myInput);
        Vertex              ParseVertex                 (String myInput);

        UnspecifiedWarning  GenerateUnspecifiedWarning  (Object myWarningXElement);
        UnspecifiedError    GenerateUnspecifiedError    (Object myErrorXElement);

    }

}
