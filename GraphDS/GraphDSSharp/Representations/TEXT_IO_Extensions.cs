/* 
 * TEXT_IO_Extensions
 * Achim 'ahzf' Friedland, 2009-2010
 */

#region Usings

using System;
using System.Text;

#endregion

namespace sones.GraphDB.Structures.Result
{

    /// <summary>
    /// Extension methods to transform a QueryResult and a DBObjectReadout into a text/plain representation
    /// </summary>

    public static class TEXT_IO_Extensions
    {

        #region ToTEXT(this myQueryResult)

        public static String ToTEXT(this QueryResult myQueryResult)
        {
            return (new TEXT_IO().Export(myQueryResult) as StringBuilder).ToString();
        }

        #endregion

        #region ToTEXT(this myQueryResult, myStringBuilder)

        public static String ToTEXT(this QueryResult myQueryResult, StringBuilder myStringBuilder)
        {
            return new TEXT_IO().Export(myQueryResult, myStringBuilder).ToString();
        }

        #endregion


        #region ToTEXT(this myDBObjectReadout)

        private static String ToTEXT(this DBObjectReadout myDBObjectReadout)
        {
            return new TEXT_IO().Export(myDBObjectReadout) as String;
        }

        #endregion

        #region ToTEXT(this myDBObjectReadout, myStringBuilder)

        private static String ToTEXT(this DBObjectReadout myDBObjectReadout, StringBuilder myStringBuilder)
        {
            return (new TEXT_IO().Export(myDBObjectReadout, myStringBuilder) as StringBuilder).ToString();
        }

        #endregion

    }

}
