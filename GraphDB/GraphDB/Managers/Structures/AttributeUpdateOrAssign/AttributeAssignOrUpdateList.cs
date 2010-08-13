using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.Errors;

namespace sones.GraphDB.Managers.Structures
{

    #region AttributeAssignOrUpdateList - This might be the same as AttributeAssignOrUpdateBasicCollection!?!

    public class AttributeAssignOrUpdateList : AAttributeAssignOrUpdate
    {

        #region Properties

        public CollectionDefinition CollectionDefinition { get; private set; }
        public Boolean Assign { get; private set; }

        #endregion

        #region Fields


        #endregion

        #region Ctor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myCollectionDefinition"></param>
        /// <param name="iDChainDefinition"></param>
        /// <param name="myAssign">True to assign, False to update</param>
        public AttributeAssignOrUpdateList(CollectionDefinition myCollectionDefinition, IDChainDefinition iDChainDefinition, Boolean myAssign)
        {
            CollectionDefinition = myCollectionDefinition;
            AttributeIDChain = iDChainDefinition;
            Assign = myAssign;
        }

        #endregion

        #region override AAttributeAssignOrUpdateOrRemove.Update - Refactor!!! Merge with the base update

        public override Exceptional<Dictionary<String, Tuple<TypeAttribute, IObject>>> Update(DBContext myDBContext, DBObjectStream myDBObjectStream, GraphDBType myGraphDBType)
        {

            Dictionary<String, Tuple<TypeAttribute, IObject>> attrsForResult = new Dictionary<String, Tuple<TypeAttribute, IObject>>();

            #region AttributeUpdateList

            #region data

            Exceptional validateResult = AttributeIDChain.Validate(myDBContext, false);
            if (validateResult.Failed())
            {
                return new Exceptional<Dictionary<String, Tuple<TypeAttribute, IObject>>>(validateResult);
            }

            TypeAttribute attrDef = AttributeIDChain.LastAttribute;
            IEdgeType elementsToBeAdded;
            EdgeTypeListOfBaseObjects undefAttrList;

            #endregion

            #region undefined attributes

            if (AttributeIDChain.IsUndefinedAttribute)
            {

                if (Assign)
                {
                    undefAttrList = new EdgeTypeListOfBaseObjects();
                }
                else
                {

                    var loadExcept = LoadUndefAttributes(AttributeIDChain.UndefinedAttribute, myDBContext, myDBObjectStream);

                    if (loadExcept.Failed())
                        return new Exceptional<Dictionary<String, Tuple<TypeAttribute, IObject>>>(loadExcept);

                    if (!loadExcept.Value.ContainsKey(AttributeIDChain.UndefinedAttribute))
                    {
                        var addExcept = myDBContext.DBObjectManager.AddUndefinedAttribute(AttributeIDChain.UndefinedAttribute, new EdgeTypeListOfBaseObjects(), myDBObjectStream);

                        if (addExcept.Failed())
                            return new Exceptional<Dictionary<String, Tuple<TypeAttribute, IObject>>>(addExcept);
                    }

                    if (!(loadExcept.Value[AttributeIDChain.UndefinedAttribute] is EdgeTypeListOfBaseObjects))
                        return new Exceptional<Dictionary<String, Tuple<TypeAttribute, IObject>>>(new Error_InvalidAttributeKind());

                    undefAttrList = (EdgeTypeListOfBaseObjects)loadExcept.Value[AttributeIDChain.UndefinedAttribute];
                }

                var elementsToBeAddedEdge = (IBaseEdge)undefAttrList.GetNewInstance();

                foreach (var tuple in CollectionDefinition.TupleDefinition)
                {
                    if (tuple.Value is ValueDefinition)
                    {
                        var elemToAdd = (tuple.Value as ValueDefinition).Value;
                        ((EdgeTypeListOfBaseObjects)elementsToBeAddedEdge).Add(elemToAdd);
                        undefAttrList.Add(elemToAdd);
                    }
                    else
                    {
                        return new Exceptional<Dictionary<String, Tuple<TypeAttribute, IObject>>>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }
                }

                if (CollectionDefinition.CollectionType == CollectionType.Set)
                    undefAttrList.UnionWith(undefAttrList);

                var addUndefExcept = myDBContext.DBObjectManager.AddUndefinedAttribute(AttributeIDChain.UndefinedAttribute, undefAttrList, myDBObjectStream);

                if (addUndefExcept.Failed())
                    return new Exceptional<Dictionary<String, Tuple<TypeAttribute, IObject>>>(addUndefExcept);

                attrsForResult.Add(AttributeIDChain.UndefinedAttribute, new Tuple<TypeAttribute, IObject>(null, elementsToBeAddedEdge));
                return new Exceptional<Dictionary<String, Tuple<TypeAttribute, IObject>>>(attrsForResult);
            }
            #endregion

            #region Validation that this is really a list

            if (attrDef.KindOfType == KindsOfType.SetOfReferences && CollectionDefinition.CollectionType == CollectionType.List)
            {
                return new Exceptional<Dictionary<String, Tuple<TypeAttribute, IObject>>>(new Error_InvalidAssignOfSet(attrDef.Name));
            }

            #endregion

            if (attrDef.KindOfType == KindsOfType.ListOfNoneReferences || attrDef.KindOfType == KindsOfType.SetOfNoneReferences)
            {
                #region ListOfNoneReferences or SetOfNoneReferences - the edge is in this case IBaseEdge

                elementsToBeAdded = ((IEdgeType)AttributeIDChain.LastAttribute.EdgeType.GetNewInstance());
                foreach (var tupleElem in CollectionDefinition.TupleDefinition)
                {
                    (elementsToBeAdded as IBaseEdge).Add(GraphDBTypeMapper.GetGraphObjectFromTypeName(attrDef.GetDBType(myDBContext.DBTypeManager).Name, (tupleElem.Value as ValueDefinition).Value.Value), tupleElem.Parameters.ToArray());
                }

                #endregion
            }
            else
            {
                #region References

                if (CollectionDefinition.CollectionType == CollectionType.SetOfUUIDs)
                {
                    var result = CollectionDefinition.TupleDefinition.GetAsUUIDEdge(myDBContext, attrDef);
                    if (result.Failed())
                    {
                        return new Exceptional<Dictionary<String, Tuple<TypeAttribute, IObject>>>(result);
                    }
                    elementsToBeAdded = (IEdgeType)result.Value;
                }
                else
                {

                    var result = CollectionDefinition.TupleDefinition.GetCorrespondigDBObjectUUIDAsList(myGraphDBType, myDBContext, AttributeIDChain.LastAttribute.EdgeType.GetNewInstance(), AttributeIDChain.LastAttribute.GetDBType(myDBContext.DBTypeManager));
                    if (result.Failed())
                    {
                        return new Exceptional<Dictionary<String, Tuple<TypeAttribute, IObject>>>(result);
                    }
                    elementsToBeAdded = (IEdgeType)result.Value;
                }

                #endregion

            }

            #region add elements

            if (elementsToBeAdded == null)
            {
                return new Exceptional<Dictionary<String, Tuple<TypeAttribute, IObject>>>(new Error_UpdateAttributeNoElements(AttributeIDChain.LastAttribute));
            }

            if (myDBObjectStream.HasAttribute(AttributeIDChain.LastAttribute.UUID, attrDef.GetRelatedType(myDBContext.DBTypeManager)))
            {
                if (Assign)
                {

                    if (elementsToBeAdded is IReferenceEdge)
                    {
                        var oldEdge = ((IListOrSetEdgeType)myDBObjectStream.GetAttribute(AttributeIDChain.LastAttribute.UUID));
                        var removeRefExcept = RemoveBackwardEdgesOnReferences(this, (IReferenceEdge)oldEdge, myDBObjectStream, myDBContext);

                        if (!removeRefExcept.Success())
                        {
                            return new Exceptional<Dictionary<string,Tuple<TypeAttribute,IObject>>>(removeRefExcept.Errors.First());
                        }
                    }

                    var alterResult = myDBObjectStream.AlterAttribute(AttributeIDChain.LastAttribute.UUID, elementsToBeAdded);
                    if (alterResult.Failed())
                    {
                        return new Exceptional<Dictionary<string, Tuple<TypeAttribute, IObject>>>(alterResult);
                    }
                }
                else
                {
                    ((IListOrSetEdgeType)myDBObjectStream.GetAttribute(AttributeIDChain.LastAttribute.UUID)).UnionWith((IListOrSetEdgeType)elementsToBeAdded);
                }
            }
            else
            {
                myDBObjectStream.AddAttribute(AttributeIDChain.LastAttribute.UUID, elementsToBeAdded);
            }

            #region add backward edges

            if (AttributeIDChain.LastAttribute.GetDBType(myDBContext.DBTypeManager).IsUserDefined)
            {
                Dictionary<AttributeUUID, IObject> userdefinedAttributes = new Dictionary<AttributeUUID, IObject>();
                userdefinedAttributes.Add(AttributeIDChain.LastAttribute.UUID, elementsToBeAdded);

                var omm = new ObjectManipulationManager();
                var setBackEdgesExcept = omm.SetBackwardEdges(myGraphDBType, userdefinedAttributes, myDBObjectStream.ObjectUUID, myDBContext);

                if (setBackEdgesExcept.Failed())
                    return new Exceptional<Dictionary<String, Tuple<TypeAttribute, IObject>>>(setBackEdgesExcept);
            }

            #endregion

            attrsForResult.Add(AttributeIDChain.LastAttribute.Name, new Tuple<TypeAttribute, IObject>(AttributeIDChain.LastAttribute, elementsToBeAdded));
            return new Exceptional<Dictionary<string, Tuple<TypeAttribute, IObject>>>(attrsForResult);

            #endregion

            #endregion

        }

