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

/* PandoraFS - DBConstants
 * (c) sones Team, 2009
 * 
 * Adds constants to the PandoraFS
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;


#endregion

namespace sones.GraphDB
{

    /// <summary>
    /// Here should all global database constants be declared and assigned
    /// </summary>
    public static class DBConstants
    {
        public static String ASTERISKSYMBOL                     = "*";

        public static String DBTYPESTREAM                       = "DBTYPESTREAM";
        public static String DbGraphType                        = "DBType";
        public static String DBTypeDefinition                   = "TypeDefinition";

        public static String DBObjectsLocation                  = "Objects";
        public static String DBAttributesLocation               = "Attributes";
        public static String DBSettingsLocation                 = "Settings";
        public static String DBIndicesLocation                  = "Indices";
        public static String DBChildTypesLocation               = "ChildTypes";

        public static String DBTypeLocations                    = ".DBTypeLocations";

        public static String DBObjectFloatSeparator             = ",";
        public static String DBObjectDateSeparator              = ".";
        public static String DBObjectBooleanTrue                = "TRUE";
        public static String DBObjectBooleanFalse               = "FALSE";

        //Compare
        public const String Comperator_Equals                   = "==";
        public const String Comperator_SmallerEquals            = "<=";
        public const String Comperator_GreaterEquals            = ">=";
        public const String Comperator_Smaller                  = "<";
        public const String Comperator_Greater                  = ">";
        public const String Comperator_Contains                 = "CONTAINS";
        public const String Comperator_Like                     = "LIKE";

        public const uint   DefaultReferenceResolutionDepth     = 0;

        public const String DBOBJECTSTREAM                      = "DBOBJECTSTREAM";
        public const String DBBACKWARDEDGESTREAM                = "DBBACKWARDEDGESTREAM";
        public const String UNDEFATTRIBUTESSTREAM               = "UNDEFINEDATTRIBUTESSTREAM";
        public const String DBINDEXSTREAM                       = "DBINDEXSTREAM";

        //public const String GUID_INDEX                          = "AUTOINDEX_GUID";
        public const String DEFAULTINDEX                        = "DEFAULTINDEX";
        public const String UNIQUEATTRIBUTESINDEX               = "UNIQUEATTRIBUTESINDEX";
        public const String InheritatedIdx                      = "_InheritatedIdx";


        /// <summary>
        /// The base of all database types
        /// </summary>
        public const String DBBaseObject                        = "DBBaseObject";

        // Build-in basic database types
        public const String DBInteger                           = "Integer";
        public const String DBInt32                             = "Int32";
        public const String DBUnsignedInteger                   = "UnsignedInteger";
        public const String DBString                            = "String";
        public const String DBDouble                            = "Double";
        public const String DBDateTime                          = "DateTime";
        public const String DBBoolean                           = "Boolean";

        /// <summary>
        /// The base of all user-defined database types
        /// </summary>
        public const String DBObject                            = "DBObject";
        public const String DBBackwardEdge                      = "DBBackwardEdge";
        public const String DBType                              = "DBType";
        public const String DBTypeAttribute                     = "DBTypeAttribute";


        // Pre- and Postfix of the LIST-types
        public const String LIST_PREFIX                         = "LIST<";
        public const String LIST_POSTFIX                        = ">";

        public const String SET_PREFIX                          = "SET<";
        public const String SET_POSTFIX                         = ">";

        public const String SET                                 = "SET";
        public const String SETOF                               = "SETOF";
        public const String LIST                                = "LIST";
        public const String LISTOF                              = "LISTOF";
        public const String SETOFUUIDS                          = "SETOFUUIDS";
        public const String IndexKeyPrefix                      = "Idx";
        public const String IndexKeySeperator                   = "_";
        public const String EdgeTraversalDelimiterSymbol        = ".";
        public const String EdgeInformationDelimiterSymbol      = "~>";
        public const String BracketLeft                         = "bracketLeft";
        public const String BracketRight                        = "bracketRight";
        

        public const String TRANSACTION_DISTRIBUTED             = "DISTRIBUTED";
        public const String TRANSACTION_LONGRUNNING             = "LONG-RUNNING";
        public const String TRANSACTION_ISOLATION               = "ISOLATION";
        public const String TRANSACTION_NAME                    = "NAME";
        public const String TRANSACTION_TIMESTAMP               = "TIMESTAMP";
        public const String TRANSACTION_COMMIT                  = "COMMIT";
        public const String TRANSACTION_ROLLBACK                = "ROLLBACK";
        public const String TRANSACTION_COMROLLASYNC            = "ASYNC";

        public const String GraphDBType                         = "GraphDBType";
        public const String DBObjectScheme                      = ".database.objectscheme";

        public const int    UpperUUIDLimitForBaseTypes          = 100;

        public const Int64  DefaultNodeWeight                   = 0;
        public const Int64  DefaultEdgeWeight                   = 0;

        public const String SettingScopeAttribute               = "SCOPE";
        public const String SettingAttributesAttribute          = "ATTRIBUTES";
        public const String SettingAttributesAttributeTYPE      = "ATTRIBUTE";
        
        public const Boolean RunMT                              = false;
        public const Boolean UseThreadedSelect                  = true;

    }

}
