/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.SonesGQL.DBExport
{
    public class InvalidDumpLocationException : ASonesQLGraphDBExportException
    {
        private String Location;
        private String Info;
        private String ExpectedFile;
        private String ExpectedHttp;

		/// <summary>
		/// Initializes a new instance of the InvalidDumpLocationException class.
		/// </summary>
		/// <param name="myLocation"></param>
		/// <param name="myExpectedFile"></param>
		/// <param name="myExpectedHttp"></param>
		/// <param name="myInfo"></param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public InvalidDumpLocationException(String myLocation, String myExpectedFile, String myExpectedHttp, String myInfo, Exception innerException = null)
			: base(innerException)
        {
            Location = myLocation;
            Info = myInfo;
            ExpectedFile = myExpectedFile;
            ExpectedHttp = myExpectedHttp;

            _msg = String.Format("Invalid dump destination: {0}. Expected {1} or {2}\n\n{3}", Location, myExpectedFile, myExpectedHttp, Info);
        }

    }
}
