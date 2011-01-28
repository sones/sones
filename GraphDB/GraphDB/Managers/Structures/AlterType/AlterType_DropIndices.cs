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

    /// <summary>
    /// Removal of attribute indices
    /// </summary>
    public class AlterType_DropIndices : AAlterTypeCommand
    {

        /// <summary>
        /// The list of the indices
        /// </summary>
        private Dictionary<String, String> _IdxDropList;

        public AlterType_DropIndices(Dictionary<String, String> myIndices)
        {
            _IdxDropList = myIndices;
        }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override TypesOfAlterCmd AlterType
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Execute the removal of attribute indices
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>        
        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {

            var retExceptional = new Exceptional();

            foreach (var index in _IdxDropList)
            {
                var dropIdxExcept = myGraphDBType.RemoveIndex(index.Key, index.Value, myDBContext);

                if (!dropIdxExcept.Success())
                {
                    retExceptional.PushIExceptional(dropIdxExcept);
                }
            }

            return retExceptional;

        }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>        
        public override IEnumerable<Vertex> CreateVertex(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return base.CreateVertex(myDBContext, myGraphDBType);
        }

    }

}
