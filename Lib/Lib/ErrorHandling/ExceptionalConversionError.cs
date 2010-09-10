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


#region Usings

using System;

#endregion

namespace sones.Lib.ErrorHandling
{

    /// <summary>
    /// This class carries information of errors.
    /// </summary>

    public class ExceptionalConversionError : GeneralError
    {
        String info = String.Empty;


        public ExceptionalConversionError()
        {
            info = String.Format("Converting Exceptional failed!");
        }

        public ExceptionalConversionError(String myDestinationType)
        {
            info = String.Format("Converting Exceptional to Exceptional<{1}> failed!", myDestinationType);
        }

        public ExceptionalConversionError(String mySourceType, String myDestinationType)
        {
            info = String.Format("Converting Exceptional<{0}> to Exceptional<{1}> failed!", mySourceType, myDestinationType);
        }

        public override string ToString()
        {
            return info;
        }

    }

}