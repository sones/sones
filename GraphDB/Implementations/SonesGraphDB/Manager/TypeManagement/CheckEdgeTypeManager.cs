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
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.LanguageExtensions;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.ErrorHandling;

namespace sones.GraphDB.Manager.TypeManagement
{
    internal class CheckEdgeTypeManager: ACheckTypeManager<IEdgeType>
    {
        #region ACheckTypeManager member

        public override void Initialize(IMetaManager myMetaManager)
        {
            _baseTypeManager = myMetaManager.BaseTypeManager;
        }

        public override void Load(TransactionToken myTransaction, 
                                    SecurityToken mySecurity)
        { }

        #endregion

        #region private helper

        #region private abstract helper

        protected override void ConvertPropertyUniques(ATypePredefinition myTypePredefinition)
        {
            if (myTypePredefinition.Properties != null)
                foreach (var uniqueProp in myTypePredefinition.Properties.Where(_ => _.IsUnique))
                {
                    (myTypePredefinition as VertexTypePredefinition)
                        .AddUnique(new UniquePredefinition(uniqueProp.AttributeName));
                }
        }

        /// <summary>
        /// Checks if the given paramter type is valid.
        /// </summary>
        /// <param name="myTypePredefinitions">The parameter to be checked.</param>
        protected override void CheckPredefinitionsType(IEnumerable<ATypePredefinition> myTypePredefinitions)
        {
            if (!(myTypePredefinitions is IEnumerable<EdgeTypePredefinition>))
                if (!myTypePredefinitions.All(_ => _ is EdgeTypePredefinition))
                    throw new InvalidParameterTypeException("TypePredefinitions",
                                                            myTypePredefinitions.GetType().Name,
                                                            typeof(IEnumerable<EdgeTypePredefinition>).GetType().Name,
                                                            "");
        }

        protected override void ConvertUnknownAttributes(ATypePredefinition myTypePredefinition)
        {
            if (myTypePredefinition.UnknownAttributes == null)
                return;

            var toBeConverted = myTypePredefinition.UnknownAttributes.ToArray();

            foreach (var unknown in toBeConverted)
            {
                if (_baseTypeManager.IsBaseType(unknown.AttributeType))
                {
                    var prop = ConvertUnknownToProperty(unknown);

                    (myTypePredefinition as EdgeTypePredefinition).AddProperty(prop);
                }
                else
                    throw new PropertyHasWrongTypeException(myTypePredefinition.TypeName, unknown.AttributeName, unknown.Multiplicity, "a base type");
            }

            myTypePredefinition.ResetUnknown();
        }

        /// <summary>
        /// Checks whether a given type name is not a basix vertex type.
        /// </summary>
        /// <param name="myTypeName">The type name to be checked.</param>
        /// <returns>True, if the type name is the name of a base vertex type (but Vertex), otherwise false.</returns>
        protected override bool CanBeParentType(string myTypeName)
        {
            BaseTypes type;
            if (!Enum.TryParse(myTypeName, out type))
                return false;

            return type == BaseTypes.Edge;
        }

        /// <summary>
        /// Checks the uniqueness of attribute names on a vertex type predefinition without asking the FS.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The vertex type predefinition to be checked.</param>
        protected override void CheckAttributes(ATypePredefinition vertexTypeDefinition)
        {
            var uniqueNameSet = new HashSet<string>();

            CheckPropertiesUniqueName(vertexTypeDefinition, uniqueNameSet);
        }

        protected override void CanRemove(IEnumerable<IEdgeType> myTypes,
                                            TransactionToken myTransaction,
                                            SecurityToken mySecurity,
                                            bool myIgnoreReprimands)
        {
            #region check if specified types can be removed

            //get child vertex types and check if they are specified by user
            foreach (var delType in myTypes)
            {
                #region check that the remove type is no base type

                if (delType == null)
                    throw new TypeRemoveException<IEdgeType>("null", "Edge Type is null.");

                if (!delType.HasParentType)
                    continue;

                if (delType.ParentEdgeType.ID.Equals((long)BaseTypes.BaseType) && IsTypeBaseType(delType.ID))
                    //Exception that base type cannot be deleted
                    throw new TypeRemoveException<IEdgeType>(delType.Name, "A BaseType connot be removed.");

                #endregion

                #region check that existing child types are specified

                if (delType.GetDescendantEdgeTypes().Any(child => !myTypes.Contains(child)))
                    throw new TypeRemoveException<IEdgeType>(delType.Name, "The given type has child types and cannot be removed.");

                #endregion
            }

            #endregion
        }

        #endregion

        #endregion
    }
}
