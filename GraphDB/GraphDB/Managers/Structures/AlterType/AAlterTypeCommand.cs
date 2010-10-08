/* 
 * AAlterTypeCommand
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Diagnostics;
using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.NewAPI;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures.Enums;

using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Managers.AlterType
{

    public abstract class AAlterTypeCommand
    {

        public abstract TypesOfAlterCmd AlterType { get; }

        public virtual Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return new Exceptional(new Error_NotImplemented(new StackTrace(true), GetType().Name));
        }

        /// <summary>
        /// Create a command specific readout. May return null if no readout applies.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="graphDBType"></param>
        /// <returns>A readout or null</returns>
        public virtual IEnumerable<Vertex> CreateVertex(DBContext dbContext, GraphDBType graphDBType)
        {
            //throw new GraphDBException(new Errors.Error_NotImplemented(new System.Diagnostics.StackTrace(true), GetType().Name));
            return null;
        }

        protected IEnumerable<Vertex> CreateRenameResult(String myAlterAction, String myFromString, String myToString, GraphDBType myType)
        {

            var payload = new Dictionary<String, Object>();

            payload.Add("TYPE",   myType);
            payload.Add("ACTION", myAlterAction);
            payload.Add("FROM",   myFromString);
            payload.Add("TO",     myToString);

            return new List<Vertex>(){new Vertex(payload)};

        }

    }

}
