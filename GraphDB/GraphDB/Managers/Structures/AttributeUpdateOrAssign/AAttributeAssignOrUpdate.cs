#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.Lib.ErrorHandling;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphDB.Managers.Structures
{

    public abstract class AAttributeAssignOrUpdate : AAttributeAssignOrUpdateOrRemove
    {

        #region Ctors

        public AAttributeAssignOrUpdate() { }

        public AAttributeAssignOrUpdate(IDChainDefinition myIDChainDefinition)
        {
            AttributeIDChain = myIDChainDefinition;
        }

        #endregion

        #region abstract GetValueForAttribute

        public abstract Exceptional<AObject> GetValueForAttribute(DBObjectStream aDBObject, DBContext dbContext, GraphDBType myGraphDBType);

        #endregion

        #region Update

        public override Exceptional<Dictionary<string, Tuple<TypeAttribute, AObject>>> Update(DBContext myDBContext, DBObjectStream myDBObjectStream, GraphDBType myGraphDBType)
        {

            Dictionary<String, Tuple<TypeAttribute, AObject>> attrsForResult = new Dictionary<String, Tuple<TypeAttribute, AObject>>();

            if (base.AttributeIDChain.IsUndefinedAttribute)
            {

                #region Undefined attribute

                var applyResult = ApplyAssignUndefinedAttribute(myDBContext, myDBObjectStream, myGraphDBType);

                if (applyResult.Failed)
                {
                    return new Exceptional<Dictionary<string, Tuple<TypeAttribute, AObject>>>(applyResult);
                }

                if (applyResult.Value != null)
                {
                    //sthChanged = true;

                    #region Add to queryResult

                    attrsForResult.Add(applyResult.Value.Item1, new Tuple<TypeAttribute, AObject>(applyResult.Value.Item2, applyResult.Value.Item3));

                    #endregion
                }

                #endregion

            }
            else
            {

                #region Usual attribute

                var applyResult = ApplyAssignAttribute(this, myDBContext, myDBObjectStream, myGraphDBType);

                if (applyResult.Failed)
                {
                    return new Exceptional<Dictionary<string, Tuple<TypeAttribute, AObject>>>(applyResult);
                }

                if (applyResult.Value != null)
                {
                    //sthChanged = true;

                    #region Add to queryResult

                    attrsForResult.Add(applyResult.Value.Item1, new Tuple<TypeAttribute, AObject>(applyResult.Value.Item2, applyResult.Value.Item3));

                    #endregion
                }

                #endregion
            
            }

            return new Exceptional<Dictionary<string, Tuple<TypeAttribute, AObject>>>(attrsForResult);

        }


        #region override AAttributeAssignOrUpdateOrRemove.Update

        private Exceptional<Tuple<String, TypeAttribute, AObject>> ApplyAssignUndefinedAttribute(DBContext myDBContext, DBObjectStream myDBObjectStream, GraphDBType myGraphDBType)
        {
            Dictionary<String, Tuple<TypeAttribute, AObject>> attrsForResult = new Dictionary<String, Tuple<TypeAttribute, AObject>>();

            #region undefined attributes

            var newValue = GetValueForAttribute(myDBObjectStream, myDBContext, myGraphDBType);
            if (newValue.Failed)
            {
                return new Exceptional<Tuple<string, TypeAttribute, AObject>>(newValue);
            }

            if (myDBObjectStream.ContainsUndefinedAttribute(AttributeIDChain.UndefinedAttribute, myDBContext.DBObjectManager))
            {
                var removeResult =myDBObjectStream.RemoveUndefinedAttribute(AttributeIDChain.UndefinedAttribute, myDBContext.DBObjectManager);
                if (removeResult.Failed)
                {
                    return new Exceptional<Tuple<string, TypeAttribute, AObject>>(removeResult);
                }
            }

            //TODO: change this to a more handling thing than KeyValuePair
            var addExcept = myDBContext.DBObjectManager.AddUndefinedAttribute(AttributeIDChain.UndefinedAttribute, newValue.Value, myDBObjectStream);

            if (addExcept.Failed)
            {
                return new Exceptional<Tuple<String, TypeAttribute, AObject>>(addExcept);
            }

            //sthChanged = true;

            attrsForResult.Add(AttributeIDChain.UndefinedAttribute, new Tuple<TypeAttribute, AObject>(null, newValue.Value));

            #endregion

            return new Exceptional<Tuple<String, TypeAttribute, AObject>>(new Tuple<String, TypeAttribute, AObject>(AttributeIDChain.UndefinedAttribute, AttributeIDChain.LastAttribute, newValue.Value));

        }

        #endregion

        internal Exceptional<Tuple<String, TypeAttribute, AObject>> ApplyAssignAttribute(AAttributeAssignOrUpdate myAAttributeAssign, DBContext myDBContext, DBObjectStream myDBObject, GraphDBType myGraphDBType)
        {

            System.Diagnostics.Debug.Assert(myAAttributeAssign != null);

            //get value for assignement
            var aValue = myAAttributeAssign.GetValueForAttribute(myDBObject, myDBContext, myGraphDBType);
            if (aValue.Failed)
            {
                return new Exceptional<Tuple<String, TypeAttribute, AObject>>(aValue);
            }

            object oldValue = null;
            AObject newValue = aValue.Value;

            if (myDBObject.HasAttribute(myAAttributeAssign.AttributeIDChain.LastAttribute.UUID, myGraphDBType))
            {

                #region Update the value because it already exists

                oldValue = myDBObject.GetAttribute(myAAttributeAssign.AttributeIDChain.LastAttribute.UUID);

                switch (myAAttributeAssign.AttributeIDChain.LastAttribute.KindOfType)
                {
                    case KindsOfType.SetOfReferences:
                        var typeOfCollection = ((AttributeAssignOrUpdateList)myAAttributeAssign).CollectionDefinition.CollectionType;

                        if (typeOfCollection == CollectionType.List)
                            return new Exceptional<Tuple<String, TypeAttribute, AObject>>(new Error_InvalidAssignOfSet(myAAttributeAssign.AttributeIDChain.LastAttribute.Name));

                        var removeRefExcept = RemoveBackwardEdgesOnReferences(myAAttributeAssign, (IReferenceEdge)oldValue, myDBObject, myDBContext);

                        if (!removeRefExcept.Success)
                            return new Exceptional<Tuple<String, TypeAttribute, AObject>>(removeRefExcept.Errors.First());

                        newValue = (ASetReferenceEdgeType)newValue;
                        break;

                    case KindsOfType.SetOfNoneReferences:
                    case KindsOfType.ListOfNoneReferences:
                        newValue = (AListBaseEdgeType)newValue;
                        break;

                    case KindsOfType.SingleNoneReference:
                        if (!(oldValue as ADBBaseObject).IsValidValue((newValue as ADBBaseObject).Value))
                        {
                            return new Exceptional<Tuple<string, TypeAttribute, AObject>>(new Error_DataTypeDoesNotMatch((oldValue as ADBBaseObject).ObjectName, (newValue as ADBBaseObject).ObjectName));
                        }
                        newValue = (oldValue as ADBBaseObject).Clone((newValue as ADBBaseObject).Value);
                        break;

                    case KindsOfType.SingleReference:
                        if (newValue is ASingleReferenceEdgeType)
                        {
                            removeRefExcept = RemoveBackwardEdgesOnReferences(myAAttributeAssign, (IReferenceEdge)oldValue, myDBObject, myDBContext);

                            if (!removeRefExcept.Success)
                                return new Exceptional<Tuple<String, TypeAttribute, AObject>>(removeRefExcept.Errors.First());

                            ((ASingleReferenceEdgeType)oldValue).Merge((ASingleReferenceEdgeType)newValue);
                            newValue = (ASingleReferenceEdgeType)oldValue;
                        }
                        break;

                    default:
                        return new Exceptional<Tuple<String, TypeAttribute, AObject>>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                }

                #endregion

            }

            var alterExcept = myDBObject.AlterAttribute(myAAttributeAssign.AttributeIDChain.LastAttribute.UUID, newValue);

            if (alterExcept.Failed)
                return new Exceptional<Tuple<string, TypeAttribute, AObject>>(alterExcept);

            if (!alterExcept.Value)
            {
                myDBObject.AddAttribute(myAAttributeAssign.AttributeIDChain.LastAttribute.UUID, newValue);
            }

            #region add backward edges

            if (myAAttributeAssign.AttributeIDChain.LastAttribute.GetDBType(myDBContext.DBTypeManager).IsUserDefined)
            {
                Dictionary<AttributeUUID, AObject> userdefinedAttributes = new Dictionary<AttributeUUID, AObject>();
                userdefinedAttributes.Add(myAAttributeAssign.AttributeIDChain.LastAttribute.UUID, newValue);

                var omm = new ObjectManipulationManager();
                var setBackEdges = omm.SetBackwardEdges(myGraphDBType, userdefinedAttributes, myDBObject.ObjectUUID, myDBContext);

                if (setBackEdges.Failed)
                    return new Exceptional<Tuple<string, TypeAttribute, AObject>>(setBackEdges);
            }

            #endregion

            return new Exceptional<Tuple<String, TypeAttribute, AObject>>(new Tuple<String, TypeAttribute, AObject>(myAAttributeAssign.AttributeIDChain.LastAttribute.Name, myAAttributeAssign.AttributeIDChain.LastAttribute, newValue));
        }

        protected Exceptional<Boolean> RemoveBackwardEdgesOnReferences(AAttributeAssignOrUpdate myAAttributeAssign, IReferenceEdge myReference, DBObjectStream myDBObject, DBContext myDBContext)
        {
            foreach (var item in myReference.GetAllReferenceIDs())
            {
                var streamExcept = myDBContext.DBObjectCache.LoadDBObjectStream(myAAttributeAssign.AttributeIDChain.LastAttribute.GetDBType(myDBContext.DBTypeManager), (ObjectUUID)item);

                if (!streamExcept.Success)
                    return new Exceptional<Boolean>(streamExcept.Errors.First());

                var removeExcept = myDBContext.DBObjectManager.RemoveBackwardEdge(streamExcept.Value, myAAttributeAssign.AttributeIDChain.LastAttribute.RelatedGraphDBTypeUUID, myAAttributeAssign.AttributeIDChain.LastAttribute.UUID, myDBObject.ObjectUUID);

                if (!removeExcept.Success)
                    return new Exceptional<Boolean>(removeExcept.Errors.First());
            }

            return new Exceptional<Boolean>(true);
        }

        #endregion

    }

}
