using System;
using System.Text;
using Irony.Parsing;

namespace sones.GraphQL
{
    public static class IronyExtensions
    {

        #region Irony: ParseTreeNode, SymbolTerminal

        public static String ToStringChildnodesIncluded(this ParseTreeNode parseTreeNode)
        {

            var retString = new StringBuilder();
            if (parseTreeNode != null)
            {
                retString.Append(parseTreeNode.ToString() + ":");
                if (parseTreeNode.HasChildNodes())
                {
                    foreach (var child in parseTreeNode.ChildNodes)
                    {
                        retString.Append(child.ToString() + ", ");
                    }
                }
            }

            return retString.ToString();

        }

        public static string ToUpperString(this KeyTerm symbolTerminal)
        {
            return symbolTerminal.ToString().ToUpper();
        }

        public static string ToString(this KeyTerm symbolTerminal)
        {
            return symbolTerminal.ToString().ToUpper();
        }

        public static Boolean HasChildNodes(this ParseTreeNode treeNode)
        {
            return treeNode.ChildNodes.Count > 0;
        }

        #endregion

    }
}
