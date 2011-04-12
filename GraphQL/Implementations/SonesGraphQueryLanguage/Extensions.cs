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