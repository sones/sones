using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.DataStructures;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;

namespace sones.GraphDB.Managers.Structures
{
    public class OrderByDefinition
    {

        #region Properties

        public SortDirection OrderDirection { get; private set; }
        public List<OrderByAttributeDefinition> OrderByAttributeList { get; private set; }

        #endregion

        #region Ctor

        public OrderByDefinition(SortDirection myOrderDirection, List<OrderByAttributeDefinition> myOrderByAttributeList)
        {
            OrderDirection = myOrderDirection;
            OrderByAttributeList = myOrderByAttributeList;
        }

        #endregion

    }
}
