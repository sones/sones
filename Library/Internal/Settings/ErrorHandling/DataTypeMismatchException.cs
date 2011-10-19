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

namespace sones.Library.Settings.ErrorHandling
{
    /// <summary>
    /// The type of the value does not match the type definition
    /// </summary>
    public sealed class DataTypeMismatchException : ASettingsException
    {
        #region Data

        /// <summary>
        /// The type that is expected by the setting
        /// </summary>
        public readonly Type ReferenceType;

        /// <summary>
        /// The type of the actual setting
        /// </summary>
        public readonly Type ActualType;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new DataTypeMismatchException
        /// </summary>
        /// <param name="myReferenceType">The type that is expected by the setting</param>
        /// <param name="myActualType">The type of the actual setting</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
		public DataTypeMismatchException(Type myReferenceType, Type myActualType, Exception innerException = null) : base(innerException)
        {
            ReferenceType = myReferenceType;
            ActualType = myActualType;
        }

        #endregion

        public override string ToString()
        {
            return String.Format("The type of the value ({0}) does not match the type definition ({1})", ActualType.Name, ReferenceType.Name);
        }
    }
}
