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
using System.IO;

namespace sones.Library.Commons.VertexStore.Definitions
{
    /// <summary>
    /// This struct represents the filesystem definition for a stream
    /// </summary>
    public sealed class StreamAddDefinition
    {
        #region data

        /// <summary>
        /// The stream that should be added to the filesystem
        /// </summary>
        public readonly Stream Stream;

        /// <summary>
        /// The id of the stream
        /// </summary>
        public readonly Int64 PropertyID;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new stream definition
        /// </summary>
        /// <param name="myPropertyID">The id of the stream</param>
        /// <param name="myStream">The stream that should be added to the filesystem</param>
        public StreamAddDefinition(
            Int64 myPropertyID,
            Stream myStream)
        {
            Stream = myStream;
            PropertyID = myPropertyID;
        }

        #endregion
    }
}