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

/* Lib - Estimated size constants in bytes
 * (c) sones Team, 2009
 * 
 * 
 * Lead programmer:
 *      Henning Rauch
 * 
 * */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;
using sones.Lib.DataStructures.UUID;


#endregion

namespace sones.Lib
{

    public static class EstimatedSizeConstants
    {
        /// <summary>
        /// The default size of a class container
        /// </summary>
        public const UInt64 ClassDefaultSize        = 30;

        /// <summary>
        /// Value for objects which are not yet estimated
        /// </summary>
        public const UInt64 UndefinedObjectSize                     = 1000;

        public const UInt64 TypeAttribute                           = 250;
        public const UInt64 UInt64                                  = 8;
        public const UInt64 UInt32                                  = 4;
        public const UInt64 Char                                    = 2;
        public const UInt64 DateTime                                = 8;
        public const UInt64 Boolean                                 = 1;
        public const UInt64 Int32                                   = 4;
        public const UInt64 WeakReference                           = 32;
        public const UInt64 EnumByte                                = 1;
        public const UInt64 EnumUInt64                              = 8;
        public const UInt64 SortedDictionary                        = 120;
        public const UInt64 Dictionary                              = 88;
        public const UInt64 List                                    = 40;
        public const UInt64 KeyValuePair                            = 24;
        public const UInt64 Tuple                                   = 32;
        public const UInt64 HashSet                                 = 64;
        public const UInt64 AFSObjectOntologyObject                 = 4000;
        public const UInt64 ObjectLocator                           = 3000;
        public const UInt64 ExtendedPosition                        = 100;
        public const UInt64 Byte                                    = 1;
        public const UInt64 EstimatedObjectStreamNameLength         = 10;
        public const UInt64 EstimatedKeyInDictionarySize            = 100;
        public const UInt64 DictionaryValueHistory                  = 72;
        public const UInt64 BigDictionary                           = 180;
        public const UInt64 UInt16                                  = 2;



        /// <summary>
        /// UInt16 + UInt32(typeCode) + ClassDefaultSize
        /// </summary>
        public const UInt64 AttributeUUID           = 36;

        public static UInt64 CalcStringSize(String aString)
        {
            if (aString != null)
            {
                return (EstimatedSizeConstants.Char * Convert.ToUInt64(aString.Length));
            }
            else
            {
                return 0UL;
            }
        }

        public static UInt64 CalcUUIDSize(UUID aUUID)
        {
            if (aUUID != null)
            {
                return aUUID.Length + EstimatedSizeConstants.UInt32 + EstimatedSizeConstants.ClassDefaultSize;            
            }
            else
            {
                return 0UL;
            }
        }

        public static ulong GetStandardSizes()
        {
            return EstimatedSizeConstants.ClassDefaultSize + EstimatedSizeConstants.UInt64;
        }

        //Sample:
//#if __MonoCS__
//        public const Boolean RunMT                              = false;  
//#else
//        public const Boolean RunMT                              = true;
//#endif


        
    }

}
