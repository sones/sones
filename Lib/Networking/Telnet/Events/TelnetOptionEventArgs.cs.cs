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


/* PandoraWebDAV
 * (c) Stefan Licht, 2009
 * 
 * This class holds the TelnetOptionEventArgs
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Networking.Telnet.Events
{
    public class TelnetOptionEventArgs
    {
        public TelnetSymbol TelnetSymbol { get; set; }
        /// <summary>
        /// For Do(Dont)Option request a True will result in a WillOption and False in a WontOption
        /// For WillOption request a True will result in a WillOption and False in a WontOption if the Accpted differs from the stored remote option settings
        /// </summary>
        public Boolean Accepted { get; set; }
    }
}
 
