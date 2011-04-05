namespace sones.GraphDB.Expression
{
    /// <summary>
    /// The enum for all binary operators like =, >= or AND
    /// </summary>
    public enum BinaryOperator
    {
        #region Comparative
        /// <summary>
        /// Comparative operators compare the left and right side of a binary expression
        /// </summary>

        Equals,
        GreaterOrEqualsThan,
        GreaterThan,
        In,
        InRange,
        LessOrEqualsThan,
        LessThan,
        NotEquals,
        NotIn,

        #endregion

        #region Logic
        /// <summary>
        /// Logic operators process the result of two expressions
        /// </summary>

        AND,
        OR

        #endregion
    }
}