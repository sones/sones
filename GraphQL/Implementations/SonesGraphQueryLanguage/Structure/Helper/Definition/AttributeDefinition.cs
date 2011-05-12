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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.GQL.Structure.Helper.Definition
{
    public sealed class AttributeDefinition
    {
        #region Properties

        public String AttributeName { get; private set; }
        public DBTypeOfAttributeDefinition AttributeType { get; private set; }
        public object DefaultValue { get; private set; }

        #endregion

        #region Ctor

        public AttributeDefinition(DBTypeOfAttributeDefinition attributeType, String attributeName, Object defaultValue = null)
        {

            AttributeType = attributeType;
            AttributeName = attributeName;
            DefaultValue = defaultValue;

        }

        #endregion
    }
}
