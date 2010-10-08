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

#endregion

namespace sones.GraphDB.Managers.AlterType
{

    public class AlterType_DropAttributes : AAlterTypeCommand
    {

        List<String> _ListOfAttributes = new List<String>();

        public AlterType_DropAttributes(List<String> listOfAttributes)
        {
            _ListOfAttributes = listOfAttributes;
        }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.Drop; }
        }

        /// <summary>
        /// Executes the removal of certain myAttributes.
        /// </summary>
        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {

            foreach (String aAttributeName in _ListOfAttributes)
            {

                //Hack: remove myAttributes in DBObjects
                var aTempResult = myDBContext.DBTypeManager.RemoveAttributeFromType(myGraphDBType.Name, aAttributeName, myDBContext.DBTypeManager);

                if (aTempResult.Failed())
                {
                    return aTempResult;
                }
            }

            return Exceptional.OK;
        }

        public override IEnumerable<Vertex> CreateVertex(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return null;
        }

    }

}
