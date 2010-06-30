/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/



#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Indices;
using sones.GraphDB.QueryLanguage.Result;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure.Helper
{
    /// <summary>
    /// this class generate an output for the describe idx(indices) commands
    /// </summary>
    public class DescrIdxOutput
    {

        #region constructors

        public DescrIdxOutput()
        { }

        #endregion

        #region Output

        /// <summary>
        /// generate an output for an index
        /// </summary>
        /// <param name="myIndex">the index</param>
        /// <param name="myName">the index name</param>
        /// <returns>list of readouts which contain the index information</returns>
        public IEnumerable<DBObjectReadout> GenerateOutput(AttributeIndex myIndex, String myName)
        {

            var _Index = new Dictionary<String, Object>();

            _Index.Add("Name",                   myName);
            _Index.Add("Edition",                myIndex.IndexEdition);
            _Index.Add("IndexType",              myIndex.IndexType);
            _Index.Add("IsUuidIndex",            myIndex.IsUuidIndex);
            _Index.Add("IsUniqueAttributeIndex", myIndex.IsUniqueAttributeIndex);            
            
            return new List<DBObjectReadout>() { new DBObjectReadout(_Index) };

        }

        #endregion
    }
}
