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
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The index does not exists
    /// </summary>
    public sealed class IndexDoesNotExistException : AGraphDBIndexException
    {
        #region data        

        public String IndexName { get; private set; }
        public String IndexEdition { get; private set; }

        #endregion

        #region constructor        

        /// <summary>
        /// Create a new IndexDoesNotExistException exception
        /// </summary>
        /// <param name="myIndexName"></param>
        /// <param name="myIndexEdition"></param>
        public IndexDoesNotExistException(String myIndexName, String myIndexEdition)
        {
            IndexName = myIndexName;
            IndexEdition = myIndexEdition;

            if (!String.IsNullOrEmpty(IndexName) && !String.IsNullOrEmpty(IndexEdition))
                _msg = String.Format("The index \"{0}\" with edition \"{1}\" does not exist!", IndexName, IndexEdition);
            if (!String.IsNullOrEmpty(IndexName))
                _msg = String.Format("The index \"{0}\" does not exist!", IndexName);
            else
                _msg = String.Format("The indexedition \"{0}\" does not exist!", IndexEdition);
        }

        #endregion
       
    }
}
