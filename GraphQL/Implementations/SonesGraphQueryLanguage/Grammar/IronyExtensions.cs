using System;
using Irony.Parsing;
using System.Text;

namespace sones.GraphQL
{

    /// <summary>
    /// A static class that contains extensions for IRONY
    /// </summary>
    public static class IronyExtensions
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

    }

}