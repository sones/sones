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

/* Graph CLI
 * (c) Henning Rauch, 2009
 * 
 * Datatype for GraphCLI options
 * 
 * Lead programmer:
 *      Henning Rauch
 * 
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace sones.GraphDS.Connectors.CLI
{

    public class AbstractCLIOption
    {

        #region Data

        private String _Option = "";
        private List<String> _Parameters = new List<string>();

        #endregion

        #region Constructors

        public AbstractCLIOption(String _OptionName)
        {
            _Option = _OptionName;
        }

        #endregion

        #region Public methods

        public String Option
        {
            get
            {
                return _Option;
            }
        }

        public List<String> Parameters
        {
            get
            {
                return _Parameters;
            }
        }

        public void AddParameter(string _Parameter)
        {
            _Parameters.Add(_Parameter);
        }

        public int Pos { get; set; }

        public int EndPos { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

        #endregion

    }

}
