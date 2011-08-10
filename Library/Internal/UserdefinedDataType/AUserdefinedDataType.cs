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
using sones.Library.Serializer;
using sones.Library.NewFastSerializer;
using sones.Library.VersionedPluginManager;

namespace sones.Library.UserdefinedDataType
{
    /// <summary>
    /// Defines an abstract class for user defined data types.
    /// </summary>
    public abstract class AUserdefinedDataType : IFastSerialize, IComparable, IPluginable
    {
        #region property

        /// <summary>
        /// The full qualified class name of the user defined type.
        /// </summary>
        public abstract String TypeName { get; }
   
        #endregion

        #region IFastSerialize Members

        public abstract void Serialize(ref SerializationWriter mySerializationWriter);


        public abstract void Deserialize(ref SerializationReader mySerializationReader);
        

        #endregion

        #region IComparable Members

        public abstract int CompareTo(object obj);        

        #endregion

        #region IPluginable Members

        public abstract string PluginName
        {
            get;
        }

        public abstract string PluginShortName
        {
            get;
        }

        public abstract PluginParameters<Type> SetableParameters
        {
            get;
        }

        public abstract IPluginable InitializePlugin(string UniqueString, Dictionary<string, object> myParameters = null);
        

        #endregion

        #region IDisposable Members

        public abstract void Dispose();
        

        #endregion
    }
}
