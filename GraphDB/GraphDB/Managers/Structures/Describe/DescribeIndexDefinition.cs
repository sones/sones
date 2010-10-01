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

/*
 * DescribeIndexDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.GraphDB.Functions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Indices;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Result;
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDB.Managers.Structures.Describe
{
    public class DescribeIndexDefinition : ADescribeDefinition
    {
        #region Data

        private String _IndexEdition;
        private String _TypeName;
        private String _IndexName;

        #endregion

        #region Ctor

        public DescribeIndexDefinition() { }

        public DescribeIndexDefinition(String myTypeName, string myIndexName, String myIndexEdition)
        {
            _TypeName = myTypeName;
            _IndexName = myIndexName;
            _IndexEdition = myIndexEdition;
        }

        #endregion

        #region ADescribeDefinition

        public override Exceptional<IEnumerable<Vertex>> GetResult(DBContext myDBContext)
        {

            if (!String.IsNullOrEmpty(_TypeName))
            {

                #region Specific index

                var type = myDBContext.DBTypeManager.GetTypeByName(_TypeName);
                if (type == null)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_TypeDoesNotExist(_TypeName));
                }

                if (String.IsNullOrEmpty(_IndexEdition))
                {
                    _IndexEdition = DBConstants.DEFAULTINDEX;
                }
                var attrIndex = type.GetAttributeIndex(_IndexName, _IndexEdition);

                if (attrIndex != null)
                {
                    return new Exceptional<IEnumerable<Vertex>>(new List<Vertex>(){(GenerateOutput(attrIndex, _IndexName))});
                }
                else
                {
                    return new Exceptional<IEnumerable<Vertex>>(new Error_IndexDoesNotExist(_IndexName, _IndexEdition));
                }

                #endregion

            }
            else
            {

                #region All indices

                var resultingReadouts = new List<Vertex>();

                foreach (var type in myDBContext.DBTypeManager.GetAllTypes(false))
                {
                    if (type.IsUserDefined)
                    {
                        foreach (var index in type.GetAllAttributeIndices())
                        {
                            resultingReadouts.Add(GenerateOutput(index, index.IndexName));
                        }
                    }
                }

                return new Exceptional<IEnumerable<Vertex>>(resultingReadouts);

                #endregion

            }
        }
        
        #endregion

        #region Output

        /// <summary>
        /// generate an output for an index
        /// </summary>
        /// <param name="myIndex">the index</param>
        /// <param name="myName">the index name</param>
        /// <returns>list of readouts which contain the index information</returns>
        private Vertex GenerateOutput(AAttributeIndex myIndex, String myName)
        {

            var _Index = new Dictionary<String, Object>();

            _Index.Add("Name",                   myName);
            _Index.Add("Edition",                myIndex.IndexEdition);
            _Index.Add("IndexType",              myIndex.IndexType);
            _Index.Add("IsUuidIndex",            myIndex is UUIDIndex);
            _Index.Add("IsUniqueAttributeIndex", myIndex.IsUniqueAttributeIndex);

            return new Vertex(_Index);
            
        }

        #endregion

    }
}
