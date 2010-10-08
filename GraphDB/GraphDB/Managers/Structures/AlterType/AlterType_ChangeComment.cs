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

    public class AlterType_ChangeComment : AAlterTypeCommand
    {

        public String NewComment { get; set; }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.ChangeComment; }
        }

        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return myDBContext.DBTypeManager.ChangeCommentOnType(myGraphDBType, NewComment);
        }

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
