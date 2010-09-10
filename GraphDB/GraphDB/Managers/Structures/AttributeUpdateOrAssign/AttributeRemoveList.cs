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
 * 
 * (c) Stefan Licht, 2010
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;

using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.Errors;
using sones.GraphDBInterface.TypeManagement;

namespace sones.GraphDB.Managers.Structures
{

    /// <summary>
    /// Removes some values from all DBOs of the given IDChain or attributeName
    /// </summary>
    public class AttributeRemoveList : AAttributeRemove
    {

        #region Properties

        public string AttributeName { get; private set; }
        public TupleDefinition TupleDefinition { get; private set; }

        #endregion

        #region Ctor

        public AttributeRemoveList(IDChainDefinition myIDChainDefinition, string myAttributeName, TupleDefinition myTupleDefinition)
        {
            // TODO: Complete member initialization
            this.AttributeIDChain = myIDChainDefinition;
            this.AttributeName = myAttributeName;
            this.TupleDefinition = myTupleDefinition;
        }

        #endregion

        #region override AAttributeAssignOrUpdateOrRemove.Update

        public override Exceptional<Dictionary<string, Tuple<TypeAttribute, IObject>>> Update(DBContext myDBContext, DBObjectStream myDBObjectStream, GraphDBType myGraphDBType)
        {

            Dictionary<String, Tuple<TypeAttribute, IObject>> attrsForResult = new Dictionary<String, Tuple<TypeAttribute, IObject>>();

            #region AttributeRemoveList

            #region data

            Exceptional validateResult = AttributeIDChain.Validate(myDBContext, false);
            if (validateResult.Failed())
            {
                return new Exceptional<Dictionary<string, Tuple<TypeAttribute, IObject>>>(validateResult);
            }

            ASetOfReferencesEdgeType _elementsToBeRemoved;
            EdgeTypeListOfBaseObjects undefAttrList;

            #endregion

            #region undefined attributes

            if (AttributeIDChain.IsUndefinedAttribute)
            {
                var loadExcept = LoadUndefAttributes(AttributeName, myDBContext, myDBObjectStream);

                if (loadExcept.Failed())
                    return new Exceptional<Dictionary<string, Tuple<TypeAttribute, IObject>>>(loadExcept);

                if (!loadExcept.Value.ContainsKey(AttributeName))
                {
                    attrsForResult.Add(AttributeName, new Tuple<TypeAttribute, IObject>(null, null));
                    return new Exceptional<Dictionary<string, Tuple<TypeAttribute, IObject>>>(attrsForResult);
                }

                if (!(loadExcept.Value[AttributeName] is EdgeTypeListOfBaseObjects))
                    return new Exceptional<Dictionary<string, Tuple<TypeAttribute, IObject>>>(new Error_InvalidAttributeKind());

                undefAttrList = (EdgeTypeListOfBaseObjects)loadExcept.Value[AttributeName];

                foreach (var tuple in TupleDefinition)
                {
                    undefAttrList.Remove((tuple.Value as ValueDefinition).Value);
                }

                myDBContext.DBObjectManager.AddUndefinedAttribute(AttributeName, undefAttrList, myDBObjectStream);

                attrsForResult.Add(AttributeName, new Tuple<TypeAttribute, IObject>(null, null));
                return new Exceptional<Dictionary<string, Tuple<TypeAttribute, IObject>>>(attrsForResult);
            }

            #endregion

            #region get elements

            if (AttributeIDChain.LastAttribute.EdgeType == null)
            {
                return new Exceptional<Dictionary<string, Tuple<TypeAttribute, IObject>>>(new Error_InvalidEdgeType(AttributeIDChain.LastAttribute.GetType()));
            }

            var elements = TupleDefinition.GetCorrespondigDBObjectUUIDAsList(myGraphDBType, myDBContext, AttributeIDChain.LastAttribute.EdgeType.GetNewInstance(), AttributeIDChain.LastAttribute.GetDBType(myDBContext.DBTypeManager));

            if(!elements.Success())
            {
                return new Exceptional<Dictionary<string,Tuple<TypeAttribute,IObject>>>(elements);
            }

            _elementsToBeRemoved = (ASetOfReferencesEdgeType)elements.Value;            

            #endregion

            #region remove elements from list

            if (_elementsToBeRemoved == null)
            {
                return new Exceptional<Dictionary<string, Tuple<TypeAttribute, IObject>>>(new Error_UpdateAttributeNoElements(AttributeIDChain.LastAttribute));
            }

            if (myDBObjectStream.HasAttribute(AttributeIDChain.LastAttribute.UUID, AttributeIDChain.LastAttribute.GetRelatedType(myDBContext.DBTypeManager)))
            {
                ASetOfReferencesEdgeType edge = (ASetOfReferencesEdgeType)myDBObjectStream.GetAttribute(AttributeIDChain.LastAttribute.UUID);

                foreach (var aUUID in (_elementsToBeRemoved as ASetOfReferencesEdgeType).GetAllReferenceIDs())
                {
                    edge.RemoveUUID(aUUID);
                }

                #region remove backward edges

                if (AttributeIDChain.LastAttribute.GetDBType(myDBContext.DBTypeManager).IsUserDefined)
                {
                    Dictionary<AttributeUUID, object> userdefinedAttributes = new Dictionary<AttributeUUID, object>();
                    userdefinedAttributes.Add(AttributeIDChain.LastAttribute.UUID, _elementsToBeRemoved);

                    RemoveBackwardEdges(myGraphDBType.UUID, userdefinedAttributes, myDBObjectStream.ObjectUUID, myDBContext);
                }

                #endregion

                attrsForResult.Add(AttributeIDChain.LastAttribute.Name, new Tuple<TypeAttribute, IObject>(AttributeIDChain.LastAttribute, edge));
                return new Exceptional<Dictionary<string, Tuple<TypeAttribute, IObject>>>(attrsForResult);
            }

            #endregion

            #endregion

            return new Exceptional<Dictionary<string, Tuple<TypeAttribute, IObject>>>(attrsForResult);

        }

        #endregion

    }

}
