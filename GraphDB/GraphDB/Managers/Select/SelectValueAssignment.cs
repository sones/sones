/*
 * SelectValueAssignment
 * (c) Stefan Licht, 2010
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Managers.Structures;

namespace sones.GraphDB.Managers.Select
{

    /// <summary>
    /// A value assignment in select. PBI 527
    /// </summary>
    public class SelectValueAssignment
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
