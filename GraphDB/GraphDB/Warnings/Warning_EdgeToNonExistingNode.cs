using System;
using System.Collections.Generic;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;

namespace sones.GraphDB.Warnings
{
    public class Warning_EdgeToNonExistingNode : GraphDBWarning
    {
        public DBObjectStream StartingNode { get; set; }
        public IEnumerable<IError> Errors { get; set; }
        public TypeAttribute Edge { get; set; }
        public GraphDBType TypeOfDBO { get; set; }

        public Warning_EdgeToNonExistingNode(DBObjectStream myStartingNode, GraphDBType myTypeOfDBO, TypeAttribute myEdge, IEnumerable<IError> myErrors)
        {
            StartingNode = myStartingNode;
            Errors = myErrors;
            Edge = myEdge;
            TypeOfDBO = myTypeOfDBO;
        }

        public override string ToString()
        {
            String ErrorString = "";
            foreach (var aError in Errors)
            {
                ErrorString += aError.ToString() + Environment.NewLine;
            }

            return String.Format("Error while loading the edge \"{0}\" of the Node with UUID \"{1}\" of type \"{2}\". " + Environment.NewLine + "Errors:" + Environment.NewLine + "{3}", Edge.Name, StartingNode.ObjectUUID, TypeOfDBO.Name, ErrorString);
        }
    }
}
