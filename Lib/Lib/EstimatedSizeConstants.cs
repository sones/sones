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

#if __MonoCS__
        public const UInt64 TypeAttribute                           = 2500;
        public const UInt64 UInt64                                  = 80;
        public const UInt64 UInt32                                  = 40;
        public const UInt64 Char                                    = 20;
        public const UInt64 DateTime                                = 80;
        public const UInt64 Boolean                                 = 10;
        public const UInt64 Int32                                   = 40;
        public const UInt64 WeakReference                           = 320;
        public const UInt64 EnumByte                                = 10;
        public const UInt64 EnumUInt64                              = 80;
        public const UInt64 SortedDictionary                        = 1200;
        public const UInt64 Dictionary                              = 880;
        public const UInt64 List                                    = 400;
        public const UInt64 KeyValuePair                            = 240;
        public const UInt64 Tuple                                   = 320;
        public const UInt64 HashSet                                 = 640;
        public const UInt64 AFSObjectOntologyObject                 = 40000;
        public const UInt64 ObjectLocator                           = 30000;
        public const UInt64 ExtendedPosition                        = 1000;
        public const UInt64 Byte                                    = 10;
        public const UInt64 EstimatedObjectStreamNameLength         = 100;
        public const UInt64 EstimatedKeyInDictionarySize            = 1000;
        public const UInt64 DictionaryValueHistory                  = 720;
        public const UInt64 BigDictionary                           = 1800;
        public const UInt64 LinkedList                              = 60; //TO be verified
#else
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
        public const UInt64 LinkedList                              = 60; //TO be verified
#endif

        public const UInt64 EstimatedValueInDictionarySize          = 100;
        public const UInt64 UInt16                                  = 2;
        public const UInt64 StringBase                              = 26;

        
        



        /// <summary>
        /// UInt16 + UInt32(typeCode) + ClassDefaultSize
        /// </summary>
        public const UInt64 AttributeUUID           = 36;

        public static UInt64 CalcStringSize(String aString)
        {
            if (aString != null)
            {
                return StringBase + (EstimatedSizeConstants.Char * Convert.ToUInt64(aString.Length));
            }
            else
            {
                return 0UL;
            }
        }

        public static UInt64 CalcByteArray(Byte[] myByteArray)
        {
            if (myByteArray != null)
            {
                return (UInt64)myByteArray.Length * EstimatedSizeConstants.Byte + EstimatedSizeConstants.ClassDefaultSize;
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
