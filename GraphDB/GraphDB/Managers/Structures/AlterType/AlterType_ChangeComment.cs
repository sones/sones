/*
 * AlterType_ChangeComment
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
    /// Change the comment on a vertex
    /// </summary>
    public class AlterType_ChangeComment : AAlterTypeCommand
    {

        /// <summary>
        /// The new comment on a vertex
        /// </summary>
        public String NewComment { get; set; }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>
        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.ChangeComment; }
        }

        /// <summary>
        /// Execute the change of a comment change
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>        
        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return myDBContext.DBTypeManager.ChangeCommentOnType(myGraphDBType, NewComment);
        }

        /// <summary>
        /// <seealso cref=" AAlterTypeCommand"/>
        /// </summary>        
        public override IEnumerable<Vertex> CreateVertex(DBContext myDBContext, GraphDBType myGraphDBType)
        {

            var payload = new Dictionary<String, Object>();

            payload.Add("TYPE", myGraphDBType);
            payload.Add("ACTION", "CHANGE COMMENT");
            payload.Add("NEW COMMENT", NewComment);

            return new List<Vertex> { new Vertex(payload) };

        }

    }

}
