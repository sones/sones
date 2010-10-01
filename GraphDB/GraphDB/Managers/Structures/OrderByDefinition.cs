/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.DataStructures;
using sones.GraphDB.ObjectManagement;


namespace sones.GraphDB.Managers.Structures
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
