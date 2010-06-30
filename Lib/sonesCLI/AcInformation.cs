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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.CLIrony.Compiler.Lalr;

namespace sones.Lib.CLI
{

    public class AcInformation
    {

        #region Data

        private List<String>    _AcLines;
        private ParserReturn    _AcMetadata;
        private String          _ToBeAced;
        private int             _IndexOfLastDelimiter;

        #endregion

        #region Getter

        public List<String>     AcLines                 { get { return _AcLines; } }
        public ParserReturn     AcMetadata              { get { return _AcMetadata; } }
        public String           ToBeAced                { get { return _ToBeAced; } }
        public int              IndexOfLastDelimiter    { get { return _IndexOfLastDelimiter; } }

        #endregion

        #region Constructor

        public AcInformation(List<String> AcLines, ParserReturn AcMetadata, String ToBeAced, int IndexOfLastDelimiter)
        {
            _AcLines                = AcLines;
            _AcMetadata             = AcMetadata;
            _ToBeAced               = ToBeAced;
            _IndexOfLastDelimiter   = IndexOfLastDelimiter;
        }

        #endregion

    }

}
