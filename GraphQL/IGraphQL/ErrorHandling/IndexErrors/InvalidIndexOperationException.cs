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

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The index operation is invalid
    /// </summary>
    public sealed class InvalidIndexOperationException : AGraphQLIndexException
    {
        #region data

        public String IndexName { get; private set; }
        public Object IndexKey { get; private set; }
        public Object Operand { get; private set; }
        public String OperationName { get; private set; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new InvalidIndexOperationException exception
        /// </summary>
        /// <param name="myIndexName">The name of the index</param>
        /// <param name="myOperationName">The name of the operation</param>
        public InvalidIndexOperationException(String myIndexName, String myOperationName)
        {
            IndexName = myIndexName;
            OperationName = myOperationName;            
            _msg =  String.Format("A invalid index operation ({0}) on \"{1}\" occurred.", OperationName, IndexName);

        }

        /// <summary>
        /// Creates a new InvalidIndexOperationException exception
        /// </summary>
        /// <param name="myIndexName">The name of the index</param>
        public InvalidIndexOperationException(String myIndexName)
        {
            IndexName = myIndexName;
            _msg =  String.Format("A invalid index operation on \"{0}\" occurred.", IndexName);
        }

        /// <summary>
        /// Creates a new InvalidIndexOperationException exception
        /// </summary>
        /// <param name="myIndexName">The name of the index</param>
        /// <param name="myIndexKey">The index key</param>
        /// <param name="myOperand">The current operand</param>
        public InvalidIndexOperationException(String myIndexName, Object myIndexKey, Object myOperand)
        {
            IndexName = myIndexName;
            IndexKey = myIndexKey;            
            Operand = myOperand;
            _msg = String.Format("A invalid index operation on \"{0}\" occurred (IndexKey: \"{1}\", Operand: \"{2}\").", IndexName, IndexKey, Operand);
        }

        #endregion
       
    }
}
