/* GraphFS - DBConstants
 * (c) sones Team, 2009
 * 
 * Adds constants to the GraphFS
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
        public static String MINUSSYMBOL                        = "-";
        public static String RHOMBSYMBOL                        = "#";

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
        public const String DBObjectRevisionID                  = "ObjectRevisionID";

        /// <summary>
        /// The base of all user-defined database types
        /// </summary>
        public const String DBObject                            = "DBObject";
        public const String DBVertex                            = "DBVertex";
        public const String DBBackwardEdge                      = "DBBackwardEdge";
        public const String DBType                              = "DBType";
        public const String DBTypeAttribute                     = "DBTypeAttribute";

        /// <summary>
        /// The base of all user-defined database vertices
        /// </summary>
        public const String DBVertexName                        = "Vertex";
        public const Int32  DBVertexID                          = 50;

        /// <summary>
        /// The base of all user-defined database edges - request from Achim
        /// </summary>
        public const String DBEdgeName                          = "DBEdge";
        public const Int32  DBEdgeID                            = 60;

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

        public const UInt16 DefaultTypeAttributeIDStart         = 100;
        public const UInt16 DefaultBackwardEdgeIDStart          = 500;

        
        
        #if __MonoCS__
        public const Boolean RunMT                              = false;  
        #else
        public const Boolean RunMT                              = true;
        #endif

        public const Boolean UseThreadedSelect                  = true;

    }

}
