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

        #endregion

        #region grammar

        public const String EdgeInformationDelimiterSymbol      = "~>";
        public const String EdgeTraversalDelimiterSymbol        = ".";
        public const String SETOF                               = "SETOF";
        public const String LISTOF                              = "LISTOF";
        public const String SETOFUUIDS                          = "SETOFUUIDS";

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

        #endregion
    }
}
