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
    public sealed class IndexDefinition
    {

        #region Properties

        public String IndexName
        {
            get;
            private set;
        }

        public String Edition
        {
            get;
            private set;
        }

        public String IndexType
        {
            get;
            private set;
        }

        public List<IndexAttributeDefinition> IndexAttributeDefinitions
        {
            get;
            private set;
        }

        #endregion

        public IndexDefinition(string myIndexName, string myEdition, string myIndexType, List<IndexAttributeDefinition> myIndexAttributeDefinitions)
        {
            IndexName = myIndexName;
            Edition = myEdition;
            IndexType = myIndexType;
            IndexAttributeDefinitions = myIndexAttributeDefinitions;
        }


    }
}