        #endregion

        #region override AAttributeAssignOrUpdate.GetValueForAttribute

        public override Exceptional<IObject> GetValueForAttribute(DBObjectStream aDBObject, DBContext dbContext, GraphDBType _Type)
        {

            #region ListOfDBObjects

            if (AttributeIDChain.LastAttribute.GetDBType(dbContext.DBTypeManager).IsUserDefined)
            {
                //userdefined
                //value = aSetNode.GetCorrespondigDBObjectUUIDs(aTaskNode.AttributeIDNodee, typeManager, dbObjectCache, mySessionToken);

                if (CollectionDefinition.CollectionType == CollectionType.SetOfUUIDs)
                {
                    var retVal = CollectionDefinition.TupleDefinition.GetAsUUIDEdge(dbContext, AttributeIDChain.LastAttribute);
                    if (!retVal.Success())
                    {
                        return new Exceptional<IObject>(retVal);
                    }
                    else
                    {
                        return new Exceptional<IObject>(retVal.Value);
                    }
                }
                else
                {
                    var edge = (IEdgeType)(CollectionDefinition.TupleDefinition.GetCorrespondigDBObjectUUIDAsList(_Type, dbContext, AttributeIDChain.LastAttribute.EdgeType.GetNewInstance(), AttributeIDChain.LastAttribute.GetDBType(dbContext.DBTypeManager)).Value);
                    return new Exceptional<IObject>(edge);
                }
            }
            else
            {
                //not userdefined

                var edge = GetBasicList(dbContext);
                if (edge.Failed())
                {
                    return new Exceptional<IObject>(edge);
                }

                // If the collection was declared as a SETOF insert
                if (CollectionDefinition.CollectionType == CollectionType.Set)
                {
                    edge.Value.Distinction();
                }

                return new Exceptional<IObject>(edge.Value);
            }

            #endregion
        }

