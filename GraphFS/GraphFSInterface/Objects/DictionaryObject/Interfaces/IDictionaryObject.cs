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
 * IDictionaryObject
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;

using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.Dictionaries;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// The interface of a DictionaryObject to store a mapping TKey => TValue.
    /// </summary>
    /// <typeparam name="TKey">Must implement IComparable</typeparam>

    public interface IDictionaryObject<TKey, TValue> : IDictionaryInterface<TKey, TValue>, IObjectLocation
        where TKey : IComparable
    {

        #region Members of AGraphHeader

        Boolean       isNew                   { get; set; }
        INode         INodeReference          { get; }
        ObjectLocator ObjectLocatorReference  { get; set; }
        ObjectUUID    ObjectUUID              { get; }

        #endregion

        #region Members of IFastSerialize

        Boolean       isDirty                 { get; set; }

        #endregion

    }

}