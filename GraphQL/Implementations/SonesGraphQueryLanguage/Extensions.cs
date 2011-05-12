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
using System.Text;
using Irony.Parsing;

namespace sones.GraphQL
{

    /// <summary>
    /// A static class that contains some extensions
    /// </summary>
    public static class Extensions
    {
        #region Irony: ParseTreeNode, KeyTerm

        /// <summary>
        /// Get all child nodes as string
        /// </summary>
        /// <param name="parseTreeNode">The interesting ParseTreeNode</param>
        /// <returns>An accumulated String</returns>
        public static String ToStringChildnodesIncluded(this ParseTreeNode parseTreeNode)
        {

            var retString = new StringBuilder();
            if (parseTreeNode != null)
            {
                retString.Append(parseTreeNode.ToString() + ":");
                if (parseTreeNode.ChildNodes != null && parseTreeNode.ChildNodes.Count > 0)
                {
                    foreach (var child in parseTreeNode.ChildNodes)
                    {
                        retString.Append(child.ToString() + ", ");
                    }
                }
            }

            return retString.ToString();

        }

        /// <summary>
        /// Gets a KeyTerm as an uppercase String
        /// </summary>
        /// <param name="symbolTerminal"></param>
        /// <returns></returns>
        public static string ToUpperString(this KeyTerm symbolTerminal)
        {
            return symbolTerminal.Text.ToUpper();
        }

        #endregion

        #region StringBuilder

        public static void Indent(this StringBuilder myStringBuilder, int myWidth, char myCharacter = ' ')
        {
            myStringBuilder.Append("".PadLeft(myWidth, myCharacter));
        }

        /// <summary>
        /// Removes the last characters of the length of <paramref name="mySuffix"/> without checking them.
        /// </summary>
        /// <param name="myStringBuilder"></param>
        /// <param name="mySuffix"></param>
        public static void RemoveSuffix(this StringBuilder myStringBuilder, String mySuffix)
        {
            if (myStringBuilder.Length > mySuffix.Length)
                myStringBuilder.Remove(myStringBuilder.Length - mySuffix.Length, mySuffix.Length);
        }

        public static void RemoveEnding(this StringBuilder myStringBuilder, Int32 myLength)
        {
            if (myStringBuilder.Length > myLength)
                myStringBuilder.Remove(myStringBuilder.Length - myLength, myLength);
        }


        #endregion
    }

}