        #endregion

        #region GetBasicList

        internal Exceptional<IBaseEdge> GetBasicList(DBContext dbContext)
        {
            var attr = AttributeIDChain.LastAttribute;
            var edge = attr.EdgeType.GetNewInstance() as IBaseEdge;

            //add some basic elements like FavoriteNumbers
            foreach (TupleElement aTupleElement in CollectionDefinition.TupleDefinition)
            {
                //if (GraphDBTypeMapper.IsAValidAttributeType(attr.GetDBType(dbContext.DBTypeManager), aTupleElement.TypeOfValue, dbContext, aTupleElement.Value))
                if (aTupleElement.Value is ValueDefinition)
                {
                    //edge.Add(GraphDBTypeMapper.GetGraphObjectFromTypeName(attr.GetDBType(dbContext.DBTypeManager).Name, aTupleElement.Value), aTupleElement.Parameters.ToArray());
                    if (!(aTupleElement.Value as ValueDefinition).IsDefined)
                    {
                        (aTupleElement.Value as ValueDefinition).ChangeType(attr.GetDBType(dbContext.DBTypeManager).UUID);
                    }
                    edge.Add((aTupleElement.Value as ValueDefinition).Value, aTupleElement.Parameters.ToArray());
                }
                else
                {
                    if (aTupleElement.Value is BinaryExpressionDefinition
                    && ((BinaryExpressionDefinition)aTupleElement.Value).ResultValue.Value is ValueDefinition
                    && GraphDBTypeMapper.IsAValidAttributeType(attr.GetDBType(dbContext.DBTypeManager), ((ValueDefinition)((BinaryExpressionDefinition)aTupleElement.Value).ResultValue.Value).Value.Type, dbContext, aTupleElement.Value))
                    {
                        var val = ((ValueDefinition)((BinaryExpressionDefinition)aTupleElement.Value).ResultValue.Value);
                        if (!val.IsDefined)
                        {
                            val.ChangeType(attr.GetDBType(dbContext.DBTypeManager).UUID);
                        }
                        ((IBaseEdge)edge).Add(val.Value, aTupleElement.Parameters.ToArray());
                    }
                    else if (!(aTupleElement.Value is BinaryExpressionDefinition))
                    {
                        return new Exceptional<IBaseEdge>(new Error_DataTypeDoesNotMatch(attr.GetDBType(dbContext.DBTypeManager).Name, aTupleElement.Value.GetType().Name));
                    }
                    else
                    {
                        //throw new GraphDBException(new Error_SetOfAssignment("Invalid type (" + aAttributeAssignNode.AttributeType + ") of attribute (" + attr.Name + ") for GraphType \"" + myType.Name + "\"."));
                        return new Exceptional<IBaseEdge>(new Error_InvalidAssignOfSet(attr.Name));
                    }
                }
            }

            return new Exceptional<IBaseEdge>(edge);
        }

        #endregion

    }

    #endregion

}
