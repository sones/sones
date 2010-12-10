/*
 * AttributeRemove
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;

using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Errors;
using sones.Lib;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.Managers.Structures
{

    /// <summary>
    /// Removes some attributes
    /// </summary>
    public class AttributeRemove : AAttributeRemove
    {
        
        #region Properties

        /// <summary>
        /// The list of attributes to remove
        /// </summary>
        public List<string> ToBeRemovedAttributes { get; private set; }

        #endregion

        #region Ctor

        public AttributeRemove(List<string> _toBeRemovedAttributes)
        {
            // TODO: Complete member initialization
            this.ToBeRemovedAttributes = _toBeRemovedAttributes;
        }

        #endregion

        #region override AAttributeAssignOrUpdateOrRemove.Update

        /// <summary>
        /// <seealso cref=" AAttributeAssignOrUpdateOrRemove"/>
        /// </summary>
        public override Exceptional<Dictionary<string, Tuple<TypeAttribute, IObject>>> Update(DBContext myDBContext, DBObjectStream myDBObjectStream, GraphDBType myGraphDBType)
        {

            Dictionary<String, Tuple<TypeAttribute, IObject>> attrsForResult = new Dictionary<String, Tuple<TypeAttribute, IObject>>();

            #region AttributeRemove

            #region divide remove list in undefined and defined attributes

            var undefAttrsExcept = myDBObjectStream.GetUndefinedAttributePayload(myDBContext.DBObjectManager);

            if (undefAttrsExcept.Failed())
            {
                return new Exceptional<Dictionary<string, Tuple<TypeAttribute, IObject>>>(undefAttrsExcept);
            }


            var undefAttrsOfObject = undefAttrsExcept.Value;

            var undefAttrsToRemove = ToBeRemovedAttributes.Where(item => undefAttrsOfObject.ContainsKey(item)).ToList();

            List<String> defAttrsToRemove = new List<String>();
            List<String> unknowAttrs = new List<String>();

            foreach (var item in ToBeRemovedAttributes.Where(item => !undefAttrsOfObject.ContainsKey(item)).ToList())
            {
                if (myGraphDBType.GetTypeAttributeByName(item) != null)
                    defAttrsToRemove.Add(item);
                else
                    unknowAttrs.Add(item);
            }

            #endregion

            #region remove undefined attributes
            
            if (!unknowAttrs.IsNullOrEmpty())
            {
                return new Exceptional<Dictionary<string, Tuple<TypeAttribute, IObject>>>(new Error_InvalidUndefinedAttributes(unknowAttrs));
            }

            foreach (var aAttribute in undefAttrsToRemove)
            {
                var removeExcept = myDBContext.DBObjectManager.RemoveUndefinedAttribute(aAttribute, myDBObjectStream);

                if (removeExcept.Failed())
                {
                    return new Exceptional<Dictionary<string, Tuple<TypeAttribute, IObject>>>(removeExcept);
                }

                attrsForResult.Add(aAttribute, new Tuple<TypeAttribute, IObject>(new UndefinedTypeAttribute(aAttribute), null));
            }

            //if (!undefAttrsToRemove.IsNullOrEmpty())
            //    sthChanged = true;

            #endregion

            #region RemoveAttribute

            var applyRemoveResult = ApplyRemoveAttribute(defAttrsToRemove, myDBContext, myDBObjectStream, myGraphDBType);

            if (applyRemoveResult.Failed())
            {
                return new Exceptional<Dictionary<string, Tuple<TypeAttribute, IObject>>>(applyRemoveResult);
            }

            if (applyRemoveResult.Value.Count > 0)
            {
                //sthChanged = true;

                #region Add to queryResult

                foreach (var attr in applyRemoveResult.Value)
                {
                    attrsForResult.Add(attr.Name, new Tuple<TypeAttribute, IObject>(attr, null));
                }

                #endregion

            }

            #endregion

            #endregion

            return new Exceptional<Dictionary<string, Tuple<TypeAttribute, IObject>>>(attrsForResult);

        }

        /// <summary>
        /// Execute the remove of attributes
        /// </summary>
        /// <param name="myAttrsToRemove">The list of attributes to remove</param>
        /// <param name="myDBContext">The db context</param>
        /// <param name="myDBDBObject">The db object from which the attributes should be deleted</param>
        /// <param name="myGraphDBType">The type of the db object</param>
        /// <returns>The list of removed attributes</returns>
        private Exceptional<List<TypeAttribute>> ApplyRemoveAttribute(List<string> myAttrsToRemove, DBContext myDBContext, DBObjectStream myDBDBObject, GraphDBType myGraphDBType)
        {
            #region data

            List<TypeAttribute> removedAttributes = new List<TypeAttribute>();

            #endregion

            var MandatoryTypeAttrib = myGraphDBType.GetMandatoryAttributesUUIDs(myDBContext.DBTypeManager);
            foreach (String aAttribute in myAttrsToRemove)
            {
                TypeAttribute typeAttribute = myGraphDBType.GetTypeAttributeByName(aAttribute);

                if (myDBDBObject.HasAttribute(typeAttribute.UUID, myGraphDBType))
                {
                    if (!MandatoryTypeAttrib.Contains(typeAttribute.UUID))
                    {
                        #region remove backward edges

                        if (typeAttribute.GetDBType(myDBContext.DBTypeManager).IsUserDefined)
                        {
                            var userdefinedAttributes = new Dictionary<AttributeUUID, object>();
                            userdefinedAttributes.Add(typeAttribute.UUID, myDBDBObject.GetAttribute(typeAttribute.UUID));

                            RemoveBackwardEdges(myGraphDBType.UUID, userdefinedAttributes, myDBDBObject.ObjectUUID, myDBContext);
                        }

                        #endregion

                        myDBDBObject.RemoveAttribute(typeAttribute.UUID);
                        removedAttributes.Add(typeAttribute);
                    }
                    else
                    {
                        return new Exceptional<List<TypeAttribute>>(new Error_MandatoryConstraintViolation("Error in update statement. The attribute \"" + typeAttribute.Name + "\" is mandatory and can not be removed."));
                    }
                }
            }


            return new Exceptional<List<TypeAttribute>>(removedAttributes);
        }

        #endregion

    }
}
