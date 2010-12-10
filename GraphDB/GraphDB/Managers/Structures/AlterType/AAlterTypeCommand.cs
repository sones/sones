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
    /// <summary>
    /// This is the abstract class for all alter type commands
    /// </summary>
    public abstract class AAlterTypeCommand
    {

        /// <summary>
        /// The type of the alter type command
        /// </summary>
        public abstract TypesOfAlterCmd AlterType { get; }

        /// <summary>
        /// Execute the alter command
        /// </summary>
        /// <param name="myDBContext">The db context</param>
        /// <param name="myGraphDBType">The db type that is to be altered</param>
        /// <returns>An exceptional</returns>
        public virtual Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return new Exceptional(new Error_NotImplemented(new StackTrace(true), GetType().Name));
        }

        /// <summary>
        /// Create a command specific readout. May return null if no readout applies. It does'nt create a new vertex.
        /// </summary>
        /// <param name="myDBContext">The db context</param>
        /// <param name="myGraphDBType">The db type that is to be altered</param>
        /// <returns>A readout or null</returns>
        public virtual IEnumerable<Vertex> CreateVertex(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            //throw new GraphDBException(new Errors.Error_NotImplemented(new System.Diagnostics.StackTrace(true), GetType().Name));
            return null;
        }

        /// <summary>
        /// Rename an vertex, attribute or backwardedge
        /// </summary>
        /// <param name="myAlterAction">The action what is to rename</param>
        /// <param name="myFromString">The current name</param>
        /// <param name="myToString">The new name</param>
        /// <param name="myType">The db type that is to be altered</param>
        /// <returns>An readout or null</returns>
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
