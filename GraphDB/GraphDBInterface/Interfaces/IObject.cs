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


#region Usings

using System;
using sones.Lib.NewFastSerializer;
using sones.Lib.Serializer;
using sones.Lib;

#endregion

namespace sones.GraphDB.TypeManagement
{

    /// <summary>
    /// Each Object which go threw the WebService need to derive from AObject.
    /// The main transforming is currently done by GraphDatabaseHost.TransformSelectionListForCustomer.
    /// </summary>
    public interface IObject : IComparable, IFastSerialize, IFastSerializationTypeSurrogate, IEstimable
    {
        
    }

}
