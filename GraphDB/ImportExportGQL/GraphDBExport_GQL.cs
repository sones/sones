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
 * GraphDBExport_GQL
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.Interfaces;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.ImportExport
{

    /// <summary>
    /// An export implementation for GQL format
    /// </summary>
    public class GraphDBExport_GQL : AGraphDBExport
    {
        public override string ExportFormat
        {
            get { return "GQL"; }
        }

        public override Exceptional Export(DBContext myDBContext, IDumpable myGrammar, IEnumerable<TypeManagement.GraphDBType> myTypes, DumpTypes myDumpType, VerbosityTypes verbosityType = VerbosityTypes.Errors)
        {

            var dumpReadout = new Dictionary<String, Object>();

            if ((myDumpType & DumpTypes.GDDL) == DumpTypes.GDDL)
            {

                var graphDDL = myGrammar.ExportGraphDDL(DumpFormats.GQL, myDBContext, myTypes);

                if (!graphDDL.Success())
                {
                    return new Exceptional(graphDDL);
                }

                //dumpReadout.Add("GDDL", graphDDL.Value);
                var writeResult = Write(DumpTypes.GDDL, graphDDL.Value);
                if (!writeResult.Success())
                {
                    return writeResult;
                }

            }

            if ((myDumpType & DumpTypes.GDML) == DumpTypes.GDML)
            {

                var graphDML = myGrammar.ExportGraphDML(DumpFormats.GQL, myDBContext, myTypes);

                if (!graphDML.Success())
                {
                    return new Exceptional(graphDML);
                }

                //dumpReadout.Add("GDML", _GraphDML.Value);
                var writeResult = Write(DumpTypes.GDML, graphDML.Value);
                if (!writeResult.Success())
                {
                    return writeResult;
                }

            }

            return Exceptional.OK;

        }

    }

}
