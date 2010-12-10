/*
 * AlterType_DropAttributes
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.NewAPI;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures.Enums;

using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;

#endregion

namespace sones.GraphDB.Managers.AlterType
{

    /// <summary>
    /// Drop attributes from a vertex
    /// </summary>
    public class AlterType_DropAttributes : AAlterTypeCommand
    {

        private List<String> _ListOfAttributes = new List<String>();

        public AlterType_DropAttributes(List<String> listOfAttributes)
        {
            _ListOfAttributes = listOfAttributes;
        }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.Drop; }
        }

        /// <summary>
        /// Executes the removal of certain myAttributes.
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            Exceptional result = new Exceptional();

            foreach (String aAttributeName in _ListOfAttributes)
            {
                var attr = myGraphDBType.GetTypeAttributeByName(aAttributeName);

                if (attr != null)
                {
                    var idxs = new List<Indices.AAttributeIndex>(myGraphDBType.GetAttributeIndices(attr.UUID));

                    foreach (var item in idxs)
                    {
                        var remIdxResult = myGraphDBType.RemoveIndex(item.IndexName, item.IndexEdition, myDBContext.DBTypeManager);

                        if (remIdxResult.Failed())
                        {
                            result.PushIExceptional(remIdxResult);
                        }
                    }
                }
                else
                {
                    result.PushIError(new Error_AttributeIsNotDefined(aAttributeName));
                    return result;
                }

                //Hack: remove myAttributes in DBObjects
                var aTempResult = myDBContext.DBTypeManager.RemoveAttributeFromType(myGraphDBType.Name, aAttributeName, myDBContext.DBTypeManager);

                if (aTempResult.Failed())
                {
                    result.PushIExceptional(aTempResult);
                    return result;
                }
            }

            return result;
        }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>        
        public override IEnumerable<Vertex> CreateVertex(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return null;
        }

    }

}
