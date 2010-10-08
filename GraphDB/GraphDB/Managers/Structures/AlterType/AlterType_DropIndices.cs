/*
 * AlterType_DropIndices
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

    public class AlterType_DropIndices : AAlterTypeCommand
    {

        private Dictionary<String, String> _IdxDropList;

        public AlterType_DropIndices(Dictionary<String, String> myIndices)
        {
            _IdxDropList = myIndices;
        }

        public override TypesOfAlterCmd AlterType
        {
            get { throw new NotImplementedException(); }
        }

        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {

            var retExceptional = new Exceptional();

            foreach (var index in _IdxDropList)
            {
                var dropIdxExcept = myGraphDBType.RemoveIndex(index.Key, index.Value, myDBContext.DBTypeManager);

                if (!dropIdxExcept.Success())
                {
                    retExceptional.PushIExceptional(dropIdxExcept);
                }
            }

            return retExceptional;

        }

        public override IEnumerable<Vertex> CreateVertex(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return base.CreateVertex(myDBContext, myGraphDBType);
        }

    }

}
