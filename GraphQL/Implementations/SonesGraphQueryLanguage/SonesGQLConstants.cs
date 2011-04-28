using System;

namespace sones.GraphQL
{
    public static class SonesGQLConstants
    {
        #region misc

        /// <summary>
        /// The application settings location
        /// </summary>
        public static String ApplicationSettingsLocation = "sonesGQLSettings.xml";
        
        /// <summary>
        /// The name of the base vertex type name which carries attributes like creation date, modification date, UUID, comment...
        /// </summary>
        public static String BaseVertexTypeName = "Vertex";

        /// <summary>
        /// The name of the sones query language
        /// </summary>
        public static String GQL = "GQL";

        #endregion

        #region grammar

        public const String EdgeInformationDelimiterSymbol      = "~>";
        public const String EdgeTraversalDelimiterSymbol        = ".";
        public const String SETOF                               = "SETOF";
        public const String LISTOF                              = "LISTOF";
        public const String SETOFUUIDS                          = "SETOFUUIDS";
        public const String SingleType                          = "SingleType";

        public const String TRANSACTION_DISTRIBUTED             = "DISTRIBUTED";
        public const String TRANSACTION_LONGRUNNING             = "LONG-RUNNING";
        public const String TRANSACTION_ISOLATION               = "ISOLATION";
        public const String TRANSACTION_NAME                    = "NAME";
        public const String TRANSACTION_TIMESTAMP               = "TIMESTAMP";
        public const String TRANSACTION_COMMIT                  = "COMMIT";
        public const String TRANSACTION_ROLLBACK                = "ROLLBACK";
        public const String TRANSACTION_COMROLLASYNC            = "ASYNC";

        public const String BracketLeft                         = "bracketLeft";
        public const String BracketRight                        = "bracketRight";
        public const String GraphDBType                         = "GraphDBType";


        public const String INCOMINGEDGE                         = "INCOMINGEDGE";
        public const String INCOMINGEDGES                        = "INCOMINGEDGES";


        #endregion

        #region select

        public static String ASTERISKSYMBOL                     = "*";
        public static String MINUSSYMBOL                        = "-";
        public static String RHOMBSYMBOL                        = "#";
        public const String Comperator_Smaller                  = "<";
        public const String Comperator_Greater                  = ">";

        #endregion
    }
}
