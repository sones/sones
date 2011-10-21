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
using System.Globalization;

namespace sones.Plugins.SonesGQL.XMLBulkImport
{
    internal static class HumanSizeParser
    {
        public static long ToBytes(String myHumanSize)
        {
            myHumanSize = myHumanSize.Trim();
            var numberString = new string(myHumanSize.TakeWhile(_ => Char.IsDigit(_) || Char.IsPunctuation(_)).ToArray());
            var unit = myHumanSize.Remove(0, numberString.Length).Trim();

            var number = double.Parse(numberString, CultureInfo.GetCultureInfo("en-us"));

            switch (unit)
            {
                case "":
                case "B":
                    return (long)number;
                case "KB":
                case "kB":
                    return (long)(number * 1000);
                case "KiB":
                    return (long)(number * 1024);
                case "MB":
                    return (long)(number * 1000000);
                case "MiB":
                    return (long)(number * 1048576);
                case "GB":
                    return (long)(number * 1000000000);
                case "GiB":
                    return (long)(number * 1073741824);
                case "TB":
                    return (long)(number * 1000000000000);
                case "TiB":
                    return (long)(number * 1099511627776);
                case "PB":
                    return (long)(number * 1000000000000000);
                case "PiB":
                    return (long)(number * 1125899906842624);

                default:
                    throw new ArgumentException("Can not read value for max memory.");
            }
            

        }
    }
}
