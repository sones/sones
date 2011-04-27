using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.GQL.Structure.Nodes.Expressions
{
    public sealed class SelectValueAssignment
    {

        #region ValueAssignmentTypes

        /// <summary>
        /// The type of the value assignment.
        /// </summary>
        public enum ValueAssignmentTypes
        {
            Always,
            IfNotExists
        }

        #endregion

        #region Properties

        /// <summary>
        /// The type of the value assignment.
        /// </summary>
        public ValueAssignmentTypes ValueAssignmentType { get; private set; }

        /// <summary>
        /// The value of the assignment. Currently only ValueDefinitions are allowed but at some time this could be anything, even AExpressionDefinition
        /// </summary>
        public ATermDefinition TermDefinition { get; internal set; }

        #endregion

        #region Ctor

        public SelectValueAssignment(ValueAssignmentTypes myValueAssignmentType, ValueDefinition myValueDefinition)
        {
            ValueAssignmentType = myValueAssignmentType;
            TermDefinition = myValueDefinition;
        }

        #endregion

    }
}
