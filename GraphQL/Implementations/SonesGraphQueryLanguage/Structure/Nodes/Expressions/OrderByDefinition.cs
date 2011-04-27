using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Nodes.Misc;
using sones.GraphQL.GQL.Structure.Helper.Enums;

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    public class OrderByAttributeDefinition
    {
        public IDChainDefinition IDChainDefinition { get; private set; }

        /// <summary>
        /// in case of an as, this would be the as-string.
        /// if there has been no as-option, the name of the last attribute of the IDNode is used
        /// </summary>
        public String AsOrderByString { get; set; }

        public OrderByAttributeDefinition(IDChainDefinition myIDChainDefinition, String myAsOrderByString)
        {
            IDChainDefinition = myIDChainDefinition;
            AsOrderByString = myAsOrderByString;
        }
    }

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
