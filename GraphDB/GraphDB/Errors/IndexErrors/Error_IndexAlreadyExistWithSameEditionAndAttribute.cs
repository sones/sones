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
using sones.GraphDB.Indices;
using sones.Lib;

namespace sones.GraphDB.Errors
{
    public class Error_IndexAlreadyExistWithSameEditionAndAttribute : GraphDBIndexError
    {
        public String ExistingIndexName { get; private set; }
        public String IndexEdition { get; private set; }
        public IndexKeyDefinition IndexKeyDefinition { get; private set; }
        private String _IndexAttributes;

        public Error_IndexAlreadyExistWithSameEditionAndAttribute(DBContext myDBContext, String myExistingIndexName , IndexKeyDefinition myIndexKeyDefinition, String myIndexEdition)
        {
            IndexEdition = myIndexEdition;
            IndexKeyDefinition = myIndexKeyDefinition;
            ExistingIndexName = myExistingIndexName;

            try
            {
                if (myDBContext != null)
                {
                    _IndexAttributes = IndexKeyDefinition.IndexKeyAttributeUUIDs.ToAggregatedString(a => myDBContext.DBTypeManager.GetTypeAttributeByAttributeUUID(a).Name);
                }
                else
                {
                    _IndexAttributes = IndexKeyDefinition.IndexKeyAttributeUUIDs.ToAggregatedString(a => a.ToString());
                }
            }
            catch { }

        }

        public override string ToString()
        {
            return String.Format("There is already an index \"{0}\" with the same edition \"{1}\" and attribute(s) \"{2}\"!", ExistingIndexName, IndexEdition, _IndexAttributes);
        }
    }
}
