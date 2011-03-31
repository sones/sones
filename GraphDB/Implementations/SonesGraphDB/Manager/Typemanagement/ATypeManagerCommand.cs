namespace sones.GraphDB.Manager.TypeManagement
{

    //ASK: Do we really need another class to handle an undoable command?

    /// <summary>
    /// This is a base class for all commands, that can be send to the type manager. 
    /// </summary>
    /// All implementation should release their internal state as a fluent interface.
    /// The responsibility of this base class is to hold the validation state of the command.
    public abstract class ATypeManagerCommand
    {
        #region c'tor

        protected ATypeManagerCommand()
        {
            IsValidated = false;
        }

        #endregion

        #region Validation

        public bool IsValidated { get; private set; }

        /// <summary>
        /// Sets the IsValidated property to true.
        /// </summary>
        /// This method is called solely by the type manager, if the the command was validated and can be executed.
        internal void Validated()
        {
            IsValidated = true;
        }

        #endregion

        #region Execution

        public bool IsExecuted { get; private set; }

        /// <summary>
        /// Sets the IsValidated property to true.
        /// </summary>
        /// This method is called solely by the type manager, if the the command was validated and can be executed.
        internal void Executed()
        {
            IsExecuted = true;
        }

        #endregion

    }
}
