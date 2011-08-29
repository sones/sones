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
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphDB.TypeManagement.Base;
using sones.Library.Commons.VertexStore.Definitions;
using sones.Library.PropertyHyperGraph;
using sones.Library.UserdefinedDataType;
using sones.Library.LanguageExtensions;


namespace sones.GraphDB.Manager.TypeManagement
{
    /// <summary>
    /// This class realize a base type managment.
    /// </summary>
    public class BaseTypeManager : IManager
    {
        #region Data
        
        /// <summary>
        /// The internal list of all base types.
        /// </summary>
        private readonly Dictionary<long, VertexInformation> _baseTypes;

        /// <summary>
        /// The name index for the base types.
        /// </summary>
        private readonly Dictionary<String, long> _nameIndex;

        /// <summary>
        /// The meta manager.
        /// </summary>
        private IMetaManager _metaManager;

        private readonly Dictionary<long, IVertex> _baseVertices;

        private IEnumerable<AUserdefinedDataType> _userdefinedPlugins;

        #endregion

        #region Constructor

        public BaseTypeManager()
        {
            _baseTypes = new Dictionary<long, VertexInformation>();
            _nameIndex = new Dictionary<string, long>();            
            _baseVertices = new Dictionary<long, IVertex>();
        }

        #endregion

        #region public methods

        /// <summary>
        /// Check's the type name is a base type or not.
        /// </summary>
        /// <param name="myBaseTypeName">The type name, which is to check.</param>
        /// <returns></returns>
        public Boolean IsBaseType(String myBaseTypeName)
        {
            return _nameIndex.ContainsKey(myBaseTypeName);
        }

        /// <summary>
        /// Converts the type name to a base type information.
        /// </summary>
        /// <param name="myBaseTypeName">The type name.</param>
        /// <returns>The vertex information of the base type.</returns>
        public VertexInformation ConvertBaseType(String myBaseTypeName)
        {
            long vertexID;

            if (_nameIndex.TryGetValue(myBaseTypeName, out vertexID))
            {
                return _baseTypes[vertexID];
            }

            throw new NotImplementedException(String.Format("The {0} type is currently not supported.", myBaseTypeName));
        }

        public Type GetBaseType(String myDataTypeName)
        {
            long vertexID;

            if (_nameIndex.TryGetValue(myDataTypeName, out vertexID))
            {
                return GetBaseType(vertexID);
            }

            throw new NotImplementedException(String.Format("The {0} type is currently not supported.", myDataTypeName));
        }

        public Type GetBaseType(long myVertexID)
        {
            IVertex outValue = null;

            if (_baseVertices.TryGetValue(myVertexID, out outValue))
            {
                switch ((BasicTypes)outValue.VertexID)
                {
                    case BasicTypes.Int32:
                        return typeof(Int32);

                    case BasicTypes.String:
                        return typeof(String);

                    case BasicTypes.DateTime:
                        return typeof(DateTime);

                    case BasicTypes.Double:
                        return typeof(Double);

                    case BasicTypes.Boolean:
                        return typeof(Boolean);

                    case BasicTypes.Int64:
                        return typeof(Int64);

                    case BasicTypes.Char:
                        return typeof(Char);

                    case BasicTypes.Byte:
                        return typeof(Byte);

                    case BasicTypes.Single:
                        return typeof(Single);

                    case BasicTypes.SByte:
                        return typeof(SByte);

                    case BasicTypes.Int16:
                        return typeof(Int16);

                    case BasicTypes.UInt32:
                        return typeof(UInt32);

                    case BasicTypes.UInt64:
                        return typeof(UInt64);

                    case BasicTypes.UInt16:
                        return typeof(UInt16);

                    case BasicTypes.TimeSpan:
                        return typeof(TimeSpan);

                    default:
                        return GetUserDefinedBaseType(outValue.GetProperty<String>((long)AttributeDefinitions.BaseTypeDotName));
                }
            }
            else
            {
                throw new NotImplementedException("The type is not supported.");
            }
        }

        #endregion

        #region Private Methods

        private Type GetUserDefinedBaseType(String myDataTypeName)
        {
            var userdefinedType = _userdefinedPlugins.Where(item => item.TypeName == myDataTypeName);

            if (userdefinedType.IsNullOrEmpty())
            {
                throw new NotImplementedException(String.Format("The type {0} is not supported.", myDataTypeName));
            }

            return userdefinedType.First().GetType();
        }

        #endregion

        #region IManager Members

        public void Initialize(IMetaManager myMetaManager)
        {
            _metaManager = myMetaManager;
        }

        public void Load(Int64 myTransaction, SecurityToken mySecurity)
        {
            var baseTypes = _metaManager.VertexStore.GetVerticesByTypeID(mySecurity, myTransaction, (long)BaseTypes.BaseType);
            _userdefinedPlugins = _metaManager.PluginManager.GetPluginsForType<AUserdefinedDataType>().Cast<AUserdefinedDataType>();

            foreach (var type in baseTypes)
            {                
                _baseTypes.Add(type.VertexID, new VertexInformation(type.VertexTypeID, type.VertexID));
                _baseVertices.Add(type.VertexID, type);
                _nameIndex.Add(type.GetProperty<String>((long)AttributeDefinitions.BaseTypeDotName), type.VertexID);
            }
        }

        #endregion
    }
}
