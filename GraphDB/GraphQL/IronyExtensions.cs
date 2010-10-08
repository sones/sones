
#region Usings

using System;
using System.Text;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL
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

        public static string ToUpperString(this SymbolTerminal symbolTerminal)
        {
            return symbolTerminal.ToString().ToUpper();
        }

        public static string ToString(this SymbolTerminal symbolTerminal)
        {
            return symbolTerminal.ToString().ToUpper();
        }

        #endregion

    }
}
